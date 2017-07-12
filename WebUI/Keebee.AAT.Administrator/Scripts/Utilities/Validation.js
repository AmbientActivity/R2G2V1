/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Utilities/Validation.js
 * Author: John Charlton
 * Date: 2017-06
 */

; (function ($) {

    utilities.validation = {
        show: function (options) {
            var config = {
                container: "",
                messages: [],
                beginWithLineBreak: false
            };

            if ((typeof options !== "undefined") && (options !== null)) {
                if (options.container === null) return;
                if (options.container.length === 0) return;
            }

            $.extend(config, options);

            $("#" + config.container).show();
            $("#" + config.container).html("");

            var html = config.beginWithLineBreak ? "</br><ul>" : "<ul>";

            for (var i = 0; i < config.messages.length; i++) {
                var msg = config.messages[i];
                html = html + "<li>" + msg + "</li>";
            }

            html = html + "</ul>";
            $("#" + config.container).append(html);
        }
    }
})(jQuery);;