"use strict";

var SynApp = SynApp || {};
var Admin = $.extend(true, SynApp, { Admin: {} });
var Employees = $.extend(true, SynApp.Admin, { Employees: {} });

$.extend(SynApp.Admin.Employees, {
    EmployeesGrid: (function () {
        function EmployeesGrid() {
            var that = this;

            var gridSettings = {
                columns: [
                    { field: "LastName", title: "Last name", width: "200px", editable: false },
                    { field: "MiddleName", title: "Middle name", width: "100px", editable: false },
                    { field: "FirstName", title: "First name", width: "200px", editable: false },
                    { field: "JobTitle", title: "Job title", width: "150px", editable: false },
                    { field: "Department", title: "Department", width: "150px", editable: false },
                    { field: "GroupName", title: "Group", width: "150px", editable: false }
                ],
                toolbar: kendo.template($("#template-grid-toolbar").html()),
                editable: {
                    mode: "popup",
                    window: {
                        title: "Employee"
                    }
                }
            };

            var gridModel = {
                id: "BusinessEntityID",
                fields: {
                    LastName: { type: "string", validation: { required: true }, editable: false  },
                    MiddleName: { type: "string", validation: { required: true }, editable: false  },
                    FirstName: { type: "string", validation: { required: true }, editable: false  },
                    JobTitle: { type: "string", editable: false  },
                    Department: { type: "string"  },
                    GroupName: { type: "string", editable: false  }
                }
            };

            var gridCallbacks = {
                onSetTransportParameterMap: function (parameters, type) {
                },
                onSetDataSourceParams: function (params) {
                },
                onView: function (item) {
                    document.location = window.applicationBaseUrl + "/admin/employees/display/" + item.id;
                }
            };

            var appParams = {
                controllerName: "Employees"
            };

            function init() {
                return new Synergos.Grid("#grid", gridSettings, gridModel, gridCallbacks, appParams);
            }

            this.Grid = init();
        }

        return EmployeesGrid;
    })()
});
