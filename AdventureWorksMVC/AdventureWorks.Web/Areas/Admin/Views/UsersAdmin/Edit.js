var form = $("#form-user-edit");

var validator = form.kendoValidator().data("kendoValidator");

$(function () {
    var ko = kendo.observable(viewModel);

    var isDataChanged = false;

    $.extend(ko, {
        isViewDisabled: false,
        save: function (e) {
            e.preventDefault();
            if (validator.validate()) {
                form.submit();
                isDataChanged = false;
            }
        },
        sendTestEmail: function (e) {
            e.preventDefault();
            //console.log(viewModel);
            var urlSend = window.applicationBaseUrl + "/api/users/sendTestEmail";

            $.post(urlSend, { id: viewModel.UserId },
                function (successData) {
                    if (successData.Status === "success")
                        swal("", "Test email sent to user.", "success");
                }, "JSON");
        }
    });

    ko.bind("change", function (e) {
        isDataChanged = true;
    });

    kendo.bind(form, ko);

    window.onbeforeunload = function (e) {
        if (isDataChanged) {
            e.returnValue = "Data on page is changed.\r\nIf you continue changes will be lost.";
            return "";
        }

        return undefined;
    };

    function confirmUnsavedLeavePage() {
        var dfdConfirmExit = jQuery.Deferred();

        swal({
            title: "Data changed",
            text: "If you continue changes on page will be lost.",
            type: "warning",
            showCancelButton: true,
            focusCancel: true,
            confirmButtonText: "Continue",
            cancelButtonText: "Stay on page"
        }).then(
            function () {
                dfdConfirmExit.resolve("leave");
            },
            function (dismiss) {
                dfdConfirmExit.reject("stay");
            });

        return dfdConfirmExit.promise();
    }

    $("ul.sidebar-menu li a").click(function(e) {
        var link = this;

        if (isDataChanged) {
            $.when(confirmUnsavedLeavePage()).then(
                function(status) {
                    isDataChanged = false; // so onbeforeunload won't trigger
                    //console.log({ statusSuccess: status });
                    window.location = link.href;
                },
                function(status) {
                    //console.log({ statusFail: status });
                },
                function(status) {
                    //console.log({ statusProgress: status });
                }
            );

            if (e.preventDefault)
                e.preventDefault();

            return false;
        }

        return true;
    });

    $("input[name='Gender']").kendoDropDownList({
        dataValueField: "GenderID",
        dataTextField: "GenderName",
        dataSource: [
            { GenderID: "F", GenderName: "Female" },
            { GenderID: "M", GenderName: "Male" }
        ]
    });
});
