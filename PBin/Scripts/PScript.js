$(document).ready(function () {
    var sts = $("#feedback").text();
    
    if (sts !== "") {
        $("#feedback").fadeIn(400).delay(3000).fadeOut(400); //fade out after 3 seconds
    } 

    $("#PublicPostCheckBox").on("change", function () {        
        if ($("#PublicPostCheckBox").is(" :checked")) {
            $("#NewPost_Public").val("True");
        } else {
            $("#NewPost_Public").val("False");
        }
       
    });

    $("#EditPublicPostCheckBox").on("change", function () {
        if ($("#EditPublicPostCheckBox").is(" :checked")) {
            $("#Post_Public").val("True");
        } else {
            $("#Post_Public").val("False");
        }

    })

    $("#deletePostButton").click(function () {

        $("#Post_Enabled").val("False");

        $("#editPostForm").submit();

    });

    /*
     * Until the next comment is the input validation for Create Account
     */ 

    $("#CreateUserButton").on("click", function (e) {
  
        if (!validateCreateAccountEmail()) {
            e.preventDefault();
        }

        if (!validateCreateAccountPassword()) {
            e.preventDefault();            
        } 

        if (!validateCreateAccountFirstName()) {
            e.preventDefault();
        }

        if (!validateCreateAccountLastName()) {
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

    $("#NewUser_FirstName").blur(function () {
        validateCreateAccountFirstName();
    });

    $("#NewUser_FirstName").on("keyup", function () {
        validateCreateAccountFirstName();
    });

    $("#NewUser_LastName").blur(function () {
        validateCreateAccountLastName();
    });

    $("#NewUser_LastName").on("keyup", function () {
        validateCreateAccountLastName();
    });

});

function validateCreateAccountEmail() {
    var email = $("#NewUser_Email").val();
    var emailRegex = new RegExp("^\\S+@\\S+$");    

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
    var passwordRegex = new RegExp("^(?=.{8,})(?=.*[a-zA-Z])(?=.*[!@#$%^&+=]).*$");

    if (!passwordRegex.test(password)) {
        $("#NewUser_Password").addClass("invalid");
        return false;
    } else {
        $("#NewUser_Password").removeClass("invalid");
        return true;
    }
}

function validateCreateAccountFirstName() {
    var firstName = $("#NewUser_FirstName").val();
   
    if (firstName.length > 0 && firstName.length <= 32) {
        $("#NewUser_FirstName").removeClass("invalid");
        return true;      
    } else {
        $("#NewUser_FirstName").addClass("invalid");
        return false;        
    }
}

function validateCreateAccountLastName() {
    var lastName = $("#NewUser_LastName").val();

    if (lastName.length > 0 && lastName.length <= 32) {
        $("#NewUser_LastName").removeClass("invalid");      
        return true;         
    } else {
        $("#NewUser_LastName").addClass("invalid");
        return false;       
    }
}



function displayMessage(message) {
    $("#feedback").text(message);
    $("#feedback").fadeIn(400).delay(3000).fadeOut(400); //fade out after 3 seconds
}