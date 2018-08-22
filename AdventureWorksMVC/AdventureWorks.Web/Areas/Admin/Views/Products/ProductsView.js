"use strict";

var SynApp = SynApp || {};
var Admin = $.extend(true, SynApp, { Admin: {} });
var Products = $.extend(true, SynApp.Admin, { Products: {} });

$.extend(SynApp.Admin.Products, {
    ProductsView: (function () {

        function ProductsView(model) {
            this.Grid = new SynApp.Admin.Products.ProductsGrid(model.SubcategoryID);
        }

        return ProductsView;
    })()
});
/*
$.extend(SynApp.Admin.Products, {
    ProductsGrid: (function () {
        function ProductsGrid() {
            var that = this;

            var gridSettings = {
                columns: [
                    { field: "name", title: 'Product name', width: "200px" },
                    { field: "productNumber", title: 'Number', width: "100px" },
                    { field: "color", title: 'Color', width: "100px" }
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
                id: 'productID',
                fields: {
                    name: { type: 'string', validation: { required: true } },
                    productNumber: { type: 'string' },
                    color: { type: 'string' }
                }
            };

            var gridCallbacks = {
                onSetTransportParameterMap: function (parameters, type) {
                },
                onSetDataSourceParams: function (params) {
                },
                onView: function (item) {
                    document.location = window.applicationBaseUrl + '/admin/products/display/' + item.id;
                }
            };

            var appParams = {
                controllerName: 'products'
            };

            function init() {
                return new Synergos.Grid("#grid", gridSettings, gridModel, gridCallbacks, appParams);
            }

            this.Grid = init();
        }

        return ProductsGrid;
    })()
});
*/
