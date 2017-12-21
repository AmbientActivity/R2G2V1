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
                allowedInputStyleTypes: "",
                allowedSensorStyleTypes: ""
            };

            $.extend(config, options);

            var allowedInputStyleTypes = config.allowedInputStyleTypes.trim().split(",");
            var allowedSensorStyleTypes = config.allowedSensorStyleTypes.trim().split(",");

            loadPhidgetStyleDropdown();

            $("#ddlPhidgetTypes").change(function (e) {
                var isInput = ($("#ddlPhidgetTypes option:selected").text()
                    .toLowerCase().indexOf("input") >= 0);

                loadPhidgetStyleDropdown(isInput);
            });

            function loadPhidgetStyleDropdown(isInput) {
                if (typeof isInput === "undefined") {
                    isInput = ($("#ddlPhidgetTypes option:selected").text()
                        .toLowerCase().indexOf("input") >= 0);
                }

                var selectedId = 0;

                if (isInput) {
                    // hide all options except On/Off and OnOnly
                    $("#ddlPhidgetStyleTypes option")
                        .each(function (index, opt) {
                            if (allowedInputStyleTypes.includes(this.value)) {
                                $(opt).show();
                                if (selectedId === 0)
                                    selectedId = parseInt(this.value);
                            } else {
                                $(opt).hide();
                                $(opt).prop("selected", false);
                            }
                        });
                } else {
                    // hide options On/Off and OnOnly
                    $("#ddlPhidgetStyleTypes option")
                        .each(function (index, opt) {
                            if (allowedSensorStyleTypes.includes(this.value)) {
                                $(opt).show();
                                if (selectedId === 0)
                                    selectedId = parseInt(this.value);
                            } else {
                                $(opt).hide();
                                $(opt).prop("selected", false);
                            }
                        });
                }
                $("#ddlPhidgetStyleTypes").val(selectedId);
            }
        }
    }
})(jQuery);