

$(document).ready(function(){
    $(".Returning.collapsed").on("click", function(e){
        e.preventDefault(); 
        $("#checkout_coupon").toggleClass("collapse"); 

        if ($("#checkout_coupon").hasClass("collapse")) {
            $("#checkout_coupon").css("transition", "display 7000s"); 
            $("#checkout_coupon").css("display", "none"); 
        } else {
            $("#checkout_coupon").css("transition", "display 7000s"); 
            $("#checkout_coupon").css("display", "inline-block"); 
        }
    });
});
