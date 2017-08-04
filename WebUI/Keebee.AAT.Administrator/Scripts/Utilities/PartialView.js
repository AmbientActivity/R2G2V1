/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Utilities/PartialView.js
 * Author: John Charlton
 * Date: 2017-06
 */

; (function ($) {

    utilities.partialview = {
        show: function (options) {
            var config = {
                url: null,
                title: null,
                type: BootstrapDialog.TYPE_INFO,
                focus: "txt",
                buttonOK: "OK",
                buttonOKClass: "btn-edit",
                okOnly: false,
                buttonCancel: "Cancel",
                params: {},
                callback: function () { },
                cancelled: function () { }
            };

            $.extend(config, options);

            $.get(config.url, config.params)
            .done(function (result) {
                if (typeof result.Success === "undefined") {
                    utilities.alert.show({
                        title: "Session Timeout",
                        type: BootstrapDialog.TYPE_INFO,
                        message: "Your session has expired.  Please login again to continue."
                    })
                    .then(function() {
                        location.reload();
                    });
                } else {
                    if (result.Success) {
                        BootstrapDialog.show({
                            type: config.type,
                            title: config.title,
                            message: $("<div></div>").append(result.Html),
                            onshown: function () {
                                $("#" + config.focus).focus();
                            },
                            closable: false,
                            buttons: getButtons()
                        });
                    } else {
                        utilities.alert.show({
                            title: "Error",
                            type: BootstrapDialog.TYPE_DANGER,
                            message: "The following error occured:\n" + result.ErrorMessage
                        });
                    }
                }
            })
            .error(function(error) {
                utilities.alert.show({
                    title: "Partial View Load Error",
                    type: BootstrapDialog.TYPE_DANGER,
                    message: "The following unexpected error occured:\n" + error.statusText
                });
            });

            function getButtons() {
                if (!config.okOnly) {
                    return [{
                            label: config.buttonCancel,
                            action: function(dialog) {
                                dialog.close();
                                config.cancelled();
                            }
                        }, {
                            label: config.buttonOK,
                            hotkey: 13, // enter
                            cssClass: config.buttonOKClass,
                            action: function(dialog) {
                                config.callback(dialog);
                            }
                        }];
                } else {
                    return [{
                            label: config.buttonOK,
                            hotkey: 13, // enter
                            cssClass: config.buttonOKClass,
                            action: function (dialog) {
                                config.callback(dialog);
                            }
                        }];
                }
            }
        }
    }
})(jQuery);