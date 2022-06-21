//Burger Menu
$(document).ready(function () {
    $(".burger").click(function () {
        $(".sidebar").addClass("show");
        $(".overlay").addClass("show");
        $(".close-menu").addClass("show");
        $("body").addClass("freeze");
    });
    $(".close-menu, .overlay").click(function () {
        $(".close-menu").removeClass("show");
        $(".sidebar").removeClass("show");
        $(".overlay").removeClass("show");
        $("body").removeClass("freeze");
    });
});