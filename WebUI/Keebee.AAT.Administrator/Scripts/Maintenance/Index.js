/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Maintenance/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

; (function ($) {

    maintenance.index = {
        init: function() {
            var cmdReinstall = $("#reinstall");
            var cmdRestart = $("#restart");
            var cmdKillDisplay = $("#kill-display");
            var cmdUninstall = $("#uninstall");

            cmdRestart.click(function () {
                BootstrapDialog.show({
                    title: "Service Utilities",
                    message: "Restart all R2G2 Windows Services?",
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

            cmdKillDisplay.click(function () {
                BootstrapDialog.show({
                    title: "Kill Display",
                    message: "Kill the display application?",
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
                                killDisplay();
                            }
                        }
                    ]
                });
            });

            // --- obsolete
            cmdReinstall.click(function() {
                BootstrapDialog.show({
                    title: "Service Utilities",
                    message: "Install or uninstall/reinstall all R2G2 Windows Services?",
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
                                execute("ReinstallServices", "Installing services...", "All services installed successfully");
                            }
                        }
                    ]
                });
            });

            cmdUninstall.click(function () {
                BootstrapDialog.show({
                    title: "Service Utilities",
                    message: "Uninstall all R2G2 Windows Services?",
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
                                execute("UninstallServices", "Uninstalling services...", "All services uninstalled successfully");
                            }
                        }
                    ]
                });
            });

            function execute(functionName, waitMessage, verbage) {
                BootstrapDialog.show({
                    type: BootstrapDialog.TYPE_INFO,
                    title: "Service Utilities",
                    message: waitMessage,
                    closable: false,
                    onshown: function(dialog) {
                        $("body").css("cursor", "wait");

                        $.ajax({
                            type: "GET",
                            async: true,
                            traditional: true,
                            url: site.url + "Maintenance/" + functionName,
                            success: function (data) {
                                $("body").css("cursor", "default");
                                dialog.close();

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
                            error: function (data) {
                                $("body").css("cursor", "default");
                                dialog.close();
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
                    }
                });
                
            };

            function killDisplay() {
                $.ajax({
                    type: "GET",
                    async: true,
                    traditional: true,
                    url: site.url + "Maintenance/KillDisplay",
                    success: function (data) {
                        $("body").css("cursor", "default");

                        if (data.length === 0)
                            BootstrapDialog.show({
                                title: "Success",
                                closable: false,
                                type: BootstrapDialog.TYPE_SUCCESS,
                                message: "Display has been killed",
                                buttons: [
                                {
                                    label: "Close",
                                    action: function (dialog) {
                                        dialog.close();
                                    }
                                }]
                            });
                       }
                });
            }
        }
    }
})(jQuery);