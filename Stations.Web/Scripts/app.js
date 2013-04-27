$(document).ready(function () {
    $(".navbar li a").each(function () {
        var elem = $(this);
        
        if (elem.attr("href") == location.pathname + location.search) {
            elem.parent().addClass("active");
            return false;
        }
    });

    $("#parserMenu").on("click", "li a", function () {
        if (!confirm("Do you really want go to parse?")) {
            return false;
        }
    });
    
    $(window).resize(function () {
        var h = $(window).height(),
            offsetTop = 100; // Calculate the top offset
        $(".google-maps").css("height", (h - offsetTop));
        $(".window-height").each(function () {
            var elem = $(this);
            elem.height(h - elem.offset().top - 20);
        });
    }).resize();
});