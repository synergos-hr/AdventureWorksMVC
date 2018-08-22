"use strict";

var Synergos = Synergos || {};

$.extend(Synergos, {
    blockUI: function (message) {
        if (typeof message === "undefined" || message === null)
            message = "Please wait...";

        $.blockUI({
            message: message,
            css: {
                border: "none",
                padding: "15px",
                backgroundColor: "white",
                "-webkit-border-radius": "10px",
                "-moz-border-radius": "10px",
                opacity: .7,
                color: "black"
            }
        });
    },
    unblockUI: function() {
        $.unblockUI();
    },
    bootstrapAlert: function (message, alertType, timeout) {
        if (timeout === undefined)
            timeout = 5000;

        // scroll #1
        //document.getElementById('alert_placeholder').scrollIntoView();
        
        // scroll #2
        // http://demos.flesler.com/jquery/scrollTo/
        
        // scroll #3
        $("html, body").animate({
            scrollTop: $("#alert_placeholder").offset().top - 20
        }, 200);
        
        var htmlPlaceholder = "<div id=\"page_placeholder\" class=\"alert alert-" + alertType + " fade in\"></div>";

        $("#alert_placeholder").html(htmlPlaceholder);

        $("#page_placeholder").html("<div class=\"alert\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-hidden=\"true\">&times;</button><span>" + message + "</span></div>");

        $("#page_placeholder button.close").click(function () {
            $("#page_placeholder").remove();
        });

        setTimeout(function () {
            $("#page_placeholder").fadeOut(900, function() {
                $("#page_placeholder").remove();
            });
        }, timeout);
    },
    showMessage: function (message, title) {
        //alert(message);

        //w2alert(message, title);

        //this.bootstrapAlert(message, 'message');

        if (typeof title === "undefined" || title === null)
            title = "";

        swal(title, message);
    },
    showInfo: function (message, title) {
        if (typeof title === "undefined" || title === null)
            title = "";

        swal(title, message, "info");
    },
    showSuccess: function (message, title) {
        if (typeof title === "undefined" || title === null)
            title = "";

        swal(title, message, "success");
    },
    showWarning: function (message, title) {
        //console.log(message);

        //w2alert(message, title);  // nema syling...

        //this.alertCustom(message, title, { titleBackgroundColorFrom: 'orange', titleBackgroundColorTo: 'orangered' });

        //this.bootstrapAlert(message, 'warning');

        if (typeof title === "undefined" || title === null)
            title = "Upozorenje";

        swal(title, message, "warning");
    },
    showError: function (message, title) {
        //console.log(message);

        //w2alert(message, title);  // nema syling...

        //this.alertCustom(message, title, { titleBackgroundColorFrom: 'orangered', titleBackgroundColorTo: 'red' });

        //this.bootstrapAlert(message, 'danger');

        if (typeof title === "undefined" || title === null)
            title = "Greška";

        swal(title, message, "error");
    },
    showValidation: function(message, title) {
        if (typeof title === "undefined" || title === null)
            title = "Data validation error";

        var dialog = $("#dialog-message").kendoDialog({
            width: "450px",
            closable: true,
            modal: true,
            //title: title,
            //content: message,
            //buttonLayout: "normal",
            //actions: [{
            //    text: "Izlaz",
            //    primary: true
            //}],
            messages: {
                close: "Close"
            }
        });

        dialog.data("kendoDialog").title(title);
        dialog.data("kendoDialog").content(message);

        dialog.data("kendoDialog").open();
    },
    showException: function (message, title) {
        if (typeof title === "undefined" || title === null)
            title = "Error";

        var dialog = $("#dialog-message").kendoDialog({
            width: "450px",
            closable: true,
            modal: true,
            //title: title,
            //content: message,
            //buttonLayout: "normal",
            //actions: [{
            //    text: "Izlaz",
            //    primary: true
            //}],
            messages: {
                close: "Close"
            }
        });

        var kendoDialog = dialog.data("kendoDialog");

        if (typeof kendoDialog === "undefined" || kendoDialog === null) {
            console.log("Missing 'dialog-message' element!");
            alert(message);
            return;
        }

        kendoDialog.title(title);
        kendoDialog.content(message);

        kendoDialog.open();
    },
    camelCaseReviver: function (key, value) {
        if (value && typeof value === "object") {
            for (var k in value) {
                if (/^[A-Z]/.test(k) && Object.hasOwnProperty.call(value, k)) {
                    value[k.charAt(0).toLowerCase() + k.substring(1)] = value[k];
                    delete value[k];
                }
            }
        }
        return value;
    },
    htmlDecode: function (value) {
        return $("<textarea/>").html(value).text();
    },
    htmlEncode: function (value) {
        return $("<textarea/>").text(value).html();
    }
});
