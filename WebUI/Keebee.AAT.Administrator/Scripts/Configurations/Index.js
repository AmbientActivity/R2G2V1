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

            // active config
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

            function ConfigDetail(id, configid, phidgettype, activitytype, responsetype, isuserresponse) {
                var self = this;

                self.id = id;
                self.configid = configid;
                self.activitytype = activitytype;
                self.responsetype = responsetype;
                self.phidgettype = phidgettype;
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
                        value.ConfigId,
                        value.PhidgetType,
                        value.ActivityType,
                        value.ResponseType,
                        value.IsUserResponse));
                };

                self.filteredConfigDetails = ko.computed(function () {
                    return ko.utils.arrayFilter(self.configDetails(), function (c) {
                        return (c.configid === self.selectedConfig());
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

                self.showEditDialog = function (row) {
                    var id = row.id;

                    if (id > 0) {
                        self.highlightRow(row);
                    }

                    self.showConfigEditDialog(row);
                };

                self.showConfigEditDialog = function (row) {
                    var id = (typeof row.id !== "undefined" ? row.id : 0);
                    var title = "<span class='glyphicon glyphicon-pencil'></span>";
                    var message;

                    if (id > 0) {
                        title = title + " Edit Config";
                        var resident = self.getResident(id);
                        self.selectedResident(resident);
                    } else {
                        title = title + " Add Resident";
                        self.selectedResident([]);
                    }

                    $.ajax({
                        type: "GET",
                        async: false,
                        url: site.url + "Configurations/GetConfigEditView/" + id,
                        success: function (data) {
                            message = data;
                        }
                    });

                    BootstrapDialog.show({
                        title: title,
                        message: message,
                        closable: false,
                        buttons: [
                            {
                                label: "Cancel",
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }, {
                                label: "OK",
                                cssClass: "btn-primary",
                                action: function (dialog) {
                                    var result = self.saveConfig();

                                    if (result.ErrorMessages === null) {
                                        lists.ConfigDetailList = result.ConfigDetalList;
                                        createConfigDetailArray(lists.ConfigDetailList);
                                        self.selectedConfig(self.getResident(result.SelectedId));
                                        self.sort({ afterSave: true });
                                        self.highlightRow(self.selectedConfig());
                                        dialog.close();
                                        $("body").css("cursor", "default");
                                    } else {
                                        $("#validation-container").show();
                                        $("#validation-container").html("");
                                        $("body").css("cursor", "default");
                                        var html = "<ul>";
                                        for (var i = 0; i < result.ErrorMessages.length; i++) {
                                            var message = result.ErrorMessages[i];
                                            html = html + "<li>" + message + "</li>";
                                        }
                                        html = html + "</ul>";
                                        $("#validation-container").append(html);
                                        $("body").css("cursor", "default");
                                    }
                                }
                            }
                        ]
                    });
                };
            }

            self.getConfig = function (configid) {
                var config = null;

                ko.utils.arrayForEach(self.configs(), function (item) {
                    if (item.id === configid) {
                        config = item;
                    }
                });

                return config;
            };

            self.getConfigDetailFromDialog = function () {
                var description = $.trim($("#txtDescription").val());
                var phidgettypeid = $.trim($("#ddlPhidgetTypes").val());
                var responsetypeid = $.trim($("#ddlResponseTypes").val());

                return {
                    Id: self.selectedResident().id, Description: description, phidgetTypeId: phidgettypeid, responseTypeId: responsetypeid
                };
            };

            function enableDetail() {
                var configId = parseInt($("#ddlConfigs").val());
                var disable = activeConfig.Id === configId;
                if (disable)
                    cmdActivate.attr("disabled", "disabled");
                else 
                    cmdActivate.removeAttr("disabled");
            }

            $("#ddlConfigs").change(function () {
                enableDetail();
            });

            // activate the selected configuration
            cmdActivate.click(function() {
                var configId = parseInt($("#ddlConfigs").val());
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
                        $("#lblActiveConfigDesc").text(activeConfig.Description);
                        enableDetail();
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