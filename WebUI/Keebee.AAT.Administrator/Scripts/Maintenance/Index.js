/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Maintenance/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

; (function ($) {

    maintenance.index = {
        init: function () {
            var cmdUninstall = $("#uninstall");
            var cmdReinstall = $("#reinstall");
            var cmdRestart = $("#restart");
            var cmdKillDisplay = $("#kill-display");
            var cmdClearServiceLogs = $("#clear-service-logs");

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
                                utilities.job.execute(
                                {
                                    type: "GET",
                                    controller: "Maintenance",
                                    action: "RestartServices",
                                    title: "Service Utilities",
                                    waitMessage: "Restarting services...",
                                    successVerbage: "All services restarted successfully"
                                });
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

            cmdClearServiceLogs.click(function() {
                BootstrapDialog.show({
                    title: "Clear Service Logs",
                    message: "Clear all R2G2 Service logs?",
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
                                clearServiceLogs();
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
                                utilities.job.execute(
                                {
                                    type: "GET",
                                    controller: "Maintenance",
                                    action: "ReinstallServices",
                                    title: "Service Utilities",
                                    waitMessage: "Installing services...",
                                    successVerbage: "All services installed successfully"
                                });
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
                                utilities.job.execute(
                                {
                                    type: "GET",
                                    controller: "Maintenance",
                                    action: "UninstallServices",
                                    title: "Service Utilities",
                                    waitMessage: "Uninstalling services...",
                                    successVerbage: "All services uninstalled successfully"
                                });
                            }
                        }
                    ]
                });
            });

            function killDisplay() {
                $.get(site.url + "Maintenance/KillDisplay")
                    .done(function (data) {
                        //$("body").css("cursor", "default");

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
                });
            }

            function clearServiceLogs() {
                $.get(site.url + "Maintenance/ClearServiceLogs")
                    .done(function(data) {
                        //$("body").css("cursor", "default");

                        if (data.length === 0)
                            BootstrapDialog.show({
                                title: "Success",
                                closable: false,
                                type: BootstrapDialog.TYPE_SUCCESS,
                                message: "R2G2 Service Logs have been cleared",
                                buttons: [
                                    {
                                        label: "Close",
                                        action: function(dialog) {
                                            dialog.close();
                                        }
                                    }
                                ]
                            });
                });
            }
        }
    }
})(jQuery);