/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Services/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

; (function ($) {

    winservices.management = {
        init: function () {

            // buttons
            var cmdRestart = $("#restart");

            // ------------------ events --------------------------
            cmdRestart.click(function () {
                BootstrapDialog.show({
                    title: "Phidget Service Restart",
                    message: "Restart Phidget Service?",
                    closable: false,
                    buttons: [
                        {
                            label: "Cancel",
                            action: function (dialog) {
                                dialog.close();
                            }
                        }, {
                            label: "OK",
                            cssClass: "btn-primary",
                            action: function (dialog) {
                                dialog.close();
                                $.ajax({
                                    type: "GET",
                                    url: site.url + "Services/RestartServices",
                                    dataType: "json",
                                    traditional: true,
                                    async: false,
                                    success: function (data) {
                                    },
                                    error: function (data) {
                                    }
                                });
                                
                                $("body").css("cursor", "default");
                            }
                        }
                    ]
                });
            });

            // ------------------ events --------------------------
        }
    }
})(jQuery);