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
                isInstalledBeaconWatcher: 0,
                isInstalledVideoCapture: 0
            };

            $.extend(config, options);

            var cmdSave = $("#save");

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
                                utilities.service.execute(
                                {
                                    controller: "Services",
                                    action: "SaveSettings",
                                    waitMessage: "Saving settings...",
                                    verbage: "Settings saved successfully",
                                    params: {
                                        activateBeaconWatcher: $.trim($("#chkIsInstalledBeaconWatcher").is(":checked")),
                                        activateVideoCapture: $.trim($("#chkIsInstalledVideoCapture").is(":checked"))
                                    }
                                }).then(function(data) {
                                    if (data.ErrorMessage === null) {
                                        var json = data.ServiceSettings;
                                        $(".chkIsInstalledBeaconWatcher").prop("checked", (json.IsInstalledBeaconWatcherService === 1));
                                        $(".chkIsInstalledVideoCapture").prop("checked", (json.IsInstalledVideoCaptureService === 1));
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