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
