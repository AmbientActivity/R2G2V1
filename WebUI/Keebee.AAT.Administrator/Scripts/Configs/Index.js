/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Configs/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

; (function ($) {

    configs.index = {
        init: function() {

            // buttons
            var cmdActivate = $("#activate");

            // inputs
            var ddlConfigs = $("#ddlConfigs");

            // activate the selected configuration
            cmdActivate.click(function() {
                var configId = ddlConfigs.val();

                $.ajax({
                    type: "POST",
                    async: false,
                    url: site.url + "Configs/Activate/",
                    data: { configId: configId },
                    dataType: "json",
                    traditional: true,
                    failure: function() {
                        $("body").css("cursor", "default");
                        $("#validation-container").html("");
                    },
                    success: function(data) {
                    },
                    error: function(data) {
                    }
                });
            });
        }
    }
})(jQuery);