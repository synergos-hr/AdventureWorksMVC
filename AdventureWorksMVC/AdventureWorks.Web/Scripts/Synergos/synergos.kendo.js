'use strict';

var Synergos = Synergos || {};

$.extend(Synergos, {
    Kendo: {
        Culture: 'en-EN',
        ExtendKendoGridDelete: function() {
            (function ($, kendo) {
                var _originalRemoveRow = kendo.ui.Grid.fn.removeRow; // store original function

                var confirmDelete = function (row, grid) {
                    var kendoWindow = $("<div />").kendoWindow({
                        title: document.title,
                        resizable: false,
                        modal: true,
                        width: '400px',
                        draggable: true,
                        actions: ['Close']
                    });
                    kendoWindow.data('kendoWindow')
                        .content($('<div class="k-window-confirm-delete"><p>' + grid.options.messages.editable.confirmation + '</p><div class="dialog-buttons"><button class="confirm-ok k-button k-button-icontext"><i class="fa fa-trash-o"></i> ' + grid.options.messages.editable.confirmDelete + '</button><a href="#" class="confirm-cancel k-button k-button-icontext"><i class="fa fa-times"></i> ' + grid.options.messages.editable.cancelDelete + '</a></div></div>'))
                        .center().open();
                    kendoWindow
                        .find(".confirm-ok, .confirm-cancel")
                        .click(function () {
                            kendoWindow.data("kendoWindow").close();
                            kendoWindow.data("kendoWindow").destroy();
                            if ($(this).hasClass("confirm-ok")) {
                                grid._removeRow(row);
                            }
                        });
                };

                var extendedGrid = kendo.ui.Grid.extend({
                    removeRow: function (row) { // new function
                        var that = this;
                        confirmDelete(row, that);
                    }
                });

                kendo.ui.plugin(extendedGrid);
            }(window.kendo.jQuery, window.kendo));
        },
        Init: function (culture) {
            if (!culture) 
                culture = this.Culture;

            if (typeof (kendo) !== "undefined" && kendo !== null && typeof(kendo.ui) !== "undefined" ) {
                kendo.culture(culture);

                if (kendo.ui.Grid)
                    this.ExtendKendoGridDelete();
            }
        },
        ApplyCommonControls: function () {
            //tabStrip
            $('.tabStripCommon').kendoTabStrip();

            //dropDowns
            //$('.lookupDropDown').each(function () {
            //    var lookupEntity = $(this).attr('lookupEntity');
            //    if (lookupEntity === "")
            //        lookupEntity = undefined;
            //    new Synergos.GridDropDown($(this).attr('id'), $(this).attr('displayfield'), undefined, lookupEntity);
            //});

            //$('.lookupComboBox').each(function () {
            //    var lookupEntity = $(this).attr('lookupEntity');
            //    if (lookupEntity === "") 
            //        lookupEntity = undefined;

            //    var lookupColumns = $(this).attr('lookupColumns');
            //    if (lookupColumns === "") 
            //        lookupColumns = undefined;
            //    new Synergos.GridComboBox($(this).attr('id'), $(this).val(), $(this).attr('displayfield'), undefined, lookupEntity, lookupColumns);
            //});

            //$('.booleanLookup').each(function () {
            //    var trueCaption = $(this).attr('trueCaption');
            //    var falseCaption = $(this).attr('falseCaption');
            //    new Synergos.BooleanLookup(this, trueCaption, falseCaption, $(this).val());
            //});
        }
    }
});
