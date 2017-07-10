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
                utilities.confirm.show({
                    type: BootstrapDialog.TYPE_PRIMARY,
                    title: "Service Utilities",
                    message: "Restart all R2G2 Windows Services?",
                    buttonOKClass: "btn-edit"
                })
                .then(function(confirm) {
                    if (confirm) {
                        utilities.job.execute({
                            url: site.url + "Maintenance/RestartServices",
                            waitMessage: "Restarting services..."
                        });
                    }
                });
            });

            cmdKillDisplay.click(function () {
                utilities.confirm.show({
                    type: BootstrapDialog.TYPE_PRIMARY,
                    title: "Kill Display",
                    message: "Kill the display application?",
                })
                .then(function (confirm) {
                    if (confirm) {
                        utilities.job.execute({
                            url: "Maintenance/KillDisplay",
                            waitMessage: "Shutting down...",
                            successVerbag: "Display has been shutdown."
                        });
                    }
                });
            });

            cmdClearServiceLogs.click(function () {
                utilities.confirm.show({
                    type: BootstrapDialog.TYPE_PRIMARY,
                    title: "Clear Service Logs",
                    message: "Clear all R2G2 Service logs?",
                })
                .then(function(confirm) {
                    if (confirm) {
                        utilities.job.execute({
                            url: site.url + "Maintenance/ClearServiceLogs",
                            waitMessage: "Clearing..."
                        });
                    }
                });
            });         

            // --- obsolete
            cmdReinstall.click(function () {
                utilities.confirm.show({
                    type: BootstrapDialog.TYPE_PRIMARY,
                    title: "Service Utilities",
                    message: "Install or uninstall/reinstall all R2G2 Windows Services?"
                })
                .then(function(confirm) {
                    if (confirm) {
                        utilities.job.execute({
                            url: site.url + "Maintenance/ReinstallServices",
                            action: "ReinstallServices",
                            title: "Service Utilities",
                            waitMessage: "Installing services...",
                            successVerbage: "All services installed successfully"
                        });
                    }
                });
            });

            cmdUninstall.click(function () {
                utilities.confirm.show({
                    type: BootstrapDialog.TYPE_PRIMARY,
                    title: "Service Utilities",
                    message: "Uninstall all R2G2 Windows Services?"
                })
                .then(function(confirm) {
                    if (confirm) {
                        utilities.job.execute({
                            url: site.url + "Maintenance/UninstallServices",
                            title: "Service Utilities",
                            waitMessage: "Uninstalling services...",
                            successVerbage: "All services uninstalled successfully"
                        });
                    }
                });
            });
        }
    }
})(jQuery);