$(document).ready(function () {
    var sts = $("#feedback").text();
    
    if (sts !== "") {
        $("#feedback").fadeIn(400).delay(3000).fadeOut(400); //fade out after 3 seconds
    } 

    $("#CreateUserButton").on("click", function (e) {
        var email = $("#NewUser_Email").val();
        var password = $("#NewUser_Password").val();
        var firstName = $("#NewUser_FirstName").val();
        var lastName = $("#NewUser_LastName").val();
        var message = "";

        var emailRegex = new RegExp("^\\S+@\\S+$");

        var eval = emailRegex.test(email);

        if (!validateCreateAccountEmail()) {
            e.preventDefault();
        }

        if (!validateCreateAccountPassword()) {
            e.preventDefault();            
        } 

        if (firstName === "") {
            e.preventDefault();
        }

        if (lastName === "") {
            e.preventDefault();            
        }
        
    });

    $("#NewUser_Email").blur(function () {
        validateCreateAccountEmail();
    });

    $("#NewUser_Email").on("keyup", function () {
        validateCreateAccountEmail();
    });

    $("#NewUser_Password").blur(function () {
        validateCreateAccountPassword();
    });

    $("#NewUser_Password").on("keyup", function () {
        validateCreateAccountPassword();
    });

});

function validateCreateAccountEmail() {
    var email = $("#NewUser_Email").val();
    var emailRegex = new RegExp("^\\S+@\\S+$");

    var eval = emailRegex.test(email);

    if (email === "") {
        $("#NewUser_Email").addClass("invalid");
        return false;
    }

    if (!emailRegex.test(email)) {        
        $("#NewUser_Email").addClass("invalid");
        return false;
    } else {
        $("#NewUser_Email").removeClass("invalid");
        return true;
    }
   
}

function validateCreateAccountPassword() {
    var password = $("#NewUser_Password").val();

    if (password === "") {
        $("#NewUser_Password").addClass("invalid");
        return false;
    } else if (password.length < 10) {
        $("#NewUser_Password").addClass("invalid");
        return false;
    } else {
        $("#NewUser_Password").removeClass("invalid");
        return true;
    }

}

function displayMessage(message) {
    $("#feedback").text(message);
    $("#feedback").fadeIn(400).delay(3000).fadeOut(400); //fade out after 3 seconds
}