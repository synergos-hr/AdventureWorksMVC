"use strict";

var SynApp = SynApp || {};
var Admin = $.extend(true, SynApp, { Admin: {} });
var Products = $.extend(true, SynApp.Admin, { Products: {} });

$.extend(SynApp.Admin.Products, {
    ProductsGrid: (function () {
        function ProductsGrid(subcategoryID) {
            var that = this;

            var gridSettings = {
                columns: [
                    { field: "Name", title: "Product name", width: "200px" },
                    { field: "ProductNumber", title: "Number", width: "100px" },
                    { field: "Color", title: "Color", width: "100px" }
                ],
                toolbar: kendo.template($("#template-grid-toolbar").html()),
                editable: {
                    mode: "popup",
                    window: {
                        title: "Product"
                    }
                }
            };

            var gridModel = {
                id: "ProductID",
                fields: {
                    Name: { type: "string", validation: { required: true } },
                    ProductNumber: { type: "string" },
                    Color: { type: "string" }
                }
            };

            var gridCallbacks = {
                onSetTransportParameterMap: function (parameters, type) {
                    if (type === "read" && parameters) {
                        parameters.extraFilters = [
                            { "Key": "SubcategoryID", "Value": subcategoryID }
                        ];
                    }
                },
                onSetDataSourceParams: function (params) {
                },
                onView: function (item) {
                    document.location = window.applicationBaseUrl + "/admin/products/display/" + item.id;
                }
            };

            var appParams = {
                controllerName: "products"
            };

            function init() {
                return new Synergos.Grid("#grid", gridSettings, gridModel, gridCallbacks, appParams);
            }

            this.Grid = init();
        }

        return ProductsGrid;
    })()
});
