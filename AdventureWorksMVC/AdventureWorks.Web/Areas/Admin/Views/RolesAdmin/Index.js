"use strict";

var SynApp = SynApp || {};
var Admin = $.extend(true, SynApp, { Admin: {} });

$.extend(SynApp.Admin,
    {
        RoleAdminView: (function() {

            function RoleAdminView() {
                this.RolesGrid = new SynApp.Admin.RolesGrid();
            }

            return RoleAdminView;
        })(),

        RolesGrid: (function() {
            var that;

            function RolesGrid() {
                that = this;

                var gridSettings = {
                    columns: [
                        { field: "DisplayName", title: "Name", width: "200px" }
                    ],
                    toolbar: kendo.template($("#template-grid-toolbar").html()),
                    editable: {
                        mode: "popup",
                        window: {
                            title: "User role"
                        }
                    }
                };

                var gridModel = {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        DisplayName: { type: "string", validation: { required: true } }
                    }
                };

                var gridCallbacks = {
                };

                var appParams = {
                    controllerName: "roles"
                };

                function init() {
                    return new Synergos.Grid("#grid", gridSettings, gridModel, gridCallbacks, appParams);
                }

                this.Grid = init();
            }

            RolesGrid.prototype.ShowUsers = function () {
                var selected = this.Grid.Selected();

                if (selected === null)
                    return;

                var window = new SynApp.Admin.RoleUsersWindow(selected);
            }

            return RolesGrid;
        })(),

        RoleUsersWindow: (function() {
            function RoleUsersWindow(record) {
                this.Window = init();

                this.Grid = new SynApp.Admin.RoleUsersGrid(record);

                kendo.bind(this.Window.element, record);

                this.Window.center().open();
            }

            function init() {
                var window = $("#dialog-users").kendoWindow({
                    width: 900,
                    //height: 500,
                    title: "Users",
                    visible: false,
                    content: {
                        template: kendo.template($("#template-dialog-users").html())
                    }
                }).data("kendoWindow");

                return window;
            }

            return RoleUsersWindow;
        })(),

        RoleUsersGrid: (function() {

            function RoleUsersGrid(record) {
                var that = this;

                this.record = record;

                var gridSettings = {
                    columns: [
                        { field: "UserName", title: "User name", width: "200px" },
                        { field: "FirstName", title: "First name", width: "120px" },
                        { field: "LastName", title: "Last name", width: "120px" },
                        {
                            field: "Locked",
                            title: "Locked",
                            width: "80px",
                            attributes: { class: "text-center" },
                            template: '#= Locked ? "Da" : "" #'
                        }
                    ]
                };
                var gridModel = {
                    id: "Id",
                    fields: {
                        UserName: { type: "string" },
                        Email: { type: "string" },
                        FirstName: { type: "string" },
                        LastName: { type: "string" },
                        Locked: { type: "boolean" }
                    }
                };
                var gridCallbacks = {
                    onSetDataSourceParams: function(params) {
                        params.pageSize = 10;
                        params.transport = {
                            read: {
                                type: "post",
                                dataType: "json",
                                url: window.applicationBaseUrl + "/api/roles/listRoleUsers",
                                data: { extraFilters: [{ 'Key': "RoleId", 'Value': that.record.id }] }
                            }
                        };
                    }
                };

                function init() {
                    return new Synergos.Grid("#grid-users", gridSettings, gridModel, gridCallbacks, {});
                }

                this.Grid = init();
            }

            return RoleUsersGrid;
        })()
    });

$(function () {
    var view = new SynApp.Admin.RoleAdminView();

    $('#grid div.toolbar-actions button[name="users"]').click(function (e) {
        view.RolesGrid.ShowUsers();
    });
});
