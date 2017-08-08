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
                selectedPhidgetStyleTypeId: 0,
                phidgetStyleTypeIdOnOff: 0,
                phidgetStyleTypeIdOnOnly: 0,
                phidgetStyleTypeIdTouch: 0,
                maxPhidgetTypeIdSensor: 0
            };

            $.extend(config, options);

            loadPhidgetStyleDropdown();
            setPhidgetStyleIndex(config.selectedPhidgetStyleTypeId);

            $("#ddlPhidgetTypes").change(function (e) {
                var isInput = (parseInt($("#ddlPhidgetTypes").val()) >= config.maxPhidgetTypeIdSensor);

                loadPhidgetStyleDropdown(isInput);

                if (isInput)
                    setPhidgetStyleIndex(config.phidgetStyleTypeIdOnOff);
                else
                    setPhidgetStyleIndex(config.phidgetStyleTypeIdTouch);
            });

            function loadPhidgetStyleDropdown(isInput) {
                if (typeof isInput === "undefined") {
                    isInput = (parseInt($("#ddlPhidgetTypes").val()) >= config.maxPhidgetTypeIdSensor);
                }

                if (isInput) {
                    // hide all options except On/Off and OnOnly
                    $("#ddlPhidgetStyleTypes option")
                        .each(function (index, opt) {
                            if (parseInt(this.value) !== config.phidgetStyleTypeIdOnOff &&
                                parseInt(this.value) !== config.phidgetStyleTypeIdOnOnly) {
                                $(opt).hide();
                            } else {
                                $(opt).show();
                            }
                        });
                } else {
                    // hide On/Off and OnOnly
                    $("#ddlPhidgetStyleTypes option")
                        .each(function (index, opt) {
                            if (parseInt(this.value) === config.phidgetStyleTypeIdOnOff ||
                                parseInt(this.value) === config.phidgetStyleTypeIdOnOnly) {
                                $(opt).hide();
                            } else {
                                $(opt).show();
                            }
                        });
                }
            }

            function setPhidgetStyleIndex(selectedId) {
                $("#ddlPhidgetStyleTypes")
                    .find("option[value=" + selectedId + "]")
                    .each(function(index, opt) {
                        $(opt).prop("selected", true);
                    });
                $("#ddlPhidgetStyleTypes").val(selectedId);
            }
        }
    }
})(jQuery);