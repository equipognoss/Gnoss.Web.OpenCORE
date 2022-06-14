/**/ 
/*jquery.ui.core.min.js*/ 
/*! jQuery UI - v1.10.4 - 2014-01-17
* http://jqueryui.com
* Copyright 2014 jQuery Foundation and other contributors; Licensed MIT */
(function(e,t){function i(t,i){var a,n,r,o=t.nodeName.toLowerCase();return"area"===o?(a=t.parentNode,n=a.name,t.href&&n&&"map"===a.nodeName.toLowerCase()?(r=e("img[usemap=#"+n+"]")[0],!!r&&s(r)):!1):(/input|select|textarea|button|object/.test(o)?!t.disabled:"a"===o?t.href||i:i)&&s(t)}function s(t){return e.expr.filters.visible(t)&&!e(t).parents().addBack().filter(function(){return"hidden"===e.css(this,"visibility")}).length}var a=0,n=/^ui-id-\d+$/;e.ui=e.ui||{},e.extend(e.ui,{version:"1.10.4",keyCode:{BACKSPACE:8,COMMA:188,DELETE:46,DOWN:40,END:35,ENTER:13,ESCAPE:27,HOME:36,LEFT:37,NUMPAD_ADD:107,NUMPAD_DECIMAL:110,NUMPAD_DIVIDE:111,NUMPAD_ENTER:108,NUMPAD_MULTIPLY:106,NUMPAD_SUBTRACT:109,PAGE_DOWN:34,PAGE_UP:33,PERIOD:190,RIGHT:39,SPACE:32,TAB:9,UP:38}}),e.fn.extend({focus:function(t){return function(i,s){return"number"==typeof i?this.each(function(){var t=this;setTimeout(function(){e(t).focus(),s&&s.call(t)},i)}):t.apply(this,arguments)}}(e.fn.focus),scrollParent:function(){var t;return t=e.ui.ie&&/(static|relative)/.test(this.css("position"))||/absolute/.test(this.css("position"))?this.parents().filter(function(){return/(relative|absolute|fixed)/.test(e.css(this,"position"))&&/(auto|scroll)/.test(e.css(this,"overflow")+e.css(this,"overflow-y")+e.css(this,"overflow-x"))}).eq(0):this.parents().filter(function(){return/(auto|scroll)/.test(e.css(this,"overflow")+e.css(this,"overflow-y")+e.css(this,"overflow-x"))}).eq(0),/fixed/.test(this.css("position"))||!t.length?e(document):t},zIndex:function(i){if(i!==t)return this.css("zIndex",i);if(this.length)for(var s,a,n=e(this[0]);n.length&&n[0]!==document;){if(s=n.css("position"),("absolute"===s||"relative"===s||"fixed"===s)&&(a=parseInt(n.css("zIndex"),10),!isNaN(a)&&0!==a))return a;n=n.parent()}return 0},uniqueId:function(){return this.each(function(){this.id||(this.id="ui-id-"+ ++a)})},removeUniqueId:function(){return this.each(function(){n.test(this.id)&&e(this).removeAttr("id")})}}),e.extend(e.expr[":"],{data:e.expr.createPseudo?e.expr.createPseudo(function(t){return function(i){return!!e.data(i,t)}}):function(t,i,s){return!!e.data(t,s[3])},focusable:function(t){return i(t,!isNaN(e.attr(t,"tabindex")))},tabbable:function(t){var s=e.attr(t,"tabindex"),a=isNaN(s);return(a||s>=0)&&i(t,!a)}}),e("<a>").outerWidth(1).jquery||e.each(["Width","Height"],function(i,s){function a(t,i,s,a){return e.each(n,function(){i-=parseFloat(e.css(t,"padding"+this))||0,s&&(i-=parseFloat(e.css(t,"border"+this+"Width"))||0),a&&(i-=parseFloat(e.css(t,"margin"+this))||0)}),i}var n="Width"===s?["Left","Right"]:["Top","Bottom"],r=s.toLowerCase(),o={innerWidth:e.fn.innerWidth,innerHeight:e.fn.innerHeight,outerWidth:e.fn.outerWidth,outerHeight:e.fn.outerHeight};e.fn["inner"+s]=function(i){return i===t?o["inner"+s].call(this):this.each(function(){e(this).css(r,a(this,i)+"px")})},e.fn["outer"+s]=function(t,i){return"number"!=typeof t?o["outer"+s].call(this,t):this.each(function(){e(this).css(r,a(this,t,!0,i)+"px")})}}),e.fn.addBack||(e.fn.addBack=function(e){return this.add(null==e?this.prevObject:this.prevObject.filter(e))}),e("<a>").data("a-b","a").removeData("a-b").data("a-b")&&(e.fn.removeData=function(t){return function(i){return arguments.length?t.call(this,e.camelCase(i)):t.call(this)}}(e.fn.removeData)),e.ui.ie=!!/msie [\w.]+/.exec(navigator.userAgent.toLowerCase()),e.support.selectstart="onselectstart"in document.createElement("div"),e.fn.extend({disableSelection:function(){return this.bind((e.support.selectstart?"selectstart":"mousedown")+".ui-disableSelection",function(e){e.preventDefault()})},enableSelection:function(){return this.unbind(".ui-disableSelection")}}),e.extend(e.ui,{plugin:{add:function(t,i,s){var a,n=e.ui[t].prototype;for(a in s)n.plugins[a]=n.plugins[a]||[],n.plugins[a].push([i,s[a]])},call:function(e,t,i){var s,a=e.plugins[t];if(a&&e.element[0].parentNode&&11!==e.element[0].parentNode.nodeType)for(s=0;a.length>s;s++)e.options[a[s][0]]&&a[s][1].apply(e.element,i)}},hasScroll:function(t,i){if("hidden"===e(t).css("overflow"))return!1;var s=i&&"left"===i?"scrollLeft":"scrollTop",a=!1;return t[s]>0?!0:(t[s]=1,a=t[s]>0,t[s]=0,a)}})})(jQuery);/**/ 
/*jquery.ui.datepicker.min.js*/ 
/*! jQuery UI - v1.10.4 - 2014-01-17
* http://jqueryui.com
* Copyright 2014 jQuery Foundation and other contributors; Licensed MIT */
(function(t,e){function i(){this._curInst=null,this._keyEvent=!1,this._disabledInputs=[],this._datepickerShowing=!1,this._inDialog=!1,this._mainDivId="ui-datepicker-div",this._inlineClass="ui-datepicker-inline",this._appendClass="ui-datepicker-append",this._triggerClass="ui-datepicker-trigger",this._dialogClass="ui-datepicker-dialog",this._disableClass="ui-datepicker-disabled",this._unselectableClass="ui-datepicker-unselectable",this._currentClass="ui-datepicker-current-day",this._dayOverClass="ui-datepicker-days-cell-over",this.regional=[],this.regional[""]={closeText:"Done",prevText:"Prev",nextText:"Next",currentText:"Today",monthNames:["January","February","March","April","May","June","July","August","September","October","November","December"],monthNamesShort:["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"],dayNames:["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"],dayNamesShort:["Sun","Mon","Tue","Wed","Thu","Fri","Sat"],dayNamesMin:["Su","Mo","Tu","We","Th","Fr","Sa"],weekHeader:"Wk",dateFormat:"mm/dd/yy",firstDay:0,isRTL:!1,showMonthAfterYear:!1,yearSuffix:""},this._defaults={showOn:"focus",showAnim:"fadeIn",showOptions:{},defaultDate:null,appendText:"",buttonText:"...",buttonImage:"",buttonImageOnly:!1,hideIfNoPrevNext:!1,navigationAsDateFormat:!1,gotoCurrent:!1,changeMonth:!1,changeYear:!1,yearRange:"c-10:c+10",showOtherMonths:!1,selectOtherMonths:!1,showWeek:!1,calculateWeek:this.iso8601Week,shortYearCutoff:"+10",minDate:null,maxDate:null,duration:"fast",beforeShowDay:null,beforeShow:null,onSelect:null,onChangeMonthYear:null,onClose:null,numberOfMonths:1,showCurrentAtPos:0,stepMonths:1,stepBigMonths:12,altField:"",altFormat:"",constrainInput:!0,showButtonPanel:!1,autoSize:!1,disabled:!1},t.extend(this._defaults,this.regional[""]),this.dpDiv=s(t("<div id='"+this._mainDivId+"' class='ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all'></div>"))}function s(e){var i="button, .ui-datepicker-prev, .ui-datepicker-next, .ui-datepicker-calendar td a";return e.delegate(i,"mouseout",function(){t(this).removeClass("ui-state-hover"),-1!==this.className.indexOf("ui-datepicker-prev")&&t(this).removeClass("ui-datepicker-prev-hover"),-1!==this.className.indexOf("ui-datepicker-next")&&t(this).removeClass("ui-datepicker-next-hover")}).delegate(i,"mouseover",function(){t.datepicker._isDisabledDatepicker(a.inline?e.parent()[0]:a.input[0])||(t(this).parents(".ui-datepicker-calendar").find("a").removeClass("ui-state-hover"),t(this).addClass("ui-state-hover"),-1!==this.className.indexOf("ui-datepicker-prev")&&t(this).addClass("ui-datepicker-prev-hover"),-1!==this.className.indexOf("ui-datepicker-next")&&t(this).addClass("ui-datepicker-next-hover"))})}function n(e,i){t.extend(e,i);for(var s in i)null==i[s]&&(e[s]=i[s]);return e}t.extend(t.ui,{datepicker:{version:"1.10.4"}});var a,r="datepicker";t.extend(i.prototype,{markerClassName:"hasDatepicker",maxRows:4,_widgetDatepicker:function(){return this.dpDiv},setDefaults:function(t){return n(this._defaults,t||{}),this},_attachDatepicker:function(e,i){var s,n,a;s=e.nodeName.toLowerCase(),n="div"===s||"span"===s,e.id||(this.uuid+=1,e.id="dp"+this.uuid),a=this._newInst(t(e),n),a.settings=t.extend({},i||{}),"input"===s?this._connectDatepicker(e,a):n&&this._inlineDatepicker(e,a)},_newInst:function(e,i){var n=e[0].id.replace(/([^A-Za-z0-9_\-])/g,"\\\\$1");return{id:n,input:e,selectedDay:0,selectedMonth:0,selectedYear:0,drawMonth:0,drawYear:0,inline:i,dpDiv:i?s(t("<div class='"+this._inlineClass+" ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all'></div>")):this.dpDiv}},_connectDatepicker:function(e,i){var s=t(e);i.append=t([]),i.trigger=t([]),s.hasClass(this.markerClassName)||(this._attachments(s,i),s.addClass(this.markerClassName).keydown(this._doKeyDown).keypress(this._doKeyPress).keyup(this._doKeyUp),this._autoSize(i),t.data(e,r,i),i.settings.disabled&&this._disableDatepicker(e))},_attachments:function(e,i){var s,n,a,r=this._get(i,"appendText"),o=this._get(i,"isRTL");i.append&&i.append.remove(),r&&(i.append=t("<span class='"+this._appendClass+"'>"+r+"</span>"),e[o?"before":"after"](i.append)),e.unbind("focus",this._showDatepicker),i.trigger&&i.trigger.remove(),s=this._get(i,"showOn"),("focus"===s||"both"===s)&&e.focus(this._showDatepicker),("button"===s||"both"===s)&&(n=this._get(i,"buttonText"),a=this._get(i,"buttonImage"),i.trigger=t(this._get(i,"buttonImageOnly")?t("<img/>").addClass(this._triggerClass).attr({src:a,alt:n,title:n}):t("<button type='button'></button>").addClass(this._triggerClass).html(a?t("<img/>").attr({src:a,alt:n,title:n}):n)),e[o?"before":"after"](i.trigger),i.trigger.click(function(){return t.datepicker._datepickerShowing&&t.datepicker._lastInput===e[0]?t.datepicker._hideDatepicker():t.datepicker._datepickerShowing&&t.datepicker._lastInput!==e[0]?(t.datepicker._hideDatepicker(),t.datepicker._showDatepicker(e[0])):t.datepicker._showDatepicker(e[0]),!1}))},_autoSize:function(t){if(this._get(t,"autoSize")&&!t.inline){var e,i,s,n,a=new Date(2009,11,20),r=this._get(t,"dateFormat");r.match(/[DM]/)&&(e=function(t){for(i=0,s=0,n=0;t.length>n;n++)t[n].length>i&&(i=t[n].length,s=n);return s},a.setMonth(e(this._get(t,r.match(/MM/)?"monthNames":"monthNamesShort"))),a.setDate(e(this._get(t,r.match(/DD/)?"dayNames":"dayNamesShort"))+20-a.getDay())),t.input.attr("size",this._formatDate(t,a).length)}},_inlineDatepicker:function(e,i){var s=t(e);s.hasClass(this.markerClassName)||(s.addClass(this.markerClassName).append(i.dpDiv),t.data(e,r,i),this._setDate(i,this._getDefaultDate(i),!0),this._updateDatepicker(i),this._updateAlternate(i),i.settings.disabled&&this._disableDatepicker(e),i.dpDiv.css("display","block"))},_dialogDatepicker:function(e,i,s,a,o){var h,l,c,u,d,p=this._dialogInst;return p||(this.uuid+=1,h="dp"+this.uuid,this._dialogInput=t("<input type='text' id='"+h+"' style='position: absolute; top: -100px; width: 0px;'/>"),this._dialogInput.keydown(this._doKeyDown),t("body").append(this._dialogInput),p=this._dialogInst=this._newInst(this._dialogInput,!1),p.settings={},t.data(this._dialogInput[0],r,p)),n(p.settings,a||{}),i=i&&i.constructor===Date?this._formatDate(p,i):i,this._dialogInput.val(i),this._pos=o?o.length?o:[o.pageX,o.pageY]:null,this._pos||(l=document.documentElement.clientWidth,c=document.documentElement.clientHeight,u=document.documentElement.scrollLeft||document.body.scrollLeft,d=document.documentElement.scrollTop||document.body.scrollTop,this._pos=[l/2-100+u,c/2-150+d]),this._dialogInput.css("left",this._pos[0]+20+"px").css("top",this._pos[1]+"px"),p.settings.onSelect=s,this._inDialog=!0,this.dpDiv.addClass(this._dialogClass),this._showDatepicker(this._dialogInput[0]),t.blockUI&&t.blockUI(this.dpDiv),t.data(this._dialogInput[0],r,p),this},_destroyDatepicker:function(e){var i,s=t(e),n=t.data(e,r);s.hasClass(this.markerClassName)&&(i=e.nodeName.toLowerCase(),t.removeData(e,r),"input"===i?(n.append.remove(),n.trigger.remove(),s.removeClass(this.markerClassName).unbind("focus",this._showDatepicker).unbind("keydown",this._doKeyDown).unbind("keypress",this._doKeyPress).unbind("keyup",this._doKeyUp)):("div"===i||"span"===i)&&s.removeClass(this.markerClassName).empty())},_enableDatepicker:function(e){var i,s,n=t(e),a=t.data(e,r);n.hasClass(this.markerClassName)&&(i=e.nodeName.toLowerCase(),"input"===i?(e.disabled=!1,a.trigger.filter("button").each(function(){this.disabled=!1}).end().filter("img").css({opacity:"1.0",cursor:""})):("div"===i||"span"===i)&&(s=n.children("."+this._inlineClass),s.children().removeClass("ui-state-disabled"),s.find("select.ui-datepicker-month, select.ui-datepicker-year").prop("disabled",!1)),this._disabledInputs=t.map(this._disabledInputs,function(t){return t===e?null:t}))},_disableDatepicker:function(e){var i,s,n=t(e),a=t.data(e,r);n.hasClass(this.markerClassName)&&(i=e.nodeName.toLowerCase(),"input"===i?(e.disabled=!0,a.trigger.filter("button").each(function(){this.disabled=!0}).end().filter("img").css({opacity:"0.5",cursor:"default"})):("div"===i||"span"===i)&&(s=n.children("."+this._inlineClass),s.children().addClass("ui-state-disabled"),s.find("select.ui-datepicker-month, select.ui-datepicker-year").prop("disabled",!0)),this._disabledInputs=t.map(this._disabledInputs,function(t){return t===e?null:t}),this._disabledInputs[this._disabledInputs.length]=e)},_isDisabledDatepicker:function(t){if(!t)return!1;for(var e=0;this._disabledInputs.length>e;e++)if(this._disabledInputs[e]===t)return!0;return!1},_getInst:function(e){try{return t.data(e,r)}catch(i){throw"Missing instance data for this datepicker"}},_optionDatepicker:function(i,s,a){var r,o,h,l,c=this._getInst(i);return 2===arguments.length&&"string"==typeof s?"defaults"===s?t.extend({},t.datepicker._defaults):c?"all"===s?t.extend({},c.settings):this._get(c,s):null:(r=s||{},"string"==typeof s&&(r={},r[s]=a),c&&(this._curInst===c&&this._hideDatepicker(),o=this._getDateDatepicker(i,!0),h=this._getMinMaxDate(c,"min"),l=this._getMinMaxDate(c,"max"),n(c.settings,r),null!==h&&r.dateFormat!==e&&r.minDate===e&&(c.settings.minDate=this._formatDate(c,h)),null!==l&&r.dateFormat!==e&&r.maxDate===e&&(c.settings.maxDate=this._formatDate(c,l)),"disabled"in r&&(r.disabled?this._disableDatepicker(i):this._enableDatepicker(i)),this._attachments(t(i),c),this._autoSize(c),this._setDate(c,o),this._updateAlternate(c),this._updateDatepicker(c)),e)},_changeDatepicker:function(t,e,i){this._optionDatepicker(t,e,i)},_refreshDatepicker:function(t){var e=this._getInst(t);e&&this._updateDatepicker(e)},_setDateDatepicker:function(t,e){var i=this._getInst(t);i&&(this._setDate(i,e),this._updateDatepicker(i),this._updateAlternate(i))},_getDateDatepicker:function(t,e){var i=this._getInst(t);return i&&!i.inline&&this._setDateFromField(i,e),i?this._getDate(i):null},_doKeyDown:function(e){var i,s,n,a=t.datepicker._getInst(e.target),r=!0,o=a.dpDiv.is(".ui-datepicker-rtl");if(a._keyEvent=!0,t.datepicker._datepickerShowing)switch(e.keyCode){case 9:t.datepicker._hideDatepicker(),r=!1;break;case 13:return n=t("td."+t.datepicker._dayOverClass+":not(."+t.datepicker._currentClass+")",a.dpDiv),n[0]&&t.datepicker._selectDay(e.target,a.selectedMonth,a.selectedYear,n[0]),i=t.datepicker._get(a,"onSelect"),i?(s=t.datepicker._formatDate(a),i.apply(a.input?a.input[0]:null,[s,a])):t.datepicker._hideDatepicker(),!1;case 27:t.datepicker._hideDatepicker();break;case 33:t.datepicker._adjustDate(e.target,e.ctrlKey?-t.datepicker._get(a,"stepBigMonths"):-t.datepicker._get(a,"stepMonths"),"M");break;case 34:t.datepicker._adjustDate(e.target,e.ctrlKey?+t.datepicker._get(a,"stepBigMonths"):+t.datepicker._get(a,"stepMonths"),"M");break;case 35:(e.ctrlKey||e.metaKey)&&t.datepicker._clearDate(e.target),r=e.ctrlKey||e.metaKey;break;case 36:(e.ctrlKey||e.metaKey)&&t.datepicker._gotoToday(e.target),r=e.ctrlKey||e.metaKey;break;case 37:(e.ctrlKey||e.metaKey)&&t.datepicker._adjustDate(e.target,o?1:-1,"D"),r=e.ctrlKey||e.metaKey,e.originalEvent.altKey&&t.datepicker._adjustDate(e.target,e.ctrlKey?-t.datepicker._get(a,"stepBigMonths"):-t.datepicker._get(a,"stepMonths"),"M");break;case 38:(e.ctrlKey||e.metaKey)&&t.datepicker._adjustDate(e.target,-7,"D"),r=e.ctrlKey||e.metaKey;break;case 39:(e.ctrlKey||e.metaKey)&&t.datepicker._adjustDate(e.target,o?-1:1,"D"),r=e.ctrlKey||e.metaKey,e.originalEvent.altKey&&t.datepicker._adjustDate(e.target,e.ctrlKey?+t.datepicker._get(a,"stepBigMonths"):+t.datepicker._get(a,"stepMonths"),"M");break;case 40:(e.ctrlKey||e.metaKey)&&t.datepicker._adjustDate(e.target,7,"D"),r=e.ctrlKey||e.metaKey;break;default:r=!1}else 36===e.keyCode&&e.ctrlKey?t.datepicker._showDatepicker(this):r=!1;r&&(e.preventDefault(),e.stopPropagation())},_doKeyPress:function(i){var s,n,a=t.datepicker._getInst(i.target);return t.datepicker._get(a,"constrainInput")?(s=t.datepicker._possibleChars(t.datepicker._get(a,"dateFormat")),n=String.fromCharCode(null==i.charCode?i.keyCode:i.charCode),i.ctrlKey||i.metaKey||" ">n||!s||s.indexOf(n)>-1):e},_doKeyUp:function(e){var i,s=t.datepicker._getInst(e.target);if(s.input.val()!==s.lastVal)try{i=t.datepicker.parseDate(t.datepicker._get(s,"dateFormat"),s.input?s.input.val():null,t.datepicker._getFormatConfig(s)),i&&(t.datepicker._setDateFromField(s),t.datepicker._updateAlternate(s),t.datepicker._updateDatepicker(s))}catch(n){}return!0},_showDatepicker:function(e){if(e=e.target||e,"input"!==e.nodeName.toLowerCase()&&(e=t("input",e.parentNode)[0]),!t.datepicker._isDisabledDatepicker(e)&&t.datepicker._lastInput!==e){var i,s,a,r,o,h,l;i=t.datepicker._getInst(e),t.datepicker._curInst&&t.datepicker._curInst!==i&&(t.datepicker._curInst.dpDiv.stop(!0,!0),i&&t.datepicker._datepickerShowing&&t.datepicker._hideDatepicker(t.datepicker._curInst.input[0])),s=t.datepicker._get(i,"beforeShow"),a=s?s.apply(e,[e,i]):{},a!==!1&&(n(i.settings,a),i.lastVal=null,t.datepicker._lastInput=e,t.datepicker._setDateFromField(i),t.datepicker._inDialog&&(e.value=""),t.datepicker._pos||(t.datepicker._pos=t.datepicker._findPos(e),t.datepicker._pos[1]+=e.offsetHeight),r=!1,t(e).parents().each(function(){return r|="fixed"===t(this).css("position"),!r}),o={left:t.datepicker._pos[0],top:t.datepicker._pos[1]},t.datepicker._pos=null,i.dpDiv.empty(),i.dpDiv.css({position:"absolute",display:"block",top:"-1000px"}),t.datepicker._updateDatepicker(i),o=t.datepicker._checkOffset(i,o,r),i.dpDiv.css({position:t.datepicker._inDialog&&t.blockUI?"static":r?"fixed":"absolute",display:"none",left:o.left+"px",top:o.top+"px"}),i.inline||(h=t.datepicker._get(i,"showAnim"),l=t.datepicker._get(i,"duration"),i.dpDiv.zIndex(t(e).zIndex()+1),t.datepicker._datepickerShowing=!0,t.effects&&t.effects.effect[h]?i.dpDiv.show(h,t.datepicker._get(i,"showOptions"),l):i.dpDiv[h||"show"](h?l:null),t.datepicker._shouldFocusInput(i)&&i.input.focus(),t.datepicker._curInst=i))}},_updateDatepicker:function(e){this.maxRows=4,a=e,e.dpDiv.empty().append(this._generateHTML(e)),this._attachHandlers(e),e.dpDiv.find("."+this._dayOverClass+" a").mouseover();var i,s=this._getNumberOfMonths(e),n=s[1],r=17;e.dpDiv.removeClass("ui-datepicker-multi-2 ui-datepicker-multi-3 ui-datepicker-multi-4").width(""),n>1&&e.dpDiv.addClass("ui-datepicker-multi-"+n).css("width",r*n+"em"),e.dpDiv[(1!==s[0]||1!==s[1]?"add":"remove")+"Class"]("ui-datepicker-multi"),e.dpDiv[(this._get(e,"isRTL")?"add":"remove")+"Class"]("ui-datepicker-rtl"),e===t.datepicker._curInst&&t.datepicker._datepickerShowing&&t.datepicker._shouldFocusInput(e)&&e.input.focus(),e.yearshtml&&(i=e.yearshtml,setTimeout(function(){i===e.yearshtml&&e.yearshtml&&e.dpDiv.find("select.ui-datepicker-year:first").replaceWith(e.yearshtml),i=e.yearshtml=null},0))},_shouldFocusInput:function(t){return t.input&&t.input.is(":visible")&&!t.input.is(":disabled")&&!t.input.is(":focus")},_checkOffset:function(e,i,s){var n=e.dpDiv.outerWidth(),a=e.dpDiv.outerHeight(),r=e.input?e.input.outerWidth():0,o=e.input?e.input.outerHeight():0,h=document.documentElement.clientWidth+(s?0:t(document).scrollLeft()),l=document.documentElement.clientHeight+(s?0:t(document).scrollTop());return i.left-=this._get(e,"isRTL")?n-r:0,i.left-=s&&i.left===e.input.offset().left?t(document).scrollLeft():0,i.top-=s&&i.top===e.input.offset().top+o?t(document).scrollTop():0,i.left-=Math.min(i.left,i.left+n>h&&h>n?Math.abs(i.left+n-h):0),i.top-=Math.min(i.top,i.top+a>l&&l>a?Math.abs(a+o):0),i},_findPos:function(e){for(var i,s=this._getInst(e),n=this._get(s,"isRTL");e&&("hidden"===e.type||1!==e.nodeType||t.expr.filters.hidden(e));)e=e[n?"previousSibling":"nextSibling"];return i=t(e).offset(),[i.left,i.top]},_hideDatepicker:function(e){var i,s,n,a,o=this._curInst;!o||e&&o!==t.data(e,r)||this._datepickerShowing&&(i=this._get(o,"showAnim"),s=this._get(o,"duration"),n=function(){t.datepicker._tidyDialog(o)},t.effects&&(t.effects.effect[i]||t.effects[i])?o.dpDiv.hide(i,t.datepicker._get(o,"showOptions"),s,n):o.dpDiv["slideDown"===i?"slideUp":"fadeIn"===i?"fadeOut":"hide"](i?s:null,n),i||n(),this._datepickerShowing=!1,a=this._get(o,"onClose"),a&&a.apply(o.input?o.input[0]:null,[o.input?o.input.val():"",o]),this._lastInput=null,this._inDialog&&(this._dialogInput.css({position:"absolute",left:"0",top:"-100px"}),t.blockUI&&(t.unblockUI(),t("body").append(this.dpDiv))),this._inDialog=!1)},_tidyDialog:function(t){t.dpDiv.removeClass(this._dialogClass).unbind(".ui-datepicker-calendar")},_checkExternalClick:function(e){if(t.datepicker._curInst){var i=t(e.target),s=t.datepicker._getInst(i[0]);(i[0].id!==t.datepicker._mainDivId&&0===i.parents("#"+t.datepicker._mainDivId).length&&!i.hasClass(t.datepicker.markerClassName)&&!i.closest("."+t.datepicker._triggerClass).length&&t.datepicker._datepickerShowing&&(!t.datepicker._inDialog||!t.blockUI)||i.hasClass(t.datepicker.markerClassName)&&t.datepicker._curInst!==s)&&t.datepicker._hideDatepicker()}},_adjustDate:function(e,i,s){var n=t(e),a=this._getInst(n[0]);this._isDisabledDatepicker(n[0])||(this._adjustInstDate(a,i+("M"===s?this._get(a,"showCurrentAtPos"):0),s),this._updateDatepicker(a))},_gotoToday:function(e){var i,s=t(e),n=this._getInst(s[0]);this._get(n,"gotoCurrent")&&n.currentDay?(n.selectedDay=n.currentDay,n.drawMonth=n.selectedMonth=n.currentMonth,n.drawYear=n.selectedYear=n.currentYear):(i=new Date,n.selectedDay=i.getDate(),n.drawMonth=n.selectedMonth=i.getMonth(),n.drawYear=n.selectedYear=i.getFullYear()),this._notifyChange(n),this._adjustDate(s)},_selectMonthYear:function(e,i,s){var n=t(e),a=this._getInst(n[0]);a["selected"+("M"===s?"Month":"Year")]=a["draw"+("M"===s?"Month":"Year")]=parseInt(i.options[i.selectedIndex].value,10),this._notifyChange(a),this._adjustDate(n)},_selectDay:function(e,i,s,n){var a,r=t(e);t(n).hasClass(this._unselectableClass)||this._isDisabledDatepicker(r[0])||(a=this._getInst(r[0]),a.selectedDay=a.currentDay=t("a",n).html(),a.selectedMonth=a.currentMonth=i,a.selectedYear=a.currentYear=s,this._selectDate(e,this._formatDate(a,a.currentDay,a.currentMonth,a.currentYear)))},_clearDate:function(e){var i=t(e);this._selectDate(i,"")},_selectDate:function(e,i){var s,n=t(e),a=this._getInst(n[0]);i=null!=i?i:this._formatDate(a),a.input&&a.input.val(i),this._updateAlternate(a),s=this._get(a,"onSelect"),s?s.apply(a.input?a.input[0]:null,[i,a]):a.input&&a.input.trigger("change"),a.inline?this._updateDatepicker(a):(this._hideDatepicker(),this._lastInput=a.input[0],"object"!=typeof a.input[0]&&a.input.focus(),this._lastInput=null)},_updateAlternate:function(e){var i,s,n,a=this._get(e,"altField");a&&(i=this._get(e,"altFormat")||this._get(e,"dateFormat"),s=this._getDate(e),n=this.formatDate(i,s,this._getFormatConfig(e)),t(a).each(function(){t(this).val(n)}))},noWeekends:function(t){var e=t.getDay();return[e>0&&6>e,""]},iso8601Week:function(t){var e,i=new Date(t.getTime());return i.setDate(i.getDate()+4-(i.getDay()||7)),e=i.getTime(),i.setMonth(0),i.setDate(1),Math.floor(Math.round((e-i)/864e5)/7)+1},parseDate:function(i,s,n){if(null==i||null==s)throw"Invalid arguments";if(s="object"==typeof s?""+s:s+"",""===s)return null;var a,r,o,h,l=0,c=(n?n.shortYearCutoff:null)||this._defaults.shortYearCutoff,u="string"!=typeof c?c:(new Date).getFullYear()%100+parseInt(c,10),d=(n?n.dayNamesShort:null)||this._defaults.dayNamesShort,p=(n?n.dayNames:null)||this._defaults.dayNames,f=(n?n.monthNamesShort:null)||this._defaults.monthNamesShort,m=(n?n.monthNames:null)||this._defaults.monthNames,g=-1,v=-1,_=-1,b=-1,y=!1,x=function(t){var e=i.length>a+1&&i.charAt(a+1)===t;return e&&a++,e},k=function(t){var e=x(t),i="@"===t?14:"!"===t?20:"y"===t&&e?4:"o"===t?3:2,n=RegExp("^\\d{1,"+i+"}"),a=s.substring(l).match(n);if(!a)throw"Missing number at position "+l;return l+=a[0].length,parseInt(a[0],10)},w=function(i,n,a){var r=-1,o=t.map(x(i)?a:n,function(t,e){return[[e,t]]}).sort(function(t,e){return-(t[1].length-e[1].length)});if(t.each(o,function(t,i){var n=i[1];return s.substr(l,n.length).toLowerCase()===n.toLowerCase()?(r=i[0],l+=n.length,!1):e}),-1!==r)return r+1;throw"Unknown name at position "+l},D=function(){if(s.charAt(l)!==i.charAt(a))throw"Unexpected literal at position "+l;l++};for(a=0;i.length>a;a++)if(y)"'"!==i.charAt(a)||x("'")?D():y=!1;else switch(i.charAt(a)){case"d":_=k("d");break;case"D":w("D",d,p);break;case"o":b=k("o");break;case"m":v=k("m");break;case"M":v=w("M",f,m);break;case"y":g=k("y");break;case"@":h=new Date(k("@")),g=h.getFullYear(),v=h.getMonth()+1,_=h.getDate();break;case"!":h=new Date((k("!")-this._ticksTo1970)/1e4),g=h.getFullYear(),v=h.getMonth()+1,_=h.getDate();break;case"'":x("'")?D():y=!0;break;default:D()}if(s.length>l&&(o=s.substr(l),!/^\s+/.test(o)))throw"Extra/unparsed characters found in date: "+o;if(-1===g?g=(new Date).getFullYear():100>g&&(g+=(new Date).getFullYear()-(new Date).getFullYear()%100+(u>=g?0:-100)),b>-1)for(v=1,_=b;;){if(r=this._getDaysInMonth(g,v-1),r>=_)break;v++,_-=r}if(h=this._daylightSavingAdjust(new Date(g,v-1,_)),h.getFullYear()!==g||h.getMonth()+1!==v||h.getDate()!==_)throw"Invalid date";return h},ATOM:"yy-mm-dd",COOKIE:"D, dd M yy",ISO_8601:"yy-mm-dd",RFC_822:"D, d M y",RFC_850:"DD, dd-M-y",RFC_1036:"D, d M y",RFC_1123:"D, d M yy",RFC_2822:"D, d M yy",RSS:"D, d M y",TICKS:"!",TIMESTAMP:"@",W3C:"yy-mm-dd",_ticksTo1970:1e7*60*60*24*(718685+Math.floor(492.5)-Math.floor(19.7)+Math.floor(4.925)),formatDate:function(t,e,i){if(!e)return"";var s,n=(i?i.dayNamesShort:null)||this._defaults.dayNamesShort,a=(i?i.dayNames:null)||this._defaults.dayNames,r=(i?i.monthNamesShort:null)||this._defaults.monthNamesShort,o=(i?i.monthNames:null)||this._defaults.monthNames,h=function(e){var i=t.length>s+1&&t.charAt(s+1)===e;return i&&s++,i},l=function(t,e,i){var s=""+e;if(h(t))for(;i>s.length;)s="0"+s;return s},c=function(t,e,i,s){return h(t)?s[e]:i[e]},u="",d=!1;if(e)for(s=0;t.length>s;s++)if(d)"'"!==t.charAt(s)||h("'")?u+=t.charAt(s):d=!1;else switch(t.charAt(s)){case"d":u+=l("d",e.getDate(),2);break;case"D":u+=c("D",e.getDay(),n,a);break;case"o":u+=l("o",Math.round((new Date(e.getFullYear(),e.getMonth(),e.getDate()).getTime()-new Date(e.getFullYear(),0,0).getTime())/864e5),3);break;case"m":u+=l("m",e.getMonth()+1,2);break;case"M":u+=c("M",e.getMonth(),r,o);break;case"y":u+=h("y")?e.getFullYear():(10>e.getYear()%100?"0":"")+e.getYear()%100;break;case"@":u+=e.getTime();break;case"!":u+=1e4*e.getTime()+this._ticksTo1970;break;case"'":h("'")?u+="'":d=!0;break;default:u+=t.charAt(s)}return u},_possibleChars:function(t){var e,i="",s=!1,n=function(i){var s=t.length>e+1&&t.charAt(e+1)===i;return s&&e++,s};for(e=0;t.length>e;e++)if(s)"'"!==t.charAt(e)||n("'")?i+=t.charAt(e):s=!1;else switch(t.charAt(e)){case"d":case"m":case"y":case"@":i+="0123456789";break;case"D":case"M":return null;case"'":n("'")?i+="'":s=!0;break;default:i+=t.charAt(e)}return i},_get:function(t,i){return t.settings[i]!==e?t.settings[i]:this._defaults[i]},_setDateFromField:function(t,e){if(t.input.val()!==t.lastVal){var i=this._get(t,"dateFormat"),s=t.lastVal=t.input?t.input.val():null,n=this._getDefaultDate(t),a=n,r=this._getFormatConfig(t);try{a=this.parseDate(i,s,r)||n}catch(o){s=e?"":s}t.selectedDay=a.getDate(),t.drawMonth=t.selectedMonth=a.getMonth(),t.drawYear=t.selectedYear=a.getFullYear(),t.currentDay=s?a.getDate():0,t.currentMonth=s?a.getMonth():0,t.currentYear=s?a.getFullYear():0,this._adjustInstDate(t)}},_getDefaultDate:function(t){return this._restrictMinMax(t,this._determineDate(t,this._get(t,"defaultDate"),new Date))},_determineDate:function(e,i,s){var n=function(t){var e=new Date;return e.setDate(e.getDate()+t),e},a=function(i){try{return t.datepicker.parseDate(t.datepicker._get(e,"dateFormat"),i,t.datepicker._getFormatConfig(e))}catch(s){}for(var n=(i.toLowerCase().match(/^c/)?t.datepicker._getDate(e):null)||new Date,a=n.getFullYear(),r=n.getMonth(),o=n.getDate(),h=/([+\-]?[0-9]+)\s*(d|D|w|W|m|M|y|Y)?/g,l=h.exec(i);l;){switch(l[2]||"d"){case"d":case"D":o+=parseInt(l[1],10);break;case"w":case"W":o+=7*parseInt(l[1],10);break;case"m":case"M":r+=parseInt(l[1],10),o=Math.min(o,t.datepicker._getDaysInMonth(a,r));break;case"y":case"Y":a+=parseInt(l[1],10),o=Math.min(o,t.datepicker._getDaysInMonth(a,r))}l=h.exec(i)}return new Date(a,r,o)},r=null==i||""===i?s:"string"==typeof i?a(i):"number"==typeof i?isNaN(i)?s:n(i):new Date(i.getTime());return r=r&&"Invalid Date"==""+r?s:r,r&&(r.setHours(0),r.setMinutes(0),r.setSeconds(0),r.setMilliseconds(0)),this._daylightSavingAdjust(r)},_daylightSavingAdjust:function(t){return t?(t.setHours(t.getHours()>12?t.getHours()+2:0),t):null},_setDate:function(t,e,i){var s=!e,n=t.selectedMonth,a=t.selectedYear,r=this._restrictMinMax(t,this._determineDate(t,e,new Date));t.selectedDay=t.currentDay=r.getDate(),t.drawMonth=t.selectedMonth=t.currentMonth=r.getMonth(),t.drawYear=t.selectedYear=t.currentYear=r.getFullYear(),n===t.selectedMonth&&a===t.selectedYear||i||this._notifyChange(t),this._adjustInstDate(t),t.input&&t.input.val(s?"":this._formatDate(t))},_getDate:function(t){var e=!t.currentYear||t.input&&""===t.input.val()?null:this._daylightSavingAdjust(new Date(t.currentYear,t.currentMonth,t.currentDay));return e},_attachHandlers:function(e){var i=this._get(e,"stepMonths"),s="#"+e.id.replace(/\\\\/g,"\\");e.dpDiv.find("[data-handler]").map(function(){var e={prev:function(){t.datepicker._adjustDate(s,-i,"M")},next:function(){t.datepicker._adjustDate(s,+i,"M")},hide:function(){t.datepicker._hideDatepicker()},today:function(){t.datepicker._gotoToday(s)},selectDay:function(){return t.datepicker._selectDay(s,+this.getAttribute("data-month"),+this.getAttribute("data-year"),this),!1},selectMonth:function(){return t.datepicker._selectMonthYear(s,this,"M"),!1},selectYear:function(){return t.datepicker._selectMonthYear(s,this,"Y"),!1}};t(this).bind(this.getAttribute("data-event"),e[this.getAttribute("data-handler")])})},_generateHTML:function(t){var e,i,s,n,a,r,o,h,l,c,u,d,p,f,m,g,v,_,b,y,x,k,w,D,T,C,S,M,N,I,P,A,z,H,E,F,O,j,W,R=new Date,L=this._daylightSavingAdjust(new Date(R.getFullYear(),R.getMonth(),R.getDate())),Y=this._get(t,"isRTL"),B=this._get(t,"showButtonPanel"),K=this._get(t,"hideIfNoPrevNext"),J=this._get(t,"navigationAsDateFormat"),Q=this._getNumberOfMonths(t),V=this._get(t,"showCurrentAtPos"),U=this._get(t,"stepMonths"),q=1!==Q[0]||1!==Q[1],X=this._daylightSavingAdjust(t.currentDay?new Date(t.currentYear,t.currentMonth,t.currentDay):new Date(9999,9,9)),G=this._getMinMaxDate(t,"min"),$=this._getMinMaxDate(t,"max"),Z=t.drawMonth-V,te=t.drawYear;if(0>Z&&(Z+=12,te--),$)for(e=this._daylightSavingAdjust(new Date($.getFullYear(),$.getMonth()-Q[0]*Q[1]+1,$.getDate())),e=G&&G>e?G:e;this._daylightSavingAdjust(new Date(te,Z,1))>e;)Z--,0>Z&&(Z=11,te--);for(t.drawMonth=Z,t.drawYear=te,i=this._get(t,"prevText"),i=J?this.formatDate(i,this._daylightSavingAdjust(new Date(te,Z-U,1)),this._getFormatConfig(t)):i,s=this._canAdjustMonth(t,-1,te,Z)?"<a class='ui-datepicker-prev ui-corner-all' data-handler='prev' data-event='click' title='"+i+"'><span class='ui-icon ui-icon-circle-triangle-"+(Y?"e":"w")+"'>"+i+"</span></a>":K?"":"<a class='ui-datepicker-prev ui-corner-all ui-state-disabled' title='"+i+"'><span class='ui-icon ui-icon-circle-triangle-"+(Y?"e":"w")+"'>"+i+"</span></a>",n=this._get(t,"nextText"),n=J?this.formatDate(n,this._daylightSavingAdjust(new Date(te,Z+U,1)),this._getFormatConfig(t)):n,a=this._canAdjustMonth(t,1,te,Z)?"<a class='ui-datepicker-next ui-corner-all' data-handler='next' data-event='click' title='"+n+"'><span class='ui-icon ui-icon-circle-triangle-"+(Y?"w":"e")+"'>"+n+"</span></a>":K?"":"<a class='ui-datepicker-next ui-corner-all ui-state-disabled' title='"+n+"'><span class='ui-icon ui-icon-circle-triangle-"+(Y?"w":"e")+"'>"+n+"</span></a>",r=this._get(t,"currentText"),o=this._get(t,"gotoCurrent")&&t.currentDay?X:L,r=J?this.formatDate(r,o,this._getFormatConfig(t)):r,h=t.inline?"":"<button type='button' class='ui-datepicker-close ui-state-default ui-priority-primary ui-corner-all' data-handler='hide' data-event='click'>"+this._get(t,"closeText")+"</button>",l=B?"<div class='ui-datepicker-buttonpane ui-widget-content'>"+(Y?h:"")+(this._isInRange(t,o)?"<button type='button' class='ui-datepicker-current ui-state-default ui-priority-secondary ui-corner-all' data-handler='today' data-event='click'>"+r+"</button>":"")+(Y?"":h)+"</div>":"",c=parseInt(this._get(t,"firstDay"),10),c=isNaN(c)?0:c,u=this._get(t,"showWeek"),d=this._get(t,"dayNames"),p=this._get(t,"dayNamesMin"),f=this._get(t,"monthNames"),m=this._get(t,"monthNamesShort"),g=this._get(t,"beforeShowDay"),v=this._get(t,"showOtherMonths"),_=this._get(t,"selectOtherMonths"),b=this._getDefaultDate(t),y="",k=0;Q[0]>k;k++){for(w="",this.maxRows=4,D=0;Q[1]>D;D++){if(T=this._daylightSavingAdjust(new Date(te,Z,t.selectedDay)),C=" ui-corner-all",S="",q){if(S+="<div class='ui-datepicker-group",Q[1]>1)switch(D){case 0:S+=" ui-datepicker-group-first",C=" ui-corner-"+(Y?"right":"left");break;case Q[1]-1:S+=" ui-datepicker-group-last",C=" ui-corner-"+(Y?"left":"right");break;default:S+=" ui-datepicker-group-middle",C=""}S+="'>"}for(S+="<div class='ui-datepicker-header ui-widget-header ui-helper-clearfix"+C+"'>"+(/all|left/.test(C)&&0===k?Y?a:s:"")+(/all|right/.test(C)&&0===k?Y?s:a:"")+this._generateMonthYearHeader(t,Z,te,G,$,k>0||D>0,f,m)+"</div><table class='ui-datepicker-calendar'><thead>"+"<tr>",M=u?"<th class='ui-datepicker-week-col'>"+this._get(t,"weekHeader")+"</th>":"",x=0;7>x;x++)N=(x+c)%7,M+="<th"+((x+c+6)%7>=5?" class='ui-datepicker-week-end'":"")+">"+"<span title='"+d[N]+"'>"+p[N]+"</span></th>";for(S+=M+"</tr></thead><tbody>",I=this._getDaysInMonth(te,Z),te===t.selectedYear&&Z===t.selectedMonth&&(t.selectedDay=Math.min(t.selectedDay,I)),P=(this._getFirstDayOfMonth(te,Z)-c+7)%7,A=Math.ceil((P+I)/7),z=q?this.maxRows>A?this.maxRows:A:A,this.maxRows=z,H=this._daylightSavingAdjust(new Date(te,Z,1-P)),E=0;z>E;E++){for(S+="<tr>",F=u?"<td class='ui-datepicker-week-col'>"+this._get(t,"calculateWeek")(H)+"</td>":"",x=0;7>x;x++)O=g?g.apply(t.input?t.input[0]:null,[H]):[!0,""],j=H.getMonth()!==Z,W=j&&!_||!O[0]||G&&G>H||$&&H>$,F+="<td class='"+((x+c+6)%7>=5?" ui-datepicker-week-end":"")+(j?" ui-datepicker-other-month":"")+(H.getTime()===T.getTime()&&Z===t.selectedMonth&&t._keyEvent||b.getTime()===H.getTime()&&b.getTime()===T.getTime()?" "+this._dayOverClass:"")+(W?" "+this._unselectableClass+" ui-state-disabled":"")+(j&&!v?"":" "+O[1]+(H.getTime()===X.getTime()?" "+this._currentClass:"")+(H.getTime()===L.getTime()?" ui-datepicker-today":""))+"'"+(j&&!v||!O[2]?"":" title='"+O[2].replace(/'/g,"&#39;")+"'")+(W?"":" data-handler='selectDay' data-event='click' data-month='"+H.getMonth()+"' data-year='"+H.getFullYear()+"'")+">"+(j&&!v?"&#xa0;":W?"<span class='ui-state-default'>"+H.getDate()+"</span>":"<a class='ui-state-default"+(H.getTime()===L.getTime()?" ui-state-highlight":"")+(H.getTime()===X.getTime()?" ui-state-active":"")+(j?" ui-priority-secondary":"")+"' href='#'>"+H.getDate()+"</a>")+"</td>",H.setDate(H.getDate()+1),H=this._daylightSavingAdjust(H);S+=F+"</tr>"}Z++,Z>11&&(Z=0,te++),S+="</tbody></table>"+(q?"</div>"+(Q[0]>0&&D===Q[1]-1?"<div class='ui-datepicker-row-break'></div>":""):""),w+=S}y+=w}return y+=l,t._keyEvent=!1,y},_generateMonthYearHeader:function(t,e,i,s,n,a,r,o){var h,l,c,u,d,p,f,m,g=this._get(t,"changeMonth"),v=this._get(t,"changeYear"),_=this._get(t,"showMonthAfterYear"),b="<div class='ui-datepicker-title'>",y="";if(a||!g)y+="<span class='ui-datepicker-month'>"+r[e]+"</span>";else{for(h=s&&s.getFullYear()===i,l=n&&n.getFullYear()===i,y+="<select class='ui-datepicker-month' data-handler='selectMonth' data-event='change'>",c=0;12>c;c++)(!h||c>=s.getMonth())&&(!l||n.getMonth()>=c)&&(y+="<option value='"+c+"'"+(c===e?" selected='selected'":"")+">"+o[c]+"</option>");y+="</select>"}if(_||(b+=y+(!a&&g&&v?"":"&#xa0;")),!t.yearshtml)if(t.yearshtml="",a||!v)b+="<span class='ui-datepicker-year'>"+i+"</span>";else{for(u=this._get(t,"yearRange").split(":"),d=(new Date).getFullYear(),p=function(t){var e=t.match(/c[+\-].*/)?i+parseInt(t.substring(1),10):t.match(/[+\-].*/)?d+parseInt(t,10):parseInt(t,10);
return isNaN(e)?d:e},f=p(u[0]),m=Math.max(f,p(u[1]||"")),f=s?Math.max(f,s.getFullYear()):f,m=n?Math.min(m,n.getFullYear()):m,t.yearshtml+="<select class='ui-datepicker-year' data-handler='selectYear' data-event='change'>";m>=f;f++)t.yearshtml+="<option value='"+f+"'"+(f===i?" selected='selected'":"")+">"+f+"</option>";t.yearshtml+="</select>",b+=t.yearshtml,t.yearshtml=null}return b+=this._get(t,"yearSuffix"),_&&(b+=(!a&&g&&v?"":"&#xa0;")+y),b+="</div>"},_adjustInstDate:function(t,e,i){var s=t.drawYear+("Y"===i?e:0),n=t.drawMonth+("M"===i?e:0),a=Math.min(t.selectedDay,this._getDaysInMonth(s,n))+("D"===i?e:0),r=this._restrictMinMax(t,this._daylightSavingAdjust(new Date(s,n,a)));t.selectedDay=r.getDate(),t.drawMonth=t.selectedMonth=r.getMonth(),t.drawYear=t.selectedYear=r.getFullYear(),("M"===i||"Y"===i)&&this._notifyChange(t)},_restrictMinMax:function(t,e){var i=this._getMinMaxDate(t,"min"),s=this._getMinMaxDate(t,"max"),n=i&&i>e?i:e;return s&&n>s?s:n},_notifyChange:function(t){var e=this._get(t,"onChangeMonthYear");e&&e.apply(t.input?t.input[0]:null,[t.selectedYear,t.selectedMonth+1,t])},_getNumberOfMonths:function(t){var e=this._get(t,"numberOfMonths");return null==e?[1,1]:"number"==typeof e?[1,e]:e},_getMinMaxDate:function(t,e){return this._determineDate(t,this._get(t,e+"Date"),null)},_getDaysInMonth:function(t,e){return 32-this._daylightSavingAdjust(new Date(t,e,32)).getDate()},_getFirstDayOfMonth:function(t,e){return new Date(t,e,1).getDay()},_canAdjustMonth:function(t,e,i,s){var n=this._getNumberOfMonths(t),a=this._daylightSavingAdjust(new Date(i,s+(0>e?e:n[0]*n[1]),1));return 0>e&&a.setDate(this._getDaysInMonth(a.getFullYear(),a.getMonth())),this._isInRange(t,a)},_isInRange:function(t,e){var i,s,n=this._getMinMaxDate(t,"min"),a=this._getMinMaxDate(t,"max"),r=null,o=null,h=this._get(t,"yearRange");return h&&(i=h.split(":"),s=(new Date).getFullYear(),r=parseInt(i[0],10),o=parseInt(i[1],10),i[0].match(/[+\-].*/)&&(r+=s),i[1].match(/[+\-].*/)&&(o+=s)),(!n||e.getTime()>=n.getTime())&&(!a||e.getTime()<=a.getTime())&&(!r||e.getFullYear()>=r)&&(!o||o>=e.getFullYear())},_getFormatConfig:function(t){var e=this._get(t,"shortYearCutoff");return e="string"!=typeof e?e:(new Date).getFullYear()%100+parseInt(e,10),{shortYearCutoff:e,dayNamesShort:this._get(t,"dayNamesShort"),dayNames:this._get(t,"dayNames"),monthNamesShort:this._get(t,"monthNamesShort"),monthNames:this._get(t,"monthNames")}},_formatDate:function(t,e,i,s){e||(t.currentDay=t.selectedDay,t.currentMonth=t.selectedMonth,t.currentYear=t.selectedYear);var n=e?"object"==typeof e?e:this._daylightSavingAdjust(new Date(s,i,e)):this._daylightSavingAdjust(new Date(t.currentYear,t.currentMonth,t.currentDay));return this.formatDate(this._get(t,"dateFormat"),n,this._getFormatConfig(t))}}),t.fn.datepicker=function(e){if(!this.length)return this;t.datepicker.initialized||(t(document).mousedown(t.datepicker._checkExternalClick),t.datepicker.initialized=!0),0===t("#"+t.datepicker._mainDivId).length&&t("body").append(t.datepicker.dpDiv);var i=Array.prototype.slice.call(arguments,1);return"string"!=typeof e||"isDisabled"!==e&&"getDate"!==e&&"widget"!==e?"option"===e&&2===arguments.length&&"string"==typeof arguments[1]?t.datepicker["_"+e+"Datepicker"].apply(t.datepicker,[this[0]].concat(i)):this.each(function(){"string"==typeof e?t.datepicker["_"+e+"Datepicker"].apply(t.datepicker,[this].concat(i)):t.datepicker._attachDatepicker(this,e)}):t.datepicker["_"+e+"Datepicker"].apply(t.datepicker,[this[0]].concat(i))},t.datepicker=new i,t.datepicker.initialized=!1,t.datepicker.uuid=(new Date).getTime(),t.datepicker.version="1.10.4"})(jQuery);/**/ 
/*global.js*/ 
/*
..........................................................................
:: Links en ventana nueva                                               ::
..........................................................................
*/
$(document).ready(function() {
	$("a[rel=external]").attr({target: "_blank"})
});

var RecargarCKEditorInicio = true;

function RecargarTodosCKEditor() {
    RecargarCKEditorInicio = false;

    if (typeof (ckeCompletoComentarios) != 'undefined' && ckeCompletoComentarios == true) {
        $('textarea.cke.comentarios').removeClass('comentarios').addClass('recursos');
    }

    var textAreas = $('textarea.cke');

    DestruirTodosCKEditor();

    if (textAreas.length > 0) {
		var urlbase = $('input.inpt_baseURL').val();

		if (document.URL.indexOf('https://') == 0) {
			if (urlbase.indexOf('https://') == -1) {
				urlbase = urlbase.replace('http', 'https');
			}
		}
        var BasePath = CKEDITOR.basePath;
		var ImageBrowseUrl = urlbase + "/conector-ckeditor";
		var ImageUploadUrl = urlbase + "/conector-ckeditor";
		//var ImageBrowseUrl = urlbase + "/ConectorCKEditor.aspx";
		//var ImageUploadUrl = urlbase + "/ConectorCKEditor.aspx";
        //var ImageBrowseUrl = BasePath + "filemanager/browser/default/browser.html?Type=Image&Connector=" + BasePath + "filemanager/connectors/aspx/connector.aspx";
        //var ImageUploadUrl = BasePath + "filemanager/connectors/aspx/upload.aspx?Type=Image";
        var lang = $('#inpt_Idioma').val();

        $('textarea.cke.mensajes').ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Mensajes' });
        $('textarea.cke.recursos').ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Recursos', filebrowserImageBrowseUrl: ImageBrowseUrl, filebrowserImageUploadUrl: ImageUploadUrl });
        $('textarea.cke.blogs').ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Blogs', filebrowserImageBrowseUrl: ImageBrowseUrl, filebrowserImageUploadUrl: ImageUploadUrl });
        $('textarea.cke.comentarios').ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Comentario' });
    }
}


function DestruirTodosCKEditor() {
    var textAreas = $('textarea.cke'),
        instanceName = "",
        i = 0;

    for (i = 0; i < textAreas.length; i++) {
        instanceName = textAreas[i].id;

        if (CKEDITOR.instances[instanceName] != null) {
            try {
                //El remove no funciona...
                //CKEDITOR.remove(CKEDITOR.instances[instanceName]);
                CKEDITOR.instances[instanceName].destroy();
            }
            catch (error) {
            }
        }
    }
}

function RecargarCKEditor(id) {
    var textArea = $('textarea.cke#' + id);

    DestruirCKEditor(id);

	var BasePath = CKEDITOR.basePath;
	var ImageBrowseUrl = BasePath + "filemanager/browser/default/browser.html?Type=Image&Connector=" + BasePath + "filemanager/connectors/aspx/connector.aspx";
	var ImageUploadUrl = BasePath + "filemanager/connectors/aspx/upload.aspx?Type=Image";
	var lang = $('#inpt_Idioma').val();

	$('textarea.cke.mensajes#' + id).ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Mensajes' });
	$('textarea.cke.recursos#' + id).ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Recursos', filebrowserImageBrowseUrl: ImageBrowseUrl, filebrowserImageUploadUrl: ImageUploadUrl });
	$('textarea.cke.blogs#' + id).ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Blogs', filebrowserImageBrowseUrl: ImageBrowseUrl, filebrowserImageUploadUrl: ImageUploadUrl });
	$('textarea.cke.comentarios#' + id).ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Comentario' });

}

function DestruirCKEditor(id) {
	if(CKEDITOR.instances[id] != null){
		CKEDITOR.instances[id].destroy();
	}
}

/*
..........................................................................
:: Comprobar email valido                                               ::
..........................................................................
*/
function validarEmail(sTesteo) {
    //var reEmail = /^(?:\w+\.?)*\w+@(?:\w+\.)+\w+$/;
    var reEmail = /^\w+([\.\-ñÑ+]?\w+)*@\w+([\.\-ñÑ]?\w+)*(\.\w{2,})+$/;
    return reEmail.test(sTesteo);
}

function validarFecha( sFecha ) {
    var reFecha = /\b(0?[1-9]|[12][0-9]|3[01])\/([1-9]|0[1-9]|1[0-2])\/(19|20\d{2})/;
    return reFecha.test( sFecha );
}

//Valida una fecha y además comprueba si es bisiesto o no el año.
function esFecha(dia,mes,anio) /* Devuelve si una fecha pasada sus tres parámetros [dia,mes,año] es válida */
{
	//Creo la cadena con la fecha en formato "dd/mm/yyyy"
	if(dia < 10)
		var miDia = '0' + dia;
	else
		var miDia = dia;
	if(mes < 10)
		var miMes = '0' + mes;
	else
		var miMes = mes;
	
	var miFecha = miDia + '/' + miMes + '/' + anio;

	//Comprobamos si es un formato correcto
	var objRegExp = /^([0][1-9]|[12][0-9]|3[01])(\/|-)(0[1-9]|1[012])\2(\d{4})$/;

	if(!objRegExp.test(miFecha))
	{
		return false; //Es una fecha incorrecta porque no cumple el formato
	}
	else
	{
		var strSeparator = miFecha.substring(2,3);

		//Creamos el array con los parámetros de la fecha [dia,mes,año]
		var arrayDate = miFecha.split(strSeparator);
		//Array con el número de días que tiene cada mes excepto febrero que se valida aparte
		var arrayLookup = { '01' : 31,'03' : 31,'04' : 30,'05' : 31, '06' : 30,'07' : 31,
		'08' : 31,'09' : 30,'10' : 31,'11' : 30,'12' : 31}

		var intDay = parseInt(arrayDate[0],10);
		var intMonth = parseInt(arrayDate[1],10);
		var intYear = parseInt(arrayDate[2],10);

		//Comprobamos que el valor del día y del mes sean correctos
		if (intMonth != null) 
		{
			if(intMonth != 2)
			{
				if (intDay <= arrayLookup[arrayDate[1]] && intDay != 0)
				{
					return true;
				}
			}
		}

		//Comprobamos si es febrero y si el valor del día es correcto [Cambia si es bisiesto o no el año]
		if (intMonth == 2)
		{
			if (intDay > 0 && intDay < 29)
			{
				return true;
			}
			else if (intDay == 29)
			{
				if ((intYear % 4 == 0) && (intYear % 100 != 0) || (intYear % 400 == 0))
				{
					return true;
				}
			}
		}
	}

	return false; //Cualquier otro valor, falso
}

//Validar una URL
function esURL(sURL)
{
    var regexURL = /^((([hH][tT][tT][pP][sS]?|[fF][tT][pP])\:\/\/)?([\w\.\-]+(\:[\w\.\&%\$\-]+)*@)?((([^\s\(\)\<\>\\\"\.\[\]\,@;:]+)(\.[^\s\(\)\<\>\\\"\.\[\]\,@;:]+)*(\.[a-zA-Z]{2,4}))|((([01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}([01]?\d{1,2}|2[0-4]\d|25[0-5])))(\b\:(6553[0-5]|655[0-2]\d|65[0-4]\d{2}|6[0-4]\d{3}|[1-5]\d{4}|[1-9]\d{0,3}|0)\b)?((\/[^\/][\w\.\,\?\'\\\/\+&%\$#\=~_\-@:]*)*[^\.\,\?\"\'\(\)\[\]!;<>{}\s\x7F-\xFF])?)$/i;
	if(sURL.length > 0 && sURL.match(regexURL)){
		return true;
	}else{
		return false;
	}
}

/*
..........................................................................
:: Plugin de jQuery para cambiar PNG's para IE6 dentro de un elemento   ::
:: Ej. uso: $('div.fulanito img').pngIE6()                              ::
:: El parametro 'blank' debe ser la ruta de un GIF transparente de 1x1  ::
:: --------------------------------------                               ::
:: Despues, plugin para agitar cosas molonamente con una sola orden     ::
:: --------------------------------------                               ::
:: Ademas, plugin para hacer fadeOut y despues destruir un elemento     ::
..........................................................................
*/
//jQuery.fn.extend({
////    pngIE6: function(blank) {
////        if (!($.browser.msie && $.browser.version < 7)) return this;
////        if (!blank) blank = 'img/blank.gif';
////        return this.each( function() {
////            this.style.filter = 'progid:DXImageTransform.Microsoft.AlphaImageLoader(src='+ this.src +',sizingMethod=image)';
////            this.src = blank;
////        });
////    },
//    shakeIt: function(o) {
//    	return this.each( function(){
//		    /* Almacenaremos en un atributo del objeto del DOM a mover los valores originales que poseia 
//		     * la primera que se llama a la funcion para evitar que al llamarla dos veces seguidas se
//		     * produzcan animaciones desde una posicion incorrecta del margen.
//		     * El objeto 'o' que se puede pasar como parametro esta destinado a la configuracion de la
//		     * animacion. Por ejemplo $('#idCualquiera').shakeit({velocidad: 200, amplitud: 40, veces: 5})
//		     */
//    		var $this = $(this),
//    		    o     = o || {},
//    		    mL    = this.mLCache || parseInt($this.css('marginLeft')),
//    		    mR    = this.mRCache || parseInt($this.css('marginRight')),
//    		    vel   = parseInt(o.velocidad) || 120,
//    		    ampl  = parseInt(o.amplitud) || 15,
//    		    veces = parseInt(o.veces) || 2;
//    		this.wCache = $this.css('width');
//    		$this.css('width', $this.width()+'px');
//    		this.mLCache = mL;
//    		this.mRCache = mR;
//    		for (var i = 0; i < veces; ++i) {
//    			$this.animate({
//    				marginLeft: (mL + ampl) + 'px',
//    				marginRight: (mR - ampl) + 'px'
//				}, vel).animate({
//					marginLeft: (mL - ampl) + 'px',
//					marginRight: (mR + ampl) + 'px'
//				}, vel);
//    		}
//    		// volvemos al estado primigenio
//    		$this.animate({
//    			marginLeft: mL + 'px',
//    			marginRight: mR + 'px'
//    		}, vel, function() {
//    			$this.css('width', this.wCache);
//    		});
//    	});
//    },
//    fundidoANada: function(o) {
//    	var o = o || {};
//    	o.velocidad = o.velocidad || 600;
//    	return this.each( function() {
//    		$(this).fadeOut(o.velocidad, function() {
//    			$(this).remove();
//    		});
//    	});
//    }
//});

/*
..........................................................................
:: Arreglo de PNG's pasado a filter:progid:... para elementos puntuales ::
..........................................................................
*/
//$( function() {
//    if (!($.browser.msie && $.browser.version < 7)) return; // SOLO PARA IE6!!!
//    var transparenciasIE = {
//        '#wrap':'scale',
//        'div.mascaraBoton':'crop',
//        '#footer':'crop',
//        '#nav a':'crop',
//        '#tarjetaLeft':'crop',
//        '#tarjetaRight':'crop',
//        'span.fotoPerfil a,span.fotoPerfilBlog a,span.fotoPerfilGrande a':'image',
//        'span.fotoPerfil a,span.fotoPerfilBlog a,span.fotoPerfilGrande a':'image',
//        'span.miniSnapshot a':'image',
//        'div.EstadoDafo0':'scale',
//        'div.EstadoDafo1':'scale',
//        'div.EstadoDafo2':'scale',
//        'div.EstadoDafo3':'scale'
//    };    
//    for (elem in transparenciasIE) {
//        $(elem).each( function() {
//        	var ruta,
//        		$this =  $(this);
//        	ruta  = $this.css('backgroundImage').replace('url(', '').replace(')', '');
//        	$this.get(0).style.filter = 'progid:DXImageTransform.Microsoft.AlphaImageLoader(src='+ ruta +',sizingMethod='+ transparenciasIE[elem] +')';
//        	$this.css('backgroundImage', 'none');
//        });
//    }
//});

/*
 * Desplegables genericos a peticion                                                                            
 * Ej. uso: crearDesplegables('selector jQuery boton', 'selector jQuery desplegables', {opciones: 'apetecaun'}) 
 * Opciones disponibles {
 *     efecto: string con cualquiera de los efectos de jQuery UI disponibles (por defecto: 'slide')
 *     opciones: objeto con las opciones del efecto de jQuery UI
 *               (por defecto: {direction: 'up'} si no se define el efecto
 *     			 las propias del jQuery UI si se define otro efecto)
 *     velocidad: duracion del efecto (por defecto: 600)
 * }
 */
function crearDesplegables(desplegar, desplegable, o) {
    var desplegar   = $(desplegar),
        desplegable = $(desplegable),
        incompatibles= false;
        o = o || {};
    if (desplegar.length != desplegable.length) {
        //alert('Error en la funcion crearDesplegables()\ndesplegar.length != desplegable.length');
        return false;
    };
    if (!o.efecto) {
        o.efecto = 'slide';
        o.opciones =  {direction: 'up'};
    }
    desplegar.each(function(indice) {
    	if (desplegable.eq(indice).find('form.busquedaAv').length) {
    		this.incompatibilidad = 'noBusquedaAv';
    	}
    	if (this.className.indexOf('activo') == -1) {
    		desplegable.eq(indice).hide();
    	} else {
			desplegable.eq(indice).show();
		}
        $(this).click( function() {
			$(this).toggleClass('activo');
            desplegable.eq(indice).toggle(o.efecto, o.opciones, o.velocidad, o.callback);
            if (this.incompatibilidad) {
                $('div.'+this.incompatibilidad).toggle(o.efecto, o.opciones, o.velocidad, o.callback);	
            }
            return false;
        });
    });
}

function crearPestanyas(pestanya, ficha, o) {
    var pestanya   = $(pestanya),
        ficha = $(ficha),
        o = o || {};
    // OJO!!!
    // DEFINIMOS VARIABLE GLOBAL:
    flagPestanyas = false;
    pestanya.eq(0).addClass('activo');
    ficha.not(':first').hide();
    if (pestanya.length != ficha.length) {
        //alert('Error en la funcion crearfichas()\npestanya.length != ficha.length');
        return false;
    };
    if (!o.efecto) {
        o.efecto = 'slide';
        o.opciones =  {direction: 'up'};
    }
    pestanya.each(function(indice) {
    	var $this = $(this);
        $this.click( function() {
        	if ($this.hasClass('activo') || flagPestanyas) return false;
        	flagPestanyas = true;
			ficha.filter(':visible').hide(o.efecto, o.opciones, o.velocidad, function() {
	        	pestanya.removeClass('activo');
				$this.addClass('activo');
				ficha.eq(indice).show(o.efecto, o.opciones, o.velocidad, function() {
					flagPestanyas = false;
				});
			});
            return false;
        });
    });
}

function crearError(textoError, contenedor) {
    crearError(textoError, contenedor, false);
}

function crearError(textoError, contenedor, moverScroll) {
    crearError(textoError, contenedor, moverScroll, false)
}

function crearError(textoError, contenedor, moverScroll, positionAbsolute) {
	// contenedor debe ser el elemento del DOM donde mostrar el error
	// o un String para llegar al elemento via jQuery
	var link = '';
	
	if(moverScroll){
	    link = '<a name="MiError" style="display:block;"></a>';
	}
	
	var $c = $(contenedor);
	if ($c.find('div.ko').length) { // si ya existe el div.ko ...
	    try
        {
	        $c.find('div.ko').html(link + textoError).shakeIt();
        }catch(err)
	    {	    
	    }


		if(positionAbsolute){
		    $c.find('div.ko')[0].style.position = 'absolute';
		}
	} else { //... si no lo creamos
		$('<div class="ko" style="display:none" >' + link + textoError + '</div>').prependTo($c).slideDown('fast');
		if(positionAbsolute){
		    $c.find('div.ko')[0].style.position = 'absolute';
		}
    }
	$c.find('div.ko').show();
	
	if(moverScroll){
	    document.location = '#MiError';
	}
}

function LimpiarHtmlControl(pControl)
{
    if (document.getElementById(pControl) != null)
    {
        //El siguiente código falla en Internet Explorer ya que no deja dar valor a innerHTML a algunos controles
        //document.getElementById(pControl).innerHTML = '';
        var nodosHijos=document.getElementById(pControl).childNodes
        
        for(i=0;i<nodosHijos.length;i++){
            document.getElementById(pControl).removeChild(nodosHijos[i]);
        }
    }
}

function mascaraCancelarAbsolute(texto, contenedor, funcionConfirmada) {
    var $confirmar = $(['<div><div class="pregunta"><span>', texto, '</span><button onclick="return false;" class="btMini">', borr.si, '</button><button onclick="return false;" class="btMini">', borr.no, '</button></div><div class="mascara"></div></div>'].join(''));

    $confirmar.css({
        'z-index': 200
    });

    $confirmar.find('div').css({
        height: $(contenedor).height() + 'px',
        paddingTop: ($(contenedor).height() / 2) + 'px',
        width: $(contenedor).width() + 'px',
        display: 'none',
        position: 'absolute'
    }).filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);

    // Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
    $confirmar.prependTo($(contenedor))
		.find('button').click(function () { // Ambos botones hacen desaparecer la mascara
		    $confirmar.fundidoANada();
		}).eq(0).click(funcionConfirmada); // pero solo el primero activa la funcionConfirmada
}

function mascaraCancelar(texto, contenedor, funcionConfirmada) {
	// contenedor debe ser el elemento del DOM que se pretende eliminar (o no)
	// o un String para llegar al elemento via jQuery
	
	/* Creamos el elemento del DOM y le asignamos estilos.
	 * Estructura creada:
	 * <div class="confirmar">
	 *     <div class="pregunta">
	 *         <span>Texto de confirmacion aqui</span>
     *         <button class="btMini">Si</button>
	 *         <button class="btMini">No</button>
	 *     </div>
	 *     <div class="mascara"></div>
	 * </div>
	 */
	 //TODO : LOZA --> Crear control para popups propios con mas contenido y personalizables.
    var $confirmar = $(['<div><div class="pregunta"><span>', texto, '</span><button onclick="return false;" class="btMini">', borr.si, '</button><button onclick="return false;" class="btMini">', borr.no, '</button></div><div class="mascara"></div></div>'].join(''));

    $confirmar.css({
        'z-index': 200
    });

    $confirmar.find('div').css({
	    height: $(contenedor).height() + 'px',
	    paddingTop: ($(contenedor).height() / 2) + 'px',
	    width: $(contenedor).width() + 'px',
	    display: 'none',
        position: 'fixed'
	}).filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);

	// Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
    $confirmar.prependTo($(contenedor))
		.find('button').click( function() { // Ambos botones hacen desaparecer la mascara
			$confirmar.fundidoANada();
		}).eq(0).click(funcionConfirmada); // pero solo el primero activa la funcionConfirmada
}

function mascaraCancelarSiNo(texto, contenedor, funcionConfirmada, funcionNo) {
	// contenedor debe ser el elemento del DOM que se pretende eliminar (o no)
	// o un String para llegar al elemento via jQuery
	var $c = $(contenedor);
	var $superC = $(contenedor).parents('[class^=wrap]').eq(0);
	
	var $confirmar = $(['<div class="confirmar"><div class="pregunta"><span>',texto,'</span><button onclick="return false;" class="btMini">',borr.si,'</button><button onclick="return false;" class="btMini">',borr.no,'</button></div><div class="mascara"></div></div>'].join(''));
	$confirmar.find('div').css({
		top       :$c.offset().top  + 'px',
		left      :$superC.offset().left + 'px',
		height    :($c.height()/2)+15+'px',
		paddingTop:($c.height()/2)-15+'px',
		width     :$superC.width() +'px',
		display: 'none'
	}).filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
	// Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
	$confirmar.prependTo($superC)
		.find('button').click( function() { // Ambos botones hacen desaparecer la mascara
			$confirmar.fundidoANada();
		}).eq(0).click(funcionConfirmada);// pero solo el primero activa la funcionConfirmada
		
	$confirmar.find('button').eq(1).click(funcionNo);
}

function mascaraCancelar2(texto, contenedor, funcionConfirmada,textoInferior) {
	// contenedor debe ser el elemento del DOM que se pretende eliminar (o no)
	// o un String para llegar al elemento via jQuery
	var $c = $(contenedor);
	var $superC = $(contenedor).parents('[class^=wrap]').eq(0);
	
	var $anterior = $superC.children('[class^=confirmar]').eq(0);
	
	if($anterior.length > 0)
	{
	    //Eliminar el anterior
	    $superC.remove($anterior);
	}
	/* Creamos el elemento del DOM y le asignamos estilos.
	 * Estructura creada:
	 * <div class="confirmar">
	 *     <div class="pregunta">
	 *         <span>Texto de confirmacion aqui</span>
     *         <button class="btMini">Si</button>
	 *         <button class="btMini">No</button>
	 *     </div>
	 *     <div class="mascara"></div>
	 * </div>
	 */
	var $confirmar = $(['<div class="confirmar"><div class="pregunta"><span>',texto,'</span><br><br><button onclick="return false;" class="btMini">',borr.si,'</button><button onclick="return false;" class="btMini">',borr.no,'</button><p class="small"><br>',textoInferior,'</p></div><div class="mascara"></div></div>'].join(''));
	$confirmar.find('div').css({
		top       :$c.offset().top  + 'px',
		left      :$superC.offset().left + 'px',
		height    :$c.height()+'px',
		paddingTop:'20px',
		width     :$superC.width() +'px',
		display: 'none'
	}).filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
	// Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
	$confirmar.prependTo($superC)
		.find('button').click( function() { // Ambos botones hacen desaparecer la mascara
			$confirmar.fundidoANada();
		}).eq(0).click(funcionConfirmada); // pero solo el primero activa la funcionConfirmada
}

function mascaraCancelarAlturaFija2Textos(texto, contenedor,altura, funcionConfirmada,textoInferior) {
	// contenedor debe ser el elemento del DOM que se pretende eliminar (o no)
	// o un String para llegar al elemento via jQuery
	var $c = $(contenedor);
	var $superC = $(contenedor).parents('[class^=wrap]').eq(0);
	
	var $anterior = $superC.children('[class^=confirmar]').eq(0);
	
	if($anterior.length > 0)
	{
	    //Eliminar el anterior
	    $superC.remove($anterior);
	}
	/* Creamos el elemento del DOM y le asignamos estilos.
	 * Estructura creada:
	 * <div class="confirmar">
	 *     <div class="pregunta">
	 *         <span>Texto de confirmacion aqui</span>
     *         <button class="btMini">Si</button>
	 *         <button class="btMini">No</button>
	 *     </div>
	 *     <div class="mascara"></div>
	 * </div>
	 */
	var $confirmar = $(['<div class="confirmar"><div class="pregunta"><span>',texto,'</span><br><br><button onclick="return false;" class="btMini">',borr.si,'</button><button onclick="return false;" class="btMini">',borr.no,'</button><p class="small"><br>',textoInferior,'</p></div><div class="mascara"></div></div>'].join(''));
	$confirmar.find('div').css({
		top       :$c.offset().top  + 'px',
		left      :$superC.offset().left + 'px',
		height    :altura+'px',
		paddingTop:'20px',
		width     :$superC.width() +'px',
		display: 'none'
	}).filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
	// Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
	$confirmar.prependTo($superC)
		.find('button').click( function() { // Ambos botones hacen desaparecer la mascara
			$confirmar.fundidoANada();
		}).eq(0).click(funcionConfirmada); // pero solo el primero activa la funcionConfirmada
}

function mascaraAlerta(texto, contenedor) {
	var $c = $(contenedor);
	var $superC = $(contenedor).parents('[class^=wrap]').eq(0);
	var $confirmar = $(['<div class="confirmar"><div class="pregunta"><span>',texto,'</span><button onclick="return false;" class="btMini">Aceptar</button></div><div class="mascara"></div></div>'].join(''));
	$confirmar.find('div').css({
		top       :$c.offset().top  + 'px',
		left      :$superC.offset().left + 'px',
		height    :($c.height()/2)+15+'px',
		paddingTop:($c.height()/2)-15+'px',
		width     :$superC.width() +'px',
		display: 'none'
	}).filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
}


function miniConfirmar(texto, contenedor, funcionConfirmada) {
	// contenedor debe ser el elemento del DOM que se pretende eliminar (o no)
	// o un String para llegar al elemento via jQuery
	var $c = $(contenedor);
	/* Creamos el elemento del DOM y le asignamos estilos.
	 * Estructura creada:
	 * <div class="miniConfirmar">
	 *     <span>Texto de confirmacion aqui</span>
     *     <button class="btMini">Si</button>
	 *     <button class="btMini">No</button>
	 * </div>
	 */
	var $miniC = $(['<div class="miniConfirmar"><span>',texto,'</span><button class="btMini">',borr.si,'</button><button class="btMini">',borr.no,'</button></div>'].join(''));
	$miniC.css({display: 'none'}).fadeIn();
	// Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
	$miniC.prependTo($c)
		.find('button').click( function() { // Ambos botones hacen desaparecer la mascara
			$miniC.fundidoANada();
		}).eq(0).click(funcionConfirmada); // pero solo el primero activa la funcionConfirmada
}

// menu de edicion desplegable para los listados
//$(function() {
//	$('div.editarElemento').find('li:last-child').addClass('ultimo').end()
//	.children('a,img').click(function(evento) {
//		var editar_eliminar = function(instaKill) {
//			$('div.editar-desplegado').removeClass('editar-desplegado');
//			if(instaKill){$clon.remove()}else{
//				$clon.fadeOut('fast',function(){$(this).remove()});
//			}
//		};
//		var $padre = $(evento.target).parent();
//		var $clon = $('#editar-clon');
//		if ($padre.hasClass('editar-desplegado') && $clon.length) {
//			clearTimeout($clon.get(0).temporizador);
//		} else {
//			//nos cargamos el que habia
//			editar_eliminar(true);
//			// y creamos el otro
//			$padre.addClass('editar-desplegado');
//			$clon = $padre.find('ul:first').clone(true).attr('id', 'editar-clon')
//			.appendTo('body').css({
//				opacity: '0',
//				display: 'block'
//			});
//			// ahora que el clon nuevo ya existe y tiene dimensiones podemos ajustar que aparezca donde interesa
//			$clon.css({
//				top: $padre.offset().top + $padre.height() + 'px',
//				left: $padre.offset().left + $padre.width() - $clon.width(),
//				opacity: '1',
//				display: 'none'
//			}).fadeIn('fast').add($padre).hover(function() {
//				clearTimeout($clon.get(0).temporizador);
//			}, function() {
//				$clon.get(0).temporizador = setTimeout(editar_eliminar, 1000);
//			});
//			
//		}
//		return false;
//		
//	});
//});

//LOZA : Funcion para desplegar el menu de acciones, aadirsela al onclick (onclick="javascript:mostrarMenu(event);") del enlace, y meter la imagen que lleve al lado dentro del propio enlace
function mostrarMenu(evento) {
        if (!evento) var evento = window.event;
		var editar_eliminar = function(instaKill) {
			$('div.editar-desplegado').removeClass('editar-desplegado');
			if(instaKill){$clon.remove()}else{
				$clon.fadeOut('fast',function(){$(this).remove()});
			}
		};
		if(!evento.target){
		var hijo = evento.srcElement;
		}
		else{
		var hijo = evento.target;
		}
		if(hijo.nodeName == 'IMG'){
		    hijo = $(hijo).parent();
		}
		var $padre = $(hijo).parent();
		
		
		var $clon = $('#editar-clon');
		if ($padre.hasClass('editar-desplegado') && $clon.length) {
			clearTimeout($clon.get(0).temporizador);
		} else {
			//nos cargamos el que habia
			editar_eliminar(true);
			// y creamos el otro
			$padre.addClass('editar-desplegado');
			$clon = $padre.find('ul:first').clone(true).attr('id', 'editar-clon')
			.appendTo('body').css({
				opacity: '0',
				display: 'block'
			});
			// ahora que el clon nuevo ya existe y tiene dimensiones podemos ajustar que aparezca donde interesa
			$clon.css({
				top: $padre.offset().top + $padre.height() + 'px',
				left: $padre.offset().left + $padre.width() - $clon.width(),
				opacity: '1',
				display: 'none'
			}).fadeIn('fast').add($padre).hover(function() {
				clearTimeout($clon.get(0).temporizador);
			}, function() {
				$clon.get(0).temporizador = setTimeout(editar_eliminar, 1000);
			});
			
		}
		return false;
		
	}

function realizarFuncion(funcion, contexto){
eval(funcion);
}


//LOZA : Funcion para cambiar entre dos pestaas con efecto slide verical
//DesplegarPestanyas(
//                      id del boton(o elemento de cabecera tipo LI) que hacemos click, 
//                      id del panel al que se asocia el elemento anterior,
//                      id del boton(o elemento de cabecera tipo LI) que se encontraba activo,
//                      id del panel al que se asocia el elemento anterior)
function DesplegarPestanyas(pBoton, pPanel,pBoton2,pPanel2) {
    var boton   = $(document.getElementById(pBoton));
    var boton2   =$(document.getElementById(pBoton2));
    if(boton2[0].className == 'activo'){
        panel = $(document.getElementById(pPanel));
        panel2 = $(document.getElementById(pPanel2));
        o = {efecto:'blind', opciones:{direction:'vertical'}};
        if (!o.efecto) {
            o.efecto = 'blind';
            o.opciones =  {direction: 'vertical'};
        }
        panel2.toggle(o.efecto, o.opciones, o.velocidad, function() {
		    boton.toggleClass('activo');
	        boton2.toggleClass('activo');
		    panel.toggle(o.efecto, o.opciones, o.velocidad, null);
	    });
        return false;
    }
}


function EjecutarScriptsIniciales(){

	//var id = setInterval("EjecutarScriptsIniciales2()",100);
	//setTimeout("clearInterval("+id+")",1000);
	setTimeout("EjecutarScriptsIniciales2()",1000);
}
//LOZA, funcion para ejecutar los scripts iniciales en cada callback, para aadir transparencias y demas
function EjecutarScriptsIniciales2(){
    jQuery.fn.extend({
//    pngIE6: function(blank) {
//        if ($.browser.msie && $.browser.version < 7)
//        {
//        if (!blank) blank = 'img/blank.gif';
//        return this.each( function() {
//            this.style.filter = 'progid:DXImageTransform.Microsoft.AlphaImageLoader(src='+ this.src +',sizingMethod=image)';
//            this.src = blank;
//            });
//        }

//    },
    shakeIt: function(o) {
    	return this.each( function(){
		    /* Almacenaremos en un atributo del objeto del DOM a mover los valores originales que poseia 
		     * la primera que se llama a la funcion para evitar que al llamarla dos veces seguidas se
		     * produzcan animaciones desde una posicion incorrecta del margen.
		     * El objeto 'o' que se puede pasar como parametro esta destinado a la configuracion de la
		     * animacion. Por ejemplo $('#idCualquiera').shakeit({velocidad: 200, amplitud: 40, veces: 5})
		     */
    		var $this = $(this),
    		    o     = o || {},
    		    mL    = this.mLCache || parseInt($this.css('marginLeft')),
    		    mR    = this.mRCache || parseInt($this.css('marginRight')),
    		    vel   = parseInt(o.velocidad) || 120,
    		    ampl  = parseInt(o.amplitud) || 15,
    		    veces = parseInt(o.veces) || 2;
    		this.wCache = $this.css('width');
    		$this.css('width', $this.width()+'px');
    		this.mLCache = mL;
    		this.mRCache = mR;
    		for (var i = 0; i < veces; ++i) {
    			$this.animate({
    				marginLeft: (mL + ampl) + 'px',
    				marginRight: (mR - ampl) + 'px'
				}, vel).animate({
					marginLeft: (mL - ampl) + 'px',
					marginRight: (mR + ampl) + 'px'
				}, vel);
    		}
    		// volvemos al estado primigenio
    		$this.animate({
    			marginLeft: mL + 'px',
    			marginRight: mR + 'px'
    		}, vel, function() {
    			$this.css('width', this.wCache);
    		});
    	});
    },
    fundidoANada: function(o) {
    	var o = o || {};
    	o.velocidad = o.velocidad || 600;
    	return this.each( function() {
    		$(this).fadeOut(o.velocidad, function() {
    			$(this).remove();
    		});
    	});
    }
});


//$( function() {
//    if (!($.browser.msie && $.browser.version < 7)) return; // SOLO PARA IE6!!!
//    var transparenciasIE = {
//        '#wrap':'scale',
//        'div.mascaraBoton':'crop',
//        '#footer':'crop',
//        '#nav a':'crop',
//        '#tarjetaLeft':'crop',
//        '#tarjetaRight':'crop',
//        'span.fotoPerfil a,span.fotoPerfilBlog a,span.fotoPerfilGrande a':'image',
//        'span.miniSnapshot a':'image',
//        'div.EstadoDafo0':'scale',
//        'div.EstadoDafo1':'scale',
//        'div.EstadoDafo2':'scale',
//        'div.EstadoDafo3':'scale',
//        'img':'crop'
//    };    
//    for (elem in transparenciasIE) {
//        $(elem).each( function() {
//        	var ruta,
//        		$this =  $(this);
//        	if($this.css('filter')==''){
//        	    ruta  = $this.css('backgroundImage').replace('url(', '').replace(')', '');
//        	    $this.get(0).style.filter = 'progid:DXImageTransform.Microsoft.AlphaImageLoader(src='+ ruta +',sizingMethod='+ transparenciasIE[elem] +')';
//        	    $this.css('backgroundImage', 'none');
//        	}
//        });
//    }
//    
//});

//Muestra los paneles con la clase panelOcultar
$( function() {
	var divsPanelOcultar =$('div.panelOcultar');
	var i;
	for(i=0;i<divsPanelOcultar.length;i++)
	{
        divsPanelOcultar[i].style.display = "block";
	}
});

//Muestra los '+' en los desplegables de las descripciones que tengan la clase TextoTiny
$( function() {
	var divsTextoTiny =$('div.TextoTiny');
	var i;
	for(i=0;i<divsTextoTiny.length;i++)
	{
	    if(divsTextoTiny[i].id != "")
	    {
		    if(divsTextoTiny[i].offsetHeight < divsTextoTiny[i].scrollHeight)
		    {
		        if($(document.getElementById(divsTextoTiny[i].id)).find('object').length > 0)
		        {
		            var objeto = $(document.getElementById(divsTextoTiny[i].id)).find('object')[0];
		            if(objeto.width > divsTextoTiny[i].offsetWidth)
		            {
		                objeto.height = objeto.height / (objeto.width / divsTextoTiny[i].offsetWidth);
		                objeto.width = divsTextoTiny[i].offsetWidth;
		            }
		        }
    		    
		        //$(document.getElementById(divsTextoTiny[i].id)).find('object').hide();
		        var objects = $(document.getElementById(divsTextoTiny[i].id)).find('object');
		        for (var count= 0;count<objects.length;count++)
		        {
		            var object = objects[count];
		            if (object != null)
		            {
		                if (object.innerHTML.indexOf('<param name="wmode" value="transparent">') == -1)
		                {
		                    object.innerHTML = '<param name="wmode" value="transparent">' + object.innerHTML;
		                }
    		            
    		            
                        if (object.innerHTML.indexOf('<embed') != -1 && object.innerHTML.indexOf('<embed wmode="transparent"') == -1)
	                    {
	                        object.innerHTML = object.innerHTML.replace('<embed','<embed wmode="transparent"');
	                    }
	                }
		        }
			    if(document.getElementById(divsTextoTiny[i].id + '_DesplegarTexto') != null)
			    {
				    document.getElementById(divsTextoTiny[i].id + '_DesplegarTexto').style['display'] = '';
			    }
		    }
		    else
		    {
			    divsTextoTiny[i].style['height'] = '';
		    }
		}
	}
});

//Oculta los difuminados en las descripciones que tengan la clase TextoDifuminado
$( function() {
	var divsTextoDifuminadoTiny =$('div.TextoDifuminado');
	var i;
	
	for(i=0;i<divsTextoDifuminadoTiny.length;i++)
	{	    
	    if(divsTextoDifuminadoTiny[i].offsetHeight >= divsTextoDifuminadoTiny[i].scrollHeight)
	    {
	         if(document.getElementById(divsTextoDifuminadoTiny[i].id + '_DifuminarTexto') != null)
		    {		
			    document.getElementById(divsTextoDifuminadoTiny[i].id + '_DifuminarTexto').style['display'] = 'none';
		    }
	    }		
	}
});

//Muestra los '+' en los desplegables de las etiquetas y categorias desplegarEtiquetas
$( function() {
	var divsDesplegarEtiquetas =$('div.desplegarEtiquetas');
	var i;
	for(i=0;i<divsDesplegarEtiquetas.length;i++)
	{
	    if(divsDesplegarEtiquetas[i].id != "")
	    {
			var imagenMas = $(document.getElementById(divsDesplegarEtiquetas[i].id)).find('img.mas')[0];
			var imagenMenos = $(document.getElementById(divsDesplegarEtiquetas[i].id)).find('img.menos')[0];
			
			imagenMas.style.display = "none";
			imagenMenos.style.display = "none";
			
		    if((divsDesplegarEtiquetas[i].offsetHeight * 2 + 2) < divsDesplegarEtiquetas[i].scrollHeight)
		    {
				imagenMas.style.display = "";
				imagenMenos.style.display = "";
			
                var alturaMaxima = divsDesplegarEtiquetas[i].getBoundingClientRect().top + divsDesplegarEtiquetas[i].offsetHeight -14; //offsetTop maximo del ultimo elemento
                var enlaces = $(divsDesplegarEtiquetas[i]).find('a');
                
			    var ultimoEnlaceFila = 0,
			        tieneImagenes = false, 
			        comprobar = true, 
			        j=0;
			     
				while(!tieneImagenes && j < enlaces.length)
		        {
		            if(comprobar)
		            {
						ultimoEnlaceFila = j;
						if(enlaces[j].getBoundingClientRect().top > alturaMaxima + 2 || enlaces[j].innerText == "")
						{
						    comprobar = false;
		                    //break;
		                }
		            }
		            if($(enlaces[j].parentNode).find('img').length > 0  && enlaces[j].parentNode.className.indexOf("desplegarEtiquetas") == -1)
		            {
		                tieneImagenes = true;
		            }
					j++
		        }
		        j = ultimoEnlaceFila - 1;

		        imagenMas.style.position = "relative";
		        imagenMas.style.top = "0px";
		        imagenMas.style.left = "0px";
		        imagenMas.style['z-index'] = "100";
		           
			    if ($(divsDesplegarEtiquetas[i]).find('span.tag').length > 0 || tieneImagenes || j<0)
			    {
			        imagenMas.style.display = "none";
			        divsDesplegarEtiquetas[i].style['height'] = '';
			    }
			    else
			    {
		            imagenMas.style.top = enlaces[j].getBoundingClientRect().top - imagenMas.getBoundingClientRect().top + 3 + 'px';
		            imagenMas.style.left = enlaces[j].getBoundingClientRect().right - imagenMas.getBoundingClientRect().right + imagenMas.offsetWidth + 3 + 'px';
			    }
		    }
		    else
		    {
			    divsDesplegarEtiquetas[i].style['height'] = '';
		    }
		}
	}
});

	//Oculta los paneles con la clase panelOcultar [Miguel]
    $( 
        function()
        {
	        var divsPanelOcultar = $('div.panelOcultar');
	        try
            {
                if (document.getElementById('ctl00_CPH1_desplegadosHack') != null)
                {
	                var desplegadosDiv = document.getElementById('ctl00_CPH1_desplegadosHack').value;
	                var i;
	                for(i=0;i<divsPanelOcultar.length;i++)
	                {
	                    //Miramos si no está desplegado para ocultarlo
	                    if (desplegadosDiv.match(divsPanelOcultar[i].id) == null)
	                    {
                            divsPanelOcultar[i].style.display = "none";
                        }
                        else
                        {
                            //Ponemos la ficha de desplegado activo [por si ha hecho F5 se repinte bien]
                            document.getElementById('Titulo' + divsPanelOcultar[i].id).className = "desplegable activo";
                        }
	                }
	            }
	        }
	        catch(err)
	        {
	            var i;
	            for(i=0;i<divsPanelOcultar.length;i++)
	            {
                    divsPanelOcultar[i].style.display = "none";
	            }
	        }
        }
     );

	//Pone el estilo a los CKEDITOR que lo hayan perdido
    $( 
        function()
        {
			try
			{
                var textAreas = $('.comentarios textarea.cke'),
                    instanceName = "",
                    config = "",
                    i = 0;

                for(i=0;i<textAreas.length;i++)
                {
                    instanceName = textAreas[i].id;					    
                    
					if(document.getElementById('cke_' + instanceName) == null)
					{
                        config = {toolbar : textAreas[i].className.split(' ')[1]};

						if(CKEDITOR.instances[instanceName] != null)
						{
						    var instance = CKEDITOR.instances[instanceName];
						    instance.destroy();
						    CKEDITOR.remove(instance);
						}
						CKEDITOR.replace(instanceName, config);
						CKEDITOR.document.getById( 'cke_contents_' + instanceName ).setStyle( 'height', '120px' );
						
						if(CKEDITOR.instances[instanceName] != null)
						{
						    var editor = CKEDITOR.instances[instanceName];
                            editor.on('paste', function(evt) {
                                evt.data.html=evt.data.html.replace(/\\u0000/g,'');
                                evt.data.html=evt.data.html.replace(/\\u00AD/g,'');
                                evt.data.html=evt.data.html.replace(/\\u0600/g,'');
                                evt.data.html=evt.data.html.replace(/\\u0604/g,'');
                                evt.data.html=evt.data.html.replace(/\\u070F/g,'');
                                evt.data.html=evt.data.html.replace(/\\u17B4/g,'');
                                evt.data.html=evt.data.html.replace(/\\u17B5/g,'');
                                evt.data.html=evt.data.html.replace(/\\u200C/g,'');
                                evt.data.html=evt.data.html.replace(/\\u200F/g,'');
                                evt.data.html=evt.data.html.replace(/\\u2028/g,'');
                                evt.data.html=evt.data.html.replace(/\\u202F/g,'');
                                evt.data.html=evt.data.html.replace(/\\u2060/g,'');
                                evt.data.html=evt.data.html.replace(/\\u206F/g,'');
                                evt.data.html=evt.data.html.replace(/\\uFEFF/g,'');
                                evt.data.html=evt.data.html.replace(/\\uFFF0/g,'');
                                evt.data.html=evt.data.html.replace(/\\uFFFF/g,'');
                                
                                var texto = evt.data.html;
	                            var expReg = [{busq:/style="[^"]*"/, reemp:''}]
                        					    
	                            for(var i=0; i< expReg.length; i++)
	                            {
		                            var expRegEnlace = expReg[i] ;
		                            var oMatch = texto.match( expRegEnlace.busq ) ;
		                            while(oMatch)
		                            {
			                            texto = texto.replace(oMatch, expRegEnlace.reemp);
			                            oMatch = texto.match( expRegEnlace.busq ) ;
		                            }
	                            }

	                            evt.data.html = texto;
		
                            }, editor.element.$);
                        }
					}
				}
			}
			catch(error)
			{
			}
        }
     );

	//Pone el estilo a los CKEDITOR que lo hayan perdido
    $( 
        function()
        {
			try
			{
			    if (RecargarCKEditorInicio) {
			        RecargarTodosCKEditor();
			    }
			}
			catch(error)
			{
			}
        }
     );

}

function DesplegarDescripcionMasNueva(imagen, panelId, alturaMin)
{
	var panel = document.getElementById(panelId);
	if(panel.offsetHeight < panel.scrollHeight)
	{
		EstirarPanel(panel);
	    $(imagen).find('img')[0].src = $(imagen).find('img')[0].src.replace('verMas.gif', 'verMenos.gif');
	    $(document.getElementById(panelId + '_DesplegarTexto')).find('span')[0].innerHTML = "";
		//$(document.getElementById(panelId)).find('object').show();
		
	}
	else
	{
		EncogerPanel(panel, alturaMin);
	    $(imagen).find('img')[0].src = $(imagen).find('img')[0].src.replace('verMenos.gif', 'verMas.gif');
	    $(document.getElementById(panelId + '_DesplegarTexto')).find('span')[0].innerHTML = "...";
		//$(document.getElementById(panelId)).find('object').hide();
	}
}

function DesplegarDescripcionConLeerMas(imagen, panelId, alturaMin)
{
	var panel = document.getElementById(panelId);
	if(panel.offsetHeight < panel.scrollHeight)
	{
		EstirarPanel(panel);
		
		var spans = $(imagen).find('span');
		if (spans.length > 0)
		{
		    spans[0].style.display = 'none';
		    $(imagen).find('img')[0].style.display = '';
		}
	}
	else
	{
		EncogerPanel(panel, alturaMin);
		
		var spans = $(imagen).find('span');
		if (spans.length > 0)
		{
		    spans[0].style.display = '';
		    $(imagen).find('img')[0].style.display = 'none';
		}
	}
}

function DesplegarEtiquetaMas(panelId, alturaMin)
{
	var imagen = $(document.getElementById(panelId)).find('img.mas')[0];
	var panel = document.getElementById(panelId);
	if(panel.offsetHeight < panel.scrollHeight)
	{
		EstirarPanel(panel);
		imagen.style.display = "none";
	}
	else
	{
		EncogerPanel(panel, alturaMin);
		imagen.style.display = "";
	}
}

function EstirarPanel(panel)
{
	if(panel.offsetHeight < panel.scrollHeight){
		panel.style.height = panel.offsetHeight + panel.scrollHeight / 20 + "px";
		//panel.style.height = panel.offsetHeight + 12 + "px";
		if(panel.offsetHeight > panel.scrollHeight){
			panel.style.height = panel.scrollHeight;
		}
		setTimeout("EstirarPanel(document.getElementById('" + panel.id + "'))",1);
	}
	else
	{
		panel.style.height = panel.scrollHeight + "px";
	}
}

function EncogerPanel(panel,alturaMin)
{
	if(panel.offsetHeight > alturaMin){
		panel.style.height = panel.offsetHeight - panel.scrollHeight / 20 + "px";
		//panel.style.height = panel.offsetHeight - 12 + "px";
		if(panel.offsetHeight <alturaMin){
			panel.style.height = alturaMin;
		}
		setTimeout("EncogerPanel(document.getElementById('" + panel.id + "')," + alturaMin + ")",1);
	}
	else
	{
		panel.style.height = alturaMin + "px";
	}
}

function isDate(texbox) {
    var fecha = texbox.value;
    var datePat = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/;
    var matchArray = fecha.match(datePat); // is the format ok?

    if (matchArray == null) {
        //alert("Please enter date as either mm/dd/yyyy or mm-dd-yyyy.");
        return false;
    }
    
    day = matchArray[1];// pasamos la fecha a variables
    month = matchArray[3];
    year = matchArray[5];

    if (month < 1 || month > 12) { // comprobamos el mes
        return false;
    }

    if (day < 1 || day > 31) {
        return false;
    }

    if ((month==4 || month==6 || month==9 || month==11) && day==31) {
        return false;
    }

    if (month == 2) { // comprobamos el 29 de febrero
        var isleap = (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0));
        if (day > 29 || (day==29 && !isleap)) {
            return false;
        }
    }

    texbox.value = day + "/" + month + "/" + year;

    return true; // fecha es valida
}


function ComprobarFechas(fecha1, fecha2, fechaCambiada) {
    if (fecha1.value != calendario.desde && fecha1.value != calendario.desde && fecha1.value != "") {
        if (isDate(fecha1)) {
            if (fecha2.value != calendario.desde && fecha2.value != calendario.desde && fecha2.value != "") {
                var datePat = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/;
                if (fechaCambiada == "1") {
                    var fecha1Array = fecha1.value.match(datePat)
                    var fecha2Array = fecha2.value.match(datePat)
                }
                else {
                    var fecha2Array = fecha1.value.match(datePat)
                    var fecha1Array = fecha2.value.match(datePat)
                }

                var day1, day2, month1, month2, year1, year2;


                if (fecha1Array != null) {
                    day1 = fecha1Array[1]; // pasamos la fecha a variables
                    month1 = fecha1Array[3];
                }
                if (fecha2Array != null) {
                    day2 = fecha2Array[1];
                    month2 = fecha2Array[3];
                }
                
                if (fecha1Array != null) {
                    year1 = fecha1Array[5];
                }
                if (fecha2Array != null) {
                    year2 = fecha2Array[5];
                }

                var resultado = false;
                if (year1 > year2) {
                    resultado = true;
                }
                else {
                    if (year1 == year2) {
                        if (month1 > month2) {
                            resultado = true;
                        }
                        else {
                            if (month1 == month2) {
                                if (day1 > day2) {
                                    resultado = true;
                                }
                            }
                        }
                    }
                }
                if (resultado) {
                    if (fechaCambiada == "1") {
                        fecha1.value = calendario.desde;
                    }
                    else {
                        fecha1.value = calendario.hasta;
                    }
                }
            }
        }
        else {
            if (fechaCambiada == "1") {
                fecha1.value = calendario.desde;
            }
            else {
                fecha1.value = calendario.hasta;
            }
        }
    }
}

/*                                                                              Grafico Trabajo
 *---------------------------------------------------------------------------------------------

 */
$(function() {
	var $context = $('#graficoTrabajo'),
		$dt = $('dt', $context),
		$li = $('li', $context),
		maxH = 0,
		totalW = $context.width();
	
	$dt.each(function(index) {
		var $t = $(this),
			proposedW = Math.floor(totalW * parseInt($t.find('big').text(), 10)/100),
			deltaW = $t.width()-proposedW;
		// preparamos los css
		$t.css('cursor', 'pointer');
		$t.width( proposedW + 'px' );
		maxH = Math.max(maxH, $t.height());
		// preparamos los eventos del DD
		$t.mouseover(function() {
			$t.next().show().css({
				left:$t.offset().left + 'px',
				top:$t.offset().top - 12 - $t.next().height() + 'px'
			})
		}).mouseout(function() {
			$t.next().hide();
		});
	}).height(maxH);
});
/*                                                                                Tooltips (Tt)
 *---------------------------------------------------------------------------------------------
 */
$(function() {
	var posicionarTt = function(event) {
		var tPosX = event.pageX - 10;
		var tPosY = event.pageY - 17 - ($("div.tooltip").height() || 0);
		if(tPosY < window.scrollY + 15)
		{
		    tPosY = tPosY + 60;
		}
		$("div.tooltip").css({
			top: tPosY,
			left: tPosX
		});
	}

	var mostrarTt = function(event){
	    $("div.tooltip").remove();
		var textoTt = (this.tooltipData) ? this.tooltipData : $(this).text();
		$("<div class='tooltip' style='display:none;'>" + textoTt + "</div>")
		   .appendTo("body")
		   .fadeIn();
	    posicionarTt(event);
	}

	var ocultarTt = function() {
		$("div.tooltip").remove();
	}

	$(".conTt").each(function() {
		if (this.title) {
			this.tooltipData=this.title;
			this.removeAttribute('title');
		}
	}).hover(mostrarTt, ocultarTt).mousemove(posicionarTt);
});

/*                                                             Tooltips para freebase (conFbTt)
 *---------------------------------------------------------------------------------------------
 */
var necesarioPosicionar = true;
var mouseOnTooltip = false;
var cerrar = 0;
var tooltipActivo = '';

$(function() {
	var posicionarFreebaseTt = function(event) {
	    if(necesarioPosicionar && $("div.tooltip").length > 0){
		    var tPosX = event.pageX - 10;
		    var tPosY = event.pageY - 17 - ($("div.tooltip").height() || 0);
    		
		    var navegador = navigator.appName;
		    var anchoVentana = window.innerWidth;
		    var altoScroll = window.pageYOffset;
    		
            if (navegador == "Microsoft Internet Explorer") {
                anchoVentana = window.document.body.clientWidth;
                altoScroll = document.documentElement.scrollTop;
            }
    		
		    var sumaX = tPosX + $("div.tooltip").width() + 30;
		    if(sumaX > anchoVentana){
		        tPosX = anchoVentana - $("div.tooltip").width() - 30;
		    }
    		
		    if(tPosY < altoScroll){
		        tPosY = event.pageY + 12
		    }
    		
		    $("div.tooltip").css({
			    top: tPosY,
			    left: tPosX
		    });
		    necesarioPosicionar = false;
		}
	}

	var mostrarFreebaseTt = function(event){
	    var hayTooltip = $("div.tooltip").length != 0;
	    var tooltipDiferente = false;
	    
	    if(hayTooltip && tooltipActivo != '' && $(this).hasClass('conFbTt')){
	        tooltipDiferente = ($(this).text() != tooltipActivo);
	    }
	    
	    if(!hayTooltip || tooltipDiferente){
	        $("div.tooltip").remove();
		    var textoTt = (this.tooltipData) ? this.tooltipData : $(this).text();
		    tooltipActivo = $(this).text();
		    $("<div class='tooltip' style='display:none; width:350px; height:auto;padding:0;' onmousemove='javascript:mouseSobreTooltip()' onmouseover='javascript:mouseSobreTooltip()' onmouseout='javascript:mouseFueraTooltip()'><div class='relatedInfoWindow'><p class='poweredby'>Powered by <a href='http://www.gnoss.com'><strong>Gnoss</strong></a></p><div class='wrapRelatedInfoWindow'>" + textoTt + "</div> <p><em>" + $('input.inpt_avisoLegal').val() + "</em></p></div></div>")
		       .appendTo("body")
		       .fadeIn();	       
		       
		       $("div.tooltip").hover(mostrarFreebaseTt, ocultarFreebaseTt).mousemove(posicionarFreebaseTt);
		    if(tooltipDiferente){
		        necesarioPosicionar = true;
		    }
	        posicionarFreebaseTt(event);
	    }
	    cerrar++;
	}

	var ocultarFreebaseTt = function() {
		setTimeout(quitarFreebaseTt, 1000);
	}

	$(".conFbTt").each(function() {
		if (this.title) {
			this.tooltipData=this.title;
			this.removeAttribute('title');
		}
	}).hover(mostrarFreebaseTt, ocultarFreebaseTt).mousemove(posicionarFreebaseTt);
});


function quitarFreebaseTt(){
    cerrar--;
    if((cerrar <= 0) && (!mouseOnTooltip)){
        $("div.tooltip").remove();
        necesarioPosicionar = true;
    }
}

function mouseFueraTooltip()
{
    mouseOnTooltip = false;
    if(cerrar <= 0){
        setTimeout(quitarFreebaseTt, 1000);
    }
}

function mouseSobreTooltip()
{
    mouseOnTooltip = true;
}

function GuardarDescrHack(pCKeID, pHackID)
{
	//document.getElementById(pHackID).value = ObtenerValorCKEditor(pCKeID);
	document.getElementById(pHackID).value = $('#' + pCKeID).val();
}

function ObtenerValorCKEditor(pCKeID)
{
    try
    {
        var editor = CKEDITOR.instances[pCKeID];

	    if(editor)
	    {
		    var texto = editor.document.$.body.innerHTML;
		    var expReg = [{busq:/target="[^"]*"/g, reemp:''} ,
		                  {busq:/id="[^"]*"/g, reemp:''} ,
		                  {busq:/href="javascript:void\(0\)\/\*\d*\*\/"/g, reemp:''} ,
		                  {busq:/_cke_saved_href=/g, reemp:'href='}]

		    for(var i=0; i< expReg.length; i++)
		    {
			    var expRegEnlace = expReg[i] ;
			    var oMatch = texto.match( expRegEnlace.busq ) ;
			    if(oMatch)
			    {
			        for(var j=0; j< oMatch.length; j++)
			        {
				        texto = texto.replace(oMatch[j], expRegEnlace.reemp);
			        }
			    }
		    }
		    texto = texto.replace(/<a /g, '<a target="_blank" ');

		    return texto;
	    }
	    else
	    {
		    return  document.getElementById(pCKeID).value;
	    }
	}
	catch(ex)
	{
	    return null;
	}
}


//Reemplaza los contadores '(n)' p.e. en la bandeja de mensajes
function reemplazarContadores(idPanel, numero)
{
    var texto = $(idPanel).html();
    var expRegEnlace ='\\([0-9]*\\)';
    var re = new RegExp(expRegEnlace);
    var oMatch = texto.match(re);
    if(oMatch)
    {
        if(numero > 0)
        {
            texto = texto.replace(oMatch, '(' + numero + ')');
        }
        else
        {
            texto = texto.replace(oMatch, '');
        }
    }
    else
    {
        if(numero > 0)
        {
            texto = texto + '(' + numero + ')';
        }
    }
    $(idPanel).html(texto);
}

function ObtenerHash(){
	var hash = window.location.hash;
	if(hash != null && hash != ''){
		var posicion = hash.indexOf(hash)
		if(posicion > -1){
			return hash;
			}
	}
	return '';
}

function urlEncodeCharacter(c)
{
	return '%' + c.charCodeAt(0).toString(16);
}

function urlDecodeCharacter(str, c)
{
	return String.fromCharCode(parseInt(c, 16));
}

function urlEncode(s)
{
    var is_firefox = navigator.userAgent.toLowerCase().indexOf('firefox/') > -1;  
    if (is_firefox)
    {
        return encodeURIComponent(s).replace( /[^0-9a-z]/g, urlEncodeCharacter );
    }
    else
    {
        return encodeURIComponent(s);
    }
}

function urlDecode(s)
{
    var is_firefox = navigator.userAgent.toLowerCase().indexOf('firefox/') > -1;  
    if (is_firefox)
    {
        return decodeURIComponent(s).replace( /\%([0-9a-f]{2})/g, urlDecodeCharacter);
    }
    else
    {
        return decodeURIComponent(s);
    }
}

Encoder = {

	// When encoding do we convert characters into html or numerical entities
    EncodeType: "entity",  // entity OR numerical
    arr1: ['&nbsp;', '&iexcl;', '&cent;', '&pound;', '&curren;', '&yen;', '&brvbar;', '&sect;', '&uml;', '&copy;', '&ordf;', '&laquo;', '&not;', '&shy;', '&reg;', '&macr;', '&deg;', '&plusmn;', '&sup2;', '&sup3;', '&acute;', '&micro;', '&para;', '&middot;', '&cedil;', '&sup1;', '&ordm;', '&raquo;', '&frac14;', '&frac12;', '&frac34;', '&iquest;', '&Agrave;', '&Aacute;', '&Acirc;', '&Atilde;', '&Auml;', '&Aring;', '&AElig;', '&Ccedil;', '&Egrave;', '&Eacute;', '&Ecirc;', '&Euml;', '&Igrave;', '&Iacute;', '&Icirc;', '&Iuml;', '&ETH;', '&Ntilde;', '&Ograve;', '&Oacute;', '&Ocirc;', '&Otilde;', '&Ouml;', '&times;', '&Oslash;', '&Ugrave;', '&Uacute;', '&Ucirc;', '&Uuml;', '&Yacute;', '&THORN;', '&szlig;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&ouml;', '&divide;', '&oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&uuml;', '&yacute;', '&thorn;', '&yuml;', '&quot;', '&amp;', '&lt;', '&gt;', '&OElig;', '&oelig;', '&Scaron;', '&scaron;', '&Yuml;', '&circ;', '&tilde;', '&ensp;', '&emsp;', '&thinsp;', '&zwnj;', '&zwj;', '&lrm;', '&rlm;', '&ndash;', '&mdash;', '&lsquo;', '&rsquo;', '&sbquo;', '&ldquo;', '&rdquo;', '&bdquo;', '&dagger;', '&Dagger;', '&permil;', '&lsaquo;', '&rsaquo;', '&euro;', '&fnof;', '&Alpha;', '&Beta;', '&Gamma;', '&Delta;', '&Epsilon;', '&Zeta;', '&Eta;', '&Theta;', '&Iota;', '&Kappa;', '&Lambda;', '&Mu;', '&Nu;', '&Xi;', '&Omicron;', '&Pi;', '&Rho;', '&Sigma;', '&Tau;', '&Upsilon;', '&Phi;', '&Chi;', '&Psi;', '&Omega;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigmaf;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&thetasym;', '&upsih;', '&piv;', '&bull;', '&hellip;', '&prime;', '&Prime;', '&oline;', '&frasl;', '&weierp;', '&image;', '&real;', '&trade;', '&alefsym;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&crarr;', '&lArr;', '&uArr;', '&rArr;', '&dArr;', '&hArr;', '&forall;', '&part;', '&exist;', '&empty;', '&nabla;', '&isin;', '&notin;', '&ni;', '&prod;', '&sum;', '&minus;', '&lowast;', '&radic;', '&prop;', '&infin;', '&ang;', '&and;', '&or;', '&cap;', '&cup;', '&int;', '&there4;', '&sim;', '&cong;', '&asymp;', '&ne;', '&equiv;', '&le;', '&ge;', '&sub;', '&sup;', '&nsub;', '&sube;', '&supe;', '&oplus;', '&otimes;', '&perp;', '&sdot;', '&lceil;', '&rceil;', '&lfloor;', '&rfloor;', '&lang;', '&rang;', '&loz;', '&spades;', '&clubs;', '&hearts;', '&diams;'],
    arr2: ['&#160;', '&#161;', '&#162;', '&#163;', '&#164;', '&#165;', '&#166;', '&#167;', '&#168;', '&#169;', '&#170;', '&#171;', '&#172;', '&#173;', '&#174;', '&#175;', '&#176;', '&#177;', '&#178;', '&#179;', '&#180;', '&#181;', '&#182;', '&#183;', '&#184;', '&#185;', '&#186;', '&#187;', '&#188;', '&#189;', '&#190;', '&#191;', '&#192;', '&#193;', '&#194;', '&#195;', '&#196;', '&#197;', '&#198;', '&#199;', '&#200;', '&#201;', '&#202;', '&#203;', '&#204;', '&#205;', '&#206;', '&#207;', '&#208;', '&#209;', '&#210;', '&#211;', '&#212;', '&#213;', '&#214;', '&#215;', '&#216;', '&#217;', '&#218;', '&#219;', '&#220;', '&#221;', '&#222;', '&#223;', '&#224;', '&#225;', '&#226;', '&#227;', '&#228;', '&#229;', '&#230;', '&#231;', '&#232;', '&#233;', '&#234;', '&#235;', '&#236;', '&#237;', '&#238;', '&#239;', '&#240;', '&#241;', '&#242;', '&#243;', '&#244;', '&#245;', '&#246;', '&#247;', '&#248;', '&#249;', '&#250;', '&#251;', '&#252;', '&#253;', '&#254;', '&#255;', '&#34;', '&#38;', '&#60;', '&#62;', '&#338;', '&#339;', '&#352;', '&#353;', '&#376;', '&#710;', '&#732;', '&#8194;', '&#8195;', '&#8201;', '&#8204;', '&#8205;', '&#8206;', '&#8207;', '&#8211;', '&#8212;', '&#8216;', '&#8217;', '&#8218;', '&#8220;', '&#8221;', '&#8222;', '&#8224;', '&#8225;', '&#8240;', '&#8249;', '&#8250;', '&#8364;', '&#402;', '&#913;', '&#914;', '&#915;', '&#916;', '&#917;', '&#918;', '&#919;', '&#920;', '&#921;', '&#922;', '&#923;', '&#924;', '&#925;', '&#926;', '&#927;', '&#928;', '&#929;', '&#931;', '&#932;', '&#933;', '&#934;', '&#935;', '&#936;', '&#937;', '&#945;', '&#946;', '&#947;', '&#948;', '&#949;', '&#950;', '&#951;', '&#952;', '&#953;', '&#954;', '&#955;', '&#956;', '&#957;', '&#958;', '&#959;', '&#960;', '&#961;', '&#962;', '&#963;', '&#964;', '&#965;', '&#966;', '&#967;', '&#968;', '&#969;', '&#977;', '&#978;', '&#982;', '&#8226;', '&#8230;', '&#8242;', '&#8243;', '&#8254;', '&#8260;', '&#8472;', '&#8465;', '&#8476;', '&#8482;', '&#8501;', '&#8592;', '&#8593;', '&#8594;', '&#8595;', '&#8596;', '&#8629;', '&#8656;', '&#8657;', '&#8658;', '&#8659;', '&#8660;', '&#8704;', '&#8706;', '&#8707;', '&#8709;', '&#8711;', '&#8712;', '&#8713;', '&#8715;', '&#8719;', '&#8721;', '&#8722;', '&#8727;', '&#8730;', '&#8733;', '&#8734;', '&#8736;', '&#8743;', '&#8744;', '&#8745;', '&#8746;', '&#8747;', '&#8756;', '&#8764;', '&#8773;', '&#8776;', '&#8800;', '&#8801;', '&#8804;', '&#8805;', '&#8834;', '&#8835;', '&#8836;', '&#8838;', '&#8839;', '&#8853;', '&#8855;', '&#8869;', '&#8901;', '&#8968;', '&#8969;', '&#8970;', '&#8971;', '&#9001;', '&#9002;', '&#9674;', '&#9824;', '&#9827;', '&#9829;', '&#9830;'],

	isEmpty : function(val){
		if(val){
			return ((val===null) || val.length==0 || /^\s+$/.test(val));
		}else{
			return true;
		}
	},
	// Convert HTML entities into numerical entities
	HTML2Numerical : function(s){
	    return this.swapArrayVals(s, this.arr1, this.arr2);
	},	

	// Convert Numerical entities into HTML entities
	NumericalToHTML : function(s){
	    return this.swapArrayVals(s, this.arr2, this.arr1);
	},


	// Numerically encodes all unicode characters
	numEncode : function(s){
		
		if(this.isEmpty(s)) return "";

		var e = "";
		for (var i = 0; i < s.length; i++)
		{
			var c = s.charAt(i);
			if (c < " " || c > "~")
			{
				c = "&#" + c.charCodeAt() + ";";
			}
			e += c;
		}
		return e;
	},
	
	// HTML Decode numerical and HTML entities back to original values
	htmlDecode : function(s){

		var c,m,d = s;
		
		if(this.isEmpty(d)) return "";

		// convert HTML entites back to numerical entites first
		d = this.HTML2Numerical(d);
		
		// look for numerical entities &#34;
		arr=d.match(/&#[0-9]{1,5};/g);
		
		// if no matches found in string then skip
		if(arr!=null){
			for(var x=0;x<arr.length;x++){
				m = arr[x];
				c = m.substring(2,m.length-1); //get numeric part which is refernce to unicode character
				// if its a valid number we can decode
				if(c >= -32768 && c <= 65535){
					// decode every single match within string
					d = d.replace(m, String.fromCharCode(c));
				}else{
					d = d.replace(m, ""); //invalid so replace with nada
				}
			}			
		}

		return d;
	},		

	// encode an input string into either numerical or HTML entities
	htmlEncode : function(s,dbl){
			
		if(this.isEmpty(s)) return "";

		// do we allow double encoding? E.g will &amp; be turned into &amp;amp;
		dbl = dbl || false; //default to prevent double encoding
		
		// if allowing double encoding we do ampersands first
		if(dbl){
			if(this.EncodeType=="numerical"){
				s = s.replace(/&/g, "&#38;");
			}else{
				s = s.replace(/&/g, "&amp;");
			}
		}

		// convert the xss chars to numerical entities ' " < >
		s = this.XSSEncode(s,false);
		
		if(this.EncodeType=="numerical" || !dbl){
			// Now call function that will convert any HTML entities to numerical codes
			s = this.HTML2Numerical(s);
		}

		// Now encode all chars above 127 e.g unicode
		s = this.numEncode(s);

		// now we know anything that needs to be encoded has been converted to numerical entities we
		// can encode any ampersands & that are not part of encoded entities
		// to handle the fact that I need to do a negative check and handle multiple ampersands &&&
		// I am going to use a placeholder

		// if we don't want double encoded entities we ignore the & in existing entities
		if(!dbl){
			s = s.replace(/&#/g,"##AMPHASH##");
		
			if(this.EncodeType=="numerical"){
				s = s.replace(/&/g, "&#38;");
			}else{
				s = s.replace(/&/g, "&amp;");
			}

			s = s.replace(/##AMPHASH##/g,"&#");
		}
		
		// replace any malformed entities
		s = s.replace(/&#\d*([^\d;]|$)/g, "$1");

		if(!dbl){
			// safety check to correct any double encoded &amp;
			s = this.correctEncoding(s);
		}

		// now do we need to convert our numerical encoded string into entities
		if(this.EncodeType=="entity"){
			s = this.NumericalToHTML(s);
		}

		return s;					
	},

	// Encodes the basic 4 characters used to malform HTML in XSS hacks
	XSSEncode : function(s,en){
		if(!this.isEmpty(s)){
			en = en || true;
			// do we convert to numerical or html entity?
			if(en){
				s = s.replace(/\'/g,"&#39;"); //no HTML equivalent as &apos is not cross browser supported
				s = s.replace(/\"/g,"&quot;");
				s = s.replace(/</g,"&lt;");
				s = s.replace(/>/g,"&gt;");
			}else{
				s = s.replace(/\'/g,"&#39;"); //no HTML equivalent as &apos is not cross browser supported
				s = s.replace(/\"/g,"&#34;");
				s = s.replace(/</g,"&#60;");
				s = s.replace(/>/g,"&#62;");
			}
			return s;
		}else{
			return "";
		}
	},

	// returns true if a string contains html or numerical encoded entities
	hasEncoded : function(s){
		if(/&#[0-9]{1,5};/g.test(s)){
			return true;
		}else if(/&[A-Z]{2,6};/gi.test(s)){
			return true;
		}else{
			return false;
		}
	},

	// will remove any unicode characters
	stripUnicode : function(s){
		return s.replace(/[^\x20-\x7E]/g,"");
		
	},

	// corrects any double encoded &amp; entities e.g &amp;amp;
	correctEncoding : function(s){
		return s.replace(/(&amp;)(amp;)+/,"$1");
	},


	// Function to loop through an array swaping each item with the value from another array e.g swap HTML entities with Numericals
	swapArrayVals : function(s,arr1,arr2){
		if(this.isEmpty(s)) return "";
		var re;
		if(arr1 && arr2){
			//ShowDebug("in swapArrayVals arr1.length = " + arr1.length + " arr2.length = " + arr2.length)
			// array lengths must match
			if(arr1.length == arr2.length){
				for(var x=0,i=arr1.length;x<i;x++){
					re = new RegExp(arr1[x], 'g');
					s = s.replace(re,arr2[x]); //swap arr1 item with matching item from arr2	
				}
			}
		}
		return s;
	},

	inArray : function( item, arr ) {
		for ( var i = 0, x = arr.length; i < x; i++ ){
			if ( arr[i] === item ){
				return i;
			}
		}
		return -1;
	}

}

function OculatarHerramientaAddto()
{
    if($.browser.msie && $.browser.version < 7)
    {
        idIntervalo = setInterval("accederWeb()",500);
    }
}

function CambiarNombre(link, nombre1, nombre2)
{
    if(link.innerHTML == nombre1)
    {
        link.innerHTML = nombre2;
    }
    else
    {
        link.innerHTML = nombre1;
    }
}


var panelFicherosDisponibles = {
	idSection: '#section',
	cssDescripcion: '.descripcion',
	cssHeader: '.descripcion .header',
	cssPanelDesplegable: '.panel',
	idPanel: '#panelFicherosDisponibles',
	desplegable: '',
	enlace: '',
	linkPDFEs: 'gnossOnto:linkPDFEs',
	linkPDFEn: 'gnossOnto:linkPDFEn',
	cssFicheroPdf: '.isPdf',
	enlaces: [],
	literales: [],
	init: function(){
		this.config();
		this.crear();
		this.configPanel();
		this.ficheros();
		this.escribirFicheros();
		this.enganchar();
		return;
	},
	config: function(){
		this.section = $(this.idSection);
		this.header = $(this.cssHeader, this.section);
		return;
	},
	ficheros: function(){
		var ficheros = $(this.cssFicheroPdf, this.section);
		var enlaces = [];
		var literales = [];
		ficheros.each(function(){
			enlaces.push($('a', this));
			literales.push($(this).prev().html());
		});
		this.enlaces = enlaces;
		this.literales = literales;
		return;
	},
	escribirFicheros: function(){
		var enlace;
		var href;
		var html;		
		var lis = '';
		var that = this;
		$(this.enlaces).each(function(indice){
			enlace = that.enlaces[indice];
			enlace = $(enlace);
			href = enlace.attr('href');
			html = enlace.html();		
			lis += '<li>' + that.literales[indice] + ' <a href="' + href + '">' + html + '</a></li>';
		});
		this.ul.html(lis);
		return;
	},
	crear: function(){
		var encabezado = $('h3', this.header);
		encabezado.after(panelFicherosDisponibles.template());
		return;
	},
	configPanel: function(){
		this.panel = $(this.idPanel, this.section);
		this.desplegable = $(this.cssPanelDesplegable, this.panel);
		this.enlace = $('.pdf a', this.panel);
		this.ul = $('ul', this.panel);
		return;
	},	
	enganchar: function(){
		var that = this;
		this.enlace.bind('click', function(evento){
			evento.preventDefault();
			that.comportamiento(evento);
			return false;
		});
		return;
	},
	comportamiento: function(evento){
		this.desplegable.is(':visible')? this.desplegable.hide():this.desplegable.show();
	},
	template: function(){
		var html = '<div id="panelFicherosDisponibles">\n';
		html += '<p class="pdf"><a href="/">'+ form.fichaTecnicaPDF +'<\/a><\/p>';
		html += '<div class="panel">\n';	
		html += '<ul>\n';	
		html += '<li>No hay ficheros disponibles<\/li>\n';	
		html += '<\/ul>\n';	
		html += '<\/div>\n';	
		html += '<\/div>\n';	
		return html;
	}
};

$(function(){
	var ficheros = $('#section .isPdf');
	var isFicherosDisponibles = ficheros.length > 0;
	if(isFicherosDisponibles) panelFicherosDisponibles.init();
})/**/ 
/*custom.js*/ 
$( function() {
    $('input.noLabel[type=text]').focus( function() {
        var $this = $(this);
        this.cache = this.cache || $this.val(); // si la cache del input esta vacia la llenamos
        $this.val('');
    }).blur( function() {
        this.value = this.value || this.cache;
    });
    $('input.password.noLabel').focus( function() {
        $(this).addClass('noBgImg');
    }).blur( function() {
    if (!this.value) {
            $(this).removeClass('noBgImg');
        }
    });
    
    // Correccion bordes de input para IE
    $('input').filter('[type=radio], [type=checkbox]').css({
		border: 0,
		padding: 0,
		marginTop: 0
    });

    /* Definimos globalmente los desplegables.
     * Para montar un desplegable basta con definir un enlace que sirva de boton (a.desplegable)
     * y una capa con clase panel (div.panel). OJO, si se usa cualquiera de los dos elementos fuera
     * de su estructura preparada se producira un error de JS y un alert(...)
     */
	 
	//LOZA : Para los desplegables, es recomendable usar la funcion "Desplegar(Boton, PanelId)" en el onclick del enlace que despliega. (Ver ejemplo en BandejaBorradores.aspx)
    //crearDesplegables('a.desplegable', 'div.panel',{efecto:'blind', opciones:{direction:'vertical'}});
    crearDesplegables('#desplegarMenu', '#menuLateral',{velocidad: 300});
    //crearPestanyas('ul.pestanyas li', 'div.pestanya',{efecto:'slide', velocidad:600, opciones:{direction:'up'}})
	
	// cambiar la url mediante el select
	$('select.cambiarListado').change(function(){
		window.location=this.value;
	});
	
	// filtros de busqueda rapida
	$('.filtroRapido').find('input').keyup( function() {
		var $this = $(this);
		if (this.value.length > 2) {
			$this.addClass('activo');
		} else {
			$this.removeClass('activo');
		}
	});
	
	// duplicar campos de registro de organizacion
	$('div.agregarCampo').find('a').click(function() {
		var $campo = $(this).parents('div.agregarCampo').eq(0).find('input:last');
		$campo.clone().insertAfter($campo);
		return false;
	});
	
	// PSEUDO FINDER
	$.fn.extend({
		reajustar:function(){// funcion interna para recalcular las alturas del finder y el scroll interno
		    return this.each(function(){
		    	var h=0;
		    	var $mascara=$(this);
		    	var anchura = $mascara.children('div').width()-1;
		    	var niveles=$mascara.find('li.activo').length||1;
		    	$mascara.find('div:visible').each(function() {
		    		var hTemp=0;
		    		$(this).children().each( function() {
		    			var $this=$(this);
		    			hTemp+=parseFloat($this.css('paddingTop'));
		    			hTemp+=parseFloat($this.height());
		    			hTemp+=parseFloat($this.css('paddingBottom'));
		    		});
		    		h=(hTemp>h)?hTemp:h;
		    	});
			    $mascara.animate({height:h+'px'},200)
			    .children('div').eq(0).animate({left:-anchura*(niveles-1)+'px'}, 600);
			    if (niveles > 1) {
			    	$mascara.find('a.volver').fadeIn(200);
			    } else {
			    	$mascara.find('a.volver').fadeOut(200);
			    }
		    });
		},
	    desactivar:function(){
	    	return this.each(function(){
	    		$(this).find('li').andSelf().removeClass('activo')
	    		    .children().removeClass('activo')
	    		    .siblings('div').hide();
	    	});
	    }
	});
	$('div.pseudoFinder').each(function() {
		var $mascara = $(this);
		// mostrar un nivel inferior
		$mascara.find('div').siblings('a:not(.volver)').click( function() {
			var $li = $(this).parent();
			var $div = $li.children('div').show();
			if ($li.hasClass('activo')) return false; //si ya esta activado pasamos de todo
			// desactivamos los hermanos, reasignamos clases y reajuste de la capa
			$li.siblings('.activo').desactivar();
			$li.children('a').andSelf().addClass('activo');
			$mascara.reajustar();
			return false;
		});
		// enchufar en el target y realizar accion final
		$mascara.find('a:only-child').css('backgroundImage','none').click(function(){
			var sHtml;
			var prime=$mascara.find('a.activo:first').text();
			var ulti=$(this).text();
			prime=(prime.length>50)?prime.substring(0,47)+'...':prime;
			ulti=(ulti.length>50)?ulti.substring(0,47)+'...':ulti;
			sHtml=['<tr><th scope="row">',prime,'</th><td>',ulti,'</td><td><img src="img/blank.gif" alt="eliminar"/></td></tr>'].join('');
			$mascara.next('.targetPseudoFinder').show() //mostramos
			.find('tbody').append(sHtml) //metemos el HTML
			.find('img:last').click(function(){ // preparamos el evento de eliminar
				var $capa=$(this).parents('div').eq(1);
				$(this).parents('tr:first').remove();
				if (!$capa.find('tr').length) {
					$capa.slideUp();
				}
			});
			return false;
		});
		// volver atras
		$mascara.find('a.volver').click(function(){
			$mascara.find('li.activo:last').desactivar().end().reajustar();
			return false;
		});
	});
	
	// selector de #baseRecursos
	$('#baseRecursos div.listadoCategorias a').click(function() {
		$('#baseRecursos div.listadoCategorias a').removeClass('activo');
		$(this).addClass('activo');
		return false;
	});
	
	
	// desplegables de base de recursos
	$('#baseRecursos h3+div.panel a').click(function(){
		$(this).parents('.panel:first').prev().find('a').click();
	});
	
	// inputs condicionados a un select de "tipo de documentos" en el apartado de subir recursos
	$('#seleccionarRecurso,#tipoRecurso').find('select:eq(0)').change(function() {
		var $this = $(this);
		$this.find('option').each(function(){
			$('#'+this.value).hide();
		});
		$('#'+$this.val()).show();
	}).change();
	
	// CURRICULUM VITAE
	var checkearDatosMyGnoss = function(){
		var $this = $('#datosMyGnoss');
		if ($this.is(':checked')) {
			$this.parents('.cajaDestacadaLila:first').find('input:text').attr('disabled','disabled');
		} else {
			$this.parents('.cajaDestacadaLila:first').find('input:text').attr('disabled',false);
		}
	}
	// asignamos el evento al cargar y al pinchar en el checkbox
	$('#datosMyGnoss').click(checkearDatosMyGnoss);
	checkearDatosMyGnoss();
});

function Desplegar(pBoton, pPanel) {
    var boton   = $(pBoton),
    panel = $(document.getElementById(pPanel));
	boton.toggleClass('activo');
	panel.toggle();
	return false;
}

function MostrarImgFactorDafo(pPanel, pBaseURL, pClaveDafo, pClaveFactorDafo, pEsFactorNuevo, pRandom)
{
	panel = document.getElementById(pPanel + '_panel');
	if(panel.className == "noCargado")
	{
		img = document.getElementById(pPanel + '_grafico');
		var srcImg = pBaseURL + "/DafoFactorGraficoVotos.aspx?DafoID=" + pClaveDafo + "&FactorID=" + pClaveFactorDafo + "&FactorNuevo=" + pEsFactorNuevo + "&nocache=" + pRandom;
		img.src = srcImg;
		panel.className = "cargado"
	}
	DesplegarPanel(pPanel + '_panel');
}

function DesplegarTreeView(pImagen, pPanel) {
    var imagen   = $(pImagen);
    if(pImagen.src.indexOf('verMas')>0){
        pImagen.src = pImagen.src.replace('verMas','verMenos');
    }
    else{
        pImagen.src = pImagen.src.replace('verMenos','verMas');
    }
    DesplegarPanel(pPanel);
}

function marcarDespTreeView(pImagen,pIdTxt,pClave){   
    mTxt = document.getElementById(pIdTxt);
    if(pImagen.src.indexOf('verMas')>0){
        mTxt.value = mTxt.value.replace(pClave + ',','');
    }
    else{
        mTxt.value = mTxt.value + pClave + ',';
    }
}

function marcarElementoTreeView(pCheck, pIdTxt, pClave) {
    mTxt = $('#' + pIdTxt);

    if ($(pCheck).is(':checked')) {
        mTxt.val(mTxt.val() + pClave + ',');
    }
    else {
        mTxt.val(mTxt.val().replace(pClave + ',', ''));
    }
}

function marcarElementoSelCat(pCheck,pIdTxt,pClave){
    marcarElementoTreeView(pCheck, pIdTxt, pClave);
    try
    {
        $('#divSelCatLista').find('span.'+pClave+' input').attr('checked', $(pCheck).is(':checked'));
        $('#divSelCatTesauro').find('span.' + pClave + ' input').attr('checked', $(pCheck).is(':checked'));
    } 
    catch(Exception){
    }
}

function marcarSoloUnElementoSelCat(pCheck,pIdTxt,pClave){
    marcarElementoTreeView(pCheck, pIdTxt, pClave);

    try {
        $('#divSelCatLista').find('span.' + pClave + ' input').attr('checked', $(pCheck).is(':checked'));
        $('#divSelCatTesauro').find('span.' + pClave + ' input').attr('checked', $(pCheck).is(':checked'));
    }
    catch (Exception) {
    }
}

function marcarElementoSelCatEHijos(pCheck,pIdTxt,pClave)
{
    marcarElementoTreeView(pCheck, pIdTxt, pClave)

    try {
        $('#divSelCatLista').find('span.' + pClave + ' input').attr('checked', $(pCheck).is(':checked'));
        $('#divSelCatTesauro').find('span.' + pClave + ' input').attr('checked', $(pCheck).is(':checked'));
    }
    catch (Exception) {
    }
    
    var hijos = $(pCheck.parentNode.parentNode).children('div').children('div').children('span').children('input');
    hijos.each(
        function (){
            if ($(pCheck).is(':checked') != $(this).is(':checked')) {
                this.click();
            }
        }
    )
}

function filtrarListaSelCat(txt){
    cadena = txt.value;

    if(cadena.length>2){
        //Volvemos a inicializar todo
        try
        {
            for(i=0;i<CatEscondidas.length;i++){
                CatEscondidas[i].style.display = 'block';
            }
        }
		catch(error)
		{
		}
        CatEscondidas = new Array();
        txt.className = "activo";
        //filtramos y guardamos los div que escondemos
        categorias = $('div#divSelCatLista div div');
        for(i=0;i<categorias.length;i++){
            cat = categorias[i];
            if(cat.style.display != 'none' && $(cat).find('span label')[0].innerHTML.toLowerCase().indexOf(cadena.toLowerCase())<0){
                CatEscondidas[CatEscondidas.length] = cat;
                cat.style.display = 'none';
            }
        }
        
    }
    else{
        //volvemos a mostrar los contactos escondidos
            for(i=0;i<CatEscondidas.length;i++){
                CatEscondidas[i].style.display = 'block';
            }
        CatEscondidas = new Array();
        txt.className = "";
    }
}

function marcarElementosSelProy(pCheck, pClave){

    var radioButtons = $('input:radio[name=tipo_' + pClave + ']', $(pCheck).closest('.proyectos'));
    var radio1 = radioButtons[0];
    var radio2 = radioButtons[1];

    if ($(pCheck).is(':checked')) {
        if (!$(radio1).is(':checked') && !$(radio2).is(':checked')) {
            $(radio1).prop('checked', true);
        }
        $(radio1).parent().show();
        $(radio2).parent().show()
    }
    else {
        $(radio1).parent().hide();
        $(radio2).parent().hide();
    }
    
}

function filtrarListaSelProy(txtBox){    
    filtro = txtBox.value;
    filtro=filtro.toLowerCase();
    filtro.replace(/á/g,'a').replace(/é/g,'e').replace(/í/g,'i').replace(/ó/g,'o').replace(/ú/g,'u');
    if(filtro.length>2){
        txtBox.className = "activo";        
        proyectos=$('tr.proyectos');       
        for(i=0;i<proyectos.length;i++)
        {
            proy = proyectos[i]; 
            textoProy=$(proy).find('td span')[0].innerHTML.toLowerCase().replace(/á/g,'a').replace(/é/g,'e').replace(/í/g,'i').replace(/ó/g,'o').replace(/ú/g,'u');
            if(textoProy.indexOf(filtro.toLowerCase())<0){
                proy.style.display = 'none';
            }else
            {
                proy.style.display = '';
            }
        }
    }else
    {
        txtBox.className = "";
        proyectos=$('tr.proyectos');
        for(i=0;i<proyectos.length;i++)
        {
            proyectos[i].style.display='';
        }
    }
}


function marcarElementosSelGrupAmigos(pCheck,pClave,pIdTxt,pSoloUnCheck){
    grupos=$('span.checkGrupo input');
    var txtAmiSel=document.getElementById(pIdTxt);
    if(pSoloUnCheck=='True')
    {
        var estaCheck=false;
        
        if ($(pCheck.childNodes[0]).is(':checked')) {
            estaCheck=true;
        }
        
        for(i=0;i<grupos.length;i++) {
            $(grupos[i]).attr('checked', false);
        }
        txtAmiSel.value="";
        $(pCheck.childNodes[0]).attr('checked', estaCheck);
    }
    if ($(pCheck.childNodes[0]).is(':checked')) {
        txtAmiSel.value=txtAmiSel.value + pClave + '|';
    }else
    {
        txtAmiSel.value = txtAmiSel.value.replace(pClave + '|','');
    }
}


function getElementPosition(elemID) {
    var offsetTrail = document.getElementById(elemID);
    var offsetLeft = 0;
    var offsetTop = 0;
    while (offsetTrail)
    {
        offsetLeft += offsetTrail.offsetLeft;
        offsetTop += offsetTrail.offsetTop;
        offsetTrail = offsetTrail.offsetParent;
    }
    if (navigator.userAgent.indexOf("Mac") != -1 && typeof document.body.leftMargin != "undefined" && navigator.appName=="Microsoft Internet Explorer" ) 
    {
        offsetLeft += parseInt(document.body.leftMargin);
        offsetTop += parseInt(document.body.topMargin);
    }
    return {left:offsetLeft, top:offsetTop};
} 

/**********  REGION JAVIER  **************/

function CheckDocEntidadPrimerNivel_Click(pElementoID, pTxtSeleccionadosID, pTxtCatDocumentacionID)
{
    if ($('#catPrimerNivel_' + pElementoID).is(':checked'))
    {
        DesmarcarDesHabilitarElementosPrimerNivelMenosUno(pElementoID, pTxtSeleccionadosID);
        
        document.getElementById(pTxtCatDocumentacionID).value = pElementoID;
    }
    else
    {
        var check = document.getElementById('catPrimerNivel_' + pElementoID);
        DesmarcarHijosElemento(check, pTxtSeleccionadosID);
        
        HabilitarTodosLosElementosArbol();
        
        document.getElementById(pTxtCatDocumentacionID).value = '';
    }
}

function DesHabilitarElementosNoSeleccionados(pElementoID)
{
    if (pElementoID != '00000000-0000-0000-0000-000000000000')
    {
        var checks = $('#divSelCatTesauro').find('input.catPrimerNivel');
        var i=0
        for (i=0;i<checks.length;i++)
        {
            if (checks[i].id.indexOf(pElementoID) == -1)
            {
                DesHabilitarElementoEHijos(checks[i]);
            }
        }
    }
}

function DesHabilitarElementoEHijos(pCheck)
{
    pCheck.disabled = true;
            
    var hijos = $(pCheck.parentNode.parentNode).children('div').children('div').children('span').children('input');
    var i =0;
    for (i=0; i<hijos.length;i++)
    {
       DesHabilitarElementoEHijos(hijos[i]);
    } 
}

function DesmarcarHijosElemento(pCheck, pTxtSeleccionadosID)
{
    var idElemento = pCheck.parentNode.className;
    $(pCheck).attr('checked', false);
    $('#' + pTxtSeleccionadosID).val($('#' + pTxtSeleccionadosID).val().replace(idElemento + ',', ''));
    
    var hijos = $(pCheck.parentNode.parentNode).children('div').children('div').children('span').children('input');
    var i =0;
    for (i=0; i<hijos.length;i++)
    {
       DesmarcarHijosElemento(hijos[i], pTxtSeleccionadosID);
    }
}

function DesmarcarDesHabilitarElementoEHijos(pCheck, pTxtSeleccionadosID)
{
    $(pCheck).attr('checked', false);
    $(pCheck).attr('disabled', true);

    var idElemento = pCheck.parentNode.className;

    $('#' + pTxtSeleccionadosID).val($('#' + pTxtSeleccionadosID).val().replace(idElemento + ',', ''));
            
    var hijos = $(pCheck.parentNode.parentNode).children('div').children('div').children('span').children('input');
    var i =0;
    for (i=0; i<hijos.length;i++)
    {
       DesmarcarElementoEHijos(hijos[i], pTxtSeleccionadosID);
    } 
}

function HabilitarTodosLosElementosArbol()
{
    var checks = $('#divSelCatTesauro').find('input.catPrimerNivel');
    for (i=0;i<checks.length;i++)
    {
        HabilitarElementoEHijos(checks[i]);
    }
}

function HabilitarElementoEHijos(pCheck)
{
    pCheck.disabled = false;
    
    var idElemento = pCheck.parentNode.className;    
    var hijos = $(pCheck.parentNode.parentNode).children('div').children('div').children('span').children('input');
    var i =0;
    for (i=0; i<hijos.length;i++)
    {
       HabilitarElementoEHijos(hijos[i]);
    } 
}

function DesmarcarDesHabilitarElementosPrimerNivelMenosUno(pElementoID, pTxtSeleccionadosID){
    var checks = $('#divSelCatTesauro').find('input.catPrimerNivel');
    var i=0
    for (i=0;i<checks.length;i++)
    {
        if (checks[i].id.indexOf(pElementoID) == -1)
        {
            DesmarcarDesHabilitarElementoEHijos(checks[i], pTxtSeleccionadosID);
        }
    }
}

function DesmarcarDesHabilitarElementoEHijos(pCheck, pTxtSeleccionadosID)
{
    $(pCheck).attr('checked', false);
    $(pCheck).attr('disabled', true);

    var idElemento = pCheck.parentNode.className;
    $('#' + pTxtSeleccionadosID).val($('#' + pTxtSeleccionadosID).val().replace(idElemento + ',', ''));
            
    var hijos = $(pCheck.parentNode.parentNode).children('div').children('div').children('span').children('input');
    var i =0;
    for (i=0; i<hijos.length;i++)
    {
       DesmarcarDesHabilitarElementoEHijos(hijos[i], pTxtSeleccionadosID);
    } 
}


/* ****    FIN REGION JAVIER   ***********/


//    if(cadena.length>2){
//        //Volvemos a inicializar todo
//            for(i=0;i<CatEscondidas.length;i++){
//                CatEscondidas[i].style.display = 'block';
//            }
//        CatEscondidas = new Array();
//        txt.className = "activo";
//        //filtramos y guardamos los div que escondemos
//        categorias = $('div#divSelCatLista div div');
//        for(i=0;i<categorias.length;i++){
//            cat = categorias[i];
//            if(cat.style.display != 'none' && $(cat).find('span label')[0].innerHTML.toLowerCase().indexOf(cadena.toLowerCase())<0){
//                CatEscondidas[CatEscondidas.length] = cat;
//                cat.style.display = 'none';
//            }
//        }
//        
//    }
//    else{
//        //volvemos a mostrar los contactos escondidos
//            for(i=0;i<CatEscondidas.length;i++){
//                CatEscondidas[i].style.display = 'block';
//            }
//        CatEscondidas = new Array();
//        txt.className = "";
//    }






//function DesplegarImgMas(pBoton,pPanel){
//    alert('Despliego');
//    var boton   = $(pBoton),
//    panel = $(document.getElementById(pPanel));
//    var img = boton.children();
//    var img2 = pBoton.firstChild;
//    alert(img2.attributes);    
//    if(panel.attr('style') == 'display: none;'){
//        
//        img.attr({ alt:'-', src:img.attr('src').replace('Mas','Menos') });
//        img.attr('alt') = '-';
//        alert('Cambio de mas a menos');
//    }
//    else{
//        img.attr({ alt:'+', src:img.attr('src').replace('Menos','Mas') });
//        alert('Cambio de menos a mas');
//    }
//    o = {efecto:'blind', opciones:{direction:'vertical'}};
//    if (!o.efecto) {
//        o.efecto = 'blind';
//        o.opciones =  {direction: 'vertical'};
//    }
//    
//			boton.toggleClass('activo');
//            panel.toggle(o.efecto, o.opciones, o.velocidad, o.callback);
//            return false;
//}

function DesplegarPanel(pPanel) {
            //var $panel = $(document.getElementsByName(pPanel)[0]);
            var $panel = $(document.getElementById(pPanel));
			if ($panel[0].style.display == 'none') {
			    $panel.show();
				//$panel['show']('blind', {direction:'vertical'}, 'fast');
			}
			else {
			    $panel.hide();
				//$panel['hide']('blind', {direction:'vertical'}, 'fast');
			}
			return false;
		}
		
function DesplegarOcultarPaneles(pPanel1, pPanel2) {
        var $panel = $(document.getElementById(pPanel1));
        var $panel2 = $(document.getElementById(pPanel2));
		if ($panel[0].style.display == 'none') {
		    $panel.show();
			//$panel['show']('blind', {direction:'vertical'}, 'fast');
		}
		else {
		    $panel.hide();
			//$panel['hide']('blind', {direction:'vertical'}, 'fast');
		}
		if ($panel2[0].style.display == 'none') {
		    $panel2.show();
			//$panel2['show']('blind', {direction:'vertical'}, 'fast');
		}
		else
		{
			$panel2.hide();
            //$panel2['hide']('blind', {direction:'vertical'}, 'fast');
		}
		return false;
		}
		
function DesplegarImgMas(pBoton,pPanel) {
            var $boton   = $(pBoton);
            var $panel = $(document.getElementById(pPanel));
            var $img = $boton.find('img');
            if($img.length == 0)
            {
                $img = $boton;
            }
            var source = $img.attr('src');
			if ('+' == $img.attr('alt')) {
				func = 'show';
				$img.attr({ alt:'-', src:source.replace('Mas','Menos') });
			} else if ('-' == $img.attr('alt')) {
				func = 'hide';
				$img.attr({ alt:'+', src:source.replace('Menos','Mas') });
			}
            if ($panel[0].tagName.toLowerCase() == 'div') {
				$panel[func]('blind', {direction:'vertical'}, 'fast');				
			} else {
				$panel[func]();
			}
			return false;
		}
		
/*   LOZA : funcion para desplegar el texto tipo ver mas */
function DesplegarDescripcionMas(pBoton,pPanel,pPanelReducido) {
            var $boton   = $(pBoton);
            var $panel = $(document.getElementById(pPanel));
            var $panelReducido = $(document.getElementById(pPanelReducido));
            var $img = $boton.find('img');
            if($img.length == 0)
            var source = $img.attr('src');
			if ('+' == $img.attr('alt')) {
			    //$panelReducido['hide']('blind', {direction:'vertical'}, 'fast');
			    document.getElementById(pPanelReducido).style.display = 'none';
				$panel.show('slow');
			} else if ('-' == $img.attr('alt')) {
			    $panel.hide('slow');
				//$panelReducido['show']('blind', {direction:'vertical'}, 'fast');
				document.getElementById(pPanelReducido).style.display = 'block';
			}
			return false;
}

/*David: Funcion para mostrar y ocultar dos paneles uno debajo de otro*/
function MostrarOcultarPanel(pPanel1,pPanel2)
{
    if(document.getElementById(pPanel1).style.display == 'none')
    {
        document.getElementById(pPanel1).style.display = 'block';
        document.getElementById(pPanel2).style.display = 'none';
    }
    else
    {
        document.getElementById(pPanel1).style.display = 'none';
        document.getElementById(pPanel2).style.display = 'block';
    }
	return false;
}

/*  LOZA : Funcion para desplegar panel desde img +   */
function DesplegarImgMasEnSpan(pBoton, pPanel) {
    var boton   = $(pBoton),
    panel = $(document.getElementById(pPanel));
        o = {efecto:'blind', opciones:{direction:'vertical'}};
    if (!o.efecto) {
        o.efecto = 'blind';
        o.opciones =  {direction: 'vertical'};
    }
    
	boton.toggleClass('activo');
    panel.toggle(o.efecto, o.opciones, o.velocidad, o.callback);
    
    var $botonOtro = $(pBoton)
    var $img = $botonOtro.find('img');
    var source = $img.attr('src');
    if ('+' == $img.attr('alt')) 
    {
		$img.attr({ alt:'-', src:source.replace('Mas','Menos') });
	} else if ('-' == $img.attr('alt')) {
		$img.attr({ alt:'+', src:source.replace('Menos','Mas') });
	}
    return false;
}

function DesplegarImgMasEnSpanSinAnimacion(pBoton, pPanel) {
    panel = (document.getElementById(pPanel));
    if (panel.style.display == 'none')
    {
        panel.style.display = 'inline';
    }
    else
    {
        panel.style.display = 'none';
    }
    
    var $botonOtro = $(pBoton)
    var $img = $botonOtro.find('img');
    var source = $img.attr('src');
    if ('+' == $img.attr('alt')) 
    {
		$img.attr({ alt:'-', src:source.replace('Mas','Menos') });
	} else if ('-' == $img.attr('alt')) {
		$img.attr({ alt:'+', src:source.replace('Menos','Mas') });
	}
    return false;
}


// drag and drop DAFO
function HabilitarDragDrop(){
	$("ol.dragDrop").sortable({
	    stop: function(event, ui) {
		   var padre = $(ui.item[0]).parent(),
		   hijos = padre.children();
		   hijos.removeClass('odd').removeClass('even');
		   hijos.filter(':odd').addClass('even');
		   hijos.filter(':even').addClass('odd');
		   
		   //Modificado por fernando
		   var i = 0;
		   var orden = "";
		   for(i; i<hijos.length; i++)
		   {
				orden += hijos[i].id + ",";
		   }
		   document.getElementById('ctl00_ctl00_CPH1_CPHContenido_txtHackOrdenFactores').value = orden;
		   eval(document.getElementById('ctl00_ctl00_CPH1_CPHContenido_lbHackOrdenFactores').href);
		},
		handle: 'span.dragDrop'
	});
}

InsertarScriptVotacionDafo();

//Inserta el script que cambia los radio buttons de la ficha de votación del dafo
//por el control de votación con los puntos y la X de cancelar voto
function InsertarScriptVotacionDafo()
{
//Sistema de votación DAFO

/*
 ### jQuery Star Rating Plugin v3.12 - 2009-04-16 ###
 * Home: http://www.fyneworks.com/jquery/star-rating/
 * Code: http://code.google.com/p/jquery-star-rating-plugin/
 *
	* Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 ###
*/

/*# AVOID COLLISIONS #*/
;if(window.jQuery) (function($){
/*# AVOID COLLISIONS #*/
	
	// IE6 Background Image Fix
	//if ($.browser.msie) try { document.execCommand("BackgroundImageCache", false, true)} catch(e) { };
	// Thanks to http://www.visualjquery.com/rating/rating_redux.html
	
	// plugin initialization
	$.fn.rating = function(options){
		if(this.length==0) return this; // quick fail
		
		// Handle API methods
		if(typeof arguments[0]=='string'){
			// Perform API methods on individual elements
			if(this.length>1){
				var args = arguments;
				return this.each(function(){
					$.fn.rating.apply($(this), args);
    });
			};
			// Invoke API method handler
			$.fn.rating[arguments[0]].apply(this, $.makeArray(arguments).slice(1) || []);
			// Quick exit...
			return this;
		};
		
		// Initialize options for this call
		var options = $.extend(
			{}/* new object */,
			$.fn.rating.options/* default options */,
			options || {} /* just-in-time options */
		);
		
		// Allow multiple controls with the same name by making each call unique
		$.fn.rating.calls++;
		
		// loop through each matched element
		this
		 .not('.star-rating-applied')
			.addClass('star-rating-applied')
		.each(function(){
			
			// Load control parameters / find context / etc
			var control, input = $(this);
			var eid = (this.name || 'unnamed-rating').replace(/\[|\]/g, '_').replace(/^\_+|\_+$/g,'');
			var context = $(this.form || document.body);
			
			// FIX: http://code.google.com/p/jquery-star-rating-plugin/issues/detail?id=23
			//var raters = context.data('rating');
			var raters = ( options.resetAll ? { count:0 } : context.data('rating') || { 
count:0 } );

			if(!raters || raters.call!=$.fn.rating.calls) raters = { count:0, call:$.fn.rating.calls };			
			var rater = raters[eid];
			options.resetAll = false;
			// if rater is available, verify that the control still exists
			if(rater) control = rater.data('rating');
			
			
			if(rater && control)//{// save a byte!
				// add star to control if rater is available and the same control still exists
				
				control.count++;
			//}// save a byte!
			else{
				// create new control if first star or control element was removed/replaced
				// Initialize options for this raters
				control = $.extend(
					{}/* new object */,
					options || {} /* current call options */,
					($.metadata? input.metadata(): ($.meta?input.data():null)) || {}, /* metadata options */
					{ count:0, stars: [], inputs: [] }
				);
				
				// increment number of rating controls
				control.serial = raters.count++;
				
				// create rating element
				rater = $('<span class="star-rating-control"/>');
				input.before(rater);
				
				// Mark element for initialization (once all stars are ready)
				rater.addClass('rating-to-be-drawn');
				
				// Accept readOnly setting from 'disabled' property
				if(input.attr('disabled')) control.readOnly = true;
				
				// Create 'cancel' button
				rater.append(
					control.cancel = $('<div class="rating-cancel"><a title="' + control.cancel + '">' + control.cancelValue + '</a></div>')
					.mouseover(function(){
						$(this).rating('drain');
						$(this).addClass('star-rating-hover');
						//$(this).rating('focus');
					})
					.mouseout(function(){
						$(this).rating('draw');
						$(this).removeClass('star-rating-hover');
						//$(this).rating('blur');
					})
					.click(function(){
					 $(this).rating('select');
					})
					.data('rating', control)
				);
				
			}; // first element of group
			
			// insert rating star
			var star = $('<div class="star-rating rater-'+ control.serial +'" style=" display:' + this.style["display"] + '"><a name="' + this.name + '" title="' + (this.title || this.value) + '">' + this.value + '</a></div>');
			rater.append(star);
			
			// inherit attributes from input element
			if(this.id) star.attr('id', this.id);
			if(this.className) star.addClass(this.className);
			
			// Half-stars?
			if(control.half) control.split = 2;
			
			// Prepare division control
			if(typeof control.split=='number' && control.split>0){
				var stw = ($.fn.width ? star.width() : 0) || control.starWidth;
				var spi = (control.count % control.split), spw = Math.floor(stw/control.split);
				star
				// restrict star's width and hide overflow (already in CSS)
				.width(spw)
				// move the star left by using a negative margin
				// this is work-around to IE's stupid box model (position:relative doesn't work)
				.find('a').css({ 'margin-left':'-'+ (spi*spw) +'px' })
			};
			
			// readOnly?
			if(control.readOnly)//{ //save a byte!
				// Mark star as readOnly so user can customize display
				star.addClass('star-rating-readonly');
			//}  //save a byte!
			else//{ //save a byte!
			 // Enable hover css effects
				star.addClass('star-rating-live')
				 // Attach mouse events
					.mouseover(function(){
						$(this).rating('fill');
						$(this).rating('focus');
					})
					.mouseout(function(){
						$(this).rating('draw');
						$(this).rating('blur');
					})
					.click(function(){
						$(this).rating('select');
					})
				;
			//}; //save a byte!
			
			// set current selection
			if($(this).is(':checked'))	control.current = star;
			
			// hide input element
			input.hide();
			
			// backward compatibility, form element to plugin
			input.change(function(){
    $(this).rating('select');
   });
			
			// attach reference to star to input element and vice-versa
			star.data('rating.input', input.data('rating.star', star));
			
			// store control information in form (or body when form not available)
			control.stars[control.stars.length] = star[0];
			control.inputs[control.inputs.length] = input[0];
			control.rater = raters[eid] = rater;
			control.context = context;
			
			input.data('rating', control);
			rater.data('rating', control);
			star.data('rating', control);
			context.data('rating', raters);
  }); // each element
		
		// Initialize ratings (first draw)
		$('.rating-to-be-drawn').rating('draw').removeClass('rating-to-be-drawn');
		
		return this; // don't break the chain...
	};
	
	/*--------------------------------------------------------*/
	
	/*
		### Core functionality and API ###
	*/
	$.extend($.fn.rating, {
		// Used to append a unique serial number to internal control ID
		// each time the plugin is invoked so same name controls can co-exist
		calls: 0,
		
		focus: function(){
			var control = this.data('rating'); if(!control) return this;
			if(!control.focus) return this; // quick fail if not required
			// find data for event
			var input = $(this).data('rating.input') || $( this.tagName=='INPUT' ? this : null );
   // focus handler, as requested by focusdigital.co.uk
			if(control.focus) control.focus.apply(input[0], [input.val(), $('a', input.data('rating.star'))[0]]);
		}, // $.fn.rating.focus
		
		blur: function(){
			var control = this.data('rating'); if(!control) return this;
			if(!control.blur) return this; // quick fail if not required
			// find data for event
			var input = $(this).data('rating.input') || $( this.tagName=='INPUT' ? this : null );
   // blur handler, as requested by focusdigital.co.uk
			if(control.blur) control.blur.apply(input[0], [input.val(), $('a', input.data('rating.star'))[0]]);
		}, // $.fn.rating.blur
		
		fill: function(){ // fill to the current mouse position.
			var control = this.data('rating'); if(!control) return this;
			// do not execute when control is in read-only mode
			if(control.readOnly) return;
			// Reset all stars and highlight them up to this element
			this.rating('drain');
			this.prevAll().andSelf().filter('.rater-'+ control.serial).addClass('star-rating-hover');
		},// $.fn.rating.fill
		
		drain: function() { // drain all the stars.
			var control = this.data('rating'); if(!control) return this;
			// do not execute when control is in read-only mode
			if(control.readOnly) return;
			// Reset all stars
			control.rater.children().filter('.rater-'+ control.serial).removeClass('star-rating-on').removeClass('star-rating-hover');
		},// $.fn.rating.drain
		
		draw: function(){ // set value and stars to reflect current selection
			var control = this.data('rating'); if(!control) return this;
			// Clear all stars
			this.rating('drain');
			// Set control value
			if(control.current){
				control.current.data('rating.input').attr('checked','checked');
				control.current.prevAll().andSelf().filter('.rater-'+ control.serial).addClass('star-rating-on');
			}
			else
			 $(control.inputs).removeAttr('checked');
			// Show/hide 'cancel' button
			control.cancel[control.readOnly || control.required?'hide':'show']();
			// Add/remove read-only classes to remove hand pointer
			this.siblings()[control.readOnly?'addClass':'removeClass']('star-rating-readonly');
		},// $.fn.rating.draw
		
		select: function(value){ // select a value
			var control = this.data('rating'); if(!control) return this;
			// do not execute when control is in read-only mode
			if(control.readOnly) return;
			
			//========================================================================================
			// LOZA: Añado esto para guardar el valor anterior antes de votar
			//========================================================================================
			var votoAnterior = '0';
			if(control.current != null) //Antes teniamos voto
			{
				votoAnterior = control.current[0].textContent;
			}
			//========================================================================================
			// LOZA: Fin de añadido
			//========================================================================================
			
			// clear selection
			control.current = null;
			// programmatically (based on user input)
			if(typeof value!='undefined'){
			 // select by index (0 based)
				if(typeof value=='number')
 			 return $(control.stars[value]).rating('select');
				// select by literal value (must be passed as a string
				if(typeof value=='string')
					//return 
					$.each(control.stars, function(){
						if($(this).data('rating.input').val()==value) $(this).rating('select');
					});
			}
			else
				control.current = this[0].tagName=='INPUT' ? 
				 this.data('rating.star') : 
					(this.is('.rater-'+ control.serial) ? this : null);
			
			// Update rating control state
			this.data('rating', control);
			// Update display
			this.rating('draw');
			
			//========================================================================================
			// LOZA: Añado esto para que me establezca el valor actual en un txt
			//========================================================================================
			
			if(this[0] == control.cancel[0]) //Hemos pulsado el boton cancelar
			{
				var baseNombre = control.inputs[0].name;
				var txtHack = document.getElementById('txtHack'+baseNombre);
				if(txtHack.value == '0'){return;}
				txtHack.value = '0';
			}
			else	//Hemos pulsado una estrella
			{
				var baseNombre = this[0].childNodes[0].name;
				var txtHack = document.getElementById('txtHack'+baseNombre);
				if(txtHack.value == this[0].textContent){return;}
				txtHack.value = this[0].textContent;
			}
			
			//========================================================================================
			// LOZA: Fin de añadido
			//========================================================================================
			
			// find data for event
			var input = $( control.current ? control.current.data('rating.input') : null );
			// click callback, as requested here: http://plugins.jquery.com/node/1655
			if(control.callback) control.callback.apply(input[0], [input[0], control,votoAnterior]);// callback event
		},// $.fn.rating.select
		
		readOnly: function(toggle, disable){ // make the control read-only (still submits value)
			var control = this.data('rating'); if(!control) return this;
			// setread-only status
			control.readOnly = toggle || toggle==undefined ? true : false;
			// enable/disable control value submission
			if(disable) $(control.inputs).attr("disabled", "disabled");
			else     			$(control.inputs).removeAttr("disabled");
			// Update rating control state
			this.data('rating', control);
			// Update display
			this.rating('draw');
		},// $.fn.rating.readOnly
		
		disable: function(){ // make read-only and never submit value
			this.rating('readOnly', true, true);
		},// $.fn.rating.disable
		
		enable: function(){ // make read/write and submit value
			this.rating('readOnly', false, false);
		}// $.fn.rating.select
		
		
		
 });

	
	/*--------------------------------------------------------*/
	
	/*
		### Default Settings ###
		eg.: You can override default control like this:
		$.fn.rating.options.cancel = 'Clear';
	*/
	$.fn.rating.options = { //$.extend($.fn.rating, { options: {
	        resetAll: true,
			cancel: 'Cancel Rating',   // advisory title for the 'cancel' link
			cancelValue: '',           // value to submit when user click the 'cancel' link
			split: 0,                  // split the star into how many parts?
						
			// Width of star image in case the plugin can't work it out. This can happen if
			// the jQuery.dimensions plugin is not available OR the image is hidden at installation
			starWidth: 16,
			
			//NB.: These don't need to be pre-defined (can be undefined/null) so let's save some code!
			//half:     false,         // just a shortcut to control.split = 2
			required: false,         // disables the 'cancel' button so user can only select one of the specified values
			//readOnly: false,         // disable rating plugin interaction/ values cannot be changed
			//focus:    function(){},  // executed when stars are focused
			//blur:     function(){},  // executed when stars are focused
			//callback: function(){},  // executed when a star is clicked
			focus: function(value, link){
				// 'this' is the hidden form element holding the current value
				// 'value' is the value selected
				// 'element' points to the link element that received the click.
				var tip = $('#hover'+ this.name);
				tip[0].data = tip[0].data || tip.html();
				tip.html(link.title || 'value: '+value);
			},
			blur: function(value, link){
				var tip = $('#hover'+ this.name);
				tip.html(tip[0].data || '');
			},
			callback: function(input, control, votoViejo){
			    //Sea lo que sea, reseteamos el control
			    var eid = (this.name || 'unnamed-rating').replace(/\[|\]/g, '_').replace(/^\_+|\_+$/g,'');
			    var context = $(this.form || document.body);
			    var raters = context.data('rating');
                if(!raters || raters.call!=$.fn.rating.calls) raters = { count:0, call:$.fn.rating.calls };
                var rater = raters[eid];
			    rater = null;
			    
				if(control.current != null) //Hemos pulsado una estrella
				{
				    input.attributes['onclick'].value=input.attributes['onclick'].value.replace('%votoViejo%',votoViejo);
				    if(typeof(input.onclick) == 'function'){
					    input.onclick();
					}
					else{
					    eval(input.onclick);
					}
				}else{	//Hemos pulsado en el boton cancelar
				    control.inputs[0].attributes['onclick'].value=control.inputs[0].attributes['onclick'].value.replace('%votoViejo%',votoViejo);
				    control.inputs[0].attributes['onclick'].value=control.inputs[0].attributes['onclick'].value.replace('&1','&0');
					//eval(control.inputs[0].attributes['onclick'].value.replace('&1','&0'));
					if(typeof(control.inputs[0].onclick) == 'function'){
					    control.inputs[0].onclick();
					}
					else{
					    eval(control.inputs[0].attributes['onclick'].value);
					}
				}
			}
			
			
			  
 }; //} });
	/*--------------------------------------------------------*/
	
	/*
		### Default implementation ###
		The plugin will attach itself to file inputs
		with the class 'multi' when the page loads
	*/
	$(function(){
	 $('input[type=radio].star').rating();
	});
	
/*# AVOID COLLISIONS #*/
})(jQuery);
/*# AVOID COLLISIONS #*/

}


$(document).ready(function(){
    var duracion=500;
    if(navigator.appName=="Microsoft Internet Explorer")
    {
        duracion= 0;
    }    
	
	$('div.gallery').gallery({
		duration: 500,
		autoRotation: 5000,
		listOfSlides: '#carousel > ul > li',
		switcher: '.switcher>li',
		effect:true
	});
	$('div.gallery').gallery({
		duration: 500,
		autoRotation: 5000,
		listOfSlides: '.items > .item',
		switcher: '.switcher>li',
		effect:true
	});
	$('#comunidades').gallery({
		duration: duracion,
		//autoRotation: 5000,
		listOfSlides: ' .contenedor > .pag_comunidades',
		switcher: ' .contenedor > .botones > li > .botoncito',
		effect:true
	});
	$('#recursos').gallery({	
	    duration:duracion,	
		autoRotation: 5000,
		listOfSlides: ' .contenedor > .pag_recurso',
		switcher: ' .contenedor > .botones > li',
		effect:true
	});
});

(function($) {
	$.fn.gallery = function(options) { return new Gallery(this.get(0), options); };
	
	function Gallery(context, options) { this.init(context, options); };
	
	Gallery.prototype = {
		options:{},
		init: function (context, options){
			this.options = $.extend({
				duration: 1400,
				slideElement: 1,
				autoRotation: false,
				effect: false,
				listOfSlides: 'ul > li',
				switcher: false,
				disableBtn: false,
				nextBtn: 'a.link-next, a.btn-next, a.next',
				prevBtn: 'a.link-prev, a.btn-prev, a.prev',
				circle: true,
				direction: false,
				event: 'click',
				IE: false
			}, options || {});
			var _el = $(context).find(this.options.listOfSlides);
			if (this.options.effect) this.list = _el;
			else this.list = _el.parent();
			this.switcher = $(context).find(this.options.switcher);
			this.nextBtn = $(context).find(this.options.nextBtn);
			this.prevBtn = $(context).find(this.options.prevBtn);
			this.count = _el.index(_el.filter(':last'));
			
			if (this.options.switcher) this.active = this.switcher.index(this.switcher.filter('.active:eq(0)'));
			else this.active = _el.index(_el.filter('.active:eq(0)'));
			if (this.active < 0) this.active = 0;
			this.last = this.active;
			
			this.woh = _el.outerWidth(true);
			if (!this.options.direction) this.installDirections(this.list.parent().width());
			else {
				this.woh = _el.outerHeight(true);
				this.installDirections(this.list.parent().height());
			}
			
			if (!this.options.effect) {
				this.rew = this.count - this.wrapHolderW + 1;
				if (!this.options.direction) this.list.css({marginLeft: -(this.woh * this.active)});
				else this.list.css({marginTop: -(this.woh * this.active)});
			}
			else {
				this.rew = this.count;
				this.list.css({opacity: 0}).removeClass('active').eq(this.active).addClass('active').css({opacity: 1}).css('opacity', 'auto');
				this.switcher.removeClass('active').eq(this.active).addClass('active');
			}
			
			if (this.options.disableBtn) {
				if (this.count < this.wrapHolderW) this.nextBtn.addClass(this.options.disableBtn);
				if (this.active == 0) this.prevBtn.addClass(this.options.disableBtn);
			}
			
			this.initEvent(this, this.nextBtn, this.prevBtn, true);
			this.initEvent(this, this.prevBtn, this.nextBtn, false);
			
			if (this.options.autoRotation) this.runTimer(this);
			
			if (this.options.switcher) this.initEventSwitcher(this, this.switcher);
		},
		installDirections: function(temp){
			this.wrapHolderW = Math.ceil(temp / this.woh);
			if (((this.wrapHolderW - 1) * this.woh + this.woh / 2) > temp) this.wrapHolderWwrapHolderW--;
		},
		fadeElement: function(){
//			if ($.browser.msie && this.options.IE){
//				this.list.eq(this.last).css({opacity:0});
//				this.list.removeClass('active').eq(this.active).addClass('active').css({opacity:'auto'});
//			}
//			else{
				this.list.eq(this.last).animate({opacity:0}, {queue:false, duration: this.options.duration});
				this.list.removeClass('active').eq(this.active).addClass('active').animate({
					opacity:1
				}, {queue:false, duration: this.options.duration, complete: function(){
					$(this).css('opacity','auto');
				}});
//			}
			if (this.options.switcher) this.switcher.removeClass('active').eq(this.active).addClass('active');
			this.last = this.active;
		},
		scrollElement: function(){
			if (!this.options.direction) this.list.animate({marginLeft: -(this.woh * this.active)}, {queue:false, duration: this.options.duration});
			else this.list.animate({marginTop: -(this.woh * this.active)}, {queue:false, duration: this.options.duration});
			if (this.options.switcher) this.switcher.removeClass('active').eq(this.active).addClass('active');
		},
		runTimer: function($this){
			if($this._t) clearTimeout($this._t);
			$this._t = setInterval(function(){
				$this.toPrepare($this, true);
			}, this.options.autoRotation);
		},
		initEventSwitcher: function($this, el){
			el.bind($this.options.event, function(){
				$this.active = $this.switcher.index($(this));
				if($this._t) clearTimeout($this._t);
				if (!$this.options.effect) $this.scrollElement();
				else $this.fadeElement();
				if ($this.options.autoRotation) $this.runTimer($this);
				return false;
			});
		},
		initEvent: function($this, addEventEl, addDisClass, dir){
			addEventEl.bind($this.options.event, function(){
				if($this._t) clearTimeout($this._t);
				if ($this.options.disableBtn &&($this.count > $this.wrapHolderW)) addDisClass.removeClass($this.options.disableBtn);
				$this.toPrepare($this, dir);
				if ($this.options.autoRotation) $this.runTimer($this);
				return false;
			});
		},
		toPrepare: function($this, side){
			if (($this.active == $this.rew) && $this.options.circle && side) $this.active = -$this.options.slideElement;
			if (($this.active == 0) && $this.options.circle && !side) $this.active = $this.rew + $this.options.slideElement;
			for (var i = 0; i < $this.options.slideElement; i++){
				if (side) {
					if ($this.active + 1 > $this.rew) {
						if ($this.options.disableBtn && ($this.count > $this.wrapHolderW)) $this.nextBtn.addClass($this.options.disableBtn);
					}
					else $this.active++;
				}
				else{
					if ($this.active - 1 < 0) {
						if ($this.options.disableBtn && ($this.count > $this.wrapHolderW)) $this.prevBtn.addClass($this.options.disableBtn);
					}
					else $this.active--;
				}
			};
			if ($this.active == $this.rew && side) if ($this.options.disableBtn &&($this.count > $this.wrapHolderW)) $this.nextBtn.addClass($this.options.disableBtn);
			if ($this.active == 0 && !side) if ($this.options.disableBtn &&($this.count > $this.wrapHolderW)) $this.prevBtn.addClass($this.options.disableBtn);
			if (!$this.options.effect) $this.scrollElement();
			else $this.fadeElement();
		},
		stop: function(){
			if (this._t) clearTimeout(this._t);
		},
		play: function(){
			if (this._t) clearTimeout(this._t);
			if (this.options.autoRotation) this.runTimer(this);
		}
	}
}(jQuery));


/**/ 
/*ejemplosUso.js*/ 
function loadCallback(paramsCallback, controlPintado, funcionJS) {
    controlPintado.load(document.location.href + '?callback=' + paramsCallback, funcionJS);
}

var $_getVariables = { isset: false };
var $_getGlobalVariables = {};
var $_GETAllVariables = function () {
    var scripts = document.getElementsByTagName("script");
    for (var i = 0; i < scripts.length; i++) {
        var script = (scripts[i].src + "").split("/");
        script = script[script.length - 1].split("?", 2);
        if (script.length > 1) {
            var parameters = script[1].split("&")
            for (var j = 0; j < parameters.length; j++) {
                var vars = parameters[j].split("=");
                if (!$_getVariables[script[0]]) $_getVariables[script[0]] = {};
                $_getVariables[script[0]][vars[0]] = vars[1];
                $_getGlobalVariables[vars[0]] = vars[1];
            }
        }
    }
    $_getVariables.isset = true;
};
$_GET = function (paramToGet, jsFile) {
    if (!$_getVariables.isset)
        $_GETAllVariables();
    if (jsFile)
        return $_getVariables[jsFile][paramToGet];
    else
        return $_getGlobalVariables[paramToGet];
};

$(function () {
    if ($('#sidebar .selectorAdd').length > 0) {
        $('#sidebar .selectorAdd').listToSelect();
    }
});

if (document.cookie != "" && document.location.href.toLowerCase().indexOf('http://www.') == 0) {
    la_cookie = document.cookie.split("; ")
    fecha_fin = new Date
    fecha_fin.setDate(fecha_fin.getDate() - 1)
    for (i = 0; i < la_cookie.length; i++) {
        mi_cookie = la_cookie[i].split("=")[0]
        document.cookie = mi_cookie + "=;expires=" + fecha_fin.toGMTString()
    }
}

var contResultados = 0;
var filtrosPeticionActual = '';
//var contFacetas = 0;

var funcionExtraResultados = "";
var funcionExtraFacetas = "";

var ExpresionRegularCaracteresRepetidos = /(.)\1{2,}/;
var ExpresionRegularNombres = /^([a-zA-ZñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ]+[a-zA-ZñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ\s-]*[a-zA-ZñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ]+)$/;
///^([a-zA-Z0-9-\sñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ'´`Çç]{0,})$/;
var ExpresionNombreCorto = /^([a-zA-Z0-9ñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ'´`çÇ-]{4,30})$/;
var ExpresionNombreCortoCentro = /^([a-zA-Z0-9ñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ '´`çÇ-]{2,10})$/;

function pasarSegundaParteOrg() {
    var $pan1 = $('#datosCuenta');
    var $pan2 = $('#loginContactoPral');
    $pan1.fadeOut('', function () {
        $pan2.fadeIn();
    });
    return false;
}

function volverPrimeraParteOrg() {
    var $pan1 = $('#loginContactoPral');
    var $pan2 = $('#datosCuenta');
    $pan1.fadeOut('', function () {
        $pan2.fadeIn();
    });
    return false;
}

function pasarSegundaParteOrgUsu() {
    var error = '';

    if (document.getElementById('ctl00_CPH1_cbUsarMyGNOSS') != null) {
        if ($('#ctl00_CPH1_cbUsarMyGNOSS').is(':checked')) {
            if (($('#ctl00_CPH1_txtCargo').val() == '') || ($('#ctl00_CPH1_txtEmail').val() == '')) {
                if ($('#ctl00_CPH1_txtCargo').val() == '') {
                    error += '<p>' + form.camposVacios + '</p>';
                    document.getElementById('ctl00_CPH1_lblCargo').style.color = 'red';
                }
                else {
                    document.getElementById('ctl00_CPH1_lblCargo').style.color = '';
                }

                if ($('#ctl00_CPH1_txtEmail').val() == '') {
                    error += '<p>' + form.emailValido + '</p>';
                    document.getElementById('ctl00_CPH1_lblEmail').style.color = 'red';
                }
                else {
                    document.getElementById('ctl00_CPH1_lblEmail').style.color = '';
                }
            }
            else {
                document.getElementById('ctl00_CPH1_lblCargo').style.color = '';
                document.getElementById('ctl00_CPH1_lblEmail').style.color = '';
            }
        }
        else {
            if ($('#ctl00_CPH1_txtCargo').val() == '' || $('#ctl00_CPH1_txtEmail').val() == '') {
                if ($('#ctl00_CPH1_txtCargo').val() == '') {
                    error += '<p>' + form.camposVacios + '</p>';
                    document.getElementById('ctl00_CPH1_lblCargo').style.color = 'red';
                }
                else {
                    document.getElementById('ctl00_CPH1_lblCargo').style.color = '';
                }

                if ($('#ctl00_CPH1_txtEmail').val() == '') {
                    error += '<p>' + form.emailValido + '</p>';
                    document.getElementById('ctl00_CPH1_lblEmail').style.color = 'red';
                }
                else {
                    document.getElementById('ctl00_CPH1_lblEmail').style.color = '';
                }
            }
            else {
                document.getElementById('ctl00_CPH1_lblCargo').style.color = '';

                if (!validarEmail($('#ctl00_CPH1_txtEmail').val())) {
                    error += '<p>' + form.emailValido + '</p>';
                    document.getElementById('ctl00_CPH1_lblEmail').style.color = 'red';
                }
                else {
                    document.getElementById('ctl00_CPH1_lblEmail').style.color = '';
                }
            }
        }
    }


    if (!$('#ctl00_CPH1_cbLegal').is(':checked')) {
        error += '<p>' + form.aceptarLegal + '</p>';
        $('#ctl00_CPH1_lblAceptarCondiciones').attr('class', 'ko');
    }
    else {
        $('#ctl00_CPH1_lblAceptarCondiciones').attr('class', '');
    }


    if (error.length) {
        crearError(error, '#divError', true);
        return false;
    }
    else {
        return true;
    }
}

//                                                                       Busquedas guardadas
//------------------------------------------------------------------------------------------
$('ul.busquedasGuardadas a.miniEliminar').click(function () {
    miniConfirmar('Desea minieliminar esta b&uacute;squeda?', $(this).parents('li').eq(0), function () {
        alert('Funciona al uso');
    });
});

//                                                                          registro de clases
//--------------------------------------------------------------------------------------------
function registroClaseSiguiente(funcion, id, id2) {
    var error = '';

    if (!(document.getElementById('ctl00_CPH1_lblErrorLogo') == null || document.getElementById('ctl00_CPH1_lblErrorLogo').innerHTML == '')) {
        return false;
    }

    error += AgregarErrorReg(error, ValidarCentroClase('ctl00_CPH1_txtCentro', 'ctl00_CPH1_lblCentro'));
    error += AgregarErrorReg(error, ValidarAsignaturaClase('ctl00_CPH1_txtAsignatura', 'ctl00_CPH1_lblAsignatura'));
    error += AgregarErrorReg(error, ValidarCurso('ctl00_CPH1_txtCurso', 'ctl00_CPH1_lblCurso'));
    error += AgregarErrorReg(error, ValidarCPOrg('ctl00_CPH1_txtCP', 'ctl00_CPH1_lblCP'));
    error += AgregarErrorReg(error, ValidarPoblacionOrg('ctl00_CPH1_txtPoblacion', 'ctl00_CPH1_lblPoblacion'));
    error += AgregarErrorReg(error, ValidarDireccionSede('ctl00_CPH1_txtDireccionSede', 'ctl00_CPH1_lblDireccionSede'));

    error += AgregarErrorReg(error, ValidarCategorias('ctl00_CPH1_txtCat', 'ctl00_CPH1_lblCat', 'ctl00_CPH1_chkCrearComunidadPrivada'));

    error += AgregarErrorReg(error, ValidarPaisOrg('ctl00_CPH1_editPais', 'ctl00_CPH1_lblPaisResidencia'));

    error += AgregarErrorReg(error, ValidarProvinciaOrg('ctl00_CPH1_editProvincia', 'ctl00_CPH1_lblProvincia', 'ctl00_CPH1_txtProvincia'));

    error += AgregarErrorReg(error, ValidarNombreCortoCentro('ctl00_CPH1_txtNombreCortoCentro', 'ctl00_CPH1_lblNombreCortoCentro'));

    //Nombre corto de la asignatura
    error += AgregarErrorReg(error, ValidarNombreCortoAsig('ctl00_CPH1_txtNombreCortoAsignatura', 'ctl00_CPH1_lblNombreCortoAsignatura'));

    //Nombre corto del curso
    error += AgregarErrorReg(error, ValidarNombreCortoCursoIntroducido('ctl00_CPH1_txtCurso', 'ctl00_CPH1_lblCurso'));

    //Grupos    
    error += AgregarErrorReg(error, ValidarGrupos('ctl00_CPH1_txtGrupo', 'ctl00_CPH1_lblGrupo'));

    //Comprobamos si se ha seleccionado el tipo de clase    
    error += AgregarErrorReg(error, ValidarTipoClase('ctl00_CPH1_rbUni20', 'ctl00_CPH1_rbEdex', 'ctl00_CPH1_lblTipoClase'));

    if (error.length) {
        crearError(error, '#tikitakaOrg', true);
        return false;
    } else {
        if (document.getElementById('lblBtnPrefieroUsuario') != null) {
            document.getElementById('lblBtnPrefieroUsuario').style.display = 'none';
        }
        id = '#' + id;
        id2 = '#' + id2;

        //Esto es necesario ya que si no compara con el nombre acabado en <br/>        
        var a = $(id).val();
        if (a.match('<br/>') != null) {
            a = a.substring(0, a.indexOf('<br/>'));
        }

        var b = $(id2).val();
        //array url
        var b2 = b.split("<br/>");

        var clases = "";

        for (i = 0; i < b2.length; i++) {
            var arrayClase = b2[i].split("/");
            if (arrayClase[arrayClase.length - 1] != "") {
                clases = clases + "|" + arrayClase[arrayClase.length - 1];
            }
        }

        var checked = true;
        if ($('#ctl00_CPH1_chkCrearComunidadPrivada').length > 0) {
            checked = $('#ctl00_CPH1_chkCrearComunidadPrivada').is(':checked');
        }

        var comunidades = '';
        try {
            comunidades = $('#ctl00_CPH1_chkCrearComunidadPrivada').val();
        } catch (Exception) { }

        funcion = funcion.replace("$$", "&" + a.replace('\'', '\\\'') + "&" + clases + "&" + checked + "&" + comunidades);
        eval(funcion);
    }
}

function ComprobarCampoRegClase(pCampo) {
    var error = '';
    var panPintarError = '';

    if (pCampo == 'Centro') {
        error = ValidarCentroClase('ctl00_CPH1_txtCentro', 'ctl00_CPH1_lblCentro');
        panPintarError = 'divKoCentroClase';
    }
    else if (pCampo == 'Asignatura') {
        error = ValidarAsignaturaClase('ctl00_CPH1_txtAsignatura', 'ctl00_CPH1_lblAsignatura');
        panPintarError = 'divKoNombreAsig';
    }
    else if (pCampo == 'Curso') {
        error = ValidarCurso('ctl00_CPH1_txtCurso', 'ctl00_CPH1_lblCurso');
        error = ValidarNombreCortoCursoIntroducido('ctl00_CPH1_txtCurso', 'ctl00_CPH1_lblCurso');
        panPintarError = 'divKoCurso';
    }
    else if (pCampo == 'Categorias') {
        error = ValidarCategorias('ctl00_CPH1_txtCat', 'ctl00_CPH1_lblCat', 'ctl00_CPH1_chkCrearComunidadPrivada');
        panPintarError = 'divKoCategoria';
    }
    else if (pCampo == 'NombreCortoCentro') {
        error = ValidarNombreCortoCentro('ctl00_CPH1_txtNombreCortoCentro', 'ctl00_CPH1_lblNombreCortoCentro');
        panPintarError = 'divKoNombreCortoCentro';
    }
    else if (pCampo == 'Asig') {
        error = ValidarNombreCortoAsig('ctl00_CPH1_txtNombreCortoAsignatura', 'ctl00_CPH1_lblNombreCortoAsignatura');
        panPintarError = 'divKoNombreCortoAsig';
    }
    else if (pCampo == 'Grupos') {
        error = ValidarGrupos('ctl00_CPH1_txtGrupo', 'ctl00_CPH1_lblGrupo');
        panPintarError = 'divKoGrupo';
    }
        //    else if (pCampo == 'Curso')
        //    {
        //        error = ValidarNombreCortoCursoIntroducido('ctl00_CPH1_txtCurso','ctl00_CPH1_lblCurso');
        //        panPintarError = '';
        //    }
    else if (pCampo == 'TipoClase') {
        error = ValidarTipoClase('ctl00_CPH1_rbUni20', 'ctl00_CPH1_rbEdex', 'ctl00_CPH1_lblTipoClase');
        panPintarError = 'divKoTipoClase';
    }

    if (error != '') {
        crearError(error, '#' + panPintarError);
    }
    else {
        LimpiarHtmlControl(panPintarError);
    }
}

function ValidarCentroClase(pTxtCentro, pLblCentro) {
    var error = '';
    var centro = document.getElementById(pTxtCentro);
    if (centro != null && centro.value == '') {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblCentro).attr('class', 'ko');
    }
    else {
        $('#' + pLblCentro).attr('class', '');
    }

    return error;
}

function ValidarAsignaturaClase(pTxtAsignatura, pLblAsignatura) {
    var error = '';
    var asignatura = document.getElementById(pTxtAsignatura);
    if (asignatura != null && asignatura.value == '') {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblAsignatura).attr('class', 'ko');
    }
    else {
        $('#' + pLblAsignatura).attr('class', '');
    }

    return error;
}

function ValidarCurso(pTxtCurso, pLblCurso) {
    var error = '';
    var curso = document.getElementById(pTxtCurso);
    if (curso != null && (curso.value.length == 0 || curso.value.length > 10)) {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblCurso).attr('class', 'ko');
    }
    else {
        $('#' + pLblCurso).attr('class', '');
    }

    return error;
}

function ValidarGrupos(pTxtGrupo, pLblGrupo) {
    var error = '';
    var grupos = document.getElementById(pTxtGrupo);

    var RegExPatternNombreCorto = /^([a-z,A-Z]{0,999})$/;
    var correcto = grupos.value.match(RegExPatternNombreCorto);

    if (!correcto) {
        error += '<p>' + form.grupoincorrecto + '</p>';
        $('#' + pLblGrupo).attr('class', 'ko');
    }
    else {
        $('#' + pLblGrupo).attr('class', '');
    }

    return error;
}

function ValidarCategorias(pTxtCat, pLblCat, pChkComPri) {
    var error = '';

    if ($('#' + pChkComPri).is(':checked')) {
        if ($('#' + pTxtCat).val() == '') {
            $('#' + pLblCat).attr('class', 'ko');
            error += '<p>' + form.camposVacios + '</p>';
        } else {
            $('#' + pLblCat).attr('class', '');
        }
    }

    return error;
}

function ValidarNombreCortoCentro(pTxtNombreCentro, pLblNombreCentro) {
    var error = '';

    //Nombre corto del centro
    var nombreCortoCentro = document.getElementById(pTxtNombreCentro).value;
    var errorNombreCortoCentro = !ValidarNombreCortoCentroYAsignatura(nombreCortoCentro);
    if (errorNombreCortoCentro) {
        error += '<p>' + form.nombrecortoincorrectocentro + '</p>';
        $('#' + pLblNombreCentro).attr('class', 'ko');
    }
    else {
        $('#' + pLblNombreCentro).attr('class', '');
    }

    return error;
}

function ValidarNombreCortoAsig(pTxtNombreAsig, pLblNombreAsig) {
    var error = '';

    //Nombre corto de la asignatura
    var nombreCortoAsignatura = document.getElementById(pTxtNombreAsig).value;
    var errorNombreCortoAsignatura = !ValidarNombreCortoCentroYAsignatura(nombreCortoAsignatura);
    if (errorNombreCortoAsignatura) {
        error += '<p>' + form.nombrecortoincorrectoasignatura + '</p>';
        $('#' + pLblNombreAsig).attr('class', 'ko');
    } else {
        $('#' + pLblNombreAsig).attr('class', '');
    }

    return error;
}

function ValidarNombreCortoCursoIntroducido(pTxtCurso, pLblCurso) {
    var error = '';

    //Nombre corto del curso
    var nombreCortoCurso = document.getElementById(pTxtCurso).value;
    var errorNombreCortoCurso = !ValidarNombreCortoCurso(nombreCortoCurso);
    if (errorNombreCortoCurso) {
        error += '<p>' + form.nombrecortoincorrectocurso + '</p>';
        $('#' + pLblCurso).attr('class', 'ko');
    } else {
        $('#' + pLblCurso).attr('class', '');
    }

    return error;
}

function ValidarTipoClase(pRbTipoClase1, pRbTipoClase2, pLblTipoClase) {
    var error = '';

    //Comprobamos si se ha seleccionado el tipo de clase
    var uniChecked = $('#' + pRbTipoClase1).is(':checked');
    var edexChecked = $('#' + pRbTipoClase2).is(':checked');
    if (!uniChecked && !edexChecked) {
        error += '<p>' + form.debesseleccionartipoclase + '</p>';
        $('#' + pLblTipoClase).attr('class', 'ko');
    } else {
        $('#' + pLblTipoClase).attr('class', '');
    }

    return error;
}

//                                                                  registro de organizaciones
//--------------------------------------------------------------------------------------------
function registroOrganizacionSiguiente(funcion, id, id2) {
    var error = '';

    if (!(document.getElementById('ctl00_CPH1_lblErrorLogo') == null || document.getElementById('ctl00_CPH1_lblErrorLogo').innerHTML == '')) {
        return false;
    }

    if ($('#ctl00_CPH1_txtRazonSocial').val() == ''
|| $('#ctl00_CPH1_editDia').val() == ''
|| (($('#ctl00_CPH1_txtProvincia').val() == '') && (document.getElementById('ctl00_CPH1_txtProvincia').style.display == ""))) {
        error += '<p>' + form.camposVacios + '</p>';
    }

    error += AgregarErrorReg(error, ValidarAlias('ctl00_CPH1_txtAlias', 'ctl00_CPH1_lblAlias'));
    error += AgregarErrorReg(error, ValidarRazonSocial('ctl00_CPH1_txtRazonSocial', 'ctl00_CPH1_lblRazonSocial'));
    error += AgregarErrorReg(error, ValidarDireccionSede('ctl00_CPH1_txtDireccionSede', 'ctl00_CPH1_lblDireccionSede'));
    error += AgregarErrorReg(error, ValidarCPOrg('ctl00_CPH1_txtCP', 'ctl00_CPH1_lblCP'));
    error += AgregarErrorReg(error, ValidarPoblacionOrg('ctl00_CPH1_txtPoblacion', 'ctl00_CPH1_lblPoblacion'));

    error += AgregarErrorReg(error, ValidarTipoOrg('ctl00_CPH1_ddlTipoOrganizacion', 'ctl00_CPH1_lblTipoOrganizacion'));

    error += AgregarErrorReg(error, ValidarSector('ctl00_CPH1_ddlSector', 'ctl00_CPH1_lblSector'));

    error += AgregarErrorReg(error, ValidarEmpleados('ctl00_CPH1_ddlEmpleados', 'ctl00_CPH1_lblNumeroEmpl'));

    error += AgregarErrorReg(error, ValidarPaisOrg('ctl00_CPH1_editPais', 'ctl00_CPH1_lblPaisResidencia'));

    error += AgregarErrorReg(error, ValidarProvinciaOrg('ctl00_CPH1_editProvincia', 'ctl00_CPH1_lblProvincia', 'ctl00_CPH1_txtProvincia'));

    error += AgregarErrorReg(error, ValidarNombreCortoOrgIntroducido('ctl00_CPH1_txtNombreCorto', 'ctl00_CPH1_lblNombreCorto'));

    if (error.length) {
        crearError(error, '#tikitakaOrg', true);
        return false;
    }
    else {
        if (document.getElementById('ctl00_CPH1_btnPrefieroUsuario') != null) {
            document.getElementById('ctl00_CPH1_btnPrefieroUsuario').style.display = 'none';
        }
        id = '#' + id;
        id2 = '#' + id2;
        funcion = funcion.replace("$$", "&" + $(id).val().replace('\'', '\\\'') + "&" + $(id2).val());
        eval(funcion);
    }
}

function ComprobarCampoRegOrg(pCampo) {
    var error = '';
    var panPintarError = '';

    if (pCampo == 'Alias') {
        error = ValidarAlias('ctl00_CPH1_txtAlias', 'ctl00_CPH1_lblAlias');
        panPintarError = 'divKoAlias';
    }
    else if (pCampo == 'RazonSocial') {
        error = ValidarRazonSocial('ctl00_CPH1_txtRazonSocial', 'ctl00_CPH1_lblRazonSocial');
        panPintarError = 'divKoRazonSocial';
    }
    else if (pCampo == 'DireccionSede') {
        error = ValidarDireccionSede('ctl00_CPH1_txtDireccionSede', 'ctl00_CPH1_lblDireccionSede');
        panPintarError = 'divKoDireccionSede';
    }
    else if (pCampo == 'CPOrg') {
        error = ValidarCPOrg('ctl00_CPH1_txtCP', 'ctl00_CPH1_lblCP');
        panPintarError = 'divKoCP';
    }
    else if (pCampo == 'Poblacion') {
        error = ValidarPoblacionOrg('ctl00_CPH1_txtPoblacion', 'ctl00_CPH1_lblPoblacion');
        panPintarError = 'divKoPoblacion';
    }
    else if (pCampo == 'TipoOrg') {
        error = ValidarTipoOrg('ctl00_CPH1_ddlTipoOrganizacion', 'ctl00_CPH1_lblTipoOrganizacion');
        panPintarError = 'divKoTipoOrg';
    }
    else if (pCampo == 'Sector') {
        error = ValidarSector('ctl00_CPH1_ddlSector', 'ctl00_CPH1_lblSector');
        panPintarError = 'divKoSector';
    }
    else if (pCampo == 'Empleados') {
        error = ValidarEmpleados('ctl00_CPH1_ddlEmpleados', 'ctl00_CPH1_lblNumeroEmpl');
        panPintarError = 'divKoEmpleados';
    }
    else if (pCampo == 'Pais') {
        error = ValidarPaisOrg('ctl00_CPH1_editPais', 'ctl00_CPH1_lblPaisResidencia');
        panPintarError = 'divKoPaisOrg';
    }
    else if (pCampo == 'Provincia') {
        error = ValidarProvinciaOrg('ctl00_CPH1_editProvincia', 'ctl00_CPH1_lblProvincia', 'ctl00_CPH1_txtProvincia');
        panPintarError = 'divKoProvincia';
    }
    else if (pCampo == 'NombreCortoOrg') {
        error = ValidarNombreCortoOrgIntroducido('ctl00_CPH1_txtNombreCorto', 'ctl00_CPH1_lblNombreCorto');
        panPintarError = 'divKoNombreCorto';
    }

    if (error != '') {
        crearError(error, '#' + panPintarError);
    }
    else {
        LimpiarHtmlControl(panPintarError);
    }
}

function ValidarAlias(pTxtAlias, pLblAlias) {
    var error = '';

    if ($('#' + pTxtAlias).val() == '') {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblAlias).attr('class', 'ko');
    }
    else {
        $('#' + pLblAlias).attr('class', '');
    }

    //Comprobamos el alias
    if (document.getElementById(pTxtAlias) != null) {
        var aliasOrg = document.getElementById(pTxtAlias).value;
        var RegExPatternaliasOrg = /<|>$/;

        if (aliasOrg.match(RegExPatternaliasOrg) || aliasOrg.length > 30 || aliasOrg.indexOf(',') != -1) {
            error += '<p>' + form.formatoAlias + '</p>';
            $('#' + pLblAlias).attr('class', 'ko');
        }
    }

    return error;
}

function ValidarRazonSocial(pTxtRazonSocial, pLblRazonSocial) {
    var error = '';
    //Comprobamos razon social
    if (document.getElementById(pTxtRazonSocial) != null) {
        if (document.getElementById(pTxtRazonSocial).style.display != 'none' && $('#' + pTxtRazonSocial).val() == '') {
            error += '<p>' + form.camposVacios + '</p>';
            $('#' + pLblRazonSocial).attr('class', 'ko');
        }
        else {
            $('#' + pLblRazonSocial).attr('class', '');

            var razonSocialOrg = document.getElementById(pTxtRazonSocial).value;
            var RegExPatternrazonSocialOrg = /<|>$/;
            if (razonSocialOrg.match(RegExPatternrazonSocialOrg)) {
                error += '<p>' + form.formatoRazonSocial + '</p>';
                $('#' + pLblRazonSocial).attr('class', 'ko');
            }
        }
    }

    return error;
}

function ValidarDireccionSede(pTxtDireccion, pLblDireccion) {
    var error = '';

    if ($('#' + pTxtDireccion).val() == '') {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblDireccion).attr('class', 'ko');
    }
    else {
        $('#' + pLblDireccion).attr('class', '');
    }

    //Comprobamos la dirección
    var direccionOrg = document.getElementById(pTxtDireccion).value;
    var RegExPatterndireccionOrg = /<|>$/;
    if (direccionOrg.match(RegExPatterndireccionOrg)) {
        error += '<p>' + form.formatoDireccion + '</p>';
        $('#' + pLblDireccion).attr('class', 'ko');
    }

    return error;
}

function ValidarCPOrg(pTxtCP, pLblCP) {
    var error = '';

    if ($('#' + pTxtCP).val() == '') {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblCP).attr('class', 'ko');
    }
    else {
        $('#' + pLblCP).attr('class', '');
    }

    //Comprobamos el CP
    var CPOrg = document.getElementById(pTxtCP).value;
    var RegExPatternCPOrg = /<|>$/;
    if (CPOrg.match(RegExPatternCPOrg)) {
        error += '<p>' + form.formatoCP + '</p>';
        $('#' + pLblCP).attr('class', 'ko');
    }

    return error;
}

/**
 * Validar el campo poblaci�n introducido para el proceso de registro.
 * Si es correcto, se a�ade una clase "is-valid"
 * Si es incorrecto (vac�o) se a�ade una clase "is-invalid"
 * @param {any} pTxtPobla: Input de la poblaci�n
 * @param {any} pLblPobla: Label asociado al input poblaci�n
 */
function ValidarPoblacionOrg(pTxtPobla, pLblPobla) {
    var error = '';

    if ($('#' + pTxtPobla).val() == '') {
        error += '<p>' + form.camposVacios + '</p>';
        //$('#' + pLblPobla).attr('class', 'ko');
        $('#' + pTxtPobla).addClass('is-invalid');
        $('#' + pTxtPobla).removeClass('is-valid');
    }
    else {
        //$('#' + pLblPobla).attr('class', '');
        $('#' + pTxtPobla).addClass('is-valid');
        $('#' + pTxtPobla).removeClass('is-invalid');
    }

    //Comprobamos la localidad
    var localidadOrg = document.getElementById(pTxtPobla).value;
    var RegExPatternlocalidadOrg = /<|>$/;
    if (localidadOrg.match(RegExPatternlocalidadOrg)) {
        error += '<p>' + form.formatoLocalidad + '</p>';
        //$('#' + pLblPobla).attr('class', 'ko');
        $('#' + pTxtPobla).addClass('is-invalid');
        $('#' + pTxtPobla).removeClass('is-valid');
    }

    return error;
}

function ValidarTipoOrg(pDdlTipoOrg, pLblDdlTipoOrg) {
    var error = '';
    var comboOrg = document.getElementById(pDdlTipoOrg);

    if (comboOrg != null && comboOrg.selectedIndex == 0) {
        error += '<p>' + form.tipoorgincorrecta + '</p>';
        $('#' + pLblDdlTipoOrg).attr('class', 'ko');
    }
    else {
        $('#' + pLblDdlTipoOrg).attr('class', '');
    }

    return error;
}

function ValidarSector(pDdlSector, pLblDdlSector) {
    var error = '';
    var comboSector = document.getElementById(pDdlSector);

    if (comboSector != null && comboSector.selectedIndex == 0) {
        error += '<p>' + form.sectorincorrecto + '</p>';
        $('#' + pLblDdlSector).attr('class', 'ko');
    }
    else {
        $('#' + pLblDdlSector).attr('class', '');
    }

    return error;
}

function ValidarEmpleados(pDdlEmpleados, pLblDdlEmpleados) {
    var error = '';
    var comboEmpleados = document.getElementById(pDdlEmpleados);

    if (comboEmpleados != null && comboEmpleados.selectedIndex == 0) {
        error += '<p>' + form.numeroemplincorrecto + '</p>';
        $('#' + pLblDdlEmpleados).attr('class', 'ko');
    }
    else {
        $('#' + pLblDdlEmpleados).attr('class', '');
    }

    return error;
}

function ValidarPaisOrg(pEditPais, pLblEditPais) {
    var error = '';

    if ((document.getElementById(pEditPais) != null) && (document.getElementById(pEditPais).selectedIndex == 0)) {
        error += '<p>' + form.paisincorrecto + '</p>';
        $('#' + pLblEditPais).attr('class', 'ko');
    }
    else {
        $('#' + pLblEditPais).attr('class', '');
    }

    return error;
}

function ValidarProvinciaOrg(pEditProvincia, pLblEditProvincia, pTxtProvincia) {
    var error = '';

    if ((document.getElementById(pEditProvincia).style.display == "") && (document.getElementById(pEditProvincia).selectedIndex == 0)) {
        error += '<p>' + form.provinciaincorrecta + '</p>';
        $('#' + pLblEditProvincia).attr('class', 'ko');
    }
    else {
        $('#' + pLblEditProvincia).attr('class', '');
    }

    //Comprobamos la provincia   
    if (document.getElementById(pTxtProvincia) != null) {
        var provinciaOrg = document.getElementById(pTxtProvincia).value;

        var RegExPatternprovinciaOrg = /<|>$/;

        if (provinciaOrg.match(RegExPatternprovinciaOrg)) {
            error += '<p>' + form.formatoProvincia + '</p>';
            $('#' + pLblEditProvincia).attr('class', 'ko');
        }
    }

    if (($('#' + pTxtProvincia).val() == '') && (document.getElementById(pTxtProvincia).style.display == "")) {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblEditProvincia).attr('class', 'ko');
    }

    return error;
}

function ValidarNombreCortoOrgIntroducido(pTxtNombre, pLblNombre) {
    var error = '';

    if (!ValidarNombreCortoUsuario($('#' + pTxtNombre).val())) {
        error += '<p>' + form.nombrecortoincorrecto + '</p>';
        $('#' + pLblNombre).attr('class', 'ko');
    }
    else {
        $('#' + pLblNombre).attr('class', '');
    }

    return error;
}


function validarCif(texto) {
    var pares = 0;
    var impares = 0;
    var suma;
    var ultima;
    var unumero;
    var uletra = new Array("J", "A", "B", "C", "D", "E", "F", "G", "H", "I");
    var xxx;

    var regular = new RegExp(/^[ABCDEFGHKLMNPQS]\d\d\d\d\d\d\d[0-9,A-J]$/g);

    if (!regular.exec(texto)) {
        error += '<p>' + form.nifincorrecto + '</p>';
    }
    else {
        ultima = texto.substr(8, 1);

        for (var cont = 1; cont < 7; cont++) {
            xxx = (2 * parseInt(texto.substr(cont++, 1))).toString() + "0";
            impares += parseInt(xxx.substr(0, 1)) + parseInt(xxx.substr(1, 1));
            pares += parseInt(texto.substr(cont, 1));
        }
        xxx = (2 * parseInt(texto.substr(cont, 1))).toString() + "0";
        impares += parseInt(xxx.substr(0, 1)) + parseInt(xxx.substr(1, 1));

        suma = (pares + impares).toString();
        unumero = parseInt(suma.substr(suma.length - 1, 1));
        unumero = (10 - unumero).toString();

        if (unumero == 10) unumero = 0;

        if (!((ultima == unumero) || (ultima == uletra[unumero])))
            error += '<p>' + form.nifincorrecto + '</p>';
    }
}

function validarNombreCortoComunidad(nombre) {
    var RegExPatternNombreCorto = /^([a-zA-Z0-9ñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ'´`çÇ-]{4,30})$/;
    return (nombre.match(RegExPatternNombreCorto) && nombre != '');
}

//                                                                    formulario de registro
//------------------------------------------------------------------------------------------

function ComprobarCampoRegistro(paginaID, pCampo) {
    if (pCampo == 'NombreUsu') {
        ValidarNombreUsu(paginaID + '_txtNombreUsuario', paginaID + '_lblNombreUsuario');
    }
    else if (pCampo == 'Contra1') {
        ValidarContrasena(paginaID + '_txtContrasenya', '', paginaID + '_lblContrasenya', '', false);
    }
    else if (pCampo == 'Contra2') {
        ValidarContrasena(paginaID + '_txtContrasenya', paginaID + '_txtcContrasenya', paginaID + '_lblContrasenya', paginaID + '_lblConfirmarContrasenya', true);
    }
    else if (pCampo == 'Mail') {
        ValidarEmailIntroducido(paginaID + '_txtCorreoE', paginaID + '_lblEmail');
    }
    else if (pCampo == 'NombrePersonal') {
        ValidarNombrePersona(paginaID + '_txtNombrePersonal', paginaID + '_lblNombre');
    }
    else if (pCampo == 'Apellidos') {
        ValidarApellidos(paginaID + '_txtApellidos', paginaID + '_lblApellidos');
    }
    else if (pCampo == 'Provincia') {
        ValidarProvincia(paginaID + '_txtProvincia', paginaID + '_lblProvincia', paginaID + '_editProvincia');
    }
    else if (pCampo == 'Sexo') {
        ValidarSexo(paginaID + '_editSexo', paginaID + '_lblSexo');
    }
    else if (pCampo == 'CentroEstudios') {
        ValidarCentroEstudios(paginaID + '_txtCentroEstudios', paginaID + '_lbCentroEstudios');
    }
    else if (pCampo == 'AreaEstudios') {
        ValidarAreaEstudios(paginaID + '_txtAreaEstudios', paginaID + '_lbAreaEstudios');
    }
}


function registroUsuarioParte1(errores, paginaID, funcionComprobar) {
    var error = errores;
    $dC = $('#' + paginaID + '_datosUsuario');

    if ((typeof RecogerDatosExtra != 'undefined')) {
        error += RecogerDatosExtra();
    }

    error += AgregarErrorReg(error, ValidarNombrePersona(paginaID + '_txtNombrePersonal', paginaID + '_lblNombre'));
    error += AgregarErrorReg(error, ValidarApellidos(paginaID + '_txtApellidos', paginaID + '_lblApellidos'));
    error += AgregarErrorReg(error, ValidarPais(paginaID + '_ddlPais', paginaID + '_lblPais'));
    error += AgregarErrorReg(error, ValidarEmailIntroducido(paginaID + '_txtCorreoE', paginaID + '_lblEmail'));
    error += AgregarErrorReg(error, ValidarContrasena(paginaID + '_txtContrasenya', '', paginaID + '_lblContrasenya', '', false));

    var errorAvisoLegal = false;
    var error3 = '';

    //Fecha nacimiento / mayor de edad
    if (document.getElementById(paginaID + '_cbMayorEdad') != null) {
        if (!$('#' + paginaID + '_cbMayorEdad').is(':checked')) {
            errorAvisoLegal = true;
            error3 += '<p>' + form.mayorEdad + '</p>';
            $('#' + paginaID + '_lblMayorEdad').attr('class', 'ko');
        } else {
            $('#' + paginaID + '_lblMayorEdad').attr('class', '');
        }
    }
    if (document.getElementById(paginaID + '_editDia') != null) {
        error += AgregarErrorReg(error, ValidarFechaNacimiento(paginaID + '_editDia', paginaID + '_editMes', paginaID + '_editAnio', paginaID + '_lblFechaNac'));
    }

    //Compruebo cláusulas adicionales:
    if ($('#' + paginaID + '_txtHackClausulasSelecc').length && $('#' + paginaID + '_txtHackClausulasSelecc').val().indexOf('||') != -1) {
        var valorHackClau = $('#' + paginaID + '_txtHackClausulasSelecc').val();
        valorHackClau = valorHackClau.substring(0, valorHackClau.indexOf('||'));

        var color = '';
        if (valorHackClau != '0') {

            if (!errorAvisoLegal) {
                error3 += '<p>' + form.aceptarLegal + '</p>';
            }
            color = '#E24973';
        }

        var labelsClau = $('.clauAdicional');

        for (var i = 0; i < labelsClau.length; i++) {
            labelsClau[i].style.color = color;
        }
    }

    if (error.length || error3.length) {
        if (error.length) {
            crearError(error, '#divKodatosUsuario', false);
        }
        else {
            LimpiarHtmlControl('divKodatosUsuario');
        }

        if (error3.length) {
            crearError(error3, '#divKoCondicionesUso', false);
        }
        else {
            LimpiarHtmlControl('divKoCondicionesUso');
        }

        return false;
    } else {
        LimpiarHtmlControl('divKodatosUsuario');
        LimpiarHtmlControl('divKoDatosPersonales');
        LimpiarHtmlControl('divKoCondicionesUso');
        funcionComprobar = funcionComprobar.replace("$$", $('#' + paginaID + '_txtCorreoE').val() + "&" + $('#' + paginaID + '_txtNombrePersonal').val() + "&" + $('#' + paginaID + '_txtApellidos').val() + "&" + $('#' + paginaID + '_captcha').val());
        eval(funcionComprobar);

        return false;
    }
}


function registroUsuario_1(ultimoCheck, funcionComprobar) {
    var error = '',
$dC = $('#ctl00_CPH1_datosUsuario');

    error += AgregarErrorReg(error, ValidarNombrePersona('ctl00_CPH1_txtNombrePersonal', 'ctl00_CPH1_lblNombre'));
    error += AgregarErrorReg(error, ValidarApellidos('ctl00_CPH1_txtApellidos', 'ctl00_CPH1_lblApellidos'));

    error += AgregarErrorReg(error, ValidarNombreUsu('ctl00_CPH1_txtNombreUsuario', 'ctl00_CPH1_lblNombreUsuario'));

    if (!ultimoCheck) {
        error += AgregarErrorReg(error, ValidarContrasena('ctl00_CPH1_txtContrasenya', 'ctl00_CPH1_txtcContrasenya', 'ctl00_CPH1_lblContrasenya', 'ctl00_CPH1_lblConfirmarContrasenya', true));
    }

    error += AgregarErrorReg(error, ValidarEmailIntroducido('ctl00_CPH1_txtCorreoE', 'ctl00_CPH1_lblEmail'));

    var error2 = '';
    error2 += AgregarErrorReg(error2, ValidarProvincia('ctl00_CPH1_txtProvincia', 'ctl00_CPH1_lblProvincia', 'ctl00_CPH1_editProvincia'));
    error2 += AgregarErrorReg(error2, ValidarSexo('ctl00_CPH1_editSexo', 'ctl00_CPH1_lblSexo'));
    error2 += AgregarErrorReg(error2, ValidarCentroEstudios('ctl00_CPH1_txtCentroEstudios', 'ctl00_CPH1_lbCentroEstudios'));
    error2 += AgregarErrorReg(error2, ValidarAreaEstudios('ctl00_CPH1_txtAreaEstudios', 'ctl00_CPH1_lbAreaEstudios'));
    error2 += AgregarErrorReg(error2, ComprobarFechaNac('ctl00_CPH1_editDia', 'ctl00_CPH1_editMes', 'ctl00_CPH1_editAnio', 'ctl00_CPH1_lblFechaNac', 'ctl00_CPH1_lblEdadMinima', 'ctl00_CPH1_'));

    var errorAvisoLegal = false;
    var error3 = '';

    //Compruebo cláusulas adicionales:
    if (document.getElementById('ctl00_CPH1_txtHackClausulasSelecc') != null && document.getElementById('ctl00_CPH1_txtHackClausulasSelecc').value.indexOf('||') != -1) {
        var valorHackClau = document.getElementById('ctl00_CPH1_txtHackClausulasSelecc').value;
        valorHackClau = valorHackClau.substring(0, valorHackClau.indexOf('||'));

        var color = '';
        if (valorHackClau != '0') {

            if (!errorAvisoLegal) {
                error3 += '<p>' + form.aceptarLegal + '</p>';
            }
            color = '#E24973';
        }

        var labelsClau = $('.clauAdicional');

        for (var i = 0; i < labelsClau.length; i++) {
            labelsClau[i].style.color = color;
        }
    }

    if (error.length || error2.length || error3.length) {
        if (error.length) {
            //crearError('<ul>'+error+'</ul>', '#ctl00_CPH1_datosUsuario', true);
            crearError(error, '#divKodatosUsuario', true);
        }
        else {
            LimpiarHtmlControl('divKodatosUsuario');
        }

        if (error2.length) {
            crearError(error2, '#divKoDatosPersonales', true);
            //$('body, html').animate({scrollTop: $dP.offset().top}, 600);
        }
        else {
            LimpiarHtmlControl('divKoDatosPersonales');
        }

        if (error3.length) {
            crearError(error3, '#divKoCondicionesUso', true);
        }
        else {
            LimpiarHtmlControl('divKoCondicionesUso');
        }

        return false;
    }
    else if (ultimoCheck == true) {
        // si no hay error y estamos comprobando el formulario por ultima vez (antes de enviarlo, por ejemplo)...
        return true;
    }
    else {
        LimpiarHtmlControl('divKodatosUsuario');
        LimpiarHtmlControl('divKoDatosPersonales');
        LimpiarHtmlControl('divKoCondicionesUso');
        funcionComprobar = funcionComprobar.replace("$$", "&" + $('#ctl00_CPH1_txtNombreUsuario').val() + "," + $('#ctl00_CPH1_txtCorreoE').val() + "," + $('#ctl00_CPH1_txtNombreCorto').val() + "," + $('#ctl00_CPH1_txtCaptcha').val());
        eval(funcionComprobar);
        //		// ... de otro modo ocultamos y mostramos las capas pertinentes y...
        //		$dC.fadeOut('', function() {
        //			$('#registroUsuario').find('div.ko').remove();
        //			$('#datosPersonales').fadeIn();
        //		});
        //		// ... evitamos el comportamiento normal del botoncico
        return false;
    }
}

function AgregarErrorReg(pErrores, pError) {
    if (pErrores.indexOf(pError) != -1)//El error ya está, no hay que agregarlo.
    {
        return '';
    }
    else {
        return pError;
    }
}

function ComprobarCampoRegUsuario(pCampo) {
    var error = '';
    var error2 = '';

    var panPintarError = '';

    if (pCampo == 'NombreUsu') {
        error = ValidarNombreUsu('ctl00_CPH1_txtNombreUsuario', 'ctl00_CPH1_lblNombreUsuario');
        panPintarError = 'divKoLogin';
    }
    else if (pCampo == 'Contra1') {
        error = ValidarContrasena('ctl00_CPH1_txtContrasenya', 'ctl00_CPH1_txtcContrasenya', 'ctl00_CPH1_lblContrasenya', 'ctl00_CPH1_lblConfirmarContrasenya', false);
        panPintarError = 'divKoContr';
    }
    else if (pCampo == 'Contra2') {
        error = ValidarContrasena('ctl00_CPH1_txtContrasenya', 'ctl00_CPH1_txtcContrasenya', 'ctl00_CPH1_lblContrasenya', 'ctl00_CPH1_lblConfirmarContrasenya', true);
        panPintarError = 'divKoContr';
    }
    else if (pCampo == 'Mail') {
        error = ValidarEmailIntroducido('ctl00_CPH1_txtCorreoE', 'ctl00_CPH1_lblEmail');
        panPintarError = 'divKoEmail';
    }
    else if (pCampo == 'NombrePersonal') {
        error = ValidarNombrePersona('ctl00_CPH1_txtNombrePersonal', 'ctl00_CPH1_lblNombre');
        panPintarError = 'divKoNombrePersonal';
    }
    else if (pCampo == 'Apellidos') {
        error = ValidarApellidos('ctl00_CPH1_txtApellidos', 'ctl00_CPH1_lblApellidos');
        panPintarError = 'divKiApellidos';
    }
    else if (pCampo == 'Provincia') {
        error2 = ValidarProvincia('ctl00_CPH1_txtProvincia', 'ctl00_CPH1_lblProvincia', 'ctl00_CPH1_editProvincia');
        panPintarError = 'divKoProvincia';
    }
    else if (pCampo == 'Sexo') {
        error2 = ValidarSexo('ctl00_CPH1_editSexo', 'ctl00_CPH1_lblSexo');
        panPintarError = 'divKoSexo';
    }
    else if (pCampo == 'CentroEstudios') {
        error2 = ValidarCentroEstudios('ctl00_CPH1_txtCentroEstudios', 'ctl00_CPH1_lbCentroEstudios');
        panPintarError = 'divKoCentroEstudios';
    }
    else if (pCampo == 'AreaEstudios') {
        error2 = ValidarAreaEstudios('ctl00_CPH1_txtAreaEstudios', 'ctl00_CPH1_lbAreaEstudios');
        panPintarError = 'divKoAreaEstudios';
    }

    if (error != '') {
        //crearError('<ul>'+error+'</ul>', '#ctl00_CPH1_datosUsuario');
        crearError(error, '#' + panPintarError);
    }
    else if (error2 == '') {
        LimpiarHtmlControl(panPintarError);
    }

    if (error2 != '') {
        //crearError('<ul>'+error2+'</ul>', '#fielDatosPersonales div.textBox:first');
        crearError(error2, '#' + panPintarError);
    }
    else if (error == '') {
        LimpiarHtmlControl(panPintarError);
    }
}

function ValidarNombreUsu(pTxtNombre, pLlbNombre) {
    var error = '';
    var RegExPatternUsuario = /^([a-zA-Z0-9-_.ñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ'´`çÇ]{4,12})$/;

    // primero comprobamos los campos
    var longUsu = $('#' + pTxtNombre).val().length;

    if ((longUsu < 4) || (longUsu > 12)) {
        error += '<p>' + form.longitudUsuario + '</p>';
        $('#' + pLlbNombre).attr('class', 'ko');
    }
    else if (!$('#' + pTxtNombre).val().match(RegExPatternUsuario) || $('#' + pTxtNombre).val() == '') {
        error += '<p>' + form.formatoUsuario + '</p>';
        $('#' + pLlbNombre).attr('class', 'ko');
    }
    else {
        $('#' + pLlbNombre).attr('class', '');
    }

    return error;
}

/**
 * Validar la contrase�a del lusuario en el proceso de registro.
 * Si es correcto, se a�ade una clase "is-valid"
 * Si es incorrecto (vac�o) se a�ade una clase "is-invalid"
 * @param {any} pTxtContra
 * @param {any} pTxtContra2
 * @param {any} pLblContra
 * @param {any} pLblContra2
 * @param {any} pValidarContr2
 */
function ValidarContrasena(pTxtContra, pTxtContra2, pLblContra, pLblContra2, pValidarContr2) {
    var error = '';
    if ($('#' + pTxtContra).length > 0) {
        var RegExPatternPass = /(?!^[0-9]*$)(?!^[a-zA-ZñÑüÜ]*$)^([a-zA-ZñÑüÜ0-9#_$*]{6,12})$/;
        correcto = true;
        if (!$('#' + pTxtContra).val().match(RegExPatternPass)) {
            error += '<p>' + form.pwFormato + '</p>';
            correcto = false;
        }
        if (pValidarContr2 && $('#' + pTxtContra).val() != $('#' + pTxtContra2).val()) {
            error += '<p>' + form.pwIgual + '</p>';
            correcto = false;
        }
        if (correcto) {
            //$('#' + pLblContra).attr('class', '');
            $('#' + pTxtContra).addClass('is-valid');
            $('#' + pTxtContra).removeClass('is-invalid');
            
            if (pValidarContr2) {
                //$('#' + pLblContra2).attr('class', '');
                $('#' + pTxtContra2).addClass('is-valid');
                $('#' + pTxtContra2).removeClass('is-invalid');
            }
        } else {
            //$('#' + pLblContra).attr('class', 'ko');
            $('#' + pTxtContra).addClass('is-invalid');
            $('#' + pTxtContra).removeClass('is-valid');
            if (pValidarContr2) {
                //$('#' + pLblContra2).attr('class', 'ko');
                $('#' + pTxtContra2).addClass('is-invalid');
                $('#' + pTxtContra2).removeClass('is-valid');
            }
        }
    }
    return error;
}

/**
 * Validar el campo email introducido para el proceso de registro.
 * Si es correcto, se a�ade una clase "is-valid"
 * Si es incorrecto (vac�o) se a�ade una clase "is-invalid"
 * @param {any} pTxtMail: El input donde se ha rellenado con datos
 * @param {any} pLblMail: El label asociado a dicho input
 */
function ValidarEmailIntroducido(pTxtMail, pLblMail) {
    var error = '';

    if (!validarEmail($('#' + pTxtMail).val())) {
        error += '<p>' + form.emailValido + '</p>';
        //$('#' + pLblMail).attr('class', 'ko');
        $('#' + pTxtMail).addClass('is-invalid');
        $('#' + pTxtMail).removeClass('is-valid');
    } else {
        //$('#' + pLblMail).attr('class', '');
        $('#' + pTxtMail).addClass('is-valid');
        $('#' + pTxtMail).removeClass('is-invalid');
    }

    return error;
}

/**
 * Validar el nombre de la persona en el proceso de registro.
 * Si es correcto, se a�ade una clase "is-valid"
 * Si es incorrecto (vac�o) se a�ade una clase "is-invalid"
 * @param {any} pNombrePersona: El inputText
 * @param {any} pLblNombrePersona: El label relacionado correspondiente con el inputText
 */
function ValidarNombrePersona(pNombrePersona, pLblNombrePersona) {
    var error = '';

    if ($('#' + pNombrePersona).val().length == 0) {
        error += '<p>' + form.camposVacios + '</p>';
        // $('#' + pLblNombrePersona).attr('class', 'ko');
        $('#' + pNombrePersona).addClass('is-invalid');
        $('#' + pNombrePersona).removeClass('is-valid');
    } else {
        // $('#' + pLblNombrePersona).attr('class', '');        
        $('#' + pNombrePersona).addClass('is-valid');
        $('#' + pNombrePersona).removeClass('is-invalid');
    }

    //Comprobamos el formato de nombre de usuario
    var nombreUsuario = document.getElementById(pNombrePersona).value;
    var RegExPatternNombreUsuario = ExpresionRegularNombres;
    if (!nombreUsuario.match(RegExPatternNombreUsuario)) {
        error += '<p>' + form.formatoNombre + '</p>';
        $('#' + pLblNombrePersona).attr('class', 'ko');
    }

    if (nombreUsuario.match(ExpresionRegularCaracteresRepetidos)) {
        error += '<p>' + form.formatoNombreCaracteresRepetidos + '</p>';
        $('#' + pLblNombrePersona).attr('class', 'ko');
    }

    return error;
}

function ValidarFechaNacimiento(pDiaFechaNacimiento, pMesFechaNacimiento, pAnioFechaNacimiento, pLblFechaNacimiento) {
    var error = '';
    dia = document.getElementById(pDiaFechaNacimiento).value;
    mes = document.getElementById(pMesFechaNacimiento).value;
    anio = document.getElementById(pAnioFechaNacimiento).value;

    fecha = new Date(anio, mes, dia);
    hoy = new Date();

    mayor = false;

    if (dia > 0 && mes > 0 && anio > 0) {
        if ((hoy.getFullYear() - fecha.getFullYear()) > 18) {
            //Los ha cumplido en algún año anterior
            mayor = true;
        }
        else if ((hoy.getFullYear() - fecha.getFullYear()) == 18) {
            if (hoy.getMonth() > fecha.getMonth()) {
                //Los ha cumplido en algún mes anterior
                mayor = true;
            }
                //Los cumple durante el año en el que estamos
            else if (hoy.getMonth() == fecha.getMonth()) {
                //Los cumple durante el mes en el que estamos        
                if (hoy.getDate() >= fecha.getDate()) {
                    //Ya los ha cumplido
                    mayor = true;
                }
            }
        }
    }

    if (mayor) {
        document.getElementById(pLblFechaNacimiento).style.color = "";
    } else {
        error += '<p>' + form.fechanacincorrecta + '</p>';
        $('#' + pLblFechaNacimiento).attr('class', 'ko');
    }

    return error;
}

/**
 * Validar los apellidos de la persona en el proceso de registro.
 * Si es correcto, se a�ade una clase "is-valid"
 * Si es incorrecto (vac�o) se a�ade una clase "is-invalid"
 * @param {any} pTxtApellidos: El input de los apellidos
 * @param {any} pLblApellidos: El label asociado a los apellidos
 */
function ValidarApellidos(pTxtApellidos, pLblApellidos) {
    var error = '';

    if ($('#' + pTxtApellidos).val().length == 0) {
        error += '<p>' + form.camposVacios + '</p>';
        //$('#' + pLblApellidos).attr('class', 'ko');
        $('#' + pTxtApellidos).addClass('is-invalid');
        $('#' + pTxtApellidos).removeClass('is-valid');
    } else {
        //$('#' + pLblApellidos).attr('class', '');
        $('#' + pTxtApellidos).addClass('is-valid');
        $('#' + pTxtApellidos).removeClass('is-invalid');
    }

    //Comprobamos el formato de los apellidos del usuario
    var apellidosUsuario = document.getElementById(pTxtApellidos).value;
    //var RegExPatternApellidosUsuario = ExpresionRegularNombres;
    var RegExPatternApellidosUsuario = ExpresionRegularNombres;
    if (!apellidosUsuario.match(RegExPatternApellidosUsuario)) {
        error += '<p>' + form.formatoApellidos + '</p>';
        $('#' + pLblApellidos).attr('class', 'ko');
    }

    if (apellidosUsuario.match(ExpresionRegularCaracteresRepetidos)) {
        error += '<p>' + form.formatoApellidosCaracteresRepetidos + '</p>';
        $('#' + pLblApellidos).attr('class', 'ko');
    }

    return error;
}

function ValidarPais(pDDLPais, pLblEditPais) {
    var error = '';

    if ((document.getElementById(pDDLPais) != null) && (document.getElementById(pDDLPais).selectedIndex == 0)) {
        error += '<p>' + form.paisincorrecto + '</p>';
        $('#' + pLblEditPais).attr('class', 'ko');
    }
    else {
        $('#' + pLblEditPais).attr('class', '');
    }

    return error;
}

function ValidarProvincia(pTxtProvincia, pLblProvincia, pEditProvincia) {
    var error = '';

    if ($('#' + pTxtProvincia).val().length == 0 && document.getElementById(pTxtProvincia).style.display == '') {
        $('#' + pLblProvincia).attr('class', 'ko');
    } else {
        $('#' + pLblProvincia).attr('class', '');
    }

    //Comprobamos provincia
    if (document.getElementById(pTxtProvincia).style.display == "") {
        var provinciaUsuario = document.getElementById(pTxtProvincia).value;
        var RegExPatternprovinciaUsuario = /<|>$/;
        if (provinciaUsuario.match(RegExPatternprovinciaUsuario)) {
            error += '<p>' + form.formatoProvincia + '</p>';
            $('#' + pLblProvincia).attr('class', 'ko');
        }
    }

    if ((document.getElementById(pEditProvincia).style.display == '') && (document.getElementById(pEditProvincia).selectedIndex == 0)) {
        error += '<p>' + form.provinciaincorrecta + '</p>';
        $('#' + pLblProvincia).attr('class', 'ko');
    }
    if (($('#' + pTxtProvincia).val() == '') && (document.getElementById(pTxtProvincia).style.display == "")) {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblProvincia).attr('class', 'ko');
    }

    return error;
}

function ValidarSexo(pNombrePersona, pLblNombrePersona) {
    var error = '';

    //Comprobamos el sexo
    if (document.getElementById(pNombrePersona).selectedIndex == 0) {
        error += '<p>' + form.sexoincorrecto + '</p>';
        $('#' + pLblNombrePersona).attr('class', 'ko');
    } else {
        $('#' + pLblNombrePersona).attr('class', '');
    }

    return error;
}

/**
 * Validar que el campo no está vacío (Ej: Proceso de registro, el campo "Cargo").
 * Si es correcto, se añade una clase "is-valid"
 * Si es incorrecto (vacío) se añade una clase "is-invalid"
* @param {any} pTxt: El inputText
 * @param {any} pLbl: El label relacionado correspondiente con el inputText
 * @param {bool} jqueryElement: Se indica si el elemento pasado est� ya en formato jquery, por lo que no hace falta analizarlo por ID
 */
function ValidarCampoNoVacio(pTxt, pLbl, jqueryElement = false) {
    var error = '';

    // Se ha pasado el elemento jquery, no el ID directamente
    if (jqueryElement == true) {
        if (pTxt.val() == '') {
            pTxt.addClass('is-invalid');
            error += '<p>' + form.camposVacios + '</p>';
        }
    } else {
        //Si es profesro comprobamos centro de estudios y area de estudios:
        if (document.getElementById(pTxt) != null && $('#' + pTxt).val() == '') {
            error += '<p>' + form.camposVacios + '</p>';
            //$('#' + pLbl).attr('class', 'ko');
            $('#' + pTxt).addClass('is-invalid');
            $('#' + pTxt).removeClass('is-valid');
        }
        else if (document.getElementById(pLbl) != null) {
            //$('#' + pLbl).attr('class', '');
            $('#' + pTxt).addClass('is-valid');
            $('#' + pTxt).removeClass('is-invalid');
        }
    }

    return error;
}

function ValidarCentroEstudios(pTxtCentro, pLblCentro) {
    var error = '';

    //Si es profesro comprobamos centro de estudios y area de estudios:
    if (document.getElementById(pTxtCentro) != null && $('#' + pTxtCentro).val() == '') {
        error += '<p>' + form.centroestudiosincorrecto + '</p>';
        $('#' + pLblCentro).attr('class', 'ko');
    }
    else if (document.getElementById(pLblCentro) != null) {
        $('#' + pLblCentro).attr('class', '');
    }

    return error;
}

function ValidarAreaEstudios(pTxtArea, pLblArea) {
    var error = '';

    if (document.getElementById(pTxtArea) != null && $('#' + pTxtArea).val() == '') {
        error += '<p>' + form.areaestudiosincorrecto + '</p>';
        $('#' + pLblArea).attr('class', 'ko');
    }
    else if (document.getElementById(pLblArea) != null) {
        $('#' + pLblArea).attr('class', '');
    }

    return error;
}

function fechaMayorQueLimite(pDiaSeleccionado, pMesSeleccionado, pAnioSeleccionado, pIDRelativoHacks) {
    var diaLimite = document.getElementById(pIDRelativoHacks + 'txtHackDiaLimite').value;
    var mesLimite = document.getElementById(pIDRelativoHacks + 'txtHackMesLimite').value;
    var anioLimite = document.getElementById(pIDRelativoHacks + 'txtHackAnioLimite').value;

    if (diaLimite.length < 2) {
        diaLimite = '0' + diaLimite;
    }

    if (mesLimite.length < 2) {
        mesLimite = '0' + mesLimite;
    }

    if (pDiaSeleccionado.length < 2) {
        pDiaSeleccionado = '0' + pDiaSeleccionado;
    }

    if (pMesSeleccionado.length < 2) {
        pMesSeleccionado = '0' + pMesSeleccionado;
    }

    if (pAnioSeleccionado > anioLimite) {
        return false;
    }
    else if (pAnioSeleccionado == anioLimite) {
        if (pMesSeleccionado > mesLimite) {
            return false;
        }
        else if (pMesSeleccionado == mesLimite) {
            if (pDiaSeleccionado > diaLimite) {
                return false;
            }
        }
    }

    return true;
}

function ComprobarFechaNac(pTxtDia, pTxtMes, pTxtAnio, pLblFechaNac, pLblEdadMinima, pInicioIDs) {
    var error = '';

    //Comprobamos la fecha
    var diaSeleccionado = document.getElementById(pTxtDia).selectedIndex;
    var mesSeleccionado = document.getElementById(pTxtMes).selectedIndex;
    var anioSeleccionado = document.getElementById(pTxtAnio).options[document.getElementById(pTxtAnio).selectedIndex].value;
    $('#' + pLblFechaNac).attr('class', '');
    if (!esFecha(diaSeleccionado, mesSeleccionado, anioSeleccionado)) {
        error += '<p>' + form.fechanacincorrecta + '</p>';
        $('#' + pLblFechaNac).attr('class', 'ko');
    }
    else if (!fechaMayorQueLimite(diaSeleccionado, mesSeleccionado, anioSeleccionado, pInicioIDs)) {
        //Si esta la fecha minima es un alumno, es decir 14 años la edad mínima
        if (document.getElementById(pLblEdadMinima) != null) {
            error += '<p>' + form.fechanacinsuficiente + '</p>';
        } else {
            error += '<p>' + form.fechanacinsuficiente18 + '</p>';
        }
    }

    return error;
}

function ValidarNombreCortoUsuario(nombre) {
    var RegExPatternNombreCorto = ExpresionNombreCorto;
    return (nombre.match(RegExPatternNombreCorto) && nombre != '');
}

function ValidarNombreCortoCentroYAsignatura(nombre) {
    var RegExPatternNombreCorto = ExpresionNombreCortoCentro;
    return (nombre.match(RegExPatternNombreCorto) && nombre != '');
}

function ValidarNombreCortoCurso(nombre) {
    var RegExPatternNombreCorto = /^([0-9]{1,10})$/;
    return (nombre.match(RegExPatternNombreCorto) && nombre != '');
}

function pasarSegundaParte() {
    $dC = $('#ctl00_CPH1_datosCuenta');
    $dC.fadeOut('', function () {
        //$('#registroUsuario').find('div.ko').remove();
        $('#ctl00_CPH1_datosPersonales').fadeIn();
    });
    $('#queEsEstoParte1').fadeOut('', function () {
        $('#queEsEstoParte2').fadeIn();
    });

}

function pasarPrimeraParte() {
    $dC = $('#ctl00_CPH1_datosPersonales');
    $dC.fadeOut('', function () {
        //document.getElementById('divErrores').className = '';
        $('#ctl00_CPH1_datosCuenta').fadeIn();
    });
    $('#queEsEstoParte2').fadeOut('', function () {
        $('#queEsEstoParte1').fadeIn();
    });
}

function registroUsuario_2(IdCaptcha, funcionComprobar) {
    if (!(document.getElementById('ctl00_CPH1_lblErrorFoto') == null || document.getElementById('ctl00_CPH1_lblErrorFoto').innerHTML == '')) {
        return false;
    }
    var $dP = $('#fielDatosPersonales'),
error = '';
    // comprobamos el anterior fieldset, por si el usuario trastea por el DOM como si se creyese Tarzan
    //	if (!registroUsuario_1(true))
    //	{
    //		$('#registroUsuario button.anterior:eq(0)').click();
    //		return false;
    //	}
    // ahora comprobamos los campos personales
    $dP.find('label:contains(*)').each(function () {
        // todos aquellos label que contengan un asterisco:
        var $label = $(this);
        var $input = $label.next();
        if (!$input.val().length) {
            error += '<p>' + $label.text().replace('*', '') + form.obligatorio + '</p>';
        }
    });

    error += AgregarErrorReg(error, ValidarProvincia('ctl00_CPH1_txtProvincia', 'ctl00_CPH1_lblProvincia', 'ctl00_CPH1_editProvincia'));

    error += AgregarErrorReg(error, ValidarSexo('ctl00_CPH1_editSexo', 'ctl00_CPH1_lblSexo'));

    //    //Comprobamos provincia
    //    if(document.getElementById('ctl00_CPH1_txtProvincia').style.display==""){
    //	    var provinciaUsuario = document.getElementById('ctl00_CPH1_txtProvincia').value;
    //	    var RegExPatternprovinciaUsuario = /<|>$/;
    //	    if (provinciaUsuario.match(RegExPatternprovinciaUsuario))
    //	    {
    //		    error += '<li>'+form.formatoProvincia+'</li>';
    //		    document.getElementById('ctl00_CPH1_lblProvincia').style.color="red";
    //	    }
    //	}
    //	//Comprobamos codigopostal
    //	var CPUsuario = document.getElementById('ctl00_CPH1_txtCodigoPost').value;
    //	var RegExPatternCPUsuario = /<|>$/;
    //	if (CPUsuario.match(RegExPatternCPUsuario))
    //	{
    //		error += '<li>'+form.formatoCP+'</li>';
    //		document.getElementById('ctl00_CPH1_lblCP').style.color="red";
    //	}
    //	//Comprobamos localidad
    //	var localidadUsuario = document.getElementById('ctl00_CPH1_txtPoblacion').value;
    //	var RegExPatternlocalidadUsuario = /<|>$/;
    //	if (localidadUsuario.match(RegExPatternlocalidadUsuario))
    //	{
    //		error += '<li>'+form.formatoLocalidad+'</li>';
    //		document.getElementById('ctl00_CPH1_lblPoblacion').style.color="red";
    //	}

    error += AgregarErrorReg(error, ValidarCentroEstudios('ctl00_CPH1_txtCentroEstudios', 'ctl00_CPH1_lbCentroEstudios'));

    error += AgregarErrorReg(error, ValidarAreaEstudios('ctl00_CPH1_txtAreaEstudios', 'ctl00_CPH1_lbAreaEstudios'));

    error += AgregarErrorReg(error, ComprobarFechaNac('ctl00_CPH1_editDia', 'ctl00_CPH1_editMes', 'ctl00_CPH1_editAnio', 'ctl00_CPH1_lblFechaNac', 'ctl00_CPH1_lblEdadMinima', 'ctl00_CPH1_'));

    if (error.length) {
        crearError(error, '#fielDatosPersonales div.textBox:first', true);
        $('body, html').animate({ scrollTop: $dP.offset().top }, 600);
        return false;
    }
    else {
        IdCaptcha = '#' + IdCaptcha;
        funcionComprobar = funcionComprobar.replace("$$", "&" + $(IdCaptcha).val());
        eval(funcionComprobar);
    }
}

function registroProfesor(funcionComprobar) {
    var error = '';

    // primero comprobamos los campos
    if (!validarEmail($('#ctl00_CPH1_txtEmail').val())) {
        error += '<p>' + form.emailValido + '</p>';
    }

    if ($('#ctl00_CPH1_txtCentroEstudios').val() == '') {
        error += '<p>' + form.centroestudiosincorrecto + '</p>';
    }

    if ($('#ctl00_CPH1_txtAreaEstudios').val() == '') {
        error += '<p>' + form.areaestudiosincorrecto + '</p>';
    }

    if (error.length) {
        crearError(error, '#ctl00_CPH1_datosCuenta div.reg', true);
        return false;
    } else {
        funcionComprobar = funcionComprobar.replace("$$", "&" + $('#ctl00_CPH1_txtEmail').val() + "," + $('#ctl00_CPH1_txtCentroEstudios').val() + "," + $('#ctl00_CPH1_txtAreaEstudios').val());
        eval(funcionComprobar);
        //		// ... de otro modo ocultamos y mostramos las capas pertinentes y...
        //		$dC.fadeOut('', function() {
        //			$('#registroUsuario').find('div.ko').remove();
        //			$('#datosPersonales').fadeIn();
        //		});
        //		// ... evitamos el comportamiento normal del botoncico
        return false;
    }
}

// asignamos los eventos
$(function () {
    //$('#registroUsuario #datosCuenta button.siguiente').click(registroUsuario_1);
    $('#registroUsuario').submit(function () {
        return registroUsuario_2();
    });
    // boton anterior
    $('#wrap').filter('.registro').find('button.anterior').click(function () {
        var $this = $(this);
        $this.parents('form').find('fieldset:visible').fadeOut('', function () {
            $this.parents('fieldset').prev('fieldset').fadeIn();
        });
        return false;
    });
});

function irAnteriorUsu() {
    $('#ctl00_CPH1_datosPersonales').fadeOut('', function () {
        $('#ctl00_CPH1_datosCuenta').fadeIn();
    });
    $('#queEsEstoParte2').fadeOut('', function () {
        $('#queEsEstoParte1').fadeIn();
    });

    return false;
}

//                                                                 Agregar datos de contacto
//------------------------------------------------------------------------------------------

function crearDatosContacto() {
    var $dP = $('#fielDatosPersonales'),
error = '';

    // ahora comprobamos los campos personales
    $dP.find('label:contains(*)').each(function () {
        // todos aquellos label que contengan un asterisco:
        var $label = $(this);
        var $input = $label.next();
        if (!$input.val().length) {
            error += '<p>' + $label.text().replace('*', '') + form.obligatorio + '</p>';
        }
    });

    //Comprobamos el formato de nombre de usuario
    var nombreUsuario = document.getElementById('ctl00_CPH1_txtNombrePersonal').value;
    var RegExPatternNombreUsuario = ExpresionRegularNombres;
    if (!nombreUsuario.match(RegExPatternNombreUsuario)) {
        error += '<p>' + form.formatoNombre + '</p>';
    }

    //Comprobamos el formato de los apellidos del usuario
    var apellidosUsuario = document.getElementById('ctl00_CPH1_txtApellidos').value;
    var RegExPatternApellidosUsuario = ExpresionRegularNombres;
    if (!apellidosUsuario.match(RegExPatternApellidosUsuario)) {
        error += '<p>' + form.formatoApellidos + '</p>';
    }

    if (document.getElementById('ctl00_CPH1_txtEmail').value != "") {
        if (!validarEmail(document.getElementById('ctl00_CPH1_txtEmail').value)) {
            error += '<p>' + form.emailValido + '</p>';
        }
    }

    if (error.length) {
        crearError(error, '#fielDatosPersonales div.textBox:first', true);
        $('body, html').animate({ scrollTop: $dP.offset().top }, 600);
        return false;
    }
    else {
        eval(document.getElementById('ctl00_CPH1_btnHackCrearDatos').href);
    }
}

//                                                                             Suscripciones
//------------------------------------------------------------------------------------------

function confirmarSuscripcion(control, accion, nombre) {
    var $this = $('#' + control);
    var texto = dialogo.confirmarSuscripcion.replace("@1@", nombre);
    mascaraCancelar(texto, $this, accion);
}

function eliminarSuscripcion(control, accion, nombre) {
    var $this = $('#' + control);
    var texto = dialogo.eliminarSuscripcion.replace("@1@", nombre);
    mascaraCancelar(texto, $this, accion);
}
function confirmarSuscripcionBlog(control, accion, nombre) {
    var $this = $('#' + control);
    var texto = dialogo.confirmarSuscripcionBlog.replace("@1@", nombre);
    mascaraCancelar(texto, $this, accion);
}

function eliminarSuscripcionBlog(control, accion, nombre) {
    var $this = $('#' + control);
    var texto = dialogo.eliminarSuscripcionBlog.replace("@1@", nombre);
    mascaraCancelar(texto, $this, accion);
}

//                                                                    listado de comunidades
//------------------------------------------------------------------------------------------
$(function () { // ejemplo de borrado de comunidad
    $('#wrap').filter('.comunidades').find('ul.imagenAsociada a.cancelarSuscr').click(function () {
        var $this = $(this);
        var $li = $this.parents('li').eq(0);
        mascaraCancelar(borr.suscripcion, $li.get(0), function () {
            alert('Aqui iria algun tipo de funcion para borrar "' + $li.find('h3').text() + '" de la base de datos o de donde se crea conveniente.');
        });
    });
});

//                                                                          listado de blogs
//------------------------------------------------------------------------------------------
$(function () { // ejemplo de borrado de suscripcion
    $('#wrap').filter('.blogs').find('ul.imagenAsociada a.cancelarSuscr').click(function () {
        var $this = $(this);
        var $li = $this.parents('li').eq(0);
        mascaraCancelar(borr.suscripcion, $li.get(0), function () {
            alert('Aqui iria algun tipo de funcion para borrar "' + $li.find('h3').text() + '" de la base de datos o de donde se crea conveniente.');
        });
    });
});

//                                                                     listado de borradores
//------------------------------------------------------------------------------------------
/*$( function() { // ejemplo de borrado de borrador
$('#wrap').filter('.borradores').find('ul.imagenAsociada a.cancelar').click( function() {
var $this = $(this); 
var $li = $this.parents('li').eq(0);
mascaraCancelar(borr.borrador, $li.get(0), function() {
alert('Aqui iria algun tipo de funcion para borrar "'+$li.find('h3').text()+'" de la base de datos o de donde se crea conveniente.');
});
});
});*/

function borrarBorrador(control, accion) {
    var $this = $(control);
    var $li = $this.parents('li').eq(0);
    mascaraCancelar(borr.borrador, $li.get(0), accion);
}
//                                                                 listado de suscripciones
//------------------------------------------------------------------------------------------
/*$( function() { // ejemplo de borrado de suscripcion
$('#wrap').filter('.suscripciones').find('ul.imagenAsociada a.cancelarSuscr').click( function() {
var $this = $(this); 
var $li = $this.parents('li').eq(0);
mascaraCancelar(borr.suscripcion, $li.get(0), function() {
alert('Aqui iria algun tipo de funcion para borrar "'+$li.find('h3').text()+'" de la base de datos o de donde se crea conveniente.');
});
});
});
*/
function borrarSuscripcion(control, accion) {
    var $this = $(control);
    var $li = $this.parents('li').eq(0);
    mascaraCancelar(borr.suscripcion, $li.get(0), accion);
}
//                                                                   listado de comentarios
//------------------------------------------------------------------------------------------

/* 
$( function() { // ejemplo de borrado de comentarios
$('#wrap').filter('.comentarios').find('ul.imagenAsociada a.cancelar').click( function() {
var $this = $(this); 
var $li = $this.parents('li').eq(0);
mascaraCancelar(borr.comentario, $li.get(0), function() {
alert('Aqui iria algun tipo de funcion para borrar "'+$li.find('h3').text()+'" de la base de datos o de donde se crea conveniente.');
});
});
});
*/
function borrarComentario(control, accion) { // ejemplo de borrado de comentarios
    var $this = $(control);
    var $li = $this.parents('li').eq(0);
    mascaraCancelar(borr.comentario, $li.get(0), accion);
}

//function AccionFichaPersona(texto,id,accion) { 
//		var $li = $(document.getElementById(id)); 
//		mascaraCancelar(texto, $li, accion);
//}


/**
 * Acci�n que se ejecuta cuando se pulsa sobre las acciones disponibles de un item/recurso de tipo "Perfil" encontrado por el buscador.
 * Las acciones que se podr�an realizar son (No/Enviar newsletter, No/Bloquear). Acciones tambi�n de vincular, desvincular recurso...
 * @param {string} titulo: T�tulo que tendr� el panel modal
 * @param {any} textoBotonPrimario: Texto del bot�n primario
 * @param {any} textoBotonSecundario: Texto del bot�n primario
 * @param {string} texto: El texto o mensaje a modo de t�tulo que se mostrar� para que el usuario sepa la acci�n que se va a realizar
 * @param {string} id: Identificador del recurso/persona sobre el que se aplicar� la acci�n
 * @param {any} accion: Acci�n o funci�n que se ejecutar� cuando se pulse en el bot�n de primario
 * @param {any} idModalPanel: Panel modal contenedor donde se insertar� este HTML (Por defecto ser� #modal-container)
 */
function AccionFichaPerfil(titulo, textoBotonPrimario, textoBotonSecundario, texto, id, accion, textoInferior=null, idModalPanel="#modal-container") {

    // Panel din�mico del modal padre donde se insertar� la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');     

    // Plantilla del panel html que se cargar� en el modal contenedor al pulsar en la acci�n
    var plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
        plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>'+ titulo +'</p>';
        plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
        plantillaPanelHtml += '<div class="formulario-edicion">';
            plantillaPanelHtml += '<div class="form-group">';
                plantillaPanelHtml += '<label class="control-label">'+texto+'</label>';
            plantillaPanelHtml += '</div>';
            if (textoInferior != null || textoInferior.length>5) {
                plantillaPanelHtml += '<div class="form-group">';
                    plantillaPanelHtml += '<label class="control-label">' + textoInferior + '</label>';
                plantillaPanelHtml += '</div>';
            }            
            plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
                // Posibles mensajes de info
                plantillaPanelHtml += '<div>';
                    plantillaPanelHtml += '<div id="menssages">';
                    plantillaPanelHtml += '<div class="ok"></div>';
                    plantillaPanelHtml += '<div class="ko"></div>';
                plantillaPanelHtml += '</div>';
            plantillaPanelHtml += '</div>';
            // Panel de botones para la acci�n
            plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
                plantillaPanelHtml += '<button data-dismiss="modal" class="btn btn-primary">'+textoBotonSecundario+'</button>'
                plantillaPanelHtml += '<button class="btn btn-outline-primary ml-1">'+textoBotonPrimario+'</button>'
            plantillaPanelHtml += '</div>';                       
        plantillaPanelHtml += '</div>';        
    plantillaPanelHtml += '</div>'; 

    // Meter el c�digo de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el c�digo
    $modalDinamicContentPanel.html(plantillaPanelHtml);       

    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');

    // Asignaci�n de la funci�n al bot�n "S�" o de acci�n
    $(botones[1]).on("click", function () {
      // Ocultar el panel modal de bootstrap - De momento estar� visible. Se ocultar�a si se muestra mensaje de OK pasados 1.5 segundos
        //$('#modal-container').modal('hide');
    }).click(accion);
}

function AccionCrearComentario(clientID, id, pCKECompleto) {
    var $c = $(document.getElementById("nuevo_" + id));

    document.getElementById("nuevo_" + id).className = 'comment-content';


    var $anterior = $c.children();

    if ($anterior.length > 0) {
        //Eliminar el anterior
        $anterior.remove();
    }

    var accion = "javascript:CrearComentario('" + clientID + "', '" + id + "');";
    var claseCK = 'cke comentarios';

    if (pCKECompleto) {
        claseCK = 'cke recursos'
    }

    var $confirmar = $(['<fieldset class="mediumLabels"><legend>', comentarios.publicarcomentario, '</legend><p><textarea class="' + claseCK + '" id="txtComentario_', id, '" rows="2" cols="20"></textarea><p><label class="error" id="error_', id, '"></label></p><input type="button" onclick="', accion, '" value="', comentarios.enviar, '" class="text medium"></p></fieldset>'].join(''));
    $confirmar.prependTo($c)
.find('button').click(function () { // Ambos botones hacen desaparecer la mascara
    $c.parents('.stateShowForm').css({ display: 'none' });
    $confirmar.css({ display: 'none' });
}).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada	

    RecargarTodosCKEditor();
}

function CrearComentario(clientID, id) {
    if ($('#txtComentario_' + id).val() != '') {
        var descripcion = $('#txtComentario_' + id).val().replace(/\|/g, '[-@@@1-]').replace(/,/g, '[-@@@2-]');
        MostrarUpdateProgress();

        var target = clientID.replace(/_/gi, "$");

        WebForm_DoCallback(target, 'ListadoComentarios_Editar|' + clientID + ',' + id + ',' + 'AgregarComentario' + ',' + document.getElementById('txtHackListDocDeComent').value + ',' + document.getElementById('txtHackMostrarSoloProyComent').value + ',' + descripcion, ReceiveServerData, '', null, false);
    }
    else {
        $('#error_' + id).html(comentarios.comentarioError);
    }
}

function AccionResponderComentario(clientID, id, pCKECompleto) {
    var $c = $(document.getElementById("respuesta_" + id));

    document.getElementById("respuesta_" + id).className = 'comment-content';

    document.getElementById(id).style.display = 'block';


    var $anterior = $c.children();

    if ($anterior.length > 0) {
        //Eliminar el anterior
        $anterior.remove();
    }

    var accion = "javascript:ResponderComentario('" + clientID + "', '" + id + "');";
    var claseCK = 'cke comentarios';

    if (pCKECompleto) {
        claseCK = 'cke recursos'
    }

    var $confirmar = $(['<fieldset class="mediumLabels"><legend>', comentarios.responderComentario, '</legend><p><textarea class="' + claseCK + '" id="txtComentario_', id, '" rows="2" cols="20"></textarea><p><label class="error" id="error_', id, '"></label></p><input type="button" onclick="', accion, '" value="', comentarios.enviar, '" class="text medium"></p></fieldset>'].join(''));
    $confirmar.prependTo($c)
.find('button').click(function () { // Ambos botones hacen desaparecer la mascara
    $c.parents('.stateShowForm').css({ display: 'none' });
    $confirmar.css({ display: 'none' });
}).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada	

    RecargarTodosCKEditor();
}

function ResponderComentario(clientID, id) {
    if ($('#txtComentario_' + id).val() != '') {
        var descripcion = $('#txtComentario_' + id).val().replace(/\|/g, '[-@@@1-]').replace(/,/g, '[-@@@2-]');
        MostrarUpdateProgress();

        var target = clientID.replace(/_/gi, "$");

        WebForm_DoCallback(target, 'ListadoComentarios_Editar|' + clientID + ',' + id + ',' + 'guardarRespuestaComentario' + ',' + document.getElementById('txtHackListDocDeComent').value + ',' + document.getElementById('txtHackMostrarSoloProyComent').value + ',' + descripcion, ReceiveServerData, '', null, false);
    }
    else {
        $('#error_' + id).html(comentarios.comentarioError);
    }
}

function AccionEditarComentario(clientID, id, pCKECompleto) {
    var $c = $(document.getElementById("respuesta_" + id));


    var mensajeAntiguo = document.getElementById(id).innerHTML;

    if (mensajeAntiguo.indexOf('<ul class="principal">') > -1) {
        // Si encuentra el elemento principal, es Inevery y tenemos la lista de responder, eliminar.... etc....
        mensajeAntiguo = mensajeAntiguo.substr(0, mensajeAntiguo.indexOf('<ul class="principal">'));
    }

    document.getElementById(id).style.display = 'none';

    document.getElementById("respuesta_" + id).className = 'comment-content';

    var $anterior = $c.children();

    if ($anterior.length > 0) {
        //Eliminar el anterior
        $anterior.remove();
    }

    var accion = "javascript:EditarComentario('" + clientID + "', '" + id + "');";
    var claseCK = 'cke comentarios';

    if (pCKECompleto) {
        claseCK = 'cke recursos'
    }

    var $confirmar = $(['<fieldset class="mediumLabels"><legend>', comentarios.editarComentario, '</legend><p><textarea class="' + claseCK + '" id="txtComentario_', id, '" rows="2" cols="20">' + mensajeAntiguo + '</textarea><p><label class="error" id="error_', id, '"></label></p><input type="button" onclick="', accion, '" value="', comentarios.guardar, '" class="text medium"></p></fieldset>'].join(''));
    $confirmar.prependTo($c)
.find('button').click(function () { // Ambos botones hacen desaparecer la mascara
    $c.parents('.stateShowForm').css({ display: 'none' });
    $confirmar.css({ display: 'none' });
}).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada	

    RecargarTodosCKEditor();
}

function EditarComentario(clientID, id) {
    if ($('#txtComentario_' + id).val() != '') {
        var descripcion = $('#txtComentario_' + id).val().replace(/\|/g, '[-@@@1-]').replace(/,/g, '[-@@@2-]');
        MostrarUpdateProgress();

        var target = clientID.replace(/_/gi, "$");
        WebForm_DoCallback(target, 'ListadoComentarios_Editar|' + clientID + ',' + id + ',' + 'guardarComentario' + ',' + document.getElementById('txtHackListDocDeComent').value + ',' + document.getElementById('txtHackMostrarSoloProyComent').value + ',' + descripcion, ReceiveServerData, '', null, false);
    }
    else {
        $('#error_' + id).html(comentarios.comentarioError);
    }
}

function AccionEnviarMensajeGrupo(clientID, id) {

    var $c = $(document.getElementById(id));

    if (CKEDITOR.instances["txtDescripcion_" + id] != null) {
        CKEDITOR.instances["txtDescripcion_" + id].destroy();
    }

    var $anterior = $c.children();

    if ($anterior.length > 0) {
        //Eliminar el anterior
        $anterior.remove();
    }

    var accion = "javascript:EnviarMensajeGrupo('" + clientID + "', '" + id + "');";

    var $confirmar = $(['<fieldset class="mediumLabels"><legend>', mensajes.enviarMensaje, '</legend><p><label for="txtAsunto_', id, '">', mensajes.asunto, '</label><input type="text" id="txtAsunto_', id, '" class="text big"></p><p><label for="txtDescripcion_', id, '">', mensajes.descripcion, '</label></p><p><textarea class="cke mensajes" id="txtDescripcion_', id, '" rows="2" cols="20"></textarea></p><p><label class="error" id="error_', id, '"></label></p><input type="button" onclick="', accion, '" value="', mensajes.enviar, '" class="text medium"></p></fieldset>'].join(''));
    //$confirmar.find('div').filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
    // Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
    $confirmar.prependTo($c)
.find('button').click(function () { // Ambos botones hacen desaparecer la mascara
    $c.parents('.stateShowForm').css({ display: 'none' });
    $confirmar.css({ display: 'none' });
}).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada

    MostrarPanelAccionDesp(clientID + "_desplegable_" + id, null);
    RecargarTodosCKEditor();
}


function AccionEnviarMensaje(clientID, id) {
    var $c = $(document.getElementById(id));

    if (CKEDITOR.instances["txtDescripcion_" + id] != null) {
        CKEDITOR.instances["txtDescripcion_" + id].destroy();
    }

    var $anterior = $c.children();

    if ($anterior.length > 0) {
        //Eliminar el anterior
        $anterior.remove();
    }

    var accion = "javascript:EnviarMensaje('" + clientID + "', '" + id + "');";

    var $confirmar = $(['<fieldset class="mediumLabels"><legend>', mensajes.enviarMensaje, '</legend><p><label for="txtAsunto_', id, '">', mensajes.asunto, '</label><input type="text" id="txtAsunto_', id, '" class="text big"></p><p><label for="txtDescripcion_', id, '">', mensajes.descripcion, '</label></p><p><textarea class="cke mensajes" id="txtDescripcion_', id, '" rows="2" cols="20"></textarea></p><p><label class="error" id="error_', id, '"></label></p><input type="button" onclick="', accion, '" value="', mensajes.enviar, '" class="text medium"></p></fieldset>'].join(''));
    //$confirmar.find('div').filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
    // Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
    $confirmar.prependTo($c)
.find('button').click(function () { // Ambos botones hacen desaparecer la mascara
    $c.parents('.stateShowForm').css({ display: 'none' });
    $confirmar.css({ display: 'none' });
}).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada

    if ($('#divContMensajesPerf').length > 0 && $('#divContMensajesPerf').html() == '') {
        $('#divContMensajesPerf').html($('#' + clientID + "_desplegable_" + id).parent().html());
        $('#' + clientID + "_desplegable_" + id).parent().html('');
    }

    MostrarPanelAccionDesp(clientID + "_desplegable_" + id, null);
    RecargarTodosCKEditor();
}


function AccionRechazarDesplegandoMensaje(clientID, textoRechazarConMensaje, textoRechazarSinMensaje) {
    //var $c = $(document.getElementById(clientID));

    if (CKEDITOR.instances["txtDescripcion_"] != null) {
        CKEDITOR.instances["txtDescripcion_"].destroy();
    }

    var id = "_rechazado";
    var accionSinMensaje = "javascript:RechazarSinMensaje();";
    var accionConMensaje = "javascript:RechazarConMensaje('" + clientID + "', '" + id + "');";

    var html = '<fieldset class="mediumLabels"><legend>' + mensajes.enviarMensaje + '</legend><p><label for="txtDescripcion' + id + '">' + mensajes.descripcion + '</label></p><p><textarea class="cke mensajes" id="txtDescripcion' + id + '" rows="2" cols="20"></textarea></p><p><label class="error" id="error' + id + '"></label></p><input type="button" onclick="' + accionConMensaje + '" value="' + textoRechazarConMensaje + '" class="text medium"><input type="button" onclick="' + accionSinMensaje + '" value="' + textoRechazarSinMensaje + '" class="text medium"></p></fieldset>';

    MostrarPanelAccionDesp(clientID, html);
    RecargarTodosCKEditor();
}

function RechazarSinMensaje() {
    WebForm_DoCallback('__Page', 'rechazarsinmensaje', ReceiveServerData, '', null, false);
}

function RechazarConMensaje(clientID, id) {
    if ($('#txtAsunto' + id).val() != '' && $('#txtDescripcion' + id).val() != '') {
        var descripcion = $('#txtDescripcion' + id).val().replace(/&/g, '[-|-]');
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', 'rechazarconmensaje&' + descripcion, ReceiveServerData, '', null, false);
    }
    else {
        $('#error' + id).html(mensajes.mensajeError);
    }
}


function EnviarMensajeGrupo(clientID, id) {
    if ($('#txtAsunto_' + id).val() != '' && $('#txtDescripcion_' + id).val() != '') {
        var asunto = $('#txtAsunto_' + id).val().replace(/&/g, '[-|-]');
        var descripcion = $('#txtDescripcion_' + id).val().replace(/&/g, '[-|-]');
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', 'FichaGrupo_EnviarMensaje&' + id + '&' + asunto + '&' + descripcion + '&' + clientID, ReceiveServerData, '', null, false);
        //AceptarPanelAccion(clientID + "_desplegable_" + id, true, mensajes.mensajeEnviado);
    }
    else {
        $('#error_' + id).html(mensajes.mensajeError);
    }
}

function AccionEditarNombreGrupo(texto, grupoId, textoInferior) {

    var $c = $(document.getElementById(grupoId));

    var $anterior = $c.children();

    if ($anterior.length > 0) {
        //Eliminar el anterior
        $anterior.remove();
    }

    var accion = "javascript:EditarNombreGrupo('" + grupoId + "');";

    var $confirmar = $(['<div><p>', texto, '</p><br><p class="small"><br>', textoInferior, '</p><br><input type="text" id=txtNombreGrupo_' + grupoId + '></input><input type="button" value="Aceptar" onclick="' + accion + '" class="btMini"></input></div>'].join(''));
    //$confirmar.find('div').filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
    // Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
    $confirmar.prependTo($c)
.find('button').click(function () { // Ambos botones hacen desaparecer la mascara
    $c.parents('.stateShowForm').css({ display: 'none' });
    $confirmar.css({ display: 'none' });
}).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada
}

function EditarNombreGrupo(grupoId) {

    var nombreGrupo = $('#txtNombreGrupo_' + grupoId).val().replace(/&/g, '[-|-]');

    contenedorNombre = document.getElementById('divNombre_' + grupoId);
    contenedorNombre.childNodes[0].textContent = nombreGrupo;

    CerrarPanelAccion('ListadoGenerico_controles_fichagrupo_ascx_desplegable_' + grupoId);
    WebForm_DoCallback('__Page', 'FichaGrupo_EditarNombreGrupo&' + grupoId + '&' + nombreGrupo + '&', null, '', null, false);
    //    AceptarPanelAccion("_desplegable_" + id, true, 'Grupo Editado');
}

function AccionEditarGrupo(grupoId) {

    WebForm_DoCallback('__Page', 'AgregarContactoAGrupo&' + grupoId + '&', ReceiveServerData, '', null, false);
}

function AccionFichaGrupo(texto, grupoId, accion, textoInferior) {
    var $c = $(document.getElementById(grupoId));

    var $anterior = $c.children();

    if ($anterior.length > 0) {
        //Eliminar el anterior
        $anterior.remove();
    }


    var $confirmar = $(['<div><p>', texto, '</p><br><p class="small"><br>', textoInferior, '</p><br><button onclick="return false;" class="btMini">', borr.si, '</button><button onclick="return false;" class="btMini">', borr.no, '</button></div>'].join(''));

    $confirmar.prependTo($c)
.find('button').click(function () { // Ambos botones hacen desaparecer la mascara
    $c.parents('.stateShowForm').css({ display: 'none' });
    $confirmar.css({ display: 'none' });
}).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada
}

function EnviarMensaje(clientID, id) {
    if ($('#txtAsunto_' + id).val() != '' && $('#txtDescripcion_' + id).val() != '') {
        var asunto = $('#txtAsunto_' + id).val().replace(/&/g, '[-|-]');
        var descripcion = $('#txtDescripcion_' + id).val().replace(/&/g, '[-|-]');
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', 'FichaPerfil_EnviarMensaje&' + id + '&' + asunto + '&' + descripcion + '&' + clientID, ReceiveServerData, '', null, false);
        //AceptarPanelAccion(clientID + "_desplegable_" + id, true, mensajes.mensajeEnviado);
    }
    else {
        $('#error_' + id).html(mensajes.mensajeError);
    }
}

function AccionFichaPersona(texto, id, accion, textoInferior) {
    var $li = $(document.getElementById(id));
    mascaraCancelar2(texto, $li, accion, textoInferior);
}

function AccionAlerta(texto, id) {
    var $li = $(document.getElementById(id));
    mascaraCancelar(texto, $li);
}

function MostrarPopUp(texto, control, accion) {
    var $this = $(control);
    var $li = $this.parents('li').eq(0);
    mascaraCancelar(texto, $li.get(0), accion);
}

function aceptarInvitacion(control, accion) {
    var $this = $(control);
    var $li = $this.parents('li').eq(0);
    mascaraCancelar(invitaciones.aceptar, $li.get(0), accion);
}

function ignorarInvitacion(control, accion) {
    var $this = $(control);
    var $li = $this.parents('li').eq(0);
    mascaraCancelar(invitaciones.ignorar, $li.get(0), accion);
}


//                                                      funciones asociadas a base recursos
//------------------------------------------------------------------------------------------
//$( function() {
//	$('#selectoresBase img.ascendente').click(function(){
//		if ('img/onUp.gif' == this.src) return false;
//		this.src = 'img/onUp.gif'; 
//		$('#selectoresBase img.descendente').attr('src', 'img/offDown.gif');
//		alert('Recargar el listado?');
//		return false;
//	});
//	$('#selectoresBase img.descendente').click(function(){
//		if ('img/onDown.gif' == this.src) return false;
//		this.src = 'img/onDown.gif'; 
//		$('#selectoresBase img.ascendente').attr('src', 'img/offUp.gif');
//		alert('Recargar el listado?');
//		return false;
//	});

//	$('#baseRecursos a:contains(Aadir recurso a categora), button:contains(Editar Categorias), #anyadirACategoria, #anyadirEditores').click( function() {
//		// meto esto aqui porque imagino que se tendra que llamar por Ajax al listado,
//		// bastar con inyectarlo como ultimo elemento del <body/>.
//		// Ademas, podeis darle un identificador al <a> para no tener que buscarlo por
//		// el DOM usando su contenido =)
//		var $capa = $('#capaModal');
//		$capa.find('div.mascara').height($(document).height());
//		$capa.find('form.anyadirCategorias').css('top', ($('html').attr('scrollTop') || $('body').attr('scrollTop') || 0) + 'px')
//		$capa.fadeIn();
//		// una vez llamado deberian prepararse los eventos
//		$capa.find('a.icoEliminar').unbind('click').click(function(){
//			$capa.fadeOut();
//		});
//		return false;
//	});

//	$('#baseRecursos a:contains(Crear categora)').click(function() {
//		// id. que anterior
//		$('#baseRecursos div.ko').remove()
//		$('#editarCategoria').slideUp();
//		$('#crearCategoria').slideDown().find('button[type=reset]').unbind('click').click(function(){
//			$('#crearCategoria').slideUp();
//		});
//		return false;
//	});
//	
//	$('#baseRecursos a:contains(Editar)').click(function() {
//		// id. que anterior
//		var $a = $('#baseRecursos div.listadoCategorias ul:gt(0) a.activo');
//		if(!$a.length){
//			crearError('<p>'+baseRec.noCategoria+'</p>','div.listadoCategorias');
//			return false;
//		}
//		$('#baseRecursos div.ko').remove();
//		$('#crearCategoria').slideUp();
//		$('#editarCategoria').slideDown().find('input').val($a.text().replace(/\(\d*\)/,'')).find('button[type=reset]').unbind('click').click(function(){
//			$('#editarCategoria').slideUp();
//		});
//		return false;
//	});
//});


/*---------     Modificado todo by Javier     --------------*/

function MostrarPopUpSelectorCategorias() {
    // meto esto aqui porque imagino que se tendra que llamar por Ajax al listado,
    // bastar con inyectarlo como ultimo elemento del <body/>.
    // Ademas, podeis darle un identificador al <a> para no tener que buscarlo por
    // el DOM usando su contenido =)

    var $capa = $('#capaModal');
    var $iframe = null;
    //var $mask = $capa.find('div.mascara').height($(document).height());
    var cssMascaraCom = {
        'height': '100%',
        'width': '100%',
        'position': 'fixed',
        'top': 0,
        'left': 0
    }
    var $mask = $capa.find('div.mascara').css(cssMascaraCom);
    //$capa.find('div.anyadirCategorias').css('top', ($('html').attr('scrollTop') || $('body').attr('scrollTop') || 0) + 'px')
    /*var cssMascaraComCat = {
    'position': 'fixed',
    'top': 100
    }
    $capa.find('div.anyadirCategorias').css(cssMascaraComCat);*/
    $capa.fadeIn();
    //    if ($.browser.msie && $.browser.version < 7) {
    //        $iframe = $('<iframe></iframe>').css({
    //            position: 'absolute',
    //            top: 0,
    //            left: '50%',
    //            zIndex: parseInt($mask.css('zIndex')) - 1,
    //            width: '1000px',
    //            marginLeft: '-500px',
    //            height: $mask.height(),
    //            filter: 'mask()'
    //        }).insertAfter($mask);
    //    }
    // una vez llamado deberian prepararse los eventos
    $capa.find('a.icoEliminar').unbind('click').click(function () {
        $capa.fadeOut();
        if ($iframe) { $iframe.remove(); }
    });
    return false;
}

function MostrarPopUpSelectorEditoresYCat(pCapa) {
    //Lo siguiente se utiliza para que el popup aparezca centrado en la pantalla del usuario
    var posY = "0px";
    if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {//si es chrome
        posY = document.body.scrollTop + (document.documentElement.clientHeight / 2);
    } else//si no es chrome
    {
        posY = document.documentElement.scrollTop + (document.documentElement.clientHeight / 2);
    }

    // meto esto aqui porque imagino que se tendra que llamar por Ajax al listado,
    // bastar con inyectarlo como ultimo elemento del <body/>.
    // Ademas, podeis darle un identificador al <a> para no tener que buscarlo por
    // el DOM usando su contenido =)

    //0 -> selector de categorias
    //1 -> selector de editores

    if (pCapa == 0) {
        document.getElementById('panEditores').style.display = 'none';
        document.getElementById('panSelectorLectores').style.display = 'none';
        if (document.getElementById('panCategorias') != null) {
            document.getElementById('panCategorias').style.display = 'block';
            document.getElementById('panCategorias').style.marginTop = posY - 300 + 'px';
        }
    }
    else if (pCapa == 1) {
        document.getElementById('panEditores').style.display = 'block';
        document.getElementById('panEditores').style.marginTop = posY - 300 + 'px';
        if (document.getElementById('panCategorias') != null) {
            document.getElementById('panCategorias').style.display = 'none';
        }
        document.getElementById('panSelectorLectores').style.display = 'none';
    }
    else {
        document.getElementById('panEditores').style.display = 'none';
        if (document.getElementById('panCategorias') != null) {
            document.getElementById('panCategorias').style.display = 'none';
        }
        document.getElementById('panSelectorLectores').style.display = 'block';
        document.getElementById('panSelectorLectores').style.marginTop = posY - 300 + 'px';
    }
    MostrarPopUpSelectorCategorias();

    /*var $capa = $('#capaModal');
    var $iframe = null;
    var $mask = $capa.find('div.mascara').height($(document).height());
    $capa.find('div.anyadirCategorias').css('top', ($('html').attr('scrollTop') || $('body').attr('scrollTop') || 0) + 'px')
    $capa.fadeIn();
    if ($.browser.msie && $.browser.version < 7) {
    $iframe = $('<iframe></iframe>').css({
    position:'absolute',
    top:0,
    left:'50%',
    zIndex:parseInt($mask.css('zIndex')) - 1,
    width:'1000px',
    marginLeft:'-500px',
    height:$mask.height(),
    filter:'mask()'
    }).insertAfter($mask);
    }
    // una vez llamado deberian prepararse los eventos
    $capa.find('a.icoEliminar').unbind('click').click(function(){
    $capa.fadeOut();
    if ($iframe) {$iframe.remove();}
    });
    return false;*/
}

function MostrarSelectorCategorias() {
    // meto esto aqui porque imagino que se tendra que llamar por Ajax al listado,
    // bastar con inyectarlo como ultimo elemento del <body/>.
    // Ademas, podeis darle un identificador al <a> para no tener que buscarlo por
    // el DOM usando su contenido =)

    if (document.getElementById('panCompartirDocPopUp') != null) {
        document.getElementById('panCompartirDocPopUp').style.display = 'none';
    }
    if (document.getElementById('panAgregarCatDocPopUp') != null) {
        document.getElementById('panAgregarCatDocPopUp').style.display = 'block';
    }
    if (document.getElementById('panAgregarTagDocPopUp') != null) {
        document.getElementById('panAgregarTagDocPopUp').style.display = 'none';
    }

    if (document.getElementById('panInfoEnvioTwitterPopUp') != null) {
        document.getElementById('panInfoEnvioTwitterPopUp').style.display = 'none';
    }
    MostrarPopUpSelectorCategorias();

    /*var $capa = $('#capaModal');
    var $iframe = null;
    var $mask = $capa.find('div.mascara').height($(document).height());
    $capa.find('div.anyadirCategorias').css('top', ($('html').attr('scrollTop') || $('body').attr('scrollTop') || 0) + 'px')
    $capa.fadeIn();
    if ($.browser.msie && $.browser.version < 7) {
    $iframe = $('<iframe></iframe>').css({
    position:'absolute',
    top:0,
    left:'50%',
    zIndex:parseInt($mask.css('zIndex')) - 1,
    width:'1000px',
    marginLeft:'-500px',
    height:$mask.height(),
    filter:'mask()'
    }).insertAfter($mask);
    }
    // una vez llamado deberian prepararse los eventos
    $capa.find('a.icoEliminar').unbind('click').click(function(){
    $capa.fadeOut();
    if ($iframe) {$iframe.remove();}
    });
    return false;*/
}

function MostrarEditorTags() {
    // meto esto aqui porque imagino que se tendra que llamar por Ajax al listado,
    // bastar con inyectarlo como ultimo elemento del <body/>.
    // Ademas, podeis darle un identificador al <a> para no tener que buscarlo por
    // el DOM usando su contenido =)

    document.getElementById('panCompartirDocPopUp').style.display = 'none';
    document.getElementById('panAgregarCatDocPopUp').style.display = 'none';
    document.getElementById('panAgregarTagDocPopUp').style.display = 'block';

    if (document.getElementById('panInfoEnvioTwitterPopUp') != null) {
        document.getElementById('panInfoEnvioTwitterPopUp').style.display = 'none';
    }

    MostrarPopUpSelectorCategorias();

    /*var $capa = $('#capaModal');
    var $iframe = null;
    var $mask = $capa.find('div.mascara').height($(document).height());
    $capa.find('div.anyadirCategorias').css('top', ($('html').attr('scrollTop') || $('body').attr('scrollTop') || 0) + 'px')
    $capa.fadeIn();
    if ($.browser.msie && $.browser.version < 7) {
    $iframe = $('<iframe></iframe>').css({
    position:'absolute',
    top:0,
    left:'50%',
    zIndex:parseInt($mask.css('zIndex')) - 1,
    width:'1000px',
    marginLeft:'-500px',
    height:$mask.height(),
    filter:'mask()'
    }).insertAfter($mask);
    }
    // una vez llamado deberian prepararse los eventos
    $capa.find('a.icoEliminar').unbind('click').click(function(){
    $capa.fadeOut();
    if ($iframe) {$iframe.remove();}
    });
    return false;*/
}

function MostrarCompartidorDocumentos() {
    // meto esto aqui porque imagino que se tendra que llamar por Ajax al listado,
    // bastar con inyectarlo como ultimo elemento del <body/>.
    // Ademas, podeis darle un identificador al <a> para no tener que buscarlo por
    // el DOM usando su contenido =)

    document.getElementById('panCompartirDocPopUp').style.display = 'block';
    if (document.getElementById('panAgregarCatDocPopUp') != null) {
        document.getElementById('panAgregarCatDocPopUp').style.display = 'none';
    }
    if (document.getElementById('panAgregarTagDocPopUp') != null) {
        document.getElementById('panAgregarTagDocPopUp').style.display = 'none';
    }

    if (document.getElementById('panInfoEnvioTwitterPopUp') != null) {
        document.getElementById('panInfoEnvioTwitterPopUp').style.display = 'none';
    }
    MostrarPopUpSelectorCategorias();
}

function MostrarInfoEnvioTwitter() {
    if (document.getElementById('panInfoEnvioTwitterPopUp') != null) {
        document.getElementById('panInfoEnvioTwitterPopUp').style.display = 'block';
    }
    if (document.getElementById('panCompartirDocPopUp') != null) {
        document.getElementById('panCompartirDocPopUp').style.display = 'none';
    }
    if (document.getElementById('panAgregarCatDocPopUp') != null) {
        document.getElementById('panAgregarCatDocPopUp').style.display = 'none';
    }
    if (document.getElementById('panAgregarTagDocPopUp') != null) {
        document.getElementById('panAgregarTagDocPopUp').style.display = 'none';
    }
    MostrarPopUpSelectorCategorias();
}

function MostrarVinculadorDocumentos() {
    document.getElementById('panCompartirDocPopUp').style.display = 'none';
    document.getElementById('panAgregarCatDocPopUp').style.display = 'none';
    document.getElementById('panAgregarTagDocPopUp').style.display = 'none';

    if (document.getElementById('panInfoEnvioTwitterPopUp') != null) {
        document.getElementById('panInfoEnvioTwitterPopUp').style.display = 'none';
    }
    document.getElementById('panVincularDocADoc').style.display = 'block';

    MostrarPopUpSelectorCategorias();
}

function CerrarSelectorCategorias() {
    CerrarCapaModal();
}

function CerrarCapaModal() {
    var $capa = $('#capaModal');
    $capa.fadeOut();
}

function CerrarCompartidorRecurso() {
    CerrarSelectorCategorias();
    document.getElementById('panCompartirDocPopUp').style.display = 'none';
}

function CerrarSelectorCualquiera(capa) {
    var $capa = $('#' + capa);
    $capa.fadeOut();
}

function AjustarPanelDesplegableBusqAvanzParaAutoCompTags() {
    if (document.getElementById('panBusquedaAv').style.overflow != "visible") {
        document.getElementById('panBusquedaAv').style.overflow = "visible";
    }
    else {
        document.getElementById('panBusquedaAv').style.overflow = "hidden";
    }
}

function CalcularTopPanelYMostrar(evento, panelID) {
    if (!evento.target) {
        var hijo = evento.srcElement;
    }
    else {
        var hijo = evento.target;
    }
    if (hijo.nodeName == 'IMG') {
        hijo = $(hijo).parent();
    }
    var $padre = $(hijo).parent();

    $(document.getElementById(panelID)).css({
        top: $padre.offset().top + $padre.height() - $(document.getElementById(panelID)).height() / 2 + 'px',
        display: ''
    });
    return false;
}

/*---------     Fin by Javier     --------------*/

/*---------     RIAM: Funcion para modificar un checkBox       --------------*/
function ModificarCheck(checkID, estado) {
    if ($('#' + checkID).length > 0) {
        $('#' + checkID).attr('checked', estado);
    }
}

/*---------     FIN RIAM: Funcion para modificar un checkBox       --------------*/

/*---------    REGION RESALTAR TAGS, BY ALTU    --------------------------------------------*/


//NOTA:   NO TOCAR SIN EL COSENTIMIENTO DE JAVIER.

function ResaltarTags(pListaTags) {
    var listaColoresResaltar = new Array(8);

    listaColoresResaltar[0] = "#8F529D"; //"rgb(143, 82, 157)";
    listaColoresResaltar[1] = "#4C8FB5"; //"rgb(76, 143, 181)";
    listaColoresResaltar[2] = "#E08552"; //"rgb(224, 133, 82)";
    listaColoresResaltar[3] = "#E55982"; //"rgb(229, 89, 130)";
    listaColoresResaltar[4] = "#C4A3CB"; //"rgb(196, 163, 203)";
    listaColoresResaltar[5] = "#B5DDF1"; //"rgb(181, 221, 241)";
    listaColoresResaltar[6] = "#F8C0A9"; //"rgb(248, 192, 169)";
    listaColoresResaltar[7] = "#F6ABCF"; //"rgb(246, 171, 207)";

    listaPlanaColoresResaltar = "#8F529D #4C8FB5 #E08552 #E55982 #C4A3CB #B5DDF1 #F8C0A9 #F6ABCF";
    listaArtiConjuPrep = ",el,la,los,las,un,una,lo,unos,unas,y,o,u,e,ni,a,con,de,del,en,para,por,al,";
    listaCaracteresExpurios = [" ", ",", "\"", "\'", "(", ")", ";", "<", ">", ":", "/"];

    var elementos = $('.Resaltable');
    var elementosTags = $('.TagResaltable');
    var listaTags = pListaTags.split(",");

    for (var i = 0; i < elementos.length; i++) {

        var texto = elementos[i].innerHTML;
        elementos[i].innerHTML = ObtenerTextoConEnfasisSegunLosTags(texto, listaTags, listaColoresResaltar, false);
    }

    for (var i = 0; i < elementosTags.length; i++) {

        var textoTags = elementosTags[i].innerHTML;
        //elementosTags[i].innerHTML = ObtenerTextoConEnfasisSegunLosTagsParaCadenaDeTags(textoTags, listaTags, listaColoresResaltar);
        elementosTags[i].innerHTML = ObtenerTextoConEnfasisSegunLosTags(textoTags, listaTags, listaColoresResaltar, false);
    }
}

function ObtenerTextoConEnfasisSegunLosTags(pTexto, pListaTags, pListaColoresResaltar, pExpandirPalabra) {
    var textoConEnfasis = pTexto;
    var count = 0;

    for (var i = 0; i < pListaTags.length; i++) {
        var tag = pListaTags[i];
        var textoEnMinusSinAcentos = QuitarAcentosConvertirMinuscula(textoConEnfasis);
        var tagEnMinusSinAcentos = QuitarAcentosConvertirMinuscula(tag);

        if (textoEnMinusSinAcentos.indexOf(tagEnMinusSinAcentos) != -1) {//Contiene todo el tag por lo enfatizo entero:

            textoConEnfasis = ProcesarTagEnTextoUnaOVariosVeces(textoConEnfasis, tag, count, pListaColoresResaltar, listaCaracteresExpurios, pExpandirPalabra);
        }
        else {
            //Puede que contenga alguna palabra del tag por lo que la enfatizo individualmente:
            var trozosTag = SepararTextoPorCarater(tag, ' ');

            for (var x = 0; x < trozosTag.length; x++) {
                var trozoTag = trozosTag[x];

                if (!EsArticuloOConjuncionOPreposicionesComunes(trozoTag)) {
                    var textoRecompuesto = "";
                    var separador = "";
                    var palabras = SepararTextoPorCarater(textoConEnfasis, ' ');

                    for (var z = 0; z < palabras.length; z++) {
                        var palabra = palabras[z];
                        var palabraLimpia = QuitarAcentosConvertirMinuscula(palabra);
                        var trozoTagLimpio = QuitarAcentosConvertirMinuscula(trozoTag);

                        if (palabraLimpia == trozoTagLimpio) {
                            textoRecompuesto += separador + AgregarEnfasisATexto(palabra, count, pListaColoresResaltar);
                        }
                        else if (pExpandirPalabra && palabraLimpia.indexOf(trozoTagLimpio) != -1) {
                            if (caracterEnPListaCaracteresExpurios(listaCaracteresExpurios, palabra.charAt(0))) {
                                textoRecompuesto += separador + palabra.charAt(0) + AgregarEnfasisATexto(palabra.substring(1), count, pListaColoresResaltar);
                            }
                            else if (caracterEnPListaCaracteresExpurios(listaCaracteresExpurios, palabra[palabra.length - 1])) {
                                textoRecompuesto += separador + AgregarEnfasisATexto(palabra.substring(0, palabra.length - 1), count, pListaColoresResaltar) + palabra.charAt(palabra.length - 1);
                            }
                            else {
                                textoRecompuesto += separador + AgregarEnfasisATexto(palabra, count, pListaColoresResaltar);
                            }
                        }
                        else {
                            textoRecompuesto += separador + palabra;
                        }
                        separador = " ";
                    }
                    textoConEnfasis = textoRecompuesto;
                }
            }
        }
        count++;

        if (count >= pListaColoresResaltar.length) {
            count = 0;
        }
    }
    return textoConEnfasis;
}

function ObtenerTextoConEnfasisSegunLosTagsParaCadenaDeTags(pTexto, pListaTags, pListaColoresResaltar) {
    var textoConEnfasis = pTexto;
    var count = 0;

    for (var i = 0; i < pListaTags.length; i++) {
        var tag = pListaTags[i];
        var tagContieneTrozoTag = false;
        var textoEnMinusSinAcentos = QuitarAcentosConvertirMinuscula(textoConEnfasis);
        var tagEnMinusSinAcentos = QuitarAcentosConvertirMinuscula(tag);

        if (textoEnMinusSinAcentos.indexOf(tagEnMinusSinAcentos) != -1) {//Contiene todo el tag por lo enfatizo el tag entero entero:
            tagContieneTrozoTag = true;
        }
        else {//Puede que contenga alguna palabra del tag por lo que la enfatizo individualmente:
            var trozosTag = SepararTextoPorCarater(tag, ' ');

            for (var x = 0; x < trozosTag.length; x++) {
                var trozoTag = trozosTag[x];

                if (!EsArticuloOConjuncionOPreposicionesComunes(trozoTag)) {
                    var palabras = SepararTextoPorCarater(textoConEnfasis, ' ');

                    for (var z = 0; z < palabras.length; z++) {
                        var palabra = palabras[z];

                        if (QuitarAcentosConvertirMinuscula(palabra) == QuitarAcentosConvertirMinuscula(trozoTag)) {
                            tagContieneTrozoTag = true;
                            break;
                        }
                    }
                }
            }
        }

        if (tagContieneTrozoTag && textoConEnfasis.indexOf("<span") == -1) {
            textoConEnfasis = AgregarEnfasisATexto(EliminarEspaciosExteriores(textoConEnfasis), count, pListaColoresResaltar);
        }
        else {
            textoConEnfasis = EliminarEspaciosExteriores(textoConEnfasis);
        }
        count++;

        if (count >= pListaColoresResaltar.Length) {
            count = 0;
        }
    }
    return textoConEnfasis;
}

function ProcesarTagEnTextoUnaOVariosVeces(pTexto, pTag, pEstiloResalto, pListaColoresResaltar, pListaCaracteresExpurios, pExpandirPalabra) {
    var textoConEnfasis = pTexto;

    var textoEnMinusSinAcentos = QuitarAcentosConvertirMinuscula(textoConEnfasis);
    var tagEnMinusSinAcentos = QuitarAcentosConvertirMinuscula(pTag);

    if (textoEnMinusSinAcentos.indexOf(tagEnMinusSinAcentos) != -1) {//Contiene todo el tag por lo enfatizo entero:

        if (pTexto.length > pTag.length) {
            var inicioTagEnTexto = textoEnMinusSinAcentos.indexOf(tagEnMinusSinAcentos);
            var finTagEnTexto = inicioTagEnTexto + pTag.length;

            if (pExpandirPalabra) {
                //Expandimos el tag hasta tener palabras completas, o el final del texto:
                while (inicioTagEnTexto != 0 && !caracterEnPListaCaracteresExpurios(pListaCaracteresExpurios, textoConEnfasis.charAt(inicioTagEnTexto - 1))) {
                    inicioTagEnTexto--;
                }
                while (finTagEnTexto != textoConEnfasis.length && !caracterEnPListaCaracteresExpurios(pListaCaracteresExpurios, textoConEnfasis.charAt(finTagEnTexto))) {
                    finTagEnTexto++;
                }

                textoConEnfasis = textoConEnfasis.substring(0, inicioTagEnTexto) + AgregarEnfasisATexto(textoConEnfasis.substring(inicioTagEnTexto, finTagEnTexto), pEstiloResalto, pListaColoresResaltar) + ProcesarTagEnTextoUnaOVariosVeces(textoConEnfasis.substring(finTagEnTexto), pTag, pEstiloResalto, pListaColoresResaltar, pListaCaracteresExpurios, pExpandirPalabra); //+ textoConEnfasis.substring(finTagEnTexto);
            }
            else {
                if ((inicioTagEnTexto == 0 || caracterEnPListaCaracteresExpurios(pListaCaracteresExpurios, textoConEnfasis.charAt(inicioTagEnTexto - 1))) && (finTagEnTexto == textoEnMinusSinAcentos.length || caracterEnPListaCaracteresExpurios(pListaCaracteresExpurios, textoConEnfasis.charAt(finTagEnTexto)))) { //Solo si la cadena contiene el propio pTag como palabra
                    textoConEnfasis = textoConEnfasis.substring(0, inicioTagEnTexto) + AgregarEnfasisATexto(textoConEnfasis.substring(inicioTagEnTexto, finTagEnTexto), pEstiloResalto, pListaColoresResaltar) + ProcesarTagEnTextoUnaOVariosVeces(textoConEnfasis.substring(finTagEnTexto), pTag, pEstiloResalto, pListaColoresResaltar, pListaCaracteresExpurios, pExpandirPalabra);
                }
                else {
                    textoConEnfasis = textoConEnfasis.substring(0, finTagEnTexto) + ProcesarTagEnTextoUnaOVariosVeces(textoConEnfasis.substring(finTagEnTexto), pTag, pEstiloResalto, pListaColoresResaltar, pListaCaracteresExpurios, pExpandirPalabra);
                }
            }
        }
        else {
            textoConEnfasis = AgregarEnfasisATexto(textoConEnfasis, pEstiloResalto, pListaColoresResaltar);
        }
    }
    return textoConEnfasis;
}

function caracterEnPListaCaracteresExpurios(pListaCaracteresExpurios, caracter) {
    for (i = 0; i < pListaCaracteresExpurios.length; i++) {
        if (pListaCaracteresExpurios[i] == caracter) {
            return true;
        }
    }
    return false;
}

function AgregarEnfasisATexto(pTexto, pEstiloResalto, pListaColoresResaltar) {
    if (pTexto.indexOf("style=") == -1 && pTexto.indexOf("<span class") == -1 && pTexto.indexOf("</span") == -1 && pTexto != "span" && pTexto != "class=" && pTexto != "tag" && pTexto != "background-color" && pTexto != "/span" && pTexto != "color" && pTexto != "#FFFFFF" && listaPlanaColoresResaltar.indexOf(pTexto) == -1) {
        return "<span class=\"tag\" style=\"color:#FFFFFF;background-color:" + pListaColoresResaltar[pEstiloResalto] + ";\">" + pTexto + "</span>";
    }
    else {
        return pTexto;
    }
}

function EliminarEspaciosExteriores(pTexto) {
    var textoSinEspacios = pTexto;
    var hayEspacios = true;

    while (textoSinEspacios.length > 0 && hayEspacios) {
        if (textoSinEspacios.charAt(0) == ' ') {
            textoSinEspacios = textoSinEspacios.substring(1);
        }
        else if (textoSinEspacios[textoSinEspacios.length - 1] == ' ') {
            textoSinEspacios = textoSinEspacios.substring(0, textoSinEspacios.length - 1);
        }
        else {
            hayEspacios = false;
        }
    }
    return textoSinEspacios;
}

function SepararTextoPorCarater(pTexto, pCaracter) {
    var palabras = pTexto.split(pCaracter);

    //Quito elementos vacíos:
    var textoAuxiliar = "";
    var separador = "";

    for (var i = 0; i < palabras.length; i++) {
        if (palabras[i] != '') {
            textoAuxiliar += separador + palabras[i];
            separador = ",";
        }
    }
    return textoAuxiliar.split(",");
}

function EsArticuloOConjuncionOPreposicionesComunes(pTexto) {
    if (pTexto.length > 4) {
        //No hay ninguna preposición, conjunción o artículo con las de 4 caracteres.
        return false;
    }
    else {
        return (listaArtiConjuPrep.indexOf(',' + pTexto + ',') != -1);
    }
}

function QuitarAcentosConvertirMinuscula(pTexto) {
    var textoLimpio = pTexto;

    textoLimpio = textoLimpio.replace(/\á/g, 'a');
    textoLimpio = textoLimpio.replace(/\Á/g, 'a');

    textoLimpio = textoLimpio.replace(/\é/g, 'e');
    textoLimpio = textoLimpio.replace(/\É/g, 'e');

    textoLimpio = textoLimpio.replace(/\í/g, 'i');
    textoLimpio = textoLimpio.replace(/\Í/g, 'i');

    textoLimpio = textoLimpio.replace(/\ó/g, 'o');
    textoLimpio = textoLimpio.replace(/\Ó/g, 'o');

    textoLimpio = textoLimpio.replace(/\ú/g, 'u');
    textoLimpio = textoLimpio.replace(/\Ú/g, 'u')

    textoLimpio = textoLimpio.toLowerCase();

    return textoLimpio;
}

/*--------    FIN REGION RESALTAR TAGS    ---------------------------------------------------*/

/*--------    REGION BUSQUEDA POR VARIAS CATEGORÍAS -----------------------------------------*/

function AceptarSelectorCatBusqueda(pTxtHackIds, pTxtControlSelector) {
    AceptarSelectorCatBusqueda(pTxtHackIds, pTxtControlSelector, true, false);
}

function AceptarSelectorCatBusqueda(pTxtHackIds, pTxtControlSelector, pFiltrarBusqueda, pCategoriasDocumentacion) {
    document.getElementById(pTxtHackIds).value = document.getElementById(pTxtControlSelector).value;

    var categoriaIDs = document.getElementById(pTxtHackIds).value;
    var divs = document.getElementById('divSelCatTesauro');
    document.getElementById('contenedorFiltrosCategorias').innerHTML = '';

    var nivel = 1;

    if (pCategoriasDocumentacion) {
        document.getElementById('contenedorFiltrosCatDocumentacion').innerHTML = '';
        nivel = 0;
    }

    var numSeleccionados = AgregarCategoriasASeleccion(divs, categoriaIDs, pTxtHackIds, pTxtControlSelector, nivel, pFiltrarBusqueda, pCategoriasDocumentacion);

    var divsLista = document.getElementById('divSelCatLista');
    AjustarCategoriasSeleccion(divsLista.children[1], categoriaIDs);

    if (document.getElementById('filtrosCategorias') != null) {
        if (numSeleccionados > 0) {
            document.getElementById('filtrosCategorias').style.display = '';
            //EjecutarScriptsIniciales();
        }
        else {
            document.getElementById('filtrosCategorias').style.display = 'none';
        }
    }

    if (pFiltrarBusqueda) {
        FiltrarBusqueda();
    }
}

function AgregarCategoriasASeleccion(pDivs, pCategoriaIDs, pTxtHackIds, pTxtControlSelector, pNivel, pFiltrarBusqueda, pCategoriasDocumentacion) {
    var numSeleccionados = 0;

    for (var i = 0; i < pDivs.children.length; i++) {
        if (pCategoriaIDs.indexOf(pDivs.children[i].children[1].className) != -1) {
            var contenedor = null;

            if (pNivel != 0) {
                contenedor = 'contenedorFiltrosCategorias';
            }
            else {
                contenedor = 'contenedorFiltrosCatDocumentacion';
            }

            document.getElementById(contenedor).innerHTML += '<a id="idTemp" onclick="EliminarCategoriaFiltroBusqueda(\'' + pDivs.children[i].children[1].className + '\',\'' + pTxtHackIds + '\', \'' + pTxtControlSelector + '\', ' + pNivel + ', ' + pFiltrarBusqueda + ', ' + pCategoriasDocumentacion + ');">' + pDivs.children[i].children[1].children[1].innerHTML + '</a> ';
            numSeleccionados++;
            $(pDivs.children[i].children[1].children[0]).attr('checked', true);
        }
        else if ($(pDivs.children[i].children[1].children[0]).is(':checked')) {
            $(pDivs.children[i].children[1].children[0]).attr('checked', false);
        }

        if (pDivs.children[i].children.length == 3) {
            numSeleccionados = numSeleccionados + AgregarCategoriasASeleccion(pDivs.children[i].children[2], pCategoriaIDs, pTxtHackIds, pTxtControlSelector, pNivel + 1, pFiltrarBusqueda, pCategoriasDocumentacion);
        }
    }

    return numSeleccionados;
}

function AjustarCategoriasSeleccion(pDivs, pCategoriaIDs) {
    for (var i = 0; i < pDivs.children.length; i++) {
        if (pCategoriaIDs.indexOf(pDivs.children[i].children[0].className) == -1) {
            $(pDivs.children[i].children[0].children[0]).attr('checked', false)
        }
        else {
            $(pDivs.children[i].children[0].children[0]).attr('checked', true)
        }

        if (pDivs.children[i].children.length == 2) {
            AjustarCategoriasSeleccion(pDivs.children[i].children[1], pCategoriaIDs);
        }
    }
}

function EliminarCategoriaFiltroBusqueda(pCategoriaID, pTxtHackIds, pTxtControlSelector, pNivel, pFiltrarBusqueda, pCategoriasDocumentacion) {
    document.getElementById(pTxtControlSelector).value = document.getElementById(pTxtControlSelector).value.replace(pCategoriaID + ',', '');

    if (pCategoriasDocumentacion && pNivel == 0) {
        document.getElementById(pTxtControlSelector).value = '';
        HabilitarTodosLosElementosArbol();
    }

    AceptarSelectorCatBusqueda(pTxtHackIds, pTxtControlSelector, pFiltrarBusqueda, pCategoriasDocumentacion);
}

function MostrarOcultarPanelCatBusqueda(pPanelCatID) {
    Desplegar(this, pPanelCatID);

    if (document.getElementById(pPanelCatID).style.display != 'none') {
        document.getElementById('aspnetForm').setAttribute('onclick', 'OcultarPanelCategoriasBusqueda(\'' + pPanelCatID + '\');');
    }
}

function OcultarPanelCategoriasBusqueda(pPanelCatID) {
    if (document.getElementById('txtHackNoCerrarSelector').value == '' && document.getElementById(pPanelCatID).style.display != 'none') {
        Desplegar(this, pPanelCatID);
    }

    document.getElementById('txtHackNoCerrarSelector').value = '';
}

function LimpiarCatSelecciondas(pTxtHackIds, pTxtControlSelector, pPanelCatID, pFiltrarBusqueda, pCategoriasDocumentacion) {
    if (pCategoriasDocumentacion) {
        HabilitarTodosLosElementosArbol();
    }

    document.getElementById(pTxtControlSelector).value = '';
    AceptarSelectorCatBusqueda(pTxtHackIds, pTxtControlSelector, pFiltrarBusqueda, pCategoriasDocumentacion);
    Desplegar(this, pPanelCatID);
}

function AjustarTopControl(pControlID) {
    //document.getElementById(pControlID).style.top = (((document.body.offsetHeight-17)/4) + document.documentElement.scrollTop)+'px';



    if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
        document.getElementById(pControlID).style.top = (200 + document.body.scrollTop) + 'px';
    } else//si no es chrome
    {
        document.getElementById(pControlID).style.top = (200 + document.documentElement.scrollTop) + 'px';
    }
}

function MostrarControles(pControles) {
    while (pControles.length > 1) {
        var controlID = '';
        if (pControles.indexOf(',') != -1) {
            controlID = pControles.substring(0, pControles.indexOf(','));
            pControles = pControles.replace(controlID + ',', '');
        }
        else {
            controlID = pControles;
            pControles = pControles.replace(controlID, '');
        }

        if (document.getElementById(controlID) != null) {
            $('#' + controlID).fadeIn();
        }
    }
}

function CalcularTagsSegunTitulo(pTituloID, pHackTituloID, pTagsID, pHackPalabrasNoRelevantesID, pHackSeparadores) {
    var tags = ObtenerTagsFrase(document.getElementById(pTituloID).value, pHackPalabrasNoRelevantesID, pHackSeparadores);
    var txtTags = ''; //toLowerCase()

    //Quito los tags antiguos:
    var txtHackTagsTitulo = document.getElementById(pHackTituloID).value
    var tagsActuales = document.getElementById(pTagsID).value.split(',');
    var separador = '';
    for (var j = 0; j < tagsActuales.length; j++) {
        var tag = tagsActuales[j].trim();
        if (tag != '' && txtHackTagsTitulo.indexOf('[&]' + tag + '[&]') == -1) {
            txtTags += separador + tag;
            separador = ', '
        }
    }

    //Agrego los tags nuevos:
    txtHackTagsTitulo = '';
    for (var i = 0; i < tags.length; i++) {
        if (tags[i] != '') {
            txtTags += separador + tags[i];
            separador = ', '
            txtHackTagsTitulo += '[&]' + tags[i] + '[&]';
        }
    }

    document.getElementById(pTagsID).value = txtTags;
    document.getElementById(pHackTituloID).value = txtHackTagsTitulo;
}

function ObtenerTagsFrase(pFrase, pHackPalabrasNoRelevantesID, pHackSeparadores) {
    var listaTags = new Array();
    var ListaPalabrasNoRelevantes = document.getElementById(pHackPalabrasNoRelevantesID).value;

    var numeroPalabrasDescartadas = 0;

    var subcadenas = ObtenerPalabras(pFrase, pHackSeparadores);

    var listaTagsTexto = '';
    for (var i = 0; i < subcadenas.length; i++) {
        var palabra = LimpiarPalabraParaTagGeneradoSegunTitulo(subcadenas[i].toLowerCase());

        if (palabra.indexOf('"') != -1 || palabra.indexOf('\'') != -1) {
            if (palabra.indexOf('"') == 0 || palabra.lastIndexOf('"') == palabra.length - 1) {
                palabra = palabra.replace(/"/g, '');
            }
            else if (palabra.indexOf('\'') == 0 || palabra.lastIndexOf('\'') == palabra.length - 1) {
                palabra = palabra.replace(/\'/g, '');
            }
        }

        if (ListaPalabrasNoRelevantes.indexOf('[&]' + palabra + '[&]') == -1 && listaTagsTexto.indexOf('[&]' + palabra + '[&]') == -1) {
            listaTags[listaTags.length] = palabra;
            listaTagsTexto += '[&]' + palabra + '[&]';
        }
        else {
            numeroPalabrasDescartadas++;
        }
    }

    if (listaTags.length != 0) {
        if (listaTags[listaTags.length - 1].indexOf(".") == (listaTags[listaTags.length - 1].length - 1)) {
            var ultima = listaTags[listaTags.length - 1];
            listaTags[listaTags.length - 1] = ultima.replace('.', '');
        }
    }

    return listaTags;
}

function LimpiarPalabraParaTagGeneradoSegunTitulo(pPalabra) {
    if (pPalabra.length == 0) {
        return pPalabra;
    }
    else if (pPalabra.indexOf('¿') == 0 || pPalabra.indexOf('?') == 0 || pPalabra.indexOf('¡') == 0 || pPalabra.indexOf('!') == 0) {
        return LimpiarPalabraParaTagGeneradoSegunTitulo(pPalabra.substring(1));
    }
    else if (pPalabra.lastIndexOf('¿') == (pPalabra.length - 1) || pPalabra.lastIndexOf('?') == (pPalabra.length - 1) || pPalabra.lastIndexOf('¡') == (pPalabra.length - 1) || pPalabra.lastIndexOf('!') == (pPalabra.length - 1)) {
        return LimpiarPalabraParaTagGeneradoSegunTitulo(pPalabra.substring(0, pPalabra.length - 1));
    }
    else {
        return pPalabra;
    }
}

function ObtenerPalabras(pFrase, pHackSeparadores) {
    var separadores = document.getElementById(pHackSeparadores).value + ' [&]';
    var listaSeparadores = separadores.split('[&]');
    var frase = [pFrase];

    for (var i = 0; i < listaSeparadores.length; i++) {
        frase = ObtenerPalabrasSeparadas(frase, listaSeparadores[i]);
    }

    return frase;
}

function ObtenerPalabrasSeparadas(pPalabras, pSeparador) {
    if (pSeparador != '') {
        var palabras = new Array();

        for (var i = 0; i < pPalabras.length; i++) {
            var palabrasInt = pPalabras[i].split(pSeparador);

            for (var j = 0; j < palabrasInt.length; j++) {
                palabras[palabras.length] = palabrasInt[j];
            }
        }

        return palabras;
    }
    else {
        return pPalabras;
    }
}

/*--------    FIN REGION BUSQUEDA POR VARIAS CATEGORÍAS    ----------------------------------*/


/*--------    REGION DAFOS ------------------------------------------------------------------*/

function CalcularNumFactoresSinVotar(elementID, txtHackID, voto) {
    var element = document.getElementById(elementID);
    var txtHack = document.getElementById(txtHackID).value;
    var numFactoresSinVotar = element.innerHTML.substring(element.innerHTML.indexOf('(', 0) + 1, element.innerHTML.length - 1);
    if (txtHack == 0 && voto > 0) {
        numFactoresSinVotar = parseFloat(numFactoresSinVotar) - 1;
    }
    else if (txtHack > 0 && voto == 0) {
        numFactoresSinVotar = parseFloat(numFactoresSinVotar) + 1;
    }
    element.innerHTML = element.innerHTML.substring(0, element.innerHTML.indexOf('(', 0) - 1) + ' (' + numFactoresSinVotar + ')';
    document.getElementById(txtHackID).value = voto;
}

/*--------    FIN REGION DAFOS    -----------------------------------------------------------*/


function cambiarFormatoFecha(fecha) {
    //Cambia una fecha en formato 01/02/2011 a 20110201
    var cachos;
    cachos = fecha.split('/');
    return cachos[2] + cachos[1] + cachos[0];
}



/*--------    REGION BLOGS ------------------------------------------------------------------*/

function AgregarCategoriaBlog(txtNombre, txtHack, panel, baseURL) {
    var categoria = txtNombre.val();
    if (txtHack.val().indexOf("##&##" + categoria + "##&##") == -1) {
        txtHack.val(txtHack.val() + categoria + "##&##");
    }
    PintarCategoriasBlog(txtHack, panel, baseURL);
    txtNombre.val('');
}

function PintarCategoriasBlog(txtHack, panel, baseURL) {
    panel.html('');
    var listaCat = txtHack.val().split("##&##");

    for (var i = 0; i < listaCat.length; i++) {
        var categoria = listaCat[i];
        if (categoria != "") {
            var html = "<label>" + categoria + "<a onclick=\"javascript:EliminarCategoriaBlog(this.parentNode.textContent, $('" + txtHack.selector + "'), $('" + panel.selector + "'), '" + baseURL + "');\"><img src='" + baseURL + "blank.gif' alt='" + borr.eliminar + "'/></a></label>"
            panel.html(panel.html() + html);
        }
    }
}

function EliminarCategoriaBlog(categoria, txtHack, panel, baseURL) {
    if (txtHack.val().indexOf("##&##" + categoria + "##&##") >= 0) {
        txtHack.val(txtHack.val().replace(categoria + "##&##", ""));
    }

    PintarCategoriasBlog(txtHack, panel, baseURL);
}

/*--------    FIN REGION BLOGS    -----------------------------------------------------------*/

function quitarFormatoHTML(cadena) {
    //return Encoder.htmlDecode(cadena.replace(/<[^>]+>/g,'').replace(/\n/g, '').replace(/^\s*|\s*$/g,""));
    return Encoder.htmlDecode(cadena.replace(/^\s*|\s*$/g, ""));
}





/*--------    REGION BUSCADOR FACETADO    -----------------------------------------------------------*/

function ObtenerHash() {
    var hash = window.location.hash;
    if (hash != null && hash != '') {
        var posicion = hash.indexOf(hash)
        if (posicion > -1) {
            return hash;
        }
    }
    return '';
}

function ObtenerFiltroRango(control1, textoInicioCtrl1, control2, textoInicioCtrl2, esFiltroFechas) {
    var resultado = '';
    if (control1 != null && $(control1).val() != "" && $(control1).val() != textoInicioCtrl1 && control1.type != 'hidden') {
        var valor = $(control1).val();
        if (esFiltroFechas) {
            valor = cambiarFormatoFecha(valor);
        }
        resultado += valor;
    }
    resultado += '-';
    if (control2 != null && $(control2).val() != "" && $(control2).val() != textoInicioCtrl2 && control2.type != 'hidden') {
        var valor2 = $(control2).val();
        if (esFiltroFechas) {
            valor2 = cambiarFormatoFecha(valor2);
        }
        resultado += valor2;
    }

    return resultado;
}


function autocompletarGenerico(control, pClaveFaceta, pOrden, pParametros) {
    $(control).autocomplete(
null,
{
    //servicio: new WS($('input.inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
    //metodo: 'AutoCompletarFacetas',
    url: $('input.inpt_urlServicioAutocompletar').val() + "/AutoCompletarFacetas",
    type: "POST",
    delay: 0,
    minLength: 4,
    scroll: false,
    selectFirst: false,
    minChars: 4,
    width: 170,
    cacheLength: 0,
    extraParams: {
        proyecto: $('input.inpt_proyID').val(),
        bool_esMyGnoss: $('input.inpt_bool_esMyGnoss').val() == 'True',
        bool_estaEnProyecto: $('input.inpt_bool_estaEnProyecto').val() == 'True',
        bool_esUsuarioInvitado: $('input.inpt_bool_esUsuarioInvitado').val() == 'True',
        identidad: $('input.inpt_identidadID').val(),
        organizacion: $('input.inpt_organizacionID').val(),
        filtrosContexto: '',
        languageCode: $('input.inpt_Idioma').val(),
        perfil: perfilID,
        nombreFaceta: pClaveFaceta,
        orden: pOrden,
        parametros: replaceAll(replaceAll(replaceAll(pParametros.replace('#', ''), '%', '%25'), '#', '%23'), '+', "%2B"),
        tipo: $('input.inpt_tipoBusquedaAutoCompl').val(),
        botonBuscar: control.id + 'botonBuscar'
    }
}
);
    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }

    pintarTagsInicio();
}


function autocompletarUsuario(control, pClaveFaceta, pOrden, pParametros, pGrafo) {
    $(control).autocomplete(
    null,
    {
        //servicio: new WS($('input.inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
        //metodo: 'AutoCompletarFacetas',
        url: $('input.inpt_urlServicioAutocompletar').val() + "/AutoCompletarFacetas",
        type: "POST",
        delay: 0,
        minLength: 4,
        scroll: false,
        selectFirst: false,
        minChars: 4,
        width: 170,
        cacheLength: 0,
        extraParams: {
            proyecto: pGrafo,
            bool_esMyGnoss: $('input.inpt_bool_esMyGnoss').val() == 'True',
            bool_estaEnProyecto: $('input.inpt_bool_estaEnProyecto').val() == 'True',
            bool_esUsuarioInvitado: $('input.inpt_bool_esUsuarioInvitado').val() == 'True',
            identidad: $('input.inpt_identidadID').val(),
            organizacion: $('input.inpt_organizacionID').val(),
            perfil: perfilID,
            filtrosContexto: '',
            languageCode: $('input.inpt_Idioma').val(),
            nombreFaceta: pClaveFaceta,
            orden: pOrden,
            parametros: pParametros,
            tipo: $('input.inpt_tipoBusquedaAutoCompl').val(),
            botonBuscar: control.id + 'botonBuscar'
        }
    }
    );
    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }
}

function autocompletarGenericoConFiltroContexto(control, pClaveFaceta, pOrden, pParametros, pFiltroContexto) {
    var proyectoBusqueda = $('input.inpt_proyID').val();

    if (parametros_adiccionales.indexOf('proyectoOrigenID=') == 0) {
        proyectoBusqueda = parametros_adiccionales.substring(parametros_adiccionales.indexOf('proyectoOrigenID=') + 'proyectoOrigenID='.length);
        proyectoBusqueda = proyectoBusqueda.substring(0, proyectoBusqueda.indexOf('|'));
    }

    $(control).autocomplete(
        null,
        {
            //servicio: new WS($('input.inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
            //metodo: 'AutoCompletarFacetas',
            url: $('input.inpt_urlServicioAutocompletar').val() + "/AutoCompletarFacetas",
            type: "POST",
            delay: 0,
            minLength: 4,
            scroll: false,
            selectFirst: false,
            minChars: 4,
            width: 170,
            cacheLength: 0,
            extraParams: {
                proyecto: proyectoBusqueda,
                bool_esMyGnoss: $('input.inpt_bool_esMyGnoss').val() == 'True',
                bool_estaEnProyecto: $('input.inpt_bool_estaEnProyecto').val() == 'True',
                bool_esUsuarioInvitado: $('input.inpt_bool_esUsuarioInvitado').val() == 'True',
                identidad: $('input.inpt_identidadID').val(),
                organizacion: $('input.inpt_organizacionID').val(),
                filtrosContexto: pFiltroContexto,
                languageCode: $('input.inpt_Idioma').val(),
                perfil: perfilID,
                nombreFaceta: pClaveFaceta,
                orden: pOrden,
                parametros: encodeURIComponent(pParametros),
                tipo: $('input.inpt_tipoBusquedaAutoCompl').val(),
                botonBuscar: control.id + 'botonBuscar'
            }
        }
    );
    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }

    pintarTagsInicio();
}

function autocompletarEtiquetas(control, pClaveFaceta, pOrden, pParametros) {
    $(control).autocomplete(
        null,
        {
            servicio: new WS($('input.inpt_urlServicioAutocompletarEtiquetas').val(), WSDataType.jsonp),
            metodo: 'AutoCompletar',
            //url: $('input.inpt_urlServicioAutocompletarEtiquetas').val() + "/AutoCompletar",
            //type: "POST",
            delay: 0,
            scroll: false,
            selectFirst: false,
            minChars: 1,
            width: 170,
            cacheLength: 0,
            extraParams: {
                pProyecto: $('input.inpt_proyID').val(),
                pTablaPropia: true,
                pFaceta: pClaveFaceta,
                pOrigen: '',
                pIdentidad: $('input.inpt_identidadID').val(),
                botonBuscar: control.id + 'botonBuscar'
            }
        }
    );
    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }

    pintarTagsInicio();
}

function autocompletarEtiquetasTipado(control, pClaveFaceta, pIncluirIdioma) {
    //var tablaPropiaAutoCompletar = $('input.inpt_tablaPropiaAutoCompletar').val().toLowerCase() == "true";

    var extraFaceta = '';

    if (pIncluirIdioma) {
        extraFaceta = '[MultiIdioma]';
    }

    $(control).autocomplete(
null,
{
    servicio: new WS($('input.inpt_urlServicioAutocompletarEtiquetas').val(), WSDataType.jsonp),
    metodo: 'AutoCompletarTipado',
    //url: $('input.inpt_urlServicioAutocompletarEtiquetas').val() + "/AutoCompletarTipado",
    //type: "POST",
    delay: 0,
    scroll: false,
    selectFirst: false,
    minChars: 1,
    width: 170,
    cacheLength: 0,
    extraParams: {
        pProyecto: $('input.inpt_proyID').val(),
        //pTablaPropia: tablaPropiaAutoCompletar,
        pFacetas: pClaveFaceta + extraFaceta,
        pOrigen: origenAutoCompletar,
        pIdentidad: $('input.inpt_identidadID').val(),
        pIdioma: $('input.inpt_Idioma').val(),
        botonBuscar: control.id + 'botonBuscar'
    }
}
);
    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }

    pintarTagsInicio();
}

function autocompletarSeleccionEntidad(control, grafo, entContenedora, propiedad, tipoEntidadSolicitada, propSolicitadas, extraWhere, idioma) {
    var limite = 10;

    if (extraWhere.indexOf('%7c%7c%7cLimite%3d') != -1) {
        limite = 200;
    }

    $(control).autocomplete(
null,
{
    url: $('input.inpt_urlServicioAutocompletar').val() + "/AutoCompletarSeleccEntDocSem",
    type: "POST",
    delay: 0,
    scroll: false,
    selectFirst: false,
    minChars: 1,
    /*width: 170,*/
    cacheLength: 0,
    matchCase: true,
    pintarConcatenadores: true,
    max: limite,
    extraParams: {
        pGrafo: grafo,
        pEntContenedora: entContenedora,
        pPropiedad: propiedad,
        pTipoEntidadSolicitada: tipoEntidadSolicitada,
        pPropSolicitadas: propSolicitadas,
        pControlID: urlEncode(control.id),
        pExtraWhere: extraWhere,
        pIdioma: idioma
    }
}
);
    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }
}

function autocompletarSeleccionEntidadGruposGnoss(control, pIdentidad, pOrganizacion, pProyecto, pTipoSolicitud) {
    var limite = 10;

    $(control).autocomplete(null, {
        //servicio: new WS($('input.inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
        //metodo: 'AutoCompletarSeleccEntPerYGruposGnoss',
        url: $('input.inpt_urlServicioAutocompletar').val() + "/AutoCompletarSeleccEntPerYGruposGnoss",
        type: "POST",
        delay: 0,
        scroll: false,
        selectFirst: false,
        minChars: 1,
        /*width: 170,*/
        cacheLength: 0,
        matchCase: true,
        pintarConcatenadores: true,
        max: limite,
        txtValoresSeleccID: control.id.replace("hack_", "selEnt_"),
        extraParams: {
            identidad: pIdentidad,
            organizacion: pOrganizacion,
            proyecto: pProyecto,
            tipoSolicitud: pTipoSolicitud
        }
    });

    $(control).result(function (event, data, formatted) {
        AgregarValorGrupoGnossAutocompletar(control.id, data);
    });

    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }
}

function autocompletarGrafoDependiente(control, pEntidad, pPropiedad, grafo, tipoEntDep, propDepende, entPropDepende) {
    EliminarValoresGrafoDependientes(pEntidad, pPropiedad, false, true);
    //var idControl = control.id.replace("hack_", "Campo_");
    //document.getElementById(idControl).value = '';

    //control.value = '';
    var idValorPadre = '';

    if (propDepende != '' && entPropDepende != '') {
        var idControlCampo = ObtenerControlEntidadProp(entPropDepende + ',' + propDepende, TxtRegistroIDs);
        idValorPadre = document.getElementById(idControlCampo).value;

        if (idValorPadre == '') {
            return;
        }
    }

    $(control).unautocomplete();

    $(control).autocomplete(
        null,
        {
            //servicio: new WS($('input.inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
            //metodo: 'AutoCompletarGrafoDependienteDocSem',
            url: $('input.inpt_urlServicioAutocompletar').val() + "/AutoCompletarGrafoDependienteDocSem",
            type: "POST",
            delay: 0,
            scroll: false,
            selectFirst: false,
            minChars: 1,
            width: 170,
            cacheLength: 0,
            matchCase: true,
            extraParams: {
                pGrafo: grafo,
                pTipoEntDep: tipoEntDep,
                pIDValorPadre: idValorPadre,
                pControlID: urlEncode(control.id)
            }
        }
    );
    //    if (control.attributes["onfocus"] != null) {
    //        control.attributes.removeNamedItem('onfocus');
    //    }
}

function EliminarValoresGrafoDependientes(pEntidad, pPropiedad, pDeshabilitar, pRecursivo) {
    var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);
    //var valorBorrar = document.getElementById(idControlCampo).value;
    document.getElementById(idControlCampo).value = '';

    var idControlAuto = idControlCampo.replace("Campo_", "hack_");
    document.getElementById(idControlAuto).value = '';


    //    if (valorBorrar != '') {
    //        var valoresGraf = document.getElementById(TxtValoresGrafoDependientes).value;
    //        var valoresGrafDef = valoresGraf.substring(0, valoresGraf.indexOf(valorBorrar));
    //        valoresGraf = valoresGraf.substring(valoresGraf.indexOf(valorBorrar));
    //        valoresGraf = valoresGraf.substring(valoresGraf.indexOf('|') + 1);
    //        valoresGrafDef += valoresGraf;
    //        document.getElementById(TxtValoresGrafoDependientes).value = valoresGrafDef;
    //    }

    document.getElementById(idControlAuto).disabled = (pDeshabilitar && !GetEsPropiedadGrafoDependienteSinPadres(pEntidad, pPropiedad));

    if (pRecursivo) {
        var pronEntDep = GetPropiedadesDependientes(pEntidad, pPropiedad);

        if (pronEntDep != null) {
            EliminarValoresGrafoDependientes(pronEntDep[1], pronEntDep[0], true, pRecursivo);
        }
    }
}

function HabilitarCamposGrafoDependientes(pEntidad, pPropiedad) {
    var pronEntDep = GetPropiedadesDependientes(pEntidad, pPropiedad);

    if (pronEntDep != null) {
        var idControlAuto = ObtenerControlEntidadProp(pronEntDep[1] + ',' + pronEntDep[0], TxtRegistroIDs).replace("Campo_", "hack_");
        document.getElementById(idControlAuto).disabled = false;
    }
}


function CrearAutocompletarTags(control, pUrlAutocompletar, pProyectoID, pEsMyGnoss, pEstaEnProyecto, pEsUsuarioInvitado, pIdentidadID) {
    $(control).autocomplete(
null,
{
    //servicio: new WS(pUrlAutocompletar, WSDataType.jsonp),
    //metodo: 'AutoCompletarFacetas',
    url: pUrlAutocompletar + "/AutoCompletarFacetas",
    type: "POST",
    delay: 300,
    minLength: 4,
    multiple: true,
    scroll: false,
    selectFirst: false,
    minChars: 4,
    width: 300,
    cacheLength: 0,
    extraParams: {
        proyecto: pProyectoID,
        bool_esMyGnoss: pEsMyGnoss,
        bool_estaEnProyecto: pEstaEnProyecto,
        bool_esUsuarioInvitado: pEsUsuarioInvitado,
        identidad: pIdentidadID,
        nombreFaceta: 'sioc_t:Tag',
        orden: '',
        parametros: '',
        tipo: '',
        perfil: '',
        organizacion: '',
        filtrosContexto: '',
        languageCode: $('input.inpt_Idioma').val()
    }
}
);
    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }

    pintarTagsInicio();
}

/////////////ETIQUETADO AUTOMATICO//////////////////        
function EtiquetadoAutomaticoDeRecursos(titulo, descripcion, txtHack, pEsPaginaEdicion) {
    //var servicio = new WS($('input.inpt_urlEtiquetadoAutomatico').val(), WSDataType.jsonp);
    var servicio = $('input.inpt_urlEtiquetadoAutomatico').val();
    var params = {};
    params['ProyectoID'] = $('input.inpt_proyID').val();
    var numMax = 15000;
    titulo = urlEncode(titulo);
    descripcion = urlEncode(descripcion);
    if (descripcion.length < numMax) {
        var metodo = 'SeleccionarEtiquetas';
        params['titulo'] = titulo;
        params['descripcion'] = descripcion;
        /*servicio.call(metodo, params, function (data) {
            procesarTags(data, txtHack, pEsPaginaEdicion);
        });*/
        $.post(obtenerUrl($('input.inpt_urlEtiquetadoAutomatico').val()) + "/" + metodo, params, function (data) {
            procesarTags(data, txtHack, pEsPaginaEdicion);
        });
    } else {
        guid = guidGenerator().toLowerCase();
        params['identificadorPeticion'] = guid;
        EtiquetadoAutomaticoDeRecursosMultiple(titulo, descripcion, numMax, params, txtHack, pEsPaginaEdicion);
    }
}

function EtiquetadoAutomaticoDeRecursosMultiple(titulo, descripcion, numMax, params, txtHack, pEsPaginaEdicion) {
    //var servicio = new WS($('input.inpt_urlEtiquetadoAutomatico').val(), WSDataType.jsonp);
    var servicio = $('input.inpt_urlEtiquetadoAutomatico').val();
    if (descripcion.length <= numMax) {
        params['fin'] = "true";
        numMax = descripcion.length;
    } else {
        params['fin'] = "false";
    }
    var metodo = 'SeleccionarEtiquetasMultiple';
    var textoEnviar = descripcion.substring(0, numMax);

    var ultimPorcentaje = textoEnviar.lastIndexOf('%');
    if (params['fin'] == "false" && ultimPorcentaje > 0) {
        textoEnviar = textoEnviar.substring(0, ultimPorcentaje);
    }

    params['titulo'] = titulo;
    params['descripcion'] = textoEnviar;
    if (params['fin'] == "false" && ultimPorcentaje > 0) {
        descripcion = descripcion.substring(ultimPorcentaje);
    } else {
        descripcion = descripcion.substring(numMax);
    }


    $.post(obtenerUrl($('input.inpt_urlEtiquetadoAutomatico').val()) + "/" + metodo, params, function (data) {
        var siguiente = data.siguiente;

        if (siguiente == true) {
            EtiquetadoAutomaticoDeRecursosMultiple(titulo, descripcion, numMax, params, txtHack, pEsPaginaEdicion);
        }
        else {
            procesarTags(data, txtHack, pEsPaginaEdicion);
        }
    });

    /*servicio.call(metodo, params, function (data) {
        var siguiente = data.siguiente;

        if (siguiente == true) {
            EtiquetadoAutomaticoDeRecursosMultiple(titulo, descripcion, numMax, params, txtHack, pEsPaginaEdicion);
        }
        else {
            procesarTags(data, txtHack, pEsPaginaEdicion);
        }
    });*/
}

/**
 * Método que se ejecutará para procesar y mostrar los Tags sugeridos.
 * Ej: En el momento de creación de un recurso
 * @param {any} data
 * @param {any} txtHack
 * @param {any} pEsPaginaEdicion
 */
function procesarTags(data, txtHack, pEsPaginaEdicion) {
    var directos = data.directas.trim();
    var propuestos = data.propuestas.trim();
    var enlaces = data.enlaces.trim();

    if (pEsPaginaEdicion) {
        propuestos = directos + propuestos;
        directos = "";
    }

    var directosAnt = "";
    var propuestosAnt = "";

    directosAnt = txtHack.val().split('[&]')[0];
    if (txtHack.val().indexOf('[&]') != -1) {
        propuestosAnt = txtHack.val().split('[&]')[1];
    }


    // Cambiar por el nuevo Front
    //if (!$('#' + txtTagsID).parent().next().hasClass('propuestos')) {
    if (!$('#' + txtTagsID).parent().parent().parent().children().hasClass("propuestos")) {
        // Cambiar el contenedor donde se establecer�n las etiquetas propuestas para el nuevo Front
        // Original -> $('#' + txtTagsID).parent().after("<div class='propuestos' style='display:none'><p>" + form.tagsPropuestos + "</p><span class='contenedor'></span></div>");
        //$('#' + txtTagsID).parent().after('<div class="propuestos"><label class="control-label d-block mb-2">' + form.tagsPropuestos + '</label></p><span class="contenedor tag-list sugerencias"></span></div>');
        $('#' + txtTagsID).parent().parent().after('<div class="propuestos"><label class="control-label d-block mb-2">' + form.tagsPropuestos + '</label></p><span class="contenedor tag-list sugerencias"></span></div>');
    }

    $('#' + txtTagsID + "Enlaces").val(enlaces);

    // Añadir Tags directos según el contenido introducido (Título, Descripción...)
    if (directos != "" || txtHack.val() != "") {
        directos = AgregarTagsDirectosAutomaticos(directos, directosAnt, txtTagsID);
    }

    if (propuestos != "" || txtHack.val() != "") {
        propuestos = AgregarTagsPropuestosAutomaticos(propuestos, propuestosAnt, txtTagsID);
    }
    // Incluir el contenido a Hack solo si hay contenido
    txtHack.val(directos + '[&]' + propuestos);
}

function AgregarTagsDirectosAutomaticos(pListaTagsNuevos, pListaTagsViejos, pTagsID) {
    var tagsDescartados = "";

    if ($('#' + pTagsID + '_Hack').parent().next().next().hasClass('descartados')) {
        tagsDescartados = $('#' + pTagsID + '_Hack').parent().next().next().find('#txtHackDescartados').val();
    }

    var tagsManuales = "";
    var tagsAgregados = "";

    var tagsActuales = $('#' + pTagsID + '_Hack').val().split(',');

    //Recorrro los tags de la caja y guardo los manuales
    for (var j = 0; j < tagsActuales.length; j++) {
        var tag = tagsActuales[j].trim();

        var estaYaAgregada = pListaTagsViejos.indexOf(', ' + tag + ',') != -1;
        estaYaAgregada = estaYaAgregada || pListaTagsViejos.substring(0, tag.length + 1) == tag + ',';

        if (tag != '' && !estaYaAgregada) {
            tagsManuales += tag + ',';
        }
    }

    var tags = pListaTagsNuevos.trim().split(',');

    //Recorro los tags nuevos
    for (var i = 0; i < tags.length; i++) {
        var tag = tags[i].trim();

        var estaYaDescartada = tagsDescartados.indexOf(',' + tag + ',') != -1;
        estaYaDescartada = estaYaDescartada || tagsDescartados.substring(0, tag.length + 1) == tag + ',';

        var estaYaAgregada = tagsManuales.indexOf(',' + tag + ',') != -1;
        estaYaAgregada = estaYaAgregada || tagsManuales.substring(0, tag.length + 1) == tag + ',';

        if (tag != '' && !estaYaAgregada && !estaYaDescartada) {
            tagsAgregados += tag + ',';
        }
    }

    $('#' + pTagsID).val(tagsManuales + tagsAgregados);

    LimpiarTags($('#' + pTagsID));
    PintarTags($('#' + pTagsID));

    return tagsAgregados;
}

/**
 * Agregará la lista de Tags propuestos que se han obtenido una vez se ha introducido una gran descripción en el momento de la creación de un recurso.
 * @param {any} pListaTagsNuevos: Lista de Tags propuestos separados por comas que se deberán mostrar al usuario
 * @param {any} pListaTagsViejos: Lista de Tags anteriores a los propuestos.
 * @param {any} pTagsID: ID del input donde el usuario puede ir registrando nuevas Tags. En él también se encuentra el contenedor de tags que el usuario ha ido metiendo.
 */
function AgregarTagsPropuestosAutomaticos(pListaTagsNuevos, pListaTagsViejos, pTagsID) {
    var tagsManuales = "";
    var tagsAgregados = "";

    var tagsActuales = $('#' + pTagsID + '_Hack').val().split(',');

    //Recorrro los tags de la caja y guardo los manuales
    for (var i = 0; i < tagsActuales.length; i++) {
        var tag = tagsActuales[i].trim();
        if (tag != '') {
            tagsManuales += tag + ',';
        }
    }

    // Nuevo Front
    //var divPropuestos = $('#' + pTagsID).parent().next().find('.contenedor');
    var divPropuestos = $('#' + pTagsID).parent().parent().parent().find(".propuestos > .contenedor");
    

    //Recorro los tags viejos y si no estan en los nuevos, los quito
    divPropuestos.find('.tag').each(function () {
        var tag = $(this).attr('title');

        var estaYaAgregada = pListaTagsViejos.indexOf(',' + tag + ',') != -1;
        estaYaAgregada = estaYaAgregada || pListaTagsViejos.substring(0, tag.length + 1) == tag + ',';        

        if (tag != '' && estaYaAgregada) {
            $(this).remove();
        }
    });

    var tags = pListaTagsNuevos.trim().split(',');

    if (tags != '' && tags.length > 0) {
        $('#' + pTagsID).parent().next().css('display', '');
    }

    //Recorro los tags nuevos
    for (i = 0; i < tags.length; i++) {
        var tag = tags[i].trim();
        if (tag != '') {
            tagsAgregados += tag + ',';
            var estilo = "";

            var estaYaAgregada = tagsManuales.indexOf(',' + tag + ',') != -1;
            estaYaAgregada = estaYaAgregada || tagsManuales.substring(0, tag.length + 1) == tag + ',';

            if (estaYaAgregada) {
                estilo = "style=\"display:none;\"";
            }

            // Crea la tag que se añadirá como propuesta
            var htmlTag = '';
            htmlTag += `<div class="tag" ${estilo} title="${tag}">`;
            htmlTag += `<div class="tag-wrap">`;
            htmlTag += `<span class="tag-text">${tag}</span>`;
            htmlTag += `<span class="tag-remove material-icons add">add</span>`;
            htmlTag += `</div>`;
            htmlTag += `</div>`;

            // Añadir el Tag al contenedor de Tags propuestas
            //divPropuestos.append("<div " + estilo + " class=\"tag\" title=\"" + tag + "\">" + tag + "<a class=\"add\" ></a></div>");
            divPropuestos.append(htmlTag);
        }
    }

    /**
    * Asignación a cada item generado la opción de "Añadir" el Tag al recurso. Al pulsar en (+), añadir el Tag al contendor de Tags del usuario           
    */
    $(divPropuestos.find('.tag-wrap .add')).each(function () {
        $(this).bind('click', function (evento) {
            var tag = $(this).parent().parent().attr('title');
            $('#' + pTagsID).val(tag);
            $(this).parent().parent().css('display', 'none');            
            PintarTags($('#' + pTagsID));
        });
    });

    return tagsAgregados;
}

function ActualizacionDelDiccionarioEtiquetas() {
    var servicio = new WS($('input.inpt_urlEtiquetadoAutomatico').val(), WSDataType.jsonp);
    var metodo = 'ActualizarDiccionarioEtiquetas';
    var params = {};
    var textoPropuesto = $('#' + txtHackTagsTituloID).val() + $('#' + txtHackTagsDescripcionID).val();
    textoPropuesto = textoPropuesto.replace(/\[&\]\[&\]/g, ',').replace(/\[&\]/g, '');
    params['textoPropuesto'] = textoPropuesto;
    params['textoElegidoUsuario'] = $('#' + txtTagsID).val();
    params['ProyectoID'] = $('input.inpt_proyID').val();
    servicio.call(metodo, params, function (data) {
    });
}

function CargaDelDiccionarioEtiquetas() {
    var servicio = new WS($('input.inpt_urlEtiquetadoAutomatico').val(), WSDataType.jsonp);
    var metodo = 'CargarDiccionarioEtiquetas';
    var params = {};
    params['ProyectoID'] = $('input.inpt_proyID').val();
    servicio.call(metodo, params, function (data) {
    });
}

//Añado al tipo string de javascript las funciones startsWith y EndsWith 
String.prototype.endsWith = function (str)
{ return (this.match(str + "$") == str) };

String.prototype.startsWith = function (str)
{ return (this.match("^" + str) == str) };

function SepararFiltro(filtro) {
    var array = new Array(4);
    var indiceFiltro = filtro.indexOf('=') + 1;
    array[0] = filtro.replace(':', '_'); //id
    array[1] = filtro.substring(indiceFiltro, filtro.length); //title
    array[2] = array[1]; //innerHTML
    array[3] = 'javascript:AgregarFacetas(filtro, null);'; //onclick
    return array;
}

function Login(usuario, password) {
    document.getElementById('usuario').value = usuario;
    document.getElementById('password').value = password;
    document.getElementById('formLogin').submit();
}

function LoginConClausulasRegistro(usuario, password, condicionesUso) {
    document.getElementById('usuario').value = usuario;
    document.getElementById('password').value = password;
    document.getElementById('clausulasRegistro').value = condicionesUso;
    document.getElementById('formLogin').submit();
}

function CambiarUrlRedirectLoginAUrlActual() {
    var accion = $('#formLogin').attr('action');

    var indiceRedirect = accion.indexOf('&redirect');
    var indiceFinRedirect = accion.indexOf('&', indiceRedirect + 1);
    var parametroRedirect = accion.substring(indiceRedirect, indiceFinRedirect);

    accion = accion.replace(parametroRedirect, '&redirect=' + document.location.href);

    $('#formLogin').attr('action', accion);
}

function ValidarLogin(arg, context) {
    if (arg == 'entrar') {
        location.reload(true);
    }
    else if (arg == 'mostrarError') {
        $('#errorLogin').html('<p>' + form.errorLogin + '</p>');
        $('#errorLogin').css('display', '');
        return false;
    }
    else {
        window.location = arg;
    }
}

$(document).ready(function () {
    $(".loginButton").click(function () {
        $("#botonLogin").toggleClass("btLogin");
    });

    $("#txtUsuario").keydown(function (event) {
        if (event.which || event.keyCode) {
            if ((event.which == 13) || (event.keyCode == 13)) {
                return false;
            }
        }
    });

    $("#txtContraseña").keydown(function (event) {
        if (event.which || event.keyCode) {
            if ((event.which == 13) || (event.keyCode == 13)) {
                controlarEntradaMaster();
                return false;
            }
        }
    });

    $("#formularioLoginHeader input.submit").click(function () {
        controlarEntradaMaster();
    });

    $(".registroRedesSociales ul a").click(function () {
        window.open(this.href, 'auth', 'width=600,height=500');
        return false;
    });
});



function beginRequestHandle(sender, Args) {
    if (Args._postBackElement != null && Args._postBackElement.id != "ctl00_ctl00_CPH1_CPHBandejaContenido_TimerRefresco") {
        MostrarUpdateProgress();
    }
}

function endRequestHandle(sender, Args) {
    OcultarUpdateProgress();
}

var timeoutUpdateProgress;

function MostrarUpdateProgress() {
    MostrarUpdateProgressTime(15000)
}

function MostrarUpdateProgressTime(time) {
    if ($('#mascaraBlanca').length > 0) {
        $('body').addClass('mascaraBlancaActiva');

        if (time > 0) {
            timeoutUpdateProgress = setTimeout("OcultarUpdateProgress()", time);
        }
    }
}

function OcultarUpdateProgress() {
    if ($('#mascaraBlanca').length > 0) {
        $('.popup').hide();
        $('#mascaraBlanca').hide()
        clearTimeout(timeoutUpdateProgress);
    }
}

$(function () {
    if ($('#inpt_Cookies').length > 0 && $('#inpt_Cookies').val().toLowerCase() == "true") {
        var obj = $(document);
        var obj_top = obj.scrollTop();
        obj.scroll(function () {
            var obj_act_top = $(this).scrollTop();
            if (obj_act_top != obj_top) {
                AceptarCookies();
            }
            obj_top = obj_act_top;
        });
    }
});

var cookiesAceptadas = false;

function AceptarCookies() {
    if (!cookiesAceptadas) {
        cookiesAceptadas = true;
        //WebForm_DoCallback('__Page', 'aceptamosCookies', ReceiveServerData, '', null, false);
    }
}

//$(document).ready(function () {
//    $('#formLogin').attr('action', $('input.inpt_UrlLogin').val());
//});


function controlarEntradaMaster() {
    var user = $('#txtUsuario').val();
    var pass = $('#txtContraseña').val();
    if (user.length > 0 && pass.length > 0) {
        Login(user, pass);
    } else {
        $('#errorLogin').html('<p>' + form.errorLoginVacio + '</p>');
        $('#errorLogin').css('display', '');
    }
}


//////////////////////////////////////////////////////////////////////////////////////////

//if(filtro.length > 1 || document.location.href.indexOf('/tag/') > -1){parametrosFacetas = " + "'AgregarFacetas|" + arg + "'; var parametrosResultados = " + "'ObtenerResultados|" + arg + "';
//var displayNone = '';
//if(HayFiltrosActivos(filtro)){
//document.getElementById('" + this.divFiltros.ClientID + "').setAttribute('style', 'padding-top:0px !important; ' + displayNone + ' margin-top:10px; ');}
//sb.AppendLine(Page.ClientScript.GetCallbackEventReference(this.Page, "parametrosResultados", "ReceiveServerData", String.Empty) + ";}
//else{
//document.getElementById('" + this.divFiltros.ClientID + "').setAttribute('style', 'padding-top:0px !important; margin-top:10px; display:none !important;');
//sb.AppendLine(Page.ClientScript.GetCallbackEventReference(this.Page, "parametrosFacetas", "ReceiveServerData", String.Empty) + ";}
//return false;
//}

/**
 * Comportamiento de facetas pop up para que se muestren todas las categorías de tipo "Tesauro" una vez se pulse en el botón "Ver más".
 * */
const comportamientoFacetasPopUpPlegado = {
    init: function () {
        this.config();
        // Faceta ID de la que se desean obtener sus subfacetas
        this.facetaActual = '';
    },
    config: function () {
        const that = this;

        // Posibles traducciones que se utilicen en los modales de Facetas
        this.FacetTranslations = {
            en: {
                searchBy: "Search by ",                
            },
            es: {
                searchBy: "Buscar por ",                
            },
        };

        // Lógica del Modal
        // Abrir modal        
        $(document).on('show.bs.modal', '.modal-resultados-lista', function (e) {
            // Faceta seleccionada
            that.facetaActual = $(e.relatedTarget).data('facetkey');
            that.facetaActualName = $(e.relatedTarget).data('facetname');
            // Registrar el id del modal abierto (para cargar los datos en el modal correspondiente)
            that.$modalLoaded = $(`#${e.target.id}`);
            // Clonar la faceta/mostrarlo en modal
            that.clonarFaceta();
            // Configurar eventos mostrados en el modal
            that.configEvents();
        });

        // Cerrar modal
        $(document).on('hidden.bs.modal', '.modal-resultados-lista', function (e) {
            // Vaciar el modal                 
            that.$modalLoaded.find('.listadoFacetas').empty();
            // Vaciar el título o cabecera titular
            that.$modalLoaded.find(".loading-modal-facet-title").text('');
        });
    },

    configEvents: function () {
        const that = this;

        // Buscador o filtrado de facetas cuando se inicie la escritura en el Input buscador dentro del modal         
        this.$modalLoaded.find(".buscador-coleccion .buscar .texto").keyup(function () {
            that.textoActual = that.eliminarAcentos($(this).val());
            that.filtrarElementos()
        });

        // Configurar click de la faceta
        this.$modalLoaded.find(".faceta").click(function (e) {
            // Cerrar el modal - Eliminar modal-backdrop del modal
            $('.modal-backdrop').remove();
            AgregarFaceta($(this).attr("name"));
            e.preventDefault();
        });
    },

    /**
     * Buscar las facetas y clonarlas en el modal    
     */
    clonarFaceta: function () {
        const that = this;

        // Buscar el panel de tipo faceta seleccionada
        this.facetaActual = this.facetaActual.replace(':', '_');
        this.panelFacetaActual = $(`#${this.facetaActual}`);

        // Establecer "Título" en la cabecera del modal y en el header      
        that.$modalLoaded.find('.loading-modal-facet-title').text(this.panelFacetaActual.find('.faceta-title').text());
        that.$modalLoaded.find('.faceta-title').text(this.panelFacetaActual.find('.faceta-title').text());

        // Recoger el html del panelFacetaActual para pintarlo donde corresponde
        const facetItems = this.panelFacetaActual.find('.listadoFacetas').children().clone();
        // Sustituir los plegar-desplegar correspondientes
        facetItems.find('.desplegarSubFaceta .material-icons').text('expand_more');

        that.$modalLoaded.find(`.listadoFacetas`).append(facetItems);

        // Placeholder del buscador
        if (configuracion.idioma == 'en') {
            that.$modalLoaded.find(".buscador-coleccion .buscar .texto").attr("placeholder", `${this.FacetTranslations.en.searchBy}${this.facetaActualName}`);
        } else {
            that.$modalLoaded.find(".buscador-coleccion .buscar .texto").attr("placeholder", `${this.FacetTranslations.es.searchBy}${this.facetaActualName}`);
        }

        // Activar plegado de facetas
        plegarSubFacetas.init();
    },

    /**     
     * @param {any} texto: Texto introducido para buscar facetas (input)
     */
    eliminarAcentos: function (texto) {
        var ts = "";
        for (var i = 0; i < texto.length; i++) {
            var c = texto.charCodeAt(i);
            if (c >= 224 && c <= 230) {
                ts += "a";
            } else if (c >= 232 && c <= 235) {
                ts += "e";
            } else if (c >= 236 && c <= 239) {
                ts += "i";
            } else if (c >= 242 && c <= 246) {
                ts += "o";
            } else if (c >= 249 && c <= 252) {
                ts += "u";
            } else {
                ts += texto.charAt(i);
            }
        }
        return ts;
    },

    filtrarElementos: function () {
        const that = this;
        var itemsListado = this.$modalLoaded.find('.facetas-carpeta li');
        itemsListado.show();
        if (that.textoActual == '') {
            // $('.boton-desplegar').removeClass('mostrar-hijos');
        } else {
            itemsListado.each(function () {
                var boton = $(this).find('.desplegarSubFaceta');
                //boton.removeClass('mostrar-hijos');
                var nombreCat = $(this).find('.textoFaceta').text();
                //boton.trigger('click');
                $(`.js-desplegar-facetas-modal`).trigger('click');
                if (nombreCat.toLowerCase().indexOf(that.textoActual.toLowerCase()) < 0) {
                    $(this).hide();
                }
                var categoriaHijo = $(this).find('ul').children('li');
                categoriaHijo.each(function () {
                    var nombreCatHijo = $(this).find('.textoFaceta').text();
                    if (nombreCatHijo.toLowerCase().indexOf(that.textoActual.toLowerCase()) < 0) {
                        $(this).hide();
                    }
                });
            });
        }
    },
};


/**
 * Comportamiento de facetas pop up para que se carguen una vez se pulse en el botÃ³n de "Ver mÃ¡s".
 * Se harÃ¡ la llamada para la obtenciÃ³n de Facetas y se muestran en un panel modal.  
 * */
const comportamientoFacetasPopUp = {
    init: function () {
        // Objetivo Observable
        const that = this;
        const target = $("#panFacetas")[0];

        if (target != undefined) {
            // Creación del observador
            let observer = new MutationObserver(function (mutations) {
                mutations.forEach(function (mutation) {
                    var newNodes = mutation.addedNodes; // DOM NodeList
                    if (newNodes !== null) { // Si se añaden nuevos nodos a DOM
                        var $nodes = $(newNodes); // jQuery set
                        $nodes.each(function () {
                            const $node = $(this);
                            // Configurar el servicio para elementos que sean de tipo 'verMasFacetasModal'                        
                            // Inicializamos el config del comportamiento FacetasPopUp si hay botones venidos de forma asíncrona de Facetas.
                            if ($node.hasClass('verMasFacetasModal')) {
                                that.config();
                                that.IndiceFacetaActual = 0;
                            }
                        });
                    }
                });
            });

            // Configuración del observador:
            var config = {
                attributes: true,
                childList: true,
                subtree: true,
                characterData: true
            };

            // Activación del observador para panel de Facetas (cargadas asíncronamente) con su configuración
            observer.observe(target, config);
            // Carga manual inicial de "verMasFacetasModal"
            this.config();
            this.IndiceFacetaActual = 0;
        }

    },
    config: function () {
        //1Âº Nombre de la faceta
        //2Âº Titulo del buscador
        //3Âº True para ordenar por orden alfabÃ©tico, False para utilizar el orden por defecto
        var that = this;

        /* Esquema que tendrÃ¡
        this.facetasConPopUp = [
            [
                "sioc_t:Tag",
                "Busca por TAGS",
                true,
            ], //Tags            
        ];*/

        // Array de Facetas que tendrÃ¡ visualizaciÃ³n con PopUp
        this.facetasConPopUp = [];

        // Recoger todos posibles botones de "Ver mÃ¡s" para construir un array de facetasConPopUp                
        this.facetsArray = $('.verMasFacetasModal');

        // Posibles traducciones que se utilicen en los modales de Facetas
        this.FacetTranslations = {
            en: {
                searchBy: "Search by ",
                loading: "Loading ...",
            },
            es: {
                searchBy: "Buscar por ",
                loading: "Cargando ...",
            },
        };

        $(this.facetsArray).each(function () {
            // Construir el objeto Array de la faceta para luego aÃ±adirlo a facetasConPopUp
            const facetaArray = [
                $(this).data('facetkey'), // Faceta para bÃºsqueda
                $(this).data('facetname'), // TÃ­tulo de la faceta
                true,                     // Ordenar por orden alfabÃ©tico,                
            ];
            that.facetasConPopUp.push(facetaArray);
        });

        for (i = 0; i < this.facetasConPopUp.length; i++) {
            // Faceta de tipo de bÃºsqueda
            var faceta = this.facetasConPopUp[i][0];
            var facetaSinCaracteres = faceta
                .replace(/\@@@/g, "---")
                .replace(/\:/g, "--");
        }

        // LÃ³gica del Modal
        // Abrir modal        
        $(document).on('show.bs.modal', '.modal-resultados-paginado', function (e) {
            // Conocer la faceta seleccionada
            const facetaActual = $(e.relatedTarget).data('facetkey');
            // Recorrer y buscar la posiciÃ³n en la que se encuentra la faceta seleccionada
            let i = 0;
            that.facetasConPopUp.forEach(facetaConPopUp => {
                if (facetaConPopUp[0] == facetaActual) {
                    // Guardar el Ã­ndice de facetasConPopUp
                    that.IndiceFacetaActual = i;
                }
            });
            // Registrar el id del modal abierto (para cargar los datos en el modal correspondiente)
            that.$modalLoaded = $(`#${e.target.id}`);
            that.cargarFaceta();
        });

        // Cerrar modal
        $(document).on('hidden.bs.modal', '.modal-resultados-paginado', function (e) {
            // Vaciar el modal                 
            that.$modalLoaded.find('.resultados-wrap').empty();
            // Eliminar navegador
            that.$modalLoaded.find('.action-buttons-resultados').remove();
            // Volver a mostrar el "Loading"
            that.$modalLoaded.find('.modal-resultados-paginado').find('.loading-modal-facet').removeClass('d-none');
            // Establecer el tÃ­tulo o cabecera titular del modal original
            if (configuracion.idioma == 'en') {
                that.$modalLoaded.find(".loading-modal-facet-title").text(`${that.FacetTranslations.en.loading}`);
            } else {
                that.$modalLoaded.find(".loading-modal-facet-title").text(`${that.FacetTranslations.es.loading}`);
            }
        });
    },
    eliminarAcentos: function (texto) {
        var ts = "";
        for (var i = 0; i < texto.length; i++) {
            var c = texto.charCodeAt(i);
            if (c >= 224 && c <= 230) {
                ts += "a";
            } else if (c >= 232 && c <= 235) {
                ts += "e";
            } else if (c >= 236 && c <= 239) {
                ts += "i";
            } else if (c >= 242 && c <= 246) {
                ts += "o";
            } else if (c >= 249 && c <= 252) {
                ts += "u";
            } else {
                ts += texto.charAt(i);
            }
        }
        return ts;
    },

    cargarFaceta: function () {
        var that = this;
        var FacetaActual = that.facetasConPopUp[that.IndiceFacetaActual][0];
        var facetaSinCaracteres = FacetaActual.replace(/\@@@/g, "---").replace(
            /\:/g,
            "--"
        );
        this.paginaActual = 1;
        this.textoActual = "";
        this.fin = true;
        this.buscando = false;
        this.arrayTotales = null;

        // Placeholder del buscador
        if (configuracion.idioma == 'en') {
            that.$modalLoaded.find(".buscador-coleccion .buscar .texto").attr("placeholder", `${this.FacetTranslations.en.searchBy}${that.facetasConPopUp[that.IndiceFacetaActual][1]}`);
        } else {
            that.$modalLoaded.find(".buscador-coleccion .buscar .texto").attr("placeholder", `${this.FacetTranslations.es.searchBy}${that.facetasConPopUp[that.IndiceFacetaActual][1]}`);
        }

        this.textoActual = "";

        // ConfiguraciÃ³n del servicio para llamar a las facetas deseadas
        var metodo = "CargarFacetas";
        var params = {};
        params["pProyectoID"] = $("input.inpt_proyID").val();
        params["pEstaEnProyecto"] =
            $("input.inpt_bool_estaEnProyecto").val() == "True";
        params["pEsUsuarioInvitado"] =
            $("input.inpt_bool_esUsuarioInvitado").val() == "True";
        params["pIdentidadID"] = $("input.inpt_identidadID").val();
        params["pParametros"] =
            "" +
            replaceAll(
                replaceAll(
                    replaceAll(
                        ObtenerHash2().replace(/&/g, "|").replace("#", ""),
                        "%",
                        "%25"
                    ),
                    "#",
                    "%23"
                ),
                "+",
                "%2B"
            );
        params["pLanguageCode"] = $("input.inpt_Idioma").val();
        params["pPrimeraCarga"] = false;
        params["pAdministradorVeTodasPersonas"] = false;
        params["pTipoBusqueda"] = tipoBusqeda;
        params["pGrafo"] = grafo;
        params["pFiltroContexto"] = filtroContexto;
        params["pParametros_adiccionales"] =
            parametros_adiccionales + "|NumElementosFaceta=10000|";
        params["pUbicacionBusqueda"] = ubicacionBusqueda;
        params["pNumeroFacetas"] = -1;
        params["pUsarMasterParaLectura"] = bool_usarMasterParaLectura;
        params["pFaceta"] = FacetaActual;

        // Buscador o filtrado de facetas cuando se inicie la escritura en el Input buscador dentro del modal         
        that.$modalLoaded.find(".buscador-coleccion .buscar .texto").keyup(function () {
            that.textoActual = that.eliminarAcentos($(this).val());
            that.paginaActual = 1;
            that.buscarFacetas();
        });


        // PeticiÃ³n al servicio para obtenciÃ³n de Facetas                
        $.post(obtenerUrl($('input.inpt_UrlServicioFacetas').val()) + "/" + metodo, params, function (data) {
            var htmlRespuesta = $("<div>").html(data);
            that.arrayTotales = new Array($(htmlRespuesta).find(".faceta").length);
            var i = 0;
            $(htmlRespuesta)
                .find(".faceta")
                .each(function () {
                    that.arrayTotales[i] = new Array(2);
                    that.arrayTotales[i][0] = that.eliminarAcentos(
                        $(this).text().toLowerCase()
                    );
                    that.arrayTotales[i][1] = $(this);
                    i++;
                });

            //Ordena por orden alfabÃ©tico
            if (that.facetasConPopUp[that.IndiceFacetaActual][2]) {
                that.arrayTotales = that.arrayTotales.sort(function (a, b) {
                    if (a[0] > b[0]) return 1;
                    if (a[0] < b[0]) return -1;
                    return 0;
                });
            }

            that.paginaActual = 1;
            // Header navegaciÃ³n de resultados de facetas
            const actionButtonsResultados = `<div class="action-buttons-resultados">
                                                <ul class="no-list-style">
                                                    <li class="js-anterior-facetas-modal">
                                                        <span class="material-icons">navigate_before</span>
                                                        <span class="texto">Anteriores</span>
                                                    </li>
                                                    <li class="js-siguiente-facetas-modal">
                                                        <span class="texto">Siguientes</span>
                                                        <span class="material-icons">navigate_next</span>
                                                    </li>
                                                </ul>
                                            </div>`;

            // AÃ±adir al header de navegaciÃ³n de facetas (Anterior)
            that.$modalLoaded.find(".indice-lista.no-letra").prepend(actionButtonsResultados);

            // ConfiguraciÃ³n de los botones (Anterior/Siguiente)
            $(".js-anterior-facetas-modal").click(function () {

                if (!that.buscando && that.paginaActual > 1) {
                    that.buscando = true;
                    that.paginaActual--;
                    var hacerPeticion = true;
                    //$(".indice-lista .js-anterior-facetas-modal").hide();
                    $(".indice-lista ul").animate(
                        {
                            marginLeft: 30,
                            opacity: 0,
                        },
                        200,
                        function () {
                            if (hacerPeticion) {
                                that.buscarFacetas();
                                hacerPeticion = false;
                            }
                            $(".indice-lista ul").css({ marginLeft: -30 });
                            $(".indice-lista ul").animate(
                                {
                                    marginLeft: 20,
                                    opacity: 1,
                                },
                                200,
                                function () {
                                    $(".js-anterior-facetas-modal").show();
                                    // Left Animation complete.
                                }
                            );
                        }
                    );
                }
            });

            // ConfiguraciÃ³n del clicks de navegaciÃ³n (Siguiente)            
            $(".js-siguiente-facetas-modal").click(function () {

                if (!that.buscando && !that.fin) {
                    that.buscando = true;
                    that.paginaActual++;
                    var hacerPeticion = true;
                    //$(".indice-lista .js-siguiente-facetas-modal").hide();
                    $(".indice-lista ul").animate(
                        {
                            marginLeft: 30,
                            opacity: 0,
                        },
                        200,
                        function () {
                            if (hacerPeticion) {
                                that.buscarFacetas();
                                hacerPeticion = false;
                            }
                            $(".indice-lista ul").css({ marginLeft: -30 });
                            $(".indice-lista ul").animate(
                                {
                                    marginLeft: 20,
                                    opacity: 1,
                                },
                                200,
                                function () {
                                    $(".js-siguiente-facetas-modal").show();
                                    // Right Animation complete.
                                }
                            );
                        }
                    );
                }
            });
            // Buscar facetas y mostrarlas
            that.buscarFacetas();
        });
    },

    buscarFacetas: function () {
        buscando = true;
        const that = this;
        this.textoActual = this.textoActual.toLowerCase();

        // Limpio antes de mostrar datos - No harÃ­a falta si elimino todo con el cierre del modal
        that.$modalLoaded.find(".indice-lista.no-letra ul.listadoFacetas").remove();

        var facetaMin = (this.paginaActual - 1) * 22 + 1;
        var facetaMax = facetaMin + 21;

        var facetaActual = 0;
        var facetaPintadoActual = 0;
        var ul = $(`<ul class="listadoFacetas">`);

        this.fin = true;

        var arrayTextoActual = this.textoActual.split(" ");

        for (i = 0; i < this.arrayTotales.length; i++) {
            var nombre = this.arrayTotales[i][0];

            var mostrar = true;
            for (j = 0; j < arrayTextoActual.length; j++) {
                mostrar = mostrar && nombre.indexOf(arrayTextoActual[j]) >= 0;
            }

            if (facetaPintadoActual < 22 && mostrar) {
                facetaActual++;
                if (facetaActual >= facetaMin && facetaActual <= facetaMax) {
                    facetaPintadoActual++;
                    if (facetaPintadoActual == 1) {
                        ul = $(`<ul class="listadoFacetas">`);
                        that.$modalLoaded.find(".indice-lista.no-letra .resultados-wrap").append(ul);
                    } else if (facetaPintadoActual == 12) {
                        ul = $(`<ul class="listadoFacetas">`);
                        that.$modalLoaded.find(".indice-lista.no-letra .resultados-wrap").append(ul);
                    }
                    var li = $("<li>");
                    li.append(this.arrayTotales[i][1]);
                    ul.append(li);
                }
            }
            if (this.fin && facetaPintadoActual == 22 && mostrar) {
                this.fin = false;
            }
        }

        // Configurar click de la faceta
        $(".indice-lista .faceta").click(function (e) {
            // Cerrar el modal - Eliminar modal-backdrop del modal
            $('.modal-backdrop').remove();
            AgregarFaceta($(this).attr("name"));
            e.preventDefault();
        });

        this.buscando = false;
        // Establecer el tÃ­tulo o cabecera titular del modal
        that.$modalLoaded.find(".loading-modal-facet-title").text(
            that.facetasConPopUp[that.IndiceFacetaActual][1]
        );
        // Ocultar el Loading
        that.$modalLoaded.find('.loading-modal-facet').addClass('d-none');
    },
};

function VerFaceta(faceta, controlID) {
    if (document.getElementById(controlID + '_aux') == null) {
        $('#' + controlID).parent().html($('#' + controlID).parent().html() + '<div style="display:none;" id="' + controlID + '_aux' + '"></div>');
        $('#' + controlID + '_aux').html($('#' + controlID).html());

        var filtros = ObtenerHash2();

        if (typeof (filtroDePag) != 'undefined' && filtroDePag != '') {
            if (filtros != '') {
                filtros = filtroDePag + '|' + filtros;
            }
            else {
                filtros = filtroDePag;
            }
        }

        MontarFacetas(filtros, false, -1, '#' + controlID, faceta + '|vermas');
    }
    else {
        var htmlAux = $('#' + controlID + '_aux').html();
        $('#' + controlID + '_aux').html($('#' + controlID).html());
        $('#' + controlID).html(htmlAux);
        if (enlazarJavascriptFacetas) {
            enlazarFacetasBusqueda();
        }
        else {
            enlazarFacetasNoBusqueda();
        }
        CompletadaCargaFacetas();
    }
    return false;
}

function VerArbol(faceta, controlID) {
    var filtros = ObtenerHash2();
    //sb.AppendLine(Page.ClientScript.GetCallbackEventReference(this.Page, "filtros", "ReceiveServerData", String.Empty) + ";
    MontarFacetas(filtros, false, -1, '#' + controlID, faceta + '_Arbol');
    return false;
}

function CrearFiltroEliminarDeElemento(elemento) {
    var onclick = '';
    if (elemento.onclick != null) {
        onclick = elemento.onclick.toString();
    }
    CrearFiltroEliminar(elemento.name, elemento.title, onclick, elemento.innerHTML);
}

function CrearFiltroEliminar(name, title, onclick, innerHTML) {
    if (name.indexOf('search_') == 0) {
        $('#ctl00_ctl00_txtBusqueda').val('');
    }
    if (title != '') {
        innerHTML = title;
    }
    else if (innerHTML.charAt(innerHTML.length - 1) == ')') {
        var indiceParentesis = innerHTML.lastIndexOf('(') - 1;
        innerHTML = innerHTML.substring(0, indiceParentesis);
    }
    var nameNuevo = name;

    var elementosAgregado = $('li[name="' + nameNuevo + '"]');

    var agregame = document.getElementById(panFiltrosPulgarcito);
    if (agregame != null) {
        if (elementosAgregado.length == 0) {
            onclick = onclick.substr(onclick.indexOf('{') + 1);
            //            var nuevo = document.createElement('a');
            //            //nuevo.innerHTML = innerHTML;
            //            nuevo.setAttribute('onclick', onclick.substr(0, onclick.length - 1));
            //            nuevo.title = title;
            //            nuevo.id = nameNuevo;
            //            nuevo.name = nameNuevo;

            var li = document.createElement('li');
            li.setAttribute('name', nameNuevo);
            li.innerHTML = innerHTML + ' <a href="#" id="' + nameNuevo + '" name="' + nameNuevo + '" title="' + title + '" onclick="' + onclick.substr(0, onclick.length - 1) + '" class="remove">' + form.eliminar + '</a>';
            //li.appendChild(nuevo);

            agregame.appendChild(li);
        }
        else {
            agregame.removeChild(elementosAgregado[0]);
        }
    }
    if (agregame.childNodes.length > 0) {
        $('.group.filterSpace').css('display', '');
        $('.searchBy').css('display', '');
    }
    else {
        $('.searchBy').css('display', 'none');
    }

}

function HayFiltrosActivos(pFiltros) {
    $('#ctl00_ctl00_txtBusqueda').val('');
    if (pFiltros != "") {
        var filtros = pFiltros.split('|');
        for (var i = 0; i < filtros.length; i++) {
            if (filtros[i].indexOf('pagina=') == -1 && filtros[i].indexOf('orden=') == -1 && filtros[i].indexOf('ordenarPor=') == -1) {
                if (filtros[i].indexOf('search=') != -1) {
                    $('#ctl00_ctl00_txtBusqueda').val(filtros[i].substring(filtros[i].indexOf('=') + 1).replace('|', ''));
                }
                return true;
            }
        }
    }
    return false;
}

//Realizamos la petición


function replaceAll(texto, busqueda, reemplazo) {
    var resultado = '';

    while (texto.toString().indexOf(busqueda) != -1) {
        resultado += texto.substring(0, texto.toString().indexOf(busqueda)) + reemplazo;
        texto = texto.substring(texto.toString().indexOf(busqueda) + busqueda.length, texto.length);
    }

    resultado += texto;

    return resultado;
}

//montamos el contexto de los mensajes
function MontarContextoMensajes(pUsuarioID, pIdentidadID, pMensajeID, pLanguageCode, pParametrosBusqueda, pPanelID) {

    var servicio = new WS($('input.inpt_UrlServicioContextos').val(), WSDataType.jsonp);

    var metodo = 'CargarContextoMensajes';
    var params = {};

    params['usuarioID'] = pUsuarioID;
    params['identidadID'] = pIdentidadID;
    params['mensajeId'] = pMensajeID;
    params['languageCode'] = pLanguageCode;
    params['pParametrosBusqueda'] = pParametrosBusqueda;

    servicio.call(metodo, params, function (data) {
        var panel = $('#' + pPanelID);

        panel.html(data);

        //si divContexto tiene mensajes muestro el divContenedorContexto
        if (panel.children().children().length > 0) {
            document.getElementById('divContenedorContexto').style['display'] = "block";
        }
        else {
            document.getElementById('divContenedorContexto').style['display'] = "none";
        }

        //intentamos limpiar el panel que sobra
        if (panel.children('#ListadoGenerico_panContenedor').length > 0) {
            var panelResultados = $([panel.children('#ListadoGenerico_panContenedor').html()].join(''));
            panelResultados.appendTo(panel);
            panel.children('#ListadoGenerico_panContenedor').html('');
        }

        /* enganchar comportamiento mensajes */
        listadoMensajesMostrarAcciones.init();
        vistaCompactadaMensajes.init();

        if (typeof CompletadaCargaContextoMensajes == 'function') {
            CompletadaCargaContextoMensajes();
        }
    });
}

var utilMapas = {
    puntos: [],
    infowindow: null,
    mapbounds: null,
    gmarkers: [],
    gmarkersAgrupados: [],
    groutes: [],
    map: null,
    geocoder: null,
    markerCluster: null,
    markerClusterAgrupados: null,
    lat: 0,
    long: 0,
    zoom: 2,
    UltimaCoordenada: null,
    filtroCoordenadasMapaLat: null,
    filtroCoordenadasMapaLong: null,
    configuracionMapa: null,
    fichaRecurso: false,
    puntoRecurso: null,
    fichaMapa: 'listing-preview-map',
    contenedor: null,
    address: null,
    region: 'ES',
    IDMap: null,
    filtroWhere: null,
	listenerAdded:false,

    EstablecerFiltroCoordMapa: function (pFiltroLat, pFiltroLong) {
        this.filtroCoordenadasMapaLat = pFiltroLat;
        this.filtroCoordenadasMapaLong = pFiltroLong;
    },

    EstablecerParametrosMapa: function (pAddress, pRegion, pIDMap, pFiltroWhere) {
        this.address = pAddress;
        this.region = pRegion;
        this.IDMap = pIDMap;
        this.filtroWhere = pFiltroWhere;
    },

    MontarMapaResultados: function (pContenedor, pDatos) {
        this.contenedor = pContenedor;
        if (this.map == null) {
            var mapOptions = {
                zoom: this.zoom,
                minZoom: 2,
                center: new google.maps.LatLng(this.lat, this.long),
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                panControl: true,
                panControlOptions: {
                    position: google.maps.ControlPosition.RIGHT_BOTTOM
                },
                zoomControlOptions: {
                    position: google.maps.ControlPosition.RIGHT_BOTTOM,
                    style: google.maps.ZoomControlStyle.LARGE
                }

            }

            if (this.fichaRecurso) {
                if (pDatos.length == 2) {
                    this.puntoRecurso = [pDatos[1].split(',')[2], pDatos[1].split(',')[3]];
                }
            } else {
                $(this.contenedor).attr('class', 'listadoRecursos mapabusqueda');
            }

            if ((typeof ConfigurarEstilosMapa != 'undefined')) {
                ConfigurarEstilosMapa(this.contenedor);
            }

            this.map = new google.maps.Map($(this.contenedor)[0], mapOptions);

            utilMapas.UltimaCoordenada = null;
            this.markerCluster = null;
            this.markerClusterAgrupados = null;

            if (this.address != null) {
                this.geocoder = new google.maps.Geocoder();

                this.geocoder.geocode({ 'address': this.address, 'region': this.region }, function (results, status) {
                    var latDocumentoID = null;
                    var longDocumentoID = null;
                    for (var i = 1; i < pDatos.length; i++) {
                        if (pDatos[i] != '') {
                            var datos = pDatos[i].split(',');
                            if (datos.length == 4) {
                                if (datos[1] == "documentoid") {
                                    latDocumentoID = datos[2];
                                    longDocumentoID = datos[3];
                                }
                            }
                        }
                    }

                    if (status == google.maps.GeocoderStatus.OK) {
                        if (latDocumentoID != null && longDocumentoID != null) {
                            utilMapas.map.setCenter(new google.maps.LatLng(latDocumentoID, longDocumentoID));
                            utilMapas.map.setZoom(13);
                        } else {
                            utilMapas.map.setCenter(results[0].geometry.location);
                            utilMapas.map.fitBounds(results[0].geometry.bounds);
                        }
                        utilMapas.UltimaCoordenada = utilMapas.map.getBounds();
                    }
                });
            }

            if (this.IDMap != null) {
                var layer = new google.maps.FusionTablesLayer({
                    query: {
                        select: 'geometry',
                        from: this.IDMap,
                        where: this.filtroWhere
                    },
                    styles: [{
                        polygonOptions:
                        {
                            fillColor: "#ffffff",
                            fillOpacity: 0.1
                        }
                    }],
                    map: utilMapas.map,
                    suppressInfoWindows: true
                });
            }
        }

        this.puntos = pDatos;
        this.mapbounds = new google.maps.LatLngBounds();
        this.gmarkers = [];
        this.infowindow = new google.maps.InfoWindow({ content: '' });

        var me = this;

        var puntosDefinidos = 0;

        this.CargarConfiguracionMapa();
        this.OcultarFichaMapa();

        for (var i = 0; i < this.groutes.length; i++) {
            this.groutes[i].setMap(null);
        }
        this.groutes = [];

        for (var i = 1; i < this.puntos.length; i++) {
            if (this.puntos[i] != '') {
                var datos = this.puntos[i].split(',');
                if (datos.length == 4) {
                    me.DefinirPunto(this.map, datos[0], datos[2], datos[3], datos[1]);
                } else {
                    var color = '';
                    if (datos.length > 5) {
                        color = datos[5];
                    }

                    me.DefinirRuta(this.map, datos[2], datos[3], datos[4].replace(/\;/g, ','), datos[1], color);
                }
                puntosDefinidos++;
            }
        }

        var filtros = ObtenerHash2();
        var filtrandoPorLatLong = filtros.indexOf(this.filtroCoordenadas > -1)

        if (!this.fichaRecurso && (puntosDefinidos > 0 && !filtrandoPorLatLong)) {
            this.map.fitBounds(this.mapbounds);
            this.map.setCenter(this.mapbounds.getCenter());
        }

        if (this.fichaRecurso) {
            this.map.setCenter(this.mapbounds.getCenter());
            if (this.puntos.length > 2) {
                this.map.fitBounds(this.mapbounds);
            } else {
                this.map.setZoom(13);
            }

        }

        this.PintarMarcas();

        if (!filtrandoPorLatLong) {
            if (this.map.getZoom() > 10) {
                this.map.setZoom(10);
            }
        }
        var that = this;

		if(!this.listenerAdded){
			this.listenerAdded = true;
			google.maps.event.addListener(this.map, 'bounds_changed', function () {
				if (!that.fichaRecurso && utilMapas.filtroCoordenadasMapaLat != null && utilMapas.filtroCoordenadasMapaLong != null) {
					that.OcultarFichaMapa();
					var coordenadas = this.getBounds();
					if (utilMapas.UltimaCoordenada == null) {
						utilMapas.UltimaCoordenada = coordenadas;
					} else if (utilMapas.UltimaCoordenada != coordenadas) {
						utilMapas.UltimaCoordenada = coordenadas;
						setTimeout("utilMapas.FiltrarPorCoordenadas('" + coordenadas + "')", 1000);
					}
				}
			});
		}
    },
    OcultarFichaMapa: function () {
        $('#' + this.fichaMapa).attr('activo', 'false');
        $('#' + this.fichaMapa).hide();
    },

    MostrarFichaMapa: function () {
        $('#' + this.fichaMapa).attr('activo', 'true');
        $('#' + this.fichaMapa).show();
    }
    ,
    DefinirPunto: function (map, id, lat, lon, tipo) {
        lat = parseFloat(lat) + (Math.random() - 0.5) / 60000;
        lon = parseFloat(lon) + (Math.random() - 0.5) / 60000;
        var claveLatLog = lat + '&' + lon;

        var icon = null;
        if (this.configuracionMapa != null && this.configuracionMapa.ImagenesPorTipo != null && this.configuracionMapa.ImagenesPorTipo[tipo] != null) {
            icon = this.configuracionMapa.ImagenesPorTipo[tipo];
        }

        var me = this;
        var marker = new google.maps.Marker({
            position: new google.maps.LatLng(lat, lon),//,
            //map: map,
            icon: icon
            //title: "Hello World " + id
        });

        marker.documentoID = id.substring(id.lastIndexOf('/') + 1);
        marker.tipo = 'punto';

        if (this.configuracionMapa != null && this.configuracionMapa.EstilosGrupos.tipos != null && this.configuracionMapa.EstilosGrupos.tipos[tipo] != null) {
            var nombreGrupo = this.configuracionMapa.EstilosGrupos.tipos[tipo];
            for (var i = 0; i < this.gmarkersAgrupados.length; i++) {
                if (this.gmarkersAgrupados[i][0] == nombreGrupo) {
                    this.gmarkersAgrupados[i][1].push(marker);
                }
            }
        } else {
            this.gmarkers.push(marker);
        }

        if (id != "") {
            google.maps.event.addListener(marker, 'click', function () {
                me.CargarPunto(map, marker, claveLatLog, tipo);
            });
        }

        me.mapbounds.extend(marker.position);
    },

    CargarPunto: function (map, marker, claveLatLog, tipo) {
        var docIDs = marker.documentoID;

        if (this.configuracionMapa != null && this.configuracionMapa.popup != null && this.configuracionMapa.popup == 'personalizado') {
            //1º Obtenemos las coordenadas
            var overlay = new google.maps.OverlayView();
            overlay.draw = function () { };
            overlay.setMap(map);

            var panelY = 0;
            var panelX = 0;

            if (marker.tipo == 'punto') {
                var posicionMapa = overlay.getProjection().fromLatLngToContainerPixel(marker.getPosition());
                var posicionDiv = $(this.contenedor).offset();
                panelY = posicionMapa.y + posicionDiv.top + (marker.anchorPoint.y / 2);
                panelX = posicionMapa.x + posicionDiv.left;
            } else {
                panelX = currentMousePos.x;
                panelY = currentMousePos.y;
            }

            var that = this;

            //2º Ocultamos el panel
            that.OcultarFichaMapa();
            $('#' + this.fichaMapa).unbind();
            $('#' + this.fichaMapa).mouseleave(function () {
                that.OcultarFichaMapa();
            });
            $('#aspnetForm').mouseup(function () {
                that.OcultarFichaMapa();
            });
            $('#' + this.fichaMapa).attr('activo', 'true')

            TraerRecPuntoMapa(null, this.fichaMapa, claveLatLog, docIDs, '', panelX, panelY, tipo);
        } else {
            var me = this;
            me.infowindow.setContent('<div><p>' + form.cargando + '...</p></div>');
            me.infowindow.open(map, marker);
            TraerRecPuntoMapa(me, null, claveLatLog, docIDs);
        }
    },

    CargarConfiguracionMapa: function () {
        if (typeof (CargarConfiguracionMapaComunidad) != 'undefined') {
            this.configuracionMapa = CargarConfiguracionMapaComunidad();
        }
        if (this.configuracionMapa != null && this.configuracionMapa.EstilosGrupos != null) {
            this.gmarkersAgrupados = [];
            for (var tipo in this.configuracionMapa.EstilosGrupos.tipos) {
                var nombreGrupo = this.configuracionMapa.EstilosGrupos.tipos[tipo];
                var yaexiste = false;
                for (var i = 0; i < this.gmarkersAgrupados.length; i++) {
                    if (this.gmarkersAgrupados[i][0] == nombreGrupo) {
                        yaexiste = true;
                    }
                }
                if (!yaexiste) {
                    var array = new Array(2);
                    array[0] = nombreGrupo;
                    array[1] = [];
                    this.gmarkersAgrupados.push(array);
                }
            }
        }
    },

    DefinirRuta: function (map, tipo, id, puntos, docID, pColor) {
        try {
            var puntosParseados = JSON && JSON.parse(puntos) || $.parseJSON(puntos);
            var puntosRuta = [];
            for (var i = 0; i < puntosParseados[id].length; i++) {
                puntosRuta[i] = new google.maps.LatLng(puntosParseados[id][i][0], puntosParseados[id][i][1]);
            }

            if (pColor == '' || pColor == null) {
                pColor = ColorAleatorio();
            }

            var ruta = new google.maps.Polyline({
                path: puntosRuta,
                geodesic: true,
                strokeColor: pColor,
                //strokeColor: ColorAleatorio(),
                strokeOpacity: 1.0,
                strokeWeight: 2
            });

            var indicePuntoMedio = Math.floor(puntosParseados[id].length / 2);
            var coordenadasPtoMedio = puntosParseados[id][indicePuntoMedio];
            var lat = coordenadasPtoMedio[0];
            var lon = coordenadasPtoMedio[1];

            this.groutes.push(ruta);

            ruta.documentoID = docID.substring(docID.lastIndexOf('/') + 1);
            ruta.tipo = 'ruta';

            var me = this;
            google.maps.event.addListener(ruta, 'click', function () {
                var claveLatLog = puntosParseados[id][0][0] + '&' + puntosParseados[id][0][1];
                me.CargarPunto(map, ruta, claveLatLog, tipo);
            });

            ruta.setMap(map);

            this.DefinirPunto(map, docID, lat, lon, tipo);
        }
        catch (ex) { }
    },

    PintarMarcas: function () {
        var maxZoomLevel = 21;
        if (this.markerCluster == null) {
            this.markerCluster = new MarkerClusterer(this.map, this.gmarkers, { maxZoom: maxZoomLevel - 1 });
        } else {
            var marKersAntiguos = this.markerCluster.markers_;
            var marKersNuevos = this.gmarkers;

            var marKersAnyadir = this.PintarMarcasAux(marKersNuevos, marKersAntiguos);
            var marKersEliminar = this.PintarMarcasAux(marKersAntiguos, marKersNuevos);

            this.markerCluster.removeMarkers(marKersEliminar);
            this.markerCluster.addMarkers(marKersAnyadir);
        }

        if (this.markerClusterAgrupados == null) {
            this.markerClusterAgrupados = [];
            for (var i = 0; i < this.gmarkersAgrupados.length; i++) {
                this.markerClusterAgrupados[i] = new MarkerClusterer(this.map, this.gmarkersAgrupados[i][1], {
                    maxZoom: maxZoomLevel - 1,
                    styles: this.configuracionMapa.EstilosGrupos[this.gmarkersAgrupados[i][0]]
                });
            }
        } else {
            for (var i = 0; i < this.gmarkersAgrupados.length; i++) {
                var marKersAntiguos = this.markerClusterAgrupados[i].markers_;
                var marKersNuevos = this.gmarkersAgrupados[i][1];

                var marKersAnyadir = this.PintarMarcasAux(marKersNuevos, marKersAntiguos);
                var marKersEliminar = this.PintarMarcasAux(marKersAntiguos, marKersNuevos);

                this.markerClusterAgrupados[i].removeMarkers(marKersEliminar);
                this.markerClusterAgrupados[i].addMarkers(marKersAnyadir);
            }
        }
    },

    PintarMarcasAux: function (pMarkersA, pMarkersB) {
        //Devuelve un array con todos los markers de A que no esten en B
        var markers = [];
        for (var i = 0; i < pMarkersA.length; i++) {
            var existe = false;
            for (var j = 0; j < pMarkersB.length; j++) {
                if (pMarkersB[j].documentoID == pMarkersA[i].documentoID) {
                    existe = true;
                }
            }
            if (!existe) {
                markers.push(pMarkersA[i]);
            }
        }
        return markers;
    },

    AjustarBotonesVisibilidad: function () {
        /*
        var vistaMapa = ($('li.mapView').attr('class') == "mapView activeView");
        var vistaChart = ($('.chartView').attr('class') == "chartView activeView");
        */
        var vistaMapa = $('li.mapView').hasClass('activeView');
        var vistaChart = $('.chartView').hasClass('activeView');

        var mapView = $('.mapView');

        if (vistaMapa || vistaChart) {
            var listView = $('.listView');
            var gridView = $('.gridView');

            $('a', listView).unbind();
            $('a', listView).bind('click', function (evento) {
                evento.preventDefault();
                $('.listView').attr('class', 'listView activeView');
                $('.gridView').attr('class', 'gridView');
                $('.mapView').attr('class', 'mapView');
                $('.chartView').attr('class', 'chartView');
                $('div.mapabusqueda').attr('class', 'listadoRecursos');
                FiltrarPorFacetas(ObtenerHash2());
            });

            $('a', gridView).unbind();
            $('a', gridView).bind('click', function (evento) {
                evento.preventDefault();
                $('.listView').attr('class', 'listView');
                $('.gridView').attr('class', 'gridView activeView');
                $('.mapView').attr('class', 'mapView');
                $('.chartView').attr('class', 'chartView');
                $('div.mapabusqueda').attr('class', 'listadoRecursos');
                FiltrarPorFacetas(ObtenerHash2());
            });

            $('.panelOrdenContenedor').css('display', 'none');
        }
        else {
            $('.panelOrdenContenedor').css('display', '');
        }

        if (vistaMapa) {
            $('a', mapView).unbind();
            $('a', mapView).bind('click', function (evento) {
                evento.preventDefault();
            });
        }
        else {
            $('a', mapView).unbind();
            $('a', mapView).bind('click', function (evento) {
                evento.preventDefault();
                $('.listView').attr('class', 'listView');
                $('.gridView').attr('class', 'gridView');
                $('.mapView').attr('class', 'mapView activeView');
                $('.chartView').attr('class', 'chartView');
                utilMapas.map = null;
                FiltrarPorFacetas(ObtenerHash2());
            });
        }

        if ($('#ctl00_ctl00_CPH1_CPHContenido_liDescargaExcel').length > 0 && $('#ctl00_ctl00_CPH1_CPHContenido_liDescargaExcel').css('display') != 'none' && !$('#ctl00_ctl00_CPH1_CPHContenido_liDescargaExcel').is(':visible')) {
            $('#ctl00_ctl00_CPH1_CPHContenido_divVista').css('display', '');
        }
    },

    FiltrarPorCoordenadas: function (coordenadas) {
        if (utilMapas.UltimaCoordenada == coordenadas) {
            var paramsCoord = coordenadas.replace(/\)/g, '').replace(/\(/g, '').split(",")
            var minLat = paramsCoord[0].trim();
            var maxLat = paramsCoord[2].trim();
            var minLong = paramsCoord[1].trim();
            var maxLong = paramsCoord[3].trim();

            var filtro = ObtenerHash2();

            filtro = this.ReemplazarFiltro(filtro, utilMapas.filtroCoordenadasMapaLat, minLat + '-' + maxLat);
            filtro = this.ReemplazarFiltro(filtro, utilMapas.filtroCoordenadasMapaLong, minLong + '-' + maxLong);

            history.pushState('', 'New URL: ' + filtro, '?' + filtro);
            FiltrarPorFacetas(filtro);
        }
    },

    ReemplazarFiltro: function (filtros, tipoFiltro, filtro) {

        //Si el filtro ya existe, cambiamos el valor del filtro
        if (filtros.indexOf(tipoFiltro) == 0 || filtros.indexOf('&' + tipoFiltro) > 0) {
            var textoAux = filtros.substring(filtros.indexOf(tipoFiltro));
            if (textoAux.indexOf('&') > -1) {
                textoAux = textoAux.substring(0, textoAux.indexOf('&'));
            }
            filtros = filtros.replace(textoAux, tipoFiltro + '=' + filtro);
        }
        else {
            if (filtros.length > 0) { filtros += '&'; }
            filtros += tipoFiltro + '=' + filtro;
        }

        return filtros;
    }
}

function ColorAleatorio() {
    hexadecimal = new Array("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F")
    color_aleatorio = "#";
    for (i = 0; i < 6; i++) {
        posarray = aleatorio(0, hexadecimal.length)
        color_aleatorio += hexadecimal[posarray]
    }
    return color_aleatorio
}

function aleatorio(inferior, superior) {
    numPosibilidades = superior - inferior
    aleat = Math.random() * numPosibilidades
    aleat = Math.floor(aleat)
    return parseInt(inferior) + aleat
}

var contador = 0;

var viewOptions = {

    id: '#viewOptions',

    cssResources: '.resourcesList',

    cssResourcesRow: 'row',

    cssResourcesGrid: 'grid',

    cssGridView: '.gridView',

    cssRowView: '.rowView',

    cssResource: '.resource',

    cssActive: 'activeView',

    isCalculadoOmega: false,

    alturas: [],

    init: function () {

        this.config();

        this.behaviours();

    },

    config: function () {

        this.componente = $(this.id);

        this.resourceList = this.componente.next();

        this.gridView = $(this.cssGridView, this.componente);

        this.rowView = $(this.cssRowView, this.componente);

        this.links = $('a', this.componente);

        this.resources = $(this.cssResource, this.resourceList);

        return;

    },

    calcularOmega: function () {

        var contador = 0;

        var masAlto = 0;

        var that = this;

        this.resources.each(function () {

            var recurso = $(this);

            var altura = recurso.height();

            if (altura > masAlto) masAlto = altura;

            contador++;

            if (contador == 3) {

                recurso.addClass('omega');

                that.alturas.push(masAlto);

                contador = 0;

                masAlto = 0;

            }

        });

        this.resources.each(function () {

            var recurso = $(this);

            recurso.css('height', masAlto + 'px');

        });

        this.isCalculadoOmega = true;

    },

    igualarAlturas: function () {

        var that = this;

        console.log(this.alturas);

        var contador = 0;

        this.resources.each(function (indice) {

            var recurso = $(this);

            var fila = parseInt(indice / 3);

            recurso.css('height', that.alturas[fila] + 'px');

        });

    },

    borrarAlto: function () {

        this.resources.each(function () {

            var recurso = $(this);

            recurso.css('height', 'auto');

        });

    },

    desmarcarActivo: function () {

        var that = this;

        this.links.each(function () {

            $(this).removeClass(that.cssActive);

        })

        return;

    },

    behaviours: function () {

        var that = this;

        this.gridView.bind('click', function (evento) {

            that.desmarcarActivo();

            $(this).addClass(that.cssActive);

            that.resourceList.removeClass(that.cssResourcesRow);

            that.resourceList.addClass(that.cssResourcesGrid);

            if (!that.isCalculadoOmega) that.calcularOmega();

            that.igualarAlturas();

            return false;

        });

        this.rowView.bind('click', function (evento) {

            that.desmarcarActivo();

            $(this).addClass(that.cssActive);

            that.resourceList.removeClass(that.cssResourcesGrid);

            that.resourceList.addClass(that.cssResourcesRow);

            that.borrarAlto();

            return false;

        });
    }
}


/*--------    REGION MASTER GNOSS    -----------------------------------------------------------*/

function aceptarNuevaNavegacion() {
    document.getElementById('capaModal').style.display = 'none';
}

//Codigo para crear el desplegable del login mediante jQuery


//function submitform() {
//    document.myform.submit();
//}


function OculatarHerramientaAddto() {
    //    if ($.browser.msie && $.browser.version < 7) {
    //        idIntervalo = setInterval("accederWeb()", 500);
    //    }
}

function CambiarNombre(link, nombre1, nombre2) {
    if (link.innerHTML == nombre1) {
        link.innerHTML = nombre2;
    }
    else {
        link.innerHTML = nombre1;
    }
}

/*--------    REGION SCRIPTS INICIALES   -----------------------------------------------------------*/

function ReceiveServerData(arg, context) {
    EjecutarScriptsIniciales();
    if (!(arg == "")) {
        var json = eval('(' + arg + ')');
        for (var i = 0; i < json.length; i++) {
            var object = json[i];
            if (object.render == true) {
                if (object.id != "") {
                    var element = document.getElementById(object.id);

                    if (element != null) {
                        if (object.reemplazarHtml) {
                            if (object.innerHTML == "") {
                                element.parentNode.removeChild(element);
                            }
                            else {
                                var divNuevoHtml = document.createElement("div");
                                divNuevoHtml.innerHTML = object.innerHTML;
                                element.parentNode.replaceChild(divNuevoHtml.childNodes[0], element);
                            }
                        }
                        else if (object.agregarHtml) {
                            element.innerHTML += object.innerHTML;
                        }
                        else if (object.agregarHtmlAlPrinc) {
                            element.innerHTML = object.innerHTML + element.innerHTML;
                        }
                        else {
                            if (element != null) { element.innerHTML = object.innerHTML; }
                        }
                    }
                } //Fin if(object.id!=\"\")
            } //Fin if(object.render == true), empieza esle
            else {
                var cadena = object.funcion.split('&');
                object.funcion = cadena[0];
                if (object.funcion == "Seleccionar") {
                    SeleccionarElemento(object.id);
                }
                else if (object.funcion == "DeSeleccionar") {
                    DeSeleccionarElemento(object.id);
                }
                else if (object.funcion == "Contraer") {
                    var element = document.getElementById(object.id);
                    element.style.display = "none";
                }
                else if (object.funcion == "HacerVisible") {
                    var element = document.getElementById(object.id);
                    element.style.display = "block";
                }
                else if (object.funcion == "Ocultar") {
                    var element = document.getElementById(object.id);
                    if (element != null) {
                        element.style.display = "none";
                    }
                }
                else if (object.funcion == "CambiarConector") {
                    var element = document.getElementById(object.id);
                    element.src = object.innerHTML;
                }
                else if (object.funcion == "CambiarCssClass") {
                    var element = document.getElementById(object.id);
                    element.className = object.innerHTML;
                }
                else if (object.funcion == "CambiarValor") {
                    var element = document.getElementById(object.id);
                    element.value = object.innerHTML;
                }
                else if (object.funcion == "CambiarAtributo") {
                    var element = document.getElementById(object.id);
                    element.setAttribute(object.atributo, object.valor);
                }
                else if (object.funcion == "CambiarAtributoStyle") {
                    var element = document.getElementById(object.id);
                    element.style[object.atributo] = object.valor;
                }
                else if (object.funcion == "CambiarID") {
                    var element = document.getElementById(object.id);
                    element.id = object.innerHTML;
                }
                else if (object.funcion == "Guardar") {
                    Guardar(object.innerHTML);
                }
                else if (object.funcion == "MarcarCheckBox") {
                    $('#' + object.id).attr('checked', true);
                }
                else if (object.funcion == "DesmarcarCheckBox") {
                    $('#' + object.id).attr('checked', false);
                }
                else if (object.funcion == "SweetTitles") {
                    if (window.sweetTitles) { sweetTitles.init(); }
                }
                else if (object.funcion == "MostrarErrorLogin") {
                    crearError('<p>' + form.errorLogin + '</p>', cadena[1]);
                }
                else if (object.funcion == "redirect") {
                    if (object.Direccion != null) {
                        window.location.href = object.Direccion;
                    }
                    else {
                        window.location.href = cadena[1];
                    }
                }
                else if (object.funcion == "hacerClick") {
                    document.getElementById(cadena[1]).click();
                }
                else if (object.funcion == "ejecutarFuncion") {
                    eval(object.innerHTML);
                }
                else if (object.funcion == "ejecutarFuncionConParametorObjeto") {
                    eval(object.metodoJS + '(object);');
                }
                else if (object.funcion == "ResaltarTags" && object.ListaTags != "") {
                    ResaltarTags(object.ListaTags);
                }
                else if (object.funcion == "reload") {
                    document.location.reload();
                }

                if (object.funcion == "TraerNumeroResultados") {
                    FiltrarBusquedaAvanzada(object.numResulCoincidentes);
                }

                if (object.funcion == "MostrarControles") {
                    MostrarControles(object.controles);
                }
            } //Fin else de if(object.render == true), empieza esle
        } //Fin foreach
    } //Fin if(!(arg == \"\"))
} //Fin function

//Seleccionar
function SeleccionarElemento(elementID) {
    var element = document.getElementById(elementID);
    element.style.border = "solid 1px black";
}
//DeSeleccionar
function DeSeleccionarElemento(elementID) {
    var element = document.getElementById(elementID);
    element.style.border = "";
}
function CambiarNombreElemento(elementID, nombre) {
    var element = document.getElementById(elementID);
    element.innerHTML = '';
    element.parentNode.innerHTML = nombre;
}
function CambiarTextoElemento(elementID, nombre) {
    var element = document.getElementById(elementID);
    element.innerHTML = nombre;
}

/**
 * Eliminar� los atributos del bot�n para que no pueda volver a ejecutar nada a menos que se vuelva a carguar la p�gina web
 * Ej: Acciones que se hacen sobre una persona ("No enviar newsletter, Bloquear...")
 * @param {any} elementId: Elemento que se desea cambiar el nombre y eliminar atributos
 * @param {any} nombre: Nombre que tendr� el bot�n una vez se haya pulsado sobre �l y las acciones se hayan realizado
 * @param {any} listaAtributos: Lista de atributos en formato String que ser�n eliminados del bot�n (Ej: "data-target", "href", "onclick")
 * */
function CambiarTextoAndEliminarAtributos(elementId, nombre, listaAtributos) {
    // Seleccionamos el elemento
    var element = $('#' + elementId);
    // Eliminar la lista de atributos deseados
    listaAtributos.forEach(atributo => $(element).removeAttr(atributo));
    // Cambiamos el nombre del elemento
    $(element).html(nombre)
    // A�adimos estilo para que no parezca que es "clickable"
    $(element).css('cursor', 'auto');
}

//Función que arregla pringues de Microsoft:
function WebForm_CallbackComplete() {
    for (var i = 0; i < __pendingCallbacks.length; i++) {
        callbackObject = __pendingCallbacks[i];
        if (callbackObject && callbackObject.xmlRequest && (callbackObject.xmlRequest.readyState == 4)) {
            WebForm_ExecuteCallback(callbackObject);
            if (__pendingCallbacks[i] != null && !__pendingCallbacks[i].async) {
                __synchronousCallBackIndex = -1;
            }
            __pendingCallbacks[i] = null;
            var callbackFrameID = "__CALLBACKFRAME" + i;
            var xmlRequestFrame = document.getElementById(callbackFrameID);
            if (xmlRequestFrame) {
                xmlRequestFrame.parentNode.removeChild(xmlRequestFrame);
            }
        }
    }
}

////Realizamos la petición
//function DesplegarDescripcionAJAX(pGuidDoc) {
//    PeticionesAJAX.CargarDescripcionRecurso(pGuidDoc, RecogerDescripcion, RecogerErroresAJAX);
//}

//function DesplegarDescripcionSuscripcionAJAX(pGuidDoc, pGuidSusc) {
//    PeticionesAJAX.CargarDescripcionRecursoSuscripcion(pGuidDoc, pGuidSusc, RecogerDescripcion, RecogerErroresAJAX);
//}

//function RecogerDescripcion(datosRecibidos) {
//    //Leemos los datos
//    var docID = datosRecibidos.substring(0, datosRecibidos.indexOf('|'));
//    var descripcion = datosRecibidos.substring(datosRecibidos.indexOf('|') + 1);
//    //Ocultamos la descripción corta y mostramos la larga
//    $('#DescripcionCorta_' + docID).hide();
//    $('#DescripcioLarga_' + docID).show();
//    $('#DescripcioLarga_' + docID).html(descripcion);
//    //Cambiamos la imágen del + y quitamos los puntos suspensivos
//    //La variable  $('input.inpt_baseURL').val() se define en la MasterGnoss
//    $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('a').children('img').attr('src',  $('input.inpt_baseURL').val() + '/img/verMenos.gif');
//    $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('span').html('');
//    //Quitamos el evento onclick del +
//    $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('a').removeAttr('onclick');
//    //Creamos el evento que alterna entre las descripciones
//    $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('a').toggle(function () {
//        $('#DescripcionCorta_' + docID).show();
//        $('#DescripcioLarga_' + docID).hide();
//        $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('a').children('img').attr('src',  $('input.inpt_baseURL').val() + '/img/verMas.gif');
//        $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('span').html('...');
//    },
//function () {
//    $('#DescripcionCorta_' + docID).hide();
//    $('#DescripcioLarga_' + docID).show();
//    $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('a').children('img').attr('src',  $('input.inpt_baseURL').val() + '/img/verMenos.gif');
//    $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('span').html('');
//});
//}

function RecogerErroresAJAX(error) {
    //alert(error);
}

function ObtenerNumMensajesSinLeerAJAX(pGuidIdent) {
    PeticionesAJAX.CargarNumMensajesSinLeer(pGuidIdent, RecogerNumMensajesSinLeer, RecogerErroresAJAX);
}

function RecogerNumMensajesSinLeer(datosRecibidos) {
    //Leemos los datos
    var arrayDatos = datosRecibidos.split('|');
    var numMensajesNoLeidos = arrayDatos[0];
    var numMensajesNoLeidosEliminados = arrayDatos[1];

    //Cambiamos el numero de mensajes recibidos
    reemplazarContadores('#ctl00_CPH1_hlCorreoRecibido', numMensajesNoLeidos);

    //Cambiamos el numero de mensajes recibidos
    reemplazarContadores('#ctl00_CPH1_hlCorreoEliminado', numMensajesNoLeidosEliminados);
}


function ObtenerNumElementosSinLeerAJAX(pGuidPerfilUsu, pGuidPerfilOrg, pGuidOrg, pEsBandejaOrg, pCaducidadSusc) {
    PeticionesAJAX.CargarNumElementosSinLeer(pGuidPerfilUsu, pGuidPerfilOrg, pGuidOrg, pEsBandejaOrg, pCaducidadSusc, RecogerNumElementosSinLeer, RecogerErroresAJAX);
}

function RecogerNumElementosSinLeer(datosRecibidos) {
    //Leemos los datos
    var arrayDatos = datosRecibidos.split('|');
    var numMensajesNoLeidos = arrayDatos[0];
    var numInvitacionesNoLeidas = arrayDatos[1];
    var numSuscripcionesNoLeidos = arrayDatos[2];
    var numComentariosNoLeidos = arrayDatos[3];

    //Cambiamos el numero de Mensajes
    reemplazarContadores('#ctl00_ctl00_CPH1_hlMensajes', numMensajesNoLeidos);

    //Cambiamos el numero de Comentarios
    reemplazarContadores('#ctl00_ctl00_CPH1_hlComentarios', numComentariosNoLeidos);

    //Cambiamos el numero de Invitaciones
    reemplazarContadores('#ctl00_ctl00_CPH1_hlInvitaciones', numInvitacionesNoLeidas);

    //Cambiamos el numero de Suscripciones
    reemplazarContadores('#ctl00_ctl00_CPH1_hlSuscripciones', numSuscripcionesNoLeidos);

}

var perfilID;
var perfilOrgID;
var organizacionID;
var esAdministradorOrg;
$(document).ready(function () {
    perfilID = $('input#inpt_perfilID').val();
    perfilOrgID = $('input#inpt_perfilOrgID').val();
    organizacionID = $('input#inpt_organizacionID').val();
    esAdministradorOrg = $('input#inpt_AdministradorOrg').val();
    refrescarNumElementosNuevos();
    //$('#descargarRDF').click(function () {
    //    var url = window.location.href;

    //    if (url.indexOf('?rdf') == -1 && url.indexOf('&rdf') == -1) {
    //        if (url.indexOf('?') == -1) {
    //            url += '?';
    //        }
    //        else {
    //            url += '&';
    //        }

    //        url += 'rdf';
    //    }

    //    window.open(url, '_blank');
    //    return false;
    //});
    engancharClicks();
});

function engancharClicks() {
    $('[clickJS]').each(function () {
        var control = $(this);
        var js = control.attr('clickJS');
        control.removeAttr('clickJS');
        control.click(function (evento) {
            evento.preventDefault();
            eval(js);
        });
    });
}


function refrescarNumElementosNuevos() {
    if (typeof (perfilID) != 'undefined' && perfilID != 'ffffffff-ffff-ffff-ffff-ffffffffffff' && perfilID != '00000000-0000-0000-0000-000000000000') {
        try {
            ObtenerNumElementosNuevosAJAX(perfilID, perfilOrgID, esAdministradorOrg);
            setTimeout("ObtenerNumElementosNuevosAJAX('" + perfilID + "', '" + perfilOrgID + "', '" + esAdministradorOrg + "')", 60000);
            setTimeout("ObtenerNumElementosNuevosAJAX('" + perfilID + "', '" + perfilOrgID + "', '" + esAdministradorOrg + "')", 240000);

            if ($('input.inpt_MantenerSesionActiva').length == 0 || $('input.inpt_MantenerSesionActiva').val().toLowerCase() == "true") {
                setInterval("ObtenerNumElementosNuevosAJAX('" + perfilID + "', '" + perfilOrgID + "', '" + esAdministradorOrg + "')", 600000);
            }
            
            //if ($('input.inpt_refescarContadoresSinLimite').val() == "1") {
            //    setInterval("ObtenerNumElementosNuevosAJAX('" + perfilID + "', '" + perfilOrgID + "', '" + esAdministradorOrg + "')", 600000);
            //}
            //else {
            //    setTimeout("ObtenerNumElementosNuevosAJAX('" + perfilID + "', '" + perfilOrgID + "', '" + esAdministradorOrg + "')", 600000);
            //}
        }
        catch (ex) { }
    }
}

function ObtenerNumElementosNuevosAJAX(pGuidPerfilUsu, pGuidPerfilOrg, pEsBandejaOrg) {
    var identCargarNov = '';
    var spanNov = $('span.novPerfilOtraIdent');

    if (spanNov.length > 0) {
        for (var i = 0; i < spanNov.length; i++) {
            identCargarNov += $(spanNov[i]).attr('id').substring($(spanNov[i]).attr('id').indexOf('_') + 1) + '&';
        }
    }

    PeticionesCookie.CargarCookie();
    PeticionesAJAX.CargarNumElementosNuevos(pGuidPerfilUsu, pGuidPerfilOrg, pEsBandejaOrg, identCargarNov, RepintarContadoresNuevosElementos, RecogerErroresAJAX);
}

function PeticionAJAX(pMetodo, pDatosPost,pFuncionOK,pFuncionKO)
{
    var urlPeticion = location.protocol + "//" + location.host + "/PeticionesAJAX/" + pMetodo;
    GnossPeticionAjax(
        urlPeticion,
        pDatosPost,
        true
    ).done(function (response) {
        pFuncionOK(response)
    }).fail(function (response) {
        pFuncionKO(response);
    });
}

/**
 * M�todo que es ejecutado para mostrar informaci�n traida del backend como Mensajes nuevos, invitaciones nuevas, suscripciones nuevas...
 * @param {any} datosRecibidos
 */
/**
 * M�todo que es ejecutado para mostrar informaci�n traida del backend como Mensajes nuevos, invitaciones nuevas, suscripciones nuevas...
 * @param {any} datosRecibidos
 */
function RepintarContadoresNuevosElementos(datosRecibidos) {
    //Leemos los datos
    var arrayDatos = datosRecibidos.split('|');
    var numMensajesNuevos = arrayDatos[0];
    var numInvitacionesNuevos = arrayDatos[1];
    var numSuscripcionesNuevos = arrayDatos[2];
    var numComentariosNuevos = arrayDatos[3];

    var numMensajesSinLeer = arrayDatos[4];
    var numInvitacionesSinLeer = arrayDatos[5];
    var numSuscripcionesSinLeer = arrayDatos[6];
    var numComentariosSinLeer = arrayDatos[7];

    var numMensajesNuevosOrg = arrayDatos[8];
    var numMensajesSinLeerOrg = arrayDatos[9];
    var numInvitacionesNuevosOrg = arrayDatos[10];
    var numInvitacionesSinLeerOrg = arrayDatos[11];

    var numInvOtrasIdent = arrayDatos[12];

    // Identificaci�n de elementos HTML para controlar el n� de mensajes nuevos
    // Mensajes nuevos    
    const mensajesMenuNavegacionItem = document.querySelectorAll('.liMensajes')//$('#navegacion').find('.liMensajes');
    const suscripcionesMenuNavegacionItem = document.querySelectorAll('.liNotificaciones'); //$('#navegacion').find('.liNotificaciones');
    const comentariosMenuNavegacionItem = document.querySelectorAll('.liComentarios'); //$('#navegacion').find('.liComentarios');
    // Quitarlo -> No se utilizan 
    // const invitacionesMenuNavegacionItem = document.querySelectorAll('.liInvitaciones'); //$('#navegacion').find('.liInvitaciones');
    const contactosMenuNavegacionItem = document.querySelectorAll('.liContactos'); //$('#navegacion').find('.liContactos');

    //Cambiamos el numero de Mensajes
    DarValorALabel('infoNumMensajes', parseInt(numMensajesNuevos) + parseInt(numMensajesNuevosOrg));
    DarValorALabel('infoNumMensajesMobile', parseInt(numMensajesNuevos) + parseInt(numMensajesNuevosOrg));
    //Cambiamos el numero de Comentarios
    DarValorALabel('infoNumComentarios', numComentariosNuevos);
    DarValorALabel('infoNumComentariosMobile', numComentariosNuevos);
    //Cambiamos el numero de Invitaciones
    DarValorALabel('infoNumInvitaciones', parseInt(numInvitacionesNuevos) + parseInt(numInvitacionesNuevosOrg));
    DarValorALabel('infoNumInvitacionesMobile', parseInt(numInvitacionesNuevos) + parseInt(numInvitacionesNuevosOrg));
    //Cambiamos el numero de Notificaciones
    DarValorALabel('infoNumNotificaciones', parseInt(numInvitacionesNuevos) + parseInt(numInvitacionesNuevosOrg) + parseInt(numComentariosNuevos));
    DarValorALabel('infoNumNotificacionesMobile', parseInt(numInvitacionesNuevos) + parseInt(numInvitacionesNuevosOrg) + parseInt(numComentariosNuevos));
    //Cambiamos el numero de Suscripciones
    DarValorALabel('infoNumSuscriopciones', numSuscripcionesNuevos);
    DarValorALabel('infoNumSuscriopcionesMobile', numSuscripcionesNuevos);

    //Cambiamos el numero de Mensajes sin leer
    DarValorALabel('infNumMensajesSinLeer', parseInt(numMensajesSinLeer) + parseInt(numMensajesSinLeerOrg));
    DarValorALabel('infNumMensajesSinLeerMobile', parseInt(numMensajesSinLeer) + parseInt(numMensajesSinLeerOrg));
    // A�adir punto rojo de 'sin leer' de Mensajes
    if (parseInt(numMensajesSinLeer) + parseInt(numMensajesSinLeerOrg) > 0) $(mensajesMenuNavegacionItem).addClass('nuevos');
    //Cambiamos el numero de Comentarios sin leer
    DarValorALabel('infNumComentariosSinLeer', numComentariosSinLeer);
    DarValorALabel('infNumComentariosSinLeerMobile', numComentariosSinLeer); 
    // A�adir punto rojo de 'sin leer' de Comentarios
    if (parseInt(numComentariosSinLeer) > 0) $(comentariosMenuNavegacionItem).addClass('nuevos');
    //Cambiamos el numero de Invitaciones sin leer
    DarValorALabel('infNumInvitacionesSinLeer', parseInt(numInvitacionesSinLeer) + parseInt(numInvitacionesSinLeerOrg));
    DarValorALabel('infNumInvitacionesSinLeerMobile', parseInt(numInvitacionesSinLeer) + parseInt(numInvitacionesSinLeerOrg));
    // A�adir punto rojo de 'sin leer' de Invitaciones --> Quitarlo no se utiliza    
    // if (parseInt(numInvitacionesSinLeer) + parseInt(numInvitacionesSinLeerOrg) > 0) $(invitacionesMenuNavegacionItem).addClass('nuevos');
    //Cambiamos el numero de Notificaciones sin leer
    DarValorALabel('infNumNotificacionesSinLeer', parseInt(numInvitacionesSinLeer) + parseInt(numInvitacionesSinLeerOrg) + parseInt(numComentariosSinLeer));
    DarValorALabel('infNumNotificacionesSinLeerMobile', parseInt(numInvitacionesSinLeer) + parseInt(numInvitacionesSinLeerOrg) + parseInt(numComentariosSinLeer));
    //Cambiamos el numero de Suscripciones sin leer
    DarValorALabel('infNumSuscripcionesSinLeer', numSuscripcionesSinLeer);
    DarValorALabel('infNumSuscripcionesSinLeerMobile', numSuscripcionesSinLeer);
    // A�adir punto rojo de 'sin leer' de Suscripciones
    if (parseInt(numSuscripcionesSinLeer)) $(suscripcionesMenuNavegacionItem).addClass('nuevos');

    if ($('.mgHerramientas').length > 0) {
        //Cambiamos el numero de Mensajes
        DarValorALabelNovedades('infoNumMensajesNovedades', numMensajesNuevos);
        //Cambiamos el numero de Comentarios
        DarValorALabelNovedades('infoNumComentariosNovedades', numComentariosNuevos);
        //Cambiamos el numero de Invitaciones
        DarValorALabelNovedades('infoNumInvitacionesNovedades', numInvitacionesNuevos);
        //Cambiamos el numero de Notificaciones
        DarValorALabelNovedades('infoNumNotificacionesNovedades', parseInt(numInvitacionesNuevos) + parseInt(numComentariosNuevos));
        //Cambiamos el numero de Suscripciones
        DarValorALabelNovedades('infoNumSuscriopcionesNovedades', numSuscripcionesNuevos);

        //Cambiamos el numero de Mensajes sin leer
        DarValorALabelPendientes('infoNumMensajesSinLeerNovedades', numMensajesSinLeer);
        //Cambiamos el numero de Comentarios sin leer
        DarValorALabelPendientes('infoNumComentariosSinLeerNovedades', numComentariosSinLeer);
        //Cambiamos el numero de Invitaciones sin leer
        DarValorALabelPendientes('infoNumInvitacionesSinLeerNovedades', numInvitacionesSinLeer);
        //Cambiamos el numero de Notificaciones sin leer
        DarValorALabelPendientes('infoNumNotificacionesSinLeerNovedades', parseInt(numInvitacionesSinLeer) + parseInt(numComentariosSinLeer));
        //Cambiamos el numero de Suscripciones sin leer
        DarValorALabelPendientes('infoNumSuscripcionesSinLeerNovedades', numSuscripcionesSinLeer);

        if (numMensajesNuevosOrg > 0 || numMensajesSinLeerOrg > 0 || numInvitacionesNuevosOrg > 0 || numInvitacionesSinLeerOrg > 0) {
            //Cambiamos el numero de Mensajes de Org
            DarValorALabelNovedades('infoNumMensajesNovedadesOrg', numMensajesNuevosOrg);
            //Cambiamos el numero de Invitaciones de Org
            DarValorALabelNovedades('infoNumInvitacionesNovedadesOrg', numInvitacionesNuevosOrg);
            //Cambiamos el numero de Notificaciones de Org
            DarValorALabelNovedades('infoNumNotificacionesNovedadesOrg', numInvitacionesNuevosOrg);
            //Cambiamos el numero de Mensajes sin leer de Org
            DarValorALabelPendientes('infoNumMensajesSinLeerNovedadesOrg', numMensajesSinLeerOrg);
            //Cambiamos el numero de Invitaciones sin leer de Org
            DarValorALabelPendientes('infoNumInvitacionesSinLeerNovedadesOrg', numInvitacionesSinLeerOrg);
            //Cambiamos el numero de Notificaciones sin leer de Org
            DarValorALabelPendientes('infoNumNotificacionesSinLeerNovedadesOrg', numInvitacionesSinLeerOrg);
        }
    }

    if (numInvOtrasIdent != '') {
        var identRef = numInvOtrasIdent.split('&');

        for (var i = 0; i < identRef.length; i++) {
            if (identRef[i] != "") {
                var perfilID_infoNov = 'infoNov_' + identRef[i].split(':')[0];
                var numNov = parseInt(identRef[i].split(':')[1]);
                DarValorALabel(perfilID_infoNov, numNov);
            }
        }
    }
}

/**
 * Pintar el n�mero de elementos (mensajes sin leer, notificaciones, suscripciones) en la label correspondiente y a�ade la coletilla de "nuevos" o sin leer.
 * De momento elimino la opci�n de mostrar "nuevos" o "sin leer". 
 * @param {any} pLabelID
 * @param {any} pNumElementos
 */
function DarValorALabel(pLabelID, pNumElementos) {
    // Cambiado por nuevo Front para buscar por clase (El men� clonado tambi�n existe y no podr�a haber 2 elementos con un mismo ID)
    //if ($('#' + pLabelID).length > 0) {
    if ($('.' + pLabelID).length > 0) {
       // Cambiado por nuevo Front: Cambiado por nuevo Front para buscar por clase (El men� clonado tambi�n existe y no podr�a haber 2 elementos con un mismo ID). Hecho abajo
        // document.getElementById(pLabelID).innerHTML = pNumElementos;        

        if (pLabelID.indexOf('SinLeer') != -1) {
            // Cambiado por el nuevo Front. No deseamos que muestre "sin leer"
            // document.getElementById(pLabelID).innerHTML += '<span class="indentado">' + mensajes.sinLeer + '</span>';
        }
        else {
            // Cambiado por el nuevo Front. No deseamos que muestre "nuevos"
            // document.getElementById(pLabelID).innerHTML += '<span class="indentado">' + mensajes.nuevos + '</span>'
        }

        // Cambiado por nuevo Front para buscar por clase (El men� clonado tambi�n existe y no podr�a haber 2 elementos con un mismo ID)
        //if (pNumElementos > 0) { document.getElementById(pLabelID).style.display = ''; } else { document.getElementById(pLabelID).style.display = 'none'; }
        // 
        if (pNumElementos > 0) {
            document.querySelectorAll('.' + pLabelID).forEach(element => {
                element.style.display = '';
                element.innerHTML = pNumElementos;
            });
        }
        else {            
            document.querySelectorAll('.' + pLabelID).forEach(element => element.style.display = 'none');
        }
    }
}

function DarValorALabelNovedades(pLabelID, pNumElementos) {
    if (document.getElementById(pLabelID) != null) {
        if (pNumElementos > 0) {
            document.getElementById(pLabelID).innerHTML = pNumElementos;
            document.getElementById(pLabelID).style.display = '';
        } else {
            document.getElementById(pLabelID).innerHTML = "";
            document.getElementById(pLabelID).style.display = 'none';
        }
    }
}

function DarValorALabelPendientes(pLabelID, pNumElementos) {
    try {
        if (pNumElementos > 0) {
            document.getElementById(pLabelID).innerHTML = " (" + pNumElementos + ")";
            document.getElementById(pLabelID).style.display = '';
        } else {
            document.getElementById(pLabelID).innerHTML = "";
            document.getElementById(pLabelID).style.display = 'none';
        }
    } catch (Exception) { }
}

function ObtenerNombreCortoUsuRegistroAJAX(pNombre, pApellidos) {
    PeticionesAJAX.ObtenerNombreCortoNuevoUsu(pNombre, pApellidos, ComponerUrlUsuario, ComponerUrlUsuario);
}

function ComponerUrlUsuario(datosRecibidos) {
    repintarUrl(datosRecibidos);
}

function ComprobarCorreoUsuRegistroAJAX(pCorreo, pMetodoJS) {
    PeticionesAJAX.ComprobarExisteCorreoUsuRegistro(pCorreo, pMetodoJS, pMetodoJS);
}

function ComprobarLoginUsuRegistroAJAX(pLogin, pMetodoJS) {
    PeticionesAJAX.ComprobarExisteLoginUsuRegistro(pLogin, pMetodoJS, pMetodoJS);
}


function FuncionObtenerNombreCortoOrgRegistroAJAX(pNombre) {
    PeticionesAJAX.ObtenerNombreCortoNuevaOrg(pNombre, ComponerUrlOrganizacion, ComponerUrlOrganizacion);
}

function ComponerUrlOrganizacion(datosRecibidos) {
    repintarUrl(datosRecibidos);
}

//function FiltrarBandejaMensajesAJAX(pFiltros) {
//    PeticionesAJAX.FiltrarBandejaMensajes(pFiltros, PintarResultados, PintarResultados);
//}

//function PintarResultados(datosRecibidos) {
//    ReceiveServerData(datosRecibidos, '');
//}

function mostrarConfirmacionListado(control, mensaje, accion) {

    var cont = $('#' + control);

    $('.confirmar').css('display', 'none');

    if (cont.children('.confirmar.eliminar').length > 0) {
        var anterior = cont.children('.confirmar.eliminar').eq(0);
        anterior.remove();
    }

    var htmlConfirmar =
'<div class="confirmar eliminar confirmacionMultiple" style="display:block; z-index: 5000;">' +
'<div class="mascara"></div>' +
'<div class="pregunta"><span>' + mensaje + '</span>' +
'<label class="btAzul boton botonancho floatRight right10"><strong><a class="botonConfirmacion">' + borr.si + '</a></strong></label>' +
'<label class="btAzul boton botonancho floatRight right10"><strong><a class="botonConfirmacion">' + borr.no + '</a></strong></label>' +
'</div>' +
'</div>';

    var panConfirmar = $([htmlConfirmar].join(''));

    panConfirmar.prependTo(cont)
.find('a.botonConfirmacion').click(function () { // Ambos botones hacen desaparecer la mascara
    panConfirmar.css('display', 'none');
}).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada
}

function mostrarConfirmacionSencilla(control, mensaje, accion) {
    mostrarConfirmacionSencillaEnPanel(control, mensaje, accion, 'ctl00_CPH1_divPanListado');
}

function mostrarConfirmacionSencillaEnPanel(control, mensaje, accion, panelID) {

    if (control.children('.confirmar').length > 0) {
        var anterior = control.children('.confirmar').eq(0);
        anterior.remove();
    }

    var altura = control.height() + 'px';
    var margin = (control.height() / 3) + 'px';
    //var top = control.position().top + 'px';
    var top = '0px';

    var htmlConfirmar =
'<div class="confirmar confirmacionSencilla" style="height:' + altura + ';top:' + top + ';display:block;z-index:1000">' +
'<div class="mascara"></div>' +
'<div class="pregunta" style="margin-top:' + margin + '"><span>' + mensaje + '</span>' +
'<label class="btAzul boton botonancho floatRight right10"><strong><a class="botonConfirmacion">' + borr.si + '</a></strong></label>' +
'<label class="btAzul boton botonancho floatRight right10"><strong><a class="botonConfirmacion">' + borr.no + '</a></strong></label>' +
'</div>' +
'</div>';

    var panConfirmar = $([htmlConfirmar].join(''));

    panConfirmar.prependTo($('#' + panelID))
.find('a.botonConfirmacion').click(function () { // Ambos botones hacen desaparecer la mascara
    panConfirmar.css('display', 'none');
}).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada
}

function mostrarConfirmacion(control, mensaje, accion) {
    var cont = document.getElementById(control);
    //Compruebo que este elemento no contiene mensajes de confirmación pendientes
    mascaraCancelar(mensaje, cont, accion);
}

function mostrarConfirmacion2(control, mensaje, accion, accion2) {
    var cont = document.getElementById(control);
    //Compruebo que este elemento no contiene mensajes de confirmación pendientes
    mascaraCancelarSiNo(mensaje, cont, accion, accion2);
}

function PrepararCapaModal(capaModal, mascara, seleccionarElementos, args) {
    var height = $(document).height() + 'px';
    var top = $('html').attr('scrollTop') + 'px';
    args = args.replace('$height$', height);
    args = args.replace('$top$', top);
    //console.info(args);
    window.setTimeout(function () {
        eval(args);
    }, 0);
    return false;
}

function MostrarCapaModal(capaModal, mascara, seleccionarElementos) {
    var $capa = $(capaModal);
    var capa = document.getElementById(capaModal);
    var mascara = document.getElementById(mascara);
    mascara.style.height = $(document).height() + 'px';
    var seleccionarElementos = document.getElementById(seleccionarElementos);
    seleccionarElementos.style.top = $('html').attr('scrollTop') + 'px';
    //// || $('body').attr('scrollTop') || 0) + 'px');

    $capa.fadeIn();
    //una vez llamado deberian prepararse los eventos
    $capa.find('a.icoEliminar').unbind('click').click(function () {
        $capa.fadeOut();
    });
    return false;
}

function EvitarEnvioRepetido(arg, elemento) {
    var element = document.getElementById(elemento);
    element.setAttribute("onclick", "return false;");
    window.setTimeout(function () {
        eval(arg);
    }, 0);
}

//desde aqui, evitar multiples postbacks
function Init(sender) {
    prm = Sys.WebForms.PageRequestManager.getInstance();
    //Ensure EnablePartialRendering isn't false which will prevent
    //accessing an instance of the PageRequestManager
    if (prm != null && prm) {
        if (!prm.get_isInAsyncPostBack()) {
            prm.add_initializeRequest(InitRequest);
        }
    }
}

var esperaID;
var timeout;
var continuar = false;
var _sender;
var _args;

function InitRequest(sender, args) {

    //if (prm.get_isInAsyncPostBack() & args.get_postBackElement().id =='btnRefresh') {
    //Could abort current request by using:  prm.abortPostBack();  
    //Cancel most recent request so that previous request will complete
    //and display : args.set_cancel(true);

    if (prm != null && prm.get_isInAsyncPostBack()) {
        //Esperamos a que termine el anterior postback
        clearInterval(esperaID);
        esperaID = setInterval("EsperarPostBack(prm, _sender, _args)", 100);
        //Cancelar el anterior postback si pasan 3 segundos
        _sender = sender;
        _args = args;
        clearTimeout(timeout);
        timeout = setTimeout("CancelarPostBack(prm, _sender, _args)", 3000);
        //args.set_cancel(true);
        //Anula el postback actual, aunque se relanzara cuando el anterior postback acabe o se cancele
        args.set_cancel(true);
    }
}

function EsperarPostBack(prm, sender, args) {
    if (prm == null || !prm.get_isInAsyncPostBack()) {
        clearTimeout(timeout);
        clearInterval(esperaID);
    }
}

function CancelarPostBack(prm, sender, args) {
    clearInterval(esperaID);
    if (prm != null) {
        prm.abortPostBack();
        var uniqueID = args.get_postBackElement().id.replace(/_/g, "$");
        prm._doPostBack(uniqueID, '');
    }
}
//hasta aqui, evitar multiples postbacks

function EndRequest(sender, args) {
    if (window.sweetTitles) { sweetTitles.init(); }

    // Check to see if there's an error on this request.
    //if (cambiosCurriculum){cambiosCurriculum.init();}

    if (args.get_error() != undefined) {
        var err = args.get_error();

        if (err.name != "Sys.WebForms.PageRequestManagerServerErrorException" || (err.httpStatusCode != 0 && err.httpStatusCode != 12030)) { alert(err.message); }
        // Let the framework know that the error is handled, 
        //  so it doesn't throw the JavaScript alert.
        args.set_errorHandled(true);

        EjecutarScriptsIniciales();
    }
}

function RecogerCheckComentarios() {
    var checksMarcados = '';
    //var checks = ObtenerElementosPorClase('checkSelectComent', 'input');
    var checks = $('input.checkSelectComent');
    for (var i = 0; i < checks.length; i++) {
        if ($(checks[i]).is(':checked')) { checksMarcados += checks[i].id.substring(checks[i].id.lastIndexOf('_') + 1) + ','; }
    }
    return checksMarcados;
}

function MarcarTodos(pCheck) {
    //var checks = ObtenerElementosPorClase('checkSelectComent', 'input');
    var checks = $('input.checkSelectComent');
    for (var i = 0; i < checks.length; i++) {
        $(checks[i]).attr('checked', $(pCheck).is(':checked'));
    }
}

function MarcarComentariosLeidos(pIdComent, pNumTotalComent, pContadorID, pMarcarLeidos) {
    var comentsID = pIdComent.split(',');
    var nombreClass = '';
    if (pMarcarLeidos) { nombreClass = 'busquedaDestacada'; }
    var nombreLinkMostrar = 'linkMarcarComentPerfilLeido_'; var nombreLinkOcultar = 'linkMarcarComentPerfilNOLeido_';
    if (pMarcarLeidos) { nombreLinkMostrar = 'linkMarcarComentPerfilNOLeido_'; nombreLinkOcultar = 'linkMarcarComentPerfilLeido_'; }
    for (var i = 0; i < comentsID.length; i++) {
        if (comentsID[i] != '') {
            document.getElementById('liComentario_' + comentsID[i]).className = nombreClass;
            document.getElementById(nombreLinkMostrar + comentsID[i]).style.display = ''; document.getElementById(nombreLinkOcultar + comentsID[i]).style.display = 'none';
            if (pMarcarLeidos) {
                document.getElementById('liComentario_' + comentsID[i]).style.backgroundColor = '#FFFFFF';
            }
            else {
                document.getElementById('liComentario_' + comentsID[i]).style.backgroundColor = '#F5F5F5';
            }
        }
    }
    if (document.getElementById(pContadorID).innerHTML.indexOf('(') != -1) {
        document.getElementById(pContadorID).innerHTML = document.getElementById(pContadorID).innerHTML.replace(document.getElementById(pContadorID).innerHTML.substring(document.getElementById(pContadorID).innerHTML.indexOf('(')), '(' + pNumTotalComent + ')');
    }
    else { document.getElementById(pContadorID).innerHTML += ' (' + pNumTotalComent + ')'; }

    if (pNumTotalComent > 0) { document.getElementById('infoNumComentariosSinLeer').innerHTML = pNumTotalComent; } else { document.getElementById('infoNumComentariosSinLeer').innerHTML = ''; }

    OcultarUpdateProgress();
}

function EstablecerContadorComentNoLeido(pNumTotalComent) {
    DarValorALabel('infNumComentariosSinLeer', pNumTotalComent);
}

function EstablecerContadorMensajesNuevos(pNumTotalComent) {
    DarValorALabel('infoNumMensajes', pNumTotalComent);
    DarValorALabelNovedades('infoNumMensajesNovedades', pNumTotalComent);
}

function DisminuirContadorMensajeNoLeido(pBandejaOrg) {
    try {

        var numMenText = document.getElementById('infNumMensajesSinLeer').innerHTML;
        var numMen = 0;

        if (numMenText.trim() != '') {
            if (numMenText.indexOf('<') != -1) {
                numMenText = numMenText.substring(0, numMenText.indexOf('<'));
            }

            numMen = parseInt(numMenText.trim()) - 1;
        }

        DarValorALabel('infNumMensajesSinLeer', numMen);

        var infBandeja = 'infoNumMensajesSinLeerNovedades';

        if (pBandejaOrg) {
            infBandeja = 'infoNumMensajesSinLeerNovedadesOrg';
        }

        numMenText = document.getElementById(infBandeja).innerHTML;
        numMen = 0;

        if (numMenText.trim() != '') {
            if (numMenText.indexOf('(') != -1) {
                numMenText = numMenText.substring(numMenText.indexOf('(') + 1);
                numMenText = numMenText.substring(0, numMenText.indexOf(')'));
            }

            numMen = parseInt(numMenText.trim()) - 1;
        }

        DarValorALabelPendientes(infBandeja, numMen);
    } catch (Exception) { }
}

function DisminuirContadorInvitacionesNoLeido(pBandejaOrg) {
    var numMenText = document.getElementById('infNumInvitacionesSinLeer').innerHTML;
    var numMen = 0;

    if (numMenText.trim() != '') {
        if (numMenText.indexOf('<') != -1) {
            numMenText = numMenText.substring(0, numMenText.indexOf('<'));
        }

        numMen = parseInt(numMenText.trim()) - 1;
    }

    DarValorALabel('infNumInvitacionesSinLeer', numMen);

    var infBandeja = 'infoNumInvitacionesSinLeerNovedades';

    if (pBandejaOrg) {
        infBandeja = 'infoNumInvitacionesSinLeerNovedadesOrg';
    }

    numMenText = document.getElementById(infBandeja).innerHTML;
    numMen = 0;

    if (numMenText.trim() != '') {
        if (numMenText.indexOf('(') != -1) {
            numMenText = numMenText.substring(numMenText.indexOf('(') + 1);
            numMenText = numMenText.substring(0, numMenText.indexOf(')'));
        }

        numMen = parseInt(numMenText.trim()) - 1;
    }

    DarValorALabelPendientes(infBandeja, numMen);
}

function DisminuirContadorSuscripcionesNoLeido(pNumDisminucion) {
    var numMenText = document.getElementById('infNumSuscripcionesSinLeer').innerHTML;
    var numMen = 0;

    if (numMenText.trim() != '') {
        if (numMenText.indexOf('<') != -1) {
            numMenText = numMenText.substring(0, numMenText.indexOf('<'));
        }

        numMen = parseInt(numMenText.trim()) - pNumDisminucion;
    }

    DarValorALabel('infNumSuscripcionesSinLeer', numMen);

    var infBandeja = 'infoNumSuscripcionesSinLeerNovedades';

    numMenText = document.getElementById(infBandeja).innerHTML;
    numMen = 0;

    if (numMenText.trim() != '') {
        if (numMenText.indexOf('(') != -1) {
            numMenText = numMenText.substring(numMenText.indexOf('(') + 1);
            numMenText = numMenText.substring(0, numMenText.indexOf(')'));
        }

        numMen = parseInt(numMenText.trim()) - pNumDisminucion;
    }

    DarValorALabelPendientes(infBandeja, numMen);
}


function RecogerValorCheckPendientes() {
    var valor = new String($('#chkSoloPedientesLeer').is(':checked'));
    if (document.getElementById('chkSoloComentUsuLeer') != null) {
        valor += ',' + new String($('#chkSoloComentUsuLeer').is(':checked')) + ',' + new String($('#chkSoloComentOrgLeer').is(':checked'));
    }
    return valor;
}

function CallServerSelectorGrupoAmigos(arg, context) {
    window.setTimeout(function () {
        eval(arg);
    }, 0);
}

function DarFormato(arg, context) {
    var element = document.getElementById(arg);
    var parametros = context.split("&");
    var accion = parametros[0];
    if (accion == 'negrita') {
        Envolver("'''", "'''", element);
    }
    else if (accion == 'cursiva') {
        Envolver("''", "''", element);
    }
    else if (accion == 'enlaceInterno') {
        Envolver("[[", "]]", element);
    }
    else if (accion == 'enlaceExterno') {
        Envolver("[", "]", element);
    }
    else if (accion == 'titulo1') {
        //Envolver("\\n==","==\\n", element);
        Envolver("==", "==", element);
    }
    else if (accion == 'titulo2') {
        //Envolver("\\n===","===\\n", element);
        Envolver("===", "===", element);
    }
    else if (accion == 'titulo3') {
        //Envolver("\\n====","====\\n", element);
        Envolver("====", "====", element);
    }
    else if (accion == 'imagen') {
        var imagen = document.getElementById(parametros[1]);
        Envolver("[[Imagen:" + imagen.value, "]]", element);
        imagen.value = "";
    }
}

function Envolver(tagOpen, tagClose, element) {
    var txtarea = element;
    var selText, isSample = false;
    if (document.selection && document.selection.createRange) { // IE/Opera            ////save window scroll position
        ////if (document.documentElement && document.documentElement.scrollTop)
        ////    var winScroll = document.documentElement.scrollTop
        ////else if (document.body)
        ////    var winScroll = document.body.scrollTop;

        //get current selection
        txtarea.focus();
        var range = document.selection.createRange();
        selText = range.text;
        //////insert tags
        ////checkSelectedText();
        range.text = tagOpen + selText + tagClose;
        //alert(range.text);
        //////mark sample text as selected
        ////if (isSample && range.moveStart) {
        ////    if (window.opera)
        ////        tagClose = tagClose.replace(/\n/g,'');
        ////    range.moveStart('character', - tagClose.length - selText.length); 
        ////    range.moveEnd('character', - tagClose.length); 
        ////}
        //range.select();
        //////restore window scroll position
        ////if (document.documentElement && document.documentElement.scrollTop)
        ////    document.documentElement.scrollTop = winScroll
        ////else if (document.body)
        ////    document.body.scrollTop = winScroll;
    }
    else if (txtarea.selectionStart || txtarea.selectionStart == '0') {// Mozilla
        //save textarea scroll position
        var textScroll = txtarea.scrollTop;
        //get current selection
        txtarea.focus();
        var startPos = txtarea.selectionStart;
        var endPos = txtarea.selectionEnd;
        selText = txtarea.value.substring(startPos, endPos);
        //insert tags
        //checkSelectedText();
        txtarea.value = txtarea.value.substring(0, startPos)
+ tagOpen + selText + tagClose
+ txtarea.value.substring(endPos, txtarea.value.length);
        //set new selection
        if (isSample) {
            //txtarea.selectionStart = startPos + tagOpen.length;
            //txtarea.selectionEnd = startPos + tagOpen.length + selText.length;
        } else {
            //txtarea.selectionStart = startPos + tagOpen.length + selText.length + tagClose.length;
            //txtarea.selectionEnd = txtarea.selectionStart;
        }
        //restore textarea scroll position
        txtarea.scrollTop = textScroll;
    }
}

function CallServerListadoBlogs(arg, context) {
    window.setTimeout(function () {
        eval(arg);
    }, 0);
}

function CallServerSelectorCategorias(arg, id, context) {
    window.setTimeout(function () {
        //eval(arg);
        WebForm_DoCallback(id, context, ReceiveServerData, '', null, false)
    }, 0);
}

function CallServerSelectorCategoriasBlog(arg, context) {
    window.setTimeout(function () {
        eval(arg);
    }, 0);
}

function CallServerSelectorEditoresBlog(arg, context) {
    window.setTimeout(function () {
        eval(arg);
    }, 0);
}

function addTag(evento, control) {
    var contenedor = $('#contenedorFiltros');
    var $this = $(control).parents('a');
    var texto = $this.text().replace(/\n/g, '').replace(/^\s*|\s*$/g, '');
    if (contenedor.parents('div.panel').is(':hidden')) {
        contenedor.parents('div.panel').show('blind', { direction: 'vertical' }, 600, function () {
            $this.effect('transfer', { to: objetivo }, 400);
        }).prevAll().find('a.desplegable').eq(0).addClass('activo');
    }
    var tagsDentro = contenedor.find('a');
    var estaDentro = false;
    for (i = 0; i < tagsDentro.length; i++) {
        if (tagsDentro[i].innerHTML == texto) { estaDentro = true; }
    }
    if (!estaDentro) {
        var objetivo = $(['<a id="idTemp">', texto, '</a><input type="hidden" name="filtros" value="', $this.text(), '" />'].join('')).appendTo(contenedor);
        contenedor.parent().andSelf().css('display', 'block');
        $this.effect('transfer', { to: objetivo }, 400);
        objetivo.click(function () {
            $(this).remove();
            eliminarTagsBusqueda(texto); return false;
            if (!contenedor.find('a').length) { contenedor.parent().andSelf().hide(); }
            return false;
        });
    }
    return false;
}

function CallServerSelectorCategorias(arg, id, context) {
    window.setTimeout(function () {
        //eval(arg);
        WebForm_DoCallback(id, context, ReceiveServerData, '', null, false)
    }, 0);
}

function marcarElementos(pCheck, pClave, pIdTxt) {
    var txtOrgSel = document.getElementById(pIdTxt);
    if ($(pCheck).is(':checked')) {
        txtOrgSel.value = txtOrgSel.value + pClave + '|';
    }
    else {
        txtOrgSel.value = txtOrgSel.value.replace(pClave + '|', '');
    }
}

function CallServerSelectorPersonas(arg, context) {
    window.setTimeout(function () {
        eval(arg);
    }, 0);
}

function CallServerSelectorRolUsuario(arg, context) {
    window.setTimeout(function () {
        eval(arg);
    }, 0);
}

var votar = null
function Votar(arg, elemento) {
    var element = document.getElementById(elemento);
    //console.info(element.value);
    votar = window.setTimeout(function () {
        CallServer(arg + "&" + element.value);
    }, 0);
}

var cambiarPagina = null;
function CambiarPagina(arg, context) {
    var pagina = document.getElementById('pagina');
    if (pagina != null) {
        var params = arg.split('&'); pagina.value = params[2];
    }
    var updateProgress = document.getElementById('ctl00_CPH1_UpdateProgress1');
    if (updateProgress != null) {
        updateProgress.style.display = "block";
    }
    //cambiarPagina = window.setTimeout(function() {

    CallServer(arg, context);
    //}, 0);
}


/* Funciones Desplegables */

function DesplegarAccion(pBoton, pPanelID, pPanelName, pCallBackPage, pArg) {
    var panel = document.getElementById(pPanelID);

    panel.children[0].children[0].style.display = 'block';
    panel.children[0].children[1].style.display = 'none';
    panel.children[0].children[2].style.display = 'none';
    panel.children[0].children[3].style.display = 'none';

    panel.children[0].style.display = 'block';
    panel.style.display = '';

    if (pBoton != null && typeof (pBoton.parentNode) != 'undefined') {
        DesactivarBotonesActivosDespl();
        pBoton.parentNode.className += ' active';
    }

    var target = '__Page';

    if (!pCallBackPage) {
        //target = pPanelID.replace(/\_/g,'$');
        target = pPanelName;
    }

    WebForm_DoCallback(target, 'CargarControlDesplegar' + pArg, ReceiveServerData, "", null, false);
}

function MostrarPanelAccionDesp(pPanelID, pHtml) {
    MostrarPanelAccionDesp(pPanelID, pHtml, false)
}

function MostrarPanelAccionDesp(pPanelID, pHtml, pPintarCargando) {
    MostrarPanelAccionDesp(pPaneID, pHtml, pPintarCargando, false);
}

function MostrarPanelAccionDesp(pPanelID, pHtml, pPintarCargando, pSoloAux) {
    var panel = document.getElementById(pPanelID);


    if (pHtml != null) {
        var html = pHtml.replace(/\j001j/g, '"').replace(/\j002j/g, '\'');
        panel.children[0].children[2].innerHTML = html;
    }
    if (!pSoloAux) {

        panel.children[0].children[0].style.display = 'none';
        panel.children[0].children[1].style.display = 'none';
        panel.children[0].children[2].style.display = 'block';
        panel.children[0].children[3].style.display = 'none';
        //panel.children[0].children[4].style.display = 'block';


        panel.children[0].style.display = 'block';
        panel.style.display = '';

    }

    else {
        panel.children[0].children[0].style.display = 'none';
        panel.children[0].children[1].style.display = 'none';
        panel.children[0].children[2].style.display = 'none';
        panel.children[0].children[3].style.display = 'block';
        panel.children[0].children[4].style.display = 'block';


        panel.children[0].style.display = 'block';
        panel.style.display = '';

    }
    if (pPintarCargando) {
        var htmlCargando = '<img id="ctl00_ctl00_controles_master_controlcargando_ascx_imgEspera" src="http://static.gnoss.net/img/espera.gif"><h3> Cargando...</h3>';
        panel.children[0].children[3].innerHTML = htmlCargando;
    }
}

function AceptarPanelAccion(pPanelID, pOk, pHtml) {
    var panel = document.getElementById(pPanelID);

    if (pHtml != null && pHtml != '' && pHtml.indexOf('<p') != 0) {
        pHtml = '<p>' + pHtml + '</p>';
    }

    if (pOk) {
        panel.children[0].children[1].children[0].style.display = 'block';
        panel.children[0].children[1].children[1].style.display = 'none';

        panel.children[0].children[1].children[0].innerHTML = pHtml;

        // Solo si ha ido bien
        panel.children[0].children[0].style.display = 'none';
        panel.children[0].children[2].style.display = 'none';
        panel.children[0].children[3].style.display = 'none';
    }
    else {
        panel.children[0].children[1].children[0].style.display = 'none';
        panel.children[0].children[1].children[1].style.display = 'block';

        panel.children[0].children[1].children[1].innerHTML = pHtml;
    }

    panel.children[0].children[1].style.display = 'block';
    panel.children[0].children[4].style.display = 'block';

    panel.children[0].style.display = 'block';
    panel.style.display = '';

    DesactivarBotonesActivosDespl();
}

function CerrarPanelAccion(pPanelID) {
    var panel = document.getElementById(pPanelID);
    panel.children[0].style.display = 'none';
    panel.style.display = 'none';

    DesactivarBotonesActivosDespl();
}

function DesactivarBotonesActivosDespl() {
    var btnActivos = $('.active');
    for (var i = 0; i < btnActivos.length; i++) {
        btnActivos[i].className = btnActivos[i].className.replace('active', '');
    }
}

/* Fin Funciones Desplegables */

var diffHoras = null;

/* Fechas actividad reciente */
function MontarFechas() {
    if (typeof (diffHoras) == 'undefined' || diffHoras == null || isNaN(diffHoras)) {
        var fechaServidor = new Date($('#inpt_serverTime').val());
        var fechaCliente = new Date();
        var diffHoras = parseInt((fechaServidor.getTime() / (1000 * 60 * 60)) - (fechaCliente.getTime() / (1000 * 60 * 60)));
    }

    $('.resource .fechaLive').each(function () {
        var enlace = $(this);
        enlace.removeClass("fechaLive");
        var fecha = enlace[0].innerHTML;
        var fechaRecurso = new Date(fecha);
        fechaRecurso.setHours(fechaRecurso.getHours() - diffHoras);
        DifFechasEvento(fechaRecurso.format("yyyy/MM/dd HH:mm"), enlace);
    });
}

function DifFechasEvento(fecha, contenedor) {
    var factual = new Date();

    var finicio = new Date(fecha);
    var dateDifDay = parseInt((factual.getTime() / (1000 * 60 * 60 * 24)) - (finicio.getTime() / (1000 * 60 * 60 * 24)));
    var difD = dateDifDay;
    if (dateDifDay < 7 && dateDifDay > 0) {
        var diaInicio = finicio.getDay();
        var diaActual = factual.getDay();
		if(diaInicio >= diaActual)
		{
			diaActual = diaActual + 7;
        }
        var difD = diaActual - diaInicio;
    }
    var difH = parseInt((factual.getTime() / (1000 * 60 * 60)) - (finicio.getTime() / (1000 * 60 * 60)));
    var difM = parseInt((factual.getTime() / (1000 * 60)) - (finicio.getTime() / (1000 * 60)));
    //Montamos la frase para el tiempo pasado
    var tiempoPasado = '';
    if (difD < 7) {
        if (difD == 0) {
            if (difH == 0) {
                if (difM <= 1) {
                    tiempoPasado = tiempo.hace + ' 1 ' + tiempo.minuto;
                }
                else {
                    tiempoPasado = tiempo.hace + ' ' + difM + ' ' + tiempo.minutos;
                }
            }
            else if (difH == 1) {
                tiempoPasado = tiempo.hace + ' 1 ' + tiempo.hora;
            }
            else {
                tiempoPasado = tiempo.hace + ' ' + difH + ' ' + tiempo.horas;
            }
        }
        else if (difD == 1) {
            tiempoPasado = tiempo.ayer;
        }
        else {
            tiempoPasado = tiempo.hace + ' ' + difD + ' ' + tiempo.dias;
        }
    }
    else {
        var dia = finicio.getDate();
        if (dia < 10) { dia = '0' + dia; }
        var mes = finicio.getMonth() + 1;
        if (mes < 10) { mes = '0' + mes; }

        //var fecha = dia + '/' + mes + '/' + finicio.getFullYear();
        var fechaPintado = tiempo.fechaBarras.replace('@1@', dia).replace('@2@', mes).replace('@3@', finicio.getFullYear());
        tiempoPasado = tiempo.eldia + ' ' + fechaPintado;
    }
    contenedor.html(tiempoPasado);
}

/* Fin fechas actividad reciente */

/* Enganchamos el evento click cuando es necesario, sabemos que en IE y las ultimas versiones de ff(16) y chrome(22) funciona bien*/
var is_ie = navigator.userAgent.indexOf("MSIE") > -1;
var is_chrome_nuevo = false;
if (navigator.userAgent.indexOf("Chrome/") > -1) {
    var nav = navigator.userAgent.substring(navigator.userAgent.indexOf("Chrome/") + 7);
    if (nav.indexOf(' ') > -1) {
        nav = nav.substring(0, nav.indexOf(' '));
    }
    if (nav.indexOf('.') > -1) {
        nav = nav.substring(0, nav.indexOf('.'));
    }
    if (parseInt(nav) > 21) {
        is_chrome_nuevo = true;
    }
}
var is_firefox_nuevo = false;
if (navigator.userAgent.indexOf("Firefox/") > -1) {
    var nav = navigator.userAgent.substring(navigator.userAgent.indexOf("Firefox/") + 8);
    if (nav.indexOf(' ') > -1) {
        nav = nav.substring(0, nav.indexOf(' '));
    }
    if (nav.indexOf('.') > -1) {
        nav = nav.substring(0, nav.indexOf('.'));
    }
    if (parseInt(nav) > 15) {
        is_firefox_nuevo = true;
    }
}

if (!is_ie && !is_chrome_nuevo && !is_firefox_nuevo) {
    HTMLElement.prototype.click = function () {
        var evt = this.ownerDocument.createEvent('MouseEvents');
        evt.initMouseEvent('click', true, true, this.ownerDocument.defaultView, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
        this.dispatchEvent(evt);
    }
}

/* Fin Enganchamos el evento click cuando es necesario*/

function AnyadirGrupoAContacto(contactoId) {
    WebForm_DoCallback('__Page', 'AgregarGrupoAContacto&' + contactoId + '&', ReceiveServerData, '', null, false);

}

function ConectarConUsuario(pIdentidad, pNombre) {
    var panelResul = 'divResultadosConRec';

    if ($('#' + panelResul).length == 0) {
        panelResul = 'divResultados';
    }

    var numSeleccioados = 0;
    var args = 'ConectarConUsuario&' + pIdentidad;
    mostrarConfirmacionSencillaEnPanel($('#divEtiquetas' + pIdentidad).parent().parent(), invitaciones.invitarContactoAceptar.replace('@1@', pNombre).replace('@2@', pNombre), function () {
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', args, ReceiveServerData, '', null, false);
    }, panelResul);
}

function IgnorarUsuario(pIdentidad, pNombre) {
    var panelResul = 'divResultadosConRec';

    if ($('#' + panelResul).length == 0) {
        panelResul = 'divResultados';
    }

    var numSeleccioados = 0;
    var args = 'IgnorarUsuario&' + pIdentidad;
    mostrarConfirmacionSencillaEnPanel($('#divEtiquetas' + pIdentidad).parent().parent(), invitaciones.ingnorarContactoAceptar.replace('@1@', pNombre), function () {
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', args, ReceiveServerData, '', null, false);
    }, panelResul);
}


function IgnorarProyecto(pProyecto, pNombre) {
    var panelResul = 'divResultadosComRec';

    if ($('#' + panelResul).length == 0) {
        panelResul = 'divResultados';
    }

    var numSeleccioados = 0;
    var args = 'IgnorarProyecto&' + pProyecto;
    mostrarConfirmacionSencillaEnPanel($('#divEtiquetas' + pProyecto).parent().parent(), invitaciones.ingnorarProyAceptar.replace('@1@', pNombre), function () {
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', args, ReceiveServerData, '', null, false);
    }, panelResul);
}

function HacerseMiembroProy(pProyecto, pNombre) {
    var panelResul = 'divResultadosComRec';

    if ($('#' + panelResul).length == 0) {
        panelResul = 'divResultados';
    }

    var numSeleccioados = 0;
    var args = 'HacerseMiembroProy&' + pProyecto;
    mostrarConfirmacionSencillaEnPanel($('#divEtiquetas' + pProyecto).parent().parent(), invitaciones.haztemiembroProyAceptar.replace('@1@', pNombre), function () {
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', args, ReceiveServerData, '', null, false);
    }, panelResul);
}

function SolicitarAccesoProy(pProyecto, pNombre) {
    var panelResul = 'divResultadosComRec';

    if ($('#' + panelResul).length == 0) {
        panelResul = 'divResultados';
    }

    var numSeleccioados = 0;
    var args = 'SolicitarAccesoProy&' + pProyecto;
    mostrarConfirmacionSencillaEnPanel($('#divEtiquetas' + pProyecto).parent().parent(), invitaciones.solicitaraccesoProyAceptar.replace('@1@', pNombre), function () {
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', args, ReceiveServerData, '', null, false);
    }, panelResul);
}

/* Seleccionar & eliminar Autocompletar*/
function seleccionarAutocompletar(nombre, identidad, PanContenedorID, txtHackID, ContenedorMostrarID, txtFiltroID) {
    document.getElementById(txtFiltroID).value = '';
    $('#selector').css('display', 'none');
    contenedor = document.getElementById(PanContenedorID);
    if (contenedor.innerHTML.trim().indexOf('<ul') == 0) { contenedor.innerHTML = contenedor.innerHTML.replace('<ul class=\"icoEliminar\">', '').replace('</ul>', ''); }
    contenedor.innerHTML = '<ul class=\"icoEliminar\">' + contenedor.innerHTML + '<li>' + nombre + '<a class="remove" onclick="javascript:eliminarAutocompletar(this,\'' + identidad + '\',\'' + PanContenedorID + '\',\'' + txtHackID + '\',\'' + ContenedorMostrarID + '\');">' + textoRecursos.Eliminar + '</a></li>' + '</ul>';

    document.getElementById(txtHackID).value = document.getElementById(txtHackID).value + "&" + identidad;
    if (ContenedorMostrarID != null) {
        $('#' + ContenedorMostrarID + '').css('display', '');
    }
}

function eliminarAutocompletar(nombre, identidad, PanContenedorID, txtHackID, ContenedorMostrarID) {
    contenedor = document.getElementById(PanContenedorID);
    contenedor.children[0].removeChild(nombre.parentNode);
    document.getElementById(txtHackID).value = document.getElementById(txtHackID).value.replace('&' + identidad, '');
    if (ContenedorMostrarID != null && document.getElementById(txtHackID).value == '') {
        $('#' + ContenedorMostrarID + '').css('display', 'none');
    }
}

function ObtenerEntidadesLOD(pUrlServicio, pUrlBaseEnlaceTag, pDocumentoID, pEtiquetas, pIdioma) {
    var servicio = new WS(pUrlServicio, WSDataType.jsonp);

    var metodo = 'ObtenerEntidadesLOD';
    var params = {};
    params['documentoID'] = pDocumentoID;
    params['tags'] = urlEncode(pEtiquetas);
    params['urlBaseEnlaceTag'] = pUrlBaseEnlaceTag;
    params['idioma'] = pIdioma;
    servicio.call(metodo, params, function (data) {
        $('.listTags').find('a').each(function () {
            var tag = this.textContent;
            if (data[tag] != null) {
                $(this).parent().attr('title', data[tag]);
                $(this).parent().attr('class', 'conFbTt');
            }
        });
        $(".conFbTt").each(function () {
            if (this.title) {
                this.tooltipData = this.title;
                this.removeAttribute('title');
            }
        }).hover(mostrarFreebaseTt, ocultarFreebaseTt).mousemove(posicionarFreebaseTt);
    });
}


/*                                                             Tooltips para freebase (conFbTt)
*---------------------------------------------------------------------------------------------
*/

var necesarioPosicionar = true;
var mouseOnTooltip = false;
var tooltipActivo = '';
var cerrar = 0;

var posicionarFreebaseTt = function (event) {
    if (necesarioPosicionar && $("div.tooltip").length > 0) {
        var tPosX = event.pageX - 10;
        var tPosY = event.pageY - 17 - ($("div.tooltip").height() || 0);

        var navegador = navigator.appName;
        var anchoVentana = window.innerWidth;
        var altoScroll = window.pageYOffset;

        if (navegador == "Microsoft Internet Explorer") {
            anchoVentana = window.document.body.clientWidth;
            altoScroll = document.documentElement.scrollTop;
        }

        var sumaX = tPosX + $("div.tooltip").width() + 30;
        if (sumaX > anchoVentana) {
            tPosX = anchoVentana - $("div.tooltip").width() - 30;
        }

        if (tPosY < altoScroll) {
            tPosY = event.pageY + 12
        }

        $("div.tooltip").css({
            top: tPosY,
            left: tPosX
        });
        necesarioPosicionar = false;
    }
}

var mostrarFreebaseTt = function (event) {
    var hayTooltip = $("div.tooltip").length != 0;
    var tooltipDiferente = false;

    if (hayTooltip && tooltipActivo != '' && $(this).hasClass('conFbTt')) {
        tooltipDiferente = ($(this).text() != tooltipActivo);
    }

    if (!hayTooltip || tooltipDiferente) {
        $("div.tooltip").remove();
        var textoTt = (this.tooltipData) ? this.tooltipData : $(this).text();
        tooltipActivo = $(this).text();
        $("<div class='tooltip entidadesEnlazadas' style='display:none; width:350px; height:auto;padding:0; opacity:1;' onmousemove='javascript:mouseSobreTooltip()' onmouseover='javascript:mouseSobreTooltip()' onmouseout='javascript:mouseFueraTooltip()'><div class='relatedInfoWindow'><p class='poweredby'>Powered by <a href='http://www.gnoss.com'><strong>Gnoss</strong></a></p><div class='wrapRelatedInfoWindow'>" + textoTt + "</div> <p><em>" + $('input.inpt_avisoLegal').val() + "</em></p></div></div>")
.appendTo("body")
.fadeIn();

        $("div.tooltip").hover(mostrarFreebaseTt, ocultarFreebaseTt).mousemove(posicionarFreebaseTt);
        if (tooltipDiferente) {
            necesarioPosicionar = true;
        }
        posicionarFreebaseTt(event);
    }
    cerrar++;
}

var ocultarFreebaseTt = function () {
    setTimeout(quitarFreebaseTt, 1000);
}

function quitarFreebaseTt() {
    cerrar--;
    if ((cerrar <= 0) && (!mouseOnTooltip)) {
        $("div.tooltip").remove();
        necesarioPosicionar = true;
    }
}

function mouseFueraTooltip() {
    mouseOnTooltip = false;
    if (cerrar <= 0) {
        setTimeout(quitarFreebaseTt, 1000);
    }
}

function mouseSobreTooltip() {
    mouseOnTooltip = true;
}

/*
*---------------------------------------------------------------------------------------------
*/

function ObtenerParametrosProyOrigen(pPestanya) {
    var pestanyas = hackBusquedaPestProyOrigen.split('[||]');

    if (pestanyas.length > 0) {
        for (var i = 0; i < pestanyas.length; i++) {
            if (pestanyas[i].split('[|]')[0] == pPestanya) {
                return pestanyas[i].split('[|]')[1];
            }
        }
    }

    return '';
}

function OnUploadCompleted(pRutaImg) {
    urlCKEImg.val(pRutaImg/*.replace('https://', 'http://')*/);
    botonCKEAceptar.click();
}

function ElemVisible(pClase) {
    var elems = $(pClase);
    for (var i = 0; i < elems.length; i++) {
        if ($(elems[i]).is(':visible')) {
            return elems[i];
        }
    }
}


function DesplegarAccionListado(pControl, pArg, pDocumentoID, pProyectoID) {
    //Limpio los paneles anteriores:
    $('.divContAccList').html('');

    ContenedorDesplBusqueda = $('.divContAccList', $(pControl).parent().parent().parent().parent())[0];
    $(ContenedorDesplBusqueda).html($('#divContDespBusq').html());

    var docIDArg = '&docBusqID=' + pDocumentoID + '&';
    if (pProyectoID != undefined && pProyectoID != '') {
        var proyIDArg = '&proyID=' + pProyectoID + '&';
    }

    DesplegarAccion(pControl, ClientDesplegarID, UniqueDesplegarID, false, docIDArg + proyIDArg + pArg);
}

function IntercambiarOnclickTag(pControl, pTexto1, pTexto2) {
    var clase = pControl.className;
    var controles = $('.' + clase);

    for (var i = 0; i < controles.length; i++) {
        $(controles[i]).html($(controles[i]).html().replace(pTexto1, pTexto2));

        var tag = controles[i].attributes['tag'].value;
        controles[i].attributes['tag'].value = controles[i].attributes['onclick'].value;
        controles[i].attributes['onclick'].value = tag;
    }
}



function AumentarNumElemento(pElemento) {
    try {
        if (pElemento.length > 0) {
            var texto = pElemento.html();

            if (texto != '') {
                if (texto.indexOf('+') != -1) {
                    texto = texto.replace('+', '');
                    var num = parseInt(texto) + 1;
                    pElemento.html(' + ' + num)
                }
                else if (texto.indexOf('-') != -1) {
                    texto = texto.replace('-', '');
                    var num = parseInt(texto) - 1;

                    if (num == 0) {
                        pElemento.html(num)
                    }
                    else {
                        pElemento.html(' - ' + num)
                    }
                }
                else {
                    var num = parseInt(texto) + 1;
                    pElemento.html(' + ' + num)
                }
            }
        }
    }
    catch (ex) { }
}

function EnviarAccGogAnac(pCat, pTipo, pLabel) {
    try {
        if (typeof pageTracker != 'undefined') {
            pageTracker._trackEvent(pCat, pTipo, pLabel);
        }
    } catch (ex) {
    }
}


var arriba;
function SubirPagina() {
    if (document.body.scrollTop != 0 || document.documentElement.scrollTop != 0) {
        window.scrollBy(0, -50);
        arriba = setTimeout('SubirPagina()', 5);
    }
    else clearTimeout(arriba);
}

function MostrarPopUpCentrado(url,width,height)
{
    var left = (screen.width - width) / 2;
    var top = (screen.height - height) / 2;
    var windowProperties = 'height=' + height + ',width=' + width + ',top=' + top + ',left=' + left + ',scrollbars=NO,resizable=NO,menubar=NO,toolbar=NO,location=NO,statusbar=NO,fullscreen=NO';
    window.open(url, '', windowProperties);
}

/****** CHAT **********/

function ActivarChat(perfil, activar, nombre) {
    if ($('#divchatframe').length == 0) {
        var panel = document.createElement("div");
        //$(panel).css('left', '5px');
        //$(panel).css('position', 'fixed');
        //$(panel).css('width', '325px');
        //$(panel).css('padding', '2px');
        $(panel).attr('id', 'divchatframe');
        $(panel).attr('class', 'chat');
        $(panel).css('height', '20px');

        var panelInt = document.createElement("div");
        $(panelInt).attr('id', 'divChat');
        $(panelInt).css('display', 'none');
        $(panelInt).css('height', '50px');
        panel.appendChild(panelInt);

        var panelAlerta = document.createElement("div");
        $(panelAlerta).attr('id', 'divAlertaChat');
        $(panelAlerta).css('display', 'none');
        panel.appendChild(panelAlerta);


        var panelCerrar = document.createElement("div");
        $(panelCerrar).attr('class', 'menuChat');
        panel.appendChild(panelCerrar);

        var aCerrar = document.createElement("a");
        panelCerrar.appendChild(aCerrar);
        $(aCerrar).attr('onclick', 'desactivarUsuChat();');
        $(aCerrar).html(textChat.desconectar);
        $(aCerrar).attr('id', 'hlCerrChat');

        var aMaximizar = document.createElement("a");
        panelCerrar.appendChild(aMaximizar);
        $(aMaximizar).attr('onclick', 'ExpandirChat(' + activar + ');');
        $(aMaximizar).html(textChat.expandir);
        $(aMaximizar).attr('id', 'hlMaxChat');

        if (!activar) {
            $(aMaximizar).html('Conectar');
            $(aCerrar).css('display', 'none');
        }

        var aMinChat = document.createElement("a");
        panelCerrar.appendChild(aMinChat);
        $(aMinChat).attr('onclick', 'ContraerChat();');
        $(aMinChat).html(textChat.contraer);
        $(aMinChat).attr('id', 'hlMinChat');
        $(aMinChat).css('display', 'none');

        var inputPerfil = document.createElement("input");
        $(inputPerfil).css('display', 'none');
        $(inputPerfil).attr('type', 'text');
        $(inputPerfil).attr('id', 'txtHackPerfilActual');
        $(inputPerfil).val(perfil);
        panelCerrar.appendChild(inputPerfil);

        var inputNomAct = document.createElement("input");
        $(inputNomAct).css('display', 'none');
        $(inputNomAct).attr('type', 'text');
        $(inputNomAct).attr('id', 'txtHackNombreActual');
        panelCerrar.appendChild(inputNomAct);

        document.body.appendChild(panel);
        $('#txtHackNombreActual').val(nombre);
        ColocarChat();

        if (activar) {
            MontarChat();
        }
    }
    else {
        $('#divchatframe').css('display', '');
    }
}

function ColocarChat() {
    $('#divchatframe').css('top', ($(window).height() - $('#divchatframe').outerHeight() - 20));
}

function ExpandirChat(activado) {
    $('#divChat').css('height', '325px');
    $('#divchatframe').css('height', '345px');
    $('#divChat').css('display', '');
    $('#hlMaxChat').css('display', 'none');
    $('#hlMinChat').css('display', '');
    ColocarChat();

    if (!activado) {
        MontarChat();
        $('#hlMaxChat').attr('onclick', 'ExpandirChat(true);');
        $('#hlMaxChat').html('Expandir');
        $('#hlCerrChat').css('display', '');
    }
}

function ContraerChat() {
    $('#divChat').css('height', '50px');
    $('#divchatframe').css('height', '20px');
    $('#divChat').css('display', 'none');
    $('#hlMaxChat').css('display', '');
    $('#hlMinChat').css('display', 'none');
    ColocarChat();
    MostrarChatsActivos(true);
}

function MostrarCargandoChat() {
    $('#divChatAccion').html('<div class=\"menuChatInt\">...</div><div class="chatCargando"><span>' + textChat.cargando + '</span></div>');
    MostrarChatsActivos(false);
}

function MontarChat() {
    try {
        $('#divChat').html('<div id="divChatsActivos"></div><div id="divChatAccion"></div>');
        MostrarCargandoChat();

        wanachat = $.connection.myChatHub;

        //$('#txtHackUsu').val('');

        wanachat.client.addMessage = function (message, remitente, idPerfilRemitenteID, chatID, mensID) {
            if (idPerfilRemitenteID == null || idPerfilRemitenteID == $('#txtHackUsu').val()) {
                if (idPerfilRemitenteID != null) {
                    PintarMensaje(remitente, message, 1, mensID);
                    MarcarNumMenChat(chatID, 0);
                    wanachat.server.chatLeido($('#txtHackPerfilActual').val(), chatID);
                }
                else {
                    PintarMensaje(remitente, message, 2, mensID);
                }
            }
            else {
                MarcarNumMenChat(chatID, 1);
                MoverChatPrimero(chatID);

                //$('#txtHackMensajes').val($('#txtHackMensajes').val() + idPerfilRemitenteID + '|' + remitente + '|' + message + '|||');
                var alerta = message;

                if (alerta.length > 30) {
                    alerta = alerta.substring(0, 27) + '...';
                }

                MontarAlerta(remitente + ': ' + alerta, idPerfilRemitenteID, remitente, chatID, 0);
            }
        };

        wanachat.client.addHtml = function (divID, html, js) {
            if (html.indexOf('[|||]') != -1) {
                var varIdiomas = html.substring(0, html.indexOf('[|||]'));
                html = html.substring(html.indexOf('[|||]') + 5);
                var variables = varIdiomas.split('|||');

                for (var i = 0; i < variables.length; i++) {
                    html = html.replace('@' + variables[i] + '@', eval(variables[i]));
                }
            }

            $('#' + divID).html(html);

            if (js != '') {
                eval(js);
            }
        };

        wanachat.client.addMasMensajes = function (htmlNuevo) {
            if (htmlNuevo.indexOf('[|||]') != -1) {
                var varIdiomas = htmlNuevo.substring(0, htmlNuevo.indexOf('[|||]'));
                htmlNuevo = htmlNuevo.substring(htmlNuevo.indexOf('[|||]') + 5);
                var variables = varIdiomas.split('|||');

                for (var i = 0; i < variables.length; i++) {
                    htmlNuevo = htmlNuevo.replace('@' + variables[i] + '@', eval(variables[i]));
                }
            }

            var html = $('ul#listMessagesChat').html();
            html = html.substring(html.indexOf('</li>') + 5);
            html = htmlNuevo + html;
            $('ul#listMessagesChat').html(html);
        };

        wanachat.client.addChat = function (htmlNuevo) {
            $('#divChatsActivos ul').html(htmlNuevo + $('#divChatsActivos ul').html());
        };

        wanachat.client.addError = function (error) {
            MontarAlerta(error, null, null, null, 1);
        };

        $.connection.hub.start().done(function () { registrarUsuChat('0') }).fail(function () { MontarAlerta('registrar', null, null, null, 1); });
    }
    catch (err) { }
}

function PintarMensaje(remitente, message, tipo, mensID) {
    var mensaje = '<span class="mensRemi">' + remitente + ':</span><span class="mensChat">' + message + '</span><span class="mensFecha">' + ObtenerFecha() + '</span>';
    if (tipo == 0) {
        $('#listMessagesChat').append('<li id="liMen_' + mensID + '" class="ownerMessage mensAccionEnviando">' + mensaje + '<span class="mensAccionEnviando">' + textChat.enviando + '...</span></li>');
    }
    else if (tipo == 1) {
        $('#listMessagesChat').append('<li id="liMen_' + mensID + '" class="contactMessage">' + mensaje + '</li>');
    }
    else if (tipo == 2) {
        $('#liMen_' + mensID).html(mensaje + '<span class="mensAccionEnviado">' + textChat.enviado + '</span>');
        $('#liMen_' + mensID).attr('class', 'ownerMessage mensAccionEnviado');
    }

    DesplazarScrollMens();
}

function ObtenerFecha() {
    var f = new Date();
    return (f.getHours() + ":" + (f.getMinutes()));
}

function DesplazarScrollMens() {
    $('.divListMessages')[0].scrollTop = $('.divListMessages')[0].scrollHeight;
    $('#listMessagesChat')[0].scrollTop = $('#listMessagesChat')[0].scrollHeight;
}

function MarcarNumMenChat(chatID, num) {
    if ($('#divChatsActivos li#liChat_' + chatID + ' span.chatNoRead').length > 0) {
        var numText = '';

        if (num > 0) {
            var antNunText = $('#divChatsActivos li#liChat_' + chatID + ' span.chatNoRead').html()
            var antNun = 0;

            if (antNunText != '') {
                antNun = parseInt(antNunText.trim());
                num += antNun;
            }

            numText = num;
        }

        $('#divChatsActivos li#liChat_' + chatID + ' span.chatNoRead').html(numText);
    }
}

function MoverChatPrimero(chatID) {
    var chats = $('#divChatsActivos li#liChat_' + chatID);

    if (chats.length > 0) {
        var ul = chats.parent();
        var li = chats[0];
        chats.remove();
        ul.prepend(li);
    }
    else {
        //TODO: HACER -> Traer chat de server y pintar por ejemplo
        ComprobarConexionChat();
        wanachat.server.getChat($('#txtHackPerfilActual').val(), chatID);
    }
}

function MostrarChatsActivos(mostrar) {
    if (mostrar) {
        $('#txtHackUsu').val('')
        $('#divChatAccion').css('display', 'none');
        $('#divChatsActivos').css('display', '');
    }
    else {
        $('#divChatAccion').css('display', '');
        $('#divChatsActivos').css('display', 'none');
    }
}

function AgregarContactoChat() {
    MostrarCargandoChat();
    ComprobarConexionChat();
    wanachat.server.addContact($('#txtHackPerfilActual').val());
}

function registrarUsuChat(pInicio) {
    if ($.connection.hub.id != null && $.connection.myChatHub.connection.state === 1/*signalR.connectionState.connected*/) {
        //var wanachat = $.connection.myChatHub;
        wanachat.server.registrar($('#txtHackPerfilActual').val(), pInicio);

        if (pInicio == '0') {
            setTimeout('ComprobarConexionChat()', 300000);
        }
    }
    else {
        setTimeout('registrarUsuChat(\'' + pInicio + '\')', 100);
    }
}

function desactivarUsuChat() {
    ContraerChat();
    $('#hlMaxChat').html(textChat.conectar);
    $('#hlMaxChat').attr('onclick', 'ExpandirChat(false);');
    $('#hlCerrChat').css('display', 'none');

    ComprobarConexionChat();

    if ($.connection.hub.id != null && $.connection.myChatHub.connection.state === 1) {
        wanachat.server.desactivarChat($('#txtHackPerfilActual').val());

        $.connection.hub.disconnect();

        $('#divchatframe').attr('class', 'divChat disabled');
    }
}

function IniciarChat(idPerfilID, nombrePerfil, chatID) {
    MostrarCargandoChat();
    ComprobarConexionChat();
    wanachat.server.addChat($('#txtHackPerfilActual').val(), $('#txtHackNombreActual').val(), idPerfilID, nombrePerfil, chatID);
    MarcarNumMenChat(chatID, 0);
}

function EnviarMensChat(id) {
    var idMen = guidGenerator();
    PintarMensaje($('#txtHackNombreActual').val(), $('#txtMessage').val(), 0, idMen);
    ComprobarConexionChat();
    wanachat.server.send($('#txtMessage').val(), $('#txtHackPerfilActual').val(), $('#txtHackUsu').val(), $('#txtHackNombreActual').val(), id, idMen);
    $('#txtMessage').val('');
}

function ComprobarConexionChat() {
    if (!($.connection.hub.id != null && $.connection.myChatHub.connection.state === 1)) {
        $.connection.hub.start().done(function () { registrarUsuChat('1') }).fail(function () { MontarAlerta('registrar', null, null, null, 1); });
    }
}

function CargarMensAntChat(id, idUsu, numLlamada) {
    ComprobarConexionChat();
    wanachat.server.cargarMensAnt(id, idUsu, numLlamada);
}


function MontarAlerta(alerta, idPerfilID, nombrePerfil, chatID, tipo) {
    if (tipo == 0) {
        $('#divAlertaChat').html('<p><a onclick="ExpandirChat(true);OcultarAlerta();IniciarChat(\'' + idPerfilID + '\', \'' + nombrePerfil + '\', \'' + chatID + '\');">Ver</a> ' + alerta + '</p>');
    }
    else {
        var error = '';

        if (alerta == 'send') {
            error = textChat.errorSend;
        }
        else if (alerta == 'registrar') {
            error = textChat.errorReg;
        }
        else if (alerta == 'desactivarChat') {
            error = textChat.errorDesac;
        }
        else if (alerta == 'addContact') {
            error = textChat.errorContac;
        }
        else if (alerta == 'addChat') {
            error = textChat.errorChat;
        }
        else if (alerta == 'cargarMensAnt') {
            error = textChat.errorCargarMenAnt;
        }
        else if (alerta == 'send') {
            error = textChat.errorSend;
        }
        else if (alerta == 'send') {
            error = textChat.errorSend;
        }
        else if (alerta == 'send') {
            error = textChat.errorSend;
        }

        $('#divAlertaChat').html('<div class=\"ko\" style="display:block;"><p>' + error + '</p></div>');
    }
    var margin = 5 + $('#divAlertaChat').height();
    $('#divAlertaChat').css('margin-top', '-' + margin + 'px');
    $('#divAlertaChat').css('display', '');
    setTimeout('OcultarAlerta()', 5000);
}

function OcultarAlerta() {
    $('#divAlertaChat').css('display', 'none');
}


/****** FIN CHAT **********/

var currentMousePos = { x: -1, y: -1 };
//Devuelve la posisición del cursor 
$(document).mousemove(function (event) {
    currentMousePos.x = event.pageX;
    currentMousePos.y = event.pageY;
});

function Distancia(lat1, lon1, lat2, lon2) {
    rad = function (x) { return x * Math.PI / 180; }

    //Radio de la tierra en km
    var R = 6378.137;
    var dLat = rad(lat2 - lat1);
    var dLong = rad(lon2 - lon1);

    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) + Math.cos(rad(lat1)) * Math.cos(rad(lat2)) * Math.sin(dLong / 2) * Math.sin(dLong / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c;

    //Retorna tres decimales
    return d.toFixed(3);
}

function ObtenerUrlParaCallBack()
{
    var url = document.location.href;
    url = url.substr(0, url.indexOf('#'));
    return url;
}


$(document).ready(function () {
    $('a.linkDescargaFichero').click(function () {
        EnviarAccGogAnac('Descargar Fichero', 'Descargar Fichero', window.location.href);
    });
});


function chequearUsuarioLogueado(funcionRespuesta)
{
    MostrarUpdateProgress();

    var request = new XMLHttpRequest();
    request.onreadystatechange = function ()
    {
        if (request.readyState == 4)
        {
            eval(funcionRespuesta);
        }
    }
    var url = window.location.protocol + '//' + window.location.host + '/solicitarcookie.aspx'
    request.open('GET', url, true);
    request.withCredentials = true;
    request.send();
}

function Redirigir(response)
{
    if (response != undefined )
    {
        window.location.href = response;
    }
}

var PeticionesCookie = {
    CargarCookie() {
        var urlPeticion = null;
        urlPeticion = $('#inpt_UrlLogin').val().split("/login")[0] + "/RefrescarCookie";
        GnossPeticionAjax(
            urlPeticion,
            null,
            true
        ).done(function (response) {
        }).fail(function (response) {
        });
    }
}

var PeticionesAJAX = {
    CargarNumElementosNuevos: function (pGuidPerfilUsu, pGuidPerfilOrg, pEsBandejaOrg, identCargarNov, RepintarContadoresNuevosElementos, RecogerErroresAJAX) {
        var urlPeticion = null;
        if ($('#inpt_proyID').val() == '11111111-1111-1111-1111-111111111111') {
            urlPeticion = location.protocol + "//" + location.host + "/PeticionesAJAX/CargarNumElementosNuevos";
        } else {
            urlPeticion = $('#inpt_baseUrlBusqueda').val() + "/PeticionesAJAX/CargarNumElementosNuevos";
        }
        var datosPost =
        {
            pPerfilUsuarioID: pGuidPerfilUsu,
            pPerfilOrganizacionID: pGuidPerfilOrg,
            pBandejaDeOrganizacion: pEsBandejaOrg,
            pOtrasIdent: identCargarNov
        };
        GnossPeticionAjax(
            urlPeticion,
            datosPost,
            true
        ).done(function (response) {
            if (response.split('|').length == 1) {
                alert('Te has conectado en otro navegador.');
                window.location.href = response;
            } else {
                RepintarContadoresNuevosElementos(response)
            }
        }).fail(function (response) {
            RecogerErroresAJAX(response);
        });
    },
    ObtenerDatosUsuarioActual: function (FuncionEjecutar) {
        var urlPeticion = location.protocol + "//" + location.host + "/PeticionesAJAX/ObtenerDatosUsuarioActual";
        GnossPeticionAjax(
            urlPeticion,
            null,
            true
        ).done(function (response) {
            FuncionEjecutar(response)
        });
    }

}/**/ 
/*busquedas.js*/ 
var estamosFiltrando = false;

window.onpopstate = function (e) {
    if (window.location.href.indexOf('?') != -1 || window.location.href.indexOf('#') == -1) {
        if (estamosFiltrando) {
            FiltrarPorFacetas(ObtenerHash2());
            e.preventDefault();
        }
        estamosFiltrando = true;
    }
    estamosFiltrando = true;
};

var enlazarJavascriptFacetas = false;

function enlazarFiltrosBusqueda() {
    // Cambiado por nuevo Front
    /*$('.limpiarfiltros')
    .unbind()
    .click(function (e) {
        LimpiarFiltros();
        e.preventDefault();
    });*/
    // Permitir click sobre la limpieza de filtros aunque el bot�n no exista ya que es cargadod de forma din�mica    
    $('#panFiltros')
        .unbind()
        .on('click','.limpiarfiltros', function (e) {
            LimpiarFiltros();
            e.preventDefault()
        });         

    $('.panelOrdenContenedor select.filtro')
    .unbind()
        .change(function (e) {            
        AgregarFiltro('ordenarPor', $(this).val(), true);
    });

    // Configurar la selecci�n de ordenaci�n de los resultados al pulsar en "Ordenado por"
    $("#panel-orderBy a.item-dropdown")
        // En ordenación, no mostraba el icono seleccionado ya que lo "desmontaba".
        //.unbind()
        .click(function (e) {                    
            var orderBy = $(this).attr("data-orderBy");
            AgregarFiltro('ordenarPor', orderBy, true);
    });

    $('.panelOrdenContenedor a.filtro')
    .unbind()
    .click(function (e) {
        var filtro = $(this).attr("name");
        AgregarFiltro(filtro.split('|')[0], filtro.split('|')[1], false);
        e.preventDefault();
    });
    $('.panelOrdenContenedor a.filtroV2')
    .unbind()
    .click(function (e) {
        var filtro = $(this).attr("name");
        AgregarFiltro(filtro.split('-')[0], filtro.split('-')[1], false);
        e.preventDefault();
    });
    $('.paginadorResultados a.filtro')
    .unbind()
    .click(function (e) {
        var filtro = $(this).attr("name");
        AgregarFiltro(filtro.split('|')[0], filtro.split('|')[1], false);

        if (typeof searchAnalitics != 'undefined') {
            searchAnalitics.pageSearch(filtro.split('|')[1]);
        }
        e.preventDefault();
    });
}

function enlazarFacetasBusqueda() {
    $('.facetedSearchBox .filtroFaceta')
	.unbind()
	.keydown(function (event) {
	    if ($(this).val().indexOf('|') > -1) {
	        $(this).val($(this).val().replace(/\|/g, ''));
	    };

	    if (event.which || event.keyCode) {
	        if ((event.which == 13) || (event.keyCode == 13)) {
	            return false;
	        }
	    } else {
	        return true;
	    };
	});

    var desde = '';
    var hasta = '';
    if ($('.facetedSearchBox .searchButton').parents('.facetedSearchBox').find('input.hasDatepicker').length > 0) {
        desde = $('.facetedSearchBox .searchButton').parents('.facetedSearchBox').find('input.hasDatepicker')[0].value;
        hasta = $('.facetedSearchBox .searchButton').parents('.facetedSearchBox').find('input.hasDatepicker')[1].value;
    }

    $('.facetedSearchBox .searchButton')
	.unbind()
	.click(function (event) {
	    if ($(this).parents('.facetedSearchBox').find('.filtroFaceta').length == 1) {
	        if ($(this).parents('.facetedSearchBox').find('.filtroFacetaTesauroSemantico').length == 1 && $(this).parents('.facetedSearchBox').find('.filtroFaceta').val().indexOf('@' + $('input.inpt_Idioma').val()) == -1) {
	            AgregarFaceta($(this).parents('.facetedSearchBox').find('.filtroFaceta').attr('name') + '=' + $(this).parents('.facetedSearchBox').find('.filtroFaceta').val() + '@' + $('input.inpt_Idioma').val());
	        } else {
	            AgregarFaceta($(this).parents('.facetedSearchBox').find('.filtroFaceta').attr('name') + '=' + $(this).parents('.facetedSearchBox').find('.filtroFaceta').val());
	        }
	    } else {
	        var filtroBusqueda = $(this).attr('name');
	        var fechaDesde = $(this).parents('.facetedSearchBox').find('input')[0];
	        var fechaHasta = $(this).parents('.facetedSearchBox').find('input')[1];
	        var formatoFecha = false;

	        if (typeof ($(this).parents('.facetedSearchBox').find('input.hasDatepicker')[0]) != 'undefined') {
	            formatoFecha = true;
	        }

	        if (desde == '') {
	            desde = $('input.inpt_Desde').val();
	        }
	        if (hasta == '') {
	            hasta = $('input.inpt_Hasta').val();
	        }

	        var filtro = ObtenerFiltroRango(fechaDesde, desde, fechaHasta, hasta, formatoFecha);

	        if (filtro != '-') {
	            AgregarFaceta(filtroBusqueda + '=' + filtro);
	        }
	    }
	    return false;
	});

    $('.facetedSearch a.faceta')
	.unbind()
	.click(function (e) {
	    AgregarFaceta($(this).attr("name"));
	    e.preventDefault();
	});
    $('.facetedSearch input.faceta')
	.unbind()
	.click(function (e) {
	    AgregarFaceta($(this).attr("name"));
	    e.preventDefault();
	});
    $('.facetedSearch a.faceta.grupo')
	.unbind()
	.click(function (e) {
	    AgregarFacetaGrupo($(this).attr("name"));
	    e.preventDefault();
	});

    $('#descargarRDF')
    .unbind()
	.click(function (e) {
	    var filtros = ObtenerHash2();
	    var url = document.location.href;

	    if (url.indexOf('?') != -1) {
	        url = url.substring(0, url.indexOf('?'));
	    }

	    var filtroRdf = '?rdf';
	    if (filtros != '') {
	        filtros = '?' + filtros;
	        filtroRdf = '&rdf';
	    }

	    url = url + filtros + filtroRdf;
	    $('#descargarRDF').prop('href', url);
	    eval($('#descargarRDF').href);
	});
}

function enlazarFacetasNoBusqueda() {
    $('.facetedSearchBox .filtroFaceta')
	.unbind()
	.keydown(function (event) {
	    if ($(this).val().indexOf('|') > -1) {
	        $(this).val($(this).val().replace(/\|/g, ''));
	    };
	    if (event.which || event.keyCode) {
	        if ((event.which == 13) || (event.keyCode == 13)) {
	            return false;
	        }
	    } else {
	        return true;
	    };
	});

    $('.facetedSearchBox .searchButton')
	.unbind()
	.click(function (event) {
	    event.preventDefault();
	    var urlRedirect = $(this).attr('href') + '?' + $(this).parent().find('.filtroFaceta').attr('name') + '=' + $(this).parent().find('.filtroFaceta').val();
	    window.location.href = urlRedirect;
	});
}

var cargarFacetas = true;

function AgregarFiltro(tipoFiltro, filtro, reiniciarPag) {
    estamosFiltrando = true;
    cargarFacetas = false;
    var filtros = ObtenerHash2();

    var tipoOrden = "";
	if (tipoFiltro ==  "ordenarPor" && filtro.split("|").length>1)
	{
        tipoOrden = filtro.split("|")[1];
        filtro = filtro.split("|")[0];
    }

    //Si el filtro ya existe, cambiamos el valor del filtro
    if (tipoFiltro == "ordenarPor" && filtros.indexOf(tipoFiltro + '=' + filtro) == 0 || filtros.indexOf('&' + tipoFiltro + '=' + filtro) > 0) {
        var tipoFiltroOrden = "orden";
        var filtroOrden = "asc";

	    if (filtros.indexOf(tipoFiltroOrden + '=') == 0 || filtros.indexOf('&' + tipoFiltroOrden + '=') > 0) {
            if (filtros.indexOf(tipoFiltroOrden + '=' + filtroOrden) == 0 || filtros.indexOf('&' + tipoFiltroOrden + '=' + filtroOrden) > 0) {
		  
	            filtroOrden = "desc";
	        }
            if (tipoOrden != "") {
		  
                filtroOrden = tipoOrden;
            }

            var textoAux = filtros.substring(filtros.indexOf(tipoFiltroOrden + '='));
            if (textoAux.indexOf('&') > -1) {
                textoAux = textoAux.substring(0, textoAux.indexOf('&'));
            }
            filtros = filtros.replace(textoAux, tipoFiltroOrden + '=' + filtroOrden);
        }
        else {
            if (filtros.length > 0) { filtros += '&'; }
            filtros += tipoFiltroOrden + '=' + filtroOrden;
        }
    }
    else if (filtros.indexOf(tipoFiltro + '=') == 0 || filtros.indexOf('&' + tipoFiltro + '=') > 0) {
        var textoAux = filtros.substring(filtros.indexOf(tipoFiltro + '='));
        if (textoAux.indexOf('&') > -1) {
            textoAux = textoAux.substring(0, textoAux.indexOf('&'));
        }
        filtros = filtros.replace(textoAux, tipoFiltro + '=' + filtro);

        //Si el filtro es un ordenar por y viene con asc o desc, lo ponemos
        if (tipoFiltro == "ordenarPor" && tipoOrden != "") {
            var textoAux2 = filtros.substring(filtros.indexOf('orden='));
            if (textoAux2.indexOf('&') > -1) {
                textoAux2 = textoAux2.substring(0, textoAux2.indexOf('&'));
            }
            filtros = filtros.replace(textoAux2, 'orden=' + tipoOrden);
        }
    }
    else {
        if (filtros.length > 0) { filtros += '&'; }
        filtros += tipoFiltro + '=' + filtro;

        //Si el filtro es un ordenar por y viene con asc o desc, lo ponemos
        if (tipoFiltro == "ordenarPor" && tipoOrden != "") {
            filtros += '&orden=' + tipoOrden;
        }
    }

    if (reiniciarPag == true) {
        var pagina = 'pagina'
        if (filtros.indexOf(pagina) > -1) {
            if (filtros.indexOf(pagina + '&') > -1) {
                textoAux = filtros.substring(filtros.indexOf(pagina + '&'));
            }
            else if (filtros.indexOf('&' + pagina) > -1) {
                textoAux = filtros.substring(filtros.indexOf('&' + pagina));
            }
            else {
                textoAux = filtros.substring(filtros.indexOf(pagina));
            }

            filtros = filtros.replace(textoAux, '');
        }
    }

    history.pushState('', 'New URL: ' + filtros, '?' + filtros);
    FiltrarPorFacetas(ObtenerHash2());
}

function AgregarFiltroAutocompletar(claveFaceta, filtro) {
    var url = document.location;
    var separador = '?';
    if (url.indexOf("?") != -1) {
        separador = '&';
    }
    document.location = url + separador + claveFaceta + '=' + filtro;
}

var clickEnFaceta = false;

function AgregarFaceta(faceta) {
    faceta = faceta.replace(/%22/g, '"');
    estamosFiltrando = true;
    //var filtros = ObtenerHash2().replace(/%20/g, ' ');
    var filtros = ObtenerHash2();
    filtros = replaceAll(filtros, '%26', '---AMPERSAND---');
    filtros = decodeURIComponent(filtros);
    filtros = replaceAll(filtros, '---AMPERSAND---', '%26');
	


    var esFacetaTesSem = false;

    if (faceta.indexOf('|TesSem') != -1) {
        esFacetaTesSem = true;
        faceta = faceta.replace('|TesSem', '');
    }

    var eliminarFacetasDeGrupo = '';
    if (faceta.indexOf("rdf:type=") != -1 && filtros.indexOf(faceta) != -1) {
        //Si faceta es RDF:type y filtros la contiene, hay que eliminar las las que empiezen por el tipo+;
        eliminarFacetasDeGrupo = faceta.substring(faceta.indexOf("rdf:type=") + 9) + ";";
    }

    var filtrosArray = filtros.split('&');
    filtros = '';

    var tempNamesPace = '';
    if (faceta.indexOf('|replace') != -1) {
        tempNamesPace = faceta.substring(0, faceta.indexOf('='));
        faceta = faceta.replace('|replace', '');
    }

    var facetaDecode = decodeURIComponent(faceta);
    var contieneFiltro = false;

    for (var i = 0; i < filtrosArray.length; i++) {
        if (filtrosArray[i] != '' && filtrosArray[i].indexOf('pagina=') == -1) {
            if (eliminarFacetasDeGrupo == '' || filtrosArray[i].indexOf(eliminarFacetasDeGrupo) == -1) {
                if (tempNamesPace == '' || (tempNamesPace != '' && filtrosArray[i].indexOf(tempNamesPace) == -1)) {
                    filtros += filtrosArray[i] + '&';
                }
            }
        }

        if (filtrosArray[i] != '' && (filtrosArray[i] == faceta || filtrosArray[i] == facetaDecode)) {
            contieneFiltro = true;
        }
    }

    if (filtros != '') {
        filtros = filtros.substring(0, filtros.length - 1);
    }
    if (faceta.indexOf('search=') == 0) {
	 
        $('h1 span#filtroInicio').remove();
    }

    if (typeof (filtroDePag) != 'undefined' && filtroDePag.indexOf(faceta) != -1) {
        var url = document.location.href;
        //var filtros = '';

        if (filtros != '') {
            filtros = '?' + filtros.replace(/ /g, '%20');
            //filtros = '?' + encodeURIComponent(filtros);
        }

        if (url.indexOf('?') != -1) {
            //filtros = url.substring(url.indexOf('?'));
            url = url.substring(0, url.indexOf('?'));
        }

        if (url.substring(url.length - 1) == '/') {
            url = url.substring(0, (url.length - 1));
        }

        //Quito los dos ultimos trozos:
        url = url.substring(0, url.lastIndexOf('/'));
        url = url.substring(0, url.lastIndexOf('/'));

        if (filtroDePag.indexOf('skos:ConceptID') != -1) {
            var busAvazConCat = false;

            if (typeof (textoCategoria) != 'undefined') {
                //busAvazConCat = (url.indexOf('/' + textoCategoria) == (url.length - textoCategoria.length - 1));
                if (url.indexOf(textoComunidad + '/') != -1) {
                    var trozosUrl = url.substring(url.indexOf(textoComunidad + '/')).split('/');
                    busAvazConCat = (trozosUrl[2] == textoCategoria);
                }
            }

            url = url.substring(0, url.lastIndexOf('/'));

            if (busAvazConCat) {
                url += '/' + textoBusqAvaz;
            }
        }


        MostrarUpdateProgress();

        document.location = url + filtros;
        return;
    }
    else if (!contieneFiltro) {
        //Si no existe el filtro, lo a?adimos
        if (filtros.length > 0) { filtros += '&'; }
        filtros += faceta;

        if (typeof searchAnalitics != 'undefined') {
            searchAnalitics.facetsSearchAdd(faceta);
        }
    }
    else {
        filtros = '';

        for (var i = 0; i < filtrosArray.length; i++) {
            if (filtrosArray[i] != '' && filtrosArray[i] != faceta && filtrosArray[i] != facetaDecode) {
            //if (filtrosArray[i] != '' && (filtrosArray[i].indexOf(faceta) == -1 || filtrosArray[i].indexOf(facetaDecode)) == -1)) {
                filtros += filtrosArray[i] + '&';
            }
        }

        if (filtros != '') {
            filtros = filtros.substring(0, filtros.length - 1);
        }

        if (!esFacetaTesSem && typeof searchAnalitics != 'undefined') {
            searchAnalitics.facetsSearchRemove(faceta);
        }
    }

    filtros = filtros.replace(/&&/g, '&');
    if (filtros.indexOf('&') == 0) {
        filtros = filtros.substr(1, filtros.length);
    }
    if (filtros[filtros.length - 1] == '&') {
        filtros = filtros.substr(0, filtros.length - 1);
    }

    filtros = filtros.replace(/\\\'/g, '\'');
    filtros = filtros.replace('|', '%7C');

    history.pushState('', 'New URL: ' + filtros, '?' + filtros);
    FiltrarPorFacetas(ObtenerHash2());
    EscribirUrlForm(filtros);
}

function EscribirUrlForm(filtros) {
    var accion = $('#aspnetForm').attr('action');
    if (accion != undefined) {
        if (accion.indexOf('?') != -1) {
            accion = accion.substring(0, accion.indexOf('?'));
        }
        accion += '?' + filtros;
        $('#aspnetForm').attr('action', accion);
    }
}

function AgregarFacetaGrupo(faceta) {
 
    estamosFiltrando = true;
    var filtros = ObtenerHash2();
    filtros = replaceAll(filtros, '%26', '---AMPERSAND---');
    filtros = decodeURIComponent(filtros);
    filtros = replaceAll(filtros, '---AMPERSAND---', '%26');

    var agregar = false;
    if (filtros.indexOf("default;" + faceta) == -1) {
        agregar = true;
    }

    var filtrosArray = filtros.split('&');
    filtros = '';

    for (var i = 0; i < filtrosArray.length; i++) {
        if (filtrosArray[i] != '' && filtrosArray[i].indexOf('pagina=') == -1) {
            //Quita los default
            filtros += filtrosArray[i].replace("default;rdf:type", "rdf:type") + '&';
        }
    }

    if (agregar) {
	 
        if (filtros.indexOf(faceta) != -1) {
            //Si lo contiene lo reemplaza
            filtros = filtros.replace(faceta, "default;" + faceta);
        } else {
            //Si no lo contiene lo a?ade
            filtros += "default;" + faceta;
        }
    }

    filtros = filtros.replace(/&&/g, '&');
    if (filtros.indexOf('&') == 0) {
        filtros = filtros.substr(1, filtros.length);
    }
    if (filtros[filtros.length - 1] == '&') {
        filtros = filtros.substr(0, filtros.length - 1);
    }

    filtros = filtros.replace(/\\\'/g, '\'');

    history.pushState('', 'New URL: ' + filtros, '?' + filtros);
    FiltrarPorFacetas(ObtenerHash2());
}

function LimpiarFiltros() {
    if (typeof (filtroDePag) != 'undefined' && filtroDePag != '') {
        var url = document.location.href;

        if ($('.limpiarfiltros') != undefined && $('.limpiarfiltros').attr('href') != undefined && $('.limpiarfiltros').attr('href') != '') {
            url = $('.limpiarfiltros').attr('href');
        }
        else {
            if (url.indexOf('?') != -1) {
                url = url.substring(0, url.indexOf('?'));
            }

            if (url.substring(url.length - 1) == '/') {
                url = url.substring(0, (url.length - 1));
            }

            //Quito los dos ultimos trozos:
            url = url.substring(0, url.lastIndexOf('/'));
            url = url.substring(0, url.lastIndexOf('/'));

            //if (filtroDePag.indexOf('skos:ConceptID') != -1) {
            //    url = url.substring(0, url.lastIndexOf('/'));
            //}
            if (filtroDePag.indexOf('skos:ConceptID') != -1) {
                var busAvazConCat = false;

                if (typeof (textoCategoria) != 'undefined') {
                    //busAvazConCat = (url.indexOf('/' + textoCategoria) == (url.length - textoCategoria.length - 1));
                    if (url.indexOf(textoComunidad + '/') != -1) {
                        var trozosUrl = url.substring(url.indexOf(textoComunidad + '/')).split('/');
                        busAvazConCat = (trozosUrl[2] == textoCategoria);
                    }
                }

                url = url.substring(0, url.lastIndexOf('/'));

                if (busAvazConCat) {
                    url += '/' + textoBusqAvaz;
                }
            }
        }

        MostrarUpdateProgress();

        document.location = url;
        return;
    }
    $('h1 span#filtroInicio').remove();

    history.pushState('', 'New URL: ', '?');
    FiltrarPorFacetas('');
}

function ObtenerHash2() {
    var url = window.location.href;

    if (url.EndsWith("&rss") || url.EndsWith("?rss") || url.EndsWith("&rdf") || url.EndsWith("?rdf")) {
        url = url.substr(0, url.length - 4);
    }
    else if (url.StartsWith("rss&") || url.StartsWith("rdf&")) {
        url = url.substr(4);
    }

    if (url.indexOf('?') > 1) {
        return url.substr(url.indexOf('?') + 1);
    }
    return "";
}

var primeraCargaDeFacetas = true;

function FiltrarPorFacetas(filtro) {
    if (typeof FiltrarBandejaMensajes != "undefined") {
        return FiltrarBandejaMensajes(filtro);
    }
    else if (typeof FiltrarPerfilUsuario != "undefined") {
        return FiltrarPerfilUsuario(filtro);
    }
    else if (typeof ProcesarFiltro != "undefined") {
        return ProcesarFiltro(filtro);
    }
    return FiltrarPorFacetasGenerico(filtro);
}

function FiltrarPorFacetasGenerico(filtro) {
    filtro = filtro.replace(/&/g, '|');

    if (typeof (filtroDePag) != 'undefined' && filtroDePag != '') {
        if (filtro != '') {
            filtro = filtroDePag + '|' + filtro;
        }
        else {
            filtro = filtroDePag;
        }
    }
    //Si hay orden por relevancia pero no hay filtro search, quito el orden para que salga el orden por defecto
    //    if(QuitarOrdenReleavanciaSinSearch(filtro))
    //    {
    //        return false;
    //    }
    filtrosPeticionActual = filtro;

    var rdf = false;
    if (filtro.indexOf('?rdf') != -1 && ((filtro.indexOf('?rdf') + 4) == filtro.length)) {
        filtro = filtro.substring(0, filtro.length - 4);
        document.location.hash = document.location.hash.substring(0, document.location.hash.length - 4);
        rdf = true;
    }

    enlazarJavascriptFacetas = true;

    var arg = filtro;

    /*
    var vistaMapa = ($('li.mapView').attr('class') == "mapView activeView");
    var vistaChart = ($('.chartView').attr('class') == "chartView activeView");
    */

    var vistaMapa = $('li.mapView').hasClass('activeView');
    var vistaChart = $('.chartView').hasClass('activeView');

    if (!primeraCargaDeFacetas && !vistaMapa) {
        MostrarUpdateProgress();
    }

    var parametrosFacetas = 'ObtenerResultados';

    var gruposPorTipo = $('#facetedSearch.facetedSearch .listadoAgrupado ').length>0;

    if (cargarFacetas && !gruposPorTipo) {
        if (typeof panFacetas != "undefined" && panFacetas != "" && $('#' + panFacetas).length > 0 && !primeraCargaDeFacetas && !gruposPorTipo) {
            $('#' + panFacetas).html('')
        }
        if (numResultadosBusq != "" && $('#' + numResultadosBusq).length > 0 && !primeraCargaDeFacetas) {
            $('#' + numResultadosBusq).html('')
            $('#' + numResultadosBusq).css('display', 'none');
        }
        if (!clickEnFaceta && panFiltrosPulgarcito != "" && $('#' + panFiltrosPulgarcito).length > 0 && !primeraCargaDeFacetas) {
            $('#' + panFiltrosPulgarcito).html('')
        }
    }

    if (!vistaMapa) {
        SubirPagina();
    }

    if (typeof idNavegadorBusqueda != "undefined") {
        $('#' + idNavegadorBusqueda).html('');
        $('#' + idNavegadorBusqueda).css('display', 'none');
    }

    if (!vistaMapa && !primeraCargaDeFacetas) {
        document.getElementById(updResultados).innerHTML = '';
        $('#' + updResultados).attr('style', '');
    }

    clickEnFaceta = false;
    var primeraCarga = false;

    if (filtro.length > 1 || document.location.href.indexOf('/tag/') > -1 || (filtroContexto != null && filtroContexto != '')) {
        parametrosFacetas = 'AgregarFacetas|' + arg;
        var parametrosResultados = 'ObtenerResultados|' + arg;
        if (!cargarFacetas) {
            var parametrosResultados = 'ObtenerResultadosSinFacetas|' + arg;
        }
        //cargarFacetas
        var displayNone = '';
        document.getElementById('query').value = parametrosFacetas;
        if (HayFiltrosActivos(filtro) && (tipoBusqeda != 12 || filtro.indexOf("=") != -1)) {
            $('#' + divFiltros).css('display', '');
            $('#' + divFiltros).css('padding-top', '0px !important');
            //$('#' + divFiltros).css('margin-top', '10px');
        }
        var pLimpiarFilt = $('p', $('#' + divFiltros)[0]);

        if (pLimpiarFilt != null && pLimpiarFilt.length > 0) {
            if (!(filtro.length > 1 || document.location.href.indexOf('/tag/') > -1)) {
                pLimpiarFilt[0].style.display = 'none';
            }
            else {
                pLimpiarFilt[0].style.display = '';
            }
        }
    }
    else {
        primeraCarga = true;
        $('#' + divFiltros).css('display', 'none');
        $('#' + divFiltros).css('padding-top', '0px !important');
        //$('#' + divFiltros).css('margin-top', '10px');
    }

    if (rdf) {
        eval(document.getElementById('rdfHack').href);
    }

    var tokenAfinidad = guidGenerator();

    if (vistaMapa || !primeraCargaDeFacetas) {
        MontarResultados(filtro, primeraCarga, 1, '#' + panResultados, tokenAfinidad);
    }

    if (panFacetas != "" && (cargarFacetas || document.getElementById(panFacetas).innerHTML == '')) {
        var inicioFacetas = 1;

        MontarFacetas(filtro, primeraCarga, inicioFacetas, '#' + panFacetas, null, tokenAfinidad);
    }

    primeraCargaDeFacetas = false;
    cargarFacetas = true;

    var txtBusquedaInt = $('.aaCabecera .searchGroup .text')
    var textoSearch = 'search=';
    if ((filtro.indexOf(textoSearch) > -1) && txtBusquedaInt.length > 0) {
        var filtroSearch = filtro.substring(filtro.indexOf(textoSearch) + textoSearch.length);
        if (filtroSearch.indexOf('|') > -1) {
            filtroSearch = filtroSearch.substring(0, filtroSearch.indexOf('|'));
        }

        txtBusquedaInt.val(decodeURIComponent(filtroSearch));
        txtBusquedaInt.blur();
    }
    CambiarOrden(filtro);
    return false;
}

function CambiarOrden(hash) {
    if ($('.panelOrdenContenedor select.filtro').length > 0) {
        var controlOrden = $('.panelOrdenContenedor select.filtro');

        var ordenarPor = 'ordenarPor=';
        var search = (hash.indexOf('search=') != -1);
        var noOrden = (hash.indexOf(ordenarPor) == -1);

        //controlOrden.find('option:selected').removeAttr('selected');
        $('.panelOrdenContenedor select.filtro option[selected]').removeAttr('selected');

        if (noOrden) {
            //Si no hay orden, compruebo cu?l es la opci?n de orden que debe estar activa
            var opcion = controlOrden.find('option[value="' + ordenPorDefecto + '"]');
            var opcionSearch = controlOrden.find('option[value="' + ordenEnSearch + '"]');
            var ordenSearch = false;
            var ordenSearchPorDefecto = (ordenEnSearch == ordenPorDefecto);

            if (search) {
                //Si viene el par?metro search, pongo a seleccionado el orden en search
                opcion = opcionSearch;
                ordenSearch = true;
            }

            if (!ordenSearchPorDefecto) {
                //Si el orden por defecto no es el mismo que el orden en search, no ser? visible salvo que est? seleccionado
                if (!ordenSearch) {
                    opcionSearch.attr('style', 'display:none');
                }
                else {
                    opcionSearch.removeAttr('style');
                }
            }
        }
        else if (noOrden != -1) {
            //Obtengo el orden actual para seleccionar esa opci?n en el combo 
            var orden = hash.substring(hash.indexOf(ordenarPor) + ordenarPor.length);
            var indiceBarraFin = orden.indexOf('|');
            if (indiceBarraFin != -1) {
                orden = orden.substring(0, indiceBarraFin);
            }

            var opcion = controlOrden.find('option[value="' + orden + '"]');
        }

        if (opcion.length > 0) {
            //Marco el orden actual como seleccionado
            opcion.attr('selected', 'selected');

            if (opcion.length > 0 && opcion.prevObject.length > 0) {
                opcion.prevObject[0].selectedIndex = opcion[0].index;
            }
        }
    }
}

function MontarFechaCliente() {
    if (typeof (diffHoras) == 'undefined' || diffHoras == null) {
        var fechaServidor = new Date($('#inpt_serverTime').val());
        $('p.publicacion strong').each(function (index) {
            if ($(this).attr('content') != null) {
                var fechaRecurso = new Date($(this).attr('content'));
                var fechaCliente = new Date();
                //var diffHoras = parseInt((fechaServidor.getTime() / (1000 * 60 * 60)) - (fechaCliente.getTime() / (1000 * 60 * 60)));
                var diffMinutos = parseInt((fechaServidor.getTime() / (1000 * 60)) - (fechaCliente.getTime() / (1000 * 60)));
                var diffHoras = diffMinutos / 60;
                //redondeo
                var resto = diffMinutos % 60;
                if (resto / 60 > 0.5) {
                    if (diffHoras > 0) {
                        diffHoras = diffHoras + 1;
                    }
                    else {
                        diffHoras = diffHoras - 1;
                    }
                }
                fechaRecurso.setHours(fechaRecurso.getHours() - diffHoras);
                var dia = fechaRecurso.getDate();
                if (dia < 10) {
                    dia = '0' + dia;
                }
                var mes = fechaRecurso.getMonth() + 1;
                if (mes < 10) {
                    mes = '0' + mes;
                }
                //var fechaPintado = fechaRecurso.format("yyyy/MM/dd HH:mm");
                var fechaPintado = tiempo.fechaPuntos.replace('@1@', dia).replace('@2@', mes).replace('@3@', fechaRecurso.getFullYear());
                $(this).html(fechaPintado);
                $(this).show();
            }
        });
    }
}

var bool_usarMasterParaLectura = false;
$(document).ready(function () {
    bool_usarMasterParaLectura = $('input.inpt_usarMasterParaLectura').val() == 'True';
});
var finUsoMaster = null;

//Realizamos la peticion
function MontarResultados(pFiltros, pPrimeraCarga, pNumeroResultados, pPanelID, pTokenAfinidad) {
    contResultados = contResultados + 1;
    if (document.getElementById('ctl00_ctl00_CPH1_CPHContenido_txtRecursosSeleccionados') != null) {
        document.getElementById('ctl00_ctl00_CPH1_CPHContenido_txtRecursosSeleccionados').value = '';
        document.getElementById('ctl00_ctl00_CPH1_CPHContenido_lblErrorMisRecursos').style.display = 'none';
    }
    var paramAdicional = parametros_adiccionales;

    /*
    if ($('li.mapView').attr('class') == "mapView activeView") {
        paramAdicional += 'busquedaTipoMapa=true';
    }*/
    /*
    if ($('.chartView').attr('class') == "chartView activeView") {
        paramAdicional = 'busquedaTipoChart=' + chartActivo + '|' + paramAdicional;
    }*/

    if ($('li.mapView').hasClass('activeView')) {
        paramAdicional += 'busquedaTipoMapa=true';
    }


    if ($('.chartView').hasClass('activeView')) {
        paramAdicional = 'busquedaTipoChart=' + chartActivo + '|' + paramAdicional;
    }

    var metodo = 'CargarResultados';
    var params = {};

    if (bool_usarMasterParaLectura) {
        if (finUsoMaster == null) {
            finUsoMaster = new Date();
            finUsoMaster.setMinutes(finUsoMaster.getMinutes() + 1);
        }
        else {
            var fechaActual = new Date();
            if (fechaActual > finUsoMaster) {
                bool_usarMasterParaLectura = false;
                finUsoMaster = null;
            }
        }
    }

    params['pUsarMasterParaLectura'] = bool_usarMasterParaLectura;
    params['pProyectoID'] = $('input.inpt_proyID').val();
    params['pEsUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';

    if (typeof (identOrg) != 'undefined') {
	 
        params['pIdentidadID'] = identOrg;
    }
    else {
	 
        params['pIdentidadID'] = $('input.inpt_identidadID').val();
    }
    params['pParametros'] = '' + pFiltros.replace('#', '');
    params['pLanguageCode'] = $('input.inpt_Idioma').val();
    params['pPrimeraCarga'] = pPrimeraCarga == "True";
    params['pAdministradorVeTodasPersonas'] = adminVePersonas == "True";
    params['pTipoBusqueda'] = tipoBusqeda;
    params['pNumeroParteResultados'] = pNumeroResultados;
    params['pGrafo'] = grafo;
    params['pFiltroContexto'] = filtroContexto;
    params['pParametros_adiccionales'] = paramAdicional;
    params['cont'] = contResultados;
    params['tokenAfinidad'] = pTokenAfinidad;

    $.post(obtenerUrl($('input.inpt_UrlServicioResultados').val()) + "/" + metodo, params, function (response) {
        if (params['cont'] == contResultados) {
            var data = response
            if (response.Value != null) {
                data = response.Value;
            }

            var vistaMapa = (params['pParametros_adiccionales'].indexOf('busquedaTipoMapa=true') != -1);
            var vistaChart = (params['pParametros_adiccionales'].indexOf('busquedaTipoChart=') != -1);

            var descripcion = data;

            var funcionJS = '';
            if (descripcion.indexOf('###ejecutarFuncion###') != -1) {
                var funcionJS = descripcion.substring(descripcion.indexOf('###ejecutarFuncion###') + '###ejecutarFuncion###'.length);
                funcionJS = funcionJS.substring(0, funcionJS.indexOf('###ejecutarFuncion###'));

                descripcion = descripcion.replace('###ejecutarFuncion###' + funcionJS + '###ejecutarFuncion###', '');
            }

            if (tipoBusqeda == 12) {
                var panelListado = $(pPanelID).parent();
                panelListado.html('<div id="' + pPanelID.replace('#', '') + '"></div><div id="' + panResultados.replace('#', '') + '"></div>')

                var panel = $(pPanelID);
                panel.css('display', 'none');
                panel.html(descripcion);
                panelListado.append(panel.find('.resource-list').html())
                panel.find('.resource-list').html('');
            } else if (!vistaMapa && !vistaChart) {
                $(pPanelID).append(descripcion);
            }
            else {
                var arraydatos = descripcion.split('|||');

                if ($('#panAuxMapa').length == 0) {
                    $(pPanelID).parent().html($(pPanelID).parent().html() + '<div id="panAuxMapa" style="display:none;"></div>');
                }

                if (vistaMapa) {
                    $('#panAuxMapa').html('<div id="numResultadosRemover">' + arraydatos[0] + '</div>');
                }

                if (vistaChart) {
                    datosChartActivo = arraydatos;
                    $(pPanelID).html('<div id="divContChart"></div>');
                    eval(jsChartActivo);
                }
                else {
                    utilMapas.MontarMapaResultados(pPanelID, arraydatos);
                }
            }
            FinalizarMontarResultados(paramAdicional, funcionJS, pNumeroResultados, pPanelID);
        }
        if (MontarResultadosScroll.pagActual != null) {
            MontarResultadosScroll.pagActual = 1;
            MontarResultadosScroll.cargarScroll();
        }
    }, "json");
}

///Uso:
/// <summary>
/// Engancha el comporatamiento del scroll
/// </summary>
/// <param name="pFooter">Identificador Jquery del elemnto que lanzará la búsqueda al llegar al scroll</param>
/// <param name="pResource">Identificador Jquery de los elementos que representan los recursos</param>
/// <returns></returns>
/// MontarResultadosScroll.init(pFooter,pResource);			

/// <summary>
/// Método que recibe los resultados
/// </summary>
/// <param name="pData">HTML con los resultados</param>
/// <returns></returns>
/// MontarResultadosScroll.CargarResultadosScroll = function (pData) {
///     var htmlRespuesta = document.createElement("div");
///     htmlRespuesta.innerHTML = pData;
///     $(htmlRespuesta).find('.resource').each(function () {
///         $('#panResultados .resource').last().after(this)
///     });
/// }

var MontarResultadosScroll = {
    footer: null,
    item: null,
    pagActual: null,
    active: true,
    init: function (idFooterJQuery, idItemJQuery) {
        this.pagActual = 1;
        this.footer = $(idFooterJQuery);
        this.item = idItemJQuery;
        this.cargarScroll();
        return;
    },
    cargarScroll: function () {
        var that = this;
        that.destroyScroll();
        // opciones del waypoint
        var opts = {
            offset: '100%'
        };
        that.footer.waypoint(function (event, direction) {
            that.peticionScrollResultados().done(function (data) {
                that.destroyScroll();
                var htmlRespuesta = document.createElement("div");
                htmlRespuesta.innerHTML = data;
                if ($(htmlRespuesta).find(that.item).length > 0) {
                    that.CargarResultadosScroll(data);
                    that.cargarScroll();
                } else {
                    that.CargarResultadosScroll('');
                }
                if ((typeof CompletadaCargaRecursos != 'undefined')) {
                    CompletadaCargaRecursos();
                }
                if (typeof (urlCargarAccionesRecursos) != 'undefined') {
                    ObtenerAccionesListadoMVC(urlCargarAccionesRecursos);
                }
            });
        }, opts);
        return;
    },
    destroyScroll: function () {
        this.footer.waypoint('destroy');
        return;
    },
    peticionScrollResultados: function () {
        var defr = $.Deferred();
        //Realizamos la peticion 
        if (this.pagActual == null) {
            this.pagActual = 1;
        }
        this.pagActual++;
        //var servicio = new WS($('input.inpt_UrlServicioResultados').val(), WSDataType.jsonp);
        var filtros = ObtenerHash2().replace(/&/g, '|');

        if (typeof (filtroDePag) != 'undefined' && filtroDePag != '') {
            if (filtros != '') {
                filtros = filtroDePag + '|' + filtros;
            }
            else {
                filtros = filtroDePag;
            }
        }

        filtros += "|pagina=" + this.pagActual;
        var params = {};

        params['pUsarMasterParaLectura'] = bool_usarMasterParaLectura;
        params['pProyectoID'] = $('input.inpt_proyID').val();
        params['pEsUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
        params['pIdentidadID'] = $('input.inpt_identidadID').val();
        params['pParametros'] = '' + filtros.replace('#', '');
        params['pLanguageCode'] = $('input.inpt_Idioma').val();
        params['pPrimeraCarga'] = false;
        params['pAdministradorVeTodasPersonas'] = adminVePersonas.toString().toLowerCase() == "true";
        params['pTipoBusqueda'] = tipoBusqeda;
        params['pNumeroParteResultados'] = 1;
        params['pGrafo'] = grafo;
        params['pFiltroContexto'] = filtroContexto;
        params['pParametros_adiccionales'] = parametros_adiccionales;
        params['cont'] = contResultados;


        $.post(obtenerUrl($('input.inpt_UrlServicioResultados').val()) + "/CargarResultados", params, function (response) {
            if (params['cont'] == contResultados) {
                var data = response
                if (response.Value != null) {
                    data = response.Value;
                }
                defr.resolve(data);
            }
        }, "json");
        return defr;
    }
}


///Uso:
/// <summary>
/// Engancha el comporatamiento de la paginación de resultados
/// </summary>
/// <param name="pEnlaceResource">Identificador Jquery de los elementos que tienen enlaces del recurso representan los recursos</param>
/// <param name="panResultados">Identificador Jquery del elemnto que identifica que se trata de una página de búsqueda</param>
/// <returns></returns>
/// MontarPaginacionBusquedaResultados.init(pEnlaceResource,'#panResultados');			

/// <summary>
/// Método que recibe los resultados
/// </summary>
/// <param name="pBusquedaArray">Array con los enlaces de los recursos de la búsqueda</param>
/// <param name="pIndice">Indice del recurso actual</param>
/// <returns></returns>
///MontarPaginacionBusquedaResultados.MontarBotonera = function (pBusquedaArray, pIndice) {
///    if (pIndice > 0) {
///        //Botón atrás
///        var botonAnterior = '<a href="' + pBusquedaArray[pIndice - 1] + '">Anterior</a>';
///        $(botonAnterior).insertBefore('footer');
///    }
///    if (pIndice < pBusquedaArray.length - 1) {
///        //Botón siguiente
///        var botonSiguiente = '<a href="' + pBusquedaArray[pIndice + 1] + '">Siguiente</a>';
///        $(botonSiguiente).insertBefore('footer');
///    }
//}
var MontarPaginacionBusquedaResultados = {
    numActual: 0,
    itemEnlace: null,
    pagBusqueda: null,
    cookiesEnabled: null,
    init: function (idEnlaceItemJQuery, idContenedorPaginaBusqueda) {
        this.itemEnlace = idEnlaceItemJQuery;
        this.pagBusqueda = idContenedorPaginaBusqueda;
        if ($(this.pagBusqueda).length > 0) {
            //Enganchar paginación de la búsqueda de resultados
            this.cargarPaginacion();
        }
        //Inseratr botones paginación
        this.botonesPaginacion();
        return;
    }, comprobarCookies: function () {
        if (this.cookiesEnabled == null) {
            var dt = new Date();
            dt.setSeconds(dt.getSeconds() + 60);
            document.cookie = "cookietest=1; expires=" + dt.toGMTString();
            this.cookiesEnabled = document.cookie.indexOf("cookietest=") != -1;
            dt.setSeconds(dt.getSeconds() - 120);
            document.cookie = "cookietest=1; expires=" + dt.toGMTString();
        }
        return this.cookiesEnabled;
    },
    cargarPaginacion: function () {
        var urlBusqueda = window.location.href;
        var guid = guidGenerator();
        var recursos = [];

        // Modificamos urls y almacenamos
        $(this.itemEnlace).each(function () {
            var newUrl = $(this).attr('href');
            if (newUrl.indexOf("?") > 0) {
                newUrl = newUrl.substring(0, newUrl.indexOf("?"));
            }

            newUrl += "?searchid=" + guid;
            $(this).attr('href', newUrl);
            if ($.inArray(newUrl, recursos) == -1) {
                recursos.push(newUrl);
            }
        });

        var vars = [], hash;
        if (window.location.href.indexOf('?') > -1) {
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push([hash[0], hash[1]]);
            }
        }

        urlBusqueda = "";
        pagina = 1;
        for (var j = 0; j < vars.length; j++) {
            if (vars[j][0] == "pagina") {
                pagina = vars[j][1];
            } else {
                urlBusqueda += "&" + vars[j][0] + "=" + vars[j][1];
            }
        }

        var InfoBusqueda = {
            resourceList: recursos,
            searchUrl: urlBusqueda,
            tipoBusqueda: tipoBusqeda,
            grafo: grafo,
            filtroContexto: filtroContexto,
            parametros_adiccionales: parametros_adiccionales,
            paginaMin: pagina,
            paginaMax: pagina
        }
        var listaBusquedas = this.comprobarCookies() ? JSON.parse(localStorage.getItem("searchids")) : null;
        if (listaBusquedas == null) {
            listaBusquedas = [];
        } else if (listaBusquedas.length >= 20) {
            localStorage.removeItem(listaBusquedas.shift());
        }
        listaBusquedas.push(guid);
        if (this.comprobarCookies()) {
            localStorage.setItem("searchids", JSON.stringify(listaBusquedas));
            localStorage.setItem(guid, JSON.stringify(InfoBusqueda));
        }
    }, botonesPaginacion: function () {
        this.numActual++;
        var searchid = window.location.search;
        searchid = searchid.substring(searchid.indexOf("searchid=") + "searchid=".length);
        var InfoBusqueda = this.comprobarCookies() ? JSON.parse(localStorage.getItem(searchid)) : null;
        if (InfoBusqueda != null) {
            var indice = $.inArray(window.location.href, InfoBusqueda.resourceList);
            if (indice == InfoBusqueda.resourceList.length - 1 && this.numActual < 2) {
                //Si estamos en el último recurso
                //Cargamos la página siguiente
                this.peticionPaginaSiguiente(searchid, InfoBusqueda, true);
            } else if (indice == 0 && InfoBusqueda.paginaMin > 1 && this.numActual < 2) {
                //Si estamos en el primer recurso y la página es mayor a la 1
                //Cargamos la página anterior
                this.peticionPaginaSiguiente(searchid, InfoBusqueda, false);
            } else if (indice != -1) {
                //Si no es ninguno de los dos casos anteriores y el recurso está en la lista montamos los botones
                this.MontarBotonera(InfoBusqueda.resourceList, indice);
            }
        }
    }, peticionPaginaSiguiente: function (searchID, InfoBusqueda, siguiente) {
        var pagina = 0;
        if (siguiente) {
            InfoBusqueda.paginaMax = InfoBusqueda.paginaMax + 1;
            pagina = InfoBusqueda.paginaMax;
        } else {
            InfoBusqueda.paginaMin = InfoBusqueda.paginaMin - 1;
            pagina = InfoBusqueda.paginaMin;
        }

        var that = this;
        var filtros = InfoBusqueda.searchUrl.substring(InfoBusqueda.searchUrl.indexOf("?") + 1).replace(/&/g, '|');
        filtros += "|pagina=" + pagina;
        var params = {};

        params['pUsarMasterParaLectura'] = bool_usarMasterParaLectura;
        params['pProyectoID'] = $('input.inpt_proyID').val();
        params['pEsUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
        params['pIdentidadID'] = $('input.inpt_identidadID').val();
        params['pParametros'] = '' + filtros.replace('#', '');
        params['pLanguageCode'] = $('input.inpt_Idioma').val();
        params['pPrimeraCarga'] = false;
        params['pAdministradorVeTodasPersonas'] = false;
        params['pTipoBusqueda'] = InfoBusqueda.tipoBusqueda;
        params['pNumeroParteResultados'] = 1;
        params['pGrafo'] = InfoBusqueda.grafo;
        params['pFiltroContexto'] = InfoBusqueda.filtroContexto;
        params['pParametros_adiccionales'] = InfoBusqueda.parametros_adiccionales;

        $.post(obtenerUrl($('input.inpt_UrlServicioResultados').val()) + "/CargarResultados", params, function (response) {
            var data = response
            if (response.Value != null) {
                data = response.Value;
            }

            // Almacenamos urls
            var htmlRespuesta = document.createElement("div");
            htmlRespuesta.innerHTML = data;
            var listaRecursos = [];
            $(htmlRespuesta).find(that.itemEnlace).each(function () {
                var newUrl = $(this).attr('href') + "?searchid=" + searchID;
                if ($.inArray(newUrl, listaRecursos) == -1) {
                    listaRecursos.push(newUrl);
                }
            });

            if (listaRecursos.length > 0) {
                if (siguiente) {
                    InfoBusqueda.resourceList = InfoBusqueda.resourceList.concat(listaRecursos);
                } else {
                    InfoBusqueda.resourceList = listaRecursos.concat(InfoBusqueda.resourceList);
                }
                if (that.comprobarCookies()) {
                    //Almacenamos los nuevos recursos
                    localStorage.setItem(searchID, JSON.stringify(InfoBusqueda));
                }
            }
            //Montamos la paginacion
            that.botonesPaginacion();
        }, "json");
    }
}


function FinalizarMontarResultados(paramAdicional, funcionJS, pNumeroResultados, pPanelID) {

    var vistaMapa = (paramAdicional.indexOf('busquedaTipoMapa=true') != -1);
    var vistaChart = (paramAdicional.indexOf('busquedaTipoChart=') != -1);

    if (pNumeroResultados == 1) {
        if (typeof googleAnalyticsActivo !== "undefined" && googleAnalyticsActivo !== null && googleAnalyticsActivo) {
            RegistrarVisitaGoogleAnalytics();
        }
    }

    $('#divFiltrar').hide();
    $(pPanelID).show();

    $('#' + panFiltrosPulgarcito).css('display', '');

    EjecutarScriptsIniciales();
    if (pNumeroResultados == 1) { OcultarUpdateProgress(); viewOptions.init(); MontarNumResultados(); }

    if (funcionJS != '') {
        eval(funcionJS);
    }

    if (tipoBusqeda == 12 || tipoBusqeda == 11) {
        var vistaCompacta = $('#col02 .resource-list.compactview').length > 0;
        $('#col02 .resource-list').find('.resource').each(function (indice) {
            var mensaje = $(this);
            var utils = mensaje.find('.utils-2');
            var acciones = mensaje.find('.acciones');
            utils.show();
            acciones.show();
            mensaje.removeClass('over');
            if (vistaCompacta) { acciones.hide(); }
            mensaje.hover(
		        function () {
		            $(this).removeClass('over');
		            utils.show();
		            acciones.show();
		        },
		        function () {
		            $(this).removeClass('over');
		            utils.show();
		            acciones.show();
		        }
	        );
        });
    }
    MontarFechas();

    limpiarActividadRecienteHome.init();

    enlazarFiltrosBusqueda();

    if (typeof listadoMensajesMostrarAcciones != 'undefined') {
        /* Lanzar el evento de las imagenes */
        listadoMensajesMostrarAcciones.init();
    }

    /* numero categorias */
    mostrarNumeroCategorias.init();

    /* numero etiquetas */
    mostrarNumeroEtiquetas.init();

    /* enganchar mas menos categorias y etiquetas */
    verTodasCategoriasEtiquetas.init();

    /* pintar iconos de los videos*/
    pintarRecursoVideo.init();

    /* Es listado de mensajes */
    if (tipoBusqeda == 12) {
        listado = $('#col02 .listadoMensajes .resource-list.compactview');
        if (listado.length > 0) {
            listadoMensajesMostrarAcciones.init();
            $('#col02 .listadoMensajes .resource-list .resource h4 a').each(function (indice) {
                var enlaceTitulo = $(this);
                var asunto = enlaceTitulo.attr("title");

                if (asunto != null && asunto.length > 33) {
                    enlaceTitulo.text(asunto.substring(0, 30) + "...");
                }
            });
        }
    }
    else if ((typeof customizarListado != 'undefined')) {
        /* En los listados de Inevery, hay que ejecutar este script */
        customizarListado.init();
    }

    if ((typeof CompletadaCargaRecursos != 'undefined')) {
        CompletadaCargaRecursos();
    }

    /* enganchar vista listado-mosaico*/
    if (!vistaMapa && !vistaChart) {
        modoVisualizacionListados.init(pPanelID);
    }

    utilMapas.AjustarBotonesVisibilidad();

    //Si hay resultados mostramos la caja de b?squeda
    if ((document.getElementById('ListadoGenerico_panContenedor') != null || vistaMapa || vistaChart) && document.getElementById('ctl00_ctl00_CPH1_CPHContenido_divCajaBusqueda') != null) {
        document.getElementById('ctl00_ctl00_CPH1_CPHContenido_divCajaBusqueda').style.display = "";
    }

    if (typeof scriptBusqueda != 'undefined') {
        if (scriptBusqueda != null && scriptBusqueda != "") {
            eval(scriptBusqueda);
        }
    }

    if (pNumeroResultados == 1 && funcionExtraResultados != "") {
        eval(funcionExtraResultados);
    }

    if (typeof (urlCargarAccionesRecursos) != 'undefined') {
        ObtenerAccionesListadoMVC(urlCargarAccionesRecursos);
    }
    MontarFechaCliente();
}

function TraerRecPuntoMapa(me, panelVerMas, claveLatLog, docIDs, docIDsExtra, panelVerMasX, panelVerMasY, tipo) {
    if (me == null) {
        $('#' + panelVerMas).html('<div><p>' + form.cargando + '...</p></div>');
    }

    var docIDsExtra = '';

    if (((docIDs.length + 1) / 37) > 10) {
        docIDsExtra = docIDs.substring(37 * 10)
        docIDs = docIDs.substring(0, 37 * 10);
    }

    var servicio = new WS($('input.inpt_UrlServicioResultados').val(), WSDataType.jsonp);

    var metodo = 'ObtenerFichaRecurso';
    var params = {};
    params['bool_usarMasterParaLectura'] = bool_usarMasterParaLectura;
    params['proyecto'] = $('input.inpt_proyID').val();
    params['bool_esMyGnoss'] = $('input.inpt_bool_esMyGnoss').val() == 'True';
    params['bool_estaEnProyecto'] = $('input.inpt_bool_estaEnProyecto').val() == 'True';
    params['bool_esUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
    params['identidad'] = $('input.inpt_identidadID').val();
    params['parametros'] = '';
    params['languageCode'] = $('input.inpt_Idioma').val();
    params['tipoBusqueda'] = tipoBusqeda;
    params['filtroContexto'] = filtroContexto;
    params['parametros_adiccionales'] = parametros_adiccionales;
    params['cont'] = contResultados;
    params['documentoID'] = docIDs;


    servicio.call(metodo, params, function (data) {
        if (docIDsExtra != '') {
            var idRecMas = guidGenerator();
            data += '<div id="mapaMasRec_' + idRecMas + '" class="verMasMapa"><a href="javascript:TraerRecPuntoMapa(null, \'mapaMasRec_' + idRecMas + '\', \'' + claveLatLog + '\',\'' + docIDsExtra + '\')">' + form.vermas + '</a></div>';
        }

        if (me != null) {
            me.infowindow.setContent(data);
            $('.fichaMapa').parent().css('overflow', 'hidden');
        }
        else {
            $('#' + panelVerMas).html(data);
            if (panelVerMasX != null && panelVerMasY != null) {
                $('#' + panelVerMas).removeAttr('class');
                if (tipo != null) {
		   
                    $('#' + panelVerMas).addClass(tipo);
                }

                $('#' + panelVerMas).attr('posx', panelVerMasX);
                $('#' + panelVerMas).attr('posy', panelVerMasY);

                $('#' + panelVerMas).css("top", '0px');
                $('#' + panelVerMas).css("left", '0px');

                ReposicionarFichaMapa();

                if ($('#' + panelVerMas).attr('activo') == 'true') {
                    $('#' + panelVerMas).show();
                }

                if ((typeof CompletadaCargaFichaMapaComunidad != 'undefined')) {
                    CompletadaCargaFichaMapaComunidad();
                }
            }
        }
        limpiarActividadRecienteHome.init();
    });
}

function ReposicionarFichaMapa() {
    var fichaMapa = $('#' + utilMapas.fichaMapa);
    var posX = 0;
    var posY = 0;
    posX = parseFloat(fichaMapa.attr('posx'));
    posY = parseFloat(fichaMapa.attr('posy'));

    //Anchura y altura del panel que mostramos
    var anchura = fichaMapa.width();
    var altura = fichaMapa.height();

    //Anchura y altura de la ventana que estamos viendo
    var centroNavegadorVentanaX = $(window).width() / 2;
    var centroNavegadorVentanaY = $(window).height() / 2;

    //Coordenadas relativas punto
    var panelVerMasXRelativo = posX - $(window).scrollLeft();
    var panelVerMasYRelativo = posY - $(window).scrollTop();

    var margenX = 35;
    var margenY = 40;
    if (panelVerMasXRelativo > centroNavegadorVentanaX) {
        posX = posX - anchura - margenX;
        fichaMapa.addClass('indicaDerecha');
    } else {
        posX = posX + margenX;
        fichaMapa.addClass('indicaIzquierda');
    }

    if (panelVerMasYRelativo > centroNavegadorVentanaY) {
        posY = posY - altura + margenY;
        fichaMapa.addClass('indicaInferior');
    } else {
        posY = posY - margenY;
        fichaMapa.addClass('indicaSuperior');
    }

    fichaMapa.css("left", posX + 'px');
    fichaMapa.css("top", posY + 'px');
    fichaMapa.css("z-index", '99999');

}

function SeleccionarChart(pCharID, pJsChart) {
    $('.listView').attr('class', 'listView');
    $('.gridView').attr('class', 'gridView');
    $('.mapView').attr('class', 'mapView');
    $('.chartView').attr('class', 'chartView activeView');

    chartActivo = pCharID;
    jsChartActivo = pJsChart;

    FiltrarPorFacetas(ObtenerHash2());
}

function ExportarBusqueda(pExportacionID, pNombreExportacion, pFormatoExportacion) {
    if (pExportacionID != "" && pNombreExportacion != "" && $('#ParametrosExportacion').length > 0 && $('#FormExportarBusqueda').length > 0 && pFormatoExportacion != "") {
        $('#ParametrosExportacion').val(pExportacionID + '|' + pNombreExportacion + '|' + pFormatoExportacion);
        $('#FormExportarBusqueda').submit();
    }
}

function MontarFacetas(pFiltros, pPrimeraCarga, pNumeroFacetas, pPanelID, pFaceta, pTokenAfinidad) {
    pFiltros = pFiltros.replace(/&/g, '|');
    if (mostrarFacetas) {
        //contFacetas = contFacetas + 1;

        var verMas = false;

        if (pFaceta != null && pFaceta.indexOf('|vermas') != -1) {
            verMas = true;
            pFaceta = pFaceta.substring(0, pFaceta.lastIndexOf('|'));
        }

        var paramAdicional = parametros_adiccionales;

        if ($('li.mapView').attr('class') == "mapView activeView") {
            paramAdicional += 'busquedaTipoMapa=true';
        }

        var metodo = 'CargarFacetas';
        var params = {};


        params['pProyectoID'] = $('input.inpt_proyID').val();
        params['pEstaEnProyecto'] = $('input.inpt_bool_estaEnProyecto').val() == 'True';
        params['pEsUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
        params['pIdentidadID'] = $('input.inpt_identidadID').val();
        params['pParametros'] = '' + pFiltros.replace('#', '');
        params['pLanguageCode'] = $('input.inpt_Idioma').val();
        params['pPrimeraCarga'] = pPrimeraCarga.toString().toLowerCase() == "true";
        params['pAdministradorVeTodasPersonas'] = adminVePersonas.toString().toLowerCase() == "true";
        params['pTipoBusqueda'] = tipoBusqeda;
        params['pGrafo'] = grafo;
        params['pFiltroContexto'] = filtroContexto;
        if (typeof urlBusqueda != "undefined") {
            params['pUrlPaginaActual'] = urlBusqueda;
        }
        params['pParametros_adiccionales'] = paramAdicional;
        params['pUbicacionBusqueda'] = ubicacionBusqueda;
        params['pNumeroFacetas'] = pNumeroFacetas;
        params['pUsarMasterParaLectura'] = bool_usarMasterParaLectura;
        params['pFaceta'] = pFaceta;
        params['tokenAfinidad'] = pTokenAfinidad;

        $.post(obtenerUrl($('input.inpt_UrlServicioFacetas').val()) + "/" + metodo, params, function (data) {
            if (decodeURI(pFiltros) == decodeURI(filtrosPeticionActual) || pFiltros == replaceAll(filtrosPeticionActual, '&', '|') || (typeof filtrosDeInicio !== "undefined" && (pFiltros == filtrosDeInicio || pFiltros == replaceAll(filtrosDeInicio, '|', '&'))) /*|| pFiltros.indexOf(filtrosPeticionActual) >= 0 Esto hace que al quitar filtros se amontonen facetas*/) {
                if (pFaceta == null && (pNumeroFacetas == 1 || pNumeroFacetas == 2)) {
                    MontarFacetas(pFiltros, pPrimeraCarga, pNumeroFacetas + 1, pPanelID, pFaceta, pTokenAfinidad);
                }
                var descripcion = data;
                if (descripcion.indexOf("class=\"box") != -1 && document.getElementById('facetaEncuentra') != null) {
                    document.getElementById('facetaEncuentra').style.display = '';
                }
                $(pPanelID).show();

                if (pFaceta != null && data != "" && pNumeroFacetas == -1) {
                    var divFacetaID = pFaceta.replace(/\:/g, '_').replace(/\@/g, '-');
                    var divFacetaID_out = '#out_' + divFacetaID;
                    if ($(divFacetaID_out).length > 0) {
                        $(divFacetaID_out).show();
                    }
                    else {
                        //No existe el panel out_faceta. Inserto en el panel original el contenido del resultante. 
                        $('#' + divFacetaID).replaceWith(data);
                    }
                }

                if (pFaceta == null || pFaceta == '' || pNumeroFacetas == 1) {
					if( pNumeroFacetas == 1)
					{
						$('#' + panFacetas).html('');
					}
                    var panelFacetas = pPanelID;
                    if ($('#facetedSearch').length) {
                        panelFacetas = '#facetedSearch';
                        descripcion = $('<div>' + descripcion + '</div>').find("#facetedSearch").html();
                    }

                    if (pNumeroFacetas > 1 && $(".listadoAgrupado").length) {
                        panelFacetas = "#" + $(".listadoAgrupado").attr("aux");
                    }

                    if (pNumeroFacetas == 1) {
                        if (!descripcion.replace('<div id="facetedSearch" class="facetedSearch">', '').replace('</div>', '').replace('</div>', '').trim().startsWith('<div id="panelFiltros" style="display:none">')) {
                            $('#facetedSearch').css('display', 'block');
                        } else {
                            //Ocultamos el panel de facetas
                            if ($('#facetedSearch').length == 0) {
                                $('#facetedSearch').css('display', 'none')
                            }
                            $(panelFacetas).css('display', 'none')
                        }
                    }
                    $(panelFacetas).append(descripcion);

                    facetedSearch.init();
                }
                else {
                    //Si viene el parámetro pFaceta, se está rellenando una faceta, hay que sustituir el contenido anterior por el actual. 

                    if (verMas) {
                        descripcion = $('#' + divFacetaID, $(data.trim())).html();
                        var htmlVerMas = $('p.moreResults', $(pPanelID)).html();
                        if (typeof (htmlVerMas) != 'undefined') {
                            htmlVerMas = htmlVerMas.substring(0, htmlVerMas.indexOf('>') + 1) + form.vermenos + '</a>';
                            descripcion += '<p class="moreResults">' + htmlVerMas + '</p>';
                        }
                        if (descripcion == null) {
						 
                            descripcion = data.trim();
                        }
                    }

                    if (descripcion.indexOf('id="' + pPanelID.substr(1) + '"') != -1) {
                        //La descripción ya contiene el panel, lo sustituyo.
                        $(pPanelID).replaceWith(descripcion);
                    }
                    else {
                        $(pPanelID).html(descripcion);
                    }
                    facetedSearch.init();
                }

                if (pNumeroFacetas == 1) { MontarPanelFiltros(); }
                /* presentacion facetas */
                // Longitud facetas por CSS
		        // limiteLongitudFacetas.init();

                //}

                if (pNumeroFacetas == 3) {
                    if ($(".filtroFacetaFecha").length > 0) {
					 
                        $(".filtroFacetaFecha").datepicker();
                    }

                    if ($("div.divdatepicker").length > 0) {
                        IniciarFacetaCalendario();
                    }
                } else if (pNumeroFacetas == -1 && $("div.divdatepicker").length > 0) {
                    IniciarFacetaCalendario(pFiltros);
                }


                if (enlazarJavascriptFacetas) {
                    enlazarFacetasBusqueda();
                }
                else {
                    enlazarFacetasNoBusqueda();
                }

                if ((typeof CompletadaCargaFacetas != 'undefined')) {
                    /* En los listados de Inevery, hay que ejecutar este script */
                    CompletadaCargaFacetas();
                }

                if (funcionExtraFacetas != "") {
                    eval(funcionExtraFacetas);
                }
            }
        });
    }
    else {
        var col1 = document.getElementById('col01');
        if (col1 != null) {
            $('#col01').css('display', 'none');
            $('#col02').css('float', 'left');
        }
    }
    primeraCargaDeFacetas = false;
}

function IniciarFacetaCalendario(fechaInicioCalendario) {

    //Cogemos los eventos
    $("div.divdatepicker").each(function () {
        var events = new Array();
        var i = 0;
        $(this).parent().children('ul').children().each(function () {
            var fecha = $(this).children('a').attr('title');
            var filtro = $(this).children('a').attr('name');

            fecha = fecha.substring(fecha.indexOf('=') + 1);
            var agno = parseInt(fecha.substr(0, 4));
            var mes = parseInt(fecha.substr(4, 2)) - 1;
            var dia = parseInt(fecha.substr(6, 2));
            events[i] = { Title: filtro, Date: new Date(agno, mes, dia, 0, 0, 0, 0) };
            i++;
        });


        var changingDate = false;
        var weekSelected = false;

        //Iniciamos el datepicker
        $(this).datepicker({
            showWeek: true,
            beforeShowDay: function (date) {
                //Revisamos si coincide
                var result = [true, '', null];
                var matching = $.grep(events, function (event) {
                    return event.Date.valueOf() === date.valueOf();
                });

                if (matching.length) {
                    result = [true, 'css-class-to-highlight', ''];
                }

                return result;
            },
            onSelect: function (dateText) {
                if (!weekSelected) {
                    var date,
                        i = 0,
                        event = null;
                    var day = parseInt(dateText.substr(0, 2));
                    var month = parseInt(dateText.substr(3, 2)) - 1;
                    var year = parseInt(dateText.substr(6, 4));
                    selectedDate = new Date(year, month, day, 0, 0, 0, 0);

                    /* Determine if the user clicked an event: */
                    while (i < events.length && !event) {
                        date = events[i].Date;

                        if (selectedDate.valueOf() === date.valueOf()) {
                            event = events[i];
                        }
                        i++;
                    }
                    if (event != null) {
                        AgregarFaceta(event.Title + '|replace');
                    }
                }
            },
            onChangeMonthYear: function (year, month) {
                if (changingDate) {
                    return;
                }
                //Lamamos al servicio de facetas para que recargue los elementos del mes siguiente.
                VerFechasSiguienteMes($(this).attr('name'), $(this).attr('name').replace(':', '_'), year, month);
            }
        });

        if (typeof (fechaInicioCalendario) != 'undefined' && fechaInicioCalendario != '') {

            var filtrosArray = fechaInicioCalendario.split('|');
            var fecha = '';
            for (var i = 0; i < filtrosArray.length; i++) {
                if (filtrosArray[i] != '' && filtrosArray[i].indexOf($(this).attr('name')) >= 0) {
                    fecha = filtrosArray[i].substr($(this).attr('name'));
                }
            }

            fecha = fecha.substr(fecha.indexOf('=') + 1) + '00';

            var agno = fecha.substr(0, 4);
            var mes = fecha.substr(4, 2);
            var dia = fecha.substr(6, 2);
            var inicio = new Date(agno, mes, dia, 0, 0, 0, 0);

            changingDate = true;
            $(this).datepicker("setDate", inicio);
            changingDate = false;
        }

        //Cargar elementos siguientes
        //Dar valor a los nuevas columnas
        $("td.ui-datepicker-week-col").on("click", function () {
            weekSelected = true;

            //Me filtra por el d?a al que le hago click...
            $(this).next().click();

            weekSelected = false;

            var fromDate = $("div.divdatepicker").datepicker("getDate");
            var toDate = $("div.divdatepicker").datepicker("getDate");
            toDate.setDate(toDate.getDate() + 6);

            var monthFrom = fromDate.getMonth() + 1;
            var monthTo = toDate.getMonth() + 1;
            if (monthFrom <= 9) {
                monthFrom = '0' + monthFrom;
            }
            if (monthTo <= 9) {
                monthTo = '0' + monthTo;
            }

            var dateFrom = fromDate.getDate();
            var dateTo = toDate.getDate();
            if (dateFrom <= 9) {
                dateFrom = '0' + dateFrom;
            }

            if (dateTo <= 9) {
                dateTo = '0' + dateTo;
            }

            var filtroFrom = fromDate.getFullYear() + '' + monthFrom + '' + dateFrom;
            var filtroTo = toDate.getFullYear() + '' + monthTo + '' + dateTo;
            var filtroSemantico = $("div.divdatepicker").attr('name');
            AgregarFaceta(filtroSemantico + '=' + filtroFrom + '-' + filtroTo + '|replace');
        });

        $('div.ui-datepicker-title').on('click', function () {
            var date = $("div.divdatepicker").datepicker("getDate");
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var dia = new Date(year, month, 0).getDate();

            if (month <= 9) {
                month = '0' + month;
            }
            if (dia <= 9) {
                dia = '0' + dia;
            }

            var filtro = year + '' + month + '01' + "-" + year + '' + month + '' + dia;
            var filtroSemantico = $("div.divdatepicker").attr('name');
            AgregarFaceta(filtroSemantico + '=' + filtro + '|replace');
        });

    });
}

function VerFechasSiguienteMes(faceta, controlID, year, month) {
    var filtros = ObtenerHash2();
    if (month <= 9) {
        month = '0' + month;
    }

    //Usamos el m?todo de limpiar filtro para limpiar el filtro que quramos.
    if (filtros.indexOf(faceta) >= 0) {
        filtros.split('&');
        var filtrosArray = filtros.split('&');
        filtros = '';

        for (var i = 0; i < filtrosArray.length; i++) {
            if (filtrosArray[i] != '' && filtrosArray[i].indexOf(faceta) == -1) {
                filtros += filtrosArray[i] + '&';
            } else if (filtrosArray[i] != '' && filtrosArray[i].indexOf('|') >= 0 && filtrosArray[i].indexOf(faceta) >= 0) {
                filtros += filtrosArray[i].substring(filtrosArray[i].indexOf('|'));
            }
        }
    }

    filtros += "|" + faceta + "=" + year + month;

    filtrosPeticionActual = "|" + faceta + "=" + year + month;

    MontarFacetas(filtros, false, -1, '#' + controlID, faceta + '|vermas');
}

function FinalizarMontarFacetas() {

    facetedSearch.init();

    MontarPanelFiltros();

    if (enlazarJavascriptFacetas) {
        enlazarFacetasBusqueda();
    }
    else {
        enlazarFacetasNoBusqueda();
    }

    if (HayFiltrosActivos(filtrosPeticionActual)) {
        $('#' + divFiltros).css('display', '');
        $('#' + divFiltros).css('padding-top', '0px !important');
        //$('#' + divFiltros).css('margin-top', '10px');
    }

    if ((typeof CompletadaCargaFacetas != 'undefined')) {
        /* En los listados de Inevery, hay que ejecutar este script */
        CompletadaCargaFacetas();
    }
}

$(document).ready(function () {
    /*$('.searchGroup .encontrar').click(function (event) {
        var txt = $(this).parent().find('.text');
        if ($(this).parent().find('#criterio').attr('origen') != undefined) {
            //Buscadores CMS
            var criterio = $(this).parent().find('#criterio').attr('origen');
            window.location.href = ObtenerUrlBusqueda(criterio) + txt.val();
            return false;
        }
        if (txt.hasClass('text') && txt.val() != '') {
            //Resto de buscadores
            window.location.href = $('input.inpt_baseUrlBusqueda').val() + '/recursos?search=' + txt.val();
            return false;
        }
        return false;
    });*/

    $('.searchGroup .text').keydown(function (event) {
        if ($(this).val().indexOf('|') > -1) {
            $(this).val($(this).val().replace(/\|/g, ''));
        };
        if (event.which || event.keyCode) {
            if ((event.which == 13) || (event.keyCode == 13)) {
                $(this).parent().find('.encontrar').click();
                return false;
            }
        } else {
            return true;
        };
    });

    $('.aaCabecera .searchGroup .text')
    .unbind()
    .keydown(function (event) {
        if ($(this).val().indexOf('|') > -1) {
            $(this).val($(this).val().replace(/\|/g, ''));
        };
        if (event.which || event.keyCode) {
            if ((event.which == 13) || (event.keyCode == 13)) {
                $(this).parent().find('.encontrar').click();
                return false;
            }
        } else {
            return true;
        };
    });

    $('.searchGroup .text').focus(function (event) {
        if ($(this).attr('class') == 'text defaultText') {
            $(this).attr('class', 'text');
            $(this).attr('value', '');
        }
    });

    //Buscador de la cabecera superior
    /*$('.searchGroup .encontrar')
    .unbind()
    .click(function (event) {
        var ddlCategorias = $('.fieldsetGroup .ddlCategorias').val();
        var url = ObtenerUrlBusqueda(ddlCategorias);
        if (typeof ($('.tipoBandeja').val()) != 'undefined' && ddlCategorias == 'Mensaje') {
            //Es una búsqueda en la bandeja de mensajes
            url = url.replace('search=', $('.tipoBandeja').val() + '&search=')
        }
        var parametros = $('.searchGroup .text').val();
        var autocompletar = $('.ac_results .ac_over');
        if (typeof (autocompletar) != 'undefined' && autocompletar.length > 0 && typeof ($('.ac_results .ac_over')[0].textContent) != 'undefined') {
		 
            parametros = $('.ac_results .ac_over')[0].textContent;
        }

        if (parametros == '') {
            url = url.replace('?search=', '').replace('/tag/', '');
        }
        window.location.href = url + parametros;
    });*/
});



$(document).ready(function () {
    if ($('#finderSection').length > 0) {
        var urlPaginaActual = $('.inpt_urlPaginaActual').val();
        if (typeof (urlPaginaActual) != 'undefined') {
            $('#inputLupa').click(function (event) {
                window.location.href = urlPaginaActual + "?search=" + encodeURIComponent($('#finderSection').val()); 
            });
        }

        $('#finderSection').keydown(function (event) {
            if ($(this).val().indexOf('|') > -1) {
                $(this).val($(this).val().replace(/\|/g, ''));
            };
            if (event.which || event.keyCode) {
                if ((event.which == 13) || (event.keyCode == 13)) {
                    $(this).parent().find('.findAction').click();
                    return false;
                }
            } else {
                return true;
            };
        });

        if (typeof (origenAutoCompletar) == 'undefined') {
            origenAutoCompletar = ObtenerOrigenAutoCompletarBusqueda($('input.inpt_tipoBusquedaAutoCompl').val());
            if (origenAutoCompletar == '') {
                var pathName = window.location.pathname;

                pathName = pathName.substr(pathName.lastIndexOf('/') + 1);
                if (pathName.indexOf('?') > 0) {
                    pathName = pathName.substr(0, pathName.indexOf('?'));
                }

                origenAutoCompletar = pathName;
            }
        }

        //var tablaPropiaAutoCompletar = $('input.inpt_tablaPropiaAutoCompletar').val().toLowerCase() == 'true';
        var urlServicioAutocompletar = $('.inpt_urlServicioAutocompletar').val();
        if (urlServicioAutocompletar.indexOf('autocompletarEtiquetas') > 0) {
            var proyID = $('.inpt_proyID').val();
            var facetasBusqPag = $('.inpt_facetasBusqPag').val();
            var identidadID = $('.inpt_identidadID').val();

            $('#finderSection').autocomplete(
			null,
			{
			    servicio: new WS(urlServicioAutocompletar, WSDataType.jsonp),
                metodo: 'AutoCompletarTipado',
                //url: urlServicioAutocompletar + "/AutoCompletarTipado",
                //type: "POST",
			    delay: 0,
			    scroll: false,
			    selectFirst: false,
			    minChars: 1,
			    width: 'auto',
			    max: 25,
			    cacheLength: 0,
			    extraParams: {
			        pProyecto: proyID,
			        //pTablaPropia: tablaPropiaAutoCompletar,
			        pFacetas: facetasBusqPag,
			        pOrigen: origenAutoCompletar,
			        pIdentidad: $('input.inpt_identidadID').val(),
			        pIdioma: $('input.inpt_Idioma').val(),
			        maxwidth: '420px',
			        botonBuscar: 'inputLupa'
			    }
			}
			);
        } else {
            var proyID = $('.inpt_proyID').val();
            var facetasBusqPag = $('.inpt_facetasBusqPag').val();
            var identidadID = $('.inpt_identidadID').val();
            var bool_esMyGnoss = $('.inpt_bool_esMyGnoss').val() == 'True';
            var bool_estaEnProyecto = $('.inpt_bool_estaEnProyecto').val() == 'True';
            var bool_esUsuarioInvitado = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
            var orgID = $('input.inpt_organizacionID').val();
            var perfilID = $('input#inpt_perfilID').val();
            var parametros = $('.inpt_parametros').val();
            var tipo = $('.inpt_tipoBusquedaAutoCompl').val();
            var facetasBusqPag = $('.inpt_facetasBusqPag').val();
            $('#finderSection').autocomplete(
				null,
				{
				    //servicio: new WS(urlServicioAutocompletar, WSDataType.jsonp),
				    //metodo: 'AutoCompletarFacetas',
				    url: urlServicioAutocompletar + "/AutoCompletarFacetas",
                    type: "POST",
				    delay: 0,
				    minLength: 4,
				    scroll: false,
				    selectFirst: false,
				    minChars: 4,
				    width: 190,
				    cacheLength: 0,
				    extraParams: {
				        proyecto: proyID,
				        bool_esMyGnoss: bool_esMyGnoss == true,
				        bool_estaEnProyecto: bool_estaEnProyecto == true,
				        bool_esUsuarioInvitado: bool_esUsuarioInvitado == true,
				        identidad: identidadID,
				        organizacion: orgID,
				        filtrosContexto: '',
				        languageCode: $('input.inpt_Idioma').val(),
				        perfil: perfilID,
				        //pTablaPropia: tablaPropiaAutoCompletar,
				        pFacetas: facetasBusqPag,
				        pOrigen: origenAutoCompletar,
				        nombreFaceta: 'search',
				        orden: '',
				        parametros: parametros,
				        tipo: tipo,
				        botonBuscar: 'inputLupa'
				    }
				}
			);
        }
    }
});

$(document).ready(function () {
    PreparaAutoCompletarComunidad();
});

function PreparaAutoCompletarComunidad() {
    $('input.autocompletar').each(function () {
        var txtBusqueda = this;
        var ddlCategorias = $(this).parent().parent().find('.ddlCategorias');
        var pOrigen = '';
        var pTipoDdlCategorias = '';
        var facetasAutoComTip = '';
        if ($(this).attr('origen') != undefined) {
            pTipoDdlCategorias = $(this).attr('origen');
            pOrigen = ObtenerOrigenAutoCompletarBusqueda($(this).attr('origen'));
            facetasAutoComTip = ObtenerFacetasAutocompletar($(this).attr('origen'));
        } else if (typeof (ddlCategorias.val()) != 'undefined' && ddlCategorias.val() != '') {
            pOrigen = ObtenerOrigenAutoCompletarBusqueda(ddlCategorias.val());
            facetasAutoComTip = ObtenerFacetasAutocompletar(ddlCategorias.val());
            pTipoDdlCategorias = ddlCategorias.val();
        } else if (typeof ($('input.inpt_tipoBusquedaAutoCompl').val()) != 'undefined') {
            if ($('input.inpt_tipoBusquedaAutoCompl').val() != '') {
                pOrigen = origenAutoCompletar;
                facetasAutoComTip = ObtenerFacetasAutocompletar($('input.inpt_tipoBusquedaAutoCompl').val());
            }
            pTipoDdlCategorias = $('input.inpt_tipoBusquedaAutoCompl').val();
        }

        //Este trozo hace que la caja de arriba de b?squeda intente buscar en la pesta?a donde est?s en vez de donde se indica. Adem?s de que a veces no se puede buscar en la pest? que te encuentras (Ej: Indice)
        /*if (pOrigen == '') {
            var pathName = window.location.pathname;

            pathName = pathName.substr(pathName.lastIndexOf('/') + 1);
            if (pathName.indexOf('?') > 0) {
                pathName = pathName.substr(0, pathName.indexOf('?'));
            }

            pOrigen = pathName;
        }*/

        var urlServicioAutocompletarEtiquetas = $('input.inpt_urlServicioAutocompletarEtiquetas').val();

        var identidadID = $('input.inpt_identidadID').val();
        var proyID = $('input.inpt_proyID').val();
        var organizacionID = $('input.inpt_organizacionID').val();
        var perfilID = $('input#inpt_perfilID').val();

        var btnBuscarID = '';
        // Size() deprecado
        //if ($(this).parent().find('.encontrar').size() > 0) {
          if ($(this).parent().find('.encontrar').length > 0) {
            btnBuscarID = $(this).parent().find('.encontrar').attr('id');
        } else {
            return;
        }

        var urlServicioAutocompletar = $('input.inpt_urlServicioAutocompletar').val();
        var bool_esMyGnoss = $('input.inpt_bool_esMyGnoss').val() == 'True';
        var bool_estaEnProyecto = $('input.inpt_bool_estaEnProyecto').val() == 'True';
        var bool_esUsuarioInvitado = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
        if (facetasAutoComTip == '') {
		 
            facetasAutoComTip = $('input.inpt_FacetasProyAutoCompBuscadorCom').val();
        }
        //var tablaPropiaAutoCompletar = $('input.inpt_tablaPropiaAutoCompletar').val().toLowerCase() == "true";
        var autocompletarProyectoVirtuoso = $('input.inpt_AutocompletarProyectoVirtuoso').val();

        $(this).unautocomplete();
        if (ddlCategorias.val() != 'MyGNOSSMeta' && ddlCategorias.val() != 'Contribuciones en Recursos' && ddlCategorias.val() != 'RecursoPerfilPersonal' && ddlCategorias.val() != 'Mensaje' && ddlCategorias.val() != 'Blog' && ddlCategorias.val() != 'Comunidad' && (ddlCategorias.val() != 'PerYOrg' || proyID != '11111111-1111-1111-1111-111111111111') && typeof (autocompletarProyectoVirtuoso) == 'undefined') {
            $(this).autocomplete(
                null,
                {
                    servicio: new WS(urlServicioAutocompletarEtiquetas, WSDataType.jsonp),
                    metodo: 'AutoCompletarTipado',
                    //url: urlServicioAutocompletarEtiquetas + "/AutoCompletarTipado",
                    //type: "POST",
                    delay: 0,
                    scroll: false,
                    selectFirst: false,
                    minChars: 1,
                    width: 'auto',
                    max: 25,
                    cacheLength: 0,
                    extraParams: {
                        pProyecto: proyID,
                        //pTablaPropia: tablaPropiaAutoCompletar,
                        pFacetas: facetasAutoComTip,
                        pOrigen: pOrigen,
                        pIdentidad: identidadID,
                        pIdioma: $('input.inpt_Idioma').val(),
                        maxwidth: '389px',
                        botonBuscar: btnBuscarID
                    }
                });
        } else {
            $(this).autocomplete(
			null,
			{
			    //servicio: new WS(urlServicioAutocompletar, WSDataType.jsonp),
			    //metodo: 'AutoCompletarFacetas',
			    url: urlServicioAutocompletar + "/AutoCompletarFacetas",
			    type: "POST",
			    delay: 0,
			    minLength: 4,
			    scroll: false,
			    selectFirst: false,
			    minChars: 4,
			    width: 190,
			    cacheLength: 0,
			    extraParams: {
			        proyecto: proyID,
			        bool_esMyGnoss: bool_esMyGnoss == 'True',
			        bool_estaEnProyecto: bool_estaEnProyecto == 'True',
			        bool_esUsuarioInvitado: bool_esUsuarioInvitado == 'True',
			        identidad: identidadID,
			        organizacion: organizacionID,
			        filtrosContexto: '',
			        languageCode: $('input.inpt_Idioma').val(),
			        perfil: perfilID,
			        nombreFaceta: 'search',
			        orden: '',
			        parametros: '',
			        tipo: pTipoDdlCategorias,
			        botonBuscar: btnBuscarID
			    }
			});
        }
    });
}

var wrap = '';

function ObtenerUrlBusqueda(tipo) {
    var tamagno = $('input.inpt_tipo_busqueda').length;
    for (var i = 0; i < tamagno; i++) {
        var valor = ($('input.inpt_tipo_busqueda')[i]).value;
        if (valor.startsWith("ub_")) {
            valor = valor.substring(3);
        }
        if (valor.split('@')[0] == tipo) {
            return valor.split('@')[1];
        }
    }

    //Devolvemos el por defecto.
    return ($('input.inpt_tipo_busqueda')[tamagno - 1]).value.split('|')[1];
}

function ObtenerOrigenAutoCompletarBusqueda(tipo) {
    var tamagno = $('input.inpt_OrigenAutocompletar').length;
    for (var i = 0; i < tamagno; i++) {
        var valor = ($('input.inpt_OrigenAutocompletar')[i]).value;
        if (valor.startsWith("oa_")) {
            valor = valor.substring(3);
        }
        if (valor.split('@')[0] == tipo) {
            return valor.replace(tipo + '@', '');
        }
    }

    //Devolvemos el por defecto.
    return '';
}

function ObtenerFacetasAutocompletar(tipo) {
    var tamagno = $('input.inpt_FacetasAutocompletar').length;
    for (var i = 0; i < tamagno; i++) {
        var valor = ($('input.inpt_FacetasAutocompletar')[i]).value;
        if (valor.startsWith("fa_")) {
            valor = valor.substring(3);
        }
        if (valor.split('@')[0] == tipo) {
            return valor.replace(tipo + '@', '');
        }
    }

    //Devolvemos el por defecto.
    return '';
}

function MontarPanelFiltros() {
    if (mostrarCajaBusqueda) {
        if (document.getElementById('panelFiltros') != null && document.getElementById('panelFiltros').innerHTML != "" && document.getElementById(panFiltrosPulgarcito) != null) {
            $('#' + panFiltrosPulgarcito).html($('#panelFiltros').html());
            $('#panelFiltros').html('');

            $('.group.filterSpace').css('display', '');
            $('.searchBy').css('display', '');
            $('.tags').css('display', '');
        }
    }
}

function MontarNumResultados() {
    if ($('#' + idNavegadorBusqueda).length > 0) {
        if ($('#navegadorRemover').find('.indiceNavegacion').length > 0) {
            $('#' + idNavegadorBusqueda).html($('#navegadorRemover').html());
            $('#' + idNavegadorBusqueda).css('display', '')
        }
        $('#navegadorRemover').remove();
    }

    if ($('#numResultadosRemover').length > 0) {
        if (mostrarCajaBusqueda) {
            $('#' + numResultadosBusq).html($('#numResultadosRemover').html());
            $('#' + numResultadosBusq).css('display', '');
            $('.group.filterSpace').css('display', '');
        }
        $('#numResultadosRemover').remove();
    }
}

//M?todos para las acciones de los listados de las b?squedas
function ObtenerAccionesListado(jsEjecutar) {
    var resources = $('.resource-list .resource');
    var idPanelesAcciones = '';
    var numDoc = 0;
    resources.each(function () {
        var recurso = $(this);
        var accion = recurso.find('.group.acciones.noGridView');
        if (accion.length == 1) {
            accion.attr('id', accion.attr('id') + '_' + numDoc);
            idPanelesAcciones += accion.attr('id') + ',';
            numDoc++;
        }
        var accionesusuario = recurso.find('.group.accionesusuario.noGridView');
        if (accionesusuario.length == 1) {
            accionesusuario.attr('id', accionesusuario.attr('id') + '_' + numDoc);
            idPanelesAcciones += accionesusuario.attr('id') + ',';
            numDoc++;
        }
    });

    if (jsEjecutar == null) {
	 
        jsEjecutar = "";
    }

    if (idPanelesAcciones != '') {
        try {
		 
            WebForm_DoCallback(UniqueDesplegarID, 'CargarControlDesplegar&ObtenerAcciones&' + idPanelesAcciones + "&jsEjecutar=" + jsEjecutar, ReceiveServerData, '', null, false);
        }
        catch (ex) { }
    } else if (jsEjecutar != null) {
        eval(jsEjecutar);
    }
}

function AccionListadoActivarDesactivar(boton) {
    if (boton.parent().attr('class') == null || boton.parent().attr('class') == '') {
        boton.parent().parent().children().each(function () {
            $(this).attr('class', '');
        });
        boton.parent().attr('class', 'active');
    } else {
        boton.parent().attr('class', '');
    }
}

function AccionListadoCambiarOnClickPorOnclickAux(boton) {
    boton.attr('onclickAux2', $(this).attr('onclick'));
    boton.attr('onclick', $(this).attr('onclickAux'));
    boton.attr('onclickAux', $(this).attr('onclickAux2'));
    boton.removeAttr('onclickAux2');
}


function FormatearFacetasGraficas() {
    //Componentes
    var facetasBarras = $('.componenteFaceta.graficoBarras');
    facetasBarras.each(function () {
        if (!$(this).hasClass("formateado")) {
            //Recorremos cada faceta
            var faceta = $(this);
            PintarGraficoFaceta(faceta, 'barras');
            faceta.addClass("formateado");
        }
    });

    var facetasSectores = $('.componenteFaceta.graficoSectores');
    facetasSectores.each(function () {
        if (!$(this).hasClass("formateado")) {
            //Recorremos cada faceta
            var faceta = $(this);
            PintarGraficoFaceta(faceta, 'sectores');
            faceta.addClass("formateado");
        }
    });
}

function PintarGraficoFaceta(faceta, estilo) {
    faceta.hide();
    var listaElementos = faceta.find('.facetedSearch ul li');
    var titulo = faceta.find('.faceta-title').text();

    var array = new Array();
    array[0] = new Array();
    array[0][0] = 'Propiedad';
    array[0][1] = 'Número';

    var num = 1;

    var arrayEnlaces = new Array();
    var total = 0;
    listaElementos.each(function () {
        //Recorremos cada elemento de la faceta
        var elemento = $(this);

        var enlace = elemento.find('a').attr('href');
        var nombreSinFormato = elemento.find('a').text();

        var nombre = nombreSinFormato.substring(0, nombreSinFormato.indexOf('(')).trim();
        var numero = nombreSinFormato.substring(nombreSinFormato.indexOf('(') + 1);
        numero = numero.substring(0, numero.indexOf(')'));

        array[num] = new Array();
        array[num][0] = nombre;
        array[num][1] = parseFloat(numero);
        arrayEnlaces[num - 1] = enlace;
        num++;
    });

    google.load("visualization", '1.1', { packages: ["corechart"] });
    google.setOnLoadCallback(drawChart);
    function drawChart() {
        var data = google.visualization.arrayToDataTable(array);

        var view = new google.visualization.DataView(data);
        view.setColumns([0, 1,
                    {
                        calc: "stringify",
                        sourceColumn: 1,
                        type: "string",
                        role: "annotation"
                    }]);

        var options = {
            title: titulo
        };

        var idGrafico = guidGenerator();
        faceta.html("<div id='div_cms_faceta" + idGrafico + "'></div>");
        var chart = null;

        if (estilo == 'barras') {
            chart = new google.visualization.BarChart(document.getElementById('div_cms_faceta' + idGrafico));
            options = {
                title: titulo,
                legend: { position: 'none' }
            };
        } else if (estilo == 'sectores') {
            chart = new google.visualization.PieChart(document.getElementById('div_cms_faceta' + idGrafico));
        }
        faceta.show();
        chart.draw(view, options);
        google.visualization.events.addListener(chart, 'select', function () {
            var enlace = '';
            var selection = chart.getSelection();
            for (var i = 0; i < selection.length; i++) {
                var item = selection[i];
                if (estilo == 'barras') {
                    if (item.row != null && item.column != null) {
                        if (item.column == '1') {
                            enlace = arrayEnlaces[item.row];
                        }
                    }
                } else if (estilo == 'sectores') {
                    enlace = arrayEnlaces[item.row];
                }
            }
            if (enlace != '') {
                document.location.href = enlace;
            }
        });
    }
}

function obtenerUrl(service) {
    var url = service;
    if (url.indexOf(',') != -1) {
        var urlMultiple = url.split(',');
        if (indicesWS[service] == null) {
		 
            indicesWS[service] = null;
        }
        if (indicesWS[service] == null) {
		 
            indicesWS[service] = this.aleatorio(0, urlMultiple.length - 1);
        } else if (indicesWS[service] > urlMultiple.length - 1) {
		 
            indicesWS[service] = 0;
        }
        url = urlMultiple[indicesWS[service]];
        indicesWS[service]++;
    }
    return url;
}

function aleatorio(inferior, superior) {
    numPosibilidades = superior - inferior;
    aleat = Math.random() * numPosibilidades;
    aleat = Math.round(aleat);
    return parseInt(inferior) + aleat;
}/**/ 
/*opcionesDestacadasMenuLateral.js*/ 
var opcionesDestacadasMenuLateral = {
	id: '#usuarioConectadoComunidades',
	idMenuSuperior: '#menuSuperiorComunidades',
	idPanelDesplegable: 'panelDesplegableOpciones',
	cssItem: '.item',
	cssPanel: '.opcionesPanel',
	cssOpcionPrincipal: '.opcionPrincipal',
	cssItemConOpciones: 'itemConOpcionesSegundoNivel',
	idItemOtrasIdentidades: '#otrasIdentidades',
	idIdentidad: '#identidad',
	paneles: [],
	cssActivo: 'activo',
	cssActivoOtras: 'activoOtras',
	timeoutOcultarMenu: '',
	ocultarMenu: false,
	init: function(){
		this.id = $(this.id);
		if(this.id.size() <= 0) return;
		this.crearPanelDesplegable();
		this.config();
		this.customizar();
		this.marcarOpcionesPrincipales();
		this.engancharComportamiento();
		this.engancharOtrasIdentidades();
		return;
	},
	config: function(){
		this.menuSuperior = this.id.find(this.idMenuSuperior);
		this.panelDesplegable = this.id.find('#' + this.idPanelDesplegable);
		this.buscador = $('#cabecera .busqueda');
		this.items= this.id.find(this.cssItem);
		this.opcionesPrincipales = this.id.find(this.cssOpcionPrincipal);
		this.otrasIdentidades = $(this.idItemOtrasIdentidades);
		this.enlaceOtrasIdentidades = this.otrasIdentidades.find('a');
		this.identidades = $(this.idIdentidad);
		return;
	},
	numeroTabs: function(){
		var that = this;
		this.itemsIdentidad = [];
		this.identidades.find('li').each(function(){
			var item = $(this);
			if(item.parents('ul.infoCuenta').size() <= 0){
				that.itemsIdentidad.push(item);
			};
		});
		return this.itemsIdentidad.length;
	},
	customizar: function(){
		var tabs = this.numeroTabs();
		if(tabs == 1) {
			var item = this.itemsIdentidad[0];
			$(item).addClass('identidadUnica');
		}else if(tabs > 1 && tabs <= 4){
			var item = this.itemsIdentidad[1];
			$(item).addClass('activoAnterior');
			item = this.itemsIdentidad[tabs - 1];
			$(item).addClass('ultimoItem');
		}else{
			var item = this.itemsIdentidad[1];
			$(item).addClass('activoAnterior');			
		}
		return;
	},
	crearPanelDesplegable: function(){
		this.id.append('<div id=\"' + this.idPanelDesplegable + '\"><\/div>\n');
		return;
	},
	marcarOpcionesPrincipales: function(){
		var that = this;
		this.items.each(function(){
			var item = $(this);
			var panel = item.find(that.cssPanel);
			if(panel.size() > 0){
				item.addClass(that.cssItemConOpciones);
				that.paneles.push(panel.html());
			} 
		});
		return;
	},
	plantilla: function(numero){
		var html = '';
		html += '<div class=\"opcionesPanel\">\n';
		html += this.paneles[numero];
		html += '<\/div>\n';
		return html;
	},
	mostrarOpcionesItem: function(numero){
		this.panelDesplegable.html(this.plantilla(numero));
		this.panelDesplegable.children('.opcionesPanel').hide();
		$(this.panelDesplegable.children('.opcionesPanel')).slideToggle(1000, function() {
		    this.timeoutOcultarMenu = setTimeout(function() {
                if(this.ocultarMenu)
                {
                    this.ocultarOpcionesItem(parent, that.cssActivo);
                    this.ocultarMenu = false;
                }
            }, 600);
            this.ocultarMenu = true;
		});
		return;
	},
	ocultarOpcionesItem: function(){
	    var that = this;
		$(this.panelDesplegable.children('.opcionesPanel')).slideToggle(1000, function() {
		    that.panelDesplegable.html('');
		});
		return;
	},
	desmarcarOpcionesItem: function(){
		var that = this;
		this.opcionesPrincipales.each(function(){
			$(this).removeClass(that.cssActivo);
		});
		return;
	},
	engancharOtrasIdentidades: function(){
		var that = this;
		var contenidoListadoOtrasIdentidades = this.otrasIdentidades.find('.listadoOtrasIdentidades');
		
		var enlace = that.enlaceOtrasIdentidades;
		var parent = enlace.parent().parent();
		var desplegable = that.panelDesplegable;
			
		//Al entrar en el enlace
		enlace.hover(function() {
		    if(parent.hasClass(that.cssActivoOtras))
		    {
		        clearTimeout(that.timeoutOcultarMenu);
		    }
		},
		//Al salir del enlace
		function() {
		    if(parent.hasClass(that.cssActivoOtras))
		    {
		        that.timeoutOcultarMenu = setTimeout(
		            function() {
		                desplegable.children('.opcionesPanel').slideUp(800, function() {parent.removeClass(that.cssActivoOtras);desplegable.removeClass('desplegarOtrasIdentidades');});
		            }
		         , 600);
		    }
		});	
		
		//Al entrar en el men� desplegado
        desplegable.hover(function() {
            if(parent.hasClass(that.cssActivoOtras))
            {
                clearTimeout(that.timeoutOcultarMenu);
            }
        },
        //Al salir del men� desplegado
        function() {
            if(parent.hasClass(that.cssActivoOtras))
            {
                that.timeoutOcultarMenu = setTimeout(
                    function() {
                        desplegable.children('.opcionesPanel').slideUp(800, function() {parent.removeClass(that.cssActivoOtras);});
                    }
                , 600);
            }
        });
		
		enlace.click(function() {
		    desplegable.children('.opcionesPanel').stop(true);
    	    
	        //Si est� desplegado
	        if(parent.hasClass(that.cssActivoOtras))
	        {
	            desplegable.children('.opcionesPanel').slideUp(800, function() {parent.removeClass(that.cssActivoOtras);desplegable.removeClass('desplegarOtrasIdentidades');});
	        }
	        //Si no est� desplegado
	        else
	        {
	            //Quitamos el timeout y las clases 'activo' que pueda haber
		        clearTimeout(that.timeoutOcultarMenu);
	            that.id.find('.' + that.cssItemConOpciones + ' ' + that.cssOpcionPrincipal + ' a').each(function(){
	                var enlace = $(this);
		            var parent = enlace.parent();
		            parent.removeClass(that.cssActivo);
	            });
				desplegable.addClass('desplegarOtrasIdentidades');
                
	            //Generamos el contenido del panel desplegable, y lo mostramos
	            parent.addClass(that.cssActivoOtras);
	            var html = '<div class=\"opcionesPanel\">\n' + $(contenidoListadoOtrasIdentidades).html() + '<\/div>\n';
	            desplegable.html(html);
	            desplegable.children('.opcionesPanel').hide();
	            desplegable.css('left',parent.position().left);
	            desplegable.children('.opcionesPanel').slideDown(800);
	        }
	        return false;
		});
		return;
	},
	engancharComportamiento: function(){
		var that = this;
		//Recorremos todos los enlaces del men� y les a�adimos los comportamientos
		this.id.find('.' + this.cssItemConOpciones + ' ' + this.cssOpcionPrincipal + ' a').each(function(indice){
			var enlace = $(this);
			var parent = enlace.parent();
			var desplegable = that.panelDesplegable;
			
			//Al entrar en el enlace
			enlace.hover(function() {
			    if(parent.hasClass(that.cssActivo))
			    {
			        clearTimeout(that.timeoutOcultarMenu);
			    }
			},
			//Al salir del enlace
			function() {
			    if(parent.hasClass(that.cssActivo))
			    {
			        that.timeoutOcultarMenu = setTimeout(
			            function() {
			                parent.removeClass(that.cssActivo);
			                desplegable.children('.opcionesPanel').slideUp(800);
			            }
			        , 600);
			    }
			});
			
			//Al entrar en el men� desplegado
	        desplegable.hover(function() {
	            if(parent.hasClass(that.cssActivo))
	            {
	                clearTimeout(that.timeoutOcultarMenu);
	            }
	        },
	        //Al salir del men� desplegado
	        function() {
	            if(parent.hasClass(that.cssActivo))
	            {
	                that.timeoutOcultarMenu = setTimeout(
	                    function() {
	                        parent.removeClass(that.cssActivo);
	                        desplegable.children('.opcionesPanel').slideUp(800);
	                    }
	                , 600);
	            }
	        });
			
			//Al hacer click en el enlace
			enlace.click(function() {
			    desplegable.children('.opcionesPanel').stop(true);
			    that.enlaceOtrasIdentidades.parent().parent().removeClass(that.cssActivoOtras);
			    desplegable.removeClass('desplegarOtrasIdentidades');
			    
			    //Si est� desplegado
			    if(parent.hasClass(that.cssActivo))
			    {
			        parent.removeClass(that.cssActivo);
			        desplegable.children('.opcionesPanel').slideUp(800);
			    }
			    //Si no est� desplegado
			    else
			    {
			        //Quitamos el timeout y las clases 'activo' que pueda haber
			        clearTimeout(that.timeoutOcultarMenu);
		            that.id.find('.' + that.cssItemConOpciones + ' ' + that.cssOpcionPrincipal + ' a').each(function(){
		                var enlace = $(this);
			            var parent = enlace.parent();
			            parent.removeClass(that.cssActivo);
		            });
		            
			        //Generamos el contenido del panel desplegable, y lo mostramos
			        parent.addClass(that.cssActivo);
			        var html = '<div class=\"opcionesPanel\">\n' + parent.parent().children('.opcionesPanel').html() + '<\/div>\n';
			        desplegable.html(html);
			        desplegable.children('.opcionesPanel').hide();
			        desplegable.css('left',enlace.position().left);
			        desplegable.children('.opcionesPanel').slideDown(800);
			    }
			    return false;
			});
		})
		return;
	}
}
var reemplazarEncabezados = {
    cssGroup: '.group',
    init: function(){
        this.config();
        this.reemplazar();  
    },
    config: function(){
        this.encabezado = subname;
        this.group = $('#section ' + this.cssGroup);
        this.groupFirst = this.group[0];
        this.title = $('h2', this.groupFirst);
    },
    template: function(encabezado){
        var html = '';
        html = '<span class=\"subname\">';
        html += encabezado;
        html += '<\/span>';
        return html;
    },
    reemplazar: function(){
        var encabezado = this.encabezado.text();
        this.title.append(this.template(encabezado));
    }
};
var subname;
$(function(){
	//opcionesDestacadasMenuLateral.init();
	$('#perfilUsuarioGnossCargando').hide();
	$('#perfilUsuarioGnoss').show();
	
	subname = $('h2#subname');
	// Deprecado size()
    //if (subname.size() > 0) reemplazarEncabezados.init();
    if (subname.legnth > 0) reemplazarEncabezados.init();
})/**/ 
/*WS.js*/ 
﻿WSDataType = { json: "json", jsonp: "jsonp" };

WS = function(service, dataType) {
    this.service = service;
    if (dataType)
        this.dataType = dataType;
};

var indicesWS = {};
WS.prototype = {
    dataType: WSDataType.json,
    service: null,
    call: function(pMethod, pArgs, pCallback, pError) {
        var url = null;
        var service = this.service;
        service = this.obtenerUrl(service);
        if (service[service.length - 1] != "/") service += "/";
        if (this.dataType == WSDataType.jsonp) {
            url = $.jmsajaxurl({
                url: service,
                method: pMethod,
                data: pArgs != null ? pArgs : {}
            });
        } else {
            url = service + pMethod;
        }
        $.ajax({
            type: this.dataType == WSDataType.json ? "POST" : "POST",
            url: url + ((this.dataType == WSDataType.jsonp) ? "&format=json" : ""),
            data: ((this.dataType == WSDataType.json) ? JSON.stringify(pArgs) : ""),
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: this.dataType
        })
        .done(function (response) {          
            if (pCallback)
            {
                if(response.d != null){
                    pCallback(response.d);
                }
                else{
                    pCallback(response);
                }
            }
        })
        .fail(function (data) {
            if (pError) {
                pError();
            }
        });
    },
    obtenerUrl: function (service) {
        var url = service;
        if (url.indexOf(',') != -1) {
            var urlMultiple = url.split(',');
            if (indicesWS[service] == null)
            {
                indicesWS[service] = null;
            }
            if (indicesWS[service] == null)
            {
                indicesWS[service] = this.aleatorio(0, urlMultiple.length - 1);
            } else if (indicesWS[service] > urlMultiple.length - 1)
            {
                indicesWS[service] = 0;
            }
            url = urlMultiple[indicesWS[service]];
            indicesWS[service]++;
        }
        return url;
    },
    aleatorio: function (inferior, superior) {
        numPosibilidades = superior - inferior;
        aleat = Math.random() * numPosibilidades;
        aleat = Math.round(aleat);
        return parseInt(inferior) + aleat;
    }
};/**/ 
/*listToSelect.jquery.js*/ 
/*
 * list to select  beta
 *
 * Copyright (c) 2011 felix tuesta
 *
 * Date: 14 / 02 / 2011
 * Library: jQuery
 * 
 */
(function($){
	$.fn.listToSelect = function(options){
		var defaults = { 
			start: -1,
			cssListaPlegada: 'plegada'			
		};
		var options = $.extend(defaults, options);  

		var componente = $(this);
		var lista = $('ul', componente);
		var boton = null;        

		return this.each(function(){
			initialize();
		});
		function initialize(){
			boton = $('p.desplegar a', componente);	
			comportamientoBotonDesplegar();			
		}
		
		function plegarDesplegarLista(){
			lista.hasClass(options.cssListaPlegada) ? lista.removeClass(options.cssListaPlegada) : lista.addClass(options.cssListaPlegada) ;
		}
		function comportamientoBotonDesplegar(){
			boton.bind('click', function(){		
			    plegarDesplegarLista();	
				this.blur();
				return false;
			})
		}
	};
})(jQuery);/**/ 
/*jMsAjax.js*/ 
/*
* jMsAjax 0.2.2 - Microsoft Ajax jQuery Plugin
*
* Copyright (c) 2008 Adam Schröder (schotime.net)
* Dual licensed under the MIT (MIT-LICENSE.txt)
* and GPL (GPL-LICENSE.txt) licenses.
*
* $Date: 2008-07-12 $
*/
(function($) {
    $.jmsajaxurl = function(options) {
        var url = options.url;
        if (url[url.length - 1] != "/") url += "/";
        url += options.method; if (options.data) {

            var data = ""; for (var i in options.data) {
                if (data != "")
                    data += "&"; data += i + "=" + msJSON.stringify(options.data[i]);
            }
            url += "?" + data; data = null; options.data = "{}";
        }
        return url;
    };
    $.jmsajax = function(options) {
        var defaults = { type: "POST", dataType: "msjson", data: {}, beforeSend: function(xhr) { xhr.setRequestHeader("Content-type", "application/json; charset=utf-8"); }, contentType: "application/json; charset=utf-8", error: function(x, s, m) { alert("Status: " + ((x.statusText) ? x.statusText : "Unknown") + "\nMessage: " + msJSON.parse(((x.responseText) ? x.responseText : "Unknown")).Message); } }; var options = $.extend(defaults, options); if (options.method)
            options.url += "/" + options.method; if (options.data) {
            if (options.type == "GET") {
                var data = ""; for (var i in options.data) {
                    if (data != "")
                        data += "&"; data += i + "=" + msJSON.stringify(options.data[i]);
                }
                options.url += "?" + data; data = null; options.data = "{}";
            }
            else if (options.type == "POST")
            { options.data = msJSON.stringify(options.data); }
        }
        if (options.success) {
            if (options.dataType) {
                if (options.dataType == "msjson") {
                    var base = options.success; options.success = function(response, status) {
                        var y = dateparse(response); if (options.version) {
                            if (options.version >= 3.5)
                                y = y.d;
                        }
                        else {
                            if (response.indexOf("{\"d\":") == 0)
                                y = y.d;
                        }
                        base(y, status);
                    }
                }
            }
        }
        return $.ajax(options);
    }; dateparse = function(data) {
        try {
            return msJSON.parse(data, function(key, value) {
                var a; if (typeof value === "string") {
                    if (value.indexOf("Date") >= 0)
                    { a = /^\/Date\((-?[0-9]+)\)\/$/.exec(value); if (a) { return new Date(parseInt(a[1], 10)); } }
                }
                return value;
            });
        }
        catch (e) { return null; }
    }
    msJSON = function() {
        function f(n) { return n < 10 ? '0' + n : n; }
        //Date.prototype.toJSON=function(key){return this.getUTCFullYear()+'-'+f(this.getUTCMonth()+1)+'-'+f(this.getUTCDate())+'T'+f(this.getUTCHours())+':'+f(this.getUTCMinutes())+':'+f(this.getUTCSeconds())+'Z';};
        var cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, escapeable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, gap, indent, meta = { '\b': '\\b', '\t': '\\t', '\n': '\\n', '\f': '\\f', '\r': '\\r', '"': '\\"', '\\': '\\\\' }, rep; function quote(string) {
            escapeable.lastIndex = 0; return escapeable.test(string) ? '"' + string.replace(escapeable, function(a) {
                var c = meta[a]; if (typeof c === 'string') { return c; }
                return '\\u' + ('0000' +
	(+(a.charCodeAt(0))).toString(16)).slice(-4);
            }) + '"' : '"' + string + '"';
        }
        function str(key, holder) {
            var i, k, v, length, mind = gap, partial, value = holder[key]; if (value && typeof value === 'object' && typeof value.toJSON === 'function') { value = value.toJSON(key); }
            if (typeof rep === 'function') { value = rep.call(holder, key, value); }
            switch (typeof value) {
                case 'string': return quote(value); case 'number': return isFinite(value) ? String(value) : 'null'; case 'boolean': case 'null': return String(value); case 'object': if (!value) { return 'null'; }
                    if (value.toUTCString) { return '"\\/Date(' + (value.getTime()) + ')\\/"'; }
                    gap += indent; partial = []; if (typeof value.length === 'number' && !(value.propertyIsEnumerable('length'))) {
                        length = value.length; for (i = 0; i < length; i += 1) { partial[i] = str(i, value) || 'null'; }
                        v = partial.length === 0 ? '[]' : gap ? '[\n' + gap +
	partial.join(',\n' + gap) + '\n' +
	mind + ']' : '[' + partial.join(',') + ']'; gap = mind; return v;
                    }
                    if (rep && typeof rep === 'object') { length = rep.length; for (i = 0; i < length; i += 1) { k = rep[i]; if (typeof k === 'string') { v = str(k, value, rep); if (v) { partial.push(quote(k) + (gap ? ': ' : ':') + v); } } } } else { for (k in value) { if (Object.hasOwnProperty.call(value, k)) { v = str(k, value, rep); if (v) { partial.push(quote(k) + (gap ? ': ' : ':') + v); } } } }
                    v = partial.length === 0 ? '{}' : gap ? '{\n' + gap + partial.join(',\n' + gap) + '\n' +
	mind + '}' : '{' + partial.join(',') + '}'; gap = mind; return v;
            }
        }
        return { stringify: function(value, replacer, space) {
            var i; gap = ''; indent = ''; if (typeof space === 'number') { for (i = 0; i < space; i += 1) { indent += ' '; } } else if (typeof space === 'string') { indent = space; }
            rep = replacer; if (replacer && typeof replacer !== 'function' && (typeof replacer !== 'object' || typeof replacer.length !== 'number')) { throw new Error('JSON.stringify'); }
            return str('', { '': value });
        }, parse: function(text, reviver) {
            var j; function walk(holder, key) {
                var k, v, value = holder[key]; if (value && typeof value === 'object') { for (k in value) { if (Object.hasOwnProperty.call(value, k)) { v = walk(value, k); if (v !== undefined) { value[k] = v; } else { delete value[k]; } } } }
                return reviver.call(holder, key, value);
            }
            cx.lastIndex = 0; if (cx.test(text)) {
                text = text.replace(cx, function(a) {
                    return '\\u' + ('0000' +
	(+(a.charCodeAt(0))).toString(16)).slice(-4);
                });
            }
            if (/^[\],:{}\s]*$/.test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@').replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) { j = eval('(' + text + ')'); return typeof reviver === 'function' ? walk({ '': j }, '') : j; }
            throw new SyntaxError('JSON.parse');
        }
        };
    } ();
})(jQuery);
/**/ 
/*jquery.gnoss.utils.js*/ 
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
 * Date: 20.02.2014.11.30 
 */
function CompletadaCargaActividadReciente() {
    if (typeof (window.CompletadaCargaActividadRecienteComunidad) == 'function') {
        CompletadaCargaActividadRecienteComunidad();
    }
}
function CompletadaCargaBiografia() {
    if (typeof (window.CompletadaCargaBiografiaComunidad) == 'function') {
        CompletadaCargaBiografiaComunidad();
    }
}
function CompletadaCargaRecursos() {
    modoVisualizacionListados.init();
    if (typeof (window.CompletadaCargaRecursosComunidad) == 'function') {
        CompletadaCargaRecursosComunidad();
    }
}
function CompletadaCargaContextos() {
    engancharClicks();
	comportamientoRecursosVinculados.init();
	if(typeof (window.CompletadaCargaContextosComunidad) == 'function') {
		CompletadaCargaContextosComunidad();
	}		
}
function CompletadaCargaFacetas(){
	comportamientoCargaFacetas.init();
	if(typeof (window.comportamientoCargaFacetasComunidad) == 'function') {
		comportamientoCargaFacetasComunidad();
	}		
}
function CompletadaCargaUsuariosVotanRecurso(){
	comportamientoCargaUsuariosVotanRecurso.init();
	if(typeof (window.comportamientoCargaUsuariosVotanRecursoComunidad) == 'function') {
		comportamientoCargaUsuariosVotanRecursoComunidad();
	}		
}
function completadaCargaAcciones(){
    herramientasRecursoCompactado.init();        
    if (body.hasClass('palco') && body.hasClass('activo')) {
        abreEnVentanaNueva.montarHerramientas();
        abreEnVentanaNueva.montarVotos();
        abreEnVentanaNueva.montarVolverFicha();        
    }
	if(typeof (window.completadaCargaAccionesComunidad) == 'function') {
		completadaCargaAccionesComunidad();
	}	
	return;
}
function CompletadaCargaAccionesListado() {
    if (typeof (window.CompletadaCargaAccionesListadoComunidad) == 'function') {
        CompletadaCargaAccionesListadoComunidad();
    }
}
var comportamientoCargaUsuariosVotanRecurso = {
	init: function(){
		this.config();
		this.engancharEnlaceMasResultados();
		if(this.listado.find('.votoNegativo').size() <= 0) return;
		this.tabsVotosPositivosNegativos();
		return;
	},
	config: function(){
		this.body = body;
		this.panelAmpliado = this.body.find('#panelVotosAmpliado');
		this.listado = this.panelAmpliado.find('.resource-list');
		this.enlace = this.panelAmpliado.find('.masUsuriosVotosRecursos a');
		return;
	},
	tabsVotosPositivosNegativos: function(){
		var tabs = this.panelAmpliado.find('.tabsVotosPositivosNegativos');
		if(tabs.size() > 0) return;
		this.tabs = $('<div>').addClass('tabsVotosPositivosNegativos tabspresentation acciones');
		var ulTabs = $('<ul>');
		var liPositivosTabs = $('<li>').addClass('mostrarPositivos');
		var liNegativosTabs = $('<li>').addClass('mostrarNegativos');
		var liTodosTabs = $('<li>').addClass('mostrarTodos active');
		var aPositivosTabs = $('<a>').attr('href', '#').text('votos positivos');
		var aNegativosTabs = $('<a>').attr('href', '#').text('votos negativos');
		var aTodosTabs = $('<a>').attr('href', '#').text('todos');
		liPositivosTabs.append(aPositivosTabs);
		liNegativosTabs.append(aNegativosTabs);
		liTodosTabs.append(aTodosTabs);
		ulTabs.append(liTodosTabs);
		ulTabs.append(liPositivosTabs);
		ulTabs.append(liNegativosTabs);
		this.tabs.append(ulTabs);
		this.listado.before(this.tabs);
		this.engancharTipoVoto();
		return;
	},
	desmarcarTabs: function(){
		this.tabs.find('li').removeClass('active');
		return;
	},
	engancharTipoVoto: function(){
		var that = this;
		this.tabs.find('a').bind('click', function(evento){
			evento.preventDefault();
			var enlace = $(evento.target);
			var li = enlace.parent();
			that.desmarcarTabs();
			that.listado.removeClass('mostrarPositivos mostrarNegativos mostrarTodos');
			that.listado.addClass(li.attr('class'));
			li.addClass('active');
		})
		return;
	},
	engancharEnlaceMasResultados: function(){
		var that = this;
		this.enlace.bind('click', function(evento){
			evento.preventDefault();
			//var enlace = $(this);
			//var url = enlace.attr('href');
			/*setTimeout(function(){
				that.traerUsuariosVotosRecursos(url);
			}, 800);*/
			//WebForm_DoCallback('ctl00$ctl00$CPH1$CPHContenido$controles_ficharecurso_ascx', 'VerVotantes', ReceiveServerData, '', null, false);
			enlace.parent().remove();
		})
		return;
	},
	traerUsuariosVotosRecursos: function(url){
		var that = this;
		//var url = url;
		//var respuesta = $.ajax({
		//  url: url,
		//  type: "GET",
		//  dataType: "html"
		//})

	    GnossPeticionAjax(url, null, true)
		.done(function(pagina) {
			var html = pagina;
			that.panelAmpliado.find('.resource-list').first().append(html);	
			var scrollTop = that.listado.scrollTop() + 710;
			that.listado.scrollTop(scrollTop);
			CompletadaCargaUsuariosVotanRecurso();
			return;
		})
		.fail(function( jqXHR, textStatus ) {
		  alert( "Request failed: " + textStatus );
		})
		return;		
	}
}
var comportamientoCargaFacetas = {
    init: function () {
        // Longitud facetas por CSS
		// limiteLongitudFacetas.init();
		facetedSearch.init();
		$('.verMasFaceta').each(function () {
		    var enlace = $(this);
		    var params = enlace.attr('rel').split('|');
		    var faceta = params[0];
		    var controlID = params[1];
		    enlace.unbind("click").click(function (evento) {
		        evento.preventDefault();
		        VerFaceta(faceta, controlID);
		    });
		});
		return;
	}
}
var comportamientoRecursosVinculados = {
	init: function(){
		this.config();
		var groupsToSemanticView = this.columnaRelacionados.find('.moveToSemanticView')
		if(groupsToSemanticView.size() > 0) this.moveToSemanticView(groupsToSemanticView);	
		return;
	},
	config: function(){
		this.content = $('#content');
		this.columnaRelacionados = this.content.find('#col01');
		this.semanticView = this.content.find('.semanticView');
		return;
	},
	moveToSemanticView: function(groupsToSemanticView){
		var that = this;
		groupsToSemanticView.each(function(){
			var grupo = this;
			that.semanticView.append(grupo);
		});
		return;
	}
}
var limpiarGruposVaciosSemanticView = {
	init: function(){
		$('.semanticView .group').each(function(indice){
			var group = $(this);
			var contentGroup = group.find('.contentGroup');
			if(contentGroup.html() == '') group.remove();
		})
	}
}
var controladorLineas = {
	caracteres: 140,
	init: function(){
		this.config();
	},
	config: function(){
		var that = this;
		this.items = $('#col02 .semanticView .limitGroup');
		this.items.each(function(indice){
			var item = $(this);
			var contentGroup = item.find('.contentGroup');
			var texto = contentGroup.text();
			var arrayTexto = texto.split(' ');
			if(arrayTexto.length <= that.caracteres) return;			
			contentGroup.addClass('activado').hide()
			var css = item.attr('class');
			var cssArray = css.split(' ');
			var cssLimite = cssArray[cssArray.length - 1];
			if(cssLimite.indexOf('limit_') >= 0){
				that.carateres = cssLimite.substring(6, cssLimite.length);
			};

			var recorte = arrayTexto.slice(0, that.caracteres);
			var textoRecortado = '<p>';
			for(var contador = 0; contador < that.caracteres; contador++){
				textoRecortado += recorte[contador] + ' ';
			};
			textoRecortado += '</p>';
			var enlace = $('<a>').attr('class','leermas').attr('href','#').text('+ leer más').attr('title','leer resto de la entrada');
			var plegar = $('<a>').attr('class','plegar').attr('href','#').text('- plegar').attr('title','plegar contendio de la entrada');
			var divTextoRecortado = $('<div>').attr('class','textoRecortado');
			divTextoRecortado.append(textoRecortado);
			contentGroup.before(divTextoRecortado);
			contentGroup.append(plegar);
			divTextoRecortado.append(enlace);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			});
			plegar.bind('click', function(evento){
				evento.preventDefault();
				that.plegarEntrada(evento);
			});			
		});
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		var group = enlace.parents('.group').first();
		var recorte = group.find('.textoRecortado');
		var original = group.find('.contentGroup');
		recorte.hide();
		original.show();
		return;
	},		
	plegarEntrada: function(evento){
		var enlace = $(evento.target);
		var group = enlace.parents('.group').first();
		var recorte = group.find('.textoRecortado');
		var original = group.find('.contentGroup');
		original.hide();
		recorte.show();
		return;
	}	
}
var modoVisualizacionListadosHomeCatalogo = {
	init: function(){
		this.body = body;
		this.grupos = this.body.find('#col02 .listadoRecursos');
		if(this.grupos.size() <= 0) return;
		this.config();
		this.preloader();
		return;
	},
	config: function(){
		return;
	},	
	preloader: function(){
		var that = this;
		var group = group;
		this.grupos.each(function(indice){
			var grupo = $(this);
			if(grupo.hasClass('listView')) return;
			that.calcular(grupo);
			var imagenes = grupo.find('.miniatura img');
			imagenes.each(function(indice){
				var imagen = $(this);
				imagen.load(function(evento){
					var imagen = $(evento.target);
					var grupo = imagen.parents('.listadoRecursos').first();
					that.calcular(grupo);
				})
			})
			
		})
		return;
	},	
	calcular: function(grupo){
		var that = this;
		var grupo = grupo;
		var contador = 0;
		var masAlto = 0;
		var resources = grupo.find('.resource');
		var alturas = [];
		resources.each(function(){
			var recurso = $(this);
			recurso.attr('style', '');
			//var categorias = recurso.find('.categorias')
			//var etiquetas = recurso.find('.etiquetas')
			//limitarNumeroItems.init(categorias);
			//limitarNumeroItems.init(etiquetas);
			var altura = recurso.height();
			if(altura > masAlto) masAlto = altura;
			contador ++;
			if(contador == 3){
				recurso.addClass('omega');
				alturas.push(masAlto);
				contador = 0;
				masAlto = 0;
			}
		});
		resources.each(function(indice){
			var recurso = $(this);
			var fila = parseInt(indice/3);
			recurso.css('height', alturas[fila] + 'px');
		});		
		this.isCalculadoOmega = true;
		return;
	}
};
var modoVisualizacionListados = {
    id: '#view',
    cssListView: 'listView',
    cssGridView: 'gridView',
    cssActiveView: 'activeView',
    cssResourceList: '.resource-list',
    isAlturaCalculada: false,
    isCalculadoOmega: false,
    isGridViewDefault: false,
    isLanzadaSeguridad: false,
    init: function () {
        body = $('body');
        this.body = body;
        var group = this.body.find('#col02 .listadoRecursos');
        this.config();
        this.setView();
        if (this.isGridViewDefault) {
            this.configGridView();
            this.preloader(group);
        } else {
            this.configListView();
        }        
        this.enganchar();
        return;
        //if(!this.isLanzadaSeguridad)this.seguridadImagenesRotas();
        //this.isLanzadaSeguridad = true;
    },
    config: function () {
        this.list = $(this.cssResourceList);
        this.view = $(this.id);
        this.listView = $('.' + this.cssListView, this.view);
        this.gridView = $('.' + this.cssGridView, this.view);
        return;
    },
    configGridView: function () {
        this.showGridView();
    },
    configListView: function () {
        this.showListView();
    },
    showGridView: function () {
        this.list.removeClass(this.cssListView);
        this.list.addClass(this.cssGridView);
    },
    showListView: function () {
        this.list.removeClass(this.cssGridView);
        this.list.addClass(this.cssListView);
    },
    preloader: function (group) {
        var that = this;
        var group = group;
        //group.css('visibility','hidden');
        group.addClass('gridPreview');
        var imagenes = $('.miniatura img', group);
        var numeroImagenes = imagenes.size();

        for (var i=1;i<=5;i++)
        {
            setTimeout(function () {
                that.calcular(group);
            }, i*1000);
        }
        
        if (numeroImagenes <= 0) {
            this.calcular(group);
            //group.css('visibility','visible');
            group.removeClass('gridPreview');
        } else {
            var contador = 0;
            //sthis.view.hide();
        };
        imagenes.load(function () {
            var imagen = $(this);
            contador++;
            if (contador > numeroImagenes - 1) {
                that.calcular(group);
                group.removeClass('gridPreview');
                //group.css('visibility','visible');
                that.view.show();
            }
        });
        return;
    },
    seguridadImagenesRotas: function () {
        var that = this;
        setTimeout(function () {
            $('#section .listadoRecursos').each(function () {
                var group = $(this);
                var atributo = group.attr('style');
                var isGroupVisible = atributo.indexOf('hidden') <= 0;
                if (!isGroupVisible) {
                    that.calcular(group);
                    group.removeClass('gridPreview');
                    //group.css('visibility','visible');
                    that.view.show();
                }
            })
        }, 5000);
    },
    calcular: function (group) {
        var that = this;
        var contador = 0;
        var masAlto = 0;
        var resources = $('.resource', group);
        var alturas = [];
        resources.each(function () {
            var recurso = $(this);
            recurso.removeAttr('style');  
            var categorias = recurso.find('.categorias')
            var etiquetas = recurso.find('.etiquetas')
            limitarNumeroItems.init(categorias);
            limitarNumeroItems.init(etiquetas);
            var altura = recurso.height();
            if (altura > masAlto) masAlto = altura;
            contador++;
            if (contador == 3) {
                recurso.addClass('omega');
                alturas.push(masAlto);
                contador = 0;
                masAlto = 0;
            }
        });
        resources.each(function (indice) {
            var recurso = $(this);
            var fila = parseInt(indice / 3);
            recurso.css('height', alturas[fila] + 'px');
        });
        this.isCalculadoOmega = true;
        return;
    },
    setView: function () {
        this.gridView.hasClass(this.cssActiveView) ? this.isGridViewDefault = true : this.isGridViewDefault = false;
        return;
    },
    enganchar: function () {
        var that = this;
        $('a', this.listView).unbind();
        $('a', this.listView).bind('click', function (evento) {
            evento.preventDefault();
            that.showListView();
            that.listView.addClass(that.cssActiveView);
            that.gridView.removeClass(that.cssActiveView);
        });

        $('a', this.gridView).unbind();
        $('a', this.gridView).bind('click', function (evento) {
            evento.preventDefault();
            that.showGridView();
            that.gridView.addClass(that.cssActiveView);
            that.listView.removeClass(that.cssActiveView);
            that.calcular();
        });
        return;
    }
};
var seccion = {
	id: '#nav',
	secciones: ['home','indice','catalogo','recurso','debate', 'dafo', 'pregunta','encuesta', 'persona'],
	seccionActiva: 0,
	init: function(){
		this.config();
		this.buscarSeccion();
		this.desmarcar();
		this.marcar();
	},
	config: function(){
		this.nav = $(this.id);
		this.li = $('li', this.nav);
		return;
	},
	buscarSeccion: function(){
		var items = this.secciones.length;
		var url = window.location.href;
		for(var contador = 0; items > contador; contador ++ ){
			var nombreSeccion = this.secciones[contador];
			if(url.indexOf(nombreSeccion) >= 0) this.seccionActiva = contador;
		};
		return;
	},
	desmarcar: function(){
		this.li.each(function(){
			$(this).removeClass('activo');
		})
		return;
	},
	marcar: function(){
		var activo = $(this.li[this.seccionActiva]);
		activo.addClass('activo');
		return;
	}	
};
function desmarcarOpcionesGrupo(lis){
	lis.each(function(){
		var link = $('a', this);
		if(!link.hasClass('noGroup')){
			$(this).removeClass('active');
		};
	});
	return;
}
function ocultarPaneles(panels){
	var panels = panels.split(' ');
	var contador = panels.length;
	for(var i = 0; contador > i; i++ ){
		$('#' + panels[i]).hide();
	};				
}
var carrusel = {
	id: '#presentation',
	preloadImages: true,
	isImagesLoaded: false,
	numImagenActiva: 0,
	isPause: false,
	vueltas: 0,
	init: function(){
		this.config();
		if(!this.preloadImages) return;
		this.loader();
	},
	config: function(){
		this.carrusel = $(this.id);
		if(this.carrusel.hasClass('nopreload')) this.preloadImages = false;
		if(!this.preloadImages) return;
		this.view = $('.galeriaPresentacion', this.carrusel);
		this.items = $('.carrusel li', this.carrusel);
		this.imagenes = $('.carrusel li img', this.carrusel);
		// Deprecado size() 
        //this.numeroImagenes = this.imagenes.size();
        this.numeroImagenes = this.imagenes.length;
		return;
	},
	loader: function(){
		var that = this;
		var contador = 0;
		this.imagenes.each(function(indice){
			var imagen = $(this);
			imagen.parent().attr('rel', indice);
			var li = that.items[indice];
			if(indice > 0) $(li).hide();
			this.onload = function(){
				contador ++;
				var imagen = $(this);
				if(indice == that.numImagenActiva){
					var li = that.items[indice];
					$(li).fadeIn('slow');
				}
				imagen.addClass('loaded');
				if(contador == that.numeroImagenes){
					that.engancharEfecto();
					that.crearPasador();
				}
			};
		});
		this.seguridad();
		return;
	},
	crearPasador: function(){
		var itemsCarrusel = '';
		var html = '';
		this.items.each(function(indice){
			var muestra = indice + 1;
			itemsCarrusel += '<li><a href=\"#\" rel=\"' + indice + '\" >' + muestra + '<\/a><\/li>';
		});
		html += '<div id=\"pasadorCarrusel\">';
		html += '<ul>';
		html += itemsCarrusel;
		html += '<\/ul>';
		html += '<\/div>';
		this.view.append(html);
		this.pasador = $('#pasadorCarrusel', this.carrusel);
		this.enlacesPasador = $('a', this.pasador);
		this.marcarItemPasadorActivo(0);
		this.engancharPasador();
		return;
	},	
	desmarcarItemsPasadorActivo: function(){
		this.enlacesPasador.each(function(){
			$(this).removeClass('activo');
		});
		return;
	},	
	marcarItemPasadorActivo: function(numero){
		var enlace = this.enlacesPasador[numero];
		$(enlace).addClass('activo');
		return;
	},
	engancharPasador: function(){
		var that = this;
		this.enlacesPasador.each(function(){
			var enlace = $(this);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				var numero = enlace.attr('rel');
				enlace.addClass('activo');
				that.isPause =  true;
				that.contadorIntervalos = 0;
				that.efectoPasador(numero);
			});
		})
		return;
	},
	efectoPasador: function(numero){
		var siguiente = numero;
		var liIn = $(this.items[siguiente]);
		var liOut = $(this.items[this.numImagenActiva]);
		this.desmarcarItemsPasadorActivo();	
		this.marcarItemPasadorActivo(siguiente);		
		liOut.fadeOut('fast', function(){
			liIn.hide().fadeIn('slow');			
		});		
		this.numImagenActiva = siguiente;	
		clearInterval(this.intervalo);
		return;
	},	
	efecto: function(){
		var siguiente = this.numImagenActiva + 1;
		if(siguiente >= this.numeroImagenes){
			siguiente = 0;
			this.vueltas ++;
			if(this.vueltas == 2) clearInterval(this.intervalo);
		}
		var liIn = $(this.items[siguiente]);
		var liOut = $(this.items[this.numImagenActiva]);
		this.desmarcarItemsPasadorActivo();
		this.marcarItemPasadorActivo(siguiente);
		liOut.fadeOut('fast', function(){
			liIn.hide().fadeIn('slow');			
		});	
		this.numImagenActiva = siguiente;	
		return;
	},
	seguridad: function(){
		var that = this;
		var imagen = this.imagenes[0];
		var isLoadedImage = false;
		imagen = $(imagen);
		setTimeout(function(){
			if(imagen.hasClass('loaded')) isLoadedImage = true;
			if(!isLoadedImage) {
				that.engancharEfecto();
				that.crearPasador();
			}
		}, 3000);			 
	},
	engancharEfecto: function(){
		var that = this;
		//this.engancharClick();
		this.contadorIntervalos = 0;
		this.intervalo = setInterval(function(){
			that.contadorIntervalos ++;
			if(that.contadorIntervalos >= 4){
				that.isPause = false;
				that.contadorIntervalos = 0;
			}
			that.efecto();
		}, 3000);
		return;
	}
}
var carruselLateralColumna  = {
	identificadorRecurso: '.resource',
	carruseles: ['.comiteCrea'],
	recursoVisible: 0,
	isPause: false,
	init: function(){
		var carrusel = this.carruseles[0];
		this.componente = $(carrusel);
		this.config();
		this.ocultar();
		this.mostrar(this.recursoVisible);
		this.crearPaginador();
		this.engancharPaginador();
		this.automatismo();
		return;
	},
	config: function(){
		this.recursos = $(this.identificadorRecurso, this.componente);
		return;
	},
	ocultar: function(){	
		this.recursos.each(function(){
			$(this).hide();
		})
		return;
	},
	mostrar: function(numero){
		var visible = this.recursos[numero];
		visible = $(visible);
		visible.show();
		return;
	},
	plantilla: function(){
		var html = '';
		html += '<div class=\"paginador\">';
		html += '<ul>';
		this.recursos.each(function(indice){
			if(indice == 0){
				html += '<li class=\"activo\"><a href=\"#\" rel=\"' + indice + '\">' + indice  + '<\/a><\/li>';
			}else{
				html += '<li><a href=\"#\" rel=\"' + indice + '\">' + indice  + '<\/a><\/li>';
			}
		});
		html += '<\/ul>';
		html += '<\/div>';
		return html;
	},
	crearPaginador: function(){
		this.componente.append(this.plantilla());
		this.paginador = $('.paginador', this.componente);
		this.itemsPaginador = $('a', this.paginador);
		return;
	},
	desmarcarPaginador: function(){
		this.itemsPaginador.each(function(){
			$(this).parent().removeClass('activo');
		});
		return;
	},
	automatismo: function(){
		var that = this;
		var contador = 0;
		setInterval(function(){
			contador++;
			if(contador >= 8){
				that.isPause = false;
				contador = 0;
			}
			if(that.isPause) return;
			that.recursoVisible ++;
            // Deprecado size()
            //if(that.recursoVisible >= that.itemsPaginador.size()) that.recursoVisible = 0;
            if (that.recursoVisible >= that.itemsPaginador.length) that.recursoVisible = 0;			
			that.siguiente();
			var activo = that.itemsPaginador[that.recursoVisible];
			activo = $(activo);
			activo.parent().addClass('activo');
		}, 4000);		
	},	
	siguiente: function(){
		this.ocultar();
		this.mostrar(this.recursoVisible);
		this.desmarcarPaginador();
		return;
	},
	engancharPaginador: function(){
		var that = this;
		this.itemsPaginador.each(function(contador){
			$(this).bind('click', function(evento){
				evento.preventDefault();
				that.isPause = true;
				var enlace = $(evento.target);
				var indice = enlace.attr('rel');
				that.recursoVisible = indice;
				that.siguiente();
				enlace.parent().addClass('activo');	
			});
		});
		return;
	}
}
var opcionesMenuIdentidad = {
	cssItemActivo: 'itemActivo',
	cssItemsOpcionesSegundoNivel: '.itemConOpcionesSegundoNivel',
	cssOpcionPrincipal: '.opcionPrincipal',
	idMenuSuperiorComunidades: '#menuSuperiorComunidades',
	idOtrasIdentidades: '#otrasIdentidades',
	idIdentidad: '#identidad',
	cssEnlaceOtrasIdentidades: '#otrasIdentidades .wrap a',
	cssListadoOtrasIdentidades: '.listadoOtrasIdentidades',
	init: function(){
        this.config();
        // Deprecado size()
        //if(this.menu.size() <= 0) return;
        if (this.menu.length <= 0) return;
		this.pantalla();
		this.enganchar();
		this.ocultarMenusInactividad();
	},
	config: function(){
		this.identidad = $(this.idIdentidad);
		this.menu = $(this.idMenuSuperiorComunidades);
		this.items = $(this.cssItemsOpcionesSegundoNivel, this.menu);
		this.enlaces = $(this.cssOpcionPrincipal + ' a', this.items);
		this.otrasIdentidades = $(this.cssOpcionPrincipal + ' a', this.items);
		this.enlaceOtrasIdentidades = $(this.cssEnlaceOtrasIdentidades, this.identidad);
		this.otrasIdentidades = $(this.cssListadoOtrasIdentidades, this.identidad);
		this.page = $('#page');
		return;
	},
	ocultarMenusInactividad: function(){
		var that = this;
		this.page.hover(
			function(){
				that.ocultar();
				that.ocultarOtrasIdentidades();
			},function(){return}
		);	
	},
	ocultar: function(){
		this.items.removeClass(this.cssItemActivo);
		return;
	},
	ocultarOtrasIdentidades: function(){
		this.enlaceOtrasIdentidades.parents('li').removeClass(this.cssItemActivo);
		this.otrasIdentidades.hide();
		return;
	},	
	pantalla: function(){
		var item = this.items[0];
		var principal = $(this.cssOpcionPrincipal, item);
		this.correccion = principal.offset().left;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		var item = enlace.parents(this.cssItemsOpcionesSegundoNivel);
		if(!item.hasClass(this.cssItemActivo)){
			this.ocultar();
			this.ocultarOtrasIdentidades();
			var principal = enlace.parent();
			principal = $(principal);
			var left = principal.offset().left - this.correccion;
			var opciones = $('.opcionesPanel', item);
			opciones.css('left', left + 'px');
			item.addClass(this.cssItemActivo);
		}else{
			item.removeClass(this.cssItemActivo);
		}
		return;
	},
	comportamientoOtrasIdentidades: function(evento){
		var enlace = $(evento.target);
		var item = enlace.parents('li');
		this.ocultar();
		if(!item.hasClass(this.cssItemActivo)){
			var left = item.offset().left - this.correccion;
			this.otrasIdentidades.css('left', left + 'px');
			this.otrasIdentidades.show();
			item.addClass(this.cssItemActivo);
		}else{
			this.ocultarOtrasIdentidades();
		}
		return;
	},	
	enganchar: function(){
		var that = this;
		this.enlaces.each(function(){
			$(this).bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			})
		});
		this.enlaceOtrasIdentidades.bind('click', function(evento){
			evento.preventDefault();
			that.comportamientoOtrasIdentidades(evento);
		});
	}
};

var facetedSearch = {
	noCollapse: 'noCollapse',
	init: function(){
		//this.config();
		//this.comportamiento();
	},
	config: function(){
		this.facetas = $('#facetedSearch .box:not(.' + this.noCollapse + '):not(.categories)');
		return;
	},
	ocultar: function(){
		this.facetas.each(function(){
			var faceta = $(this);
			var searchBox = $('.facetedSearchBox', faceta);
			if(faceta.height() < 72) return;
			searchBox.hide();
			faceta.css({
				'height': '72px',
				'background': '#f0f0f0',
				'border-bottom': 'none'
			});			
		});
		return;
	},
	comportamiento: function(){
		var that = this;
		this.facetas.each(function(){
			var faceta = $(this);
			var searchBox = $('.facetedSearchBox', faceta);
			if(faceta.height() < 72) return;
			searchBox.hide();
			faceta.css({
				'height': '72px',
				'overflow': 'hidden'
			});
			faceta.hover(
				function(){
					that.ocultar();
					searchBox.show();
					var altura = faceta.height();
					faceta.css({
						'height': '100%',
						'background': '#eee'
					}, 1000);
				},
				function(){
					return;
					searchBox.hide();
					faceta.css({
						'height': '72px',
						'background': '#f0f0f0',
						'border-bottom': 'none'
					});
				})
		});		
	}
}; 
var limitarNumeroItems = {
	init: function(listado){
		this.listado = listado;
		this.listado.addClass('limitado');
		this.config();
		this.comportamiento();
		return;
	},
	config: function(){
		this.recurso = this.listado.parents('div.resource');
		this.enlace = this.recurso.find('.title a');
		return;
	},
	comportamiento: function(){
		var ul = this.listado.find('ul');
		var lis = this.listado.find('li');
		if(lis.size() <= 3) return;
		lis.each(function(indice){
			if(indice > 2) $(this).hide();
		});
		ul.append('<li><a class=\"verTodasCategoriasEtiquetas mas\" href="' + this.enlace.attr('href') + '" title=\"ver todos\">' + form.masMIN + '...<\/a><\/li>');
		return;
	}
};

var mostrarNumeroCategorias = {
    init: function () {
        this.comportamiento();
    },
    config: function () { return },
    comportamiento: function () {
        var categorias = $('#section .resource .categorias ul');
        categorias.each(function () {
            var ul = $(this);
            var verMas = ul.find('a.verTodasCategoriasEtiquetas');
            if (verMas.length == 0) {
                var lis = $('li', ul);
                if (lis.size() <= 3) return;
                lis.each(function (indice) {
                    if (indice > 2) $(this).hide();
                })
                ul.append('<li><a class=\"verTodasCategoriasEtiquetas mas\" href="#" title=\"ver todos\">' + form.masMIN + '...<\/a><\/li>');
            }
        });
        return;
    }
};

var mostrarNumeroEtiquetas = {
    init: function () {
        this.comportamiento();
    },
    config: function () { return },
    comportamiento: function () {
        $('#section  .resource .etiquetas ul').each(function () {
            var ul = $(this);
            var verMas = ul.find('a.verTodasCategoriasEtiquetas');
            if (verMas.length == 0) {
                var lis = $('li', ul);
                if (lis.size() <= 3) return;
                lis.each(function (indice) {
                    if (indice > 2) $(this).hide();
                })
                ul.append('<li><a class=\"verTodasCategoriasEtiquetas mas\" href="#" title=\"ver todos\">' + form.masMIN + '...<\/a><\/li>');
            }
        });
        return;
    }
};



/**
 * Calcular y recortar la longitud de las facetas que aparecen en el panel izquierdo de "B�squedas". 
 */
var limiteLongitudFacetas = {
    init: function () {
        this.comportamiento();
    },
    config: function () { return },
    comportamiento: function () {
        $('#facetedSearch .box li a .textoFaceta').each(function () {
            var digitos = 24;
            var hayNumero = false;
            var enlace = $(this);
            var textoEnlace = $.trim(enlace.text());
            var longitud = textoEnlace.length;
            var caracter = '';

            var p1 = longitud;

            if (textoEnlace.lastIndexOf('(') > 0) {
                p1 = textoEnlace.lastIndexOf('(');
            }

            if (enlace.parent().children('img').length > 0) {
                digitos--;
            }

            var margenLeft = enlace.css('margin-left');
            margenLeft = margenLeft.substring(0, margenLeft.length - 2);
            while (margenLeft >= 10) {
                margenLeft = margenLeft - 10;
                digitos--;
            }

            if (enlace.parents('ul').length > 1) {
                digitos = digitos - (enlace.parents('ul').length * 2);
            }

            hayNumero = (textoEnlace.charAt(textoEnlace.length - 1) == ')');
            if (hayNumero) {
                digitos = digitos - (longitud - p1);
            }

            var c1 = $.trim(textoEnlace.substring(0, p1));
            if (c1.length >= digitos) {
                longitud = digitos - 4;
                c1 = c1.substring(0, longitud);
                c1 = c1 + ' ...';
            }
            var textoEnlaceNuevo = c1;
            if (hayNumero) {
                var cantidad = textoEnlace.substring(p1 + 1, textoEnlace.length - 1);
                textoEnlaceNuevo += '<span>(';
                textoEnlaceNuevo += cantidad;
                textoEnlaceNuevo += ')<\/span>';
            }
            enlace.html(textoEnlaceNuevo);
        });
    }
};


var verTodasCategoriasEtiquetas = {
    init: function () {
        this.comportamiento();
    },
    config: function () { return },
    comportamiento: function () {
        $('#section  .resource .verTodasCategoriasEtiquetas').each(function () {
            $(this).unbind();
            $(this).bind('click', function (evento) {
                evento.preventDefault();
                var enlace = $(evento.target);
                var ul = enlace.parents('ul');
                var lis = $('li', ul);
                var total = lis.size();
                if (enlace.hasClass('mas')) {
                    lis.show();
                    enlace.removeClass('mas');
                    enlace.text(form.menos);
                } else {
                    lis.each(function (indice) {
                        var li = $(this);
                        if (indice > 2 && indice < total - 1) li.hide();
                    });
                    enlace.addClass('mas');
                    enlace.text(form.masMIN+'...');
                }
            })
        });
        return;
    }
};
var limpiarActividadRecienteHome = {
    init: function () {	
		var body = $('body');
        if(body.hasClass('fichaComunidad') || body.hasClass('fichaRecurso')) return;
        this.limpiar();
    },
    mostrarContenidos: function (content) {
        var items = content.children();
        items.hide();
        var itemsMostrados = 0;
        primerParrafoConImagen = false;
		var primerItem = items.first();
		var segundoItem = primerItem.next();
        var item = '';
		var longitud = 250;
        if (primerItem.hasClass('miniatura')) {
            primerItem.show();
            segundoItem.show();
            item = segundoItem;
        }else {
			var longitudPrimerParrafo = primerItem.text().length;
			if(longitudPrimerParrafo > longitud){
				primerItem.show();
                item = primerItem;
                // Eliminar los párrafos que no se muestran para que no generen espacio adicional excluyendo el primero
                for (var i = 1, l = items.length; i < l; i++) {
                    items[i].remove();
                }
			}else{
				primerItem.show();
				segundoItem.show();
				item = segundoItem;
				longitud = longitud - longitudPrimerParrafo;
			}
        }
		this.acortarItem(item, longitud);			
        this.buscarEnlace(item);
        return;
    },
    acortarItem: function (item, longitud) {
        var texto = item.text();
        if (texto.length > longitud) {
            var acortado = texto.substring(0, longitud);
            //item.text(acortado);
            // Añadir ... para que de sensación de continuidad
            item.text(acortado + "...");
        }
        return;
    },
    buscarEnlace: function (item) {
        var content = item.parents('div').first();
        var verMasRecurso = content.next();
		if(!verMasRecurso.hasClass('verMasRecurso')) return;
        var enlace = verMasRecurso.find('a');
        item.append(enlace);
		return;
    },
    resetear: function (item) {
        var isSpan = item.nodeName == 'SPAN';
        var item = $(item);
        if (!isSpan) item.attr('style', '');
        if (!item.hasClass('miniatura')) item.attr('class', '');
        item.attr('size', '');
        item.attr('face', '');
        return;
    },

    /**
    * Limpiar el contenido de recursos o comentarios para que no aparezcan formatos HTML. Adem�s de acortarlos textos de comentarios (Vista preliminar)".
    */
    limpiar: function () {
        var that = this;
        // Actualizado el item para que sepa donde limpiar el contenido
        $('.resource-list .descripcionResumida, #content .comment-content').each(function () {
            var content = $(this);
            if (!content.hasClass('limpio')) {
                var parrafos = $('p', content);
                var span = $('span', content);
                var font = $('font', content);
                var img = $('img', content);
                var div = $('div', content);
                var a = $('a', content);
                var ul = $('ul', content);
                var li = $('li', content);
                var items = [parrafos, span, font, img, div, a, ul, li];
                $.each(items, function () {
                    $.each(this, function () {
                        that.resetear(this);
                    });
                });
                parrafos.each(function (indice) {
                    var item = $(this);
                    if (item.hasClass('miniatura')) return;
                    var texto = item.text();
                    texto = $.trim(texto);
                    if (texto == '' || texto == ' ' || texto == '&nbsp' || texto == ' &nbsp' || texto == null) {
                        item.remove();
                    };
                });
                div.each(function (indice) {
                    var item = $(this);
                    var hasParrafos = false;
                    var parrafos = $('p', item);
                    // Deprecado la funci�n size -> Usar propiedad length
                    //if (parrafos.size() > 0) hasParrafos = true;                    
                    if (parrafos.length > 0) hasParrafos = true;
                    if (!hasParrafos) {
                        var html = '<p>';
                        html += item.html();
                        html += '<\/p>';
                        item.after(html);
                        item.remove();
                    } else {
                        item.after(item.html());
                        item.remove();
                    }
                });
                that.mostrarContenidos(content);                
                content.addClass('limpio');
            }
        })
        return;
    }
}
var pintarRecursoVideo = {
	css: '.recursoVideo',
	cssMiniatura: '.miniatura',
	cssListado: '.resource-list',
	cssItemsVideo: '#content .resource-list .descripcionResumida .recursoVideo',
	init: function(){
		this.config();
		this.comportamiento();
	},
	config: function(){
		this.itemsVideo = $(this.cssItemsVideo, '#section');
		return;
	},
	comportamiento: function(){
		this.itemsVideo.each(function(){
			var item = $(this);
			var enlace = $('a', item);
			var ruta = enlace.attr('href');
			item.append('<a href=\"' + ruta + '\" class=\"resourceTypeVideo\">recurso video<\/a>');
		});
		return;
	}
}
var viewGridHome = {
	resource: '.resource',
	init: function(listado){
		this.listado = $(listado);
		this.recursos = $(this.resource, this.listado);
		var contador = 0;
		this.recursos.each(function(){
			if(contador == 2){
				var recurso = $(this);
				recurso.addClass('omega');
				recurso.after('<div class=\"clearFile\"><\/div>');
				contador = -1;
			}
			contador ++;
		});
	}
}
var viewListDestacadoHome = {
	resource: '.resource',
	init: function(listado){
		this.listado = $(listado);
		this.recursos = $(this.resource, this.listado);
		var contador = 0;
		this.recursos.each(function(){
			if(contador == 1){
				var recurso = $(this);
				recurso.addClass('omega');
				recurso.after('<div class=\"clearFile\"><\/div>');
				contador = -1;
			}
			contador ++;
		});
	}
}
var recursoCompactado = {
	cssContendioExtendido: 'contendioExtendido',
	idCustomAboutResource: '#customAboutResource',
	opcionesDesplegables: ['li.licencia','li.certificado'],
	init: function(){
		this.config();
		this.enganchar();
		return;
	},
	config: function(){
		this.customAboutResource = $(this.idCustomAboutResource);
		this.customAboutResource.append('<div class=\"'+ this.cssContendioExtendido +'\"><p>contenido extendido<\/p><\/div>');
		this.contenidoExtendido = $('.' + this.cssContendioExtendido, this.customAboutResource);
		return;
	},
	enganchar: function(){
		var that = this;
		for (var contador = 0; contador < this.opcionesDesplegables.length; contador ++){
			var opcion = $(this.opcionesDesplegables[contador], this.customAboutResource);
			var etiqueta = opcion.find('span.label');
			etiqueta.addClass('activado');
			var lis = $('li', this.customAboutResource);
			etiqueta.bind('click', function(evento){
				evento.stopPropagation();
				var etiqueta = $(evento.target);
				var opcion = etiqueta.parent();
				var valor = opcion.find('span.value').html();
				if(!opcion.hasClass('activo')){
					lis.removeClass('activo');
					that.contenidoExtendido.html(valor);
					that.contenidoExtendido.show();	
					opcion.addClass('activo');
				}else{
					that.contenidoExtendido.hide();	
					opcion.removeClass('activo');
				}
			});
		};
		return;
	}
}

var iconografia = {
	cssItems: ['a.megusta', 'a.nomegusta'],
	cssIconizer: 'iconizer',
	init: function(){
		var that = this;
		$.each(this.cssItems, function(indice, valor){
			var items = $(valor);
			items.each(function(){
				var item = $(this);
				if(!item.hasClass(that.cssIconizer)){
					item.prepend('<span class="\icono"\><\/span>');
					item.addClass(that.cssIconizer);
				}
			});

		});
	}
}
var ajustarTextoLogoComunidad = {
	id: '#corporativo',
	css: '.content',
	cssClase: '.identificadorClase',
	isClase: false,
	hasLogoImagen: false,
	init: function(){
		this.config();
		if(this.hasLogoImagen) return;
		this.ajustar();
		return;
	},
	config: function(){
		this.caja = $(this.id);
		this.clase = $(this.cssClase, this.caja);
		// Deprecado size()
        if (this.clase.length > 0) this.isClase = true;
		this.wrapcaja = $(this.css, this.caja);
		this.encabezado = $('h1 a', this.caja);
		this.imagen = $('img', this.encabezado);
		// Deprecado size() 
        //if (this.imagen.size() > 0) this.hasLogoImagen = true;
        if (this.imagen.length > 0) this.hasLogoImagen = true;
		return;
	},
	ajustar: function(){
		this.wrapcaja.hide();
		var texto = this.encabezado.text();
		var caracteres = texto.length;
		this.wrapcaja.css({'margin-top':'10px'});
		if(caracteres > 70){
			this.encabezado.css({'font-size':'32px'});
		}else if(caracteres > 40 && caracteres <= 60){
			this.encabezado.css({'font-size':'36px'});			
		}else if(caracteres > 20 && caracteres <= 40){
			this.encabezado.css({'font-size':'47px'});
		}else{
			this.encabezado.css({'font-size':'60px'});
		}
		this.wrapcaja.show();
		return;
	}
}
var herramientasRecursoCompactado = {
    idCustomAboutResource: '#customAboutResource',
    cssResourceTools: '.acciones',
    cssUlPrincipal: '.principal',
    cssLiToSecondary: '.toSecondary',
    anchoMaxUlPrincipal: 540,
    isCreatedSecondary: false,
    init: function () {
        this.config();
        this.resources.addClass('activo');
        this.opcionesSecundarias();
        this.anchoUlPrincipal();
        this.iconografia();
        this.engancharMoreOptions();
        this.engancharMoreOptionsLayer();
    },
    config: function () {
        this.about = $(this.idCustomAboutResource);
        this.resources = this.about.find(this.cssResourceTools);
        this.ulPrincipal = this.resources.find(this.cssUlPrincipal);
        this.liPrincipal = this.ulPrincipal.find('li');
        this.lisToSecundary = this.ulPrincipal.find('li' + this.cssLiToSecondary);
        return;
    },
    engancharMoreOptionsLayer: function () {
        var capa = this.resources.find('.moreTools');
        var parent = capa.parent();
        capa.unbind().hover(
			function () {
			    return;
			}, function () {
			    parent.hasClass('showing') ? parent.removeClass('showing') : parent.addClass('showing');
			    return;
			}
		);
        return;
    },
    engancharMoreOptions: function () {
        var enlace = this.resources.find('.opMoreOptions > a');
        enlace.unbind().bind('click', function (evento) {
            evento.preventDefault();
            var parent = enlace.parent();
            parent.hasClass('showing') ? parent.removeClass('showing') : parent.addClass('showing');
        })
        return;
    },
    crearUlSecondary: function () {
        this.ulSecondary = $('<ul class=\"secondary\">');
        this.resources.append(this.ulSecondary);
        this.isCreatedSecondary = true;
        return;
    },
    opcionesSecundarias: function () {
		// Deprecado size();
        //if (this.lisToSecundary.size() > 0) {
        if (this.lisToSecundary.length > 0) {
            this.crearUlSecondary();
            this.ulSecondary.html(this.lisToSecundary);
            this.liPrincipal = this.ulPrincipal.find('li');
        }
        return;
    },
    iconografia: function () {
        var enlaces = this.resources.find('a');
        enlaces.each(function () {
            var enlace = $(this);
            enlace.prepend('<span><\/span>');
        })
    },
    htmlLisMoreOptions: function (lis) {
        var lis = $(lis);
        var html = '';
        lis.each(function () {
            var li = $(this);
            html += '<li>' + li.html() + '<\/li>';
        });
        //html += '<li class=\"opClose last\"><a href=\"#\">cerrar<\/a><\/li>';
        return html;
    },
    anchoUlPrincipal: function () {
        var that = this;
        if (this.ulPrincipal.width() > this.anchoMaxUlPrincipal) {
            var lisMoreOptions = [];
            var lisVisibleOptions = [];
            var ancho = 0;
            this.liPrincipal.each(function () {
                var li = this;
                ancho += ($(li).width() + 22);
                if (ancho >= that.anchoMaxUlPrincipal) {
                    lisMoreOptions.push(li);
                    $(li).remove();
                };
            })
            if (!this.isCreatedSecondary) this.crearUlSecondary();

            if (this.ulSecondary.find('.opMoreOptions').size()==0) {
                this.liMoreOptions = $('<li class=\"opMoreOptions\">');
                this.aliMoreOptions = $('<a href=\"#\">Más opciones<\/a>');
                this.divMoreOptions = $('<div class=\"moreTools\">');
                this.ulDivMoreOptions = $('<ul>');
                this.divMoreOptions.append(this.ulDivMoreOptions);
                this.ulDivMoreOptions.append(this.htmlLisMoreOptions(lisMoreOptions));
                this.liMoreOptions.append(this.aliMoreOptions);
                this.liMoreOptions.append(this.divMoreOptions);
                this.ulSecondary.append(this.liMoreOptions);
            }
        };
        return;
    }
}

var onlymembers = {
	init: function(){
		$('.onlyMembers').each(function(){
			var capa = $(this);
			var imagen = $('.image', capa);
			imagen.append('<div class=\"\wrap\"><\/div>');
			capa.append('<p class=\"message\">Solo miembros / Only members<\/p>');
			var wrap = $('.wrap', imagen);
			wrap.css('opacity', 0.6);
		})	
	}
}

var onlymembersContent = {
	init: function(){
		$('.onlyMembersContent').each(function(){
			var capa = $(this);
			var mensajePersonalizado = $('.mensajePersonalizadoSoloMiembros');
			var wrap = $('<div>').attr('class','wrap').css('opacity', 0.8);
			capa.prepend(wrap);
			if(mensajePersonalizado.size() > 0) return
			capa.append('<div class=\"message\"><p>Solo miembros / Only members<\/p><p><a href=\"mgLogin.php\">Accede y participa ...<\/a><\/p><\/div>');
		});	
	}
}

var subcategoriasMenuPrincipal = {
	idPrincipal: '#nav',
	init: function(){
		this.config();
		if(!this.hasSubcategorias) return;
		this.nav.addClass('activado');
		this.engancharComportamiento();
	},
	config: function(){
		this.page = $('#page');
		this.nav = $(this.idPrincipal);
		this.subcategorias = this.nav.find('ul ul');
		// Deprecado size()
        //this.hasSubcategorias = this.subcategorias.size() > 0;
        this.hasSubcategorias = this.subcategorias.length > 0;
	},
	engancharComportamiento: function(){
		var that = this;
		this.subcategorias.each(function(indice){
			var ul = $(this);
			var li = ul.parents('li').first();
			var a = li.find('a').first();
			li.addClass('hasSubcategorias');
			a.hover(
				function(){
					that.desmarcarTodos();
					var enlace = $(this);
					var li = enlace.parent().addClass('current on');
					//that.comportamiento(evento);
				},
				function(){
					var enlace = $(this);
					var li = enlace.parent().removeClass('current');					
					return;
				}
			);
			ul.hover(
				function(evento){				
					return;
				},
				function(evento){
					var ul = $(this);
					var li = ul.parent().removeClass('on');					
					return;
				}
			);		
		});
		this.page.hover(
			function(){
				that.desmarcarTodos()
			},
			function(){
				return
			}
		)	
		return;
	},
	desmarcarTodos: function(){
		var items = this.nav.find('.on');
		items.each(function(){
			var item = $(this);
			if(!item.hasClass('current')) item.removeClass('on')
		});
		return;
	}
}
var abreEnVentanaNueva = {
	isFicha: false,
	idFichaCatalogo: 'fichaCatalogo',
	idFichaRecurso: 'fichaRecurso',
	init: function(){
		this.body = $('body');
		if(this.body.hasClass(this.idFichaCatalogo) || this.body.hasClass(this.idFichaRecurso)) this.isFicha = true;
		if(!this.isFicha) return;
		this.config();
		this.comportamiento();
		return;
	},
	config: function(){
		this.recurso = this.body.find('#col02 .resource').first();
		this.title = this.recurso.find('.title');
		this.enlace = this.title.find('a');
		return;
	},
	comportamiento: function(){
		var html = '<span class=\"icono\" title=\"enlace recurso externo\"></span>';
		var isExterno = $(this.enlace).attr('target') == '_blank';
		if(isExterno) this.enlace.append(html);
		return;
	}
}
var redesSocialesRecursoCompactado = {
	idRedesSociales: '.redesSocialesCompartir',
	init: function(){
		this.config();
		this.ocultar();
		this.engancharComportamiento();
	},
	config: function(){
		this.redes = $('#content ' + this.idRedesSociales).first();
		this.ul = this.redes.find('ul');
		this.lis = this.redes.find('li');
		return;
	},
	ocultar: function(){
		this.lis.each(function(){
			var li = $(this);
			if(!li.hasClass('big') && !li.hasClass('mostrar')) li.hide();
		});
		this.isRedesOcultas = true;
		return;
	},
	mostrar: function(){
		this.lis.each(function(){
			var li = $(this);
			if(!li.hasClass('big') && !li.hasClass('mostrar')) li.show();
		});
		this.isRedesOcultas = false;
		return;
	},	
	engancharComportamiento: function(){
		var that = this;
		this.enlace = $('<a href="#">').text('mostrar');
		this.liMostrar = $('<li>').attr('class','mostrarMas').append(this.enlace);
		this.ul.append(this.liMostrar);
		this.enlace.bind('click', function(evento){
			evento.preventDefault();
			that.comportamiento(evento);
		})
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		if(this.isRedesOcultas){
			this.mostrar();
			enlace.parent().addClass('menos');
		}else{
			this.ocultar();
			enlace.parent().removeClass('menos');		
		}
		return;
	}
}

var seleccionarPreferencias = {
    componentes: ['.fieldset01', '.fieldset02', '.fieldset03', '.fieldset04', '.fieldset05', '.fieldset06', '.fieldset07', '.fieldset08', '.fieldset09', '.fieldset010', '.fieldset011'],
	init:function(){
		var that = this;
		$.each(this.componentes, function(indice, valor){
			var item = $(valor);
			var items = item.find('li');
			var labels = item.find('label');
			var checks = item.find('input[type=checkbox]');
			items.each(function(contador){
				var item = $(this);
				item.addClass('item' + indice + contador);
			});
			item.addClass('activo');
			items.bind('click', function (evento) {
			    evento.preventDefault();
				that.comportamiento(evento);
			});
			labels.bind('click', function(evento){
				evento.preventDefault();
			});		
		});
	},
	changeCheck: function(li){
		var li = $(li);
		var check = li.find('input[type=checkbox]');
		li.hasClass('on') ? li.removeClass('on') : li.addClass('on');
		check.is(':checked') ? check.attr('checked', false) : check.attr('checked', true);
		return;
	},
	comportamiento:function(evento){
		var item = $(evento.target);
		var name = evento.target.nodeName;
		var li = item;
		if(name != 'LI') li = item.parents('li').first();
		this.changeCheck(li);
		return;
	}
}
var marcarObligatorios = {
	init: function(){
		this.datosTipoTexto();
		return;
	},
	datosTipoTexto: function(){
	    var campos = $('.fieldset01 input[type=text], .fieldset01 input[type=password], .fieldset01 select.dato');
		campos.each(function(indice){
			var campo = $(this);
			var padre = campo.parent();
			var label = padre.find('label');
			if(label.hasClass('datoObligatorio')){
				label.attr('title', 'campo obligatorio');
				label.prepend('<span class="datoObligatorio">*</span>');
			}
		})
		return;
	}
}
var customizeFile = {
	init: function(){
		$('.customizeFileUpload').each(function(){
			var contenedor = $(this);
			var enlaces = contenedor.find('a.cambiar');
			var selectorFile = contenedor.attr('rel');
			var contenedorFile = $('.' + selectorFile).hide();
			enlaces.bind('click', function(evento){
				evento.preventDefault();
				var enlace = $(evento.target);
				var contenedor = enlace.parents('.customizeFileUpload');
				var selectorFile = contenedor.attr('rel');
				var contenedorFile = $('.' + selectorFile);
				var selectorFile = contenedor.attr('rel');
				var file = contenedorFile.find('input[type="file"]');
				file.trigger('click');
			});								
		})
	}
};
var ajusteFechaPublicador = {
	init: function(){
		$('.publicacion').each(function(indice){
			var item = $(this);
			var recurso = item.parents('.resource');
			var author = recurso.find('.author');
			var hasAuthor = author.size() > 0;
			if(hasAuthor){
				item.addClass('enCajaAuthor');
				author.append(item)
			}
		});	
	}
}
var desplegableGenerico = {
	idView: '#view',
	cssItemDesplegable: 'desplegable',
	lis: [],
	init: function(){
		this.config();
		this.view.addClass('activado');
		if(this.view <= 0) return;
		this.enganchar();
	},
	config: function(){
		var that = this;
		this.view = $(this.idView);
		var li = this.view.find('li').first();
		var lis = li.siblings();
		this.lis.push(li);
		lis.each(function(indice, valor){
			var li = $(this);	
			if(li.text() == ''){
				li.remove();
			}else{
				that.lis.push(li);
			}
		})
		return;
	},
	ocultar: function(){
		$.each(this.lis, function(indice){
			var item = this;
			if(!item.hasClass('current')) item.addClass('off');
		})
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		enlace.blur();
		var li = enlace.parents('li').first();
		li.addClass('current');
		this.ocultar();
		li.hasClass('off') ? li.removeClass('off') : li.addClass('off');
		li.removeClass('current');
		return;
	},	
	enganchar: function(){
		var that = this;
		$.each(this.lis, function(indice){
			var item = this;
			var enlace = item.find('a').first();
			var ul = item.find('ul');
			var lis = ul.find('li');
            // Deprecado size()
            //if (ul.size() > 0) {
            if (ul.length > 0) {
                // Deprecado size()
                //if (lis.size() > 0) {
                if (lis.length > 0) {
					enlace.addClass('principal');
					item.addClass('withOpciones off');
					enlace.bind('click', function(evento){
						evento.preventDefault();
						that.comportamiento(evento);
					})
				}else{
					item.hide();
				}
			}
		})
		return;
	}
}

// Cambiado por antiguo front
// Lo eliminar�
/*var marcarPasosFormulario = {
	init: function(){
		this.config();
		this.marcarItems();
		return;
	},
	config: function(){
		this.content = content;
		this.wraper = this.content.find('.formSteps').first();
		this.pasos = this.wraper.find('li');
		return;
	},
	marcarItems: function(){
		var items = this.pasos.size();
		this.pasos.each(function(indice){
			var item = $(this);
			item.addClass('item item0' + (indice + 1));
			if(indice == 0) item.addClass('first');
			if(indice == (items - 1)) item.addClass('last');
			if(item.hasClass('activo')){
				item.parent().addClass('activoItem0' + (indice + 1));
			}
		})
		return;
	}
	
}*/

var presentacionVotosRecurso = {
    isFicha: false,
    init: function () {
        var that = this;
        this.config();
        if (!this.isFicha) return;
        // Deprecado size()
        //if (this.enlace.size() <= 0) return;
        if (this.enlace.length <= 0) return;
        /*/*setTimeout(function(){
        that.traerUsuariosVotosRecursosSencillo();
        }, 1000)*/
        //WebForm_DoCallback('ctl00$ctl00$CPH1$CPHContenido$controles_ficharecursoinevery_ascx', 'VerVotantesSimple', ReceiveServerData, '', null, false);
        this.botonCerrarPanelAmpliado();
        this.enmascarar();
        this.enchangar();
        return;
    },
    config: function () {
        this.body = body;
        this.isFicha = this.body.hasClass('fichaComunidad') || this.body.hasClass('fichaComunidad');
        this.page = page;
        this.content = content;
        this.columna = this.content.find('#col02');
        this.resource = this.columna.find('.resource').first();
        this.utils = this.resource.find('.utils-1').addClass('js-activado');
        this.enlace = this.utils.find('.votosPositivos a');
        this.panelSencillo = this.utils.find('#panelVotosSimple');
        this.panelAmpliado = this.utils.find('#panelVotosAmpliado');
        return;
    },
    posicionar: function () {
        var ancho = $(window).width();
        var alto = $(window).height();
        var top = $(window).scrollTop();
        var anchoPanel = this.columna.width();
        var left = (ancho - anchoPanel) / 2;
        if (left < 0) left = 0;
        this.mascara.css({
            'width': ancho + 'px',
            'height': alto + 'px',
            'top': top + 'px'
        });
        this.panelAmpliado.css({
            'left': left + 'px',
            'top': (top + 100) + 'px'
        });
        return;
    },
    enmascarar: function () {
        this.mascara = $('<div>').addClass('mascaraPanelAmpliado').hide();
        this.body.append(this.mascara);
        return;
    },
    botonCerrarPanelAmpliado: function () {
        var that = this;
        var parrafo = $('<p>').addClass('cerrarPanelAmpliado');
        var enlace = $('<a>').attr('href', '#cerrarPanelAmpliado').text('cerrar');
        parrafo.append(enlace);
        this.panelAmpliado.append(parrafo);
        enlace.bind('click', function (evento) {
            evento.preventDefault();
            var enlace = $(evento.target);
            that.body.css("overflow", "auto");
            that.panelAmpliado.hide();
            that.mascara.hide();
        })
        return;
    },
    traerUsuariosVotosRecursosSencillo: function () {
        var that = this;
        ////var url = 'includes/recurso/usuariosVotaronRecursoSencillo.php';
        //WebForm_DoCallback('ctl00$ctl00$CPH1$CPHContenido$controles_ficharecursoinevery_ascx', 'VerVotantesSimple', ReceiveServerData, '', null, false);

        /////
        /*var respuesta = $.ajax({
        url: url,
        type: "GET",
        dataType: "html"
        })
        .done(function(pagina) {
        var html = pagina;
        that.panelSencillo.children().html(html);	
        return;
        })
        .fail(function( jqXHR, textStatus ) {
        alert( "Request failed: " + textStatus );
        })*/
        return;
    },
    traerUsuariosVotosRecursos: function () {
        //if ($('#ctl00_ctl00_CPH1_CPHContenido_controles_ficharecurso_ascx_divResource').length > 0) {
        //    WebForm_DoCallback('ctl00$ctl00$CPH1$CPHContenido$controles_ficharecurso_ascx', 'VerVotantes', ReceiveServerData, '', null, false);
        //}
        //else {
        //    WebForm_DoCallback('ctl00$ctl00$CPH1$CPHContenido$controles_ficharecursoinevery_ascx', 'VerVotantes', ReceiveServerData, '', null, false);
        //}
        var that = this;
        var url = document.location.href + '/load-voters';
        //var respuesta = $.ajax({
        //    url: url,
        //    type: "POST",
        //    dataType: "html"
        //})
        GnossPeticionAjax(url, null, true)
        .done(function (html) {
            that.panelAmpliado.find('.wrap').first().html(html);	
            CompletadaCargaUsuariosVotanRecurso();
            return;
        })
        //.fail(function( jqXHR, textStatus ) {
        //alert( "Request failed: " + textStatus );
        //})
        return;
    },
    enchangar: function () {
        var that = this;
        this.enlace.bind('click', function (evento) {
            evento.preventDefault();
            $(window).scrollTop(0);
            that.panelSencillo.hide();
            that.posicionar();
            that.body.append(that.panelAmpliado);
            that.body.css("overflow", "hidden");
            that.panelAmpliado.show();
            that.mascara.show();
            if (that.panelAmpliado.hasClass("no-data-panel")) {
                that.traerUsuariosVotosRecursos();
                /*setTimeout(function(){
                that.traerUsuariosVotosRecursos();
                }, 800);*/
                //if ($('#ctl00_ctl00_CPH1_CPHContenido_controles_ficharecurso_ascx_divResource').length > 0) {
                //    WebForm_DoCallback('ctl00$ctl00$CPH1$CPHContenido$controles_ficharecurso_ascx', 'VerVotantes', ReceiveServerData, '', null, false);
                //}
                //else {
                //    WebForm_DoCallback('ctl00$ctl00$CPH1$CPHContenido$controles_ficharecursoinevery_ascx', 'VerVotantes', ReceiveServerData, '', null, false);   
                //}
            }
            that.panelAmpliado.removeClass("no-data-panel");
        });
        this.enlace.hover(
			function () {
			    var enlace = $(this);
			    if (!that.panelAmpliado.is(':visible')) that.panelSencillo.show();
			},
			function () {
			    var enlace = $(this);
			    that.panelSencillo.hide();
			}
		);
        $(window).scroll(function () {
            if (that.panelAmpliado.is(':visible')) $(window).scrollTop(0);
        })
        return;
    },
    comportamiento: function () {
        return;
    }
}
/**
 * Clase jquery para poder gestionar la aparici� y login de una vista modal para que el usuario haga login.
 * Aparecer� siempre y cuando el usuario realice una acci�n y no disponga de permisos para ejecutarla.
 * */
var operativaLoginEmergente = {
    /**
     * Acci�n que dispara directamente el panel modal de Login
     */
    init: function () {
        this.config();
        this.showModal();
        this.doHashManagement();
        this.configEvents();
    },
    /*
     * Acci�n de cerrar la vista modal 
     */
    closeModal: function () {
        $(this.idModalPanel).modal('toggle');
		return;
    },
    /*
     * Acci�n de mostrar la vista modal
     * */
    showModal: function () {
        $(this.idModalPanel).modal('show');
        return;
    },
    /*
     * Opciones de configuraci�n de la vista con el formulario modal
     * */
    config: function () {
        // Inicializar las vistas cuando est�n visibles
        // Panel Modal
        this.idModalPanel = '#modal-login',
        // Referencia al formulario
        this.idForm = '#formPaginaLogin';
        this.bodyClassNameRegistro = 'operativaRegistro';

        // Captar el formulario Login si se est� en la p�gina Login. Si no est� en p�gina Login --> Coger el formulario del modal
        this.form = this.isLoginCurrentPage ? $('body').find(this.idForm) : $(this.idModalPanel).find(this.idForm);

        // Inputs y botones
        this.idInputEmail = '#usuario_Login',
        this.inputEmail = $(this.form).find(this.idInputEmail),
        this.idInputPassword= '#password_login',    
        this.inputPassword= $(this.form).find(this.idInputPassword),
        this.idButtonLogin= '#btnSubmit',
        this.buttonLogin= $(this.form).find(this.idButtonLogin),	
        // Paneles de error
        this.idLoginPanelError= '#loginError .ko',
        this.loginPanelError= $(this.form).find(this.idLoginPanelError),
        this.idLoginPanelErrorTwice= '#logintwice .ko',
        this.loginPanelErrorTwice= $(this.form).find(this.idLoginPanelErrorTwice),
        this.idLoginErrorAutenticacionExterna = '#loginErrorAutenticacionExterna .ko',
        this.loginErrorAutenticacionExterna = $(this.form).find(this.idLoginErrorAutenticacionExterna),
        this.panelesError = [this.loginPanelError, this.loginPanelErrorTwice, this.loginErrorAutenticacionExterna];
    },

    /**
    * Configuraci�n de los eventos (clicks, focus) de los inputs del panel/formulario  
    */
    configEvents: function () {
        // Referencia al 'emergente panel login'
        const that = this;

        // Bot�n Login
        this.buttonLogin.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.panelesError.forEach(panelError => panelError.hide());
            // Hacer login solo si los datos han sido introducidos
            if (that.validarCampos() == true) {
                // Mostrar loading
                MostrarUpdateProgress();
                that.form.submit();
            } else {
                that.loginPanelError.show();
            }
        });

        // Input Password (Hacer login si se pulsa "Enter" desde input password)
        this.inputPassword.keypress(function (event) {
            event.keyCode === 13 ? that.buttonLogin.click() : null
        });

    },
 
    /**
    * Comprobar que los campos (email y password) no est�n vac�os
    * @returns {bool}    
    */
    validarCampos: function () {
        return (this.inputEmail.val() != '' && this.inputPassword.val() != '');
    },

    /**
    * Comprobar si la p�gina actual es la p�gina principal de Login de una comunidad
    * Se puede saber comprobando si se dispone de una clase en el Login concreta    
    * @returns {bool}    
    */
    isLoginCurrentPage: function () {
        return $('body').hasClass(this.bodyClassNameRegistro);
    },

    /**
    * Gest�n de los Hash que hac�a en la p�gina Login anterior -> Gesti�n de errores
    * Solo ha de ejecutarse cuando el usuario se encuentre en la p�gina Login
    */
    doHashManagement: function () {
        if (this.isLoginCurrentPage()) {
            if (ObtenerHash() == '#error') {
                this.loginPanelError.show();
            }
            else if (ObtenerHash().indexOf('&') > 0) {
                var mensajeError = ObtenerHash().split('&')[1];
                if (mensajeError != '') {
                    $('#mensajeError').text(mensajeError);
                    this.loginPanelError.show();
                }
            }
            else if (document.location.href.endsWith('logintwice')) {
                this.loginPanelErrorTwice.show();
            }
            if (ObtenerHash() == '#errorAutenticacionExterna') {
                this.loginErrorAutenticacionExterna.show();
            }
        }        
    },
};

/**
 * Comprobar si el bot�n de may�sculas est� activado
 *
 * @param {Object} e A keypress event
 * @returns {Boolean} isCapsLock
 */
function isCapsLock(e) {
    e = (e) ? e : window.event;

    var charCode = false;
    if (e.which) {
        charCode = e.which;
    } else if (e.keyCode) {
        charCode = e.keyCode;
    }

    var shifton = false;
    if (e.shiftKey) {
        shifton = e.shiftKey;
    } else if (e.modifiers) {
        shifton = !!(e.modifiers & 4);
    }

    if (charCode >= 97 && charCode <= 122 && shifton) {
        return true;
    }

    if (charCode >= 65 && charCode <= 90 && !shifton) {
        return true;
    }

    return false;
};

/**
 * Clase jquery para poder gestionar el proceso de registro de un usuario
 * Este proceso en cuesti�n se encarga de la gesti�n del paso 1 de registro del usuario
 * Ej: Proceso de registro al acceder a la url: http://depuracion.net/comunidad/gnoss-developers-community/hazte-miembro
 * */
const operativaRegistroUsuarioPaso1 = {    
    /**
     * Acci�n para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },

    /*
     * Opciones de configuraci�n de la vista con todas los inputs necesarios para realizar el registro del usuario
     * */
    config: function (pParams) {
        // Inicializaci�n de las vistas    
        this.txtFechaNac = $(`#${pParams.idTxtFechaNac}`);
        this.txtFechaNacDia = $(`#${pParams.idTxtFechaNacDia}`);
        this.txtFechaNacMes = $(`#${pParams.idTxtFechaNacMes}`);
        this.txtFechaNacAnio = $(`#${pParams.idTxtFechaNacAnio}`);
        this.txtNombre = $(`#${pParams.idTxtNombre}`);
        this.nombreCampoTxtNombre = pParams.nombreCampoTxtNombre;
        this.txtApellidos = $(`#${pParams.idTxtApellidos}`);
        this.nombreCampoTxtApellidos = pParams.nombreCampoTxtApellidos;
        this.txtCargo = $(`#${pParams.idTxtCargo}`);
        this.lblCargo = pParams.lblCargo;
        this.txtEmail = $(`#${pParams.idTxtEmail}`);
        this.txtNombreUsuario = $(`#${pParams.idTxtNombreUsuario}`);
        this.txtEmailTutor = $(`#${pParams.idTxtEmailTutor}`);
        this.txtContrasenya = $(`#${pParams.idTxtContrasenya}`);
        this.nombreCampoTxtContrasenya = pParams.nombreCampoTxtContrasenya;
        this.captcha = $(`#${pParams.idCaptcha}`);
        this.ddlPais = $(`#${pParams.idDdlPais}`);

        this.mensajePersonalizado = pParams.mensajePersonalizado;
        this.currentUrl = pParams.currentUrl;
        this.datepickerConfigOptions = pParams.datepickerConfigOptions;
    },

    /**
    * Configuración de los eventos 
    */
    configEvents: function () {
        // Referencia al objeto
        const that = this;

        // Input para Nombre (Quitar foco del input)
        this.txtNombre.blur(function () {
            ComprobarCampoRegistroMVC(that.nombreCampoTxtNombre);
        });

        // Input para Apellidos (Quitar foco del input)
        this.txtApellidos.blur(function () {
            ComprobarCampoRegistroMVC(that.nombreCampoTxtApellidos);
        });

        // Input para Cargo (Quitar foco del input)
        this.txtCargo.blur(function () {
            ValidarCampoNoVacio(that.txtCargo, that.lblCargo);
        });

        // Input para Cargo (Quitar foco del input)
        this.txtEmail.blur(function () {
            ComprobarEmailUsuario(that.currentUrl);
        });   

        // Input para Contrase�a (Quitar foco del input)
        this.txtContrasenya.blur(function () {
            ComprobarCampoRegistroMVC(that.nombreCampoTxtContrasenya);
        });

        // Configuraci�n del datepicker
        this.txtFechaNac.datepicker({
            changeMonth: true,
            changeYear: true,
            yearRange: that.datepickerConfigOptions.yearRange,
            maxDate: that.datepickerConfigOptions.maxdate,
        });       
    },
};

/**
 * Clase para poder gestionar la edici�n de perfil de un usuario en la comunidad
 * 
 * */
const operativaEditarPerfilUsuario = {

    /**
     * Acci�n para inicializar elementos y eventos
     * @param {any} pParams
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents(pParams);
    },

    /**
     * Opciones de configuraci�n de la vista (recoger ids para poder interactuar)
     * @param {any} pParams
     */
    config: function (pParams) {

        // Inicializaci�n de las vistas
        this.name = $(`#${pParams.perfilPersonal.idName}`);
        this.lastName = $(`#${pParams.perfilPersonal.idLastName}`);
        this.email = $(`#${pParams.perfilPersonal.idEmail}`);
        this.emailProfesional = $(`#${pParams.perfilPersonal.idEmailProfesional}`);
        this.bornDate = $(`#${pParams.perfilPersonal.idBornDate}`);
        this.country = $(`#${pParams.perfilPersonal.idCountry}`);
        this.region = $(`#${pParams.perfilPersonal.idRegion}`);
        this.location = $(`#${pParams.perfilPersonal.idLocation}`);
        this.postalCode = $(`#${pParams.perfilPersonal.idPostalCode}`);
        this.lang = $(`#${pParams.perfilPersonal.idLang}`);
        this.sex = $(`#${pParams.perfilPersonal.idSex}`);
        this.emailTutor = $(`#${pParams.perfilPersonal.idEmailTutor}`);
        this.idIsSearched = $(`#${pParams.perfilPersonal.idIsSearched}`);
        this.idIsExternalSearched = $(`#${pParams.perfilPersonal.idIsExternalSearched}`);

        // Perfil profesional
        this.nameOrganization = $(`#${pParams.perfilPersonal.idNameOrganization}`);
        this.countryOrganization = $(`#${pParams.perfilPersonal.idCountryOrganization}`);
        this.postalCodeOrganization = $(`#${pParams.perfilPersonal.idPostalCodeOrganization}`);
        this.locationOrganization = $(`#${pParams.perfilPersonal.idLocationOrganization}`);
        this.aliasOrganization = $(`#${pParams.perfilPersonal.idAliasOrganization}`);
        this.websiteOrganization = $(`#${pParams.perfilPersonal.idWebsiteOrganization}`);
        this.addressOrganization = $(`#${pParams.perfilPersonal.idAddressOrganization}`);

        // Edici�n secci�n Bio (CV)
        this.description = $(`#${pParams.curriculum.idDescription}`);
        this.tags = $(`#${pParams.curriculum.idTags}`);

        //Edici�n secci�n Redes Sociales
        this.urlUsuario = $(`#${pParams.redesSociales.idUrlUsuario}`);
        this.tblRedesSociales = $(`#${pParams.redesSociales.idTblRedesSociales}`);
        this.btnRedSocial = $(`#${pParams.redesSociales.idBtnRedSocial}`);
        this.twitterSocial = $(`#${pParams.redesSociales.idTwitterSocial}`);
        this.facebookSocial = $(`#${pParams.redesSociales.idFacebookSocial}`);
        this.linkedinSocial = $(`#${pParams.redesSociales.idLinkedinSocial}`);

        // Se utilizar� la clase ya que hay muchos elementos para borrar (bot�n papelera con clase btnBorrarURL)
        this.classBorrarURL = pParams.redesSociales.idBtnBorrarUrl;
        this.btnBorrarUrl = $(`.${pParams.redesSociales.idBtnBorrarUrl}`);

        // Otros (Paneles, botones, url)
        this.divPanelInfo = $(`#${pParams.others.idDivPanelInfo}`);
        this.saveButton = $(`#${pParams.others.idSaveButton}`);
        this.urlPersonalProfileSaveProfile = pParams.others.urlPersonalProfileSaveProfile;
        this.urlPersonalProfileSaveBio = pParams.others.urlPersonalProfileSaveBio;
        this.urlPersonalProfileSaveSocialWebs = pParams.others.urlPersonalProfileSaveSocialWebs;

        // Inputs que NO podr�n quedar vac�os
        this.inputsNoEmpty = [this.name,
        this.lastName,
        this.email,
        this.bornDate,
        this.emailProfesional,
        this.nameOrganization,
        this.countryOrganization,
        this.postalCodeOrganization,
        this.locationOrganization,
        this.addressOrganization
        ];

        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";
    },

    /**
     * Validar que los campos aqu� mencionados no est�n vac�os
     * @param {any} inputs: Array de inputs para ser recorridos y verificar que ninguno de los aqu� indicados est�n vac�os
     * @returns {bool}: Devolver� true o false siempre y cuando los inputs pasados sean diferente de vac�o
     */
    validarCampos: function (inputs) {
        let areInputsOK = false;
        let error = "";

        for (input of inputs) {
            if (input.length > 0) {
                error = ValidarCampoNoVacio(input, undefined, true);
                if (error.length > 0) { break }
            }
        }

        if (error.length > 0) {
            this.showInfoPanelErrorOrOK(false, true, error);
        } else {
            areInputsOK = true
            this.showInfoPanelErrorOrOK(false, false, undefined);
        }
        return areInputsOK;
    },

    /**
     * Mostrar el panel informativo con un mensaje de error o ko. Si los dos son falsos, el panel quedar� oculto
     * @param {boolean} showOK: Si se desea mostrar el mensaje de OK
     * @param {boolean} showError: Si se desea mostrar el mensaje KO
     * @param {string} message: El mensaje que ir� en el panel informativo
     */
    showInfoPanelErrorOrOK: function (showOK, showError, message) {
        const that = this;

        // Mostrar panel OK
        that.divPanelInfo.html(message);
        if (showOK) {
            that.divPanelInfo.addClass(this.okClass);
            that.divPanelInfo.removeClass(this.errorClass);

        } else if (showError) {
            // Mostrar panel Error
            that.divPanelInfo.removeClass(this.okClass);
            that.divPanelInfo.addClass(this.errorClass);
        } else {
            // Ocultar el panel de mensajes
            this.divPanelInfo.hide();
            return
        }
        // Mostrar el panel
        this.divPanelInfo.show();
    },

    /**
     * Configuraci�n de los eventos de los elementos html (click, focus...)
     * */
    configEvents: function (pParams) {
        const that = this;

        // Valor cambiado de inputs -> Avisar al usuario con sobreado rojo (o quitarlo) si es vac�o campo obligatorio
        this.inputsNoEmpty.forEach(input => {
            input.on("change", function () {
                if ($(this).val().length == 0) {
                    $(this).addClass('is-invalid');
                } else {
                    $(this).removeClass('is-invalid');
                }
            });
        });

        // Bot�n de guardado de los datos
        this.saveButton.on("click", function () {
            if (that.validarCampos(that.inputsNoEmpty)) {
                // Guardar secci�n de datos personales (Nombre, Apellidos)
                that.savePersonalDataProfile();
                // Guardar secci�n de Curriculum (Tags, Descripcion)
                that.saveBioUserProfile(false);

            }
        });

        // Bot�n click para guardado de URL en perfil del usuario
        this.btnRedSocial.on("click", function () {
            that.addSocialWeb(that.urlUsuario.val());
        });

        // Pulsaci�n Enter para guardado de URL en perfil de usuario
        this.urlUsuario.keypress(function (event) {
            event.keyCode === 13 ? that.addSocialWeb(that.urlUsuario.val()) : null
        });

        // Bot�n/Icono de papelera para borrar una red social-web
        $(document).on("click", `.${that.classBorrarURL}`, function () {
            that.eliminarUrlRedSocial($(this));
        });

        // Evento change de país para que se carguen las C. Autónomas asociadas al país.
        this.country.change(function () {
            // Mostrado de Loading
            MostrarUpdateProgress();

            const dataPost = {
                callback: "CambiarPais",
                pais: $(this).val()
            }

            GnossPeticionAjax(
                location.href,
                dataPost,
                true
            ).done(function (response) {
                //that.region.parent().replaceWith(response);
                $(`#${pParams.perfilPersonal.idRegion}`).parent().replaceWith(response);
                // Reiniciar el combobox de select2 al haber sido creado de 0 siempre y cuando sea un elemento select
                if ($(`#${pParams.perfilPersonal.idRegion}`).prop('nodeName') == "SELECT") {
                    $(`#${pParams.perfilPersonal.idRegion}`).select2();
                }
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

    },

    /**
     * Acci�n de guardar los datos del perfil del secci�n 'Datos Personales' (Nombre, Apellidos...)
     * */
    savePersonalDataProfile: function () {
        const that = this;
        // Mostrado de Loading
        MostrarUpdateProgress();
        // Construcci�n de objeto formData
        const dataPost = new FormData();
        dataPost.append('peticionAJAX', true);

        // Recorrido de cada input para coger su name-value
        $("#formularioEdicionPerfilPersonal").find(':input').each(function () {
            let valor = "";
            if ($(this).is(':checkbox')) {
                valor = $(this).is(':checked')
            }
            else if (!$(this).is(':button')) {
                valor = $(this).val();
            }
            dataPost.append($(this).attr('name'), valor);
        });

        // Realizar petici�n de guardado de datos personales del perfil del usuario        
        GnossPeticionAjax(this.urlPersonalProfileSaveProfile, dataPost, true, false)
            .done(function (data) {
                //GuardadoCVRapido('OK'); 
                // Guardar las redes sociales si se han introducido en los inputs de Twitter, Facebook, Linkedin
                that.addSocialMedia();
                //that.showInfoPanelErrorOrOK(true, false, data);
                mostrarNotificacion('success', data);

            })
            .fail(function (data) {
                //that.showInfoPanelErrorOrOK(false, true, data);
                mostrarNotificacion('error', data);
            })
            .always(function () {
                OcultarUpdateProgress();
            });
    },

    /**
     * Acci�n de guardar los datos del perfil del usuario, secci�n 'Curriculum' (Tags, Descripci�n)   
     * */

    /**     
     * @param {boolean} isNecessaryToHaveTagsAndDescription: El API exige que haya al menos una descripci�n y un Tag para el guardado de estos datos. Tenerlo en cuenta. 
     * La idea es quitar esta restricci�n tal y como se ha comentado a Juan (29-06-2021)
     */
    saveBioUserProfile: function (isNecessaryToHaveTagsAndDescription) {

        const that = this;

        // Controlar que los items existan en la web (Organizaci�n no los suele cargar)
        if (this.tags.length > 0 && this.description.length > 0) {
            if (isNecessaryToHaveTagsAndDescription == true) {
                if (this.tags.val().length <= 1 && this.description.val().length == 0) {
                    return;
                }
            }
        } else {
            return;
        }
        // Mostrado de Loading
        MostrarUpdateProgress();


        // Construcci�n del objeto POST
        const dataPost = {
            Description: that.description.val(),
            Tags: that.tags.val()
        }
        GnossPeticionAjax(this.urlPersonalProfileSaveBio, dataPost, true, false)
            .done(function (data) {
            }).fail(function (data) {
                //GuardadoCVRapido('KO');            
            }).always(function () {
                OcultarUpdateProgress();
            });
    },

    /**
     * A�adir una determinada url al perfil del usuario
     * */
    addSocialWeb: function (urlUsuario, isNecessaryDisplayInTable = true) {
        // Mostrar loading
        const that = this;

        if (urlUsuario.length > 1) {
            MostrarUpdateProgress();

            const dataPost = {
                callback: "AnyadirRedSocial",
                url: urlUsuario,
            }

            GnossPeticionAjax(this.urlPersonalProfileSaveSocialWebs, dataPost, true, false)
                .done(function (data) {
                    // Obtener el nombre de la red social/web
                    data = data.substring("OK:".length);
                    if (isNecessaryDisplayInTable) {
                        that.montarUrlRedSocial(data, that.urlUsuario.val());
                        that.tblRedesSociales.removeClass('d-none');
                        that.urlUsuario.val('');
                    }
                    that.showInfoPanelErrorOrOK(false, false, undefined);
                }).fail(function (data) {
                    that.showInfoPanelErrorOrOK(false, true, data);
                }).always(function () {
                    OcultarUpdateProgress();
                });
        }
    },
    /**
     * Añadir una red social que se almacenará en los inputs correspondientes (Linkedin, Twitter, Facebook) al hacer click en "Guardar"
     * */
    addSocialMedia: function () {
        this.twitterSocial.val().length > 1 && this.addSocialWeb(this.twitterSocial.val(), false);
        this.facebookSocial.val().length > 1 && this.addSocialWeb(this.facebookSocial.val(), false);
        this.linkedinSocial.val().length > 1 && this.addSocialWeb(this.linkedinSocial.val(), false);
    },

    /**
     * Mostrar la url reci�n agregada en la tabla correspondiente de urls del usuario al haber sido agregada habiendo pulsado en el bot�n "A�adir"
     * @param {any} data: Nombre de la url. No se refiere a la URL o direcci�n de la web a�adida, sino al nombre propiamente dicho
     * @param {any} url: Url a�adida por el usuario
     */
    montarUrlRedSocial: function (data, url) {
        const htmlFila = `
            <tr>
              <td><img src="https://www.google.com/s2/favicons?domain=${url}" /></td>
              <td class="urlName">${data}</td>
              <td class="urlTitle">${url}</td>
              <td><span data-urlName="${data}"  class="material-icons-outlined ${this.btnBorrarUrl.attr('id')}" style="cursor: pointer" title="Eliminar" alt="Eliminar">delete</span></td>
            </tr>
        `;

        // Agregar la fila de la red social a la tabla
        this.tblRedesSociales.find('tbody').append(htmlFila);
    },

    /**
     * Acci�n de eliminar una URL del servidor y tambi�n de la tabla de url del usuario     
     * @param {any} btnDeleteUrl: Bot�n de borrado pulsado
     */
    eliminarUrlRedSocial: function (btnDeleteUrl) {
        const that = this;

        // Nombre de la red que se desea borrar
        const nombreRed = btnDeleteUrl.data("urlname");

        // Mostrar Loading
        MostrarUpdateProgress();
        // Construcci�n del objeto dataPost
        const dataPost = {
            callback: "EliminarRedSocial",
            nombreRed: nombreRed,
        }

        GnossPeticionAjax(this.urlPersonalProfileSaveSocialWebs, dataPost, true, false)
            .done(function (data) {
                // Eliminar la red social/web de la tabla
                var tBody = btnDeleteUrl.closest("tbody");
                // Eliminar la row de la tabla                
                btnDeleteUrl.fadeOut("normal", function () {
                    $(this).closest("tr").remove();
                    if ($('tr', tBody).length == 0) {
                        that.tblRedesSociales.addClass('d-none');
                    }
                });

                that.showInfoPanelErrorOrOK(false, false, undefined);
            }).fail(function (data) {
                that.showInfoPanelErrorOrOK(false, true, data);
            }).always(function () {
                OcultarUpdateProgress();
            });
    }
};

/**
 * Clase jquery para poder gestionar la solicitud de cambio de contrase�a de un usuario
 * 
 * */
const operativaSolicitarCambiarContrasenia = {
    /**
     * Acci�n para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },
    /*
     * Opciones de configuraci�n de la vista con el formulario modal
     * */
    config: function (pParams) {
        // Inicializaci�n de IDS de las vistas        
        this.idTxtOldPassword = "#txtOldPassword";
        this.idTxtNewPassword = "#txtNewPassword";
        this.idTxtConfirmedPassword = "#txtConfirmedPassword";
        this.idBtnCambiarPassword = "#btnCambiarPassword";
        this.idWarningPanel = "#warning";
        this.idExpiredPanel = "#expiredPanel";
        this.idPasswordEmptyPanel = "#passwordEmptyPanel";
        this.idPasswordRequestInfoPanel = "#passwordRequestInfoPanel";

        // Url necesarias
        this.urlPasswordRequest = pParams.urlPasswordRequest;

        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";

        // Inicializaci�n de las vistas        
        this.txtOldPassword = $(this.idTxtOldPassword);
        this.txtConfirmedPassword = $(this.idTxtConfirmedPassword);
        this.txtNewPassword = $(this.idTxtNewPassword);
        this.btnCambiarPassword = $(this.idBtnCambiarPassword);
        this.warningPanel = $(this.idWarningPanel);
        this.passwordEmptyPanel = $(this.idPasswordEmptyPanel);
        this.passwordRequestInfoPanel = $(this.idPasswordRequestInfoPanel);
        this.panelesError = [this.warningPanel, this.passwordEmptyPanel];
        this.inputsFields = [this.txtOldPassword, this.txtConfirmedPassword, this.txtNewPassword];
    },

    /**
    * Comprobar que los campos (password old, password new y password confirmado ) no est�n vac�os
    * Comprobar que los password new y confirmado son iguales. En caso contrario, mostrar� un error
    * @returns {bool}    
    */
    validarCampos: function () {
        let isPasswordValid = false;
        if (this.txtOldPassword.val() != '' && this.txtNewPassword.val() != '' && this.txtConfirmedPassword.val() != '') {
            if (this.txtNewPassword.val() === this.txtConfirmedPassword.val()) {
                isPasswordValid = true;
            }
        } else {
            isPasswordValid = false;
        }
        return isPasswordValid;
    },

    /**
    * Configuraci�n de los eventos 
    */
    configEvents: function () {
        // Referencia al objeto
        const that = this;

        // Input Password Confirmado ENTER
        this.txtConfirmedPassword.keypress(function (event) {
            // Avisar con un mensaje si est�n activadas las may�sculas del teclado
            isCapsLock(event) ? that.warningPanel.fadeIn("slow") : that.warningPanel.fadeOut("slow");
            event.keyCode === 13 ? that.btnCambiarPassword.click() : null
        });

        // Bot�n de Aceptar - Solicitar cambio de contrase�a         
        this.btnCambiarPassword.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideErrorPanels();
            // Hacer login solo si los datos han sido introducidos
            if (that.validarCampos() == true) {
                // Realizar la petici�n de cambio de contrase�a
                that.cambiarPassword();
            } else {
                that.passwordEmptyPanel.fadeIn("slow");
            }
        });
    },

    /**
     * Ocultar los paneles informativos de errores
     * */
    hideErrorPanels() {
        this.panelesError.forEach(panelError => panelError.hide());
    },

    /**
      * Vaciar de los inputs utilizados (Ej: Una vez efectuada la solicitud, vaciar los inputs)
      */
    emptyInputs() {
        this.inputsFields.forEach(input => input.val(''));
    },

    /**
     * Funci�n para realizar la petici�n del cambio de contrase�a solicitado por el usuario
     * */
    cambiarPassword: function () {
        // Referencia al objeto
        const that = this;
        this.btnCambiarPassword.hide();
        MostrarUpdateProgress();
        // Construcci�n del objeto con los passwords
        const params = {
            OldPassword: that.txtOldPassword.val(),
            NewPassword: that.txtNewPassword.val(),
            ConfirmedPassword: that.txtConfirmedPassword.val(),
        };
        // Realizar la petici�n de cambio de password
        GnossPeticionAjax(that.urlPasswordRequest, params, false)
            .done(function () {
                // Ocultar posibles paneles de error
                that.hideErrorPanels();

                // Mostrar panel info con su clase
                that.passwordRequestInfoPanel.addClass(that.okClass);
                that.passwordRequestInfoPanel.removeClass(that.errorClass);
                var transfer = that.getParam("transferto", location.href);
                if (transfer != undefined) {
                    location.href = transfer;
                }
            })
            .fail(function () {
                // Mostrar panel info con su clase
                that.passwordRequestInfoPanel.removeClass(that.okClass);
                that.passwordRequestInfoPanel.addClass(that.errorClass);
            })
            .always(function (html) {
                // Mostrar el mensaje de success/error
                that.passwordRequestInfoPanel.html(html);
                // Mostrar el mensaje de error o de success
                that.passwordRequestInfoPanel.fadeIn("slow");
                // Mostrar de nuevo el bot�n para solicitar cambio de contrase�a
                that.btnCambiarPassword.fadeIn("slow");
                // Vaciar los inputs
                that.emptyInputs();
                // Ocultar el loading
                OcultarUpdateProgress();
            });
    },

    /**
     * Funci�n ejecutada desde cambiarPassword 
     * @param {any} param
     * @param {any} url
     */
    getParam: function (param, url) {
        /* Buscar a partir del signo de interrogaci�n ? */
        url = String(url.match(/\?+.+/));
        /* limpiar la cadena quit�ndole el signo ? */
        url = url.replace("?", "");
        /* Crear un array con parametro=valor */
        url = url.split("&");
        /*
        Recorrer el array url
        obtener el valor y dividirlo en dos partes a trav�s del signo =
        0 = parametro
        1 = valor
        Si el par�metro existe devolver su valor
        */
        x = 0;
        while (x < url.length) {
            p = url[x].split("=");
            if (p[0] == param) {
                return decodeURIComponent(p[1]);
            }
            x++;
        }
    }
};

/*
Plugin de CKEditor simplificado. Se activará sobre cualquier input que tenga la clase cke y cuyo "sibling" (activador del ckEditor) también tenga la clase 'ckeSimple'.
- El plugin hará que:
    - La "ToolBar" no esté visible si no se hace click en el input para añadir información.
    - La "ToolBar" esté visible si se hace click en el editor CKEditor.
    - La altura del CKEditor sea de 100px y si se está editando, que sea de 200px.
    - Si hay contenido, se mostrará el ckEditor. Si por el contrario está vacío, se verá de momento un único input.
*/
(function ($) {
    // Declaración del plugin.
    $.fn.ckEditorSimple = function (options) {

        const ckEditorSimplePlugin = $.fn.ckEditorSimple;

        options = $.extend(
            {
                $ckEditor: undefined,                            // El propio ckEditor
                $ckEditorInstance: undefined,                   // La instancia jquery del CKEditor creado
                $ckEditorToolbar: undefined,                    // Toolbar del CKEditor
                $ckEditorContent: undefined,	                // Content del CKEditor
                ckeEditorContentLength: 0,                      // Longitud de textos que hay en el editor CKEditor
            },
            options
        );

        // Variables utilizadas en métodos del plugin
        const $ckEditor = options.$ckEditor;
        const $ckEditorInstance = options.$ckEditorInstance;
        const $ckEditorToolbar = options.$ckEditorToolbar;
        const $ckEditorContent = options.$ckEditorContent;
        const ckeEditorContentLength = options.ckeEditorContentLength;

        // Variables adicionales 
        const speed = 200;                                      // Velocidad con la que se expandirá el panel 'contents'
        let isCKEditorContentPanelExpanded = true;              // Valor por defecto que indica que el panel Contents está o no expandido
        let isCKEditorToolBarVisible = true;                    // Valor por defecto que indica que la Toolbar está o no visible
        let isOnFocusCKEditor = false;                          // Valor por defecto que indica si está el foco en el ckEditor (El usuario está introduciendo datos)
        const contentMinHeight = '40px';                        // Altura mínima de Contentl del CKEditor (contraido)
        const contentMaxHeight = '200px';                       // Altura máxima del Content del CKEditor (desplegado)
        const $ckEditorBody = $ckEditor.document.getBody();     // Contenido del iframe donde se añade el contenido deseado (Texto, imágenes)

        /*
         * Método para extender o hacer más grande o más pequeño el panel Contents de CKEditor (donde irá el contenido)
         * @param {bool} animate: Indicar si se desea animar o no
         * @param {bool} extendPanel: Hacer más grande o no el Content del CKEditor         
         * */
        $.fn.ckEditorSimple.animateHeightCKEditor = function (animate, extendPanel) {

            // Altura que tendrá el panel
            const panelHeight = extendPanel ? contentMaxHeight : contentMinHeight;

            // Quitar posible scrollbar si el panel está contraido
            extendPanel ? ckEditorSimplePlugin.showScrollbar(true) : ckEditorSimplePlugin.showScrollbar(false);

            // Controlar animación (true/false)
            if (animate) {
                $ckEditorContent.animate({
                    height: panelHeight
                }, speed, function () {
                    // Guardamos flag de que el tamaño del panel ha sido modificado
                    isCKEditorContentPanelExpanded = !isCKEditorContentPanelExpanded;
                });
            } else {
                // Establecer tamaño sin animación               
                $ckEditorContent.css('height', panelHeight);
                // Guardamos flag de que el tamaño del panel ha sido modificado
                isCKEditorContentPanelExpanded = !isCKEditorContentPanelExpanded;
            }
        }

        /*
         * Método mostrar u ocultar el scrollbar del contenido del ckeditor. Solo estaría disponible si el editor está desplegado (Por si hay contenido añadido)         
         * @param {bool} showScrollbar: Ocultar o permitir el scrollbar en el contenido del CKEditor
         * */
        $.fn.ckEditorSimple.showScrollbar = function (showScrollBar) {
            showScrollBar ? $ckEditorBody.removeClass('overflow-hidden') : $ckEditorBody.addClass('overflow-hidden');
        }

        /*
         * Método para visualizar o no el Toolbar del CKEditor
         * @param {bool} animate: Indicar si se desea animar o no la desaparición del Toolbar         
         * */
        $.fn.ckEditorSimple.animateToolbarCKEditor = function (animate, extendPanel) {

            // Si hay texto no hacer nada
            if (($ckEditor.document.getBody().getText().length > 1)) {
                return;
            }

            // Altura que tendrá el panel
            const panelHeight = extendPanel ? 31 : 0;

            // Controlar animación (true/false)
            if (animate) {
                $ckEditorToolbar.animate({
                    height: panelHeight
                }, 300, function () {
                    // Guardamos flag de que el tamaño del panel ha sido modificado
                    isCKEditorToolBarVisible = !isCKEditorToolBarVisible;
                    // Animación visualización del $ckEditorContent
                    ckEditorSimplePlugin.animateHeightCKEditor(animate, extendPanel);
                });
            } else {
                // Establecer tamaño sin animación               
                $ckEditorToolbar.css('height', panelHeight);
                // Guardamos flag de que el tamaño del panel ha sido modificado
                isCKEditorToolBarVisible = !isCKEditorToolBarVisible;
                // Establecer visualización del ToolbarCKEditor sin animación
                ckEditorSimplePlugin.animateHeightCKEditor(animate, extendPanel);
            }
        }

        // Primera ejecución para aplicar secciones de CKEditor -> Dejarlo desplegado si ya hay contenido (Ej: Editar un recurso) 
        if ( !(ckeEditorContentLength > 1) ) {
            ckEditorSimplePlugin.animateToolbarCKEditor(false, !isCKEditorContentPanelExpanded);
        }        

        // Configuración del focus en la instancia del CKEditor para ocultar/no ocultar Toolbar y expandir/contraer el Contents de CKEditor
        // Observo posibles cambios en las clases del editor de CKEditor
        let observer = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.attributeName === "class") {
                    let attributeValue = $(mutation.target).prop(mutation.attributeName);
                    const arrayClassList = attributeValue.split(" ");                    
                    // Buscar si está o no focus en el CKEditor
                    if (jQuery.inArray("cke_focus", arrayClassList) != -1) {
                        // Está el Foco del CKEditor
                        isOnFocusCKEditor = true
                    } else {
                        // No está el Foco del CKEditor
                        isOnFocusCKEditor = false;
                    }
                    // Animar altura CKEditor siempre que sea diferente al valor actual
                    isCKEditorContentPanelExpanded != isOnFocusCKEditor && ckEditorSimplePlugin.animateToolbarCKEditor(true, !isCKEditorToolBarVisible);
                }
            });
        });

        // Ejecutamos observer para comprobar posibles cambios en clases JS
        observer.observe($ckEditorInstance[0], {
            attributes: true
        });
    }
})(jQuery);


/**
 * Clase jquery para poder gestionar las búsquedas de fechas en Facetas (Mes pasado, semana pasada, semestre pasado)
 * Calculará la fecha teniendo en cuenta la opción pulsada para escribir el valor en el input "Desde" y "Hasta"
 * Se utilizará la librería moment.js para el trabajo con fechas
 * 
 * */
var operativaFechasFacetas = {

    /**
     * Acción que dispara directamente la lógica de operativaFechas
     */
    init: function () {
        this.config();
        this.configEvents();
    },

    /**
     * Configuración de los elementos que tendrán eventos
     */
    config: function () {
        // Opción buscador de fechas "Última semana"    
        this.optionLastWeek = `.facet-last-week`,
            // Opción buscador de fechas "Último mes"    
            this.optionLastMonth = `.facet-last-month`,
            // Opción buscador de fechas "Último semestre"    
            this.optionLastSemester = `.facet-last-semester`,
            // Opción buscador de fechas "Último semestre"    
            this.optionLastYear = `.facet-last-year`,
            // Botón de buscar 
            this.searchButton = `.searchButton`,
            this.dropdownMenu = `.dropdown-menu`,
            // Input from-to
            this.inputFromDate = `.facet-from`,
            this.inputToDate = `.facet-to`;

    },

    /**
     * Configuración eventos de elementos HTML
     * */
    configEvents: function () {

        const that = this;

        // Opción LastWeek
        $(document).on('click', this.optionLastWeek, function (event) {
            that.getAndSetDate(this);
        });
        // Opción LastMonth
        $(document).on('click', this.optionLastMonth, function (event) {
            that.getAndSetDate(this);
        });
        // Opción Semester
        $(document).on('click', this.optionLastSemester, function (event) {
            that.getAndSetDate(this);
        });
        // Opción Year
        $(document).on('click', this.optionLastYear, function (event) {
            that.getAndSetDate(this);
        });
    },

    /**
     * Calcular el plazo de tiempo deseado y establecerlo en los inputs "from" y "to"
     * */
    getAndSetDate: function (item) {
        let localLocale = moment();
        moment.locale('es');
        localLocale.locale(false);

        // Fecha inicial
        let startDate = '';
        // Fecha final (actual)
        let endDate = localLocale.format('L');

        // Coger la última semana del día actual (-De momento se escogen por días)
        //console.log(moment().subtract(1, 'weeks').startOf('isoWeek').format('L'));
        //console.log(moment().subtract(1, 'weeks').endOf('isoWeek').format('L'));

        if ($(item).hasClass(`facet-last-week`)) {
            startDate = localLocale.subtract(7, "days");
        } else if ($(item).hasClass(`facet-last-month`)) {
            startDate = localLocale.subtract(30, "days");
        } else if ($(item).hasClass(`facet-last-semester`)) {
            startDate = localLocale.subtract(180, "days");
        } else {
            // Selección del último año
            startDate = localLocale.subtract(365, "days");
        }

        // Botón para búsqueda,  inputs para establecer fechas
        const searchBtn = $(item).parent().parent().parent().find(this.searchButton);
        const fromDateValue = $(item).parent().parent().parent().find(this.inputFromDate);
        const toDateValue = $(item).parent().parent().parent().find(this.inputToDate);

        // Escribir las fechas en inputs
        fromDateValue.val(startDate.format('L'));
        toDateValue.val(endDate);

        // Hacer click en botón de búsqueda
        searchBtn.trigger("click");
    },
}


/**
 * Clase jquery para poder gestionar la "peticion" de solicitud de cambio de contrase�a de un usuario
 * Este tipo de petici�n es ejecutada cuando el usuario ha solicitado cambio de contrase�a (por olvido), ha recibido un email y ha accedido a esa url para 
 * proceder a cambiar su contrase�a
 * 
 * */
const operativaPeticionCambiarContrasenia = {
    /**
     * Acci�n para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },
    /*
     * Opciones de configuraci�n de la vista con el formulario modal
     * */
    config: function (pParams) {

        // Inicializaci�n de las vistas    
        this.txtLogin = $(`#${pParams.idTxtLogin}`);
        this.txtPasswordNueva = $(`#${pParams.idTxtPasswordNueva}`);
        this.txtPasswordConfirmar = $(`#${pParams.idTxtPasswordConfirmar}`);
        this.btnCambiarPassword = $(`#${pParams.idBtnCambiarPassword}`);
        this.btnRechazarPeticionCambiarPassword = $(`#${pParams.idBtnRechazarPeticionCambiarPassword}`);
        this.panelErroresInformativo = $(`#${pParams.idPanelErroresInformativo}`);
        this.bloqMayInfoPanel = $(`#${pParams.idBloqMayInfoPanel}`);
        this.panelCambioPassword = $(`#${pParams.idPanelCambioPassword}`);

        // Inputs
        this.inputsFields = [this.txtLogin, this.txtPasswordNueva, this.txtPasswordConfirmar];
        // Paneles de errores/info
        this.panelesError = [this.panelErroresInformativo, this.bloqMayInfoPanel];
        // Mensajes de error preconfigurados
        this.errorMsgNoUsuario = pParams.errorMsgNoUsuario;
        this.errorMsgNoPassword = pParams.errorMsgNoPassword;
        this.errorPasswordNoIguales = pParams.errorPasswordNoIguales;
        this.okRejectPasswordMessage = pParams.okRejectPasswordMessage;
        this.okPasswordCambiado = pParams.okPasswordCambiado;

        // Url necesarias
        this.urlPasswordRequest = pParams.urlPasswordRequest;
        this.urlRejectPasswordRequest = pParams.urlRejectPasswordRequest;

        // Comprobaci�n que los inputs han sido rellenados
        this.areInputsFilled = false;
        // Comprobaci�n que las contrase�as coinciden
        this.arePasswordsTheSame = false;


        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";
    },

    /**
    * Comprobar que los campos (password old, password new y password confirmado ) no est�n vac�os
    * y que las contrase�as introducidas coinciden
    * @returns {bool}    
    */
    validarCampos: function () {

        if (this.txtLogin.val() != '' && this.txtPasswordNueva.val() != '' && this.txtPasswordConfirmar.val() != '') {
            this.areInputsFilled = true;
            // Comprobar que las contrase�as introducidas son iguales
            if (this.txtPasswordNueva.val() === this.txtPasswordConfirmar.val()) {
                this.arePasswordsTheSame = true;
            } else {
                this.arePasswordsTheSame = false;
            }
        }
    },

    /**
    * Configuraci�n de los eventos 
    */
    configEvents: function () {
        // Referencia al objeto
        const that = this;

        // Input Password - Control de may�sculas
        this.txtPasswordNueva.keypress(function (event) {
            // Avisar con un mensaje si est�n activadas las may�sculas del teclado
            isCapsLock(event) ? that.bloqMayInfoPanel.fadeIn() : that.bloqMayInfoPanel.fadeOut();
        });

        // Input Password Confirmar - Control de may�sculas + Enter
        this.txtPasswordConfirmar.keypress(function (event) {
            // Avisar con un mensaje si est�n activadas las may�sculas del teclado
            isCapsLock(event) ? that.bloqMayInfoPanel.fadeIn() : that.bloqMayInfoPanel.fadeOut();
            event.keyCode === 13 ? that.btnCambiarPassword.click() : null
        });

        // Bot�n de Aceptar - Solicitar cambio de contrase�a         
        this.btnCambiarPassword.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideErrorPanels();
            // Hacer login solo si los datos han sido introducidos
            that.validarCampos();
            if (that.arePasswordsTheSame == true && that.areInputsFilled == true) {
                // Realizar la petici�n de cambio de contrase�a
                //that.cambiarPassword();
                that.cambiarPassword();
            } else {
                // Mostrar el panel de error correspondiente
                if (that.arePasswordsTheSame == true) {
                    that.panelErroresInformativo.html(that.errorPasswordNoIguales);
                } else if (that.txtLogin.val() == "") {
                    that.panelErroresInformativo.html(that.errorMsgNoUsuario);
                } else if (that.txtPasswordNueva.val() == "" || that.txtPasswordConfirmar.val() == "") {
                    that.panelErroresInformativo.html(that.errorMsgNoPassword);
                } else {
                    that.panelErroresInformativo.html(that.errorPasswordNoIguales);
                }
                that.panelErroresInformativo.fadeIn();
            }
        });


        // Link/ Bot�n de cancelar solicitud de cambio de contrase�a
        this.btnRechazarPeticionCambiarPassword.click(function () {
            that.rechazarPeticion();
        });
    },

    /**
     * Ocultar los paneles informativos de errores
     * */
    hideErrorPanels() {
        this.panelesError.forEach(panelError => {
            panelError.hide();
            panelError.val('');
        });
    },

    /**
      * Vaciar de los inputs utilizados (Ej: Una vez efectuada la solicitud, vaciar los inputs)
      */
    emptyInputs() {
        this.inputsFields.forEach(input => input.val(''));
    },

    /**
     * B�scar los par�metros mandados por URL.
     * Este m�todo es llamado desde la funci�n que realiza la petici�n de cambio de password
     * @param {any} param: 
     * @param {any} url: Url a la que se realiza la petici�n para cambio de contrase�a
     */
    getParam: function (param, url) {
        /* Buscar a partir del signo de interrogaci�n ? */
        url = String(url.match(/\?+.+/));
        /* limpiar la cadena quit�ndole el signo ? */
        url = url.replace("?", "");
        /* Crear un array con parametro=valor */
        url = url.split("&");
        /*
        Recorrer el array url
        obtener el valor y dividirlo en dos partes a trav�s del signo =
        0 = parametro
        1 = valor
        Si el par�metro existe devolver su valor
        */
        x = 0;
        while (x < url.length) {
            p = url[x].split("=");
            if (p[0] == param) {
                return decodeURIComponent(p[1]);
            }
            x++;
        }
    },

    /**
     * Funci�n para realizar la petici�n del cambio de contrase�a solicitado por el usuario
     * */
    cambiarPassword: function () {
        // Referencia al objeto
        const that = this;
        this.btnCambiarPassword.hide();
        MostrarUpdateProgress();
        // Construcci�n del objeto con los passwords
        const dataPost = {
            User: that.txtLogin.val(),
            Password: that.txtPasswordNueva.val(),
            PasswordConfirmed: that.txtPasswordConfirmar.val(),
        }

        // Realizar la petici�n de cambio de password
        GnossPeticionAjax(that.urlPasswordRequest, dataPost, true)
            .done(function () {
                // Ocultar posibles paneles de error
                that.hideErrorPanels();

                // Mostrar panel info con su clase
                that.panelErroresInformativo.addClass(that.okClass);
                that.panelErroresInformativo.removeClass(that.errorClass);
                that.panelErroresInformativo.html(that.okPasswordCambiado);
                var transfer = that.getParam("transferto", location.href);
                if (transfer != undefined) {
                    location.href = transfer;
                }
                // Destruimos o eliminamos el panel de inputs para que no pueda volver a solicitar cambio de contrase�a
                that.panelCambioPassword.remove()
            })
            .fail(function () {
                // Mostrar panel info con su clase
                that.panelErroresInformativo.removeClass(that.okClass);
                that.panelErroresInformativo.addClass(that.errorClass);
                // Mostrar el mensaje de error
                that.panelErroresInformativo.html(html);
            })
            .always(function (html) {
                // Mostrar el mensaje de error o de success
                that.panelErroresInformativo.fadeIn();
                // Vaciar los inputs
                that.emptyInputs();
                // Ocultar el loading
                OcultarUpdateProgress();
            });
    },

    /**
     * Funci�n para cancelar o rechazar la solicitud de cambio de contrase�a.
     * */
    rechazarPeticion: function () {
        const that = this;
        MostrarUpdateProgress();
        // Ocultar posibles paneles de error
        this.hideErrorPanels();

        GnossPeticionAjax(this.urlRejectPasswordRequest, null, true).done(function () {
            // Destruimos o eliminamos el panel de inputs para que no pueda volver a solicitar cambio de contrase�a
            that.panelCambioPassword.remove()
            that.btnCambiarPassword.remove()
            // Mostrar mensaje de cancelar la solicitud de cambio de password
            that.panelErroresInformativo.html(that.okRejectPasswordMessage);
            that.panelErroresInformativo.addClass(that.okClass);
            that.panelErroresInformativo.removeClass(that.errorClass);
            that.panelErroresInformativo.fadeIn();
            OcultarUpdateProgress();
        });
    },
};

/**
 * Clase jquery para poder realizar env�os de invitaciones a una comunidad y de links de recursos (desde la ficha de recurso) a correos o contactos de una comunidad.
 * Para acceder a esta vista se acceder� 
 *  - Desde la propia ficha de recurso (Enviar Link)
 *  - Panel lateral del usuario si dispone de permisos en la comunidad para enviar invitaciones 
 * */
const operativaEnviarResource_Link_Community_Invitation= {
    /**
     * Acci�n para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
        this.configAutocompleteService(pParams.autocompleteParams);
    },
    /*
     * Opciones de configuraci�n de la vista
     * */
    config: function (pParams) {
        // Inicializaci�n de las vistas  
        this.txtFiltro = $(`#${pParams.idTxtFiltro}`);
        this.txtCorreoAInvitar = $(`#${pParams.idTxtCorreoAInvitar}`);
        this.buttonLitAniadirCorreo = $(`#${pParams.idButtonLitAniadirCorreo}`);
        this.buttonLitAniadirCorreo = $(`#${pParams.idButtonLitAniadirCorreo}`);
        this.txtHackInvitados = $(`#${pParams.idTxtHackInvitados}`);
        // El autocomplete necesita solo el nombre del input oculto
        this.txtHackInvitadosInputName = pParams.idTxtHackInvitados;
        this.panContenedorInvitados = $(`#${pParams.idPanContenedorInvitados}`);
        this.listaDestinatarios = $(`#${pParams.idListaDestinatarios}`);
        this.noDestinatarios = $(`#${pParams.idNoDestinatarios}`);
        this.btnEnviarInvitaciones = $(`#${pParams.idBtnEnviarInvitaciones}`);        
        this.lblInfoCorreo = $(`#${pParams.idLblInfoCorreo}`);        
        this.panelInfoInvitationSent = $(`#${pParams.idPanelInfoInvitationSent}`);

        // Campos especiales para env�o de link (idioma & notas/mensaje)                
        this.txtNotas = $(`#${pParams.idTxtNotas}`);
        this.dllIdioma = $(`#${pParams.idDlIdioma}`);
       
        // Paneles de error/info
        this.panelesInfo = [this.panelInfoInvitationSent];

        // Url necesarias para realizar petici�n        
        this.urlSend = pParams.urlSend;
    },

    /**
    * Ocultar los paneles informativos (errores o info)
    **/
    hideInfoPanels() {
        this.panelesInfo.forEach(panel => panel.hide());
    },

    /**
    * Configuraci�n de los eventos
    */
    configEvents: function () {
        const that = this;

        // Bot�n de Aceptar - Solicitar link para cambio de contrase�a
        this.btnEnviarInvitaciones.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideInfoPanels();
            // Comprobar que el input de invitados (el oculto que almacena los ids al menos tiene emails o contactos)
            if (that.validarCampos()) {
                // Realizar la petici�n de cambio de contrase�a
                that.enviarInvitacion_EnlaceSubmit();                           
            }
        });

        // Bot�n de A�adir correo 
        this.buttonLitAniadirCorreo.click(function () {
            if (that.txtCorreoAInvitar.val().length > 3) {
                that.crearInvitado(null, that.txtCorreoAInvitar.val(), that.txtCorreoAInvitar.val(), true);
            }
        });

        // Configurar el borrado de elementos al pulsar en (x) de un item de los destinatarios
        this.listaDestinatarios.on('click', 'span', function (event) {
            const identidad = $(event.target).parent().parent().attr("id");
            that.eliminarUsuario(null, identidad);            
        });    
    },

    /**
    * Comprobar que el campo oculto que contiene posibles destinatarios tiene cierta longitud
    * @returns {bool}    
    */
    validarCampos: function () {
        return (this.txtHackInvitados.val().length > 3);
    },

    /**
     * Acci�n que se ejecuta cuando se realice una b�squeda escribiendo el nombre de un usuario y al pulsar en uno de los resultados devueltos por autocomplete.
     * Con los datos devueltos, construir� el item y lo  meter� en "panContenedorInvitados". *@
     * @param {any} ficha
     * @param {any} nombre: El nombre del usuario seleccionado
     * @param {any} identidad: La identidad del item seleccionado
     * @param {boolean} isAnEmail: Valor que indicar� si lo que se est� intentando a�adir es un contacto (normal) o un email de un usuario
     */
    crearInvitado: function (ficha, nombre, identidad, isAnEmail) {     
        // Item que se a�adir� como elemento seleccionado
        let itemHtml = "";

        if (!isAnEmail) {
            itemHtml += `<div class="tag" id="${identidad}" data-item="${identidad}">`;
            itemHtml += `<div class="tag-wrap">`;
            itemHtml += `<span class="tag-text">${nombre}</span>`;
            itemHtml += `<span class="tag-remove material-icons">close</span>`;
            itemHtml += `</div>`;
            itemHtml += `</div>`;
            // A�adir la identidad al input de invitados
            this.txtHackInvitados.val(`${this.txtHackInvitados.val()}&${identidad}`);        
        } else {            
            // Construyo correos separados por comas
            const correos = this.txtCorreoAInvitar.val().split(',');                        
            // Validar si son correos v�lidos     
            for (let i = 0; i < correos.length; i++) {
                if (correos[i] != '') {
                    if (!validarEmail(correos[i].replace(/^\s*|\s*$/g, ""))) {
                        // No es email v�lido, muestra mensaje de error
                        this.lblInfoCorreo.html(form.emailValido);
                        this.lblInfoCorreo.parent().parent().fadeIn();
                        return;
                    } else {
                        this.lblInfoCorreo.parent().parent().fadeOut();
                    }
                }
            }
            // Recorrer array de correos para ser a�adidos a la vista
            for (let i = 0; i < correos.length; i++) {
                if (correos[i] != '') {                    
                    let data_item = correos[i].replace(/\@/g, '_');
                    data_item = data_item.replace(".",'_');
                    itemHtml += `<div class="tag" id="${correos[i]}" data-item="${data_item}">`;
                    itemHtml += `<div class="tag-wrap">`;
                    itemHtml += `<span class="tag-text">${correos[i]}</span>`;
                    itemHtml += `<span class="tag-remove material-icons">close</span>`;
                    itemHtml += `</div>`;
                    itemHtml += `</div>`;                                        
                    // A�adir el correo al input de invitados
                    this.txtHackInvitados.val(`${this.txtHackInvitados.val()}&${correos[i].replace(/^\s*|\s*$/g, "")}`);  
                }
            }
        }
        
        // A�adir el item en el contenedor de destinatarios
        this.listaDestinatarios.append(itemHtml);

        // Ocultar el panel de "No destinatarios" ya que hay a�adidos
        this.noDestinatarios.fadeOut();

        // Vaciamos el input donde se ha introducido al usuario
        isAnEmail ? this.txtCorreoAInvitar.val('') : this.txtFiltro.val('');
        // Quitamos posible mensaje de error de correo a�adido
        this.lblInfoCorreo.val('');
        this.lblInfoCorreo.hide();

        if (ficha != null) {
            ficha.style.display = 'none';
        }        
    },

    /**
     * Acci�n que eliminar� a un elemento al pulsar sobre su (x). Desaparecer� del contenedor y del input oculto que contiene
     * los items seleccionados para el env�o de la solicitud
     * @param {any} fichaId             
     */
    eliminarUsuario: function (fichaId, identidad) {
               
        // Eliminar la identidad al input construyendo el nuevo valor que tomar�
        let newTxtHackInvitados = this.txtHackInvitados.val().replace('&' + identidad, '');
        this.txtHackInvitados.val(newTxtHackInvitados);                   

        // Tratar de eliminar caracteres especiales para buscar el atributo de data-item (para casos de correos electr�nicos)
        let data_item = identidad.replace(/\@/g, '_');
        data_item = data_item.replace(".", '_');
        const itemToDelete = $(`*[data-item="${data_item}"]`);
        itemToDelete.remove();

        // Comprobar si hay items para mostrar u ocultar mensaje de "Ning�n destinatario..."
        const numItems = this.listaDestinatarios.children().length;
        numItems >= 1 ? this.noDestinatarios.hide() : this.noDestinatarios.show();
    },

    /**
     * Configuraci�n del servicio autocomplete para el input buscador de nombres
     * Se pasaran los par�metros necesarios los cuales se han obtenido de la vista
     * @param {any} autoCompleteParams
     */
    configAutocompleteService(autoCompleteParams) {
        const that = this;

        // Objeto que albergar� los extraParams para el servicio autocomplete
        let extraParams = {};

        // Configuraci�n de extraParams dependiendo isEcosistemasinMetaProyecto
        if (autoCompleteParams.isEcosistemasinMetaProyecto) {
            extraParams = {
                identidad: autoCompleteParams.identidad,
                identidadMyGnoss: autoCompleteParams.identidadMyGnoss, 
                identidadOrg: autoCompleteParams.identidadOrg, 
                proyecto: autoCompleteParams.proyecto, 
                bool_esPrivada: autoCompleteParams.esPrivada
            }
        } else {
            extraParams = {
                identidad: autoCompleteParams.identidad,
                proyecto: autoCompleteParams.proyecto,
            }
        }
        // Configuraci�n del autocomplete para el input de b�squeda de nombres
        this.txtFiltro.autocomplete(
            null,
            {
                //servicio: new WS($(`#${autoCompleteParams.idInpt_urlServicioAutocompletar}`).val(), WSDataType.jsonp),
                //metodo: autoCompleteParams.metodo,
                url: $(`#${autoCompleteParams.idInpt_urlServicioAutocompletar}`).val() + '/AutoCompletarEnvioMensajes',
                type: "POST",
                delay: 0,
                multiple: true,
                scroll: false,
                selectFirst: false,
                minChars: 1,
                width: 300,
                cacheLength: 0,
                NoPintarSeleccionado: true,
                txtValoresSeleccID: that.txtHackInvitadosInputName,
                extraParams,
            }
        );

        // Configuraci�n la acci�n select (cuando se seleccione un item de autocomplete)
        this.txtFiltro.result(function (event, data, formatted) {
            that.crearInvitado(null, data[0], data[1], false);
        });
    },

    /**
    * Acci�n de env�o de la invitaci�n de la comunidad o del enlace
    * Se disparar� al pulsar el bot�n de "Enviar"
    */
    enviarInvitacion_EnlaceSubmit: function () {
        const that = this;
        
        MostrarUpdateProgress();

        // Construcci�n del objeto dataPost
        let dataPost = {};
        // Construir la URL teniendo en cuenta el tipo de env�o
        let newUrlRequest = "";
        //Tener en cuenta de si no existe el idioma -> Invitaci�n de comunidad
        if (this.dllIdioma.length == 0) {
            dataPost = {
                Guests: that.txtHackInvitados.val(),
                Message: "",
            };
            newUrlRequest = that.urlSend + document.location.search;
        } else {
            dataPost = {
                Receivers: that.txtHackInvitados.val(),
                Message: encodeURIComponent(that.txtNotas.val().replace(/\n/g, '')),
                Lang: that.dllIdioma.val()
            }
            newUrlRequest = that.urlSend;
        }

        GnossPeticionAjax(newUrlRequest,
            dataPost,
            true
        ).done(function (response) {
            claseDiv = "ok";            
        }).fail(function (response) {
            claseDiv = "ko";
        }).always(function (response) {
            that.panelInfoInvitationSent.addClass(claseDiv)
            that.panelInfoInvitationSent.html(response);
            that.panelInfoInvitationSent.fadeIn();
            OcultarUpdateProgress();
        });
    }
};

/**
 * Clase jquery para poder gestionar la suscripci�n de un usuario a las categor�as de una comunidad
 * Se puede acceder desde el panel lateral del usuario, pulsando en "Suscribirse".
 * Podr� elegir (mediante checkbox disponibles) las categor�as a las que suscribirse y si 
 * desea recibir newsletters de forma diaria o semanal
 * */

const operativaGestionarSuscripcionComunidad = {
    /**
     * Acci�n para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },

    /*
     * Opciones de configuraci�n de la vista
     * */
    config: function (pParams) {
        // Inicializaci�n de las vistas             
        this.chkRecibirBoletin = $(`#${pParams.idChkRecibirBoletin}`);
        this.panelFrecuenciaRecibirBoletin = $(`#${pParams.idPanelFrecuenciaRecibirBoletin}`);
        this.radioNameSuscription = $(`#${pParams.nameSuscripcion}`);
        this.txtHackCatTesSel = $(`#${pParams.idTxtHackCatTesSel}`);
        this.panelInfoSuscripcionCategorias = $(`#${pParams.idPanelInfoSuscripcionCategorias}`);
        this.rbtnSuscripcionDiaria = $(`#${pParams.idRbtnSuscripcionDiaria}`);
        this.rbtnSuscripcionSemanal = $(`#${pParams.idRbtnSuscripcionSemanal}`);
        this.btnSaveSubscriptionPreferences = $(`#${pParams.idBtnSaveSubscriptionPreferences}`);

        // Valores por defecto de Recibir boletines y su frecuencia (por defecto, false)
        this.isReceivingNewsletters = false;
        this.isFrequencyDaily = false;
        this.isFrequencyWeekly = false;

        // Url necesaria a la que habr� que hacer la petici�n
        this.urlRequestCommunitySubscription = pParams.urlRequestCommunitySubscription;
        // Paneles de info/error                                     
        this.panelesInfo = [this.panelInfoSuscripcionCategorias];

        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";
    },

    /**
    * Ocultar los paneles informativos (errores o info)
    **/
    hideInfoPanels() {
        this.panelesInfo.forEach(panel => panel.hide());
    },

    /**
    * Configuraci�n de los eventos
    */
    configEvents: function () {
        const that = this;

        // Checkbox (si existe) para mostrar la frecuencia de boletines newsletter
        if (this.chkRecibirBoletin != undefined) {
            this.chkRecibirBoletin.on("click", function () {
                if ($(this).is(':checked')) {
                    that.panelFrecuenciaRecibirBoletin.fadeIn();
                    // Dejar checked por defecto una opci�n del radioButton (si antes no se ha seleccionado nada)
                    if (!that.rbtnSuscripcionDiaria.is(':checked') && !that.rbtnSuscripcionSemanal.is(':checked')) {
                        // Dejar por defecto la opci�n diaria checkeada
                        that.rbtnSuscripcionDiaria.prop("checked", true);
                    }
                } else {
                    that.panelFrecuenciaRecibirBoletin.fadeOut();
                }
            });
        }
        // Bot�n de guardar cambios/enviar al servidor
        this.btnSaveSubscriptionPreferences.on("click", function () {            
            // Comprobar los valores para el env�o
            that.checkRadioButtonsAndCheckValues();
            // Enviar los datos
            that.gestionSuscripcionComunidadSubmit();
        });
    },

    /**
     * M�todo para comprobar los checks y radioButtons para crear los valores booleanos
     * para el env�o de datos al servidor
     * Comprueba primero si existen los inputs y una vez comprobado, analiza los datos
     * */
    checkRadioButtonsAndCheckValues: function () {
        if (this.chkRecibirBoletin != undefined) {
            if (this.chkRecibirBoletin.is(':checked')) {
                this.isReceivingNewsletters = true;
                // Comprobaci�n diaria o semanal
                this.rbtnSuscripcionDiaria.is(':checked') ? this.isFrequencyDaily = true : this.isFrequencyDaily = false;
                this.rbtnSuscripcionSemanal.is(':checked') ? this.isFrequencyWeekly = true : this.isFrequencyWeekly = false;
            }
        } else {
            this.chkRecibirBoletin.is(':checked') ? this.isReceivingNewsletters = true : this.isReceivingNewsletters = false;
        }
    },

    /**
    * Acci�n de env�o de ajustes en suscripci�n de la comunidad
    * Se disparar� al pulsar el bot�n de "Enviar"
     * */
    gestionSuscripcionComunidadSubmit: function () {
        const that = this;

        // Mostrar loading
        MostrarUpdateProgress();
        // Ocultar posibles mensajes de info/error
        this.hideInfoPanels();
        // Construcci�n del objeto para enviar datos
        const dataPost = {
            SelectedCategories: that.txtHackCatTesSel.val(),
            ReceiveSubscription: that.isReceivingNewsletters,
            DailySubscription: that.isFrequencyDaily,
            WeeklySubscription: that.isFrequencyWeekly
        }

        // Envio de los datos
        GnossPeticionAjax(
            that.urlRequestCommunitySubscription,
            dataPost,
            true
        ).done(function () {            
            that.panelInfoSuscripcionCategorias.addClass(that.okClass);
            that.panelInfoSuscripcionCategorias.removeClass(that.errorClass);
        }).fail(function () {
            that.panelInfoSuscripcionCategorias.removeClass(that.okClass);
            that.panelInfoSuscripcionCategorias.addClass(that.errorClass);
        }).always(function (response) {
            // Mostrar el mensaje de error
            that.panelInfoSuscripcionCategorias.html(response);
            that.panelInfoSuscripcionCategorias.fadeIn();
            OcultarUpdateProgress();
        });
    },
};

/**
 * Clase jquery para poder gestionar la solicitud de recibir (o no) de newsletters de la comunidad
 * Se puede acceder desde el panel lateral del usuario, pulsando en "Recibir newsletter".
 *
 * */
const operativaSolicitarRecibirNewsletter = {
    /**
     * Acci�n para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },
    /*
     * Opciones de configuraci�n de la vista
     * */
    config: function (pParams) {
        // Inicializaci�n de IDS de las vistas  
        this.idChkRecibirNewsletter = pParams.chkRecibirNewsletter;
        this.idChkNoRecibirNewsletter = pParams.chkNoRecibirNewsletter;
        this.btnSubmitReceiveNewsletters = $(`#${pParams.btnSubmitReceiveNewsletters}`);

        // Nombre de los inputs
        this.nameChkReceiveNewsletter = pParams.nameChkReceiveNewsletter;

        // Inicializaci�n de las vistas 
        // Panel de posibles mensajes
        this.chkRecibirNewsletterNameInfoPanel = $(`#${pParams.chkRecibirNewsletterNameInfoPanel}`);

        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";

        // Url necesaria a la que habr� que hacer la petici�n
        this.urlRequestReceiveNewsletters = pParams.urlRequestReceiveNewsletters;

        // Inicializaci�n de las vistas        
        this.panelesInfo = [this.chkRecibirNewsletterNameInfoPanel]; 
    },

    /**
    * Ocultar los paneles informativos (errores o info)
    **/
    hideInfoPanels() {
        this.panelesInfo.forEach(panel => panel.hide());
    },

    /**
    * Configuraci�n de los eventos
    */
    configEvents: function () {
        const that = this;

        // Bot�n de Aceptar - Solicitar env�o o no de newsletters
        this.btnSubmitReceiveNewsletters.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideInfoPanels();

            // Conocer el inputRadio activo. Ser� el que comparemos con idChkRecibirNewsletter o idChkNoRecibirNewsletter (Desea o no desea newsletters)
            that.checkRecibirNewsLetterValue = $(`input[name=${that.nameChkReceiveNewsletter}]:checked`).val();
            // Realizar la petici�n de cambio de contrase�a
            const isReceivingNewsletters = that.checkRecibirNewsLetterValue === that.idChkRecibirNewsletter ? true : false;
            that.recibirNewsletterSubmit(isReceivingNewsletters);
        });
    },

    /**
     * Acci�n de petici�n de recibir (o no) de newsletters. Se dispara cuando se pulsa en el bot�n submit del modal     
     * @param {Bool} option: Dependiendo del input pulsado, el usuario querr� o no recibir newsletters.
     */
    recibirNewsletterSubmit: function (option) {
        const that = this;
        MostrarUpdateProgress();
        that.hideInfoPanels();
        // Construcci�n del DataPost para enviar la petici�n
        const dataPost = {            
            recibirNewsletter: option,
        };

        // Realizar la petici�n 
        GnossPeticionAjax(
            this.urlRequestReceiveNewsletters,
            dataPost,
            true
        ).done(function () {
            // Ocultar posibles paneles de info/error
            that.hideInfoPanels();
            // Mostrar panel info con su clase
            that.chkRecibirNewsletterNameInfoPanel.addClass(that.okClass);
            that.chkRecibirNewsletterNameInfoPanel.removeClass(that.errorClass);

            // Ocultar modal pasados 1.5 segundos
            setTimeout(() => {
                $('#modal-receive-newsletters').modal('hide');
            }, 1500);

        }).fail(function () {
            // Ocultar posibles paneles de error
            that.hideInfoPanels();
            // Mostrar panel info con su clase
            that.chkRecibirNewsletterNameInfoPanel.removeClass(that.okClass);
            that.chkRecibirNewsletterNameInfoPanel.addClass(that.errorClass);
        }).always(function (response) {
            // Mostramos el mensaje informativo
            that.chkRecibirNewsletterNameInfoPanel.html(response);
            that.chkRecibirNewsletterNameInfoPanel.fadeIn();
            OcultarUpdateProgress();
        });
    },
};

/**
 * Clase jquery para poder la solicitud de restablecimiento de la contrase�a por parte del usuario.
 * Para acceder a esta vista, se acceder� desde un link en "Login" de "Olvido de contrase�a"
 *
 * */
const operativaOlvidoPassword = {
    /**
     * Acci�n para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },
    /*
     * Opciones de configuraci�n de la vista
     * */
    config: function (pParams) {
        // Inicializaci�n de las vistas  
        this.txtUserLogin = $(`#${pParams.idTxtuserLogin}`);
        this.forgetPasswordInfoPanel = $(`#${pParams.idForgetPasswordInfoPanel}`);
        this.btnEnviar = $(`#${pParams.idBtnEnviar}`);

        // Paneles de error/info
        this.panelesInfo = [this.forgetPasswordInfoPanel];

        // Url necesarias para realizar petici�n
        this.urlForgetPasswordRequest = pParams.urlForgetPasswordRequest;

        // Mensajes preconfigurados de error
        this.msgInfoEmptyField = pParams.msgInfoEmptyField;
        this.msgErrorForgetPasswordRequest = pParams.msgErrorForgetPasswordRequest;                                      
    },

    /**
    * Ocultar los paneles informativos (errores o info)
    **/
    hideInfoPanels() {
        this.panelesInfo.forEach(panel => panel.hide());
    },

    /**
    * Configuraci�n de los eventos
    */
    configEvents: function () {
        const that = this;

        // Bot�n de Aceptar - Solicitar link para cambio de contrase�a
        this.btnEnviar.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideInfoPanels();
            // Comprobar que el input no es vac�o
            if (that.validarCampos()) {                
                // Realizar la petici�n de cambio de contrase�a
                that.cambiarPasswordSubmit();
            } else {                
                that.forgetPasswordInfoPanel.html(that.msgInfoEmptyField);
                that.forgetPasswordInfoPanel.fadeIn();
            }         
        });
    },

    /**
    * Comprobar que los campos indicados no est�n vac�os (email indicado por el usuario)
    * @returns {bool}    
    */
    validarCampos: function () {
        return (this.txtUserLogin.val() != '');
    },

    /**
    * Acci�n de petici�n de solicitar cambio de contrase�a por olvido
    * Se disparar� al pulsar el bot�n y al validar que el campo del email no est� vac�o
    */
    cambiarPasswordSubmit: function () {
        const that = this;
        // Construcci�n del objeto dataPost
        var dataPost = {
            User: this.txtUserLogin.val()
        }

        MostrarUpdateProgress();
        GnossPeticionAjax(this.urlForgetPasswordRequest, dataPost, true).fail(function () {
            // Mostrar mensaje de error con la informaci�n traida del backend
            that.forgetPasswordInfoPanel.html(that.msgErrorForgetPasswordRequest);
            that.forgetPasswordInfoPanel.fadeIn("slow");            
            OcultarUpdateProgress();
        });
    }
};


var engancharComportamientoIdiomas = {
	idiomas: [],
	init: function(){
		this.config();
		this.comprobarNumeroIdiomas();
		if(this.idiomas.length < 3) return;
		this.identidad.addClass('idiomasCustomizado');
		this.montarListado();
		this.montarDesplegable();
		this.enganchar();
		this.mostrarIdiomaActivo();
		this.controlarItemBeta();
		return;
	},
	config: function(){
		this.body = body;
		this.perfilUsuarioGnoss = this.body.find('#perfilUsuarioGnoss');
		this.identidad = this.perfilUsuarioGnoss.find('#identidadGNOSS');
		// Deprecado size()
        //if (this.identidad.size() <= 0) this.identidad = this.body.find('#identidad');
        if (this.identidad.length <= 0) this.identidad = this.body.find('#identidad');
		this.gnoss = this.identidad.find('#gnoss');
		return;
	},
	comprobarNumeroIdiomas: function(){
		var that = this;
		var items = this.gnoss.children();
		items.each(function(){
			var item = $(this);
			if(!item.hasClass('logo')) that.idiomas.push(this);
		})
		return;
	},
	mostrarIdiomaActivo: function(){
		var ul = this.capa.children();
		var items = ul.first().find('li');
		var primero = items.first();
		var segundo = primero.next();
		if(primero.children().hasClass('activo')) return;
		var opciones = this.capa.find('#idiomasSelector');
		var activo = opciones.find('.activo');
		var clonado = activo.clone();
		var lang = clonado.attr('lang');
		clonado.text(lang);
		var li = $('<li />');
		li.append(clonado);
		primero.before(li);
		segundo.remove();
		return;
	},	
	montarListado: function(){
		this.capa = $('<div />').attr('id', 'idiomas');
		var ul = $('<ul />');
		var li = $('<li />');
		this.desplegar = $('<a />').addClass('desplegar').text('desplegar');
		$.each(this.idiomas, function(indice){
			var item = $(this);
			var clonado = item.clone();
			if(item.children().hasClass('activo')){
				clonado.children().addClass('activo');
			}			
			if(indice < 2){
				ul.append(clonado);
			}
		});
		li.append(this.desplegar);
		ul.append(li);
		this.capa.append(ul);
		this.gnoss.before(this.capa);
		return;
	},	
	montarDesplegable: function(){		
		var div = $('<div />').attr('id', 'idiomasSelector');
		var ul = $('<ul />');
		$.each(this.idiomas, function(indice){
			var item = $(this);
			var enlace = item.children();
			var idioma = enlace.attr('title');
			var abreviatura = enlace.attr('lang');
			var texto = idioma + ' (' + abreviatura + ')';
			enlace.text(texto);
			ul.append(item);
		});		
		div.append(ul);
		this.capa.append(div);
		div.hover(
		function(){
			return;
		},
		function(){
			$(this).hide();
		})
		return;
	},	
	controlarItemBeta: function(){
		var beta = this.capa.find('.beta');
		this.capa.after(beta);
		return;
	},
	enganchar: function(){
		this.desplegar.bind('click', function(evento){
			evento.preventDefault();
			var enlace = $(evento.target);
			var padre = enlace.parents('div');
			var opciones = padre.find('#idiomasSelector');
			opciones.is(':visible') ? opciones.hide() : opciones.show();
		})
		return;
	}
}
var html, body, page, content;

$(function () {
    html = $('html');
    body = html.find('body');
    page = body.find('#page');
    content = page.find('#content');

    var navegador = navigator.userAgent;

    if (navegador.indexOf('MSIE 7.0') > 0) {
        body.addClass('msie7');
    } else if (navegador.indexOf('MSIE 8.0') > 0) {
        body.addClass('msie8');
    }

    $('.hidePanel').each(function () {
        var link = $(this);
        link.bind('click', function (event) {
            event.preventDefault();
            var panel = link.attr('href');
			var operativa
            $(panel).hide();
        });
    });
    $('.hideShowPanel').each(function () {
        var link = $(this);
        link.bind('click', function (event) {
            event.preventDefault();
            var panel = link.attr('href');
            var panels = link.attr('rel');
            var li = link.parent();
            var ul = li.parents('ul');
            var lis = $('li', ul);
            if (!link.hasClass('noGroup')) desmarcarOpcionesGrupo(lis);
            var hasOtherPanels = false;
            if (panels != null && panels != '') hasOtherPanels = true;
            if (hasOtherPanels) ocultarPaneles(panels);
            panel = $(panel);
            if (panel.is(':visible')) {
                li.removeClass('active');
                panel.slideUp();
            } else {
                li.addClass('active');
                /*if( $.browser.msie) panel.css('display','block');*/
                panel.slideDown();
            }
        });
    });

    /* longitud facetas... */
    // Longitud facetas por CSS
    // limiteLongitudFacetas.init();

    /* numero categorias */
    mostrarNumeroCategorias.init();

    /* numero etiquetas */
    mostrarNumeroEtiquetas.init();

    /* enganchar mas menos categorias y etiquetas */
    verTodasCategoriasEtiquetas.init();

    /* presentacion facetas */
    //facetedSearch.init();


    /* presentacion icono certificado */
    $('#section p.certificado').each(function () {
        $(this).prepend('<span class=\"icono\"><\/span>');
    })

    /* presentacion votos */

    presentacionVotosRecurso.init();

    /* 
    presentacion votos 
    @deprecated 19.09.2013
    $('#section p.votosPositivos a').each(function(){
    var enlace = $(this);
    var div = enlace.parents('div').first();
    var panel = div.find('.panelVotos');
    panel.hide();
    panel.addClass('activado');
    enlace.bind('click', function(evento){
    evento.preventDefault();
    panel.is(':visible') ? panel.hide() : panel.show(); 
			
    })
    });
    */

    /* carrusel home */
    carrusel.init();

    /* carrusel comite crea */
    carruselLateralColumna.init();

    /* opciones menu identidad */
    opcionesMenuIdentidad.init();

    /* marcar seccion del menu principal activa */
    //seccion.init();
    /* enganchar modo visualizacion listados */
    //modoVisualizacionListados.init();

    /* limpiar actividad reciente home */
    limpiarActividadRecienteHome.init();

    if (body.hasClass('homeCatalogo')) {
        modoVisualizacionListadosHomeCatalogo.init();
    }
    /* add icono video */
    pintarRecursoVideo.init();

    /* ficha. acerca de este recurso compactado */
    herramientasRecursoCompactado.init();
    redesSocialesRecursoCompactado.init();

    iconografia.init();

    /* texto logo */
    ajustarTextoLogoComunidad.init();

    /* only members */
    onlymembers.init();
    onlymembersContent.init();

    /* subcategorias menu principal 2012.10.08 */
    subcategoriasMenuPrincipal.init();

    /* subcategorias menu principal 2012.11.12 */
    abreEnVentanaNueva.init();

    /* desplegables modo visualizacion */
    desplegableGenerico.init();

    /* comportamiento idiomas v.2014.05.09.01 */
	engancharComportamientoIdiomas.init();

    /* paneles colapsables 2012.11.26 
    panelesColapsablesTresNiveles.init(); 
    */

    /* ajuste paginadores de recursos */
    $('#col01 .paginadorSiguienteAnterior').each(function () {
        var paginador = $(this);
        var grupo = paginador.parents('.group.resources');
        var contextoOtraComunidad = grupo.find('.context-header');
        if (contextoOtraComunidad.size() <= 0) grupo.addClass('grupoPaginado');
    });

    /* ajuste fecha publicador 
    */
    ajusteFechaPublicador.init();

    controladorLineas.init();
    limpiarGruposVaciosSemanticView.init();

    /* selector paso 01 registro */
    if (body.hasClass('registroPaso01')) seleccionarPreferencias.init();
    // Cambiado por nuevo Front
    // Eliminarlo porque no tiene sentido
    //if (body.hasClass('operativaRegistro')) marcarPasosFormulario.init();

    /* registro */
    // Deprecado size()
    //if ($('.formularioRegistroUsuarios').size() > 0) {
    if ($('.formularioRegistroUsuarios').length > 0) {
        marcarObligatorios.init();
    }
    /* customizador file */
    customizeFile.init()

    /* swf component fullscreen  
    var swfFullScreen = $('#section .swffullscreen a');
    if(swfFullScreen.size() > 0){
    swfFullScreen.fancybox({
    'padding'           : 0,
    'autoScale'     	: true,
    'transitionIn'		: 'none',
    'transitionOut'		: 'none',
    'width'				: '98%',
    'height'			: '90%'
    });
    }
    */

    /* Ajustar pestañas */
    var pestanyas= $('#header #nav .principal li');
    // Deprecado size()
    //if (pestanyas.find('activo').size() == 0)
    if (pestanyas.find('activo').length == 0)
    {
        var rutaActual=document.location.href;
        pestanyas.each(function () {
            var pestanya = $(this);
            var enlace= pestanya.find('a');
            if(enlace!=undefined && enlace.attr('href')!=undefined && enlace.attr('href')==rutaActual)
            {
                pestanya.addClass('activo');
            }
        });
    }    
});/**/ 
/*jquery.gnoss.apply.js*/ 
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

var isPendingSuccess = false;

var filter = {
	cssFilterSpace: '.filterSpace',
	cssSearchBy: '.searchBy',
	cssTags: '.tags',
	cssCounter: '.counter',
	isAppliedTags: false,
	init: function(){
		this.config();
	},
	config: function(){
		this.filterSpace = $(this.cssFilterSpace);
		this.searchBy = $(this.cssSearchBy, this.filterSpace);
		this.tags = $(this.cssTags, this.filterSpace);
		this.counter = $(this.cssCounter, this.filterSpace);
		this.number = $('strong', this.counter);
		if(!this.isAppliedTags){
			this.searchBy.hide();
			this.tags.hide();
		}
		return;
	},
	templateTag: function(tag){
		var literal = $(tag).text();
		var rel = $(tag).attr('rel');
		var html = '<li>';
		html += literal;
		html += ' <a href="#" rel="' + rel + '" class="remove">eliminar</a>';
		html += '</li>';
		return html;
	},
	addTag: function(tag){
		if(!this.isAppliedTags){
			this.tags.html('');		
			this.searchBy.show();
			this.tags.show();
		};		
		this.tags.append(this.templateTag(tag));
		this.isAppliedTags = true;
		return;
	},
	changeCounter: function(number){
		this.number.html(number);
		return;
	}
}
var results = {
	cssList: '.resource-list',
	init: function(){
		this.config();
	},
	config: function(){
		this.list = $(this.cssList);
		return;
	},
	loading: function(){
		this.list.html(this.templateLoading);
		return;
	},
	addContent: function(html){
		var that = this;
		setTimeout(function(){
			that.list.html(html);
			isPendingSuccess = false;
			// Cambiar por el nuevo Front. Al hacer click en una faceta de tipo tree, cargar los resultados en la zona de contenido
            //modoVisualizacionListados.init('#col02 .resource-list');            
            modoVisualizacionListados.init('#panResultados .resource-list');

		}, 500);
		return;
	},	
    /*
     * Función que se ejecuta cuando se pulsa en una opción de link de tipo Tree o ListTree en Facetas. 
     * Muestra un pensaje de Cargando hasta que los datos son devueltos por el servidor     
     */
	templateLoading: function(){
		//var html = '<p class="loading">procesing linked data...</p>';
        var html = "";
        html += '<div class="align-items-center mb-2">';
        html += '<div ';
        html += 'class="spinner-border texto-primario mr-2"';
        html += 'role="status"';
        html += 'aria-hidden="true"';           
        html += '></div>';
        html += '<strong>Cargando resultados ...</strong>';       
        html += '</div>';
		return html;
	}
}
function templateRecurso(resource){
	html_response = "<div class=\"resource\">";
	html_response += "<div class=\"box description\">";
	html_response += "<div class=\"group title\">";
	html_response += "<h4><a href=\"" + resource.url + "\">" + resource.titulo + "</a></h4>";
	html_response += "<p class=\"resourceType digital\"><span>tipo de documento<\/span><a href=\"#resource\">Archivo digital<\/a><\/p>";
	html_response += "</div>";
	html_response += "<div class=\"group content\">";
	html_response += resource.contenidoBreve;
	html_response += "<\/div>";
	html_response += "<div class=\"group utils-2\">";
	html_response += "<p>Autores: <a href=\"personas.php\">Equipo GNOSS, Dr. Martin Hepp<\/a><\/p>";
	html_response += "<p>Publicado el 21.04.10 por <a href=\"personas.php\">Equipo GNOSS<\/a><\/p>";
	html_response += "<p>Editores: <a href=\"personas.php\">Lina Aguirre, Equipo GNOSS<\/a>, <a href=\"personas.php\">Ricardo Alonso Maturana<\/a><\/p>";
	html_response += "<\/div>";
	html_response += "<div class=\"group categorias\">";
	html_response += "<p>Categorías:<\/p>";
	html_response += "<ul>";
	html_response += "<li><a href=\"recursos.php\">Representación del conocimiento/ Knowledge representation,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">Vocabularios Semánticos/Data Vocabularies,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">Ontologías/ Ontologies,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">Marketing semántico/Semantic marketing<\/a><\/li>";
	html_response += "<\/ul>";
	html_response += "<\/div>";
	html_response += "<div class=\"group etiquetas\">";
	html_response += "<p>Etiquetas: <\/p>";
	html_response += "<ul>";
	html_response += "<li><a href=\"recursos.php\">data,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">e-commerce,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">linked data,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">ontología,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">owl,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">posicionamiento,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">rdf,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">rdfa,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">rdf-s,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">search,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">searchmonkey,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">sem,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">semantic web,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">seo,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">servicios,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">web,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">yahoo<\/a><\/li>";
	html_response += "<\/ul>";
	html_response += "</div>";	
	html_response += "<\/div>";
	html_response += "<\/div>";
	return html_response;
}
function suggest(){
  $.ajax({
    data: "parametro1=valor1&amp;parametro2=valor2",
	cache: false,
    type: "GET",
    dataType: "json",
    url: "includes/recursos/data.php",
    success: function(resources){ 
		var html_response = "";
		$.each(resources, function(indice, resource){
			if(indice == 'resumen'){
				filter.changeCounter(resource.numero);
			}else{
				html_response += templateRecurso(resource);
			}
		});
		results.addContent(html_response);
	},
	error: function(e, xhr){
		//console.log(e);
	}	
   });
}
$(function(){
	filter.init();
	results.init();
	$('.layout02 #facetedSearch .box ul a').each(function(){
		$(this).bind('click', function(event){
			var enlace = $(this);
			var li = enlace.parent().html();
			if(!enlace.hasClass('applied')){
				if(isPendingSuccess == false){
					isPendingSuccess = true;
					filter.addTag(li);
					results.loading();
					enlace.addClass('applied');
					suggest();
				};
			};
			event.preventDefault();
		})
	});
})/**/ 
/*jquery.autocomplete.responsive.js*/ 
/*
 * jQuery Autocomplete plugin 1.1
 *
 * Copyright (c) 2009 Jörn Zaefferer
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 *
 * Revision: $Id: jquery.autocomplete.js 15 2009-08-22 10:30:27Z joern.zaefferer $
 */

; (function($) {
	
$.fn.extend({
	autocomplete: function(urlOrData, options) {
		var isUrl = typeof urlOrData == "string";
		options = $.extend({}, $.Autocompleter.defaults, {
			url: isUrl ? urlOrData : null,
			data: isUrl ? null : urlOrData,
			delay: isUrl ? $.Autocompleter.defaults.delay : 10,
			max: options && !options.scroll ? 10 : 150,
            urlmultiple: null,
            urlActual: 0,
            urlParteAsmx: null,
            urlServicio: function() {
					if (this.urlmultiple != null)
                    {
                        var urlServ = this.urlmultiple[this.urlActual] + this.urlParteAsmx;
                        this.urlActual++;
                        if (this.urlActual == this.urlmultiple.length){this.urlActual = 0}
                        return urlServ;
                    }

                    return this.servicio.service;
				}
		}, options);
		
		// if highlight is set to false, replace it with a do-nothing function
		options.highlight = options.highlight || function(value) { return value; };
		
		// if the formatMatch option is not specified, then use formatItem for backwards compatibility
		options.formatMatch = options.formatMatch || options.formatItem;

        //Cargo las urls multiples en caso de haberlas:
        ObtenerUrlMultiple(options);
		
		return this.each(function() {
			new $.Autocompleter(this, options);
		});
	},
	result: function(handler) {
		return this.bind("result", handler);
	},
	search: function(handler) {
		return this.trigger("search", [handler]);
	},
	flushCache: function() {
		return this.trigger("flushCache");
	},
	setOptions: function(options){
		return this.trigger("setOptions", [options]);
	},
	unautocomplete: function() {
		return this.trigger("unautocomplete");
	}
});

$.Autocompleter = function(input, options) {

	var KEY = {
        LEFT: 37,
		UP: 38,
        RIGHT: 39,
		DOWN: 40,
		DEL: 46,
		TAB: 9,
		RETURN: 13,
		ESC: 27,
		COMMA: 188,
		PAGEUP: 33,
		PAGEDOWN: 34,
		BACKSPACE: 8
	};

	// Create $ object for input element
	var $input = $(input).attr("autocomplete", "off").addClass(options.inputClass);

    var cont = 0;
	var timeout;
	var previousValue = "";
	var cache = $.Autocompleter.Cache(options);
	var hasFocus = 0;
	var lastKeyPressCode;
	var config = {
		mouseDownOnSelect: false
	};
	var select = $.Autocompleter.Select(options, input, selectCurrent, pintarSeleccionado, config);
	
	var blockSubmit;
	
	// prevent form submit in opera when selecting with return key
//	$.browser.opera && $(input.form).bind("submit.autocomplete", function() {
//		if (blockSubmit) {
//			blockSubmit = false;
//			return false;
//		}
//	});
	
	 // only opera doesn't trigger keydown multiple times while pressed, others don't work with keypress at all
	$input.bind((/*$.browser.opera ? "keypress" : */"keyup") + ".autocomplete", function(event) {
		// a keypress means the input has focus
		// avoids issue where input had focus before the autocomplete was applied
		hasFocus = 1;
		// track last key pressed
		lastKeyPressCode = event.keyCode;
		switch(event.keyCode) {
		
			case KEY.UP:
				event.preventDefault();
				if ( select.visible() ) {
					select.prev();
				} else {
					onChange(0, true);
				}
				break;
				
			case KEY.DOWN:
				event.preventDefault();
				if ( select.visible() ) {
					select.next();
				} else {
					onChange(0, true);
				}
				break;
				
			case KEY.PAGEUP:
				event.preventDefault();
				if ( select.visible() ) {
					select.pageUp();
				} else {
					onChange(0, true);
				}
				break;
				
			case KEY.PAGEDOWN:
				event.preventDefault();
				if ( select.visible() ) {
					select.pageDown();
				} else {
					onChange(0, true);
				}
				break;
			// matches also semicolon
			case options.multiple && $.trim(options.multipleSeparator) == "," && KEY.COMMA:
				select.hide();
				PintarTags($input);
				break;
			case KEY.TAB:
			case KEY.RETURN:
			    cancelEvent(event);
				if( selectCurrent() ) {
					// stop default to prevent a form submit, Opera needs special handling
					event.preventDefault();
					blockSubmit = true;
					return false;
				}
				select.hide();
				PintarTags($input);
				break;
			case KEY.LEFT:
			case KEY.RIGHT:
			case KEY.ESC:
				select.hide();
				break;
			default:
				clearTimeout(timeout);
				timeout = setTimeout(onChange, options.delay);
				break;
		}
	}).focus(function(){
		// track whether the field has focus, we shouldn't process any
		// results if the field no longer has focus
		hasFocus++;
	}).blur(function() {
		hasFocus = 0;
		if (!config.mouseDownOnSelect) {
			hideResults();
		}
	}).click(function() {
		// show select when clicking in a focused field
		if ( hasFocus++ > 1 && !select.visible() ) {
			onChange(0, true);
		}
	}).bind("search", function() {
		// TODO why not just specifying both arguments?
		var fn = (arguments.length > 1) ? arguments[1] : null;
		function findValueCallback(q, data) {
			var result;
			if( data && data.length ) {
				for (var i=0; i < data.length; i++) {
					if( data[i].result.toLowerCase() == q.toLowerCase() ) {
						result = data[i];
						break;
					}
				}
			}
			if( typeof fn == "function" ) fn(result);
			else $input.trigger("result", result && [result.data, result.value]);
		}
		$.each(trimWords($input.val()), function(i, value) {
			request(value, findValueCallback, findValueCallback);
		});
	}).bind("flushCache", function() {
		cache.flush();
	}).bind("setOptions", function() {
		$.extend(options, arguments[1]);
		// if we've updated the data, repopulate
		if ( "data" in arguments[1] )
			cache.populate();
	}).bind("unautocomplete", function() {
		select.unbind();
		$input.unbind();
		$(input.form).unbind(".autocomplete");
	});
	
	
	function selectCurrent() {
		var selected = select.selected();
		if( !selected )
		{
            forzarClickBoton('', '');
			return false;
		}
        
        pintarSeleccionado($input, selected.result);
		
		hideResultsNow();
		$input.trigger("result", [selected.data, selected.value]);
			
        var faceta = '';
		
		if (selected.data.length > 2 /*&& selected.data[2] != ''*/)
		{
//		    var url = selected.data[2];
//		    url = url.substring(url.indexOf('url=') + 4);
//		    
//		    if (url.indexOf('|||') != -1)
//		    {
//		        url = url.substring(0, url.indexOf('|||'));
//		    }
		    
//		    if (selected.data[1] == 'foaf:firstName')
//		    {
//		        window.location.href = baseUrlBusqueda + '/' + urlRecursosBusqueda + '/' + url;
//		    }
//		    else if (selected.data[1] == 'gnoss:hasnombrecompleto')
//		    {
//		        url = url.replace('[perfil]',urlPerfilBusqueda).replace('[organizacion]',urlOrganizacionBusqueda).replace('[clase]',urlClaseBusqueda);
//		        window.location.href = baseUrlBusqueda + '/' + url;
//		    }
//          else if (selected.data[1] == 'formSem')
            if (selected.data[1] == 'formSem')
            {
                AgregarEntidadSeleccAutocompletar(selected.data);
            }
            else if (selected.data[1] == 'formSemGrafoDependiente')
            {
                AgregarValorGrafoDependienteAutocompletar(selected.data);
            }
            else if (selected.data[1] == 'formSemGrafoAutocompletar')
            {
                AgregarValorGrafoAutocompletar(selected.data);
            }
            else if (selected.data[1] == 'idioma' || selected.data[1].indexOf('[MultiIdioma]') != -1)
            {
                $input.val($input.val() + '@' + $('input.inpt_Idioma').val());
            }            
            else if (selected.data[1] == 'datoextraproyectovirtuoso')
            {
                $input.val($input.val());
                $input.attr('aux',$input.val());
                var inputHidden = $input.parent().find($('#'+$input.attr('id') + 'hack'));
                if(typeof(inputHidden.attr('id')) != 'undefined')
                {
                    inputHidden.val(selected.data[2]);
                    inputHidden.attr('aux',selected.data[2]);
                }
            }
            else if (typeof (facetasCMS) != 'undefined' && facetasCMS)
            {
                var botonBuscar = document.getElementById(options.extraParams.botonBuscar);
                if (botonBuscar.attributes['href'] != null)
                {
                    var urlRedirect = botonBuscar.attributes['href'].value;

                    if (urlRedirect.indexOf('?') != -1)
                    {
                        urlRedirect = urlRedirect.substring(0, rlRedirect.indexOf('?'));
                    }

                    window.location =  urlRedirect + '?' + selected.data[1] + '=' + $input.val();
                    return true;
                }
            }
            else
            {
                forzarClickBoton('', selected.result);
		        return true;
            }
		}
		else if (selected.data.length > 1)
		{
//		    faceta = selected.data[1];
		}
			
		forzarClickBoton(faceta, selected.result);
        
		return true;
	}
	
    function pintarSeleccionado(textbox, resultado)
    {
        if (!options.pintarConcatenadores)
        {
            resultado = QuitarContadores(resultado);
        }

        if (textbox.attr('id') == 'finderSection' || textbox.attr('id') == 'txtBusquedaPrincipal')
        {
            /*Si es el buscador de una página de busqueda o el metabuscador superior, autocompleta con "" */
            resultado='"'+resultado+'"';
        }   


        var v = resultado;
        previousValue = v;
		var cursorAt = textbox.selection().start;
		if(cursorAt < 0)
		{
		    cursorAt = textbox.val().indexOf(v) + resultado.length;
		}
        
	    if ( options.multiple ) {
		    var words = trimWords(textbox.val());
		    if ( words.length > 1 ) 
		    {
			    var seperator = options.multipleSeparator.length;
			    var wordAt, progress = 0;
			    $.each(words, function(i, word) {
				    progress += word.length;
				    if (cursorAt <= progress) {
					    wordAt = i;
					    return false;
				    }
				    progress += seperator;
			    });
			    words[wordAt] = v;
			    v = words.join( options.multipleSeparator );
		    }
	    }
	    
	    if (options.NoPintarSeleccionado == null || !options.NoPintarSeleccionado)
	    {
	        textbox.val(v);
	        PintarTags(textbox);
	    }
    }
	
	function forzarClickBoton(pFaceta, result)
	{		
	    //envia los datos al seleccionar una fila del autocompletar
        var objecte = document.getElementById(options.extraParams.botonBuscar);
		if(objecte != null)
		{
		    if (pFaceta != '') {
		        if (objecte.attributes['onclick'].value.indexOf('= url + parametros') != -1) {
		            eval(objecte.attributes['onclick'].value.replace('= url + parametros', '= url.replace("search=","' + pFaceta + '=") + parametros'));
		        }
		        else {
		            eval(objecte.attributes['onclick'].value.replace('search=', pFaceta + '=').replace('return false;', ''));
		        }
		    }
		        /*else if(document.createEvent)
                {
                    var evObj = document.createEvent('MouseEvents');
                    evObj.initEvent( 'click', true, false );
                    objecte.dispatchEvent(evObj);
                }*/
		    else {
		        if ($input.val().indexOf("\"") != 0 && $input.val().lastIndexOf("\"") != $input.val().length - 1 && typeof (result) != 'undefined' && result == '' && typeof ($input[0]) != 'undefined' && typeof ($input[0].className) != 'undefined' && $input[0].className.indexOf("filtroFaceta") >= 0) {
		            pintarSeleccionado($input, '>>' + $input.val())
		        }
		        objecte.click();
		    }
        }
	}
	
	function onChange(crap, skipPrevCheck) {
		if( lastKeyPressCode == KEY.DEL ) {
			select.hide();
			return;
		}
		
		var currentValue = $input.val();
		
		if ( !skipPrevCheck && currentValue == previousValue )
			return;
		
		previousValue = currentValue;
		
		currentValue = lastWord(currentValue);
		if ( currentValue.length >= options.minChars) {
			$input.addClass(options.loadingClass);
			if (!options.matchCase)
				currentValue = currentValue.toLowerCase();
			request(currentValue, receiveData, hideResultsNow);
		} else {
			stopLoading();
			select.hide();
		}
	};
	
	function trimWords(value) {
		if (!value)
			return [""];
		if (!options.multiple)
			return [$.trim(value)];
		return $.map(value.split(options.multipleSeparator), function(word) {
			return $.trim(value).length ? $.trim(word) : null;
		});
	}
	
	function lastWord(value) {
		if ( !options.multiple )
			return value;
		var words = trimWords(value);
		if (words.length == 1) 
			return words[0];
		var cursorAt = $(input).selection().start;
		if (cursorAt == value.length) {
			words = trimWords(value)
		} else {
			words = trimWords(value.replace(value.substring(cursorAt), ""));
		}
		return words[words.length - 1];
	}
	
	// fills in the input box w/the first match (assumed to be the best match)
	// q: the term entered
	// sValue: the first matching result
	function autoFill(q, sValue){
		// autofill in the complete box w/the first match as long as the user hasn't entered in more data
		// if the last user key pressed was backspace, don't autofill
		if( options.autoFill && (lastWord($input.val()).toLowerCase() == q.toLowerCase()) && lastKeyPressCode != KEY.BACKSPACE ) {
			// fill in the value (keep the case the user has typed)
			$input.val($input.val() + sValue.substring(lastWord(previousValue).length));
			// select the portion of the value not typed by the user (so the next character will erase)
			$(input).selection(previousValue.length, previousValue.length + sValue.length);
		}
	};

	function hideResults() {
		clearTimeout(timeout);
		timeout = setTimeout(hideResultsNow, 200);
	};

	function hideResultsNow() {
		var wasVisible = select.visible();
		select.hide();
		clearTimeout(timeout);
		stopLoading();
		if (options.mustMatch) {
			// call search and run callback
			$input.search(
				function (result){
					// if no value found, clear the input box
					if( !result ) {
						if (options.multiple) {
							var words = trimWords($input.val()).slice(0, -1);
							$input.val( words.join(options.multipleSeparator) + (words.length ? options.multipleSeparator : "") );
						}
						else {
							$input.val( "" );
							$input.trigger("result", null);
						}
					}
				}
			);
		}
	};

	function receiveData(q, data) {
		if ( data && data.length && hasFocus ) {
			stopLoading();
			select.display(data, q);
			autoFill(q, data[0].value);

			if (typeof (completadaCargaAutocompletar) != "undefined") {
			    completadaCargaAutocompletar();
			}

			select.show();
		} else {
			hideResultsNow();
		}
	};

	function request(term, success, failure) {
		
		modificarAutoCompletarHome();

	    term = replaceAll(replaceAll(replaceAll(term, '%', '%25'), '#', '%23'), '+', "%2B");
		if (!options.matchCase)
			term = term.toLowerCase();
		var data = cache.load(term);
		// recieve the cached data
		if (data && data.length) {
			success(term, data);
		// if an AJAX url has been supplied, try loading the data now
		} else if( (typeof options.url == "string") && (options.url.length > 0) ){
			
			var extraParams = {
				timestamp: +new Date()
			};
			$.each(options.extraParams, function(key, param) {
				extraParams[key] = typeof param == "function" ? param() : param;
			});
			
			$.ajax({
				// try to leverage ajaxQueue plugin to abort previous requests
				mode: "abort",
				// limit abortion to this input
				port: "autocomplete" + input.name,
				dataType: options.dataType,
				//setup new options for asmx - amokan			
				//end new options - amokan
				url: options.url,
				data: $.extend({
					q: lastWord(term),
					limit: options.max
				}, extraParams),
				success: function(data) {
					var parsed = options.parse && options.parse(data) || parse(data);
					cache.add(term, parsed);
					success(term, parsed);
				}
			});
		}
		else if (options.servicio != null) {
		    cont = cont + 1;
		    var extraParams = {
		        q: lastWord(term),
		        limit: options.max,
		        cont: cont,
		        lista: ''
		    };
	        if(options.multiple)
	        {
	            if (options.classTxtValoresSelecc != null) {
	                var valorLista = '';

	                $('.' + options.classTxtValoresSelecc).each(function () {
	                    valorLista += $(this).val().replace(/&/g, ',');
	                });

	                extraParams["lista"] = valorLista;
	            }
	            else if (options.txtValoresSeleccID == null) {
	                extraParams["lista"] = $('#' + input.id + '_Hack').val().trim() + $input.val().trim();
	                //extraParams["lista"] = previousValue.trim();
	            }
	            else {
	                if (options.txtValoresSeleccID.indexOf('|') != -1) {
	                    var idtxthacks = options.txtValoresSeleccID.split('|');
	                    var valorLista = '';
	                    for (var i = 0; i < idtxthacks.length; i++) {
	                        valorLista += document.getElementById(idtxthacks[i]).value.replace(/&/g, ',');
	                    }
	                    extraParams["lista"] = valorLista;
	                }
	                else {
	                    extraParams["lista"] = document.getElementById(options.txtValoresSeleccID).value.replace(/&/g, ',');
	                }
	            }
	        }
		    $.each(options.extraParams, function(key, param) {
		        extraParams[key] = typeof param == "function" ? param() : param;
		    });

            options.servicio.service = options.urlServicio(options);

		    options.servicio.call(options.metodo, extraParams, function(data) {
	            if(extraParams.cont == cont && $('#' + extraParams.botonBuscar).prev().parent().css('display') != 'none')
	            {
				    var parsed = options.parse && options.parse(data) || parse(data);
				    cache.add(term, parsed);
				    success(term, parsed);
				}
			});	
		} else {
			// if we have a failure, we need to empty the list -- this prevents the the [TAB] key from selecting the last successful match
			select.emptyList();
			failure(term);
		}
	};
	
	function parse(data) {
		var parsed = [];
		try
		{
			var rows = data.split("\n");
			for (var i=0; i < rows.length; i++) {
				var row = $.trim(rows[i]);
				if (row) {
				    if (row.indexOf('|||') != -1)
					{
					    row = row.split("|||");
					}
					else if (row.indexOf('|') != -1)
					{
					    var valor = row.substring(0, row.lastIndexOf('|'));
					    var attControl = row.substring(row.lastIndexOf('|') + 1);
					    row = [valor, attControl];
					}
					else
					{
					    row = row.split("|");//Para que cree un array de un elemento.
					}
					
					parsed[parsed.length] = {
						data: row,
						value: row[0],
						result: options.formatResult && options.formatResult(row, row[0]) || row[0]
					};
				}
			}
		}
		catch(ex)
		{}
		return parsed;
	};

	function stopLoading() {
		$input.removeClass(options.loadingClass);
	};
};

function QuitarContadores(cadena)
{
    var resultado = cadena;
    if (cadena.lastIndexOf('(') > -1)
    {
        if (cadena.lastIndexOf(')') > -1 && cadena.lastIndexOf(')') >cadena.lastIndexOf('('))
        {
            resultado = cadena.substring(0,cadena.lastIndexOf('(') -1);
        }
    }
    return resultado;
}

$.Autocompleter.defaults = {
	inputClass: "ac_input",
	resultsClass: "ac_results",
	loadingClass: "ac_loading",
	minChars: 1,
	delay: 400,
	matchCase: false,
	matchSubset: true,
	matchContains: false,
	cacheLength: 10,
	max: 100,
	mustMatch: false,
	extraParams: {},
	selectFirst: true,
	formatItem: function(row) { return row[0]; },
	formatMatch: null,
	autoFill: false,
	width: 0,
	multiple: false,
	multipleSeparator: ", ",
	/*highlight: function(value, term) {
		return value.replace(new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + term.replace(/([\^\$\(\)\[\]\{\}\*\.\+\?\|\\])/gi, "\\$1") + ")(?![^<>]*>)(?![^&;]+;)", "gi"), "<strong>$1</strong>");
	},*/
    scroll: true,
    scrollHeight: 180
};

$.Autocompleter.Cache = function(options) {

	var data = {};
	var length = 0;
	
	function matchSubset(s, sub) {
		if (!options.matchCase) 
			s = s.toLowerCase();
		var i = s.indexOf(sub);
		if (options.matchContains == "word"){
			i = s.toLowerCase().search("\\b" + sub.toLowerCase());
		}
		if (i == -1) return false;
		return i == 0 || options.matchContains;
	};
	
	function add(q, value) {
		if (length > options.cacheLength){
			flush();
		}
		if (!data[q]){ 
			length++;
		}
		data[q] = value;
	}
	
	function populate(){
		if( !options.data ) return false;
		// track the matches
		var stMatchSets = {},
			nullData = 0;

		// no url was specified, we need to adjust the cache length to make sure it fits the local data store
		if( !options.url ) options.cacheLength = 1;
		
		// track all options for minChars = 0
		stMatchSets[""] = [];
		
		// loop through the array and create a lookup structure
		for ( var i = 0, ol = options.data.length; i < ol; i++ ) {
			var rawValue = options.data[i];
			// if rawValue is a string, make an array otherwise just reference the array
			rawValue = (typeof rawValue == "string") ? [rawValue] : rawValue;
			
			var value = options.formatMatch(rawValue, i+1, options.data.length);
			if ( value === false )
				continue;
				
			var firstChar = value.charAt(0).toLowerCase();
			// if no lookup array for this character exists, look it up now
			if( !stMatchSets[firstChar] ) 
				stMatchSets[firstChar] = [];

			// if the match is a string
			var row = {
				value: value,
				data: rawValue,
				result: options.formatResult && options.formatResult(rawValue) || value
			};
			
			// push the current match into the set list
			stMatchSets[firstChar].push(row);

			// keep track of minChars zero items
			if ( nullData++ < options.max ) {
				stMatchSets[""].push(row);
			}
		};

		// add the data items to the cache
		$.each(stMatchSets, function(i, value) {
			// increase the cache size
			options.cacheLength++;
			// add to the cache
			add(i, value);
		});
	}
	
	// populate any existing data
	setTimeout(populate, 25);
	
	function flush(){
		data = {};
		length = 0;
	}
	
	return {
		flush: flush,
		add: add,
		populate: populate,
		load: function(q) {
			if (!options.cacheLength || !length)
				return null;
			/* 
			 * if dealing w/local data and matchContains than we must make sure
			 * to loop through all the data collections looking for matches
			 */
			if( !options.url && options.matchContains ){
				// track all matches
				var csub = [];
				// loop through all the data grids for matches
				for( var k in data ){
					// don't search through the stMatchSets[""] (minChars: 0) cache
					// this prevents duplicates
					if( k.length > 0 ){
						var c = data[k];
						$.each(c, function(i, x) {
							// if we've got a match, add it to the array
							if (matchSubset(x.value, q)) {
								csub.push(x);
							}
						});
					}
				}				
				return csub;
			} else 
			// if the exact item exists, use it
			if (data[q]){
				return data[q];
			} else
			if (options.matchSubset) {
				for (var i = q.length - 1; i >= options.minChars; i--) {
					var c = data[q.substr(0, i)];
					if (c) {
						var csub = [];
						$.each(c, function(i, x) {
							if (matchSubset(x.value, q)) {
								csub[csub.length] = x;
							}
						});
						return csub;
					}
				}
			}
			return null;
		}
	};
};

$.Autocompleter.Select = function (options, input, select, pintar, config) {
	var CLASSES = {
		ACTIVE: "ac_over"
	};
	
	var listItems,
		active = -1,
		data,
		term = "",
		needsInit = true,
		element,
		list;
	
	// Create results
	function init() {
		if (!needsInit)
			return;
		element = $("<div/>")
		.hide()
		.addClass(options.resultsClass)
		.css("position", "absolute")
		//.appendTo(document.body);

        if (typeof panelContAutoComplet != 'undefined') {
            element.appendTo($("#" + panelContAutoComplet));
        }
        else
        {
            element.appendTo(document.body);
        }
	
		list = $("<ul/>").appendTo(element).mouseover( function(event) {
			if(target(event).nodeName && target(event).nodeName.toUpperCase() == 'LI') {
	            active = $("li", list).removeClass(CLASSES.ACTIVE).index(target(event));
			    $(target(event)).addClass(CLASSES.ACTIVE);
	        }
		}).click(function(event) {
			$(target(event)).addClass(CLASSES.ACTIVE);
			select();
			// TODO provide option to avoid setting focus again after selection? useful for cleanup-on-focus
			input.focus();
			return false;
		}).mousedown(function(event) {
            cancelEvent(event);
			config.mouseDownOnSelect = true;
		}).mouseup(function() {
			config.mouseDownOnSelect = false;
		});
		
		if( options.width > 0 )
			element.css("width", options.width);
			
		if (options.extraParams.maxwidth)
		    element.css("max-width", options.extraParams.maxwidth);
			
		needsInit = false;
	} 
	
	function target(event) {
		var element = event.target;
		while(element && element.tagName != "LI")
			element = element.parentNode;
		// more fun with IE, sometimes event.target is empty, just ignore it then
		if(!element)
			return [];
		return element;
	}

	function moveSelect(step) {
		listItems.slice(active, active + 1).removeClass(CLASSES.ACTIVE);
		movePosition(step);
        var activeItem = listItems.slice(active, active + 1).addClass(CLASSES.ACTIVE);
        if(options.scroll) {
            var offset = 0;
            listItems.slice(0, active).each(function() {
				offset += this.offsetHeight;
			});
            if((offset + activeItem[0].offsetHeight - list.scrollTop()) > list[0].clientHeight) {
                list.scrollTop(offset + activeItem[0].offsetHeight - list.innerHeight());
            } else if(offset < list.scrollTop()) {
                list.scrollTop(offset);
            }
        } 

//	    try
//	    {
//            pintar($(input), activeItem.html());
//        }
//        catch(ex)
//        {}
		//$(input).val($(input).val().replace(lastWord($(input).val()), QuitarContadores(activeItem.html())));
		//$(input).val(QuitarContadores(activeItem.html()));     
	};
	
	function movePosition(step) {
		active += step;
		if (active < 0) {
			active = listItems.size() - 1;
		} else if (active >= listItems.size()) {
			active = 0;
		}
	}
	
	function limitNumberOfItems(available) {
		return options.max && options.max < available
			? options.max
			: available;
	}
	
	function fillList() {
		list.empty();
		var max = limitNumberOfItems(data.length);
		for (var i=0; i < max; i++) {
			if (!data[i])
				continue;
			var formatted = options.formatItem(data[i].data, i+1, max, data[i].value, term);
			if ( formatted === false )
				continue;
			var li = $("<li/>").html( options.highlight(formatted, term) ).addClass(i%2 == 0 ? "ac_even" : "ac_odd").appendTo(list)[0];
			$.data(li, "ac_data", data[i]);
		}
		listItems = list.find("li");
		if ( options.selectFirst ) {
			listItems.slice(0, 1).addClass(CLASSES.ACTIVE);
			active = 0;
		}
		// apply bgiframe if available
		if ( $.fn.bgiframe )
			list.bgiframe();
	}
	
	return {
		display: function(d, q) {
			init();
			data = d;
			term = q;
			fillList();
		},
		next: function() {
			moveSelect(1);
		},
		prev: function() {
			moveSelect(-1);
		},
		pageUp: function() {
			if (active != 0 && active - 8 < 0) {
				moveSelect( -active );
			} else {
				moveSelect(-8);
			}
		},
		pageDown: function() {
			if (active != listItems.size() - 1 && active + 8 > listItems.size()) {
				moveSelect( listItems.size() - 1 - active );
			} else {
				moveSelect(8);
			}
		},
		hide: function() {
			element && element.hide();
			listItems && listItems.removeClass(CLASSES.ACTIVE);
			active = -1;
		},
		visible : function() {
			return element && element.is(":visible");
		},
		current: function() {
			return this.visible() && (listItems.filter("." + CLASSES.ACTIVE)[0] || options.selectFirst && listItems[0]);
		},
		show: function() {
            if (typeof panelContAutoComplet != 'undefined') {
                element.css('display','block');
            }
            else
            {
            	var identificador = $(input).attr('id');
			    var offset = $(input).offset();
			    element.addClass(identificador);
			    element.css({
				    width: typeof options.width == "string" || options.width > 0 ? options.width : $(input).width(),
				    top: offset.top + input.offsetHeight - 50,
				    left: offset.left
			    }).show();
            }
            if(options.scroll) {
                list.scrollTop(0);
                list.css({
					maxHeight: options.scrollHeight,
					overflow: 'auto'
				});
				
//                if($.browser.msie && typeof document.body.style.maxHeight === "undefined") {
//					var listHeight = 0;
//					listItems.each(function() {
//						listHeight += this.offsetHeight;
//					});
//					var scrollbarsVisible = listHeight > options.scrollHeight;
//                    list.css('height', scrollbarsVisible ? options.scrollHeight : listHeight );
//					if (!scrollbarsVisible) {
//						// IE doesn't recalculate width when scrollbar disappears
//						listItems.width( list.width() - parseInt(listItems.css("padding-left")) - parseInt(listItems.css("padding-right")) );
//					}
//                }
                
            }
		},
		selected: function() {
			var selected = listItems && listItems.filter("." + CLASSES.ACTIVE).removeClass(CLASSES.ACTIVE);
			return selected && selected.length && $.data(selected[0], "ac_data");
		},
		emptyList: function (){
			list && list.empty();
		},
		unbind: function() {
			element && element.remove();
		}
	};
};

$.fn.selection = function(start, end) {
	if (start !== undefined) {
		return this.each(function() {
			if( this.createTextRange ){
				var selRange = this.createTextRange();
				if (end === undefined || start == end) {
					selRange.move("character", start);
					selRange.select();
				} else {
					selRange.collapse(true);
					selRange.moveStart("character", start);
					selRange.moveEnd("character", end);
					selRange.select();
				}
			} else if( this.setSelectionRange ){
				this.setSelectionRange(start, end);
			} else if( this.selectionStart ){
				this.selectionStart = start;
				this.selectionEnd = end;
			}
		});
	}
	var field = this[0];
	if ( field.createTextRange && document.selection && document.selection.createRange ) {
		var range = document.selection.createRange(),
			orig = field.value,
			teststring = "<->",
			textLength = range.text.length;
		range.text = teststring;
		var caretAt = field.value.indexOf(teststring);
		field.value = orig;
		this.selection(caretAt, caretAt + textLength);
		return {
			start: caretAt,
			end: caretAt + textLength
		}
	} else if( field.selectionStart !== undefined ){
		return {
			start: field.selectionStart,
			end: field.selectionEnd
		}
	}
};
})(jQuery);


function PintarTags(textBox)
{	

    if(textBox.val().trim() != "")
    {
        var tags = textBox.val().replace(';', ',').split(',');
        
        var contenedor = textBox.parents('.autocompletar').find('.contenedor');
        var textBoxHack = textBox.parents('.autocompletar').find('input').last();
        
        if(textBoxHack.length > 0)
        {
            for(var i=0; i<tags.length; i++)
            {
                var tagNombre = tags[i].trim();
                var tagNombreEncode = Encoder.htmlEncode(tagNombre);
                
                var estaYaAgregada = textBoxHack.val().trim().indexOf(',' + tagNombre + ',') != -1;
                estaYaAgregada = estaYaAgregada || textBoxHack.val().trim().substring(0, tagNombre.length + 1) == tagNombre + ',';
                
                if(tagNombre != '' && (!estaYaAgregada || textBox.parents('.tag').length > 0))
                {
	                var html = "<div class=\"tag\" title=\"" + tagNombreEncode + "\"><div>" + tagNombre + "<a class=\"remove\" ></a></div><input type=\"text\" value=\"" + tagNombreEncode + "\"></div>";
                    if(textBox.parents('.tag').length > 0)
                    {
                        textBox.parents('.tag').before(html);
                    }
                    else
                    {
	                    contenedor.append(html);  
                    }
                    
                    textBoxHack.val(textBoxHack.val() + tagNombre.toLowerCase() + ',')
                }
            }
            
            textBox.val('');
            
            if(textBox.parents('.tag').length == 0)
            {
                PosicionarTextBox(textBoxHack.prev());
            }
            
            if(!textBoxHack.prev().hasClass("no-edit"))
            {
                textBox.parents('.autocompletar').find('.tag').each(function(){
                    $(this).bind('click', function(evento){
                        cancelEvent(evento);
                         
                        var divTag = $(this).children('div');
                        var textBox = divTag.parent().find('input');
                        if(textBox.css('display') == 'none')
                        {
                            textBox.width(textBox.parent().width());
                            divTag.css('display', 'none');
                            textBox.css('display', 'block');
                            textBox.focus();
                            posicionarCursor(textBox,textBox.val().length);
                            textBox.blur(function(){ActualizarTag(textBox, divTag, textBoxHack)});
                            textBox.keydown(function(evento){
                                $(this).attr('size',$(this).val().length + 5);
                                if(evento.which || evento.keyCode){
                                    if ((evento.which == 13) || (evento.keyCode == 13)) {
                                        ActualizarTag(textBox, divTag, textBoxHack);
                                        return false;
                                    }
                                }
                            });
                            textBox.keyup(function(evento){
                                if(evento.which || evento.keyCode){
                                    if ((evento.which == 188) || (evento.keyCode == 188)) {
                                        ActualizarTag(textBox, divTag, textBoxHack);
                                    }
                                }
                            });
                        }
                    });
                });  
            }
            
             textBox.parents('.autocompletar').find('.tag .remove').each(function(){
                if($(this).data("events") == null)
                {
                    $(this).bind('click', function(evento){
                        cancelEvent(evento);
                        EliminarTag($(this).parents('.tag'), evento)
                    });
                }
            });
        }
    }
    tagYaPintado = true;
}
function PintarTagsSinEliminar2(textBox) {

    if (textBox.val().trim() != "") {
        var tags = textBox.val().replace(';', ',').split(',');

        var contenedor = textBox.parents('.autocompletar').find('.contenedor');
        var textBoxHack = textBox.parents('.autocompletar').find('input').last();

        if (textBoxHack.length > 0) {
            for (var i = 0; i < tags.length; i++) {
                var tagNombre = tags[i].trim();
                var tagNombreEncode = Encoder.htmlEncode(tagNombre);

                var estaYaAgregada = textBoxHack.val().trim().indexOf(',' + tagNombre + ',') != -1;
                estaYaAgregada = estaYaAgregada || textBoxHack.val().trim().substring(0, tagNombre.length + 1) == tagNombre + ',';

                if (tagNombre != '' && (!estaYaAgregada || textBox.parents('.tag').length > 0)) {
                    var html = "<div class=\"tag\" title=\"" + tagNombreEncode + "\"><div>" + tagNombre + "<a class=\"remove\" ></a></div><input type=\"text\" value=\"" + tagNombreEncode + "\"></div>";
                    if (textBox.parents('.tag').length > 0) {
                        textBox.parents('.tag').before(html);
                    }
                    else {
                        contenedor.append(html);
                    }

                    textBoxHack.val(textBoxHack.val() + tagNombre.toLowerCase() + ',')
                }
            }

            textBox.val('');

            if (textBox.parents('.tag').length == 0) {
                PosicionarTextBox(textBoxHack.prev());
            }

            if (!textBoxHack.prev().hasClass("no-edit")) {
                textBox.parents('.autocompletar').find('.tag').each(function () {
                    $(this).bind('click', function (evento) {
                        cancelEvent(evento);

                        var divTag = $(this).children('div');
                        var textBox = divTag.parent().find('input');
                        if (textBox.css('display') == 'none') {
                            textBox.width(textBox.parent().width());
                            divTag.css('display', 'none');
                            textBox.css('display', 'block');
                            textBox.focus();
                            posicionarCursor(textBox, textBox.val().length);
                            textBox.blur(function () { ActualizarTag(textBox, divTag, textBoxHack) });
                            textBox.keydown(function (evento) {
                                $(this).attr('size', $(this).val().length + 5);
                                if (evento.which || evento.keyCode) {
                                    if ((evento.which == 13) || (evento.keyCode == 13)) {
                                        ActualizarTag(textBox, divTag, textBoxHack);
                                        return false;
                                    }
                                }
                            });
                            textBox.keyup(function (evento) {
                                if (evento.which || evento.keyCode) {
                                    if ((evento.which == 188) || (evento.keyCode == 188)) {
                                        ActualizarTag(textBox, divTag, textBoxHack);
                                    }
                                }
                            });
                        }
                    });
                });
            }

            textBox.parents('.autocompletar').find('.tag .remove').each(function () {
                if ($(this).data("events") == null) {
                    $(this).bind('click', function (evento) {
                        cancelEvent(evento);
                        EliminarTag($(this).parents('.tag'), evento)
                    });
                }
            });
        }
    }
    tagYaPintado = true;
}

function PosicionarTextBox(textBox)
{
    textBox.width(150);
    textBox.css('top', '0px');
    textBox.css('left', '0px');

    if (textBox.parent().find('.tag').length == 0 || textBox.position().top > textBox.parent().find('.tag').last().position().top)
    {
        textBox.css('width', '100%');
    }
    else
    {
        var tbLeft = textBox.parent().find('.tag').last().position().left + textBox.parent().find('.tag').last().width() + 5;
        textBox.width(textBox.parent().width() - (tbLeft - textBox.parent().position().left));
    }
}

function LimpiarTags(textBox)
{
    $('#' + txtTagsID + '_Hack').val('');
    $('#' + txtTagsID).parent().find('.tag').remove();
}

function ActualizarTag(textBox, divTag, textBoxHack)
{	
    var ultimoElemento = textBoxHack.parent();
    if(ultimoElemento.next().hasClass('propuestos'))
    {
        ultimoElemento = ultimoElemento.next();
    }
    descartarTag(textBox.parents('.tag'), ultimoElemento);
    
    textBox.css('display', '');
    PintarTags(textBox);
    
    var valorAnterior = textBox.parents('.tag').attr('title').toLowerCase();
    var hack = textBoxHack.val().trim();
    
    if(hack.indexOf(',' + valorAnterior + ',') != -1)
    {
        hack = hack.replace(',' + valorAnterior + ',', ',');
    }
    else if(hack.substring(0, valorAnterior.length + 1) == valorAnterior + ',')
    {
        hack = hack.replace(valorAnterior + ',', '');
    }
                
    textBoxHack.val(hack.trim());
    textBox.parent().remove();
    PosicionarTextBox(textBoxHack.prev());

}

function EliminarTag(elemento, evento)
{
    var divAutocompletar = elemento.parents('.autocompletar');
    if(divAutocompletar.find('input').length > 0)
    {
        var valorAnterior = elemento.attr('title');
        var textBoxHack = divAutocompletar.find('input').last();
        textBoxHack.val(textBoxHack.val().replace(valorAnterior.toLowerCase() + ',', ''));
        var ultimoElemento = divAutocompletar;
        if(divAutocompletar.next().hasClass('propuestos'))
        {
            var listaPropuestos = divAutocompletar.next().find('.tag');

            for(var i=0;i<listaPropuestos.length;i++)
            {
                if($(listaPropuestos[i]).attr('title') == valorAnterior)
                {
                    $(listaPropuestos[i]).css('display', '');
                }
            }
            ultimoElemento = divAutocompletar.next();
        }
        
        descartarTag(elemento, ultimoElemento);
        
        elemento.remove();
        PosicionarTextBox(textBoxHack.prev());
    }
}

function descartarTag(elemento, ultimoElemento)
{
    if(!ultimoElemento.next().hasClass('descartados'))
    {
        ultimoElemento.after("<div class='descartados' style='display:none;'><input id='txtHackDescartados' type='text'/></div>");
        ultimoElemento.next().find('#txtHackDescartados').val(elemento.attr('title').toLowerCase() + ',');
    }
    else
    {
        var descartados = ultimoElemento.next().find('#txtHackDescartados').val();
        
        var estaYaAgregada = descartados.indexOf(',' + elemento.attr('title') + ',') != -1;
        estaYaAgregada = estaYaAgregada || descartados.substring(0, elemento.attr('title').length + 1) == elemento.attr('title') + ',';
        
        if(!estaYaAgregada)
        {
            descartados += elemento.attr('title').toLowerCase() + ','
            ultimoElemento.next().find('#txtHackDescartados').val(descartados);
        }
    }
}

function posicionarCursor(textbox, pos) {
    if (textbox.get(0).setSelectionRange) {
        textbox.get(0).setSelectionRange(pos, pos);
    } else if (textbox.get(0).createTextRange) {
        var range = textbox.get(0).createTextRange();
        range.collapse(true);
        range.moveEnd('character', pos);
        range.moveStart('character', pos);
        range.select();
    }
}

$(document).ready(function() {
    pintarTagsInicio();
});

function pintarTagsInicio()
{
    $('.autocompletar').each(function(){
        $(this).bind('click', function(evento){
            cancelEvent(evento);
            $(this).find('input.txtAutocomplete').focus();
        });
    });
    
    $('.autocompletar input.txtAutocomplete').each(function(){
        PosicionarTextBox($(this));
        $(this).bind('keydown', function(evento){
            if ((evento.which == 8) || (evento.keyCode == 8)) {
                if($(this).val() == "")
                {
                    if($(this).parent().find('.tag').last().hasClass("selected"))
                    {
                        EliminarTag($(this).parent().find('.tag').last(), evento);
                    }
                    else
                    {
                        $(this).parent().find('.tag').last().addClass("selected");
                    }
                    return false;
                }
            }
            else if ((evento.which == 9) || (evento.keyCode == 9) || (evento.which == 13) || (evento.keyCode == 13)) {
                //Tabulador o Intro
                return false;
            }
            else
            {
                if($(this).parent().find('.tag').last().hasClass("selected"))
                {
                    $(this).parent().find('.tag').last().removeClass("selected");
                }
            }
        });
        $(this).bind('blur', function(evento){            
            PintarTags($(this));
        });
        $(this).bind('click', function(evento){
            cancelEvent(evento);
        });
        PintarTags($(this));
    });
}


function cancelEvent(e) {
    if (!e) e = window.event;
    if (e.preventDefault) {
        e.preventDefault();
    } else {
        e.returnValue = false;
    }
        
    if (!e) e = window.event;
    if (e.stopPropagation) {
        e.stopPropagation();
    } else {
        e.cancelBubble = true;
    }
}

function ObtenerUrlMultiple(pOptions){
    if (pOptions.servicio != null && pOptions.servicio.service.indexOf(',') != -1)
    {
        pOptions.urlParteAsmx = pOptions.servicio.service.substring(pOptions.servicio.service.lastIndexOf(',') + 1);
        pOptions.urlmultiple = pOptions.servicio.service.substring(0, pOptions.servicio.service.lastIndexOf(',')).split(',');
        pOptions.urlActual = aleatorio(0, pOptions.urlmultiple.length - 1);
    }
}

function aleatorio(inferior,superior){ 
    numPosibilidades = superior - inferior;
    aleat = Math.random() * numPosibilidades;
    aleat = Math.round(aleat);
    return parseInt(inferior) + aleat;
}/**/ 
/*jquery.gnoss.header.js*/ 
var globalIsContrayendo = false;
var confirmacionEliminacionMultiple = {
	cssConfirmacion: '.confirmacionMultiple',
	items: [
		'.listToolBar .delete a'
	],	
	init: function(){
		this.config();
		return;
	},
	config: function(){
		this.confirmacion = $(this.cssConfirmacion);
		var that = this;
		$.each(this.items, function(indice){
			var item = $(that.items[indice]);
			item.bind('click', function(evento){
				that.engancharEvento(evento);
				evento.preventDefault();
			});
		});
		return;
	},
	engancharEvento: function(evento){
		this.mostrarConfirmacion();
		return;
	},
	mostrarConfirmacion: function(){
		this.confirmacion.show();
	}
}
var confirmacionEliminacionSencilla = {
	cssConfirmacion: '.confirmacionSencilla',
	items: [
		'.resource-list .resource .delete'
	],	
	init: function(){
		this.config();
		return;
	},
	config: function(){
		this.confirmacion = $(this.cssConfirmacion);
		this.pregunta = this.confirmacion.find('.pregunta');
		var that = this;
		$.each(this.items, function(indice){
			var item = $(that.items[indice]);
			item.bind('click', function(evento){
				that.engancharEvento(evento);
				evento.preventDefault();
			});
		});
		return;
	},
	engancharEvento: function(evento){
		this.currentBoton = $(evento.target);
		this.encontrarRecurso();
		this.mostrarConfirmacion();
		return;
	},
	encontrarRecurso: function(){
		this.currentRecurso = this.currentBoton.parents('.resource');
		return;
	},
	mostrarConfirmacion: function(){
		var altura = this.currentRecurso.height() + 'px';
		var margin = (this.currentRecurso.height() / 3) + 'px';
		var top = this.currentRecurso.position().top + 'px';
		this.confirmacion.css({
			'height': altura,
			'top': top
		});
		this.pregunta.css('margin-top', margin);
		
		this.confirmacion.show();
	}
}
var desplegableUsuario = {
	idEspacios: '#usuarioConectado',
	cssEnlace: '.enlaceDesplegable',
	cssDesplegable: '.desplegable',
	cssListadoAmpliado: '.listadoAmpliado',
	init: function(){
		this.config();
		this.engancharComportamiento();
	},
	config: function(){
		this.espacios = $(this.idEspacios);
		this.enlaces = this.espacios.find(this.cssEnlace);
	},
	engancharComportamiento: function(){
		var that = this;
		this.enlaces.each(function(indice){
			var enlace = $(this);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			});
		});
		return;
	},
	ocultarTodos: function(){
		this.enlaces.each(function(indice){
			var parrafo = $(this);
			var div = parrafo.parent();
			parrafo.removeClass('activo');
			div.removeClass('showDesplegable');
		});
		return;
	},
	comportamiento: function(evento){
		var parrafo = $(evento.target);
		if(!parrafo.is('P')){
			parrafo = parrafo.parents('p').first();
		};
		var div = parrafo.parent();
		if(!parrafo.hasClass('activo')) this.ocultarTodos();
		parrafo.hasClass('activo') ? parrafo.removeClass('activo') : parrafo.addClass('activo');
		div.hasClass('showDesplegable') ? div.removeClass('showDesplegable') : div.addClass('showDesplegable');
		return;	
	}
}

var desplegablesEspacios = {
	idEspacios: '#espacios',
	cssEnlace: '.enlaceDesplegable',
	cssDesplegable: '.desplegable',
	cssListadoAmpliado: '.listadoAmpliado',
	init: function(){
		this.config();
		this.engancharComportamiento();
	},
	config: function(){
		this.espacios = $(this.idEspacios);
		this.enlaces = this.espacios.find(this.cssEnlace);
	},
	engancharComportamiento: function(){
		var that = this;
		this.enlaces.each(function(indice){
			var enlace = $(this);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			});
		});
		return;
	},
	ocultarTodos: function(){
		this.enlaces.each(function(indice){
			var enlace = $(this);
			var li = enlace.parent();
			enlace.removeClass('activo');
			li.removeClass('showDesplegable');
		});
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		var li = enlace.parent();
		if(!enlace.hasClass('activo')) this.ocultarTodos();
		enlace.hasClass('activo') ? enlace.removeClass('activo') : enlace.addClass('activo');
		li.hasClass('showDesplegable') ? li.removeClass('showDesplegable') : li.addClass('showDesplegable');
		return;	
	}
}

var desplegablesEspaciosGNOSS = {
	idEspacios: '#identidadGNOSS',
	cssEnlace: '.enlaceDesplegable',
	cssDesplegable: '.desplegable',
	cssListadoAmpliado: '.listadoAmpliado',
	init: function(){
		this.config();
		this.engancharComportamiento();
	},
	config: function(){
		this.espacios = $(this.idEspacios);
		this.enlaces = this.espacios.find(this.cssEnlace);
	},
	engancharComportamiento: function(){
		var that = this;
		this.enlaces.each(function(indice){
			var enlace = $(this);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			});
		});
		return;
	},
	ocultarTodos: function(){
		this.enlaces.each(function(indice){
			var enlace = $(this);
			var li = enlace.parent();
			enlace.removeClass('activo');
			li.removeClass('showDesplegable');
		});
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		var li = enlace.parent();
		if(!enlace.hasClass('activo')) this.ocultarTodos();
		enlace.hasClass('activo') ? enlace.removeClass('activo') : enlace.addClass('activo');
		li.hasClass('showDesplegable') ? li.removeClass('showDesplegable') : li.addClass('showDesplegable');
		return;	
	}
}

var desplegablesOtrasIdentidades = {
	idEspacios: '#otrasIdentidades',
	cssEnlace: '.enlaceDesplegable',
	cssDesplegable: '.desplegable',
	cssListadoAmpliado: '.listadoAmpliado',
	init: function(){
		this.config();
		this.engancharComportamiento();
	},
	config: function(){
		this.espacios = $(this.idEspacios);
		this.enlaces = this.espacios.find(this.cssEnlace);
	},
	engancharComportamiento: function(){
		var that = this;
		this.enlaces.each(function(indice){
			var enlace = $(this);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			});
		});
		return;
	},
	ocultarTodos: function(){
		var that = this;
		this.enlaces.each(function(indice){
			var enlace = $(this);
			enlace.removeClass('activo');
			that.espacios.removeClass('showDesplegable');
		});
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		if(!enlace.hasClass('activo')) this.ocultarTodos();
		enlace.hasClass('activo') ? enlace.removeClass('activo') : enlace.addClass('activo');
		this.espacios.hasClass('showDesplegable') ? this.espacios.removeClass('showDesplegable') : this.espacios.addClass('showDesplegable');
		return;	
	}
}

var ampliarContraerListados = {
	css: '.mostrarListadoAmpliado',
	cssVerTodas: '.mostrarListadoTodos',
	init: function(){
		this.config();
		this.engancharComportamiento();
	},
	config: function(){
		this.enlaces = $(this.css);
	},
	engancharComportamiento: function(){
		var that = this;
		this.enlaces.each(function(indice){
			var parrafo = $(this);
			var enlace = parrafo.find('a');
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			});					
		});
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		var contenedor = enlace.parent().parent();
		var ulAmpliado = contenedor.find('ul.listadoAmpliado');
		var ulListado = ulAmpliado.prev();
		var pVerTodos = enlace.parent().next();
		var hasVerTodos = pVerTodos.size() > 0;
		var texto = enlace.text();
		var textoAlt = enlace.attr('rel');
		enlace.text(textoAlt);
		enlace.attr('rel', texto);
		if(ulAmpliado.is(':visible')){
			ulAmpliado.hide();
			globalIsContrayendo = true;
		}else{
			if(hasVerTodos){
				ulListado.hide();
				enlace.parent().hide();
				pVerTodos.show();
				ulAmpliado.show();
				
			}else{
				ulAmpliado.show();
			}
			globalIsContrayendo = false;
		}
	}
}

var ocultarDesplegables = {
	idPage: '#page',
	init: function(){
		$(this.idPage).hover( function(){
			if(globalIsContrayendo) return;
			desplegablesEspacios.ocultarTodos();
			desplegableUsuario.ocultarTodos();
			desplegablesOtrasIdentidades.ocultarTodos();
			desplegablesEspaciosGNOSS.ocultarTodos();
			desplegablesIdentidad.ocultarTodos();
		},function(){});
	}
}

var hoverDesplegable = {
	desplegables: '.desplegable',
	init: function(){
		$(this.desplegables).each(function(){
			var desplegable = $(this);
			desplegable.hover(
			function(){
				globalIsContrayendo = false;
			},function(){});			
		});
	}
}

var montarModuloOtrasIdentidades = {
	id: '#otrasIdentidades',
	cssDesplegable: '.panelMasIdentidades',
	n: 4,
	isMuchasIdentidades: false,
	init: function(){
		this.config();
		this.determinarItemsAMostrar();
	},
	config: function(){
		this.modulo = $(this.id);
		this.desplegable = this.modulo.find(this.cssDesplegable);
		this.ulVisible = this.modulo.find('ul').first();
		this.botonMasIdentidades = this.ulVisible.find('li.masIdentidades');
		this.ul = this.desplegable.find('ul').first();
		this.lis = this.ul.find('li');
		return;
	},
	determinarItemsAMostrar: function(){	
		// Deprecado size() 
        // var numeroItems = this.lis.size();
        var numeroItems = this.lis.length;
		if(numeroItems == 0){
			this.modulo.remove();
		}else{
			if(numeroItems > this.n) this.isMuchasIdentidades = true;
			this.mostrarLosNPrimeros()
		}
		return;
	},
	mostrarLosNPrimeros: function(){
		var that = this;
		if(!this.isMuchasIdentidades){
			this.desplegable.remove();
			this.botonMasIdentidades.remove();
			this.lisMostrar = this.lis.splice(0, this.n);
		}else{
			this.lisMostrar = this.lis.splice(0, this.n - 1);			
		}	
		this.lisMostrar = $(this.lisMostrar.reverse());
		this.lisMostrar.each(function(indice){
			var li = $(this);
			that.ulVisible.prepend(li);
		});
		return;
	}
}
marcarItemsDesplegables = {
	css: '.enlaceDesplegable',
	init: function(){
		this.comportamiento();
	},
	config: function(){
		return;
	},
	comportamiento: function(){
		var enlaces = $(this.css);
		enlaces.each(function(){
			var enlace = $(this);
			if(enlace.parents('.identidadGNOSS').size() > 0) return;
			enlace.addClass('menuDesplegable');
		});
		return;
	}
}
var marcarIdentidadProfesor = {
	cssProfesor: '.profesor',
	init: function(){
		this.config();
		this.marcar();
	},
	config: function(){
		this.identidades = $('#otrasIdentidades ' + this.cssProfesor);
		return;
	},
	marcar: function(){
		this.identidades.each(function(){
			var identidad = $(this);
			var title = identidad.attr('title');
			var li = identidad.parent();
			li.css('position','relative');
			li.append('<span class="identidadProfesor" title="' + title + '">identidad profesor</span>');
		});
		return;
	}
}

var desplegablesIdentidad = {
	idEspacios: '#identidad',
	cssEnlace: '.enlaceDesplegable',
	cssDesplegable: '.desplegable',
	cssListadoAmpliado: '.listadoAmpliado',
	init: function(){
		this.config();
		this.engancharComportamiento();
	},
	config: function(){
		this.espacios = $(this.idEspacios);
		this.enlaces = this.espacios.find(this.cssEnlace);
	},
	engancharComportamiento: function(){
		var that = this;
		this.enlaces.each(function(indice){
			var enlace = $(this);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			});
		});
		return;
	},
	ocultarTodos: function(){
		this.enlaces.each(function(indice){
			var enlace = $(this);
			var li = enlace.parent();
			enlace.removeClass('activo');
			li.removeClass('showDesplegable');
		});
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		var li = enlace.parent();
		if(!enlace.hasClass('activo')) this.ocultarTodos();
		enlace.hasClass('activo') ? enlace.removeClass('activo') : enlace.addClass('activo');
		li.hasClass('showDesplegable') ? li.removeClass('showDesplegable') : li.addClass('showDesplegable');
		return;	
	}
}
var buscadorCabecera = {
    idBuscador: '#buscador',
    cssSearchGroup: '.searchGroup',
    literales: [],
    init: function (id) {
        var buscador = id || this.idBuscador;
        var that = this;
        this.buscador = $(buscador);
        this.config();
        this.wrapBuscador.show();
        this.anchoSearchGroup = this.searchGroup.width();
        // Nuevo Front. Cargaba recursos, y m�s opciones cuando el usuario no estaba registrado (Lo montaba despu�s del buscador). No tiene sentido
        //this.montarSelector();
        // A simple vista, no monta ninguna opci�n. Parece ser un submen�. Ahora con nuevo Front no har�a falta.
        //this.montarOpciones();
        this.marcarDefaultInput();
        // Asocia clase cuando hay "focus". Lo elimino de momento
        //this.engancharInput();
        // Comportamiento para paneles y subpaneles (Desplegado, no desplegado). Para el nuevo Front, lo elimino de momento
        //this.engancharSelector();
        // C�lculo din�mico de paneles del viejo front. De momento lo oculto
        //this.engancharOpciones();
        // Comportamiento de establecer "selected" a opciones. Para el nuevo Front, lo elimino de momento
        //this.defaultSeleccionado();
        // Calcular ancho para el input de buscador. Para el nuevo Front, de momento lo elimino
        /*setTimeout(function () {
            that.calcularAnchoInputDisponible();
        }, 200)*/
        return;
    },
    config: function () {
        this.wrapBuscador = $('fieldset', this.buscador);
        this.searchGroup = $(this.cssSearchGroup, this.buscador);
        this.defaultSelect = $('select', this.buscador);
        this.defaultOptions = $('option', this.defaultSelect);
        this.defaultInput = $('input[type=text]', this.buscador);
        this.anchoBuscador = this.buscador.width();
        return;
    },
    marcarDefaultInput: function () {
        this.defaultInput.addClass('defaultText');
        this.defaultInputText = this.defaultInput.attr('value');
        return;
    },
    defaultSeleccionado: function () {
        var that = this;
        this.defaultOptions.each(function (indice) {
            var option = $(this);
            if (option.attr('selected')) that.enlaceSelector.html(that.literales[indice]);
        });
    },
    engancharInput: function () {
        var that = this;
        this.defaultInput.bind('focus', function () {
            var texto = that.defaultInput.val();
            if (texto == '' || texto == that.defaultInputText) {
                that.defaultInput.attr('value', '');
                that.defaultInput.removeClass('defaultText');
            }
        });
        this.defaultInput.bind('blur', function () {
            var texto = that.defaultInput.val();
            if (texto == '' || texto == that.defaultInputText) {
                that.defaultInput.addClass('defaultText');
                that.defaultInput.attr('value', that.defaultInputText);
            }
        });
        return;
    },
    deseleccionar: function () {
        this.defaultOptions.each(function () {
            $(this).removeAttr('selected');
        });
        return;
    },
    engancharOpciones: function () {
        var that = this;
        var enlaces = $('a', this.opciones);
        enlaces.each(function () {
            var enlace = $(this);
            enlace.bind('click', function (evento) {
                evento.preventDefault();
                enlace = $(evento.target);
                var indice = enlace.attr('rel');
                that.enlaceSelector.html(that.literales[indice]);
                that.deseleccionar();
                var opcionSeleccionada = that.defaultOptions[indice];
                $(opcionSeleccionada).attr('selected', 'selected');
                that.defaultSelect[0].selectedIndex = indice;
                that.calcularAnchoInputDisponible();
                if (typeof (window.PreparaAutoCompletarComunidad) == 'function') {
                    PreparaAutoCompletarComunidad();
                }
            });
        })
        return;
    },
    calcularAnchoInputDisponible: function () {
        var anchoBuscador = this.anchoBuscador;
        var anchoSearchGroup = this.anchoSearchGroup;
        var anchoSelector = this.selector.width();
        var anchoDefaultInput = anchoSearchGroup - (anchoSelector + 54);
        // Si el usuario no está en ninguna comunidad, el buscador se quedaba en 0px (No tiene sentido) -> Cambiado por nuevo Front
        //this.defaultInput.css('width', anchoDefaultInput + 'px');
        return;
    },
    engancharSelector: function () {
        var that = this;
        this.selector.hover(function () {
            that.opciones.show();
            that.selector.addClass('desplegado');
            that.calcularAnchoInputDisponible();
        }, function () {
            that.opciones.hide();
            that.selector.removeClass('desplegado');
            that.calcularAnchoInputDisponible();
        });
        return;
    },
    plantillaSelector: function () {
        var html = '<div id=\"selector\">';
        html += '<p class=\"seleccionado\"><a href=\"#\"><\/a><span><\/span><\/p>';
        html += '<div id=\"opciones\">';
        html += '<\/div>';
        html += '<\/div>';
        return html;
    },
    montarSelector: function () {
        this.searchGroup.prepend(this.plantillaSelector());
        this.selector = $('#selector', this.buscador);
        this.enlaceSelector = $('a', this.selector);
        this.opciones = $('#opciones', this.selector);
        return;
    },
    montarOpciones: function () {
        var that = this;
        var options = $('option', this.defaultSelect);
        var html = '<ul>';
        options.each(function (indice) {
            var opcion = $(this);
            var literal = opcion.html();
            that.literales[indice] = literal;
            html += '<li><a href=\"#\" rel=\"' + indice + '\">';
            html += literal;
            html += '<\/li><\/a>';
        });
        if (options.length == 1) {
            this.selector.hide();
        }
        this.opciones.html(html);
        return;
    }
}
$(function () {

    /* buscador */
    buscadorCabecera.init();

	confirmacionEliminacionMultiple.init();
	confirmacionEliminacionSencilla.init();
	
	marcarIdentidadProfesor.init();
	montarModuloOtrasIdentidades.init();
	desplegablesIdentidad.init();
	desplegablesEspacios.init();
	desplegableUsuario.init();
	desplegablesOtrasIdentidades.init();
	desplegablesEspaciosGNOSS.init();
	ampliarContraerListados.init();
	ocultarDesplegables.init();
	hoverDesplegable.init();
	marcarItemsDesplegables.init();
});/**/ 
/*waypoints.min.js*/ 
// Generated by CoffeeScript 1.6.2
/*!
jQuery Waypoints - v2.0.5
Copyright (c) 2011-2014 Caleb Troughton
Licensed under the MIT license.
https://github.com/imakewebthings/jquery-waypoints/blob/master/licenses.txt
*/
(function(){var t=[].indexOf||function(t){for(var e=0,n=this.length;e<n;e++){if(e in this&&this[e]===t)return e}return-1},e=[].slice;(function(t,e){if(typeof define==="function"&&define.amd){return define("waypoints",["jquery"],function(n){return e(n,t)})}else{return e(t.jQuery,t)}})(window,function(n,r){var i,o,l,s,f,u,c,a,h,d,p,y,v,w,g,m;i=n(r);a=t.call(r,"ontouchstart")>=0;s={horizontal:{},vertical:{}};f=1;c={};u="waypoints-context-id";p="resize.waypoints";y="scroll.waypoints";v=1;w="waypoints-waypoint-ids";g="waypoint";m="waypoints";o=function(){function t(t){var e=this;this.$element=t;this.element=t[0];this.didResize=false;this.didScroll=false;this.id="context"+f++;this.oldScroll={x:t.scrollLeft(),y:t.scrollTop()};this.waypoints={horizontal:{},vertical:{}};this.element[u]=this.id;c[this.id]=this;t.bind(y,function(){var t;if(!(e.didScroll||a)){e.didScroll=true;t=function(){e.doScroll();return e.didScroll=false};return r.setTimeout(t,n[m].settings.scrollThrottle)}});t.bind(p,function(){var t;if(!e.didResize){e.didResize=true;t=function(){n[m]("refresh");return e.didResize=false};return r.setTimeout(t,n[m].settings.resizeThrottle)}})}t.prototype.doScroll=function(){var t,e=this;t={horizontal:{newScroll:this.$element.scrollLeft(),oldScroll:this.oldScroll.x,forward:"right",backward:"left"},vertical:{newScroll:this.$element.scrollTop(),oldScroll:this.oldScroll.y,forward:"down",backward:"up"}};if(a&&(!t.vertical.oldScroll||!t.vertical.newScroll)){n[m]("refresh")}n.each(t,function(t,r){var i,o,l;l=[];o=r.newScroll>r.oldScroll;i=o?r.forward:r.backward;n.each(e.waypoints[t],function(t,e){var n,i;if(r.oldScroll<(n=e.offset)&&n<=r.newScroll){return l.push(e)}else if(r.newScroll<(i=e.offset)&&i<=r.oldScroll){return l.push(e)}});l.sort(function(t,e){return t.offset-e.offset});if(!o){l.reverse()}return n.each(l,function(t,e){if(e.options.continuous||t===l.length-1){return e.trigger([i])}})});return this.oldScroll={x:t.horizontal.newScroll,y:t.vertical.newScroll}};t.prototype.refresh=function(){var t,e,r,i=this;r=n.isWindow(this.element);e=this.$element.offset();this.doScroll();t={horizontal:{contextOffset:r?0:e.left,contextScroll:r?0:this.oldScroll.x,contextDimension:this.$element.width(),oldScroll:this.oldScroll.x,forward:"right",backward:"left",offsetProp:"left"},vertical:{contextOffset:r?0:e.top,contextScroll:r?0:this.oldScroll.y,contextDimension:r?n[m]("viewportHeight"):this.$element.height(),oldScroll:this.oldScroll.y,forward:"down",backward:"up",offsetProp:"top"}};return n.each(t,function(t,e){return n.each(i.waypoints[t],function(t,r){var i,o,l,s,f;i=r.options.offset;l=r.offset;o=n.isWindow(r.element)?0:r.$element.offset()[e.offsetProp];if(n.isFunction(i)){i=i.apply(r.element)}else if(typeof i==="string"){i=parseFloat(i);if(r.options.offset.indexOf("%")>-1){i=Math.ceil(e.contextDimension*i/100)}}r.offset=o-e.contextOffset+e.contextScroll-i;if(r.options.onlyOnScroll&&l!=null||!r.enabled){return}if(l!==null&&l<(s=e.oldScroll)&&s<=r.offset){return r.trigger([e.backward])}else if(l!==null&&l>(f=e.oldScroll)&&f>=r.offset){return r.trigger([e.forward])}else if(l===null&&e.oldScroll>=r.offset){return r.trigger([e.forward])}})})};t.prototype.checkEmpty=function(){if(n.isEmptyObject(this.waypoints.horizontal)&&n.isEmptyObject(this.waypoints.vertical)){this.$element.unbind([p,y].join(" "));return delete c[this.id]}};return t}();l=function(){function t(t,e,r){var i,o;if(r.offset==="bottom-in-view"){r.offset=function(){var t;t=n[m]("viewportHeight");if(!n.isWindow(e.element)){t=e.$element.height()}return t-n(this).outerHeight()}}this.$element=t;this.element=t[0];this.axis=r.horizontal?"horizontal":"vertical";this.callback=r.handler;this.context=e;this.enabled=r.enabled;this.id="waypoints"+v++;this.offset=null;this.options=r;e.waypoints[this.axis][this.id]=this;s[this.axis][this.id]=this;i=(o=this.element[w])!=null?o:[];i.push(this.id);this.element[w]=i}t.prototype.trigger=function(t){if(!this.enabled){return}if(this.callback!=null){this.callback.apply(this.element,t)}if(this.options.triggerOnce){return this.destroy()}};t.prototype.disable=function(){return this.enabled=false};t.prototype.enable=function(){this.context.refresh();return this.enabled=true};t.prototype.destroy=function(){delete s[this.axis][this.id];delete this.context.waypoints[this.axis][this.id];return this.context.checkEmpty()};t.getWaypointsByElement=function(t){var e,r;r=t[w];if(!r){return[]}e=n.extend({},s.horizontal,s.vertical);return n.map(r,function(t){return e[t]})};return t}();d={init:function(t,e){var r;e=n.extend({},n.fn[g].defaults,e);if((r=e.handler)==null){e.handler=t}this.each(function(){var t,r,i,s;t=n(this);i=(s=e.context)!=null?s:n.fn[g].defaults.context;if(!n.isWindow(i)){i=t.closest(i)}i=n(i);r=c[i[0][u]];if(!r){r=new o(i)}return new l(t,r,e)});n[m]("refresh");return this},disable:function(){return d._invoke.call(this,"disable")},enable:function(){return d._invoke.call(this,"enable")},destroy:function(){return d._invoke.call(this,"destroy")},prev:function(t,e){return d._traverse.call(this,t,e,function(t,e,n){if(e>0){return t.push(n[e-1])}})},next:function(t,e){return d._traverse.call(this,t,e,function(t,e,n){if(e<n.length-1){return t.push(n[e+1])}})},_traverse:function(t,e,i){var o,l;if(t==null){t="vertical"}if(e==null){e=r}l=h.aggregate(e);o=[];this.each(function(){var e;e=n.inArray(this,l[t]);return i(o,e,l[t])});return this.pushStack(o)},_invoke:function(t){this.each(function(){var e;e=l.getWaypointsByElement(this);return n.each(e,function(e,n){n[t]();return true})});return this}};n.fn[g]=function(){var t,r;r=arguments[0],t=2<=arguments.length?e.call(arguments,1):[];if(d[r]){return d[r].apply(this,t)}else if(n.isFunction(r)){return d.init.apply(this,arguments)}else if(n.isPlainObject(r)){return d.init.apply(this,[null,r])}else if(!r){return n.error("jQuery Waypoints needs a callback function or handler option.")}else{return n.error("The "+r+" method does not exist in jQuery Waypoints.")}};n.fn[g].defaults={context:r,continuous:true,enabled:true,horizontal:false,offset:0,triggerOnce:false};h={refresh:function(){return n.each(c,function(t,e){return e.refresh()})},viewportHeight:function(){var t;return(t=r.innerHeight)!=null?t:i.height()},aggregate:function(t){var e,r,i;e=s;if(t){e=(i=c[n(t)[0][u]])!=null?i.waypoints:void 0}if(!e){return[]}r={horizontal:[],vertical:[]};n.each(r,function(t,i){n.each(e[t],function(t,e){return i.push(e)});i.sort(function(t,e){return t.offset-e.offset});r[t]=n.map(i,function(t){return t.element});return r[t]=n.unique(r[t])});return r},above:function(t){if(t==null){t=r}return h._filter(t,"vertical",function(t,e){return e.offset<=t.oldScroll.y})},below:function(t){if(t==null){t=r}return h._filter(t,"vertical",function(t,e){return e.offset>t.oldScroll.y})},left:function(t){if(t==null){t=r}return h._filter(t,"horizontal",function(t,e){return e.offset<=t.oldScroll.x})},right:function(t){if(t==null){t=r}return h._filter(t,"horizontal",function(t,e){return e.offset>t.oldScroll.x})},enable:function(){return h._invoke("enable")},disable:function(){return h._invoke("disable")},destroy:function(){return h._invoke("destroy")},extendFn:function(t,e){return d[t]=e},_invoke:function(t){var e;e=n.extend({},s.vertical,s.horizontal);return n.each(e,function(e,n){n[t]();return true})},_filter:function(t,e,r){var i,o;i=c[n(t)[0][u]];if(!i){return[]}o=[];n.each(i.waypoints[e],function(t,e){if(r(i,e)){return o.push(e)}});o.sort(function(t,e){return t.offset-e.offset});return n.map(o,function(t){return t.element})}};n[m]=function(){var t,n;n=arguments[0],t=2<=arguments.length?e.call(arguments,1):[];if(h[n]){return h[n].apply(null,t)}else{return h.aggregate.call(null,n)}};n[m].settings={resizeThrottle:100,scrollThrottle:30};return i.on("load.waypoints",function(){return n[m]("refresh")})})}).call(this);/**/ 
/*MVC.FichaDocumento.js*/ 
function DesplegarPanelMVC(pPanelID) {
    var panel = $('#' + pPanelID);

    panel.children().css("display", "block");
    panel.css("display", "block");

    panel.children().children('#loading').css("display", "none");
    panel.children().children('#menssages').css("display", "none");
    panel.children().children('#action').css("display", "block");
}

function DesplegarAccionConPanelIDMVC(pPanelCopiar, pBoton, pPanelID) {
    var panel = $('#' + pPanelID);

    panel.children().children('#loading').css("display", "block");
    panel.children().children('#menssages').css("display", "none");
    panel.children().children('#action').css("display", "none");

    panel.children().css("display", "block");
    panel.css("display", "block");

    if (pBoton != null && typeof (pBoton.parentNode) != 'undefined') {
        DesactivarBotonesActivosDespl();
        pBoton.parentNode.className += ' active';
    }

    panel.children().children('#action').html('').append($('#' + pPanelCopiar).clone());
    panel.children().children('#loading').css("display", "none");
    panel.children().children('#menssages').css("display", "none");
    panel.children().children('#action').css("display", "block");
    
}

// Desplegar y mostrar la vista o contenido devuelto de una petición vía urlAccion para ser mostrado en el panel pPanelID
/**
 * 
 * @param urlAccion: URL de la petición que será pasada al controller para que este devuelva datos (Puede ser una vista y datos que se controlar�n en el modelo de la p�gina)
 * @param {any} pBoton: Botón que ha desplegado la acción.
 * @param {any} pPanelID: ID del panel donde se devolverá ese código HTML devuelto por la petición mandada en el parámetro urlAccion
 * @param {any} pArg: Argumentos adicionales
 */
function DeployActionInModalPanel(urlAccion, pBoton, pPanelID, pArg) {    
    // Panel principal (padre) donde se mostrar�n todos los paneles
    var panel = $('#' + pPanelID);    
    // Panel donde se mostrar� el contenido
    var panelContent = panel.children('#content');
    // Panel de mensajes de OK/KO del contenedor padre (Posible error en la carga del servidor)
    var panelMessagesResult = panel.children().children('#resource-message-results');    
    // Ocultado mensaje de errores por defecto
    panelMessagesResult.css("display", "none");

    // Realizar la petici�n AJAX
    GnossPeticionAjax(urlAccion, null, true).done(function (data) {        
        panelContent.html(data);        
        // Ocultar panel de mensajes mensajes
        panelMessagesResult.css("display", "none");       
        panelContent.css("display", "block");
        // Llamar a inicializar las DataTable dentro del modal para acci�n "Historico" en Recurso
        accionHistorial.montarTabla();
        // Llamar a inicializar los despliegues para acci�n "Categorizar" en Recurso        
        accionDesplegarCategorias.init();

        // Recargar CKEditor si hubiera alg�n Input con clase de CKEditor
        if ($(panelContent).find('.cke').length > 0) RecargarTodosCKEditor(); 

    }).fail(function (data) {
        if (data == 'invitado') {
            operativaLoginEmergente.init();
            OcultarUpdateProgress();
            CerrarPanelAccion(pPanelID);
        } else {            
            // Mostrar mensaje de error
            panelMessagesResult.css("display", "block");
            panelContent.css("display", "none");
            panelMessagesResult.children(".ok").css("display", "none");
            panelMessagesResult.children(".ok").css("display", "block");
            panelMessagesResult.children(".ok").html('error');            
        }
    });
}


// Resetear el contenido del contenedor Modal para que la informaci�n no est� visible y tenga que volver a cargarse de nuevo.
// El contenedor #modal-container es utilizado para albergar modales de un Recurso de tal manera que pueda reutilizarse cada vez que este se cierra.
// Ej: En ficha de recurso, el modal "Historico" se carga de forma din�mica. Cuando se cierre el contenedor padre, habr�a que quitar el contenido para volver a ser reutilizado
var resetModalContainer = {
    // Inicializar el comportamiento cuando la p�gina web est� cargada
    init: function () {        
        $("#modal-container").on("hidden.bs.modal", function () {            
            // Panel que hay que resetear/rellenar con el initialContainerContent
            var panelToReset = $(this).find("#content");
            // HTML que cargaremos de nuevo una vez se cierre el formulario (resetearlo de inicio)
            var initialContainerContent = '';
            initialContainerContent += '<div class="modal-header">';
            initialContainerContent += '<p class="modal-title">';
            initialContainerContent += '<span class="spinner-border white-color mr-2"></span>';
            initialContainerContent += 'Cargando ...';
            initialContainerContent += '</p>';
            initialContainerContent += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
            initialContainerContent += '</div>';
            initialContainerContent += '<div class="modal-body"></div>';
            initialContainerContent += '<div id="resource-message-results" class="modal-footer">';
            initialContainerContent += '<div class="ok"></div>';
            initialContainerContent += '<div class="ko"></div>';
            initialContainerContent += '</div>';
            // Vuelvo a incluir el panel inicial para ser reutilizado (ocultar el contenido previo si se abri� antes el modal)
            panelToReset.html(initialContainerContent).fadeIn();
        });
        return;
    },

       
    // Vaciar el contenedor de un modal y dejarlo como "loading" hasta que este vuelva a llenarse con datos v�a API REST (Ficha Recurso: Eliminar - Eliminar Selectivo)
    // Sirve para volver a llenar un modal sin que este sea cerrado.
    resetModalContent: function () {
        // Contenedor padre de los modales
        var $modalContainer = $("#modal-container");
        // Panel donde se vaciar� el contenido actual para emular la carga (Loading)
        var panelToReset = $modalContainer.find("#content");
        // HTML que cargaremos de nuevo una vez se cierre el formulario (resetearlo de inicio)
        var initialContainerContent = '';
        initialContainerContent += '<div class="modal-header">';
        initialContainerContent += '<p class="modal-title">';
        initialContainerContent += '<span class="spinner-border white-color mr-2"></span>';
        initialContainerContent += 'Cargando ...';
        initialContainerContent += '</p>';
        initialContainerContent += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
        initialContainerContent += '</div>';
        initialContainerContent += '<div class="modal-body"></div>';
        initialContainerContent += '<div id="resource-message-results" class="modal-footer">';
        initialContainerContent += '<div class="ok"></div>';
        initialContainerContent += '<div class="ko"></div>';
        initialContainerContent += '</div>';
        // Vuelvo a incluir el panel inicial para ser reutilizado (ocultar el contenido previo si se abri� antes el modal)
        panelToReset.html(initialContainerContent);       
    },
};

// Activar al inicio de carga de la página
$(function () {
    // Inicialización de reseteo de contenedor de modales (Ficha Recurso)
    resetModalContainer.init();
    // Activacin de tooltips de bootstrap para que estén disponibles en toda la página
    $('[data-toggle="tooltip"]').tooltip();
});


function DesplegarAccionMVC(urlAccion, pBoton, pPanelID, pArg) {
    var panel = $('#' + pPanelID);

    panel.children().children('#loading').css("display", "block");
    panel.children().children('#menssages').css("display", "none");
    panel.children().children('#action').css("display", "none");

    panel.children().css("display", "block");
    panel.css("display", "block");

    if (pBoton != null && typeof (pBoton.parentNode) != 'undefined') {
        DesactivarBotonesActivosDespl();
        pBoton.parentNode.className += ' active';
    }

    GnossPeticionAjax(urlAccion, null, true).done(function (data) {
        panel.children().children('#action').html(data);
        panel.children().children('#loading').css("display", "none");
        panel.children().children('#menssages').css("display", "none");
        panel.children().children('#action').css("display", "block");
    }).fail(function (data) {
        if (data == 'invitado') {
            operativaLoginEmergente.init();
            OcultarUpdateProgress();
            CerrarPanelAccion(pPanelID);
        } else {
            panel.children().children('#loading').css("display", "none");
            panel.children().children('#menssages').css("display", "block");
            panel.children().children('#action').css("display", "none");

            panel.children().children('#menssages').children(".ok").css("display", "none");
            panel.children().children('#menssages').children(".ko").css("display", "block");
            panel.children().children('#menssages').children(".ko").html('ERROR');
        }
    });
}

/**
 * Acción para desplegar mensaje informativo una vez realizada una acción
 * @param {any} pPanelID: Panel id donde se ha de mostrar el mensaje informativo (antes de toastr)
 * @param {any} pOk: Valor booleano que indica si la acción ha sido OK o no.
 * @param {any} pHtml: Mensaje informativo que se mostrará al usuario.
 * @param {any} timeOut: Tiempo de espera hasta que se muestra la notificación correcta. (milisegundos)
 */
function DesplegarResultadoAccionMVC(pPanelID, pOk, pHtml, timeOut = 1500) {
    var panel = $('#' + pPanelID);

    if (pHtml != null && pHtml != '' && pHtml.indexOf('<p') != 0) {
        pHtml = '<p>' + pHtml + '</p>';
    }

    if (pOk) {
        /* Cambiado por nuevo Front -> Toastr
        panel.children().children('#menssages').children(".ok").css("display", "block");
        panel.children().children('#menssages').children(".ko").css("display", "none");

        panel.children().children('#menssages').children(".ok").html(pHtml);
        */

        // Hacer desaparecer el modal si la acción es correcta
        $('#modal-container').modal('hide');

        // Mostrar mensaje OK
        setTimeout(() => {
            mostrarNotificacion('success', pHtml);
        }, timeOut)
    }
    else {
        /* Cambiado por nuevo Front -> Toastr
        panel.children().children('#menssages').children(".ok").css("display", "none");
        panel.children().children('#menssages').children(".ko").css("display", "block");

        panel.children().children('#menssages').children(".ko").html(pHtml);
        */
        mostrarNotificacion('error', pHtml);
    }

    /* Cambiado por nuevo Front -> Toastr    
    panel.children().children('#loading').css("display", "none");
    panel.children().children('#menssages').css("display", "block");
    panel.children().children('#action').css("display", "none");

    panel.children().css("display", "block");
    panel.css("display", "block");
    */

    DesactivarBotonesActivosDespl();
}

function AccionRecurso_PintarVotos(data) {
    $('#panUtils1 .votos').remove();
    $('#panUtils1 .votosPositivos').remove();
    $('#panUtils1 .panelVotosSimple').remove();
    $('#panUtils1 .panelVotosAmpliado').remove();
    $('#panUtils1 .visitas').after(data);
    presentacionVotosRecurso.init();
}

function CambiarTextoPorTextoAux(that) {
    var texto = $(that).text().trim();
    var textoAux = $(that).attr('textoAux');

    $(that).html($(that).html().replace(texto, textoAux));
    $(that).attr('textoAux', texto);
}

function CambiarOnClickPorOnclickAux(that) {
    var onclick = $(that).attr('onclick');
    var onclickAux = $(that).attr('onclickAux');

    $(that).attr('onclick', onclickAux);
    $(that).attr('onclickAux', onclick);
}

/**
 * Acción de realizar voto positivo de un recurso. Realización de una forma más visual.
 * - Aparecerá un pequeño "Loading" durante la acción del voto
 * - Actualizará el num de votos cuando finalice la acción
 * - Estará disponible la acción inversa cuando finalice el voto realizado.
 * @param {any} that: El botón pulsado (Span) con el icono de thumbs_up_alt
 * @param {any} urlVotarRecurso: La URL para realizar el voto
 * @param {any} urlVotarRecursoInvertido: La URL para realizar el voto contrario/invertido
 */
function AccionRecurso_VotarPositivo(that, urlVotarRecurso, urlVotarRecursoInvertido) {
    $(that).parent().find('.eleccionUsuario').removeClass("eleccionUsuario");
    $(that).addClass("eleccionUsuario");

    // Par�metros
    var funcionVotarInvertido = "AccionRecurso_VotarEliminar(this, '" + urlVotarRecursoInvertido + "', '" + urlVotarRecurso + "')";
    var iconoVoto = "thumb_up_alt";
    //var iconoVotoInvertido = "thumb_down_alt";
    // Ser� el mismo icono pero cambiado el color
    var iconoVotoInvertido = iconoVoto;
    var nombreClaseVotoOK = "activo"
    var claseMaterialIcons = "material-icons";
    var $numVotos = $(that).parent().find(".number");
    var numVotosActual = parseInt($numVotos.text());

    // Clase de tipo "Loading"
    var loadingClass = "spinner-border spinner-border-sm texto-primario"       

    // Ocultar icono del voto actual para mostrar solo el "Loading"
    $(that).html("");    
    // Elimino la clase de material icons para poder cambiar el color
    $(that).removeClass(claseMaterialIcons);    
    // Muestrar loading hasta que se complete la petici�n de "Votar"
    $(that).addClass(loadingClass);

    if (urlVotarRecurso != "") {
        GnossPeticionAjax(urlVotarRecurso, null, true).done(function (data) {
            AccionRecurso_PintarVotos(data);
            EnviarAccGogAnac('Acciones sociales', 'Votar', urlVotarRecurso);
            if (typeof voteAnalitics != 'undefined') {
                voteAnalitics.voteResource(1);
            }
            // Cambiar icono a Voto negativo        
            $(that).html(iconoVotoInvertido);
            // Cambiar la funci�n a realizar a Voto negativo
            $(that).attr("onclick", funcionVotarInvertido);
            // A�ado de nuevo la clase de material icons
            $(that).addClass(claseMaterialIcons);
            // A�ado la clase de megusta directamente al padre
            $(that).parent().toggleClass(nombreClaseVotoOK);
            $(that).removeClass(loadingClass);
            // Cambiar el n�mero del voto realizado a +1
            numVotosActual += 1;
            $numVotos.html(numVotosActual);
        }).fail(function (data) {
            // A�ado de nuevo la clase de material icons
            $(that).addClass(claseMaterialIcons);
            // Cambiar el loading y volver a como estaba antes (original) del error        
            $(that).html(iconoVoto);
            $(that).removeClass(loadingClass);
            if (data == "invitado") { operativaLoginEmergente.init(); }
        });
    } else {
        // A�ado de nuevo la clase de material icons
        $(that).addClass(claseMaterialIcons);
        // Cambiar el loading y volver a como estaba antes (original) del error        
        $(that).html(iconoVoto);
        $(that).removeClass(loadingClass);
        operativaLoginEmergente.init();
    }
}

function AccionRecurso_VotarPositivoListado(that, urlVotarRecurso) {
    if ($(that).parents('.acciones').find('.acc_nomegusta').hasClass('active')) {
        var botonNoMeGusta = $(that).parents('.acciones').find('.acc_nomegusta a')[0];
        CambiarTextoPorTextoAux(botonNoMeGusta);
        $(botonNoMeGusta).parent().removeClass('active')
        CambiarOnClickPorOnclickAux($(botonNoMeGusta));
    }

    CambiarTextoPorTextoAux(that);
    $(that).parent().addClass("active");
    CambiarOnClickPorOnclickAux($(that));

    GnossPeticionAjax(urlVotarRecurso, null, true).done(function (data) {
        EnviarAccGogAnac('Acciones sociales', 'Votar', urlVotarRecurso);
        if (typeof voteAnalitics != 'undefined') {
            voteAnalitics.voteResource(1);
        }
    }).fail(function (data) {
        if (data == "invitado") { operativaLoginEmergente.init(); }
    });
}


function AccionRecurso_VotarNegativo(that, urlVotarRecurso) {
    $(that).parent().find('.eleccionUsuario').removeClass("eleccionUsuario");
    $(that).addClass("eleccionUsuario");

    GnossPeticionAjax(urlVotarRecurso, null, true).done(function (data) {
        AccionRecurso_PintarVotos(data);
        EnviarAccGogAnac('Acciones sociales', 'Votar Negativamente', urlVotarRecurso);
        if (typeof voteAnalitics != 'undefined') {
            voteAnalitics.voteResource(-1);
        }
    }).fail(function (data) {
        if (data == 'invitado') { operativaLoginEmergente.init(); }
    });
}


function AccionRecurso_VotarNegativoListado(that, urlVotarRecurso) {
    
    if ($(that).parents('.acciones').find('.acc_megusta').hasClass('active')) {
        var botonMeGusta = $(that).parents('.acciones').find('.acc_megusta a')[0];
        CambiarTextoPorTextoAux(botonMeGusta);
        $(botonMeGusta).parent().removeClass('active')
        CambiarOnClickPorOnclickAux($(botonMeGusta));
    }

    CambiarTextoPorTextoAux(that);
    $(that).parent().addClass("active");
    CambiarOnClickPorOnclickAux($(that));

    GnossPeticionAjax(urlVotarRecurso, null, true).done(function (data) {
        EnviarAccGogAnac('Acciones sociales', 'Votar Negativamente', urlVotarRecurso);
        if (typeof voteAnalitics != 'undefined') {
            voteAnalitics.voteResource(-1);
        }
    }).fail(function (data) {
        if (data == 'invitado') { operativaLoginEmergente.init(); }
    });
}

/**
 * Acci�n de realizar voto negativo de un recurso. Realizaci�n de una forma m�s visual.
 * - Aparecer� un peque�o "Loading" durante la acci�n del voto
 * - Actualizar� el num de votos cuando finalice la acci�n
 * - Estar� disponible la acci�n inversa cuando finalice el voto realizado.
 * @param {any} that: El bot�n pulsado (Span) con el icono de thumbs_up_alt
 * @param {any} urlVotarRecurso: La URL para realizar el voto
 * @param {any} urlVotarRecursoInvertido: La URL para realizar el voto contrario/invertido
 */

function AccionRecurso_VotarEliminar(that, urlVotarRecurso, urlVotarRecursoInvertido) {
    $(that).parent().find('.eleccionUsuario').removeClass("eleccionUsuario");

    // Par�metros
    var funcionVotarInvertido = "AccionRecurso_VotarPositivo(this, '" + urlVotarRecursoInvertido + "', '" + urlVotarRecurso + "')";
    var iconoVoto = "thumb_up_alt";
    //var iconoVotoInvertido = "thumb_up_alt";
    // Ser� el mismo icono pero cambiado el color
    var iconoVotoInvertido = iconoVoto;
    var nombreClaseVotoOK = "activo"
    var claseMaterialIcons = "material-icons";
    var $numVotos = $(that).parent().find(".number");
    var numVotosActual = parseInt($numVotos.text());

    // Clase de tipo "Loading"
    var loadingClass = "spinner-border spinner-border-sm texto-primario"

    // Ocultar icono del voto actual para mostrar solo el "Loading"
    $(that).html("");
    // Elimino la clase de material icons para poder cambiar el color
    $(that).removeClass(claseMaterialIcons);
    // Muestrar loading hasta que se complete la petici�n de "Votar"
    $(that).addClass(loadingClass);


    if (urlVotarRecurso != "") {
        GnossPeticionAjax(urlVotarRecurso, null, true).done(function (data) {
            AccionRecurso_PintarVotos(data);
            if (typeof voteAnalitics != 'undefined') {
                voteAnalitics.voteResource(0);
            }
            // Cambiar icono a Voto negativo        
            $(that).html(iconoVotoInvertido);
            // Cambiar la funci�n a realizar a Voto negativo
            $(that).attr("onclick", funcionVotarInvertido);
            // A�ado de nuevo la clase de material icons
            $(that).addClass(claseMaterialIcons);
            $(that).removeClass(loadingClass);
            // A�adimos la clase para el color a voto realizado (Like)
            $(that).parent().toggleClass(nombreClaseVotoOK);
            // Cambiar el n�mero del voto realizado a +1
            numVotosActual -= 1;
            $numVotos.html(numVotosActual);
        }).fail(function (data) {
            // A�ado de nuevo la clase de material icons
            $(that).addClass(claseMaterialIcons);
            // Cambiar el loading y volver a como estaba antes (original) del error        
            $(that).html(iconoVoto);
            $(that).removeClass(loadingClass);
            var error = data.substr(data.indexOf('.') + 1);
            if (error == 'Invitado') { operativaLoginEmergente.init(); }
        });
    } else {
        // A�ado de nuevo la clase de material icons
        $(that).addClass(claseMaterialIcons);
        // Cambiar el loading y volver a como estaba antes (original) del error        
        $(that).html(iconoVoto);
        $(that).removeClass(loadingClass);
        operativaLoginEmergente.init();
    }
}


function AccionRecurso_VotarEliminarListado(that, urlVotarRecurso) {
    if ($(that).parents('.acciones').find('.acc_megusta').hasClass('active')) {
        CambiarTextoPorTextoAux($(that).parents('.acciones').find('.acc_megusta a')[0]);
        $(that).parents('.acciones').find('.acc_megusta').removeClass('active')
    }
    if ($(that).parents('.acciones').find('.acc_nomegusta').hasClass('active')) {
        CambiarTextoPorTextoAux($(that).parents('.acciones').find('.acc_nomegusta a')[0]);
        $(that).parents('.acciones').find('.acc_nomegusta').removeClass('active')
    }

    CambiarOnClickPorOnclickAux($(that));

    GnossPeticionAjax(urlVotarRecurso, null, true).done(function (data) {
        if (typeof voteAnalitics != 'undefined') {
            voteAnalitics.voteResource(0);
        }
    }).fail(function (data) {
        var error = data.substr(data.indexOf('.') + 1);
        if (error == 'Invitado') { operativaLoginEmergente.init(); }
    });
}

function AccionRecurso_EnviarNewsletter_Aceptar(idioma, urlEnviarNewsletter, documentoID) {
    MostrarUpdateProgress();

    var dataPost = {
        Language: idioma
    }
    
    GnossPeticionAjax(urlEnviarNewsletter, dataPost, true).done(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, data);
        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
        OcultarUpdateProgress();
    });
}

function AccionRecurso_EnviarNewsletterGrupos_Aceptar(idioma, urlEnviarNewsletterGrupos, documentoID, listaGrupos) {
    MostrarUpdateProgress();

    if (listaGrupos.indexOf('&') == 0) {
        listaGrupos = listaGrupos.substr(1);
    }

    var dataPost = {
        Language: idioma,
        Groups: listaGrupos.split('&')
    }

    GnossPeticionAjax(urlEnviarNewsletterGrupos, dataPost, true).done(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, data);
        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
        OcultarUpdateProgress();
    });
}

function AccionRecurso_RestaurarVersion_Aceptar(urlRestaurar, documentoID) {
    MostrarUpdateProgress();
    
    GnossPeticionAjax(urlRestaurar, null, true).done(function (data) {
        window.location.href = data;
    }).fail(function (data) {
        var html = textoRecursos.restaurarFALLO;
        if (data != "") {
            html = data;
            html = html.substr(html.indexOf('|') + 1);
        }
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, html);
        OcultarUpdateProgress();
    });
}

function AccionRecurso_Vincular_Aceptar(urlVincularRecurso, urlCargarVinculados, documentoID) {
    MostrarUpdateProgress();
    var urlDocVinculado = $("#txtUrlDocVinculado_" + documentoID).val();

    var datosPost = {
        UrlResourceLink: urlDocVinculado
    }
    GnossPeticionAjax(urlVincularRecurso, datosPost, true).done(function (data) {
        EnviarAccGogAnac('Acciones sociales', '"Vincular recurso', urlVincularRecurso);
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, textoRecursos.recursoVinculadoOK);
        OcultarUpdateProgress();
        Vinculados_CargarVinculados(urlCargarVinculados);
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
        OcultarUpdateProgress();
    });
}

/**
 * Mostrar el mensaje correcto de recurso desvinculado. 
 * Seguidamente, realiza la petici�n para cargar los nuevos recursos vinculados.
 * @param {any} urlDesvincularRecurso
 * @param {any} urlCargarVinculados
 * @param {any} documentoID
 * @param {any} documentoDesVincID
 */
function AccionRecurso_DesVincular_Aceptar(urlDesvincularRecurso, urlCargarVinculados, documentoID, documentoDesVincID) {
    MostrarUpdateProgress();

    var datosPost = {
        ResourceUnLinkKey: documentoDesVincID
    }

    GnossPeticionAjax(urlDesvincularRecurso, datosPost, true).done(function () {
        // Cambiado por nuevo Front
        //DesplegarResultadoAccionMVC("despAccionRec_" + documentoDesVincID, true, textoRecursos.DesvincularOK);
        DesplegarResultadoAccionMVC("modal-dinamic-action-response", true, textoRecursos.DesvincularOK);
        // Esperar 1,5 segundos y ocultar el panel
        setTimeout(() => {
            $('#modal-container').modal('hide');
        }, 1500);
        OcultarUpdateProgress();      
        Vinculados_CargarVinculados(urlCargarVinculados);
    }).fail(function () {
        // Cambiado por nuevo Front
        //DesplegarResultadoAccionMVC("despAccionRec_" + documentoDesVincID, false, textoRecursos.DesvincularFALLO);
        DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, textoRecursos.DesvincularFALLO);
        OcultarUpdateProgress();
    });
}

function Vinculados_CargarVinculados(urlCargarVinculados) {
    GnossPeticionAjax(urlCargarVinculados, null, true).done(function (data) {
        $('#panVinculadosInt').html(data);
        CompletadaCargaContextos();
    });
}

/**
 * Actualizar la acci�n para poder certificar un recurso. Debido al cambio del Front, ahora no se hace mediante "Select" sino con Radio
 * @param {any} urlPaginaCertificar: Url para realizar la acci�n de certificar un recurso.
 * @param {any} documentoID: Documento ID o identificador del recurso.
 * @param {any} textoCertificado: Parece ser simplemente el texto de "certificaci�n".
 */
function AccionRecurso_Certificar_Aceptar(urlPaginaCertificar, documentoID, textoCertificado) {
    MostrarUpdateProgress();
    // Por cambio en el Front
    // var comboCertificado = $("#comboCertificado_" + documentoID);
    // var valorSeleccionado = comboCertificado.find("option:selected").text();   

    // Cogemos el atributo "data-value" que ser� el valor de la label elegida
    var valorSeleccionado = $('input[name=certificar-recurso]:checked').attr("data-value");
    // Cogemos id o value (no el texto en bruto) de la opci�n seleccionada en el radio button.
    var opcionSeleccionada = $('input[name=certificar-recurso]:checked').attr("data-option");
    
    var dataPost = {
        //CertificationID: comboCertificado.val()
        CertificationID: opcionSeleccionada
    }
    
    GnossPeticionAjax(urlPaginaCertificar, dataPost, true).done(function (data) {
        if ($('#panUtils1').length > 0) {
            $('#contCertificado').remove();
            if (comboCertificado.val() != "00000000-0000-0000-0000-000000000000") {
                $('#panUtils1').append("<div id=\"contCertificado\"><p class=\"certificado\"><span class=\"icono\"></span>" + textoCertificado + ":<strong>" + valorSeleccionado + "</strong></p></div>");
            }
        }
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, textoRecursos.certificacionOK);
        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, textoRecursos.certificacionFALLO);
        OcultarUpdateProgress();
    });
}

function AccionRecurso_Eliminar_Aceptar(urlEliminarDocumento, documentoID) {
    MostrarUpdateProgress();

    if (typeof resourceAnalitics != 'undefined') {
        resourceAnalitics.resourceDeleted();
    }

    GnossPeticionAjax(urlEliminarDocumento, null, true).done(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, data);
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
    });
}

function AccionRecurso_EliminarSelectivo_Aceptar(urlEliminarDocumento, documentoid) {
    MostrarUpdateProgress();

    var div = $("#eliminarSelectivo_" + documentoid);

    var i = 0;
    var lista = {};
    div.parent().find('input:checked').each(function () {
        lista[i] = this.id;
        i++;
    });

    var dataPost = {
        SharedCommunities: lista
    }

    if (typeof resourceAnalitics != 'undefined') {
        resourceAnalitics.resourceDeleted();
    }

    GnossPeticionAjax(urlEliminarDocumento, dataPost, true);
}

function BloquearComentarios(btnBloquear, textDesbloquear, urlBloquearComentarios, documentoID) {
    MostrarUpdateProgress();

    //var dataPost = {
    //    callback: 'acciones|AccionRecurso_Comentarios_Bloquear'
    //}
    GnossPeticionAjax(urlBloquearComentarios, null, true).done(function (data) {
        var html = textoRecursos.comentBloqOK;

        var accion = $(btnBloquear).attr('onclick');
        accion = accion.replace("BloquearComentarios(", "DesbloquearComentarios(");
        accion = accion.replace("/lock-comments", "/unlock-comments");
        accion = accion.replace(textDesbloquear, $(btnBloquear).text());
        $(btnBloquear).attr('onclick', accion);

        $(btnBloquear).html($(btnBloquear).html().replace($(btnBloquear).text(), textDesbloquear));
        
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, html);

        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);

        OcultarUpdateProgress();
    });
}

function DesbloquearComentarios(btnDesbloquear, textBloquear, urlDesbloquearComentarios, documentoID) {
    MostrarUpdateProgress();

    //var dataPost = {
    //    callback: 'acciones|AccionRecurso_Comentarios_DesBloquear'
    //}

    //$.post(urlDesbloquearComentarios, dataPost, function (data) {
    GnossPeticionAjax(urlDesbloquearComentarios, null, true).done(function (data) {
        var html = textoRecursos.comentDesBloqOK;

        var accion = $(btnDesbloquear).attr('onclick');
        accion = accion.replace("DesbloquearComentarios(", "BloquearComentarios(");
        accion = accion.replace("/unlock-comments", "/lock-comments");
        accion = accion.replace(textBloquear, $(btnDesbloquear).text());
        $(btnDesbloquear).attr('onclick', accion);

        $(btnDesbloquear).html($(btnDesbloquear).html().replace($(btnDesbloquear).text(), textBloquear));
        
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, html);

        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);

        OcultarUpdateProgress();
    });
}

function CrearVersionDocSem(urlPagina, documentoID) {
    MostrarUpdateProgress();

    $.post(urlPagina + '/create-version-semantic-document', function (data) {
        var html = "";
        var ok = true;
        if (data.indexOf("OK") != 0) {
            ok = false;
            html = data;
            html = html.substr(html.indexOf('|') + 1);
            DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, ok, html);

            OcultarUpdateProgress();
        }
        else {
            window.location = data.substr(data.indexOf('|') + 1);
        }
    });
}


function AccionRecurso_ReportPage_Aceptar(urlReportPage, documentoID) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);
    var mensaje = $(".reportPage textarea", panelDespl).val();

    var dataPost = {
        message: mensaje
    }

    GnossPeticionAjax(urlReportPage, dataPost, true).done(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, data);
    }).fail(function (error) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, error);
    }).always(function () {
        OcultarUpdateProgress();
    });
}

function AccionRecurso_AgregarTags_Aceptar(urlAgregarTags, documentoID, tags, permisoEditarTags, urlBaseTags) {
    MostrarUpdateProgress();

    var listaTags = tags.split(',');

    var lista = {};
    for (var i = 0; i < listaTags.length; i++) {
        if (listaTags[i].trim() != "") {
            lista[i] = listaTags[i];
        }
    }

    var dataPost = {
        Tags: lista
    }
    
    GnossPeticionAjax(urlAgregarTags, dataPost, true).done(function () {
        var html = textoRecursos.tagsOK;

        EnviarAccGogAnac('Acciones sociales', 'Añadir etiquetas', urlAgregarTags);

        var htmlTags = "";
        var separador = ", ";
        if (permisoEditarTags) {
            separador = "";
        }
        $.each(tags.split(','), function () {
            tag = this.trim();
            if (tag != "") {
                var enlaceCompleto = urlBaseTags + "/" + tag;
                htmlTags += "<li>" + separador + "<a href=\"" + enlaceCompleto + "\" rel=\"sioc:topic\" resource=\"" + enlaceCompleto + "\"><span typeof=\"sioc_t:Tag\" property=\"dcterms:name\" about=\"" + enlaceCompleto + "\">" + tag + "</span></a></li>";
                separador = ", ";
            }
        });
        
        // Cambio por nuevo Front
        //var ul = $('#despAccionRec_' + documentoID).parents('.wrapDescription').children('.group.etiquetas').find('ul');
        const ul = $('.group.etiquetas').find('.listTags');
        var ultimoLI = ul.children().last();

        if (ultimoLI.find('a').hasClass("verTodasCategoriasEtiquetas")) {
            if (permisoEditarTags) {
                ul.children().each(function () {
                    if (!$(this).find('a').hasClass("verTodasCategoriasEtiquetas")) {
                        $(this).remove();
                    }
                });
            }
            ultimoLI.before(htmlTags);
            
            if (ultimoLI.find('a').hasClass('mas')) {
                ultimoLI.find('a').click();
            }
        }
        else {
            if (permisoEditarTags) {
                ul.html(htmlTags);
            }
            else {
                ultimoLI.after(htmlTags);
            }
        }
        
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, html);

        OcultarUpdateProgress();
    }).fail(function (data) {
        var html = textoRecursos.tagsFALLO;
        var numError = data;
        if (numError == "0") {
            html = form.errordtag;
        }

        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, html);

        OcultarUpdateProgress();
    });
}

function AccionRecurso_AgregarCategorias_Aceptar(urlAgregarCategorias, documentoID, categorias, permisoEditarCategorias, urlBaseCategorias) {
    MostrarUpdateProgress();

    var listaCategorias = categorias.split(',');

    var lista = {};
    for (var i = 0; i < listaCategorias.length; i++) {
        if (listaCategorias[i].trim() != "") {
            lista[i] = listaCategorias[i];
        }
    }

    var dataPost = {
        Categories: lista
    }
    
    GnossPeticionAjax(urlAgregarCategorias, dataPost, true).done(function () {
        var html = textoRecursos.categoriasOK;

        EnviarAccGogAnac('Acciones sociales', 'Añadir recurso a categoría', urlAgregarCategorias);

        var htmlCategorias = "";
        var separador = "";
        /*if (permisoEditarCategorias) {
            separador = "";
        }*/
        $.each(categorias.split(','), function () {
            categoriaID = this.trim();
            if (categoriaID != "") {
                var nombreCat = $('#despAccionRec_' + documentoID).find('.divCategorias').find('.' + categoriaID).find('label').text();
                var enlaceCompleto = urlBaseCategorias + "/" + nombreCat.toLowerCase().replace(' ', '-') + "/" + categoriaID;
                htmlCategorias += "<li>" + separador + "<a href=\"" + enlaceCompleto + "\"><span>" + nombreCat + "</span></a></li>";
                separador = ", ";
            }
        });

        //Cambio por nuevo Front
        //var ul = $('#despAccionRec_' + documentoID).parents('.wrapDescription').children('.group.categorias').find('ul');
        const ul = $('.group.categorias').find('.listCat');
        var ultimoLI = ul.children().last();
           
        if (ultimoLI.find('a').hasClass("verTodasCategoriasEtiquetas")) {
            //if (permisoEditarCategorias) {
                ul.children().each(function () {
                    if (!$(this).find('a').hasClass("verTodasCategoriasEtiquetas")) {
                        $(this).remove();
                    }
                });
            //}
            ultimoLI.before(htmlCategorias);

            if (ultimoLI.find('a').hasClass('mas')) {
                ultimoLI.find('a').click();
            }
        }
        else {
            /*if (permisoEditarCategorias) {*/
                ul.html(htmlCategorias);
            /*}
            else {
                ultimoLI.after(htmlCategorias);
            }*/
        }
        
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, html);

        OcultarUpdateProgress();
    }).fail(function (data) {
        var html = textoRecursos.categoriasFALLO;
        var numError = data;
        if (numError == "0") {
            html = form.errordcategoriaSelect;
        }

        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, html);

        OcultarUpdateProgress();
    });
}

/**
 * Método que se ejecuta desde la acción de "Compartir" para actualizar las comunidades existentes y así poder compartir el recurso donde se deseee.
 * @param {any} urlAccionCambiarCompartir: Llamada URL para proceder a realizar la acción de Compartir-Cambiar y así actualizar la lista
 * @param documentoID: El ID del documento o del recurso en cuestión
 * @param {any} personaID: El id de la persona que ha realizado la acción
 * @param {any} pListaUrlsAutocompletar
 */
function AccionRecurso_Compartir_Cambiar(urlAccionCambiarCompartir, documentoID, personaID, pListaUrlsAutocompletar) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);
    var txtBaseRecursos = panelDespl.find('#ddlCompartir option:selected');

    var dataPost = {
        SelectedBR: txtBaseRecursos.val()
    }

    GnossPeticionAjax(urlAccionCambiarCompartir, dataPost, true).done(function (data) {
        var html = "";
        for (var i in data) {
            html += data[i].html;
        }

        // Actualizarlo por el nuevo Front
        // panelDespl.find('#divSelCatTesauro').html(html);
        //panelDespl.find('.divCategorias').html(html);        
        panelDespl.find('#panCategoriasTesauroArbol').html(html);
        // Llamar a inicializar los despliegues para acci�n "Compartir" en Recurso        
        accionDesplegarCategorias.init()               

        panelDespl.find('#txtSeleccionados').val('');

        var liBaseRecursos = panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar #' + txtBaseRecursos.val());
        if (liBaseRecursos.length > 0)
        {
            var catSeleccionadas = liBaseRecursos.find('input').val().split('$$')[1]
            panelDespl.find('#txtSeleccionados').val(catSeleccionadas);
            $.each(catSeleccionadas.split(','), function () {
                if (this != "") {
                    panelDespl.find('#divSelCatTesauro').find('span.' + this + ' input').prop('checked', true);
                    var imgVerMas = panelDespl.find('#divSelCatTesauro').find('span.' + this + ' input').parents('.panHijos').parent().children('img');
                    $.each(imgVerMas, function () {
                        if (this.src.indexOf('verMas') > 0) {
                            MVCDesplegarTreeView(this);
                        }
                    });
                }
            });
        }

        if (panelDespl.find('#panEditoresLectores').length > 0)
        {
            if (txtBaseRecursos.attr("rel") == "private") {
                panelDespl.find('#panEditoresLectores').find('#panLectores').show();
                panelDespl.find('#panEditoresLectores').find('#panEditores').show();
            }
            else if (txtBaseRecursos.attr("rel") == "public") {
                panelDespl.find('#panEditoresLectores').find('#panLectores').hide();
                panelDespl.find('#panEditoresLectores').find('#panEditores').show();
            }
            else if (txtBaseRecursos.attr("rel") == "org") {
                panelDespl.find('#panEditoresLectores').find('#panLectores').hide();
                panelDespl.find('#panEditoresLectores').find('#panEditores').hide();
            }
        }

        AccionRecurso_Compartir_GenerarAutocompletar(documentoID, personaID, pListaUrlsAutocompletar);

        OcultarUpdateProgress();
    });
}

function AccionRecurso_AgregarABR_Aceptar(urlAceptarGuardarEn, urlCargarCompartidos, documentoID) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);
    var categorias = panelDespl.find('#txtSeleccionados').val().split(',');

    var lista = {};
    for (var i = 0; i < categorias.length; i++) {
        if (categorias[i].trim() != "") {
            lista[i] = categorias[i];
        }
    }

    var dataPost = {
        categoriesList: lista
    }

    GnossPeticionAjax(urlAceptarGuardarEn, dataPost, true).done(function () {
        $('#resource_' + documentoID + ' .acc_guardar').remove();
        $('#divGroupAccionesRec .opAddPersonal').remove();

        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, textoRecursos.compartirBROK);

        if (typeof shareAnalitics != 'undefined') {
            shareAnalitics.sharePersonalSpace();
        }
    }).fail(function () {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, textoRecursos.compartirBRFALLO);

    }).always(function () {
        if ($('#divCompartido').length > 0) {
            var dataPost2 = {
                callback: 'Compartidos|CargarCompartidos'
            }
            $.post(urlCargarCompartidos, dataPost2, function (data) {
                $('#divCompartido').html(data);
            });
        }
        OcultarUpdateProgress();
    });
}

function AccionRecurso_AgregarCategoriaABR_Aceptar(urlAceptarGuardarEn, urlCargarCompartidos, documentoID) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);
    panelDespl.find('#menssages.createCat').hide();
    panelDespl.find('#menssages.createCat .ko').hide();
    panelDespl.find('#menssages.createCat .ok').hide();
    var nombre = panelDespl.find('#inputNombreCategoria').val();
    var idCategoriaPadre = panelDespl.find('#selectCategoriaPadre').val();
    
    var dataPost = {
        categoryName: nombre,
        parentCategoryID: idCategoriaPadre
    }

    GnossPeticionAjax(urlAceptarGuardarEn, dataPost, true).done(function (mensaje) {   
        var marcados = panelDespl.find("#action input:checked");
        panelDespl.find("#action").html(mensaje);
        var txtSeleccionados = panelDespl.find('#txtSeleccionados');
        if(marcados.length > 0){
            for(var i = 0; i < marcados.length; i++){
                $("#" + marcados[i].id).prop( "checked", true );
                var claseInput = $("#" + marcados[i].id).parent().attr("class");
                txtSeleccionados.val(txtSeleccionados.val() + claseInput + ',');
            }
        }
        panelDespl.find('#menssages.createCat .ok').html('<p>' + textoRecursos.categoriasOK + '</p>');
        panelDespl.find('#menssages.createCat').show();
        panelDespl.find('#menssages.createCat .ok').show();
    }).fail(function (error) {
        panelDespl.find('#menssages.createCat').show();
        panelDespl.find('#menssages.createCat .ko').html('<p>' + error + '</p>');
        panelDespl.find('#menssages.createCat .ko').show();
    }).always(function () {
        panelDespl.find('#menssages.createCat').show();
        OcultarUpdateProgress();
    });
}

function AccionRecurso_Compartir_Aceptar(urlAceptarCompartir, documentoID, urlDocumento) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);
    var inputsBaseRecursos = panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar li input');
    var categorias = {};
    var contCategorias = 0;
    $.each(inputsBaseRecursos, function () {
        categorias[contCategorias] = $(this).val();
        contCategorias++;
    });

    var editores = {};
    var txtEditores = panelDespl.find('#panEditores').find('#divContEditores').find('#txtHackEditores_' + documentoID);
    if (txtEditores.length > 0) {
        var listaEditores = txtEditores.val().split(',');
        var contEditores = 0;
        for (var i = 0; i < listaEditores.length; i++) {
            if (listaEditores[i].trim() != "") {
                editores[contEditores] = listaEditores[i];
                contEditores++;
            }
        }
    }

    var lectores = {};
    var txtLectores = panelDespl.find('#panLectores').find('#divContLectores').find('#txtHackLectores_' + documentoID);
    if (txtLectores.length > 0) {
        var listaLectores = txtLectores.val().split(',');
        var contLectores = 0;
        for (var i = 0; i < listaLectores.length; i++) {
            if (listaLectores[i].trim() != "") {
                lectores[contLectores] = listaLectores[i];
                contLectores++;
            }
        }
    }

    var visibleSoloEditores = panelDespl.find('#panLectores').find('#rbLectoresEditores').is(':checked');
    
    var dataPost = {
        Categories: categorias,
        Editors: editores,
        Readers: lectores,
        Private: visibleSoloEditores
    }

    GnossPeticionAjax(urlAceptarCompartir, dataPost, true).done(function () {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, textoRecursos.compartirOK);

        if (typeof shareAnalitics != 'undefined') {
            //$.each(categorias, function () {
            //    var br = this.split("$$")[0];
            //    shareAnalitics.shareCommunity(br);
            //});
            if (typeof shareAnalitics != 'undefined') {
                shareAnalitics.shareCommunity();
            }
        }

    }).fail(function () {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, textoRecursos.compartirFALLO);
    }).always(function () {
        OcultarUpdateProgress();
        if ($('#divCompartido').length > 0) {
            var dataPost2 = {
                callback: "Compartidos|CargarCompartidos"
            }

            GnossPeticionAjax(urlDocumento, dataPost2, true).done(function (data) {
                $('#divCompartido').html(data);
            });
        }
    });
}

function AccionRecurso_Compartir_GenerarAutocompletar(documentoID, personaID, pListaUrlsAutocompletar) {
    var panelDespl = $('#despAccionRec_' + documentoID);
    
    var panEditores = panelDespl.find('#panEditores').find('#divContEditores');
    var panLectores = panelDespl.find('#panLectores').find('#divContLectores');

    CargarAutocompletarLectoresEditoresPorBaseRecursos(panEditores.find('#txtEditores_' + documentoID), panelDespl.find('#ddlCompartir option:selected').val(), personaID, 'txtHackEditores_' + documentoID, pListaUrlsAutocompletar, 'despAccionRec_' + documentoID, 'panContenedorEditores', 'txtHackEditores_' + documentoID);

    CargarAutocompletarLectoresEditoresPorBaseRecursos(panLectores.find('#txtLectores_' + documentoID), panelDespl.find('#ddlCompartir option:selected').val(), personaID, 'txtHackLectores_' + documentoID, pListaUrlsAutocompletar, 'despAccionRec_' + documentoID, 'panContenedorLectores', 'txtHackLectores_' + documentoID);
}

function AccionRecurso_Compartir_AjustarPrivacidadRecurso(documentoID, pRbSelecc) {
    var panelDespl = $('#despAccionRec_' + documentoID);

    var panEditores = panelDespl.find('#panEditores').find('#divContEditores');
    var panLectores = panelDespl.find('#panLectores').find('#divContLectores');

    if (pRbSelecc.id == 'rbEditoresYo') {
        $.each(panelDespl.find('#panEditores').find('#panContenedorEditores > ul > li').children(), function (i, v)         {
            $(this).click();
        });
        panEditores.hide();
    }
    else if (pRbSelecc.id == 'rbEditoresOtros') {
        panEditores.show();
    }
    else if (pRbSelecc.id == 'rbLectoresComunidad' || pRbSelecc.id == 'rbLectoresEditores') {
        $.each(panelDespl.find('#panLectores').find('#panContenedorLectores > ul > li').children(), function (i, v)         {
            $(this).click();
        });
        panLectores.hide();
    }
    else if (pRbSelecc.id == 'rbLectoresEspecificos') {
        panLectores.show();
    }
}

/**
 * Método para agregar una categoría desde la ficha Recurso al acceder a "Compartir.
 * Cuando se hace click sobre una categoría, esta se posiciona en un panel informativo para que el usuario posteriormente, confirme la acción.
 * Se ha incluido un botón de "delete" para que quede más intuitivo y parecido al nuevo Front.
 * @param documentoID: El id del documento que será asociado a una determinada comunidad.
 */
function AccionRecurso_Compartir_Agregar(documentoID) {
    MostrarUpdateProgress();

    var panelDespl = $('#despAccionRec_' + documentoID);

    var txtBaseRecursosID = panelDespl.find('#ddlCompartir option:selected').val();
    var txtSeleccionadas = panelDespl.find('#txtSeleccionados').val();
    // Cambiar debido al nuevo Front
    //var tieneCategorias = $('#divSelCatTesauro', panelDespl).children().length > 0;    
    var tieneCategorias = $('.divCategorias', panelDespl).children().length > 0;

    if (txtBaseRecursosID != "" && (txtSeleccionadas != "" || !tieneCategorias))
    {
        var html = "<li id='" + txtBaseRecursosID + "'>" +
        "   <input type=\"hidden\" value=\"" + txtBaseRecursosID + "$$" + txtSeleccionadas + "\" />" +
        "   <strong>" + panelDespl.find('#ddlCompartir option:selected').text() + "</strong>";

        var numCat = 0;
        $.each(txtSeleccionadas.split(','), function () {
            if (this != "") {
                // Cambiar debido al nuevo Front
                // var nombreCat = panelDespl.find('#divSelCatTesauro').find('span.' + this + ' label').text();
                var nombreCat = panelDespl.find('.divCategorias').find('div.' + this + ' label').text();
                if (numCat > 0) {
                    html += "   <label>,</label>";
                }
                html += "   <label>" + nombreCat + "</label>";

                numCat++;
            }
        });

        html += "   <a onclick=\"AccionRecurso_Compartir_Eliminar('" + txtBaseRecursosID + "', '" + documentoID + "')\" class=\"remove\"><span class=\"material-icons\" style=\"cursor:pointer\">delete</span></a>" +
        "</li>";

        panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar #' + txtBaseRecursosID).remove();
        panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar').append(html);

        panelDespl.find('#panContenedorBRAgregadas').show();
        panelDespl.find('#panBotonAceptar').show();
    }

    OcultarUpdateProgress();
}

function AccionRecurso_Compartir_Eliminar(baseRecursosID, documentoID) {
    var panelDespl = $('#despAccionRec_' + documentoID);
    var txtBaseRecursosID = panelDespl.find('#ddlCompartir option:selected').val();
    if (txtBaseRecursosID == baseRecursosID)
    {
        panelDespl.find('#txtSeleccionados').val('');
        panelDespl.find('#divSelCatTesauro').find('input:checked').prop('checked', false);
    }
    panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar #' + baseRecursosID).remove();

    if (panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar').children().length == 0)
    {
        panelDespl.find('#panContenedorBRAgregadas').hide();
        panelDespl.find('#panBotonAceptar').hide();
    }
}

/**
 * Acción para descompartir un recurso de una determinada comunidad
 * @param {any} urlDescompartir: Url a la que habrá que llamar para aplicar la acción de descompartir
 * @param {any} btnDescompartir: El botón de la papelera de la comunidad de la que se desea descompartir
 * @param {any} brID: 
 * @param {any} proyID: Id del proyecto
 * @param {any} orgID: Id de la organización
 */
function AccionRecurso_Descompartir(urlDescompartir, btnDescompartir, brID, proyID, orgID) {
    // Mostrar loading
    MostrarUpdateProgress();
    
    const dataPost = {
        BR: brID,
        ProyectID: proyID,
        OrganizationID: orgID
    };
    
    GnossPeticionAjax(urlDescompartir, dataPost, true)
        .done(function () {
            const ul = $(btnDescompartir).parent().parent();
            $(btnDescompartir).parent().remove();
            if (ul.children().length == 0) {
                ul.parent().remove();
            }
        })
        .always(function () {
            OcultarUpdateProgress();
        });    
}

function AccionRecurso_Duplicar_Aceptar(urlAceptarDuplicar, documentoID, urlDocumento) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);

    var txtBaseRecursos = panelDespl.find('#ddlCompartir option:selected');

    if (txtBaseRecursos.length > 0) {
        var urlDuplicarRecurso = txtBaseRecursos[0].value;
        urlDuplicarRecurso += urlAceptarDuplicar;
        window.location.href = urlDuplicarRecurso;
    } else {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, textoRecursos.compartirFALLO);
    }
}

/**
 * Método para votar un recurso de tipo Encuesta
 * @param {any} urlPagina
 * @param {any} documentoID
 */
function AccionRecurso_Encuesta_Votar(urlPagina, documentoID) {
    MostrarUpdateProgress();
    var panelContenedor = $('#encuesta_' + documentoID);
    var opcionEncuesta = panelContenedor.find('input[type = radio]:checked');

    const divPanelInfo = `<div id="divPanelInfo" class="alert alert-success mb-5" role="alert">Voto enviado correctamente.</div>`


    if (opcionEncuesta.length > 0) {
        var dataPost = {
            callback: 'Encuesta|Votar|' + opcionEncuesta.attr('id')
        }

        $.post(urlPagina, dataPost, function (data) {
            //panelContenedor.html(data);
            panelContenedor.html(divPanelInfo);
            OcultarUpdateProgress();
        });
    }
    else {
        OcultarUpdateProgress();
    }
}

function AccionRecurso_Encuesta_VerResultados(urlPagina, documentoID) {
    MostrarUpdateProgress();
    var panelContenedor = $('#encuesta_' + documentoID);
    var dataPost = {
        callback: 'Encuesta|VerResultados'
    }

    $.post(urlPagina, dataPost, function (data) {
        panelContenedor.html(data);
        OcultarUpdateProgress();
    });
}

function Comentario_CrearComentario(urlCrearComentario, documentoID) {
    if ($('#txtNuevoComentario_' + documentoID).val() != '') {
        MostrarUpdateProgress();
        $('#error_' + documentoID).html('');
        var descripcion = encodeURIComponent($('#txtNuevoComentario_' + documentoID).val());
        var datosPost = {
            Description: descripcion
        };
        GnossPeticionAjax(urlCrearComentario, datosPost, true).done(function (data) {
            $('span#numComentarios').text(parseInt($('span#numComentarios').text()) + 1);
            $('#txtNuevoComentario_' + documentoID).val('');
            var html = "";
            // Comprobar si es un objeto o array (La versión 5 devuelve un objeto de arrays)             
            if (!Array.isArray(data)) {
                // V5 -> Objeto de arrays -> Acceder al elemento 2 del objeto
                for (var i in data.$values) {
                    html += data.$values[i].html;
                }
            } else {
                // V4 -> Array solo ha sido devuelto
                for (var i in data) {
                    html += data[i].html;
                }
            }

            $('#panComentarios').html(html);
            if ((typeof CompletadaCargaComentarios != 'undefined')) {
                CompletadaCargaComentarios();
            }
            MontarFechas();

            if (typeof commentsAnalitics != 'undefined') {
                var comentarioID = $('#panComentarios .comment').first().attr('id');
                commentsAnalitics.commentCreated(comentarioID);
            }

            OcultarUpdateProgress();
        });
    }
    else {
        $('#error_' + documentoID).html(comentarios.comentarioError);
    }
}

/**
 * Acción para eliminar un comentario dentro de la ficha de un recurso
 * @param {any} urlEliminar: Url de api al que realizar la petición de borrado
 * @param {any} comentarioID: ID del comentario que se desea eliminar
 */
function Comentario_EliminarComentario(urlEliminar, comentarioID) {
    MostrarUpdateProgress();

    //contamos comentarios hijos para restarselos
    var panComentario = $('#' + comentarioID);
    var numComentariosRestar = $(panComentario).find('.comentario-contenido').length;

    GnossPeticionAjax(urlEliminar, null, true).done(function (data) {
        $('span#numComentarios').text(parseInt($('span#numComentarios').text()) - numComentariosRestar);

        $('#panComentarios #' + comentarioID).remove();

        if ((typeof CompletadaCargaEliminarComentarios != 'undefined')) {
            CompletadaCargaEliminarComentarios();
        }

        if (typeof commentsAnalitics != 'undefined') {
            commentsAnalitics.commentDeleted(comentarioID);
        }

        OcultarUpdateProgress();
    });
}

function Comentario_EliminarComentarioAnterior(urlEliminar, comentarioID) {
    MostrarUpdateProgress();

    //contamos comentarios hijos para restarselos
    var panComentario = $('#' + comentarioID);
    var numComentariosRestar = $(panComentario).find('.comment-content').length;
    
    GnossPeticionAjax(urlEliminar, null, true).done(function (data) {
        $('span#numComentarios').text(parseInt($('span#numComentarios').text()) - numComentariosRestar);

        $('#panComentarios #' + comentarioID).remove();

        if ((typeof CompletadaCargaEliminarComentarios != 'undefined')) {
            CompletadaCargaEliminarComentarios();
        }

        if (typeof commentsAnalitics != 'undefined') {
            commentsAnalitics.commentDeleted(comentarioID);
        }

        OcultarUpdateProgress();
    });
}

/**  
 * Método que se ejecuta cuando se pulsa en "Editar" un comentario dentro de la ficha de un recurso
 * @param {any} urlResponder: Url del comentario que se editará 
 * @param {any} comentarioID: ID del comentario que será editado
 */
function Comentario_EditarComentario(urlEditar, comentarioID) {
    // Panel dinamico del modal padre donde se insertara la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Contenido del comentario a editar
    const panComentario = $('#' + comentarioID);
    const panTextoComentario = $(panComentario).find('.comentario-contenido');
    const mensajeAntiguo = panTextoComentario.html();

    // Plantilla del panel html que se cargar� en el modal contenedor al pulsar en la acci�n
    let plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
    plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>Editar comentario</p>';
    plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
    plantillaPanelHtml += '<div class="formulario-edicion">';
    // Cuerpo de la respuesta - TextArea
    plantillaPanelHtml += '<div class="form-group">';
    plantillaPanelHtml += '<textarea class="form-control cke mensajes" id="txtComentario_Editar_' + comentarioID + '" rows="3">' + mensajeAntiguo + '</textarea>';
    plantillaPanelHtml += '</div>';

    // Contenedor de mensajes y botones
    plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
    // Posibles mensajes de info
    plantillaPanelHtml += '<div>';
    plantillaPanelHtml += '<div class="menssages_' + comentarioID + '" id="menssages_' + comentarioID + '">';
    plantillaPanelHtml += '<div class="ok"></div>';
    plantillaPanelHtml += '<div class="ko"></div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    // Panel de botones para la acci�n
    plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
    plantillaPanelHtml += '<button class="btn btn-primary">Editar comentario</button>'
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    // Meter el código de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el c�digo
    $modalDinamicContentPanel.html(plantillaPanelHtml);

    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');
    // Recargar editor CKE
    RecargarTodosCKEditor();

    // Asignación de la función al botón "Sí" o de acción
    $(botones[0]).on("click", function () {
        Comentario_EnviarEditarComentario(urlEditar, comentarioID);
    });
}

/**
 * Acción de poder editar un comentario sin modal
 * @param {any} urlEditar
 * @param {any} comentarioID
 */
function Comentario_Editar_JIRA(urlEditar, comentarioID) {

    // 0 - Botón de editar, guardar, cancelar
    const $btnEditar = $(`#edit-comment-${comentarioID}`);
    const $btnEditarMobile = $(`#edit-comment-mobile-${comentarioID}`);
    const $btnCancelar = $(`#cancel-comment-${comentarioID}`);
    const $btnCancelarMobile = $(`#cancel-comment-mobile-${comentarioID}`);
    const $btnGuardar = $(`#save-comment-${comentarioID}`);
    const $btnGuardarMobile = $(`#save-comment-mobile${comentarioID}`);

    // 1 - Detectar la caja de comentarios 
    const $comentarioEditar = $(`#${comentarioID}`).find('.comentario-contenido');
    // 2- Texto del comentario que se desea editar
    const textoComentarioEditar = $comentarioEditar.html();
    // 3- Crear un input text area dinámico para el CKEditor
    let ckeEditor = `<div class="form-group">`;
    ckeEditor += `<textarea cols="20" rows="3" id="txtComentario_Editar_${comentarioID}" class="cke ckeSimple comentarios">${textoComentarioEditar}</textarea>`;
    ckeEditor += `</div>`;

    // 4- Ocultar el comentario actual 
    $comentarioEditar.fadeOut();
    // 5- Añadir el CKEditor / TextArea
    $comentarioEditar.parent().append(ckeEditor)

    // 6- Ocultar el botón de Editar (ya se está editando) y mostrar "Guardar" y "Cancelar"
    $btnEditar.parent().toggleClass('d-none');
    $btnEditarMobile.parent().toggleClass('d-none');
    $btnCancelar.parent().toggleClass('d-none');
    $btnCancelarMobile.parent().toggleClass('d-none');
    $btnGuardar.parent().toggleClass('d-none');
    $btnGuardarMobile.parent().toggleClass('d-none');

    // Recargar editor CKE
    RecargarTodosCKEditor();
}

/**
 * Acción de poder Cancelar un comentario sin modal
 * @param {any} comentarioID
 */
function Comentario_Cancelar_JIRA(comentarioID) {

    // 0 - Botón de editar, guardar, cancelar
    const $btnEditar = $(`#edit-comment-${comentarioID}`);
    const $btnEditarMobile = $(`#edit-comment-mobile-${comentarioID}`);
    const $btnCancelar = $(`#cancel-comment-${comentarioID}`);
    const $btnCancelarMobile = $(`#cancel-comment-mobile-${comentarioID}`);
    const $btnGuardar = $(`#save-comment-${comentarioID}`);
    const $btnGuardarMobile = $(`#save-comment-mobile${comentarioID}`);

    // 1 - Detectar la caja de comentarios que estará oculta
    const $comentarioEditar = $(`#${comentarioID}`).find('.comentario-contenido');
    // 2 - Detectar el CKEditor que estará visible
    const $ckEditor = $(`#txtComentario_Editar_${comentarioID}`);
    // 3 - Remover el CKEditor actual (el padre contenedor) ya que se ha cancelado la acción
    $ckEditor.parent().remove();
    // 4 - Volver a mostrar la caja anterior del texto    
    $comentarioEditar.fadeIn();

    // 5- Ocultar el botón de Editar (ya se está editando) y mostrar "Guardar" y "Cancelar"
    $btnEditar.parent().toggleClass('d-none');
    $btnEditarMobile.parent().toggleClass('d-none');
    $btnCancelar.parent().toggleClass('d-none');
    $btnCancelarMobile.parent().toggleClass('d-none');
    $btnGuardar.parent().toggleClass('d-none');
    $btnGuardarMobile.parent().toggleClass('d-none');

}

function Comentario_EditarComentarioAnterior(urlEditar, comentarioID) {
    var panComentario = $('#' + comentarioID);

    var panTextoComentario = $(panComentario).find('.comment-content');
    panTextoComentario.hide();

    var mensajeAntiguo = panTextoComentario.html();

    $(panComentario).find('.comment-enviar').remove();
    $(panComentario).find('.comment-responder').remove();

    var accion = "javascript:Comentario_EnviarEditarComentario('" + urlEditar + "', '" + comentarioID + "');";
    var claseCK = 'cke comentarios';

    var confirmar = $(['<div class="comment-enviar"><fieldset class="mediumLabels"><legend>', comentarios.editarComentario, '</legend><p><textarea class="' + claseCK + '" id="txtComentario_Editar_', comentarioID, '" rows="2" cols="20">' + mensajeAntiguo + '</textarea><p><label class="error" id="error_', comentarioID, '"></label></p><input type="button" onclick="', accion, '" value="', comentarios.guardar, '" class="btn btn-primary text medium sendComment"></p></fieldset></div>'].join(''));

    panTextoComentario.after(confirmar);

    RecargarTodosCKEditor();
}

/**
 * Acción de Enviar al servidor (api) el comentario editado
 * @param {any} urlEditar: URL donde hay que hacer la petición para realizar la edición del comentario
 * @param {any} comentarioID: Id del comentario que se desea editar
 */
function Comentario_EnviarEditarComentario_JIRA(urlEditar, comentarioID) {
    if ($('#txtComentario_Editar_' + comentarioID).val() != '') {
        MostrarUpdateProgress();

        var descripcion = $('#txtComentario_Editar_' + comentarioID).val();

        var datosPost = {
            Description: encodeURIComponent(descripcion)
        };

        GnossPeticionAjax(urlEditar, datosPost, true).done(function (data) {
            if (typeof commentsAnalitics != 'undefined') {
                commentsAnalitics.commentModified(comentarioID);
            }
            // Ocultar el modal al haber editado el comentario            
            const panComentario = $('#' + comentarioID);
            const panTextoComentario = $(panComentario).find('.comentario-contenido');
            panTextoComentario.html(descripcion);
            // Ocultamos ckEditor y volvemos a la posición original
            Comentario_Cancelar_JIRA(comentarioID);
        })
            .always(function () {
                OcultarUpdateProgress();
            });
    }
}

/**
 * Acción de Enviar al servidor (api) el comentario editado
 * @param {any} urlEditar: URL donde hay que hacer la petición para realizar la edición del comentario
 * @param {any} comentarioID: Id del comentario que se desea editar
 */
function Comentario_EnviarEditarComentario(urlEditar, comentarioID) {
    if ($('#txtComentario_Editar_' + comentarioID).val() != '') {
        MostrarUpdateProgress();
        $('#error_' + comentarioID).html('');
        var descripcion = $('#txtComentario_Editar_' + comentarioID).val();

        var datosPost = {
            Description: encodeURIComponent(descripcion)
        };

        GnossPeticionAjax(urlEditar, datosPost, true).done(function (data) {
            if (typeof commentsAnalitics != 'undefined') {
                commentsAnalitics.commentModified(comentarioID);
            }
            // Ocultar el modal al haber editado el comentario
            $('#modal-container').modal('hide');
            const panComentario = $('#' + comentarioID);
            const panTextoComentario = $(panComentario).find('.comentario-contenido');
            panTextoComentario.html(descripcion);
        })
        .always(function () {
            OcultarUpdateProgress();
        });
    }
    else {
        $('#error_' + comentarioID).html(comentarios.comentarioError);
    }
}

function Comentario_EnviarEditarComentarioAnterior(urlEditar, comentarioID) {
    if ($('#txtComentario_Editar_' + comentarioID).val() != '') {
        MostrarUpdateProgress();
        $('#error_' + comentarioID).html('');
        var descripcion = $('#txtComentario_Editar_' + comentarioID).val();

        var datosPost = {
            Description: encodeURIComponent(descripcion)
        };

        GnossPeticionAjax(urlEditar, datosPost, true).done(function (data) {
            if (typeof commentsAnalitics != 'undefined') {
                commentsAnalitics.commentModified(comentarioID);
            }
        });

        var panComentario = $('#' + comentarioID);
        var panTextoComentario = $(panComentario).find('.comment-content');
        $(panComentario).find('.comment-enviar').remove();

        panTextoComentario.html(descripcion);
        panTextoComentario.show();

        OcultarUpdateProgress();
    }
    else {
        $('#error_' + comentarioID).html(comentarios.comentarioError);
    }
}

/**  
 * Método que se ejecuta cuando se pulsa en "Responder" un comentario dentro de la ficha de un recurso
 * @param {any} urlResponder: Url a la que se le responderá 
 * @param {any} comentarioID: ID del comentario al que se desea responder
 */
function Comentario_ResponderComentario(urlResponder, comentarioID) {
    // Panel dinamico del modal padre donde se insertara la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Plantilla del panel html que se cargar� en el modal contenedor al pulsar en la acci�n
    let plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
    plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>Responder comentario</p>';
    plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
    plantillaPanelHtml += '<div class="formulario-edicion">';
    // Cuerpo de la respuesta - TextArea
    plantillaPanelHtml += '<div class="form-group">';
    plantillaPanelHtml += '<textarea class="form-control cke mensajes" id="txtComentario_Responder_' + comentarioID + '" rows="3"> </textarea>';
    plantillaPanelHtml += '</div>';

    // Contenedor de mensajes y botones
    plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
    // Posibles mensajes de info
    plantillaPanelHtml += '<div>';
    plantillaPanelHtml += '<div class="menssages_' + comentarioID + '" id="menssages_' + comentarioID + '">';
    plantillaPanelHtml += '<div class="ok"></div>';
    plantillaPanelHtml += '<div class="ko"></div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    // Panel de botones para la acci�n
    plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
    plantillaPanelHtml += '<button class="btn btn-primary">' + mensajes.enviar + '</button>'
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    // Meter el código de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el c�digo
    $modalDinamicContentPanel.html(plantillaPanelHtml);

    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');
    // Recargar editor CKE
    RecargarTodosCKEditor();

    // Asignación de la función al botón "Sí" o de acción
    $(botones[0]).on("click", function () {
        Comentario_EnviarResponderComentario(urlResponder, comentarioID);
    });

}


function Comentario_ResponderComentarioAnterior(urlResponder, comentarioID) {
    var panComentario = $('#' + comentarioID);

    var panTextoComentario = $(panComentario).find('.comment-content');

    $(panComentario).find('.comment-enviar').remove();
    $(panComentario).find('.comment-responder').remove();

    var accion = "javascript:Comentario_EnviarResponderComentario('" + urlResponder + "', '" + comentarioID + "');";
    var claseCK = 'cke comentarios';

    var confirmar = $(['<div class="comment-responder"><fieldset class="mediumLabels"><legend>', comentarios.responderComentario, '</legend><p><textarea class="' + claseCK + '" id="txtComentario_Responder_', comentarioID, '" rows="2" cols="20"></textarea><p><label class="error" id="error_', comentarioID, '"></label></p><input type="button" onclick="', accion, '" value="', comentarios.enviar, '" class="text medium sendComment"></p></fieldset></div>'].join(''));

    panTextoComentario.after(confirmar);

    RecargarTodosCKEditor();
}

function Comentario_ResponderComentario_V2(urlResponder, comentarioID) {
    var panComentario = $('#' + comentarioID);

    var panTextoComentario = $(panComentario).find('.comment-content');

    $(panComentario).find('.comment-enviar').remove();
    $(panComentario).find('.comment-responder').remove();

    var accion = "javascript:Comentario_EnviarResponderComentario('" + urlResponder + "', '" + comentarioID + "');";
    var claseCK = 'cke comentarios';

    var confirmar = $(['<div class="escribir-comentario"><div class="publicador"></div><div class="escribe"><fieldset class="mediumLabels"><p><textarea class="' + claseCK + '" id="txtComentario_Responder_', comentarioID, '" rows="2" cols="20"></textarea><p><label class="error" id="error_', comentarioID, '"></label></p><input type="button" onclick="', accion, '" value="', comentarios.enviar, '" class="btn btn-primary text medium sendComment"></p></fieldset></div></div>'].join(''));

    panTextoComentario.after(confirmar);

    RecargarTodosCKEditor();
}

/**
 * Acción de Envio al servidor del comentario respondido
 * @param {any} urlResponder: URL de la api hacia donde hay que realizar la respuesta
 * @param {any} comentarioID: ID del comentario que se enviará.
 */
function Comentario_EnviarResponderComentario(urlResponder, comentarioID) {
    if ($('#txtComentario_Responder_' + comentarioID).val() != '') {
        MostrarUpdateProgress();
        $('#error_' + comentarioID).html('');
        var descripcion = encodeURIComponent($('#txtComentario_Responder_' + comentarioID).val().replace(/\n/g, ''));
        var datosPost = {
            Description: descripcion
        };

        GnossPeticionAjax(urlResponder, datosPost, true).done(function (data) {
            // Ocultar el modal al haber enviado correctamente la respuesta
            $('#modal-container').modal('hide');
            $('span#numComentarios').text(parseInt($('span#numComentarios').text()) + 1);
            var html = "";
            // Comprobar si es un objeto o array (La versión 5 devuelve un objeto de arrays)            
            if (!Array.isArray(data)) {
                // V5 -> Objeto de arrays -> Acceder al elemento 2 del objeto
                for (var i in data.$values) {
                    html += data.$values[i].html;
                }
            } else {
                // V4 -> Array solo ha sido devuelto
                for (var i in data) {
                    html += data[i].html;
                }
            }

            $('#panComentarios').html(html);
            if ((typeof CompletadaCargaComentarios != 'undefined')) {
                CompletadaCargaComentarios();
            }
            MontarFechas();

            if (typeof commentsAnalitics != 'undefined') {
                var comentarioRespuestaID = $('#panComentarios #' + comentarioID + ' .comment').first().attr('id');
                commentsAnalitics.commentCreated(comentarioRespuestaID);
            }
            OcultarUpdateProgress();
        });
    }
    else {
        $('#error_' + comentarioID).html(comentarios.comentarioError);
    }
}

function Comentario_VotarPositivo(that, urlVotarPositivo, comentarioID) {
    MostrarUpdateProgress();
    $(that).parent().find('.eleccionUsuario').removeClass("eleccionUsuario");
    $(that).addClass("eleccionUsuario");

    $.post(urlVotarPositivo, null, function (data) {
        var html = "";
        for (var i in data) {
            html += data[i].html;
        }
        $('#panComentarios').html(html);
        if ((typeof CompletadaCargaVotoComentario != 'undefined')) {
            CompletadaCargaVotoComentario();
        }
        MontarFechas();
        OcultarUpdateProgress();
    });
}

function Comentario_VotarNegativo(that, urlVotarNegativo, comentarioID) {
    MostrarUpdateProgress();
    $(that).parent().find('.eleccionUsuario').removeClass("eleccionUsuario");
    $(that).addClass("eleccionUsuario");

    $.post(urlVotarNegativo, null, function (data) {
        var html = "";
        for (var i in data) {
            html += data[i].html;
        }
        $('#panComentarios').html(html);
        if ((typeof CompletadaCargaVotoComentario != 'undefined')) {
            CompletadaCargaVotoComentario();
        }
        MontarFechas();
        OcultarUpdateProgress();
    });
}

function Comentario_EliminarVoto(that, urlEliminarVoto, comentarioID) {
    MostrarUpdateProgress();
    $(that).parent().find('.eleccionUsuario').removeClass("eleccionUsuario");

    $.post(urlEliminarVoto, null, function (data) {
        var html = "";
        for (var i in data) {
            html += data[i].html;
        }
        $('#panComentarios').html(html);
        if ((typeof CompletadaCargaVotoComentario != 'undefined')) {
            CompletadaCargaVotoComentario();
        }
        MontarFechas();
        OcultarUpdateProgress();
    });
}

function MontarFacetaFichaFormSem(pUrlFac, pGrafo, pFaceta, pParametros, pIdentidad, pLanguageCode, pPanelID) {

    pIdentidad = pIdentidad.toUpperCase();
    var servicio = new WS(pUrlFac, WSDataType.jsonp);

    var metodo = 'CargarFacetas';
    var params = {};
    params['pProyectoID'] = pGrafo;
    //params['bool_esMyGnoss'] = 'False';
    params['pEstaEnProyecto'] = true;
    params['pEsUsuarioInvitado'] = (pIdentidad == 'FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF');
    params['pIdentidadID'] = pIdentidad;
    params['pUbicacionBusqueda'] = 'MetahomeCatalogo';
    params['pParametros'] = pParametros;
    params['pLanguageCode'] = pLanguageCode;
    params['pPrimeraCarga'] = false;
    params['pAdministradorVeTodasPersonas'] = false;
    params['pTipoBusqueda'] = 0;
    params['pNumeroFacetas'] = 1;
    params['pFaceta'] = pFaceta;
    params['pGrafo'] = pGrafo;
    params['pFiltroContexto'] = '';
    params['pParametros_adiccionales'] = 'factFormSem=true';
    params['pUsarMasterParaLectura'] = false;

    servicio.call(metodo, params, function (data) {
        var descripcion = data;
        $('#' + pPanelID).show();
        $('#' + pPanelID).html(descripcion);
        $('.facetedSearchBox', $('#' + pPanelID)).hide();

        if ((typeof CompletadaCargaFacetasFichaRec != 'undefined')) {
            CompletadaCargaFacetasFichaRec();
        }
    });
}

function PaginarSelectorEnt(link, urlAccion, entidad, propiedad, elemsPag, totalElem) {
    var divPadre = $($(link).parent('.pagSelectEnt')[0]);

    if (divPadre.data('pag') == null) {
        divPadre.data('pag', 0);
        divPadre.data('elemsCarg', 1);
    }

    var pagActual = divPadre.data('pag');
    var elemsCarg = divPadre.data('elemsCarg');

    if (link.className == 'sigPagSelectEnt') {
        if (totalElem <= (elemsPag * (pagActual + 1))) {
            return;
        }

        pagActual++;

        if (pagActual > (elemsCarg - 1)) {
            var divCargando = $(divPadre.parent().children('.loadpagSelectEnt')[0]);
            var dataPost = { entidad: entidad, propiedad: propiedad, inicioPag: (elemsPag * pagActual) };
            GnossPeticionAjax(urlAccion, dataPost, true).done(function (data) {
                divCargando.before('<div class="pagSelEnt" style="display:none;">' + data + '</div>');
                elemsCarg++;
                divPadre.data('elemsCarg', elemsCarg);
            }).fail(function () {
                pagActual--;
            }).always(function () {
                divCargando.css('display', 'none');
                divPadre.css('display', '');
                AjustarDatosSelectorEntPaginado(pagActual, divPadre);
            });

            divCargando.css('display', '');
            divPadre.css('display', 'none');
        }
    }
    else if (pagActual > 0) {
        pagActual--;
    }

    divPadre.data('pag', pagActual);
    
    AjustarDatosSelectorEntPaginado(pagActual, divPadre);
}

function AjustarDatosSelectorEntPaginado(pagActual, divPadre) {
    var divDatos = divPadre.parent().children('.pagSelEnt');

    for (var i = 0; i < divDatos.length; i++) {
        if (i == pagActual) {
            $(divDatos[i]).css('display', '');
        }
        else {
            $(divDatos[i]).css('display', 'none');
        }
    }
}

function MontarFechaCliente() {
    if (typeof (diffHoras) == 'undefined' || diffHoras == null) {
        var fechaServidor = new Date($('#inpt_serverTime').val());
        $('p.publicacion, span.fechaPublicacion').each(function (index) {
            if ($(this).attr('content') != null) {
                var fechaRecurso = new Date($(this).attr('content'));
                var fechaCliente = new Date();
                //var diffHoras = parseInt((fechaServidor.getTime() / (1000 * 60 * 60)) - (fechaCliente.getTime() / (1000 * 60 * 60)));
                var diffMinutos = parseInt((fechaServidor.getTime() / (1000 * 60)) - (fechaCliente.getTime() / (1000 * 60)));
                var diffHoras = diffMinutos / 60;
                //redondeo
                var resto = diffMinutos % 60;
                if (resto / 60 > 0.5) {
                    if (diffHoras > 0) {
                        diffHoras = diffHoras + 1;
                    }
                    else {
                        diffHoras = diffHoras - 1;
                    }
                }
                fechaRecurso.setHours(fechaRecurso.getHours() - diffHoras);
                var dia = fechaRecurso.getDate();
                if (dia < 10) {
                    dia = '0' + dia;
                }
                var mes = fechaRecurso.getMonth() + 1;
                if (mes < 10) {
                    mes = '0' + mes;
                }
                //var fechaPintado = fechaRecurso.format("yyyy/MM/dd HH:mm");
                var fechaPintado = tiempo.fechaPuntos.replace('@1@', dia).replace('@2@', mes).replace('@3@', fechaRecurso.getFullYear());
                $(this).html(fechaPintado);
                $(this).show();
            }
        });
    }
}

function InitializeRouteMapGoogle(pDivID, pRoute, pColor) {
    try {
        var mapOptions = {
            zoom: 3,
            center: new google.maps.LatLng(0, 0),
            mapTypeId: google.maps.MapTypeId.TERRAIN
        };

        var map = new google.maps.Map(document.getElementById(pDivID), mapOptions);

        var mapbounds = new google.maps.LatLngBounds();

        pRoute = pRoute.replace(/\;/g, ',');
        var puntosParseados = JSON && JSON.parse(pRoute) || $.parseJSON(pRoute);
        var puntosRuta = [];
        for (var i = 0; i < puntosParseados["geo:coordinates"].length; i++) {
            puntosRuta[i] = new google.maps.LatLng(puntosParseados["geo:coordinates"][i][0], puntosParseados["geo:coordinates"][i][1]);

            mapbounds.extend(puntosRuta[i]);
        }

        map.fitBounds(mapbounds);
        map.setCenter(mapbounds.getCenter());

        if (pColor == '' || pColor == null) {
            pColor = '#FF0000';
        }

        var ruta = new google.maps.Polyline({
            path: puntosRuta,
            geodesic: true,
            strokeColor: pColor,
            strokeOpacity: 1.0,
            strokeWeight: 2
        });

        ruta.setMap(map);
    }
    catch (ex) {
    }
}

$(function () {
    $('input.btnAccionSemCms').click(function () { AccionFichaRecSemCms(this); });
});

function AccionFichaRecSemCms(boton) {
    if (typeof (AccionFichaRecSemCmsPersonalizado) != 'undefined') {
        AccionFichaRecSemCmsPersonalizado(boton);
        return;
    }

    var that = boton;
    var idBtn = $(boton).attr('id');
    idBtn = idBtn.replace('btn_', '');
    var arg = {};
    arg.ActionID = idBtn;
    arg.ActionEntityID = $(boton).attr('rel');
    $('#errorbtn_' + idBtn).remove();

    if (arg.ActionEntityID == null || arg.ActionEntityID == '') {
        return;
    }

    MostrarUpdateProgressTime(0);

    GnossPeticionAjax(urlActionSemCms, arg, true).done(function (data) {
        $(that).after('<div id="errorbtn_' + idBtn + '" class="ok" style="display:block">' + data + '</div>');
    }).fail(function (data) {
        var mensajeError = data;
        if (data == "NETWORKERROR") {
            mensajeError = 'Has perdido la conexión. Comprueba tu conexión a internet y verifica que tus cambios se han guardado correctamente.';
        }
        $(that).after('<div id="errorbtn_' + idBtn + '" class="ko" style="display:block">' + mensajeError + '</div>');
    }).always(function () {
        OcultarUpdateProgress();
    });
}/**/ 
/*MVC.FichaPerfil.js*/ 
﻿/**
 * Acci�n de seguir a un usuario de una comunidad. Se ejecuta cuando se pulsa en el bot�n "Seguir"
 * @param {any} that: El bot�n que ha disparado la acci�n
 * @param {any} urlSeguirPerfil: URL a la que hay que llamar para realizar la llamda de "No seguir"
 */
function AccionPerfil_Seguir(that, urlSeguirPerfil) {

    // Icono del bot�n
    var buttonIcon = $(that).find("span.material-icons");
    // Texto del bot�n
    var textButton = $(buttonIcon).siblings();
    // Textos e iconos una vez pulsado el bot�n
    var newTextButton = "Siguiendo";
    var newIconButton = "person_outline";


    EnviarAccGogAnac('Acciones sociales', 'Seguir a', urlSeguirPerfil);


    that.attributes.removeNamedItem('onclick');
    
    GnossPeticionAjax(urlSeguirPerfil, null, true).fail(function (data) {
        if (data == "invitado") {
            operativaLoginEmergente.init();
        }
    });

    // Nuevo front
    // $(that).html($(that).html().replace('Seguir', 'Siguiendo'));
    ChangeButtonAndText(that, newTextButton, newIconButton, "btn-primary");
}

/**
 * Acci�n de no seguir a un usuario de una comunidad. Se ejecuta cuando se pulsa en el bot�n "No seguir"
 * Cambiar� el nombre al bot�n indicando "Siguiendo"
 * Eliminar� el atributo onClick para que no se vuelva a ejecutar la acci�n del bot�n
 * @param {any} that: El bot�n que ha disparado la acci�n
 * @param {any} urlNoSeguirPerfil: URL a la que hay que llamar para realizar la llamda de "No seguir"
 */
function AccionPerfil_NoSeguir(that, urlNoSeguirPerfil) {

    // Icono del bot�n
    var buttonIcon = $(that).find("span.material-icons");
    // Texto del bot�n
    var textButton = $(buttonIcon).siblings();
    // Textos e iconos una vez pulsado el bot�n
    var newTextButton = "Sin seguimiento";
    var newIconButton = "person_outline";
    
    GnossPeticionAjax(urlNoSeguirPerfil, null, true).fail(function (data) {
        if (data == 'invitado') {
            operativaLoginEmergente.init();
        }
    });
    // Nuevo Front
    //$(that).parent().remove();
    // Cambiar el nombre e icono del bot�n
    ChangeButtonAndText(that, newTextButton, newIconButton, "btn-primary");
}

/**
 * Cambiar el texto, el icono (material-icons) y eliminar el evento click de un bot�n
 * @param {any} button: El bot�n que se desea modificar
 * @param {any} newTextButton: El nuevo texto que tendr� el bot�n
 * @param {any} newIconButton: El nuevo icono (material-icons) que tendr� el bot�n
 * @param {any} classToBeDeleted: La clase que se eliminar� del bot�n 
 */
function ChangeButtonAndText(button, newTextButton, newIconButton, classToBeDeleted) {

    // Icono del bot�n
    var buttonIcon = $(button).find("span.material-icons");
    var textButton = "";
    var icon = "";

    if (buttonIcon.length == 0) {
        // No hay botón --> Buscar un span
        buttonIcon = $(button).siblings();
        textButton = $(button);

    } else {
        textButton = $(buttonIcon).siblings();
    }

    // Cambiar nombre al bot�n 
    textButton.text(newTextButton);
    // Cambiar el icono al bot�n
    buttonIcon.text(newIconButton);
    // Eliminar atributo onclick del bot�n
    $(button).prop("onclick", null).off("click");
    // A�adir cursor: normal
    $(button).css("cursor", "auto");
    // Quitar el estilo de bot�n (no clickeable)
    $(button).removeClass(classToBeDeleted);
}


/**
 * Acci�n que se ejecuta cuando se pulsa sobre las acciones disponibles de un perfil para mandar un "Email". 
 * @param {string} titulo: T�tulo que tendr� el panel modal 
 * @param {string} texto: El texto o mensaje a modo de t�tulo que se mostrar� para que el usuario sepa la acci�n que se va a realizar
 * @param {string} id: Identificador de la persona sobre el que se aplicar� la acci�n 
 * @param {any} idModalPanel: Panel modal contenedor donde se insertar� este HTML (Por defecto ser� #modal-container)
 * */
function AccionEnviarMensajeMVC(urlPagina, id, titulo, idModalPanel = "#modal-container") {
    
    // Panel din�mico del modal padre donde se insertar� la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Declaraci�n de la acci�n que se realizar� al hacer click en Enviar mensaje
    var accion = "EnviarMensaje('" + urlPagina + "', '" + id + "');";
    
    // Plantilla del panel html que se cargar� en el modal contenedor al pulsar en la acci�n
    var plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
        plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>' + titulo + '</p>';
        plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
        plantillaPanelHtml += '<div class="formulario-edicion">';
            // Asunto del email - InputText
            plantillaPanelHtml += '<div class="form-group">';
                plantillaPanelHtml += '<label for="txtAsunto_'+id+'">' + mensajes.enviarMensaje + '</label>';
                plantillaPanelHtml += '<input type="text" class="form-control" id="txtAsunto_'+id+'" rows="3"> </textarea>' ;
            plantillaPanelHtml += '</div>';

            // Cuerpo del email - TextArea
            plantillaPanelHtml += '<div class="form-group">';
                plantillaPanelHtml += '<label>' + mensajes.descripcion + '</label>';
                plantillaPanelHtml += '<textarea class="form-control cke mensajes" id="txtDescripcion_'+id+'" rows="3"> </textarea>' ;
            plantillaPanelHtml += '</div>';

        // Contenedor de mensajes y botones
        plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
        // Posibles mensajes de info
            plantillaPanelHtml += '<div>';
                plantillaPanelHtml += '<div class="menssages_'+id+'" id="menssages">';
                    plantillaPanelHtml += '<div class="ok"></div>';
                    plantillaPanelHtml += '<div class="ko"></div>';
                plantillaPanelHtml += '</div>';
            plantillaPanelHtml += '</div>';
        // Panel de botones para la acci�n
            plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
                plantillaPanelHtml += '<button class="btn btn-primary">' + mensajes.enviar + '</button>'                
            plantillaPanelHtml += '</div>';
        plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    // Meter el c�digo de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el c�digo
    $modalDinamicContentPanel.html(plantillaPanelHtml);

    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');

    RecargarTodosCKEditor();

    // Asignaci�n de la funci�n al bot�n "S�" o de acci�n
    $(botones[0]).on("click", function () {
        EnviarMensaje(urlPagina, id);
    });   
    
}

/**
 * Acci�n que se ejecuta para mandar un email a un contacto. Ej: Desde el perfil de un usuario, se puede pulsar en el bot�n "Escribir mensaje".
 * Esta acci�n es ejecutada desde AccionEnviarMensajeMVC
 * @param {any} urlPagina
 * @param {any} id
 */
function EnviarMensaje(urlPagina, id) {
    if ($('#txtAsunto_' + id).val() != '' && $('#txtDescripcion_' + id).val() != '') {
        var asunto = $('#txtAsunto_' + id).val();
        var mensaje = $('#txtDescripcion_' + id).val();
        MostrarUpdateProgress();

        var dataPost = {
            callback: "Accion_EnviarMensaje",
            asunto: asunto,
            mensaje: encodeURIComponent(mensaje.replace(/\n/g, ''))
        }

        GnossPeticionAjax(urlPagina, dataPost, true).done(function (data) {
            // Cambiado por nuevo Front
            //DesplegarResultadoAccionMVC("desplegable_" + id, true, mensajes.mensajeEnviado);
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", true, mensajes.mensajeEnviado);
            // Esperar 1,5 segundos y ocultar el panel
            setTimeout(() => {
                $('#modal-container').modal('hide');
            }, 1500)
        }).fail(function (data) {
            //DesplegarResultadoAccionMVC("desplegable_" + id, false, "");
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, "Se ha producido un error al enviar el mensaje. Por favor int�ntalo de nuevo m�s tarde.");
        }).always(function (data) {
            OcultarUpdateProgress();
        });
    }
    else {
        $('.menssages_' + id).find(".ko").css("display", "block");
        $('.menssages_' + id).find(".ko").html(mensajes.mensajeError);
    }
}

function AccionEnviarMensajeMVCTutor(urlPagina, id) {
    var $c = $('#' + id);

    if (CKEDITOR.instances["txtDescripcion_" + id] != null) {
        CKEDITOR.instances["txtDescripcion_" + id].destroy();
    }

    if ($c.children().length > 0) {
        //Eliminar el anterior
        $c.children().remove();
    }

    var accion = "javascript:EnviarMensajeTutor('" + urlPagina + "', '" + id + "');";

    var $confirmar = $(['<fieldset class="mediumLabels"><legend>', mensajes.enviarMensaje, '</legend><p><label for="txtAsunto_', id, '">', mensajes.asunto, '</label><input type="text" id="txtAsunto_', id, '" class="text big"></p><p><label for="txtDescripcion_', id, '">', mensajes.descripcion, '</label></p><p><textarea class="cke mensajes" id="txtDescripcion_', id, '" rows="2" cols="20"></textarea></p><p><label class="error" id="error_', id, '"></label></p><input type="button" onclick="', accion, '" value="', mensajes.enviar, '" class="text medium"></p></fieldset>'].join(''));
    //$confirmar.find('div').filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
    // Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
    $confirmar.prependTo($c)
.find('button').click(function () { // Ambos botones hacen desaparecer la mascara
    $c.parents('.stateShowForm').css({ display: 'none' });
    $confirmar.css({ display: 'none' });
}).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada

    if ($('#divContMensajesPerf').length > 0 && $('#divContMensajesPerf').html() == '') {
        $('#divContMensajesPerf').html($('#desplegable_' + id).parent().html());
        $('#desplegable_' + id).parent().html('');
    }

    MostrarPanelAccionDesp("desplegable_" + id, null);
    RecargarTodosCKEditor();
}


function EnviarMensajeTutor(urlPagina, id) {
    if ($('#txtAsunto_' + id).val() != '' && $('#txtDescripcion_' + id).val() != '') {
        var asunto = $('#txtAsunto_' + id).val();
        var mensaje = $('#txtDescripcion_' + id).val();
        MostrarUpdateProgress();

        var dataPost = {
            callback: "Accion_EnviarMensajeTutor",
            asunto: asunto,
            mensaje: encodeURIComponent(mensaje.replace(/\n/g, ''))
        }

        GnossPeticionAjax(urlPagina, dataPost, true).done(function (data) {
            DesplegarResultadoAccionMVC("desplegable_" + id, true, mensajes.mensajeEnviado);
        }).fail(function (data) {
            DesplegarResultadoAccionMVC("desplegable_" + id, false, data);
        }).always(function (data) {
            OcultarUpdateProgress();
        });
    }
    else {
        $('#error_' + id).html(mensajes.mensajeError);
    }
}

/**
 * Acción de mandar a API endPoint la acción de agregar un contacto 
 * @param {any} urlPagina: Url a para ejecutar la acción
 */
function AgregarContacto(urlPagina) {
    var dataPost = {
        callback: "Accion_AgregarContacto"
    }
    $.post(urlPagina, dataPost);
    // Cerrar el modal pasados 1,5 segundos
    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500);
}

/**
 * Acción de mandar a API endPoint la acci�n de Eliminar un contacto
 * @param {any} urlPagina: Url a para ejecutar la acci�n
 */
function EliminarContacto(urlPagina) {
    var dataPost = {
        callback: "Accion_EliminarContacto"
    }
    $.post(urlPagina, dataPost);
    // Cerrar el modal pasados 1,5 segundos
    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500);
}

function AgregarContactoOrg(urlPagina) {
	var dataPost = {
		callback: "Accion_AgregarContactoOrg"
	}
	$.post(urlPagina, dataPost);
}

function EliminarContactoOrg(urlPagina) {
	var dataPost = {
		callback: "Accion_EliminarContactoOrg"
	}
	$.post(urlPagina, dataPost);
}

function NotificarCorreccion(urlPagina) {
	var dataPost = {
		callback: "Accion_EnviarCorreccion"
	}
	$.post(urlPagina, dataPost);
}

function NotificarCorreccionDefinitiva(urlPagina) {
	var dataPost = {
		callback: "Accion_EnviarCorreccion",
		EnviarCorreccionDefinitiva: true
	}
	$.post(urlPagina, dataPost);
}

function ValidarCorreccion(urlPagina) {
	var dataPost = {
		callback: "Accion_ValidarCorreccion"
	}
	$.post(urlPagina, dataPost);
}

function EliminarPersona(urlPagina) {
	var dataPost = {
		callback: "Accion_EliminarPersona"
	}
	$.post(urlPagina, dataPost);
}

/**
 * Acci�n para expulsar a una persona de una comunidad. Se ejecuta cuando (por ejemplo) se selecciona desde listado de personas, la opci�n de "Expulsar"
 * Se cargar� un nuevo modal para hacer la gesti�n de la explusi�n
 * @param {any} urlPagina: Url a la que se realizar� la petici�n para expulsar a la persona
 * @param {any} id: Identificador de la persona a la que se expulsar�
 * @param {any} titulo: Titulo de la ventana modal
 * @param {any} textoBotonPrimario: Titulo del bot�n primario
 * @param {any} textoBotonSecundario: Titulo del bot�n secundario (No/Cancelar)
 * @param {any} texto: Explicaci�n de la acci�n de expulsar usuario
  * @param {any} accionCambiarNombreHtml: Accion JS que servir� para cambiar el nombre del elemento una vez se proceda a expulsar a una persona.
 * @param {any} idModalPanel: Panel modal contenedor donde se insertar� este HTML (Por defecto ser� #modal-container)
 */
function Expulsar(urlPagina, id, titulo, textoBotonPrimario, textoBotonSecundario, texto, accionCambiarNombreHtml, idModalPanel = "#modal-container") {
    
    // Acci�n que se ejecutar� al pulsar sobre el bot�n primario (Realizar la acci�n de Expulsar)
    var accion = "EnviarAccionExpulsar('" + urlPagina + "', '" + id + "');";

    // Panel din�mico del modal padre donde se insertar� la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Plantilla del panel html que se cargar� en el modal contenedor al pulsar en la acci�n
    var plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
        plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>' + titulo + '</p>';
        plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
        plantillaPanelHtml += '<div class="formulario-edicion">';
            plantillaPanelHtml += '<div class="form-group">';
                plantillaPanelHtml += '<label class="control-label">' + texto + '</label>';
            plantillaPanelHtml += '</div>';

        // Cuerpo del panel -> TextArea para enviar un email explicando la raz�n de la expulsi�n
            plantillaPanelHtml += '<div class="form-group">';
                plantillaPanelHtml += '<label for="txtMotivoExpulsion_'+ id +'">'+ accionesUsuarioAdminComunidad.motivoExpulsion +'</label>';
                plantillaPanelHtml += '<textarea class="form-control" id="txtMotivoExpulsion_'+id+'" rows="3"></textarea>';
            plantillaPanelHtml += '</div>';

            plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
        // Posibles mensajes de info
                plantillaPanelHtml += '<div>';
                    plantillaPanelHtml += '<div id="menssages">';
                        plantillaPanelHtml += '<div class="ok"></div>';
                        plantillaPanelHtml += '<div class="ko"></div>';
                    plantillaPanelHtml += '</div>';
                plantillaPanelHtml += '</div>';
            // Panel de botones para la acci�n
                plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
                    plantillaPanelHtml += '<button data-dismiss="modal" class="btn btn-primary">' + textoBotonSecundario + '</button>'
                    plantillaPanelHtml += '<button class="btn btn-outline-primary ml-1" onclick="' + accion + '">'+ textoBotonPrimario + ", " + accionesUsuarioAdminComunidad.expulsarUsuario + '</button>'
                plantillaPanelHtml += '</div>';
            plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    // Meter el c�digo de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el c�digo
    $modalDinamicContentPanel.html(plantillaPanelHtml);

    // Asignar acciones adicionales al bot�n primario (Cambiar nombre del html)
    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');

    // Asignaci�n de la funci�n al bot�n "S�" o de acci�n
    $(botones[1]).on("click", function () {
        // Ocultar el panel modal de bootstrap si hiciera falta        
    }).click(accionCambiarNombreHtml);
}

/**
 * Enviar la nueva petici�n del cambio de expulsi�n de una comunidad a un perfil sobre el que se ha pulsado el bot�n de "S�, Expulsar" del modal.
 * @param {any} urlPagina: Url donde se lanzar� la petici�n para cambiar el rol
 * @param {any} id: Identificador de la persona 
 */
function EnviarAccionExpulsar(urlPagina, id) {

    if ($('#txtMotivoExpulsion_' + id).val() != '') {
        var motivo = $('#txtMotivoExpulsion_' + id).val();
        MostrarUpdateProgress();

        var dataPost = {
            callback: "Accion_Expulsar",
            motivo: motivo
        }

        GnossPeticionAjax(urlPagina, dataPost, true).done(function (data) {
            // Cambiado por el nuevo front
            // DesplegarResultadoAccionMVC("desplegable_" + id, true, accionesUsuarioAdminComunidad.miembroExpulsado);
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", true, accionesUsuarioAdminComunidad.miembroExpulsado);
            // Esperar 1 segundo y cerrar la ventana
            setTimeout(() => {
                $('#modal-container').modal('hide');
            }, 1500)
            
        }).fail(function (data) {
            // Cambiado por nuevo front
            //DesplegarResultadoAccionMVC("desplegable_" + id, false, "");
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, "Error al tratar de expulsar al perfil de la comunidad. Por favor, int�ntalo de nuevo m�s tarde.");
        }).always(function (data) {
            OcultarUpdateProgress();
        });
    }
    else {
        // Cambiado por nuevo Front
        //$('#error_' + id).html(accionesUsuarioAdminComunidad.expulsionMotivoVacio);
        DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, accionesUsuarioAdminComunidad.expulsionMotivoVacio);
    }
}

/**
 * Acci�n que se ejecuta cuando se pulsa sobre la acci�n de "Cambiar rol" disponible en un item/recurso de tipo "Perfil" encontrado por el buscador.  
 * @param {string} id: Identificador del recurso (en este caso de la persona) sobre el que se aplicar� la acci�n 
 * @param {any} rol: El rol actual del recurso (Perfil) clickeado
 * @param {any} urlPagina: Pagina sobre la que se lanzar� la llamada para realizar la acci�n de cambiar rol
 * @param {any} idModalPanel: Panel modal contenedor donde se insertar� este HTML (Por defecto ser� #modal-container)
 */
function CambiarRol(id, rol, urlPagina, idModalPanel = "#modal-container") {

    // Acci�n que se ejecutar� al pulsar sobre el bot�n primario (Realizar la acci�n de cambiar rol)
    var accion = "EnviarAccionCambiarRol('" + urlPagina + "', '" + id + "', '" + rol + "');";
    // Permisos para pintar los checkbox a mostrar al usuario
    var checkedAdmin = '';
    var checkedSupervisor = '';
    var checkedUsuario = '';

    if (rol == 0) {
        checkedAdmin = ' checked';
    }
    else if (rol == 1) {
        checkedSupervisor = ' checked';
    }
    else if (rol == 2) {
        checkedUsuario = ' checked';
    }

    // Panel din�mico del modal padre donde se insertar� la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Plantilla del panel html que se cargar� en el modal contenedor al pulsar en la acci�n
    var plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
        plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>' + accionesUsuarioAdminComunidad.cambiarRol + '</p>';
        plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
        plantillaPanelHtml += '<div class="formulario-edicion">';
            plantillaPanelHtml += '<div class="form-group">';
                plantillaPanelHtml += '<label class="control-label">' + accionesUsuarioAdminComunidad.selecionaRol + '</label>';
            plantillaPanelHtml += '</div>';
            // Cuerpo del panel -> Opciones de checkbox a cargar
            plantillaPanelHtml += '<div class="form-group">';
                plantillaPanelHtml += '<div class="themed primary custom-control custom-radio">';
                    plantillaPanelHtml += '<input name="cambiarRol_' + id + '" type="radio" id="cambiarRol_administrador' + id + '" value="0" class="custom-control-input"'+ checkedAdmin +'/>';
                    plantillaPanelHtml += '<label class="custom-control-label" for="cambiarRol_administrador' + id + '">'+ accionesUsuarioAdminComunidad.administrador +'</label>';
                plantillaPanelHtml += '</div>';
                plantillaPanelHtml += '<div name="cambiarRol' + id + '" class="themed primary custom-control custom-radio">';
                    plantillaPanelHtml += '<input name="cambiarRol_' + id + '" type="radio" id="cambiarRol_supervisor' + id + '" value="1" class="custom-control-input"'+ checkedSupervisor +'/>';
                    plantillaPanelHtml += '<label class="custom-control-label" for="cambiarRol_supervisor' + id + '">'+ accionesUsuarioAdminComunidad.supervisor +'</label>';
                plantillaPanelHtml += '</div>';
                plantillaPanelHtml += '<div class="themed primary custom-control custom-radio">';
                    plantillaPanelHtml += '<input name="cambiarRol_' + id + '" type="radio" id="cambiarRol_usuario' + id + '" value="2" class="custom-control-input"'+ checkedUsuario +'/>';
                    plantillaPanelHtml += '<label class="custom-control-label" for="cambiarRol_usuario' + id + '">'+ accionesUsuarioAdminComunidad.usuario +'</label>';
                plantillaPanelHtml += '</div>';
            plantillaPanelHtml += '</div>'; 

    plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
    // Posibles mensajes de info
        plantillaPanelHtml += '<div>';
            plantillaPanelHtml += '<div id="menssages">';
                plantillaPanelHtml += '<div class="ok"></div>';
                plantillaPanelHtml += '<div class="ko"></div>';
            plantillaPanelHtml += '</div>';
        plantillaPanelHtml += '</div>';
    // Panel de botones para la acci�n
        plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
            plantillaPanelHtml += '<button class="btn btn-primary" onclick="'+ accion +'">' + accionesUsuarioAdminComunidad.cambiarRol + '</button>'            
        plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
plantillaPanelHtml += '</div>';

    // Meter el c�digo de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el c�digo
    $modalDinamicContentPanel.html(plantillaPanelHtml);

}

/**
 * Enviar la nueva petici�n del cambio de rol una vez se ha pulsado sobre el bot�n de "Cambiar rol"
 * @param {any} urlPagina: Url donde se lanzar� la petici�n para cambiar el rol
 * @param {any} id: Identificador de la persona
 * @param {any} rol: Rol actual de la persona. Si es el mismo, no har�a nada
 */
function EnviarAccionCambiarRol(urlPagina, id, rol) {
    var rolNuevo = $('input[name="cambiarRol_' + id + '"]:checked').val();

    if (rolNuevo != rol) {
        MostrarUpdateProgress();

        var dataPost = {
            callback: "Accion_CambiarRol",
            rol: rolNuevo
        }

        GnossPeticionAjax(urlPagina, dataPost, true).done(function (data) {
            // Cambiado mensaje por el nuevo Front
            // CambiarNombreElemento(id + '_CambiarRol', accionesUsuarioAdminComunidad.rolCambiado);
            CambiarTextoAndEliminarAtributos(id + '_CambiarRol', accionesUsuarioAdminComunidad.rolCambiado, ['onclick', 'data-target', 'data-toggle']);
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", true, accionesUsuarioAdminComunidad.rolCambiado);
            // Esperar 1 segundo y cerrar la ventana
            setTimeout(() => {
                $('#modal-container').modal('hide');
            }, 1500)
        }).fail(function (data) {
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, "Error al tratar de cambiar el rol. Por favor, int�ntalo de nuevo m�s tarde.");
        }).always(function (data) {
            OcultarUpdateProgress();
        });        
    }
}

function Readmitir(urlPagina) {
    var dataPost = {
        callback: "Accion_Readmitir"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}

function Bloquear(urlPagina) {
    var dataPost = {
        callback: "Accion_Bloquear"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}

function Desbloquear(urlPagina) {
    var dataPost = {
        callback: "Accion_Desbloquear"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}

function EnviarNewsletter(urlPagina) {
    var dataPost = {
        callback: "Accion_EnviarNewsletter"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}

function NoEnviarNewsletter(urlPagina) {
    var dataPost = {
        callback: "Accion_NoEnviarNewsletter"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}

function CambiarNombreElementoMVC(that, nombre) {
	$(that).html('');
	$(that).parent().html(nombre)
}/**/ 
/*MVC.Registro.js*/ 
﻿function ComprobarDatosRegistroLogeado() {
    var errorCondiciones = ComprobarClausulas();

    if (errorCondiciones.length > 0) {
        crearError(errorCondiciones, '#divKoCondicionesUso', false);
        return true;
    }
    else {
        $('#divKoCondicionesUso').html('');
        return false;
    }
}

function ComprobarDatosRegistro(pEdadMinimaRegistro) {
    var error = "";

    if ((typeof RecogerDatosExtra != 'undefined')) {
        error += RecogerDatosExtra();
    }

    error += AgregarErrorReg(error, ValidarNombrePersona('txtNombre', 'lblNombre'));
    error += AgregarErrorReg(error, ValidarApellidos('txtApellidos', 'lblApellidos'));
    error += ValidarCampoLogin('txtNombreUsuario');
    error += ValidacionesEmail('txtEmail', 'txtEmailTutor');
    /*
    error += AgregarErrorReg(error, ValidarEmailIntroducido('txtEmail', 'lblEmail'));*/
    error += AgregarErrorReg(error, ValidarContrasena('txtContrasenya', '', 'lblContrasenya', '', false));
    error += AgregarErrorReg(error, ValidarCampoNoVacio('txtCargo', 'txtCargo'));

    if (document.getElementById('txtFechaNac') != null) {
        error += AgregarErrorReg(error, ValidarFechaNacimientoMVC($('#txtFechaNac').val(), 'lblFechaNac', '' /*pEdadMinimaRegistro*/));
    }
    if (document.getElementById('#ddlPais') != null) {
        error += AgregarErrorReg(error, ValidarPais('ddlPais', 'lblPais'));
    }
    if (document.getElementById('#txtProvincia') != null) {
        error += AgregarErrorReg(error, ValidarProvincia('txtProvincia', 'lblProvincia', 'ddlProvincia'));
    }
    if (document.getElementById('#txtLocalidad') != null) {
        error += AgregarErrorReg(error, ValidarPoblacionOrg('txtLocalidad', 'lblLocalidad'));
    }
    if (document.getElementById('#ddlSexo') != null) {
        error += AgregarErrorReg(error, ValidarSexo('ddlSexo', 'lblSexo'));
    }

    //Compruebo cláusulas adicionales:
    var errorCondiciones = ComprobarClausulas();

    if (error.length > 0 || errorCondiciones.length > 0) {
        if (error.length > 0) {
            //crearError(error, '#divKodatosUsuario', false);
            $('#divKodatosUsuario').children().addClass('d-block');
            $('#divKodatosUsuario').children().html(error);
        }
        else {
            //$('#divKodatosUsuario').html('');
            $('#divKodatosUsuario').children().removeClass('d-block');
            $('#divKodatosUsuario').children().html('');
        }

        if (errorCondiciones.length > 0) {
            //crearError(errorCondiciones, '#divKoCondicionesUso', false);
            $('#divKoCondicionesUso').children().addClass('d-block');
            $('#divKoCondicionesUso').children().html(errorCondiciones);
        }
        else {
            $('#divKoCondicionesUso').children().removeClass('d-block');
            $('#divKoCondicionesUso').html('')
        }

        return true;
    } else {
        $('#divKodatosUsuario').html('');
        $('#divKoCondicionesUso').html('')

        return false;
    }
}

function ValidarCampoLogin(pTxtLogin) {
    var error = '';
    var textLogin = $('#' + pTxtLogin).val();
    if (textLogin != undefined && textLogin != '') {
        if (textLogin.length > 12) {
            error += '<p> El nombre del usuario no puede exceder de 12 caracteres </p>';
        }
    }
    return error;

}

function ValidacionesEmail(pTxtEmail, pTxtEmailTutor) {
    var error = "";
    var textEmail = $('#' + pTxtEmail).val();
    var textEmailTutor = $('#' + pTxtEmailTutor).val();
    if (textEmailTutor != undefined) {
        if (textEmail == '' && textEmailTutor == '') {
            error += '<p> Debes rellenar uno de los dos campos de Email. </p>';
        }
        else {
            if (ValidacionEmailTutor(pTxtEmailTutor, 'lblEmailTutor')) {
                error += AgregarErrorReg(error, ValidarEmailIntroducido(pTxtEmailTutor, 'lblEmailTutor'));
            }

        }
    }
    else {
        error += AgregarErrorReg(error, ValidarEmailIntroducido(pTxtEmail, 'lblEmail'));
    }

    return error;
}

function ValidacionEmail(pTxtMail, pLblMail) {
    var textEmail = $('#' + pTxtMail).val();
    if (textEmail == '') {
        return false;
    }
    return true;
}

function ValidacionEmailTutor(pTxtMail, pLblMail) {
    var textEmail = $('#' + pTxtMail).val();
    if (textEmail != undefined && textEmail == '') {
        return false;
    }
    return true;
}

function ComprobarClausulas() {
    error = "";
    var listaChecks = '';
    $('#condicionesUso input[type=checkbox]').each(function () {
        var that = $(this);
        if (that.is(':checked')) {
            listaChecks += that.attr('id') + ',';
        }
        that.next().css("color", "");
        if (!that.hasClass("optional")) {
            if (!that.is(':checked')) {
                that.next().css("color", "#E24973");
                if (that.parent().attr('id') == "liClausulaMayorEdad") {
                    error = '<p>' + form.mayorEdad + '</p>';
                } else {
                    error = '<p>' + form.aceptarLegal + '</p>';
                }
            }
        }
    });
    $('#clausulasSelecc').val(listaChecks);
    return error;
}

function MostrarPanelExtra() {
    $('#despleReg').show();
    $('#despleReg .stateShowForm').show();
    $('html,body').animate({ scrollTop: $('#despleReg').offset().top }, 'slow');
    return false;
}


function ComprobarEmailUsuario(pUrlPagina) {
    if (ValidarEmailIntroducido('txtEmail', 'lblEmail') == '') {
        var dataPost = {
            callback: "comprobarEmail",
            email: $('#txtEmail').val()
        }
        $.post(pUrlPagina, dataPost, function (data) {
            ProcesarEmailComprobado(data);
        });
    }
}

/**
 * Validar que el campo del email sea válido. 
 * Si es correcto, se añade una clase "is-valid"
 * Si es incorrecto (vacío) se añade una clase "is-invalid"
 * Añade además un div con un mensaje personalizado o lo oculta si todo es correcto.
 * @param {any} pDatosRecibidos
 */
function ProcesarEmailComprobado(pDatosRecibidos) {
    if ((typeof ProcesarEmailComprobadoPersonalizado != 'undefined')) {
        ProcesarEmailComprobadoPersonalizado(pDatosRecibidos);
    }
    else
    {
        if (pDatosRecibidos == 'KO') {
            if ($('#divKoEmail').length == 0) {
                $('#lblEmail').parent().after('<p id="divKoEmail"></p>');
            }
            var mensaje = '<p>' + form.emailrepetido + '</p>';

            if (typeof (MensajePersonalizado) != 'undefined' && MensajePersonalizado.length > 0) {
                mensaje = mensaje + '<p>' + MensajePersonalizado + '</p>'
            }

            crearError(mensaje, '#divKoEmail');
            //$('#lblEmail').attr('class', 'ko');
            $('#txtEmail').addClass('is-invalid');
            $('#txtEmail').removeClass('is-valid');
            
        } else {
            $('#divKoEmail').html('');
            //$('#lblEmail').attr('class', '');
            $('#txtEmail').addClass('is-valid');
            $('#txtEmail').removeClass('is-invalid');
        }
    }
}

function comprobarCheck(pCheck, pTxtHackID) {
    var num = parseInt($('#' + pTxtHackID).val().substring(0, $('#' + pTxtHackID).val().indexOf('||')));
    if (pCheck.checked) { num--; } else { num++; }
    $('#' + pTxtHackID).val(num + $('#' + pTxtHackID).val().substring($('#' + pTxtHackID).val().indexOf('||')))
}

function ComprobarCampoRegistroMVC(pCampo) {
    if (pCampo == 'NombreUsu') {
        ValidarNombreUsu('txtNombreUsuario', 'lblNombreUsuario');
    }
    else if (pCampo == 'Contra1') {
        ValidarContrasena('txtContrasenya', '', 'lblContrasenya', '', false);
    }
    else if (pCampo == 'Contra2') {
        ValidarContrasena('txtContrasenya', 'txtcContrasenya', 'lblContrasenya', 'lblConfirmarContrasenya', true);
    }
    else if (pCampo == 'Mail') {
        ValidarEmailIntroducido('txtEmail', 'lblEmail');
    }
    else if (pCampo == 'Nombre') {
        ValidarNombrePersona('txtNombre', 'lblNombre');
    }
    else if (pCampo == 'Apellidos') {
        ValidarApellidos('txtApellidos', 'lblApellidos');
    }
    else if (pCampo == 'Provincia') {
        ValidarProvincia('txtProvincia', 'lblProvincia', 'editProvincia');
    }
    else if (pCampo == 'Sexo') {
        ValidarSexo('editSexo', 'lblSexo');
    }
    else if (pCampo == 'CentroEstudios') {
        ValidarCentroEstudios('txtCentroEstudios', 'lbCentroEstudios');
    }
    else if (pCampo == 'AreaEstudios') {
        ValidarAreaEstudios('txtAreaEstudios', 'lbAreaEstudios');
    }
}

function validaFechaDDMMAAAA(fecha) {
    var dtCh = "/";
    var minYear = 1900;
    var maxYear = 2100;
    function isInteger(s) {
        var i;
        for (i = 0; i < s.length; i++) {
            var c = s.charAt(i);
            if (((c < "0") || (c > "9"))) return false;
        }
        return true;
    }
    function stripCharsInBag(s, bag) {
        var i;
        var returnString = "";
        for (i = 0; i < s.length; i++) {
            var c = s.charAt(i);
            if (bag.indexOf(c) == -1) returnString += c;
        }
        return returnString;
    }
    function daysInFebruary(year) {
        return (((year % 4 == 0) && ((!(year % 100 == 0)) || (year % 400 == 0))) ? 29 : 28);
    }
    function DaysArray(n) {
        var daysInMonth = [];
        for (var i = 1; i <= n; i++) {
            daysInMonth[i] = 31
            if (i == 4 || i == 6 || i == 9 || i == 11) { daysInMonth[i] = 30 }
            if (i == 2) { daysInMonth[i] = 29 }
        }
        return daysInMonth;
    }
    function isDate(dtStr) {
        var daysInMonth = DaysArray(12)
        var pos1 = dtStr.indexOf(dtCh)
        var pos2 = dtStr.indexOf(dtCh, pos1 + 1)
        var strDay = dtStr.substring(0, pos1)
        var strMonth = dtStr.substring(pos1 + 1, pos2)
        var strYear = dtStr.substring(pos2 + 1)
        strYr = strYear
        if (strDay.charAt(0) == "0" && strDay.length > 1) strDay = strDay.substring(1)
        if (strMonth.charAt(0) == "0" && strMonth.length > 1) strMonth = strMonth.substring(1)
        for (var i = 1; i <= 3; i++) {
            if (strYr.charAt(0) == "0" && strYr.length > 1) strYr = strYr.substring(1)
        }
        month = parseInt(strMonth)
        day = parseInt(strDay)
        year = parseInt(strYr)
        if (pos1 == -1 || pos2 == -1) {
            return false
        }
        if (strMonth.length < 1 || month < 1 || month > 12) {
            return false
        }
        if (strDay.length < 1 || day < 1 || day > 31 || (month == 2 && day > daysInFebruary(year)) || day > daysInMonth[month]) {
            return false
        }
        if (strYear.length != 4 || year == 0 || year < minYear || year > maxYear) {
            return false
        }
        if (dtStr.indexOf(dtCh, pos2 + 1) != -1 || isInteger(stripCharsInBag(dtStr, dtCh)) == false) {
            return false
        }
        return true
    }
    if (isDate(fecha)) {
        return true;
    } else {
        return false;
    }
}

function ValidarFechaNacimientoMVC(pFechaNacimiento, pLblFechaNacimiento, pEdadMinima) {
    var error = '';
    $('#' + pLblFechaNacimiento).removeClass("ko");
    mayor = false;

    if (pFechaNacimiento != null && validaFechaDDMMAAAA(pFechaNacimiento)) {

        fecha = new Date(pFechaNacimiento.split('/')[2], pFechaNacimiento.split('/')[1], pFechaNacimiento.split('/')[0]);
        hoy = new Date();

        if ((hoy.getFullYear() - fecha.getFullYear()) > pEdadMinima) {
            //Los ha cumplido en algún año anterior
            mayor = true;
        }
        else if ((hoy.getFullYear() - fecha.getFullYear()) == pEdadMinima) {
            if (hoy.getMonth() > fecha.getMonth()) {
                //Los ha cumplido en algún mes anterior
                mayor = true;
            }
                //Los cumple durante el año en el que estamos
            else if (hoy.getMonth() == fecha.getMonth()) {
                //Los cumple durante el mes en el que estamos        
                if (hoy.getDate() >= fecha.getDate()) {
                    //Ya los ha cumplido
                    mayor = true;
                }
            }
        }

        if (!mayor) {
            error += '<p>' + form.mayorEdad.replace('18', pEdadMinima) + '</p>';
            $('#' + pLblFechaNacimiento).attr('class', 'ko');
        }
    }
    else {
        error += '<p>' + form.fechanacincorrecta + '</p>';
        $('#' + pLblFechaNacimiento).attr('class', 'ko');
    }

    return error;
}


function CargarFormLoginRegistro(urlPagina) {
    MostrarUpdateProgress();;
    var dataPost = {
        callback: "cargarFormLogin"
    };
    $.post(urlPagina, dataPost, function (data) {
        $('.box.box01').html(data);
        OcultarUpdateProgress();
    });
}


function ValidarCampoObligatorio(pTxtCampo, pLblCampo) {
    var error = '';
    if (pTxtCampo.val() == '') {
        error += '<p>' + form.camposVacios + '</p>';
        pLblCampo.attr('class', 'ko');
    }
    else {
        pLblCampo.attr('class', '');
    }
    return error;
}

function ValidarCampoSelectObligatorio(pTxtCampo, pLblCampo) {
    var error = '';
    if (pTxtCampo.val() == '00000000-0000-0000-0000-000000000000') {
        error += '<p>' + form.camposVacios + '</p>';
        pLblCampo.attr('class', 'ko');
    }
    else {
        pLblCampo.attr('class', '');
    }
    return error;
}

function ValidarCampoRadioObligatorio(pRadioButons, pLblCampo) {
    var error = '<p>' + form.camposVacios + '</p>';
    pLblCampo.attr('class', 'ko');
    pRadioButons.each(function () {
        if($(this).is(':checked')){
            pLblCampo.attr('class', '');
            error = '';
        }
    });
    return error;
}

function PrepararAutocompletar(inputID, argumentos, proyectoID) {
    $('#' + inputID).unautocomplete().unbind("focus")
    .autocomplete(
        null,
        {
            //servicio: new WS($('#inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
            //metodo: 'AutoCompletarDatoExtraProyectoVirtuoso',
            url: $('#inpt_urlServicioAutocompletar').val() + "/AutoCompletarDatoExtraProyectoVirtuoso",
            type: "POST",
            delay: 0,
            scroll: false,
            selectFirst: false,
            minChars: 1,
            cacheLength: 0,
            extraParams: {
                pProyectoID: proyectoID,
                pOrigen: inputID,
                pArgumentos: function (e) { return ObtenerValoresArgumentos(argumentos); }
            }
        }
    );
}

function ObtenerValoresArgumentos(variables) {
    var valores = '';
    if (typeof (variables) != 'undefined' && variables != '') {
        var filtros = variables.split('|')
        if (filtros.length > 0 && filtros[0] != '') {
            for (var i = 0; i < filtros.length; i++) {
                valores += $('#' + filtros[i]).val() + '|';
            }
        }
    }
    return valores;
}/**/ 
/*MVC.Comun.js*/ 
﻿function CargarAutocompletarGrupos(pTxtFiltroID, pTxtValSeleccID, pIdentidadID, pIdentidadMyGnossID, pIdentidadOrgID, pProyectoID, pListaUrlsAutocompletar) {
	$('#' + pTxtFiltroID).autocomplete(null, {
		//servicio: new WS(pListaUrlsAutocompletar, WSDataType.jsonp),
        //metodo: 'AutoCompletarGruposInvitaciones',
        url: $('input.inpt_urlServicioAutocompletar').val() + "/AutoCompletarGruposInvitaciones",
        type: "POST",
		delay: 0,
		multiple: true,
		scroll: false,
		selectFirst: false,
		minChars: 1,
		width: 300,
		cacheLength: 0,
		NoPintarSeleccionado: true,
		txtValoresSeleccID: pTxtValSeleccID,
		extraParams: {
			identidad: pIdentidadID,
			identidadMyGnoss: pIdentidadMyGnossID,
			identidadOrg: pIdentidadOrgID,
			proyecto: pProyectoID,
		}
	});
}

function CargarAutocompletarLectoresEditoresPorBaseRecursos(control, baseRecursosID, personaID, pTxtValSeleccID, pListaUrlsAutocompletar, panelContenedorID, panResultadosID, txtHackID) {
	control.unautocomplete().autocomplete(
		null,
		{
			servicio: new WS(pListaUrlsAutocompletar, WSDataType.jsonp),
			metodo: 'AutoCompletarLectoresEditoresPorBaseRecursos',
			delay: 0,
			multiple: true,
			scroll: false,
			selectFirst: false,
			minChars: 1,
			width: 300,
			cacheLength: 0,
			NoPintarSeleccionado: true,
			txtValoresSeleccID: pTxtValSeleccID,
			extraParams: {
				baserecursos: baseRecursosID,
				persona: personaID
			}
		}
	);
	control.result(function (event, data, formatted) {
		seleccionarAutocompletarMVC(data[0], data[1], control, panelContenedorID, panResultadosID, txtHackID);
	});
}

/* Seleccionar & eliminar Autocompletar*/
function seleccionarAutocompletarMVC(nombre, identidad, txtFiltro, panelContenedorID, panResultadosID, txtHackID) {
	txtFiltro.val('');

	$('#selector').css('display', 'none');

	var contenedor = $('#' + panelContenedorID).find('#' + panResultadosID);

	if (contenedor.html().trim().indexOf('<ul') == 0) {
		contenedor.html(contenedor.html().replace('<ul class=\"icoEliminar\">', '').replace('</ul>', ''));
	}
	contenedor.html('<ul class=\"icoEliminar\">' + contenedor.html() + '<li>' + nombre + '<a class="remove" onclick="javascript:eliminarAutocompletarMVC(this,\'' + identidad + '\',\'' + panelContenedorID + '\',\'' + panResultadosID + '\',\'' + txtHackID + '\');">' + textoRecursos.Eliminar + '</a></li>' + '</ul>');

	var txtHack = $('#' + panelContenedorID).find('#' + txtHackID);
	txtHack.val(txtHack.val() + "," + identidad);

	contenedor.css('display', '');
}

function eliminarAutocompletarMVC(that, identidad, panelContenedorID, panResultadosID, txtHackID) {
	$(that).parent().remove();

	var txtHack = $('#' + panelContenedorID).find('#' + txtHackID);
	txtHack.val(txtHack.val().replace(',' + identidad, ''));

	if (txtHack.val() == "") {
		var contenedor = $('#' + panelContenedorID).find('#' + panResultadosID);
		contenedor.css('display', 'none');
	}
}


/*-------------------------------------------------------------------------------------------------*/


function ActividadReciente_MostrarMas(pUrlLoadMoreActivity, pUrlLoadActions, pTipoActividad, pNumPeticion, idPanel, pComponentKey, pProfileKey)
{
	MostrarUpdateProgress();
	var datosPost = {
	    NumPeticion: pNumPeticion,
	    TypeActivity: pTipoActividad,
	    ComponentKey: pComponentKey,
	    ProfileKey: pProfileKey
	};

	$.post(pUrlLoadMoreActivity, datosPost, function (data) {
		$("#actividadReciente_" + idPanel).replaceWith(data);
		OcultarUpdateProgress();
		ObtenerAccionesListadoMVC(pUrlLoadActions);
	});
}

//function AgregarFiltroPerfil(pUrlPagina, pCallBack, idPanel) {
//    MostrarUpdateProgress();
//    var datosPost = {
//        callback: pCallBack
//    };

//    $.post(pUrlPagina, datosPost, function (data) {
//        $("#actividadReciente_" + idPanel).replaceWith(data);
//        OcultarUpdateProgress();
//    });
//}

//Métodos para las acciones de los listados de las búsquedas

var ObteniendoAcciones = false;
function ObtenerAccionesListadoMVC(pUrlPagina) {
    if (!ObteniendoAcciones) {
        ObteniendoAcciones = true;
        var resources = $('.resource-list .resource');
        var idPanelesAcciones = '';
        var numDoc = 0;
        resources.each(function () {
            var recurso = $(this);
            var accion = recurso.find('.group.acciones.noGridView');
            if (accion.length == 1 && accion.attr('id') != null) {
                accion.attr('id', accion.attr('id') + '_' + numDoc);
                idPanelesAcciones += accion.attr('id') + ',';
                numDoc++;
            }
            var accionesusuario = recurso.find('.group.accionesusuario.noGridView');
            if (accionesusuario.length == 1 && accionesusuario.attr('id') != null) {
                accionesusuario.attr('id', accionesusuario.attr('id') + '_' + numDoc);
                idPanelesAcciones += accionesusuario.attr('id') + ',';
                numDoc++;
            }
        });

        if (idPanelesAcciones != '') {
            try {
                var datosPost = {
                    callback: "CargarListaAccionesRecursos",
                    listaRecursos: idPanelesAcciones
                };

                $.post(pUrlPagina, datosPost, function (data) {
                    ObteniendoAcciones = false;
                    var paneles = idPanelesAcciones.split(',')
                    if (data != "") {
                        for (var i in data) {
                            if (data[i].updateTargetId.indexOf("AccionesRecursoListado_") == 0) {
                                var docID = data[i].updateTargetId.substr("AccionesRecursoListado_".length, 36);
                                var proyID = data[i].updateTargetId.substr(("AccionesRecursoListado_" + docID).length, 36);

                                var panel = "";

                                $.each(idPanelesAcciones.split(','), function (i, val) {

                                    if (val.indexOf("accionesListado_" + docID) == 0) {
                                        if (val.indexOf(proyID) > 0) {
                                            panel = val;
                                        }
                                        else if (panel == "") {
                                            panel = val;
                                        }
                                    }
                                });

                                if (typeof (PintarAccionesRecursoPersonalizado) != "undefined")
                                {
                                    PintarAccionesRecursoPersonalizado($('#' + panel), data[i].html);
                                }
                                else
                                {
                                    $('#' + panel).replaceWith(data[i].html);
                                }
                            }
                        }
                    }
                });
            }
            catch (ex) { }
        }
        else { ObteniendoAcciones = false; }
    }
}

function DesplegarImgMasMVC(pBoton, pPanel) {
	var $boton = $(pBoton);
	var $panel = $(pPanel);
	var $img = $boton.find('img');
	if ($img.length == 0) {
		$img = $boton;
	}
	var source = $img.attr('src');
	if ($img.attr('alt') == '+') {
		$panel.show();
		$img.attr({ alt: '-', src: source.replace('Mas', 'Menos') });
	} else if ($img.attr('alt') == '-') {
		$panel.hide()
		$img.attr({ alt: '+', src: source.replace('Menos', 'Mas') });
	}

	return false;
}

function paginadorGadget_Siguiente(urlPagina, gadgetID) {
    var botonSiguiente = $('#btnSiguiente_' + gadgetID)
    if (!botonSiguiente.hasClass("desactivado")) {
        var botonAnterior = $('#btnAnterior_' + gadgetID)
        botonAnterior.removeClass("desactivado");
        var paneles = $('#' + gadgetID).find('.contexto.resource-list');
        var panelActivo = $('#' + gadgetID).find('.contexto.resource-list.activo');

        var panelSiguiente = panelActivo.next();

        panelActivo.hide();
        panelSiguiente.show();
        panelActivo.removeClass("activo");
        panelSiguiente.addClass("activo");

        var ultimoPanel = panelSiguiente.next('.contexto.resource-list').length == 0 || panelSiguiente.next('.contexto.resource-list').is(":empty");
        if (ultimoPanel) {
            botonSiguiente.addClass("desactivado");
            if (!panelSiguiente.next('.contexto.resource-list').is(":empty")) {
                panelSiguiente.after("<div class=\"contexto resource-list\" style=\"display:none\"></div>");

                var datapost = {
                    callback: "paginadorGadget",
                    gadgetid: gadgetID,
                    numPagina: paneles.length + 1,
                }

                $.post(urlPagina, datapost, function (data) {
                    var htmlRecursos = "";
                    for (var i in data) {
                        if (data[i].updateTargetId.indexOf("FichaRecursoMini_") == 0) {
                            htmlRecursos += data[i].html;
                        }
                    }
                    if (htmlRecursos != "") {
                        botonSiguiente.removeClass("desactivado");
                        panelSiguiente.next().remove();
                        panelSiguiente.after("<div class=\"contexto resource-list\" style=\"display:none\">" + htmlRecursos + "</div>");
                    }
                    CompletadaCargaContextos();
                });
            }
        } else
        {
            CompletadaCargaContextos();
        }
    }
}

function paginadorGadget_Anterior(urlPagina, gadgetID) {
    var botonAnterior = $('#btnAnterior_' + gadgetID)
    if (!botonAnterior.hasClass("desactivado")) {
        var botonSiguiente = $('#btnSiguiente_' + gadgetID)
        botonSiguiente.removeClass("desactivado");
        var paneles = $('#' + gadgetID).find('.contexto.resource-list');
        var panelActivo = $('#' + gadgetID).find('.contexto.resource-list.activo');

        var panelAnterior = panelActivo.prev();

        panelActivo.hide();
        panelAnterior.show();
        panelActivo.removeClass("activo");
        panelAnterior.addClass("activo");

        var ultimoPanel = panelAnterior.prev('.contexto.resource-list').length == 0;
        if (ultimoPanel) {
            botonAnterior.addClass("desactivado");
        }
        CompletadaCargaContextos();
    }
}

function paginadorVinculados_Siguiente(urlPagina) {
    var botonSiguiente = $('#btnSiguiente_vinc');
    if (!botonSiguiente.hasClass("desactivado")) {
        var botonAnterior = $('#btnAnterior_vinc');
        botonAnterior.removeClass("desactivado");
        var paneles = $('#panVinculadosInt').find('.resource-list.vinculados');
        var panelActivo = $('#panVinculadosInt').find('.resource-list.vinculados.activo');

        var panelSiguiente = panelActivo.next();

        panelActivo.hide();
        panelSiguiente.show();
        panelActivo.removeClass("activo");
        panelSiguiente.addClass("activo");

        var ultimoPanel = panelSiguiente.next().length == 0 || panelSiguiente.next().is(":empty");
        if (ultimoPanel) {
            botonSiguiente.addClass("desactivado");
            if (!panelSiguiente.next().is(":empty")) {
                panelSiguiente.after("<div class=\"resource-list vinculados\" style=\"display:none\"></div>");
                var datapost = {
                    page: paneles.length + 1
                }
                $.post(urlPagina + "/load-linked-resources", datapost, function (data) {
                    var htmlNuevo = $('<div/>').html(data).find('.resource-list.vinculados');
                    if (htmlNuevo.length) {
                        botonSiguiente.removeClass("desactivado");
                        panelSiguiente.next().remove();
                        panelSiguiente.after("<div class=\" resource-list vinculados\" style=\"display:none\">" + htmlNuevo.html() + "</div>");
                    }
                    CompletadaCargaContextos();
                });
            }
        } else {
            CompletadaCargaContextos();
        }
    }
}

function paginadorVinculados_Anterior(urlPagina) {
    var botonAnterior = $('#btnAnterior_vinc');
    if (!botonAnterior.hasClass("desactivado")) {
        var botonSiguiente = $('#btnSiguiente_vinc')
        botonSiguiente.removeClass("desactivado");
        var paneles = $('#panVinculadosInt').find('.resource-list.vinculados');
        var panelActivo = $('#panVinculadosInt').find('.resource-list.vinculados.activo');

        var panelAnterior = panelActivo.prev();

        panelActivo.hide();
        panelAnterior.show();
        panelActivo.removeClass("activo");
        panelAnterior.addClass("activo");

        var ultimoPanel = panelAnterior.prev('.resource-list.vinculados').length == 0;
        if (ultimoPanel) {
            botonAnterior.addClass("desactivado");
        }
        CompletadaCargaContextos();
    }
}

function GnossPeticionAjax(url, parametros, traerJson, redirectActive = true)
{
    var defr = $.Deferred();

    var esFormData = parametros instanceof FormData;

    $.ajax({
        url: url,
        type: "POST",
        headers: {
            Accept: traerJson ? 'application/json' : '*/*'
        },
        processData: !esFormData,
        contentType: esFormData ? false : 'application/x-www-form-urlencoded',
        data: parametros,
        xhr: function () {
            var xhr = new window.XMLHttpRequest();
            // Handle progress
            //Upload progress
            xhr.upload.addEventListener("progress", function (evt) {
                if (evt.lengthComputable) {
                    var percentComplete = evt.loaded / evt.total;
                    //Do something with upload progress
                    defr.notify(Math.round(percentComplete * 100));
                }
            }, false);

            return xhr;
        }
    }).done(function (response) {
        if (response == null || response.Status == undefined) {
            defr.resolve(response);
        }
        if (response.Status == "NOLOGIN") {
            //Hacer una peticion a la web, que nos devuelva la url del servicio de login + token
            //Hacer la peticion al servicio de login a recuperar la sesion
            //Si no estamos conectados, mostrar un panel de login
            //Si estamos conectados, re-llamar a esta funcion
            defr.reject("invitado");
        }
        else if (response.Status == "OK") {
            if (response.UrlRedirect != null)
            {
                if(redirectActive){ location.href = response.UrlRedirect; }
                else{ defr.resolve(response.UrlRedirect); }
            }
            else if (response.Html != null) {
                defr.resolve(response.Html);
            }
            else {
                defr.resolve(response.Message);
            }
        }
        else if (response.Status == "Error") {
            defr.reject(response.Message);
        }
    }).fail(function (er) {
        //Comprobar el error
        var newtWorkError = er.readyState == 0;// && er.statusText == "error";
        if (newtWorkError) {
            defr.reject("NETWORKERROR");
        }
        else {
            defr.reject(er.statusText);
        }
    });

    return defr;
}

var comportamientoNetworkError = {
    intentarRedirigirFichaRecurso: function (url, funcion) {
        if (documentoID != null && documentoID != '') {
            url += "?documentoID=" + documentoID;
        }
        this.urlObtenerFicha = url;
        this.funcionEjecutar = funcion;
        this.cont = 0;
        var that = this;
        //setTimeout(function () { that.obtenerFichaRecurso(); }, 1000);
        that.obtenerFichaRecurso();
    },
    obtenerFichaRecurso: function () {
        var that = this;
        $.ajax({
            url: that.urlObtenerFicha,
            method: 'GET',
            async: false,
            success: function (data) {
                //la ficha existe, redirigir
                if (data != '') {
                    document.location.href = data;
                }
                else {
                    if (that.cont < 10) {
                        that.cont++;
                        setTimeout(function () { that.obtenerFichaRecurso(); }, 1000);
                    }
                    else {
                        //mostrar error
                        that.obtenerMensajeError();
                    }
                }
            },
            error: function (data) {
                //la ficha no existe
                if (data.readyState == 0 && that.cont < 10) {
                    that.cont++;
                    setTimeout(function () { that.obtenerFichaRecurso(); }, 1000);
                }
                else {
                    //mostrar error
                    that.obtenerMensajeError();
                }
            }
        });
    },
    obtenerMensajeError: function () {
        this.funcionEjecutar('Has perdido la conexión y no se ha podido recuperar el recurso. Comprueba tu conexión a internet y verifica que tus cambios se han guardado correctamente ');
    }
}

function guidGenerator() {
    var S4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

String.prototype.EndsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

String.prototype.StartsWith = function (searchString, position) {
    position = position || 0;
    return this.lastIndexOf(searchString, position) === position;
};

//Añadimos esta funcion que antes la añadia .Net
Date.prototype.format = function (format) {
    var date = this,
        day = date.getDate(),
        month = date.getMonth() + 1,
        year = date.getFullYear(),
        hours = date.getHours(),
        minutes = date.getMinutes(),
        seconds = date.getSeconds();

    if (!format) {
        format = "MM/dd/yyyy";
    }

    format = format.replace("MM", month.toString().replace(/^(\d)$/, '0$1'));

    if (format.indexOf("yyyy") > -1) {
        format = format.replace("yyyy", year.toString());
    } else if (format.indexOf("yy") > -1) {
        format = format.replace("yy", year.toString().substr(2, 2));
    }

    format = format.replace("dd", day.toString().replace(/^(\d)$/, '0$1'));

    if (format.indexOf("t") > -1) {
        if (hours > 11) {
            format = format.replace("t", "pm");
        } else {
            format = format.replace("t", "am");
        }
    }

    if (format.indexOf("HH") > -1) {
        format = format.replace("HH", hours.toString().replace(/^(\d)$/, '0$1'));
    }

    if (format.indexOf("hh") > -1) {
        if (hours > 12) {
            hours -= 12;
        }

        if (hours === 0) {
            hours = 12;
        }
        format = format.replace("hh", hours.toString().replace(/^(\d)$/, '0$1'));
    }

    if (format.indexOf("mm") > -1) {
        format = format.replace("mm", minutes.toString().replace(/^(\d)$/, '0$1'));
    }

    if (format.indexOf("ss") > -1) {
        format = format.replace("ss", seconds.toString().replace(/^(\d)$/, '0$1'));
    }

    return format;
};

/*Tesauros*/
function MVCDesplegarTreeView(pImagen) {

    if (typeof MarcarDesplegarTreeView == "function")
    {
        MarcarDesplegarTreeView(pImagen);
    }

    var imagen = $(pImagen);
    if (pImagen.src.indexOf('verMas') > 0) {
        pImagen.src = pImagen.src.replace('verMas', 'verMenos');
    }
    else {
        pImagen.src = pImagen.src.replace('verMenos', 'verMas');
    }
    MVCDesplegarPanel($(pImagen).parent().children('.panHijos'));
}

function MVCDesplegarPanel(pPanel) {
    if (pPanel.css("display") == 'none') {
        pPanel.show();
    }
    else {
        pPanel.hide();
    }
    return false;
}

/**
 * Método para filtrar elementos. En este caso, en la lista de Categor�as, modo "Lista" de la ficha Recurso.
 * Al teclear en el input, filtrará (ocultará) los elementos que no correspondan con el texto de la búsqueda
 * @param {any} txt
 * @param {any} panDesplID
 */
function MVCFiltrarListaSelCat(txt, panDesplID) {
    cadena = $(txt).val();    
    var itemsListado = $('#' + panDesplID).find('#divSelCatLista').children('div');
    itemsListado.show();
    itemsListado.each(function () {
        //var nombreCat = $(this).find('span label').text();
        // Cambia al ser nuevo front - Nuevo estilo de cada item de categorías        
        var nombreCat = $(this).find('div div label').text();               
        if (nombreCat.toLowerCase().indexOf(cadena.toLowerCase()) < 0) {
            $(this).hide();
        }
    });
}

/**
 * Método para filtrar elementos. En este caso, en la lista de Categorías, modo "Árbol" de la ficha Recurso.
 * Al teclear en el input, filtrará (ocultará) los elementos que no correspondan con el texto de la búsqueda
 * @param {any} txt
 * @param {any} panDesplID
 */
function MVCFiltrarListaSelCatArbol(txt, id) {
    var cadena = $(txt).val();
    var itemsListado = $('#' + id).find('.divTesArbol .categoria-wrap');
    itemsListado.show();
    if (cadena == '') {
        $('.boton-desplegar').removeClass('mostrar-hijos');
    } else {
        itemsListado.each(function () {
            var boton = $(this).find('.boton-desplegar');
            boton.removeClass('mostrar-hijos');
            var nombreCat = $(this).find('.categoria label').text();
            boton.trigger('click');
            if (nombreCat.toLowerCase().indexOf(cadena.toLowerCase()) < 0) {
                $(this).hide();
            }

            var categoriaHijo = $(this).find('.panHijos').children('.categoria-wrap');
            categoriaHijo.each(function () {
                var nombreCatHijo = $(this).find('.categoria label').text();
                if (nombreCatHijo.toLowerCase().indexOf(cadena.toLowerCase()) < 0) {
                    $(this).hide();
                }
            });
        });
    }
}


function MVCMarcarTodosElementosCat(pCheck, panDesplID) {
    var txtSeleccionados = "";

    var itemsListado = $('#' + panDesplID).find('#divSelCatLista').children('div');

    var marcarChecks = $(pCheck).is(':checked');

    itemsListado.each(function () {
        var claseInput = $(this).find('span').attr('class');

        $('#' + panDesplID).find('#divSelCatTesauro').find('span.' + claseInput + ' input').prop('checked', marcarChecks);
        $('#' + panDesplID).find('#divSelCatLista').find('span.' + claseInput + ' input').prop('checked', marcarChecks);

        if (marcarChecks) {
            txtSeleccionados += claseInput + ",";
        }
    });

    $('#' + panDesplID).find('#txtSeleccionados').val(txtSeleccionados);
}

/**
 * Acci�n que se ejecuta cuando se selecciona un item de categor�a para ser seleccionado e introducido en un input vac�o para as� tener control sobre el elemento que se ha seleccionado.
 * @param {any} pCheck: Ser� el input check que se ha seleccionado.
 * @param {any} panDesplID: El id del panel donde se encontrar� el input vac�o.
 * @param {any} hackedInputId: El id del inputId que estar� oculto que se utilizar� para establecer opciones que puedan servir para mandar al servidor. Si no se pasa nada, se har� caso al panDesplID. En caso contrario, se acceder� al panDesplIDSecundario para buscar ese input
 */
function MVCMarcarElementoSelCat(pCheck, panDesplID, hackedInputId = undefined) {
    // Debido al nuevo Front - No se accede al padre sino al ID del propio Input
    //var claseInput = $(pCheck).parent().attr("class");
    var claseInput = $(pCheck).attr("data-item");    

    var txtSeleccionados = '';

    // Observar un panel u otro
    if (hackedInputId == undefined) {
        txtSeleccionados = $('#' + panDesplID).find('#txtSeleccionados');
    } else {
        txtSeleccionados = $('#' + panDesplID).find('#' + hackedInputId);
    }

    if ($(pCheck).is(':checked')) {
        txtSeleccionados.val(txtSeleccionados.val() + claseInput + ',');
    }
    else {
        txtSeleccionados.val(txtSeleccionados.val().replace(claseInput + ',', ''));
    }

    $('#' + panDesplID).find('#divSelCatTesauro').find('span.' + claseInput + ' input').prop('checked', $(pCheck).is(':checked'));
    $('#' + panDesplID).find('#divSelCatLista').find('span.' + claseInput + ' input').prop('checked', $(pCheck).is(':checked'));

    MVCComprobarChecks(panDesplID);
}

function MVCComprobarChecks(panDesplID) {
    var itemsListado = $('#' + panDesplID).find('#divSelCatLista').find('div span input');

    var numItemsChecked = $('#' + panDesplID).find('#divSelCatLista').find('div span input:checked').length;
    if (numItemsChecked == itemsListado.length) {
        $('#chkSeleccionarTodos').prop('checked', true);
    }
    else {
        $('#chkSeleccionarTodos').prop('checked', false);
    }
}
/*Fin Tesauros*/
var inicializadoSubirRecurso = false;

function InicializarSubirRecursoExt(urlPaginaSubir) {
    if (inicializadoSubirRecurso) { return; }

    $("#linkNota").click(function () {
        MostrarUpdateProgress();

        GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 0 }, true).always(function () {
            OcultarUpdateProgress();
        });
    });

    $("#lbSiguienteURL").click(function () { validarUrlExt(urlPaginaSubir, false); });

    $("#lbSiguienteReferencia").click(function () { validarDocFisicoExt(urlPaginaSubir, false); });

    $("#lbSiguienteWiki").click(function () {
        var url = document.getElementById("txtArticuloWiki").value;

        if (url == '') {
            return false;
        }

        MostrarUpdateProgress();

        GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 3, Link: url }, true).fail(function (data) {
            $('#pWikiError').remove();
            $('#divWiki fieldset').append('<div id="pWikiError" class="ko" style="display:block;"><p>' + data + '</p></div>');
        }).always(function () {
            OcultarUpdateProgress();
        });
    });

    $("#lbSiguienteArchivo").click(function () { validarDocAdjuntarExt(urlPaginaSubir, false, null); });

    inicializadoSubirRecurso = true;
}

function mostrarPanSubirRecurso(nombre) {

    if (typeof (InicioMostrarPanSubirRecurso) != "undefined") {
        InicioMostrarPanSubirRecurso(nombre);
    }

    if (document.getElementById('panEnlRep') != null) {
        $('#panEnlRep').remove();
    }

    if (document.getElementById("divArchivo") != null) {
        document.getElementById("divArchivo").style.display = "none";
    }
    if (document.getElementById("divReferenciaDoc") != null) {
        document.getElementById("divReferenciaDoc").style.display = "none";
    }
    if (document.getElementById("divURL") != null) {
        document.getElementById("divURL").style.display = "none";
    }
    if (document.getElementById("divBrightcove") != null) {
        document.getElementById("divBrightcove").style.display = "none";
    }
    if (document.getElementById("divTOP") != null) {
        document.getElementById("divTOP").style.display = "none";
    }
    if (document.getElementById("divWiki") != null) {
        document.getElementById("divWiki").style.display = "none";
    }

    switch (nombre) {
        case 'Archivo':
            document.getElementById("divArchivo").style.display = "block";
            break;
        case 'Referencia':
            document.getElementById("divReferenciaDoc").style.display = "block";
            break;
        case 'URL':
            document.getElementById("divURL").style.display = "block";
            break;
        case 'Brightcove':
            document.getElementById("divBrightcove").style.display = "block";
            var srcAux = $('#iframeBrightcove').attr('srcAux');
            var onloadAux = $('#iframeBrightcove').attr('onloadAux');
            if (srcAux != null && srcAux != "") {
                $('#iframeBrightcove').attr('src', srcAux);
                $('#iframeBrightcove').removeAttr('srcAux');
            }
            if (onloadAux != null && onloadAux != "") {
                $('#iframeBrightcove').attr('onload', onloadAux);
                $('#iframeBrightcove').removeAttr('onloadAux');
            }
            break;
        case 'TOP':
            document.getElementById("divTOP").style.display = "block";
            var srcAux = $('#iframeTOP').attr('srcAux');
            var onloadAux = $('#iframeTOP').attr('onloadAux');
            if (srcAux != null && srcAux != "") {
                $('#iframeTOP').attr('src', srcAux);
                $('#iframeTOP').removeAttr('srcAux');
            }
            if (onloadAux != null && onloadAux != "") {
                $('#iframeTOP').attr('onload', onloadAux);
                $('#iframeTOP').removeAttr('onloadAux');
            }
            break;
        case 'Wiki':
            document.getElementById("divWiki").style.display = "block";
            break;
    }
}

/**
 * /**
 * Acci�n que se ejecuta para comprobar que el Link adjunto es correcto. Es el paso previo que se realiza antes de poder crear un recurso de tipo "Enlace externo" 
 * @param {any} urlPaginaSubir: Url o enlace escrito por el usuario
 * @param {any} omitirCompRep
 */
function validarUrlExt(urlPaginaSubir, omitirCompRep) {
    try //Intentamos validar la url
    {
        var lblUrl = document.getElementById("lblIntroducirURL");
        var url = document.getElementById("txtURLDoc");

        // Panel donde se mostrar�n posibles errores en la subida del un recurso de tipo Enlace Externo (Nuevo Front)
        const panelResourceFileErrorMessage = $('#modal-add-resource-link-messages-wrapper .ko');
        // Vaciar el panel de posibles errores anteriores y ocultarlo
        panelResourceFileErrorMessage.empty().hide(); 

        var regexURL = /^((([hH][tT][tT][pP][sS]?|[fF][tT][pP])\:\/\/)?([\w\.\-]+(\:[\w\.\&%\$\-]+)*@)?((([^\s\(\)\<\>\\\"\.\[\]\,@;:]+)(\.[^\s\(\)\<\>\\\"\.\[\]\,@;:]+)*(\.[a-zA-Z]{2,4}))|((([01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}([01]?\d{1,2}|2[0-4]\d|25[0-5])))(\b\:(6553[0-5]|655[0-2]\d|65[0-4]\d{2}|6[0-4]\d{3}|[1-5]\d{4}|[1-9]\d{0,3}|0)\b)?((\/[^\/][\w\.\,\?\'\\\/\+&%\$#\=~_\-@:]*)*[^\.\,\?\"\'\(\)\[\]!;<>{}\s\x7F-\xFF])?)$/i;
        if (url.value.length > 0 && url.value.match(regexURL)) {
            MostrarUpdateProgress();

            GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 1, Link: url.value, SkipRepeat: omitirCompRep }, true).done(function (data) {
                // No ocultarlo para reintentar de nuevo
                //$('#divURL').hide();
                // Cambio por nuevo Front
                //$('#liURL').append(data);                
                // A�adir el mensaje y mostrarlo
                panelResourceFileErrorMessage.append(data).show();                                

            }).fail(function (data) {
                document.getElementById("lblIntroducirURL").style.color = "Red";
            }).always(function () {
                OcultarUpdateProgress();
            });

            return true;
        }
        else {
            lblUrl.style.color = "Red";
            return false;
        }
    } catch (e) {
        //Error provocado porque no existe el elemento url (no es un recurso con url)
    }
    return true;
}


function validarDocFisicoExt(urlPaginaSubir, omitirCompRep) {
    try //Intentamos validar un documento físico
    {
        var doc = document.getElementById("txtUbicacionDoc");

        if (doc.value.length > 0) {
            MostrarUpdateProgress();

            GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 2, Link: doc.value, SkipRepeat: omitirCompRep }, true).done(function (data) {
                $('#divReferenciaDoc').hide();
                $('#liReferenciaDoc').append(data);
            }).fail(function (data) {
                document.getElementById("lblDescribaUbic").style.color = "Red";
            }).always(function () {
                OcultarUpdateProgress();
            });

            return true;
        } else {
            document.getElementById("lblDescribaUbic").style.color = 'Red';
            return false;
        }
    } catch (e) {
        //Error provado porque no existe el elemento doc (no es un elemento físico)
    }
}

/**
 * Inicialización de Input de tipo "File" para que coja el nombre del fichero seleccionado (Bootstrap)
 * ---------------
 */
$(document).on('change', '.custom-file-input', function (event) {
    $(this).next('.custom-file-label').html(event.target.files[0].name);
});

/**
 * Acci�n que se ejecuta para comprobar que el fichero adjunto es correcto. Es el paso previo que se realiza antes de poder crear un recurso de tipo "Adjunto"
 * @param {any} urlPaginaSubir: P�gina a la que se redireccionar� para completar la creaci�n del recurso de tipo "Adjunto"
 * @param {Boolean} omitirCompRep
 * @param {Boolean} extraArchivo
 */
function validarDocAdjuntarExt(urlPaginaSubir, omitirCompRep, extraArchivo) {
    try //Intentamos validar que haya un documento para adjuntar
    {
        var lblDoc = document.getElementById("lblSelecionaUnDoc");
        var doc = document.getElementById("fuExaminar");
        // Panel donde se mostrar�n posibles errores en la subida del archivo (Nuevo Front)
        const panelResourceFileErrorMessage = $('#modal-add-resource-file-messages-wrapper .ko');
        // Vaciar el panel de posibles errores anteriores y ocultarlo
        panelResourceFileErrorMessage.empty().hide(); 

        if (omitirCompRep) {
            MostrarUpdateProgress();

            GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 4, ExtraFile: extraArchivo, SkipRepeat: omitirCompRep }, true).fail(function (data) {
                document.getElementById("lblDescribaUbic").style.color = "Red";
            }).always(function () {
                OcultarUpdateProgress();
            });
        }
        else if (doc.value.length > 0) {
            var data = new FormData();
            var files = $("#fuExaminar").get(0).files;
            if (files.length > 0) {
                MostrarUpdateProgressTime(0);

                var bar = $('#progressBarArchivo .bar');
                var percent = $('#progressBarArchivo .percent');

                var percentVal = '0%';
                bar.width(percentVal);
                percent.html(percentVal);

                $('#lbSiguienteArchivo').hide();
                $('#progressBarArchivo').show();

                data.append("TypeResourceSelected", 4);
                data.append("File", files[0]);
                data.append("FileName", files[0].name);

                GnossPeticionAjax(urlPaginaSubir + '/selectresource', data, true).done(function (data) {
                    // No ocultar nada para reitentar el env�o o subida de un fichero
                    //$('#divArchivo').hide();
                    // Cambio por nuevo Front
                    // $('#liArchivo').append(data);
                    // A�ado el error y muestro el div
                    panelResourceFileErrorMessage.append(data).show();
                    // Mostrar de nuevo bot�n de "Siguiente" para reintentar env�o de fichero
                    $('#lbSiguienteArchivo').show();
                    
                }).fail(function (data) {
                    document.getElementById("lblSelecionaUnDoc").style.color = "Red";
                    $('#pArchError').remove();
                    var mensajeError = data;
                    if (data == "NETWORKERROR") {
                        mensajeError = 'Has perdido la conexión. Comprueba tu conexión a internet e intenta adjuntar el recurso de nuevo.';
                    }
                    // Cambiado por nuevo front
                    // $('#divArchivo fieldset').append('<div id="pArchError" class="ko" style="display:block;"><p>' + mensajeError + '</p></div>');
                    panelResourceFileErrorMessage.append('<p>' + mensajeError + '</p>').show();
                    $('#lbSiguienteArchivo').show();
                    $('#progressBarArchivo').hide();
                }).progress(function (progreso) {
                    var percentVal = progreso + '%';
                    bar.width(percentVal);
                    percent.html(percentVal);
                }).always(function () {
                    OcultarUpdateProgress();
                });
            }
            return true;
        } else {
            lblDoc.style.color = 'Red';
            return false;
        }
    } catch (e) {
        //Error provado porque no existe el elemento doc (no es un recurso nuevo archivo)
    }
}

function comprobarSubidaBrightcove() {
    var iframe = document.getElementById("iframeBrightcove");
    var doc;
    if (!window.opera && document.all && document.getElementById) {
        doc = iframe.contentWindow.document;
    } else if (document.getElementById) {
        doc = iframe.contentDocument;
    }
    try {
        if (doc.getElementById("lblVideoBrightcoveOK") != null) {
            MostrarUpdateProgress();
            location.href = URLVideo;
        }
        if (doc.getElementById("lblAudioBrightcoveOK") != null) {
            MostrarUpdateProgress();
            location.href = URLAudio;
        }
    } catch (error) { }
}

function comprobarSubidaTOP() {
    var iframe = document.getElementById("iframeTOP");
    var doc;
    if (!window.opera && document.all && document.getElementById) {
        doc = iframe.contentWindow.document;
    } else if (document.getElementById) {
        doc = iframe.contentDocument;
    }
    try {
        if (doc.getElementById("lblVideoTOPOK") != null) {
            MostrarUpdateProgress();
            location.href = URLVideo;
        }
        if (doc.getElementById("lblAudioTOPOK") != null) {
            MostrarUpdateProgress();
            location.href = URLAudio;
        }
    } catch (error) { }
}/**/ 
/*MVC.ComAdmin.js*/ 
﻿function FormularioSubidaEstilos() {
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
}/**/ 
