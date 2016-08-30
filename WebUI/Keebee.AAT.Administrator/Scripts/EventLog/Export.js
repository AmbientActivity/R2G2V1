/*!
 * 1.0 Keebee AAT Copyright © 2015
 * EventLog/Export.js
 * Author: John Charlton
 * Date: 2016-08
 */

; (function ($) {

    eventlog.exporter = {
        init: function () {

            // turn off caching
            $.ajaxSetup({ cache: false });

            // inputs
            var dtDate = $("#export-date");

            // buttons
            var cmdExport = $("#export");

            // ------------------ private --------------------------

            function loadScreen() {
                cmdExport.attr("disabled", "disabled");
                //var currentDate = new Date();
                //dtDate.val(currentDate);
            }

            function validateInput() {
                if (dtDate.val() === "")
                    cmdExport.attr("disabled", "disabled");
                else
                    cmdExport.removeAttr("disabled");
            }

            // ------------------ private --------------------------

            // ----------------- initialization -------------------

            loadScreen();

            // ----------------- initialization -------------------

            // ------------------ events --------------------------

            // select the date
            dtDate.change(function () {
                validateInput();
            });

            // upload, validate and import the file
            cmdExport.click(function () {
                var date = dtDate.val();

                $.ajax({
                    url: site.url + "EventLog/DoExport/?date=" + date,
                    type: "POST",
                    contentType: false,
                    processData: false,
                    success: function (data) {
                    },
                    error: function (data) {
                    }
                });
            });
        }
    }
})(jQuery);