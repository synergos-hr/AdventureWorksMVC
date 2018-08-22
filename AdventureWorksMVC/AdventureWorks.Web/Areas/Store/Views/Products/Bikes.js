"use strict";

var SynApp = SynApp || {};
var Products = $.extend(true, SynApp, { Products: {} });

$.extend(SynApp.Products, {
    ViewBikes: (function () {

        function ViewBikes(model) {
            this.List = new SynApp.Products.ListBikes(model.SubcategoryID);
        }

        return ViewBikes;
    })()
});

$.extend(SynApp.Products, {
    ListBikes: (function () {
        function ListBikes(subcategoryID) {
            var that = this;

            var dataSource = new kendo.data.DataSource({
                serverFiltering: true,
                serverPaging: true,
                pageSize: 24,
                transport: {
                    read: { type: "post", dataType: "json", url: window.applicationBaseUrl + "/api/products/listFiltered", data: { subcategoryID: subcategoryID } }
                },
                schema: {
                    type: "json",
                    data: function (data) { return data.Records; },
                    total: function (data) { return data.TotalCount; },
                    errors: function (data) { return data.Status !== "success" ? data.Message : null; }
                },
                error: function (e) {
                    if (e.status === "customerror") {
                        kendo.alert(e.errors);
                    } else {
                        kendo.alert("Status: " + e.status + "<br/>Error: " + e.errorThrown + "<br/>Message: " + e.errors);
                    }
                }
            });

            $("#pager_products").kendoPager({
                dataSource: dataSource
            });

            this.List = $("#list_products").kendoListView({
                dataSource: dataSource,
                selectable: false,
                pageable: true,
                template: kendo.template($("#template_product").html())
                //change: onChangedListView
            });
        }

        return ListBikes;
    })()
});
