$(document).ready(function () {
    $.fn.showLoader = function () {
        var elem = $(this);
        var loader = $('<div class="loader" />')
            .append('<img src="/content/images/loader.gif" />');

        var elemOffset = elem.offset();
        loader.css("top", elemOffset.top);
        loader.css("left", elemOffset.left);
        loader.width(elem.width());
        loader.height(elem.height());

        elem.data("loader", true);
        elem.data("loaderElem", loader);
        $(document.body).append(loader);
    };

    $.fn.hideLoader = function () {
        var elem = $(this);
        if (elem.data("loader")) {
            var loader = elem.data("loaderElem");
            loader.remove();
            elem.data("loader", false);
            elem.removeData("loaderElem");
        }
    };

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