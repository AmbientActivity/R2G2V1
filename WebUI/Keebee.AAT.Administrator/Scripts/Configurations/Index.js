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
            var cmdEdit = $("#edit");
            var cmdDelete = $("#delete");
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

            function ConfigDetail(id, configid, sortorder, phidgettype, phidgetstyletype, description, responsetype, candelete) {
                var self = this;

                self.id = id;
                self.configid = configid;
                self.sortorder = sortorder;
                self.phidgettype = phidgettype;
                self.phidgetstyletype = phidgetstyletype;
                self.description = description;
                self.responsetype = responsetype;
                self.candelete = candelete;
            }

            function Config(id, description, isactive, canedit, candelete) {
                var self = this;

                self.id = id;
                self.description = description;
                self.isactive = isactive;
                self.canedit = canedit;
                self.candelete = candelete;
            }

            function ConfigViewModel() {
                var tblConfigDetail = $("#tblConfigDetail");

                var self = this;

                self.configs = ko.observableArray([]);
                self.configDetails = ko.observableArray([]);
                self.activeConfig = ko.observable(activeConfig.Id);
                self.selectedConfig = ko.observable(activeConfig.Id);
                self.activeConfigDesc = ko.observable(activeConfig.Description);
                self.selectedConfigDetail = ko.observable([]);
                self.isLoadingConfigs = ko.observable(false);
                self.totalConfigDetails = ko.observable(0);

                createConfigDetailArray(lists.ConfigDetailList);
                createConfigArray(lists.ConfigList);
                self.canEdit = ko.observable(activeConfig.CanEdit);

                initializeScreen();

                function createConfigDetailArray(list) {
                    self.configDetails.removeAll();
                    $(list)
                        .each(function(index, value) {
                            pushConfigDetail(value);
                        });
                };

                function createConfigArray(list) {
                    self.isLoadingConfigs(true);
                    self.configs.removeAll();
                    $(list)
                        .each(function (index, value) {
                            pushConfig(value);
                        });
                    self.isLoadingConfigs(false);
                };

                function initializeScreen() {
                    cmdActivate.attr("disabled", "disabled");
                    cmdDelete.attr("disabled", "disabled");
                    // edit button
                    if (!self.canEdit())
                        cmdEdit.attr("disabled", "disabled");
                    else
                        cmdEdit.removeAttr("disabled");
                }

                self.columns = ko.computed(function() {
                    var arr = [];
                    arr.push({ title: "Phidget", sortKey: "phidgettype" });
                    arr.push({ title: "Style", sortKey: "phidgetstyletype" });
                    arr.push({ title: "Description", sortKey: "description" });
                    arr.push({ title: "Response", sortKey: "responsetype" });
                    return arr;
                });

                function pushConfig(value) {
                    self.configs.push(new Config(value.Id, value.Description, value.IsActive, value.CanEdit, value.CanDelete));
                };

                function pushConfigDetail(value) {
                    self.configDetails.push(new ConfigDetail(value.Id,
                        value.ConfigId,
                        value.SortOrder,
                        value.PhidgetType,
                        value.PhidgetStyleType,
                        value.Description,
                        value.ResponseType,
                        value.CanDelete));
                };

                self.sort = function() {
                    self.filteredConfigDetails().sort(function (a, b) {
                        return a.sortorder > b.sortorder ? 1 : 0;
                    });
                };

                self.filteredConfigDetails = ko.computed(function () {
                    return ko.utils.arrayFilter(self.configDetails(), function (c) {
                        return (c.configid === self.selectedConfig());
                    });
                });

                self.configDetailsTable = ko.computed(function () {
                    var filteredConfigDetails = self.filteredConfigDetails();
                    self.totalConfigDetails(filteredConfigDetails.length);
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

                self.showEditDialog = function () {
                    var id = self.selectedConfig();
                    self.showConfigEditDialog(id);
                };

                self.showAddDialog = function () {
                    self.showConfigEditDialog(0);
                };

                self.showDeleteDialog = function () {
                    var id = self.selectedConfig();

                    self.showConfigDeleteDialog(id);
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

                self.showConfigEditDialog = function (id) {
                    var title = "<span class='glyphicon glyphicon-pencil'></span>";
                    var message;

                    if (id > 0) {
                        title = title + " Edit Configuration";
                    } else {
                        title = title + " Duplicate Configuration";
                    }

                    $.ajax({
                        type: "GET",
                        async: false,
                        url: site.url + "Configurations/GetConfigEditView/" + id,
                        data: { selectedConfigid: self.selectedConfig() },
                        success: function (data) {
                            message = data;
                        }
                    });

                    BootstrapDialog.show({
                        title: title,
                        message: message,
                        onshown: function () {
                            $("#txtDescription").focus();
                        },
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
                                    var result = self.saveConfig(id);

                                    if (result.ErrorMessages === null) {
                                        $.extend(lists, result);
                                        createConfigArray(lists.ConfigList);
                                        createConfigDetailArray(lists.ConfigDetailList);
                                        self.selectedConfig(result.SelectedId);
                                        activeConfig = result.ConfigList.filter(function (value) { return value.IsActive; })[0];
                                        self.activeConfig(activeConfig.Id);
                                        self.activeConfigDesc(activeConfig.Description);
                                        self.enableDetail();
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

                self.showConfigDeleteDialog = function (id) {
                    var title = "<span class='glyphicon glyphicon-trash'></span>";
                    if (id <= 0) return;

                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_DANGER,
                        title: title + " Delete Configuration",
                        message: "Permanently delete the configuration <i><b>" + self.selectedConfigDesc() + "</b></i>?\n\n" +
                            "<b>Warning:</b> All configuration detail will be removed!",
                        closable: false,
                        buttons: [
                            {
                                label: "No",
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }, {
                                label: "Yes, Delete",
                                cssClass: "btn-danger",
                                action: function (dialog) {
                                    var result = self.deleteConfig(id);
                                    lists.ConfigList = result.ConfigList;
                                    createConfigArray(lists.ConfigList);
                                    dialog.close();
                                    $("body").css("cursor", "default");
                                }
                            }
                        ]
                    });
                };

                self.showConfigActivateDialog = function (id) {
                    var title = "<span class='glyphicon glyphicon-ok'></span>";
                    if (id <= 0) return;

                    BootstrapDialog.show({
                        title: title + " Activate Configuration",
                        message: "Activate the configuration <b>" + self.selectedConfigDesc() + "</b>?",
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
                                    var result = self.activateSelectedConfig();
                                    $.extend(lists, result);
                                    createConfigArray(lists.ConfigList);
                                    createConfigDetailArray(lists.ConfigDetailList);
                                    activeConfig = result.ConfigList.filter(function (value) { return value.IsActive; })[0];
                                    self.activeConfig(activeConfig.Id);
                                    self.selectedConfig(activeConfig.Id);
                                    self.activeConfigDesc(activeConfig.Description);
                                    self.enableDetail();
                                    dialog.close();
                                    $("body").css("cursor", "default");
                                }
                            }
                        ]
                    });
                };

                self.showConfigDetailEditDialog = function (row) {
                    var id = (row.id !== undefined ? row.id : 0);
                    var title = "<span class='glyphicon glyphicon-pencil'></span>";
                    var message;

                    if (id > 0) {
                        title = title + " Edit Configuration Item";
                        var configDetail = self.getConfigDetail(id);
                        self.selectedConfigDetail(configDetail);
                    } else {
                        title = title + " Add Configuration Item";
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
                        onshown: function () {
                            $("#txtDescription").focus();
                        },
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
                                        self.enableDetail();
                                        self.sort();
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
                    var title = "<span class='glyphicon glyphicon-trash'></span>";
                    var id = (row.id !== undefined ? row.id : 0);
                    if (id <= 0) return;
                    var cd = self.getConfigDetail(id);

                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_DANGER,
                        title: title + " Delete Configuration Detail",
                        message: "Delete <b>" + cd.phidgettype + "</b> detail item for actvity <i><b>" + cd.description + "</b></i>?",
                        closable: false,
                        buttons: [
                            {
                                label: "No",
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }, {
                                label: "Yes, Delete",
                                cssClass: "btn-danger",
                                action: function (dialog) {
                                    var result = self.deleteConfigDetail(row.id);
                                    lists.ConfigDetailList = result.ConfigDetailList;
                                    createConfigDetailArray(lists.ConfigDetailList);
                                    self.enableDetail();
                                    dialog.close();
                                    $("body").css("cursor", "default");
                                }
                            }
                        ]
                    });
                };

                self.getConfigFromDialog = function () {
                    var description = $.trim($("#txtDescription").val());

                    return {
                        Description: description
                    };
                };

                self.saveConfig = function (id) {
                    var config = self.getConfigFromDialog();
                    config.Id = id;

                    var jsonData = JSON.stringify(config);
                    var result;

                    $("body").css("cursor", "wait");

                    $.ajax({
                        type: "POST",
                        async: false,
                        url: site.url + "Configurations/Save/",
                        data: { config: jsonData, selectedConfigId: self.selectedConfig() },
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

                self.deleteConfig = function (id) {
                    $("body").css("cursor", "wait");

                    var result;

                    $.ajax({
                        type: "POST",
                        async: false,
                        url: site.url + "Configurations/Delete/",
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
                    var phidgetstyletypeid = $("#ddlPhidgetStyleTypes").val();
                    var responsetypeid = $("#ddlResponseTypes").val();

                    return {
                        Id: self.selectedConfigDetail().id, Description: description, PhidgetTypeId: phidgettypeid, PhidgetStyleTypeId: phidgetstyletypeid, ResponseTypeId: responsetypeid
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
                    var result;

                    $.ajax({
                        type: "POST",
                        async: false,
                        url: site.url + "Configurations/Activate/",
                        data: { configId: configId },
                        dataType: "json",
                        traditional: true,
                        failure: function (data) {
                            result = data;
                            $("body").css("cursor", "default");
                            $("#validation-container").html("");
                        },
                        success: function (data) {
                            result = data;
                        },
                        error: function(data) {
                            result = data;
                        }
                    });

                    return result;
                };

                self.enableDetail = function () {
                    if (self.isLoadingConfigs()) return;

                    var configId = self.selectedConfig();

                    // count of details
                    var detailCount = self.configDetails().filter(function(value) {
                        return value.configid === configId;
                    }).length;

                    // activate  button
                    if (self.activeConfig() === configId || detailCount === 0) {
                        cmdActivate.attr("disabled", "disabled");
                    } else {
                        cmdActivate.removeAttr("disabled");
                    }

                    // edit button
                    if (!self.canEditConfig(configId))
                        cmdEdit.attr("disabled", "disabled");
                    else
                        cmdEdit.removeAttr("disabled");

                    // delete button
                    if (!self.canDeleteConfig(configId))
                        cmdDelete.attr("disabled", "disabled");
                    else
                        cmdDelete.removeAttr("disabled");
                };

                self.canDeleteConfig = function (id) {
                    return self.configs()
                        .filter(function (value) { return value.id === id; })
                        [0].candelete;
                };

                self.canEditConfig = function (id) {
                    return self.configs()
                        .filter(function (value) { return value.id === id; })
                        [0].canedit;
                };

                self.selectedConfigDesc = function () {
                    return self.configs()
                        .filter(function (value) { return value.id === self.selectedConfig(); })
                        [0].description;
                };
            }
        }
    }
})(jQuery);