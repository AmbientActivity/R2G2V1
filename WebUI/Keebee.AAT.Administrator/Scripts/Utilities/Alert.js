/*!
 * 1.0 Keebee AAT Copyright © 2017
 * Utilities/Confirm.js
 * Author: John Charlton
 * Date: 2017-06
 */

; (function ($) {

    utilities.alert = {
        show: function (options) {
            var config = {
                type: BootstrapDialog.TYPE_INFO,
                title: null,
                message: null,
                buttonClose: "OK",
                buttonOKClass: "btn-default"
            };

            return new Promise(function (resolve) {

                if ((typeof options !== "undefined") && (options !== null)) {
                    if (options.type === null) reject("Type cannot be null");
                    if (options.title === null) reject("Title cannot be null");
                    if (options.message === null) reject("Message cannot be null");
                    if (options.buttonClose === null) reject("Close button text cannot be null");
                }

                $.extend(config, options);

                BootstrapDialog.show({
                    title: config.title,
                    closable: false,
                    type: config.type,
                    message: config.message,
                    buttons: [
                        {
                            label: config.buttonClose,
                            cssClass: config.buttonOKClass,
                            action: function (dialog) {
                                dialog.close();
                                resolve();
                            }
                        }
                    ]
                });
            });
        }
    }
})(jQuery);