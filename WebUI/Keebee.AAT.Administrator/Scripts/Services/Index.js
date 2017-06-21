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

            cmdSave.prop("disabled", true);

            function beaconWatcherServiceHasChanged() {
                var isCheckedBeaconWatcher = $.trim(chkBeaconWatcher.is(":checked"));
                if ((isCheckedBeaconWatcher === "false" && config.IsInstalledBeaconWatcherService === 1)
                    || (isCheckedBeaconWatcher === "true" && config.IsInstalledBeaconWatcherService === 0)) {
                    return true;
                }

                return false;
            }

            function videoCaptureServiceHasChanged() {
                var isCheckedVideoCapture = $.trim(chkVideoCapture.is(":checked"));
                if ((isCheckedVideoCapture === "false" && config.IsInstalledVideoCaptureService === 1)
                    || (isCheckedVideoCapture === "true" && config.IsInstalledVideoCaptureService === 0)) {
                    return true;
                }

                return false;
            }

            function enableDetail() {
                if (beaconWatcherServiceHasChanged() || videoCaptureServiceHasChanged()) {
                    cmdSave.prop("disabled", false);
                    return;
                }

                cmdSave.prop("disabled", true);
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
                                    type: "GET",
                                    title: "Service Activation/Deactivation",
                                    waitMessage: "Saving settings...",
                                    params: {
                                        activateBeaconWatcher: $.trim(chkBeaconWatcher.is(":checked")),
                                        activateVideoCapture: $.trim(chkVideoCapture.is(":checked"))
                                    }
                                }).then(function(data) {
                                    if (data.ErrorMessage === null) {
                                        var beaconServiceHasChanged = beaconWatcherServiceHasChanged();

                                        $.extend(config, data.ServiceSettings);
 
                                        chkBeaconWatcher.prop("checked", (config.IsInstalledBeaconWatcherService === 1));
                                        chkVideoCapture.prop("checked", (config.IsInstalledVideoCaptureService === 1));
                                        cmdSave.attr("disabled", "disabled");

                                        if (beaconServiceHasChanged)
                                            window.location.reload();                               
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