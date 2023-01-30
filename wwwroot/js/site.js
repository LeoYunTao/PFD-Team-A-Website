<<<<<<< HEAD
﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $("#idButton").click(function () {
        $("#idform").css('display', 'block')
    });

    // with only the if, it is able to pop up but not able to disappear but with the else, it completely doesnt work.

    $("#idform").submit(function (event) {
        event.preventDefault();
        var email = $("#newEmail").val();           // input Value
        var nextLine = document.createElement("br");   // br to so it is not printed in same ine as button
        $("#idButton").before(email, nextLine);    // Insert new elements after submit
    });

    // for some reason prints 2 elements twice
});
=======
﻿
>>>>>>> executeTestCases
