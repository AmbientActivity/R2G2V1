/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Services/Index.js
 * Author: John Charlton
 * Date: 2017-06
 */

; (function($) {

    services.index = {
        init: function (options) {

            var config = {
                IsInstalledBeaconWatcherService: 0,
                IsInstalledVideoCaptureService: 0
            };

            $.extend(config, options);

            var cmdSave = $("#save");
            var chkBeaconWatcher = $("#chkIsInstalledBeaconWatcher");
            var chkVideoCapture = $("#chkIsInstalledVideoCapture");

            var cmdSave = $("#save");

            cmdSave.attr("disabled", "disabled");

            function enableDetail() {
                var isCheckedBeaconWatcher = $.trim(chkBeaconWatcher.is(":checked"));
                var isCheckedVideoCapture = $.trim(chkVideoCapture.is(":checked"));

                if ((isCheckedBeaconWatcher === "false" && config.IsInstalledBeaconWatcherService === 1)
                    || (isCheckedBeaconWatcher === "true" && config.IsInstalledBeaconWatcherService === 0)) {
                    cmdSave.removeAttr("disabled");
                    return;
                }

                if ((isCheckedVideoCapture === "false" && config.IsInstalledVideoCaptureService === 1)
                    || (isCheckedVideoCapture === "true" && config.IsInstalledVideoCaptureService === 0)) {
                    cmdSave.removeAttr("disabled");
                    return;
                }

                cmdSave.attr("disabled", "disabled");
            }

            chkBeaconWatcher.change(function() {
                enableDetail();
            });

            chkVideoCapture.change(function () {
                enableDetail();
            });

            cmdSave.click(function () {
                BootstrapDialog.show({
                    title: "Service Settings",
                    message: "Save settings?",
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
                                    controller: "Services",
                                    action: "SaveSettings",
                                    title: "Service Activation/Deactivation",
                                    waitMessage: "Saving settings...",
                                    verbage: "Settings saved successfully",
                                    params: {
                                        activateBeaconWatcher: $.trim(chkBeaconWatcher.is(":checked")),
                                        activateVideoCapture: $.trim(chkVideoCapture.is(":checked"))
                                    }
                                }).then(function(data) {
                                    if (data.ErrorMessage === null) {
                                        $.extend(config, data.ServiceSettings);
 
                                        chkBeaconWatcher.prop("checked", (config.IsInstalledBeaconWatcherService === 1));
                                        chkVideoCapture.prop("checked", (config.IsInstalledVideoCaptureService === 1));
                                        cmdSave.attr("disabled", "disabled");
                                    }
                                });
                            }
                        }
                    ]
                });
            });
        }
    }
})(jQuery);