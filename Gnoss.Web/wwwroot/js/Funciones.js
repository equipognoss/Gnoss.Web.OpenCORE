startList = function() {
    if (document.all&&document.getElementById) {
      navRoot = document.getElementById("menuPrincipal");
      if(navRoot != null)
      {
          for (i=0; i<navRoot.childNodes.length; i++) {
	        node = navRoot.childNodes[i];
	        if (node.nodeName=="LI") {
	          node.onmouseover=function() {
		        this.className+=" over";
	          }
	          node.onmouseout=function() {
		        this.className=this.className.replace(" over", "");
	          }
	        }
          }
      }
    }
}
window.onload=startList;