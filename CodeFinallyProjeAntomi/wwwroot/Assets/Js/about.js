

$(".panel-default").on("click", function(e){
    e.preventDefault();
    
    var collapseElement = $(this).find(".collapse");

    
    if (collapseElement.css("opacity") === "1" && collapseElement.css("height") === "200px") {
   
        collapseElement.css({
            "opacity": "0",
            "height": "0px"
        });
    } else {
        collapseElement.css({
            "opacity": "1",
            "height": "200px"
        });
    }
});









