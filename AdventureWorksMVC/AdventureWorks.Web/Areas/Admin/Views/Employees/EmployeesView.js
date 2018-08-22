'use strict';

var SynApp = SynApp || {};
var Admin = $.extend(true, SynApp, { Admin: {} });
var Employees = $.extend(true, SynApp.Admin, { Employees: {} });

$.extend(SynApp.Admin.Employees, {
    EmployeesView: (function () {

        function EmployeesView() {
            //console.log(model);
            this.Grid = new SynApp.Admin.Employees.EmployeesGrid();
        }

        return EmployeesView;
    })()
});
