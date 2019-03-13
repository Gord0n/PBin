$(document).ready(function () {
    var sts = $("#feedback").text();
    
    if (sts !== "") {
        $("#feedback").fadeIn(400).delay(3000).fadeOut(400); //fade out after 3 seconds
    } 

});
