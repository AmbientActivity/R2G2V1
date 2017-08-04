/*!
 * 1.0 Keebee AAT Copyright © 2017
 * PhidgetConfig/_ConfigDetailEdit.js
 * Author: John Charlton
 * Date: 2017-07
 */

;
(function ($) {
    phidgetconfig.detailedit = {
        init: function (options) {
            var config = {
                phidgetStyleTypeIdOnOff: 0,
                phidgetStyleTypeIdTouch: 0,
                maxPhidgetTypeIdSensor: 0
            };

            $.extend(config, options);

            initPhidgetDropdowns();

            $("#ddlPhidgetTypes").change(function(e) {
                initPhidgetDropdowns();
            });

            function initPhidgetDropdowns() {
                var phidgetTypeId = parseInt($("#ddlPhidgetTypes").val());

                if (phidgetTypeId >= config.maxPhidgetTypeIdSensor) {

                    $($("#ddlPhidgetStyleTypes")
                            .find("option[value != '" + config.phidgetStyleTypeIdOnOff + "']"))
                        .each(function (index, opt) {
                            $(opt).hide();
                        });

                    $("#ddlPhidgetStyleTypes")
                        .find("option[value = '" + config.phidgetStyleTypeIdOnOff + "']")
                        .each(function (index, opt) {
                            $(opt).prop("selected", true);
                        });

                    $("#ddlPhidgetStyleTypes").val(config.phidgetStyleTypeIdOnOff);
                } else {
                    // for "Sensor" phidgets, "On/Off" style is not allowed
                    $("#ddlPhidgetStyleTypes")
                        .find("option[value = '" + config.phidgetStyleTypeIdOnOff + "']")
                        .each(function (index, opt) {
                            $(opt).hide();
                            $(opt).prop("selected", false);
                        });

                    $("#ddlPhidgetStyleTypes")
                        .find("option[value != '" + config.phidgetStyleTypeIdOnOff + "']")
                        .each(function (index, opt) {
                            $(opt).show();
                            if (index === 0)
                                $(opt).prop("selected", true);
                        });

                    $("#ddlPhidgetStyleTypes").val(config.phidgetStyleTypeIdTouch);
                }
            }
        }
    }
})(jQuery);