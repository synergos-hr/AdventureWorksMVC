'use strict';

var SynApp = SynApp || {};
var Admin = $.extend(true, SynApp, { Admin: {} });
var Departments = $.extend(true, SynApp.Admin, { Departments: {} });

$.extend(SynApp.Admin.Departments, {
    DepartmentsView: (function () {

        function DepartmentsView() {
            //console.log(model);
            this.Grid = new SynApp.Admin.Departments.DepartmentsGrid();
        }

        return DepartmentsView;
    })()
});
