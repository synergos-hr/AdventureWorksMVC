"use strict";

var SynApp = SynApp || {};
var Admin = $.extend(true, SynApp, { Admin: {} });

$.extend(SynApp.Admin, {
    UserAdminView: (function () {

        function UserAdminView() {
            this.Grid = new SynApp.Admin.UsersGrid();
        }

        return UserAdminView;
    })(),

    UsersGrid: (function () {
        function UsersGrid() {
            var that = this;

            var gridSettings = {
                columns: [
                    { field: "UserName", title: "User name", width: 150 },
                    { field: "Email", title: "Email", width: 200 },
                    { field: "FirstName", title: "First name", width: 100 },
                    { field: "LastName", title: "Last name", width: 100 },
                    {
                        field: "Locked",
                        title: "Locked",
                        width: 100,
                        attributes: { class: "text-center" },
                        template: '#= Locked ? "Da" : "" #'
                    }
                ],
                toolbar: kendo.template($("#template-grid-toolbar").html()),
                editable: {
                    mode: "popup",
                    template: $("#template-popup").html(),
                    window: {
                        title: "User"
                    }
                }
            };

            var gridModel = {
                id: "UserId",
                fields: {
                    UserId: { editable: false, nullable: true },
                    UserName: { type: "string" },
                    Email: { type: "string" },
                    DomainUserName: { type: "string" },
                    FirstName: { type: "string" },
                    LastName: { type: "string" },
                    DisplayName: { type: "string" },
                    RegionName: { type: "string" },
                    DepartmentName: { type: "string" },
                    WorkPositionName: { type: "string" },
                    Locked: { type: "boolean" }
                }
            };

            var gridCallbacks = {
                onDoubleClick: function (e) {
                    that.EditCustom();
                    return false;
                }
            };

            var appParams = {
                controllerName: "users"
            };

            function init() {
                return new Synergos.Grid("#grid", gridSettings, gridModel, gridCallbacks, appParams);
            }

            this.Grid = init();
        }

        UsersGrid.prototype.Create = function() {
            //var that = this;
            //var callbacks = {
            //    onSaved: function () {
            //        that.Grid.Refresh();
            //    }
            //}
            //var window = new SynApp.Admin.UserCreateWindow(callbacks);

            window.location.href = window.applicationBaseUrl + "/Admin/UsersAdmin/Create/";
        };

        UsersGrid.prototype.EditCustom = function() {
            var selected = this.Grid.Selected();

            if (selected === null)
                return;

            window.location.href = window.applicationBaseUrl + "/Admin/UsersAdmin/Edit/" + selected.id;
        };

        UsersGrid.prototype.ChangePassword = function() {
            var user = this.Grid.Selected();
            if (user === null)
                return;
            var window = new SynApp.Admin.SetPasswordWindow(user);
        };

        UsersGrid.prototype.Lock = function() {
            var user = this.Grid.Selected();
            if (user === null)
                return;
            var that = this;
            console.log(user);
            var userName = user.DisplayName;
            swal({
                title: "Locking user",
                text: "Lock user " + userName + "?",
                type: "warning",
                showCancelButton: true,
                confirmButtonText: "Lock",
                cancelButtonText: "Cancel"
            }).then(function() {
                    var dataItem = that.Grid.kendoGrid.dataSource.get(user.id);
                    var urlLock = window.applicationBaseUrl + "/api/users/lock";
                    $.post(urlLock,
                        { id: user.id },
                        function(successData) {
                            dataItem.set("Locked", true);
                        },
                        "JSON");
                },
                function(dismiss) {
                });
        };

        UsersGrid.prototype.Unlock = function() {
            var user = this.Grid.Selected();
            if (user === null)
                return;
            var that = this;
            var userName = user.DisplayName;
            swal({
                title: "Unlocking user",
                text: "Unlock user " + userName + "?",
                type: "warning",
                showCancelButton: true,
                confirmButtonText: "Unlock",
                cancelButtonText: "Cancel"
            }).then(function() {
                    var dataItem = that.Grid.kendoGrid.dataSource.get(user.id);
                    var urlLock = window.applicationBaseUrl + "/api/users/unlock";
                    $.post(urlLock,
                        { id: user.id },
                        function(successData) {
                            dataItem.set("Locked", false);
                        },
                        "JSON");
                },
                function(dismiss) {
                });
        };

        UsersGrid.prototype.Roles = function() {
            var user = this.Grid.Selected();
            if (user === null)
                return;
            var that = this;
            var window = new SynApp.Admin.UserRolesWindow(user);
        };

        UsersGrid.prototype.ShowHistory = function () {
            var selected = this.Grid.Selected();

            if (selected === null)
                return;

            var history = new Synergos.History.HistoryWindow("UserProfile", selected);
        }

        return UsersGrid;
    })(),

    UserCreateWindow: (function () {
        var that;

        function UserCreateWindow(callbacks) {
            that = this;

            this.Window = initWindow();
            this.Validator = initValidator();
            this.ViewModel = initViewModel(callbacks);

            var model = {};

            this.ViewModel.dataSource.add(model);

            var selected = this.ViewModel.dataSource.at(0);

            this.ViewModel.set("selected", selected);

            kendo.bind(this.Window.element, this.ViewModel);

            this.Window.center().open();
        }

        function initWindow() {
            var window = $("#dialog-user-create").kendoWindow({
                width: 750,
                //height: 500,
                title: "New user",
                visible: false,
                content: {
                    template: kendo.template($("#template-dialog-user-create").html())
                }
            }).data("kendoWindow");

            return window;
        }

        function initValidator() {
            var validator = $("#dialog-user-create").kendoValidator().data("kendoValidator"); // create a validator instance

            return validator;
        }

        function initViewModel(callbacks) {
            var viewModel = kendo.observable({
                dataSource: new kendo.data.DataSource({
                    transport: {
                        update: {
                            url: window.applicationBaseUrl + "/api/users/create",
                            type: "post",
                            dataType: "json"
                        },
                        create: {
                            url: window.applicationBaseUrl + "/api/users/create",
                            type: "post",
                            dataType: "json"
                        },
                        parameterMap: function (parameters, type) {
                            if (type !== "read" && parameters) {
                                return parameters;
                            }
                            return null;
                        }
                    },
                    //autobind: false,
                    schema: {
                        type: "json",
                        model: {
                            id: "email",
                            fields: {
                                email: { type: "string", validation: { required: true } },
                                password: { type: "string", validation: { required: true, pattern: ".{6,20}" } },    // TODO: min lenght validation (6)
                                confirmPassword: { type: "string", validation: { required: true, pattern: ".{6,20}" } }
                            }
                        },
                        errors: function (data) { return data.Status !== "success" ? data.message : null; }
                    },
                    // TODO: success refresh grid
                    error: function (e) {
                        if (e.Status === "customerror") {
                            kendo.alert(e.Errors);
                        } else {
                            debugger;   // LINK: http://docs.telerik.com/kendo-ui/framework/datasource/crud#remote-transport-error-handling
                            kendo.alert("Status: " + e.Status + "<br/>Error: " + e.ErrorThrown + "<br/>Message: " + e.Errors);
                        }
                    },
                    change: function () {
                        //that.Window.content(template(view[0]));
                        //$("#data-container").html(kendo.render(template, this.view())); //render the template with current data
                    }
                }),
                selected: {}, //this field will contain the edited dataItem
                sync: function () {
                    if (that.Validator.validate()) { //validate the user input
                        this.dataSource.sync(); //sync the changes through the transport
                        that.Window.close();

                        if (typeof callbacks !== "undefined" && $.isFunction(callbacks.onSaved))
                            callbacks.onSaved();
                    }
                },
                cancel: function () {
                    this.dataSource.cancelChanges(); //cancel all the change
                    that.Validator.hideMessages(); //hide the warning messages
                    that.Window.close();
                }
            });

            return viewModel;
        }

        return UserCreateWindow;
    })(),

    SetPasswordWindow: (function () {
        var instance;

        function SetPasswordWindow(record) {
            instance = this;

            this.Record = record;

            this.Window = $("#dialog-set-password").kendoWindow({
                width: 750,
                //height: 500,
                title: "Promjena lozinke",
                visible: false
            }).data("kendoWindow");

            this.Validator = $("#form-set-password").kendoValidator({
                rules: {
                    validateNewPassword: function (input) {
                        if (input.is("[name=NewPassword]")) {
                            if (input.val().length < 6) {
                                input.attr("data-validateNewPassword-msg", "'New password' have to be at least 6 characters.");
                                return false;
                            }
                            if (input.val().length > 20) {
                                input.attr("data-validateNewPassword-msg", "'New password' have to be maximum 20 characters.");
                                return false;
                            }
                        }
                        return true;
                    },
                    validateConfirmPassword: function (input) {
                        if (input.is("[name=ConfirmPassword]")) {
                            if (input.val().length < 6) {
                                input.attr("data-validateConfirmPassword-msg", "'Confirm password' have to be at least 6 characters.");
                                return false;
                            }
                            if (input.val().length > 20) {
                                input.attr("data-validateConfirmPassword-msg", "'Confirm password' have to be maximum 20 characters.");
                                return false;
                            }
                            if (input.val() !== $("input[name=NewPassword]").val()) {
                                input.attr("data-validateConfirmPassword-msg", "'New password' i 'Confirm password' doesn't match.");
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }).data("kendoValidator");

            this.ViewModel = kendo.observable({
                UserId: record.id,
                NewPassword: null,
                ConfirmPassword: null,
                sync: function (e) {
                    e.preventDefault();
                    if (instance.Validator.validate()) {
                        saveChanges();
                    }
                },
                cancel: function (e) {
                    e.preventDefault();
                    instance.Validator.hideMessages();
                    instance.Window.close();
                }
            });

            kendo.bind($("#form-set-password"), this.ViewModel);

            this.Window.center().open();
        }

        function saveChanges() {
            var url = window.applicationBaseUrl + "/api/users/setPassword";

            $.post(url, $("#form-set-password").serialize(), function (data) {
                if (data.Status === "success") {
                    swal({
                        title: "User",
                        text: "User password is changed.",
                        type: "success"
                    }).then(function () {
                            instance.Window.close();
                        },
                        function(dismissData) {
                        });
                    return true;
                } else if (data.Status === "validation") {
                    Synergos.showWarning(data.Message);
                    return false;
                } else if (data.Status === "error") {
                    Synergos.showException(data.Message);
                    return false;
                } else {
                    console.log({ message: "Unknown status:" + data.Status, data: data });
                    return false;
                }
            });
        }

        return SetPasswordWindow;
    })(),

    UserRolesWindow: (function () {
        function UserRolesWindow(record) {
            var that = this;
            this.Window = init();
            this.Grid = new SynApp.Admin.UserRolesGrid(record);
            $.extend(record, {
                sync: function () {
                    that.Window.close();
                },
                cancel: function () {
                    that.Window.close();
                }
            });
            kendo.bind(this.Window.element, record);
            this.Window.center().open();
        }

        function init() {
            var window = $("#dialog-roles").kendoWindow({
                width: 900,
                //height: 500,
                title: "User roles",
                visible: false,
                content: {
                    template: kendo.template($("#template-dialog-roles").html())
                }
            }).data("kendoWindow");

            return window;
        }

        return UserRolesWindow;
    })(),

    UserRolesGrid: (function () {

        function UserRolesGrid(record) {
            var that = this;
            this.record = record;
            var gridSettings = {
                columns: [
                    { field: "RoleName", title: "Role name", width: "200px" },
                    { field: "Selected", title: "Selected", width: "100px", attributes: { class: "text-center" }, template: '#= Selected ? "Yes" : "" #' }
                ],
                toolbar: kendo.template($("#template-grid-roles-toolbar").html())
            };
            var gridModel = {
                id: "RoleId",
                fields: {
                    RoleId: { editable: false, validation: { required: false } },
                    RoleName: { type: "string" },
                    Selected: { type: "boolean" }
                }
            };
            var gridCallbacks = {
                onSetTransportParameterMap: function (parameters, type) {
                },
                onSetDataSourceParams: function (params) {
                    params.transport = {
                        read: { type: "post", dataType: "json", url: window.applicationBaseUrl + "/api/users/listAllRoles", data: { extraFilters: [{ 'Key': "UserId", 'Value': that.record.id }] } }
                        //update: { type: 'post', dataType: 'json', url: window.applicationBaseUrl + '/api/users/changeRoles', complete: function (e) { if (e.responseJSON.Status !== 'success') kendoGrid.dataSource.cancelChanges(); } }
                    };
                }
            };
            var appParams = {};
            function init() {
                return new Synergos.Grid("#grid-roles", gridSettings, gridModel, gridCallbacks, appParams);
            }
                
            this.Grid = init();

            $('#grid-roles div.toolbar-actions button[name="setRole"]').click(function (e) {
                that.SetRole();
            });
            $('#grid-roles div.toolbar-actions button[name="unsetRole"]').click(function (e) {
                that.UnsetRole();
            });
        }

        UserRolesGrid.prototype.Selected = function () {
            var selected = this.Grid.kendoGrid.select();
            if (selected.length === 0) {
                swal("", "Role is not selected.");
                return null;
            }
            return this.Grid.kendoGrid.dataItem(selected[0]);
        };
        UserRolesGrid.prototype.SetRole = function () {
            var role = this.Selected();
            if (role === null || typeof role === "undefined")
                return;
            if (role.Selected === true) {
                swal("", "User is already in role.");
                return;
            }
            var that = this;
            swal({
                title: "Adding user to role",
                text: "Add user to role " + role.RoleName + "?",
                type: "warning",
                showCancelButton: true,
                confirmButtonText: "Add",
                cancelButtonText: "Cancel"
            }).then(function () {
                var dataItem = that.Grid.kendoGrid.dataSource.get(role.RoleId);
                var url = window.applicationBaseUrl + "/api/users/setRole";
                $.post(url, { UserId: that.record.id, RoleId: role.RoleId }, function (successData) {
                    if (successData.Status === "success")
                        dataItem.set("Selected", true);
                    else
                        swal("Error", successData.Message);
                }, "JSON");
            }, function (dismiss) {
            });
        };
        UserRolesGrid.prototype.UnsetRole = function () {
            var role = this.Selected();
            if (role === null || typeof role === "undefined")
                return;
            if (role.Selected === false) {
                swal("", "User is not in role.");
                return;
            }
            var that = this;
            swal({
                title: "Removing user from role",
                text: "Remove user from role " + role.RoleName + "?",
                type: "warning",
                showCancelButton: true,
                confirmButtonText: "Remove",
                cancelButtonText: "Cancel"
            }).then(function () {
                var dataItem = that.Grid.kendoGrid.dataSource.get(role.RoleId);
                var url = window.applicationBaseUrl + "/api/users/unsetRole";
                $.post(url, { UserId: that.record.id, RoleId: role.RoleId }, function (successData) {
                    if (successData.Status === "success")
                        dataItem.set("Selected", false);
                    else
                        swal("Error", successData.Message);
                }, "JSON");
            }, function (dismiss) {
            });
        };

        return UserRolesGrid;
    })()
});

$(function () {
    var view = new SynApp.Admin.UserAdminView();

    $('#grid div.toolbar-actions button[name="create"]').click(function (e) {
        view.Grid.Create();
    });
    $('#grid div.toolbar-actions button[name="editCustom"]').click(function (e) {
        view.Grid.EditCustom();
    });
    $('#grid div.toolbar-actions button[name="change-password"]').click(function (e) {
        view.Grid.ChangePassword();
    });
    $('#grid div.toolbar-actions button[name="lock"]').click(function (e) {
        view.Grid.Lock();
    });
    $('#grid div.toolbar-actions button[name="unlock"]').click(function (e) {
        view.Grid.Unlock();
    });
    $('#grid div.toolbar-actions button[name="roles"]').click(function (e) {
        view.Grid.Roles();
    });
    $('#grid div.toolbar-actions button[name="history"]').click(function (e) {
        view.Grid.ShowHistory();
    });
});