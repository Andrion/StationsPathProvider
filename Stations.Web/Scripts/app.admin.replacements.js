$(function () {
    var btnAddReplacement = $("#btnAddReplacement");
    var frmAddReplacement = $("#frmAddReplacement");

    btnAddReplacement.click(function () {
        frmAddReplacement.show();
        btnAddReplacement.hide();
    });

    $.get("/Admin/GetReplacements", {}, function (data) {
        $("#tmplReplacement").tmpl(data).appendTo("#lstReplacements");
    });

    frmAddReplacement.submit(function () {
        var $this = $(this);
        if ($this.valid()) {
            var req = {
                value: $this.find(".value").val(),
                replace: $this.find(".replacement").val()
            };

            $.post("/Admin/SetReplacement", req, function (data) {
                if (!data.success) {
                    alert("ERROR WHILE SAVE REPLACEMENT");
                }
            });
        }

        return false;
    });
});