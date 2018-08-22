"use strict";

var SynApp = SynApp || {};
var Admin = $.extend(true, SynApp, { Admin: {} });
var Departments = $.extend(true, SynApp.Admin, { Departments: {} });

$.extend(SynApp.Admin.Departments, {
    DepartmentsGrid: (function () {
        function DepartmentsGrid() {
            var that = this;

            var gridSettings = {
                columns: [
                    { field: "Name", title: "Name", width: "200px" },
                    { field: "GroupName", title: "Group", width: "200px" },
                    { field: "ModifiedDate", title: "Modified", width: "100px", attributes: { class: "grid-col-date" }, format: "{0:dd.MM.yyyy}", editor: Synergos.Grid.PopupDateEditor }
                ],
                toolbar: kendo.template($("#template-grid-toolbar").html()),
                editable: {
                    mode: "popup",
                    window: {
                        title: "Department"
                    }
                }
            };

            var gridModel = {
                id: "DepartmentID",
                fields: {
                    Name: { type: "string", validation: { required: true } },
                    GroupName: { type: "string" },
                    ModifiedDate: { type: "date" }
                }
            };

            var gridCallbacks = {
                onSetTransportParameterMap: function (parameters, type) {
                },
                onSetDataSourceParams: function (params) {
                },
                onView: function (item) {
                    document.location = window.applicationBaseUrl + "/admin/departments/display/" + item.id;
                }
            };

            var appParams = {
                controllerName: "Departments"
            };

            function init() {
                return new Synergos.Grid("#grid", gridSettings, gridModel, gridCallbacks, appParams);
            }

            this.Grid = init();
        }

        return DepartmentsGrid;
    })()
});
