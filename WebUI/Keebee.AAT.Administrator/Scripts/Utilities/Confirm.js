/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Utilities/Confirm.js
 * Author: John Charlton
 * Date: 2017-06
 */

; (function ($) {

    utilities.confirm = {
        show: function (options) {
            var config = {
                type: BootstrapDialog.TYPE_DEFAULT,
                title: null,
                message: null,
                buttonCancel: "Cancel",
                buttonOK: "OK",
                buttonOKClass: "btn-edit"
            };

            if ((typeof options !== "undefined") && (options !== null)) {
                if (options.type === null) reject("Type cannot be null");
                if (options.title === null) reject("Title cannot be null");
                if (options.message === null) reject("Message cannot be null");
                if (options.buttonCancel === null) reject("Cancel text cannot be null");
                if (options.buttonOK === null) reject("OK text cannot be null");
            }

            $.extend(config, options);

            return new Promise(function(resolve) {
                BootstrapDialog.show({
                    type: config.type,
                    title: config.title,
                    message: config.message,
                    closable: false,
                    buttons: [{
                        label: config.buttonCancel,
                        action: function (dialog) {
                            dialog.close();
                            resolve(false);
                        }
                    }, {
                        label: config.buttonOK,
                        cssClass: config.buttonOKClass,
                        action: function (dialog) {
                            dialog.close();
                            resolve(true);
                        }
                    }]
                });
            });
        }
    }
})(jQuery);;