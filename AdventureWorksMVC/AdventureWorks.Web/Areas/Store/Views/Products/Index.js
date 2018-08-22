'use strict';

var SynApp = SynApp || {};
var Products = $.extend(true, SynApp, { Products: {} });

$.extend(SynApp.Products, {
    ProductsView: (function () {

        function ProductsView() {
            //this.Grid = new SynApp.Products.ProductsGrid();

            this.List = new SynApp.Products.ProductsList();
        }

        return ProductsView;
    })()
});

$.extend(SynApp.Products, {
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
                    document.location = window.applicationBaseUrl + '/products/display/' + item.id;
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

$.extend(SynApp.Products, {
    ProductsList: (function () {
        function ProductsList() {
            var that = this;

            var dataSource = new kendo.data.DataSource({
                serverFiltering: true,
                pageSize: 24,
                transport: {
                    read: { type: 'post', dataType: 'json', url: window.applicationBaseUrl + '/api/products/listFiltered', data: {} }
                },
                schema: {
                    type: 'json',
                    data: function (data) { return data.records; },
                    total: function (data) { return data.totalCount; },
                    errors: function (data) { return data.status !== 'success' ? data.message : null; }
                },
                error: function (e) {
                    if (e.status == "customerror") {
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
                selectable: true,
                template: kendo.template($("#template_product").html())
                //change: onChangedListView
            });
        }

        return ProductsList;
    })()
});

$(function () {
    var view = new SynApp.Products.ProductsView();
});