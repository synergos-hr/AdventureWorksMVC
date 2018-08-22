var Synergos = Synergos || {};
var SynApp   = SynApp   || {};

(function () {
    // https://stackoverflow.com/questions/17367736/jquery-ui-dialog-missing-close-icon
    var bootstrapButton = $.fn.button.noConflict(); // return $.fn.button to previously assigned value
    $.fn.bootstrapBtn = bootstrapButton;            // give $().bootstrapBtn the Bootstrap functionality

    Synergos.Init();

    //requirejs.config(
    //    {
    //        baseUrl: window.applicationBaseUrl + '/Scripts/',
    //        paths: {
    //            jquery: './lib/jquery-3.1.1',
    //            kendo: './Kendo/2017.1.118/kendo.web.min',
    //            kendoCulture: './Kendo/2017.1.118/cultures/kendo.culture.hr-HR.min'
    //            //app': './app'
    //        },
    //        shim: {
    //            'kendoCulture': {
    //                deps: ['kendo']
    //            },
    //            'kendo': {
    //                exports: 'kendo',
    //                deps: ['jquery']
    //            }
    //        }
    //    }
    //);

    //require([
    //    'require',
    //    'jquery'
    //], function (require, $) {
    //    domReady(function () {
    //        require(['kendoCulture']);

    //        kendo.culture("hr-HR");

    //        //$("#tabstrip").kendoTabStrip({
    //        //    animation:{
    //        //        open:{
    //        //            effects:"fadeIn"
    //        //        }
    //        //    }
    //        //});
    //    });
    //});

    //require(['alerter'],
    //    function (alerter) {
    //        alerter.showMessage();
    //    });

    //w2utils.locale(window.applicationBaseUrl + '/Content/w2ui_locales/hr-hr.txt');

    //w2utils.settings['dataType'] = 'JSON';
    //w2utils.settings['groupSymbol'] = ',';

    $('#userMenuLogout').click(function () {
        $('#userMenuLogoutForm').submit();
    });
})();
window.addEventListener("load", function () {
    $(".display-after-load").show();

    $(".wait-loading-window").remove();
});