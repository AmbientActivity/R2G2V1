/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Services/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

; (function ($) {

    winservices.management = {
        init: function() {
            var cmdReinstall = $("#reinstall");
            var cmdRestart = $("#restart");

            cmdReinstall.click(function() {
                BootstrapDialog.show({
                    title: "Maintenance Utilities",
                    message: "Uninstall/Reinstall Keebee AAT Services?",
                    closable: false,
                    buttons: [
                        {
                            label: "Cancel",
                            action: function(dialog) {
                                dialog.close();
                            }
                        }, {
                            label: "OK",
                            cssClass: "btn-primary",
                            action: function(dialog) {
                                dialog.close();
                                execute("ReinstallServices", "Reinstalling services...", "All services reinstalled successfully");
                            }
                        }
                    ]
                });
            });

            cmdRestart.click(function () {
                BootstrapDialog.show({
                    title: "Maintenance Utilities",
                    message: "Restart Keebee AAT Services?",
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
                                execute("RestartServices", "Restarting services...", "All services restarted successfully");
                            }
                        }
                    ]
                });
            });

            function execute(functionName, waitMessage, verbage) {
                $.blockUI({ message: "<h4>" + waitMessage + "</h4>" });
                $("body").css("cursor", "wait");

                $.ajax({
                    type: "GET",
                    async: true,
                    traditional: true,
                    url: site.url + "Utilities/" + functionName,
                    success: function(data) {
                        $("body").css("cursor", "default");
                        $.unblockUI();

                        if (data.length === 0)
                            BootstrapDialog.show({
                                title: "Success",
                                closable: false,
                                type: BootstrapDialog.TYPE_SUCCESS,
                                message: verbage,
                                buttons: [
                                {
                                    label: "Close",
                                    action: function (dialog) {
                                        dialog.close();
                                    }
                                }]
                            });
                        else
                            BootstrapDialog.show({
                                title: "Error",
                                closable: false,
                                type: BootstrapDialog.TYPE_DANGER,
                                message: "The following error occured:\n" + data,
                                buttons: [{
                                    label: "Close",
                                    action: function (dialog) {
                                        dialog.close();
                                    }
                                }]
                            });
                    },
                    error: function(data) {
                        $("body").css("cursor", "default");
                        $.unblockUI();
                        BootstrapDialog.show({
                            title: "Error",
                            closable: false,
                            type: BootstrapDialog.TYPE_DANGER,
                            message: "The following error occured:\n" + data,
                            buttons: [{
                                label: "Close",
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }]
                        });
                    }
                });
            };
        }
    }
})(jQuery);