﻿/*!
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
            var cmdAdd = $("#add");
            var cmdEdit = $("#edit");
            var cmdDelete = $("#delete");

            var cmdAddDetail = $("#add-detail");
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
                        activeConfig = lists.ConfigList.filter(function (value) { return value.IsActive; })[0];
                    }
                });
            }

            function ConfigDetail(id, configid, phidgettype, description, responsetype, candelete) {
                var self = this;

                self.id = id;
                self.configid = configid;
                self.phidgettype = phidgettype;
                self.description = description;
                self.responsetype = responsetype;
                self.candelete = candelete;
            }

            function Config(id, description, isactive, candelete) {
                var self = this;

                self.id = id;
                self.description = description;
                self.isactive = isactive;
                self.candelete = candelete;
            }

            function ConfigViewModel() {
                var tblConfigDetail = $("#tblConfigDetail");

                var self = this;

                self.configs = ko.observableArray([]);
                self.configDetails = ko.observableArray([]);
                self.activeConfig = ko.observable(activeConfig);
                self.selectedConfig = ko.observable(activeConfig.Id);
                self.selectedConfigDesc = ko.observable(activeConfig.Description);
                self.selectedConfigDetail = ko.observable([]);

                createConfigDetailArray(lists.ConfigDetailList);
                createConfigArray(lists.ConfigList);
                initializeScreen();

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

                function initializeScreen() {
                    cmdActivate.attr("disabled", "disabled");
                    cmdDelete.attr("disabled", "disabled");
                }

                self.columns = ko.computed(function() {
                    var arr = [];
                    arr.push({ title: "Phidget", sortKey: "phidgettype" });
                    arr.push({ title: "Description", sortKey: "description" });
                    arr.push({ title: "Response", sortKey: "responsetype" });
                    return arr;
                });

                function pushConfig(value) {
                    self.configs.push(new Config(value.Id, value.Description, value.IsActive, value.CanDelete));
                };

                function pushConfigDetail(value) {
                    self.configDetails.push(new ConfigDetail(value.Id,
                        value.ConfigId,
                        value.PhidgetType,
                        value.Description,
                        value.ResponseType,
                        value.CanDelete));
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

                self.showEditDetailDialog = function (row) {
                    var id = row.id;

                    if (id > 0) {
                        self.highlightRow(row);
                    }

                    self.showConfigDetailEditDialog(row);
                };

                self.showDeleteDetailDialog = function (row) {
                    var id = row.id;

                    if (id > 0) {
                        self.highlightRow(row);
                    }

                    self.showConfigDetailDeleteDialog(row);
                };

                self.deleteSelectedDetail = function (row) {
                    deleteConfigDetail(row.id);
                };

                self.showConfigDetailEditDialog = function (row) {
                    var id = (row.id !== undefined ? row.id : 0);
                    var title = "<span class='glyphicon glyphicon-pencil'></span>";
                    var message;

                    if (id > 0) {
                        title = title + " Edit Configuration Detail";
                        var configDetail = self.getConfigDetail(id);
                        self.selectedConfigDetail(configDetail);
                    } else {
                        title = title + " Add Configuration Detail";
                        self.selectedConfigDetail([]);
                    }

                    $.ajax({
                        type: "GET",
                        async: false,
                        url: site.url + "Configurations/GetConfigDetailEditView/" + id,
                        data: { id: id, configId: self.selectedConfig() },
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
                                    var result = self.saveConfigDetail();

                                    if (result.ErrorMessages === null) {
                                        lists.ConfigDetailList = result.ConfigDetailList;
                                        createConfigDetailArray(lists.ConfigDetailList);
                                        self.selectedConfigDetail(self.getConfigDetail(result.SelectedId));
                                        self.highlightRow(self.selectedConfigDetail());
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

                self.showConfigDetailDeleteDialog = function (row) {
                    var id = (row.id !== undefined ? row.id : 0);
                    if (id <= 0) return;
                    var r = self.getConfigDetail(id);

                    BootstrapDialog.show({
                        title: "Delete Config Detail?",
                        message: "Are you sure?",
                        closable: false,
                        buttons: [
                            {
                                label: "No",
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }, {
                                label: "Yes",
                                cssClass: "btn-primary",
                                action: function (dialog) {
                                    var result = self.deleteConfigDetail(row.id);
                                    lists.ConfigDetailList = result.ConfigDetailList;
                                    createConfigDetailArray(lists.ConfigDetailList);
                                    dialog.close();
                                    $("body").css("cursor", "default");
                                }
                            }
                        ]
                    });
                };

                self.getConfigDetail = function (configDetailid) {
                    var configDetail = null;

                    ko.utils.arrayForEach(self.configDetails(), function (item) {
                        if (item.id === configDetailid) {
                            configDetail = item;
                        }
                    });

                    return configDetail;
                };

                self.getConfigDetailFromDialog = function () {
                    var description = $.trim($("#txtDescription").val());
                    var phidgettypeid = $("#ddlPhidgetTypes").val();
                    var responsetypeid = $("#ddlResponseTypes").val();

                    return {
                        Id: self.selectedConfigDetail().id, Description: description, phidgetTypeId: phidgettypeid, responseTypeId: responsetypeid
                    };
                };

                self.saveConfigDetail = function () {
                    var configdetail = self.getConfigDetailFromDialog();
                    configdetail.ConfigId = self.selectedConfig();
                    var jsonData = JSON.stringify(configdetail);
                    var result;

                    $("body").css("cursor", "wait");

                    $.ajax({
                        type: "POST",
                        async: false,
                        url: site.url + "Configurations/SaveDetail/",
                        data: { configdetail: jsonData },
                        dataType: "json",
                        traditional: true,
                        failure: function () {
                            $("body").css("cursor", "default");
                            $("#validation-container").html("");
                        },
                        success: function (data) {
                            result = data;
                        },
                        error: function (data) {
                            result = data;
                        }
                    });

                    return result;
                };

                self.deleteConfigDetail = function (id) {
                    $("body").css("cursor", "wait");

                    var result;

                    $.ajax({
                        type: "POST",
                        async: false,
                        url: site.url + "Configurations/DeleteDetail/",
                        data: { id: id },
                        dataType: "json",
                        traditional: true,
                        failure: function () {
                            $("body").css("cursor", "default");
                            $("#validation-container").html("");
                        },
                        success: function (data) {
                            result = data;
                        },
                        error: function (data) {
                            result = data;
                        }
                    });

                    return result;
                };

                self.activateSelectedConfig = function() {
                    var configId = self.selectedConfig();
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
                        success: function(data) {
                            createConfigArray(data.ConfigList);
                            self.activeConfig = data.ConfigList.filter(function(value) { return value.IsActive; })[0];
                            self.selectedConfig(self.activeConfig.Id);
                            self.selectedConfigDesc(self.activeConfig.Description);
                            $("body").css("cursor", "default");
                        },
                        error: function(data) {
                            $("body").css("cursor", "default");
                        }
                    });
                };

                self.enableDetail = function() {
                    var configId = self.selectedConfig();

                    // Activate  Button
                    var disableActivate = self.activeConfig().Id === configId;
                    if (disableActivate) {
                        cmdActivate.attr("disabled", "disabled");
                    } else {
                        cmdActivate.removeAttr("disabled");
                    }

                    // Delete Button
                    var candeleteConfig = self.canDeleteConfig(configId);
                    if (!candeleteConfig)
                        cmdDelete.attr("disabled", "disabled");
                    else
                        cmdDelete.removeAttr("disabled");
                };

                self.canDeleteConfig = function (id) {
                    if (id === undefined) return false;
                    if (isNaN(id)) return false;
                    return self.configs()
                        .filter(function(value) {
                            return value.id === id;
                        })[0].candelete;
                };
            }
        }
    }
})(jQuery);