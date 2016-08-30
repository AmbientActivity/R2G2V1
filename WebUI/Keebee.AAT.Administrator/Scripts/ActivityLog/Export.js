/*!
 * 1.0 Keebee AAT Copyright © 2015
 * ActivityLog/Export.js
 * Author: John Charlton
 * Date: 2016-08
 */

; (function ($) {

    activitylog.exporter = {
        init: function () {

            // inputs
            var dtDate = $("#export-date");

            // buttons
            var cmdExport = $("#export");

            // ------------------ private --------------------------

            function initializeExportLinkButton() {
                cmdExport.attr("href", "DoExport?date=" + dtDate.val());
            }

            function initializeDate() {
                var today = new Date();
                var dd = today.getDate();
                var mm = today.getMonth() + 1;
                var yyyy = today.getFullYear();
                if (dd < 10) dd = "0" + dd;
                if (mm < 10) mm = "0" + mm;
                today = mm + "/" + dd + "/" + yyyy;
                dtDate.val(today);
            }

            function loadScreen() {
                initializeDate();
                initializeExportLinkButton();
            }

            function validateInput() {
                if (dtDate.val() === "")
                    cmdExport.attr("disabled", "disabled");
                else {
                    cmdExport.removeAttr("disabled");
                    initializeExportLinkButton();
                }
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

            // ------------------ events --------------------------
        }
    }
})(jQuery);