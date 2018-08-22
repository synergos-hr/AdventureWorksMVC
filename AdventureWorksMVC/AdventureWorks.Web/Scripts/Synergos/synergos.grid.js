'use strict';

var Synergos = Synergos || {};

$.extend(Synergos, {
    Grid: (function () {
        function Grid(container, settings, model, callbacks, dataSourceParams) {
            var that = this;

            this.container = container;
            this.settings = settings;
            this.model = model;
            this.callbacks = callbacks;
            this.dataSourceParams = dataSourceParams;

            if (typeof settings.filterable === "undefined")
                settings.filterable = true;

            if (typeof settings.selectable === "undefined")
                settings.selectable = true;

            if (typeof settings.scrollable === "undefined")
                settings.scrollable = true;

            if (typeof settings.pageable === "undefined")
                settings.pageable = {
                    input: true,
                    numeric: false
                };

            if (typeof settings.editable === "undefined")
                settings.editable = false;

            if (typeof settings.doubleClickMethod === "undefined")
                settings.doubleClickMethod = "edit";

            this.setActiveToolbarButtonsEnabled = function (enable) {
                enable ? $(container + " div.toolbar-actions button.activeOnSelect").removeClass("k-state-disabled") : $(container + " div.toolbar-actions button.activeOnSelect").addClass("k-state-disabled");
            };

            var kendoGridSettings = {
                columns: settings.columns,
                toolbar: settings.toolbar,
                scrollable: settings.scrollable,
                resizable: true,
                sortable: true,
                filterable: settings.filterable,
                pageable: settings.pageable,
                selectable: settings.selectable,
                editable: settings.editable,
                detailInit: settings.detailInit,
                edit: function (e) {
                    // https://stackoverflow.com/questions/16415604/kendoui-grid-edit-popup-how-to-hide-field/32813643
                    e.sender.columns.forEach(function (element, index /*, array */) {
                        if (element.hideInPopup) {
                            e.container.find(".k-edit-label:eq(" + index + "), "
                                + ".k-edit-field:eq( " + index + ")"
                            ).hide();
                        }
                    });

                    //e.container.parent().find('.k-window-title').text(e.model.isNew() ? gridSettings.popupEditor.titleNew : gridSettings.popupEditor.titleEdit);
                },
                change: function(e) {
                    that.setActiveToolbarButtonsEnabled(this.select().length > 0);

                    if (typeof callbacks !== "undefined" && $.isFunction(callbacks.onChange))
                        callbacks.onChange(e, this);
                },
                save: function(e) {
                    that.setActiveToolbarButtonsEnabled(false);
                },
                cancel: function(e) {
                    that.setActiveToolbarButtonsEnabled(this.select().length > 0);
                },
                noRecords: {
                    template: "<span class='no-data'>No data available.</span>"
                },
                dataBound: function(e) {
                    //const grid = this;
                    this.tbody.find("tr").dblclick(function(e) {
                        //var dataItem = grid.dataItem(this);
                        if (typeof callbacks !== "undefined" && $.isFunction(callbacks.onDoubleClick)) {
                            callbacks.onDoubleClick(e);
                            if (e.isDefaultPrevented)
                                return false;
                        }
                        if (settings.doubleClickMethod === "edit") {
                            if (that.settings.editable === true)
                                that.Edit();
                            return false;
                        }
                        return true;
                    });
                    if (typeof callbacks !== "undefined" && $.isFunction(callbacks.onDataBound))
                        callbacks.onDataBound(e, this);
                }
            };

            if (Synergos.isDefined(settings.height))
                kendoGridSettings.height = settings.height;

            this.kendoGrid = $(container).kendoGrid(kendoGridSettings).data('kendoGrid');

            that.setActiveToolbarButtonsEnabled(false); // disable all selectable buttons initially

            $.extend(callbacks, {
                onCallCompleted: function (responseJson) {
                    if (responseJson.Status !== "success")
                        that.kendoGrid.dataSource.cancelChanges();
                }
            });

            $.extend(callbacks, {
                dataSourceRead: function (e, url) {
                    if (typeof callbacks !== "undefined" && $.isFunction(callbacks.shouldCancelDataSourceCall))
                        if (callbacks.shouldCancelDataSourceCall(e.data, "read")) {
                            //console.log("Grid read dataSource cancelled."); // TODO: trace
                            e.success("");
                            return;
                        }

                    if (typeof callbacks !== "undefined" && $.isFunction(callbacks.onSetTransportParameterMap))
                        callbacks.onSetTransportParameterMap(e.data, "read");

                    $.post(url, e.data, function (responseJson) {
                        if (responseJson.Status === "success")
                            e.success(responseJson);
                        else
                            e.error("", responseJson.Status, responseJson.Message);
                    }, "JSON");
                },
                dataSourceCreate: function (e, url) {
                    if (typeof callbacks !== "undefined" && $.isFunction(callbacks.onSetTransportParameterMap))
                        callbacks.onSetTransportParameterMap(e.data, "create");
                    $.post(url, e.data, function (responseJson) {
                        if (responseJson.Status === "success") {
                            e.success(responseJson);

                            if (typeof callbacks !== "undefined" && $.isFunction(callbacks.onSuccessCreate))
                                callbacks.onSuccessCreate(e, responseJson);
                        }
                        else {
                            e.error("", responseJson.Status, responseJson.Message);
                            that.kendoGrid.dataSource.cancelChanges();
                        }
                    }, "JSON");
                },
                dataSourceUpdate: function (e, url) {
                    if (typeof callbacks !== "undefined" && $.isFunction(callbacks.onSetTransportParameterMap))
                        callbacks.onSetTransportParameterMap(e.data, "update");
                    $.post(url, e.data, function (responseJson) {
                        if (responseJson.Status === "success") {
                            e.success(responseJson);
                        }
                        else {
                            e.error("", responseJson.Status, responseJson.Message);
                            that.kendoGrid.dataSource.cancelChanges();
                        }
                    }, "JSON");
                },
                dataSourceDelete: function (e, url) {
                    if (typeof callbacks !== "undefined" && $.isFunction(callbacks.onSetTransportParameterMap))
                        callbacks.onSetTransportParameterMap(e.data, "delete");
                    $.post(url, e.data, function (responseJson) {
                        if (responseJson.Status === "success") {
                            e.success(responseJson);
                        }
                        else {
                            e.error("", responseJson.Status, responseJson.Message);
                            that.kendoGrid.dataSource.cancelChanges();
                        }
                    }, "JSON");
                }
            });

            var dataSource = getDataSource(model, callbacks, dataSourceParams);

            this.kendoGrid.setDataSource(dataSource);

            $(container + " div.toolbar-actions button[name='edit']").click(function (e) {
                e.preventDefault();
                if ($(e.currentTarget).hasClass("k-state-disabled"))
                    return;
                that.Edit();
            });

            $(container + " div.toolbar-actions button[name='delete']").click(function (e) {
                e.preventDefault();
                if ($(e.currentTarget).hasClass("k-state-disabled"))
                    return;
                that.Delete();
            });

            $(container + " div.toolbar-actions button[name='view']").click(function (e) {
                e.preventDefault();
                if ($(e.currentTarget).hasClass("k-state-disabled"))
                    return;
                that.View();
            });

            $(container + " div.toolbar-actions button[name='refresh']").click(function (e) {
                e.preventDefault();
                if ($(e.currentTarget).hasClass("k-state-disabled"))
                    return;
                that.Refresh();
            });
        }

        function getDataSource(model, callbacks, dataSourceParams) {
            dataSourceParams = Synergos.setIfNotDefined(dataSourceParams, {});

            dataSourceParams.pageSize = Synergos.setIfNotDefined(dataSourceParams.pageSize, 20);

            dataSourceParams.methodList = Synergos.setIfNotDefined(dataSourceParams.methodList, "list");

            var params = {
                serverPaging: true,
                serverSorting: true,
                serverFiltering: true,
                pageSize: dataSourceParams.pageSize,
                schema: {
                    type: "json",
                    data: function (data) { return data.Records; },
                    total: function (data) { return data.TotalCount; },
                    errors: function (data) { return data.Status !== "success" ? data.Message : null; },
                    model: model
                },
                error: function (e) {
                    if (e.status === "customerror") {
                        Synergos.showException(e.errors);
                    } else if (e.status === "validation") {
                        Synergos.showValidation(e.errorThrown);
                    } else {
                        //kendo.alert("Status: " + e.status + "<br/>Error: " + e.errorThrown + "<br/>Message: " + e.errors);
                        Synergos.showException(e.errorThrown);
                    }
                },
                sync: function(e) {
                    if (typeof callbacks !== "undefined" && $.isFunction(callbacks.onDataSourceSync))
                        callbacks.onDataSourceSync(e);
                    //console.log({ sync: e });
                },
                requestEnd: function (e) {
                    if (typeof callbacks !== "undefined" && $.isFunction(callbacks.onDataSourceRequestEnd))
                        callbacks.onDataSourceRequestEnd(e);
                    //console.log({ requestEnd: e });
                }
            };

            if (Synergos.isDefined(dataSourceParams.controllerName)) {
                params.transport = {
                    read: function (e) {
                        callbacks.dataSourceRead(e, window.applicationBaseUrl + "/api/" + dataSourceParams.controllerName + "/" + dataSourceParams.methodList);
                    },
                    create: function (e) {
                        callbacks.dataSourceCreate(e, window.applicationBaseUrl + "/api/" + dataSourceParams.controllerName + "/create");
                    },
                    update: function (e) {
                        callbacks.dataSourceUpdate(e, window.applicationBaseUrl + "/api/" + dataSourceParams.controllerName + "/update");
                    },
                    destroy: function (e) {
                        callbacks.dataSourceDelete(e, window.applicationBaseUrl + "/api/" + dataSourceParams.controllerName + "/delete");
                    },
                    parameterMap: function (parameters, type) {
                        if (typeof callbacks !== "undefined" && $.isFunction(callbacks.onSetTransportParameterMap))
                            callbacks.onSetTransportParameterMap(parameters, type);
                        return parameters;
                    }
                };
            }

            if (callbacks !== null && typeof callbacks !== "undefined" && $.isFunction(callbacks.onSetDataSourceParams))
                callbacks.onSetDataSourceParams(params);

            return new kendo.data.DataSource(params);
        }

        Grid.prototype.Selected = function() {
            var selected = this.kendoGrid.select();

            if (selected.length === 0) {
                Synergos.showWarning("No row selected.");
                return null;
            }

            return this.kendoGrid.dataItem(selected[0]);
        };

        Grid.prototype.Edit = function() {
            this.kendoGrid.editRow(this.kendoGrid.select());
        };

        Grid.prototype.Delete = function() {
            this.kendoGrid.removeRow(this.kendoGrid.select());
        };

        Grid.prototype.View = function() {
            var selected = this.Selected();

            if (selected === null)
                return;

            if (typeof this.callbacks !== "undefined" && $.isFunction(this.callbacks.onView))
                this.callbacks.onView(selected);
        };

        Grid.prototype.Refresh = function() {
            this.kendoGrid.dataSource.read();
            this.kendoGrid.refresh();

            this.setActiveToolbarButtonsEnabled(false);
        };

        Grid.InputMediumEditor = function(container, options) {
            //console.log({ container: container, options: options, required: modelField });
            $('<input class="k-input k-textbox" name="' + options.field + '"  style="width: 50%;"/>')
                .prop("required", isFieldRequired(options.model.fields[options.field]))
                .appendTo(container);
        };

        Grid.PopupDateEditor = function(container, options) {
            $('<input data-text-field="' + options.field + '" data-value-field="' + options.field + '" data-bind="value:' + options.field + '" data-format="' + options.format + '" class="kendoInputRightAlign"/>')
                .prop("required", isFieldRequired(options.model.fields[options.field]))
                .appendTo(container)
                .kendoDatePicker({
                    format: "yyyy-MM-dd",
                    parseFormats: ["yyyy-MM-dd"]
                });
        };

        Grid.PopupDateTimeEditor = function (container, options) {
            $('<input data-text-field="' + options.field + '" data-value-field="' + options.field + '" data-bind="value:' + options.field + '" data-format="' + options.format + '"/>')
                .prop("required", isFieldRequired(options.model.fields[options.field]))
                .appendTo(container)
                .kendoDatePicker({
                    format: "yyyy-MM-dd HH:mm",
                    parseFormats: ["yyyy-MM-dd HH:mm"]
                });
        };

        Grid.TextBoxEditor = function(container, options) {
            $('<textarea class="k-textbox" name="' + options.field + '"  style="height: 60px; width: 90%;"/>')
                .prop("required", isFieldRequired(options.model.fields[options.field]))
                .appendTo(container);
        };

        Grid.RichTextEditor = function(container, options) {
            $('<textarea name="' + options.field + '"  style="width: 400px"/>')
                .prop("required", isFieldRequired(options.model.fields[options.field]))
                .appendTo(container)
                .kendoEditor({ tools: [] });
        };

        Grid.NumericTextBoxEditor = function(container, options) {
            $('<input data-bind="value:' + options.field + '" class="kendoInputRightAlign"/>')
                .prop("required", isFieldRequired(options.model.fields[options.field]))
                .appendTo(container)
                .kendoNumericTextBox({
                    format: options.format
                });
        };

        function isFieldRequired(field) {
            return Synergos.isDefined(field.validation) && Synergos.isDefined(field.validation.required) && field.validation.required === true;
        }

        return Grid;
    })()
});
