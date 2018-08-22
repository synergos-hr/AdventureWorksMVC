'use strict';

var Synergos = Synergos || {};

$.extend(Synergos, {
    View: {
    },
    EditForm: {
        HasChanges: false,
        Submitting: false
    },
    Models: {
        ViewModel: null
    },
    Init: function () {
        Synergos.Kendo.Init();

        //Synergos.Layout.OnResize();

        ////menu highlighting
        //Synergos.Layout.MenuSetup();
        ////other kendo controls
        //Synergos.Kendo.ApplyCommonControls();

        window.onbeforeunload = Synergos.onBeforeUnloadWindow;

        Synergos.EditForm.Submitting = false;
        Synergos.EditForm.HasChanges = false;
    },
    Enums: {
        AlertType: {
            Message: 0,
            Error: 1,
            Warning: 2,
            LoadingMessage: 3
        },
        MultiGridLayout: {
            Horizontal: 0,
            Vertical: 1
        }
    },
    Constants: {
        GuidEmpty: '00000000-0000-0000-0000-000000000000'
    },
    onBeforeUnloadWindow: function (e) {
        if (!Synergos.EditForm.Submitting && Synergos.EditForm.HasChanges) {
            var message = "Data is changed.\r\nIf you leave page changes will be lost.";
            e.returnValue = message;
            return message;
        }

        delete e.returnValue;
        return undefined;
    },
    jsonEscape: function (str) {
        return str.replace(/\n/g, "\\n").replace(/\r/g, "\\r").replace(/\t/g, "\\t");  //.replace(/"/g, '\\"');
    },
    isDefined: function(o) {
        return typeof(o) !== "undefined" && o !== null;
    },
    setIfNotDefined: function (obj, newValue) {
        if (!Synergos.isDefined(obj))
            return newValue;
        return obj;
    },
    detectIE: function() {
        var ua = window.navigator.userAgent;

        var msie = ua.indexOf("MSIE ");
        if(msie > 0) {
            // IE 10 or older => return version number
            return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
        }

        var trident = ua.indexOf("Trident/");
        if(trident > 0) {
            // IE 11 => return version number
            var rv = ua.indexOf("rv:");
            return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
        }

        var edge = ua.indexOf("Edge/");
        if(edge > 0) {
            // Edge (IE 12+) => return version number
            return parseInt(ua.substring(edge + 5, ua.indexOf('.', edge)), 10);
        }

        // other browser
        return false;
    }
});
