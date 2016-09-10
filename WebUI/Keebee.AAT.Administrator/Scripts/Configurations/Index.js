/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Configurations/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

; (function ($) {

    configurations.index = {
        init: function() {
            var HIGHLIGHT_ROW_COLOUR = "#e3e8ff";

            // buttons
            var cmdActivate = $("#activate");

            // inputs
            var activeConfig = {};

            var lists = {  
                ConfigList: [],
                ConfigDetailList: []
            };

            loadData();

            ko.applyBindings(new ConfigViewModel());

            function loadData() {
                $.ajax({
                    type: "GET",
                    url: site.url + "Configurations/GetData/",
                    dataType: "json",
                    traditional: true,
                    async: false,
                    success: function (data) {
                        $.extend(lists, data);
                        activeConfig = data.ActiveConfig;
                    }
                });
            }

            function ConfigDetail(id, activitytype, responsetype, phidget, isuserresponse) {
                var self = this;

                self.id = id;
                self.activitytype = activitytype;
                self.responsetype = responsetype;
                self.phidget = phidget;
                self.isuserresponse = isuserresponse;
            }

            function Config(id, description) {
                var self = this;

                self.id = id;
                self.description = description;
            }

            function ConfigViewModel() {
                var tblConfigDetail = $("#tblConfigDetail");

                var self = this;

                self.configs = ko.observableArray([]);
                self.configDetails = ko.observableArray([]);
                self.selectedConfig = ko.observable(activeConfig.Id);
                self.selectedConfigDesc = ko.observable(activeConfig.Description);

                createConfigDetailArray(lists.ConfigDetailList);
                createConfigArray(lists.ConfigList);

                function createConfigDetailArray(list) {
                    self.configDetails.removeAll();
                    $(list)
                        .each(function(index, value) {
                            pushConfigDetail(value);
                        });
                };

                function createConfigArray(list) {
                    self.configs.removeAll();
                    $(list)
                        .each(function (index, value) {
                            pushConfig(value);
                        });
                };

                self.columns = ko.computed(function() {
                    var arr = [];
                    arr.push({ title: "Phidget", sortKey: "phidgettype" });
                    arr.push({ title: "Activity", sortKey: "activitytype" });
                    arr.push({ title: "Response", sortKey: "responsetype" });
                    arr.push({ title: "User Response", sortKey: "isuserresponse" });
                    return arr;
                });

                function pushConfig(value) {
                    self.configs.push(new Config(value.Id, value.Description));
                };

                function pushConfigDetail(value) {
                    self.configDetails.push(new ConfigDetail(value.Id,
                        value.ActivityType,
                        value.ResponseType,
                        value.Phidget,
                        value.IsUserResponse));
                };

                self.filteredConfigDetails = ko.computed(function () {
                    return ko.utils.arrayFilter(self.configDetails(), function (c) {
                        return (c.id === self.selectedConfig());
                    });
                });

                self.configDetailsTable = ko.computed(function () {
                    var filteredConfigDetails = self.filteredConfigDetails();
                    return filteredConfigDetails;
                });

                self.highlightRow = function (row) {
                    if (row == null) return;

                    var rows = tblConfigDetail.find("tr:gt(0)");
                    rows.each(function () {
                        $(this).css("background-color", "#ffffff");
                    });

                    var r = tblConfigDetail.find("#row_" + row.id);
                    r.css("background-color", HIGHLIGHT_ROW_COLOUR);
                    tblConfigDetail.attr("tr:hover", HIGHLIGHT_ROW_COLOUR);
                };
            }

            // activate the selected configuration
            cmdActivate.click(function() {
                var configId = $("select[id=ddlConfigs]").val();

                $("body").css("cursor", "wait");

                $.ajax({
                    type: "POST",
                    async: false,
                    url: site.url + "Configurations/Activate/",
                    data: { configId: configId },
                    dataType: "json",
                    traditional: true,
                    failure: function() {
                        $("body").css("cursor", "default");
                        $("#validation-container").html("");
                    },
                    success: function (data) {
                        activeConfig = data.ActiveConfig;
                        var lblActiveConfigDesc = $("label[id=lblActiveConfigDesc]");
                        lblActiveConfigDesc.text(activeConfig.Description);
                        $("body").css("cursor", "default");
                    },
                    error: function (data) {
                        $("body").css("cursor", "default");
                    }
                });
            });
        }
    }
})(jQuery);