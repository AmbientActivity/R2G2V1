/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Utilities/InProgress.js
 * Author: John Charlton
 * Date: 2017-06
 */

; (function ($) {

    utilities.inprogress = {
        show: function (options) {
            var config = {
                message: "Saving..."
            };

            return new Promise(function (resolve) {

                if ((typeof options !== "undefined") && (options !== null)) {
                    if (options.message === null) reject("Wait Message cannot be null");
                }

                $.extend(config, options);

                var inProgessDialog = new
                BootstrapDialog({
                    message: "<div class='message-please-wait'><h4>" + config.message + "</h4></div>",
                    closable: false,
                    onshown: function(dialog) {
                        resolve(dialog);
                    }
                });

                inProgessDialog.realize();
                inProgessDialog.getModalContent().css("width", "auto");
                inProgessDialog.getModalHeader().hide();
                inProgessDialog.getModalFooter().hide();
                inProgessDialog.getModalBody().addClass("btn-edit");
                inProgessDialog.getModalBody().css("color", "#fff");
                inProgessDialog.open();
            });
        }
    }
})(jQuery);