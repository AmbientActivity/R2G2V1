/*!
 * 1.0 Keebee AAT Copyright © 2017
 * Utilities/SessionExpiry.js
 * Author: John Charlton
 * Date: 2017-08
 */

; (function ($) {

    utilities.sessionexpired = {
        show: function () {
            utilities.alert.show({
                title: "Session Timeout",
                type: BootstrapDialog.TYPE_INFO,
                message: "The action cannot be completed because your session has expired.\n" +
                    "Please log back in and try again."
            })
            .then(function () {
                location.reload();
            });
        }
    }
})(jQuery);