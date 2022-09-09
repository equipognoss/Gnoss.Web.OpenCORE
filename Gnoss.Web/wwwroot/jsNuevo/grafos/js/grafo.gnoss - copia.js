
DeadSimpleRenderer = function (canvas) {
    var canvas = $(canvas).get(0)
    var ctx = canvas.getContext("2d");
    //var ctx = arbor.Graphics(canvas);
    var particleSystem = null
    var sys = null;

    var that = {
        //
        // the particle system will call the init function once, right before the
        // first frame is to be drawn. it's a good place to set up the canvas and
        // to pass the canvas size to the particle system
        //
        init: function (system) {
            // save a reference to the particle system for use in the .redraw() loop
            particleSystem = system

            // inform the system of the screen dimensions so it can map coords for us.
            // if the canvas is ever resized, screenSize should be called again with
            // the new dimensions
            particleSystem.screenSize(canvas.width, canvas.height)
            particleSystem.screenPadding(80) // leave an extra 80px of whitespace per side
        },

        // 
        // redraw will be called repeatedly during the run whenever the node positions
        // change. the new positions for the nodes can be accessed by looking at the
        // .p attribute of a given node. however the p.x & p.y values are in the coordinates
        // of the particle system rather than the screen. you can either map them to
        // the screen yourself, or use the convenience iterators .eachNode (and .eachEdge)
        // which allow you to step through the actual node objects but also pass an
        // x,y point in the screen's coordinate system
        // 

        redraw: function () {
            ctx.clearRect(0, 0, canvas.width, canvas.height)
            //gfx.clear();

            particleSystem.eachEdge(function (edge, pt1, pt2) {
                // edge: {source:Node, target:Node, length:#, data:{}}
                // pt1:  {x:#, y:#}  source position in screen coords
                // pt2:  {x:#, y:#}  target position in screen coords

                // draw a line from pt1 to pt2
                //ctx.strokeStyle = "rgba(255,255,255, .333)"
                ctx.strokeStyle = "green"
                ctx.lineWidth = 1 + 4 * edge.data.weight
                ctx.beginPath()
                ctx.moveTo(pt1.x, pt1.y)
                ctx.lineTo(pt2.x, pt2.y)
                ctx.stroke()
            })

            particleSystem.eachNode(function (node, pt) {
                // node: {mass:#, p:{x,y}, name:"", data:{}}
                // pt:   {x:#, y:#}  node position in screen coords

                // draw a rectangle centered at pt
                var w = 10
                /*ctx.fillStyle = "white"
                ctx.fillRect(pt.x-w/2, pt.y-w/2, w,w)*/

                //ctx.oval(pt.x-w/2, pt.y-w/2, w, w, {fill:node.data.color, alpha:node.data.alpha})
                //ctx.text(node.name, pt.x, pt.y+7, {color:"white", align:"center", font:"Arial", size:12})
                //ctx.text(node.name, pt.x, pt.y+7, {color:"white", align:"center", font:"Arial", size:12})

                var centerX = pt.x - w / 2;
                var centerY = pt.y - w / 2;
                //var radius = 70;
                var radius = node.data.radio;

                ctx.beginPath();
                ctx.arc(centerX, centerY, radius, 0, 2 * Math.PI, false);
                ctx.fillStyle = node.data.color;
                ctx.fill();
                ctx.lineWidth = 1;
                ctx.strokeStyle = '#003300';
                ctx.stroke();

                var letra = radius / 4;
                ctx.font = letra + 'pt Calibri';
                ctx.fillStyle = 'black';
                ctx.textAlign = 'center';
                ctx.fillText(node.data.text, centerX, centerY + 3);

            })
        }
    }
    return that
}

function MontarGrafoFicRec(grafoID, propEnl, nodosLimNivel) {

    //Creo el canvas:
    $('.resource .wrapDescription').after('<canvas width="600" height="600" id="can_' + grafoID + '"></canvas>');
    PintarGrafo('can_' + grafoID);
}

function PintarGrafo(canvasID) {
    var canvas = $('#' + canvasID);

    sys = arbor.ParticleSystem(1000, 800, 0.5) // create the system with sensible repulsion/stiffness/friction
    sys.renderer = DeadSimpleRenderer("#" + canvasID) // our newly created renderer will have its .init() method called shortly by sys...

    // pick a random datafile and load it
    var allbirds = ["bk42w74", "bk43w73", "bk70bk62", "bk95bk3", "g81w58", "g83w57", "pk60gr7", "r15bl29", "r17pu46"]
    var alltrans = "frm"
    var randBird = allbirds[Math.floor(Math.random() * allbirds.length)] + "-" + alltrans[Math.floor(Math.random() * alltrans.length)] + ".json"

    // load the data into the particle system as is (since it's already formatted correctly for .grafting)
    //var data = $.getJSON("birds/"+randBird,function(data){
    //  sys.graft({nodes:data.nodes, edges:data.edges})
    //})
    var data = {
        "nodes": {
            "a": { color: "red", shape: "dot", alpha: 1, text: "GNOSS", radio: 120 },
            "b": { color: "blue", shape: "dot", alpha: 1, text: "Didactalia", radio: 70 },
            "c": { color: "white", shape: "dot", alpha: 1, text: "Mapas Flash", radio: 40 },
            "d": { color: "grey", shape: "dot", alpha: 1, text: "PaperToys", radio: 40 }
        },
        "edges": {
            "a": {
                "b": {
                    "weight": 1.96831
                }
            },
            "b": {
                "c": {
                    "weight": 0.265957
                },
                "d": {
                    "weight": 0.265957
                }
            }
        },
        "_": "zebra finch bk95bk3 - syllable transitions (reverse probabilities)"
    };
    sys.graft({ nodes: data.nodes, edges: data.edges })

    PonerToopTip(canvasID);
}


function PonerToopTip(canvasID)
{
	/*var html = '<div id="divCont_n" class="toolNode">Cargando...</div>'

	$(sys.getNode('a')).qtip({
      content: html,
      show: {
         when: 'click', // Show it on click...
         solo: true // ...but hide all others when its shown
      },
      position: {
        my: 'top center',
        at: 'bottom center',
        adjust: {
          cyViewport: true
        }
      },
      style: {
        classes: 'qtip-bootstrap',
        tip: true, // Create speech bubble tip at the set tooltip corner above
      },
	  events: {
        render: function(event, api) {
            //alert('dd');
			//var self = this;
			PonerHtml(api);
        }
    }
    });*/
	
	/*$(sys.getNode('a')).qtip({
         content: 'Mouse tracking!',
         position: {
             target: 'mouse', // Track the mouse as the positioning target
             adjust: { x: 5, y: 5 } // Offset it slightly from under the mouse
         }
     });*/
	 
	 $($('a.another')).qtip({
         content: 'Mouse tracking!',
         position: {
             target: 'mouse', // Track the mouse as the positioning target
             adjust: { x: 5, y: 5 } // Offset it slightly from under the mouse
         }
     });
	 
	 var canvas = $('#' + canvasID);
	 $(canvas).mousedown(function(e){
            var pos = $(this).offset();
            var p = {x:e.pageX-pos.left, y:e.pageY-pos.top}
            selected = nearest = dragged = sys.nearest(p);

            if (selected.node !== null){
				//// dragged.node.tempMass = 10000
                //dragged.node.fixed = true;
				
				if (nearest.distance <= nearest.node.data.radio)
				{
					alert(nearest.node.name);
				}
            }
            return false;
        });
}