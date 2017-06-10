/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Maintenance/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

; (function ($) {

    maintenance.index = {
        init: function (values) {

            var config = {
                isActiveBeaconService: 0
            }

            $.extend(config, values);

            var cmdUninstall = $("#uninstall");
            var cmdReinstall = $("#reinstall");
            var cmdRestart = $("#restart");
            var cmdKillDisplay = $("#kill-display");
            var cmdClearServiceLogs = $("#clear-service-logs");
            var cmdInstallBeaconService = $("#install-beacon-service");

            if (config.isActiveBeaconService === 1) {
                cmdInstallBeaconService.html("Uninstall Beacons");
                cmdInstallBeaconService.removeClass("btn-success");
                cmdInstallBeaconService.addClass("btn-danger");
            } else {
                cmdInstallBeaconService.html("Install Beacons");
                cmdInstallBeaconService.removeClass("btn-danger");
                cmdInstallBeaconService.addClass("btn-success");
            }

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

            cmdInstallBeaconService.click(function () {
                var message;
                var actionName;
                var inProgressDesc;
                var completedDesc;
                var buttonText;
                var cssClassNew;
                var cssClassOld;

                if (config.isActiveBeaconService === 0) {
                    message = "Install Beacon Service?";
                    actionName = "InstallBeaconService";
                    inProgressDesc = "Installing Beacon Service...";
                    completedDesc = "Beacon Service installed successfully";
                    buttonText = "Uninstall Beacons";
                    config.isActiveBeaconService = 1;
                    cssClassNew = "btn-danger";
                    cssClassOld = "btn-success";
                } else {
                    message = "Uninstall Beacon Service?";
                    actionName = "UninstallBeaconService";
                    inProgressDesc = "Uninstalling Beacon Service...";
                    completedDesc = "Beacon Service uninstalled successfully";
                    buttonText = "Install Beacons";
                    config.isActiveBeaconService = 0;
                    cssClassNew = "btn-success";
                    cssClassOld = "btn-danger";
                }

                BootstrapDialog.show({
                    title: "Service Utilities",
                    message: message,
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
                                execute(actionName, inProgressDesc, completedDesc);
                                cmdInstallBeaconService.html(buttonText);
                                cmdInstallBeaconService.removeClass(cssClassOld);
                                cmdInstallBeaconService.addClass(cssClassNew);
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

                        $.get({
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
                                            action: function (dlg) {
                                                dlg.close();
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
                                            action: function (dlg) {
                                                dlg.close();
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
                                        action: function (dlg) {
                                            dlg.close();
                                        }
                                    }]
                                });
                            }
                        });
                    }
                });
                
            };

            function killDisplay() {
                $.get({
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

            function clearServiceLogs() {
                $.get({
                    url: site.url + "Maintenance/ClearServiceLogs",
                    success: function(data) {
                        $("body").css("cursor", "default");

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
                    }
                });
            }
        }
    }
})(jQuery);