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
                focus: "",
                buttonOK: "OK",
                buttonOKClass: "btn-edit",
                buttonCancel: "Cancel",
                params: {},
                callback: function() {}
            };

            if ((typeof options !== "undefined") && (options !== null)) {
                if (options.url === null) reject("URL cannot be null");
            }

            $.extend(config, options);

            $.get(config.url, config.params)
                .done(function(message) {
                    BootstrapDialog.show({
                        type: config.type,
                        title: config.title,
                        message: $("<div></div>").append(message),
                        onshown: function() {
                            $("#" + config.focus).focus();
                        },
                        closable: false,
                        buttons: [
                            {
                                label: config.buttonCancel,
                                action: function(dialog) {
                                    dialog.close();
                                }
                            }, {
                                label: config.buttonOK,
                                hotkey: 13, // enter
                                cssClass: config.buttonOKClass,
                                action: function (dialog) {
                                    config.callback(dialog);
                                }
                            }
                        ]
                    });
                })
                .error(function(request) {
                    utilities.alert.show({
                        title: "Partial View Load Error",
                        type: BootstrapDialog.TYPE_DANGER,
                        message: "The following error occured:\n" + request.responseText
                    });
                });
            }
    }
})(jQuery);;