/*!
 * 1.0 Keebee AAT Copyright © 2016
 * PhidgetConfig/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

; (function ($) {

    phidgetconfig.index = {
        init: function () {
            var isBinding = true;

            // buttons            
            var cmdAdd = $("#add");
            var cmdEdit = $("#edit");
            var cmdDelete = $("#delete");
            var cmdActivate = $("#activate");
            var cmdAddDetail = $("#add-detail");

            // active config
            var activeConfig = {};
            
            var lists = {  
                ConfigList: [],
                ConfigDetailList: []
            };

            cmdDelete.prop("disabled", false);

            utilities.job.execute({
                url: site.url + "PhidgetConfig/GetData/"
            })
            .then(function (data) {
                $.extend(lists, data);
                activeConfig = lists.ConfigList.filter(function (value) { return value.IsActive; })[0];

                $("#error-container").hide();
                $("#loading-container").hide();
                $("#tblConfigDetail").show();
                $("#col-button-container").show();

                cmdAdd.show();
                cmdEdit.show();
                cmdDelete.show();
                cmdActivate.prop("disabled", false);
                cmdAddDetail.prop("disabled", false);

                var tooltipProps = { delay: { show: 100, hide: 100 } };
                cmdAdd.tooltip(tooltipProps);
                cmdEdit.tooltip(tooltipProps);
                cmdDelete.tooltip(tooltipProps);
                cmdActivate.tooltip(tooltipProps);
                cmdAddDetail.tooltip(tooltipProps);

                ko.bindingHandlers.tableUpdated = {
                    update: function (element, valueAccessor, allBindings) {
                        ko.unwrap(valueAccessor());
                        isBinding = false;
                    }
                }

                ko.bindingHandlers.tableRender = {
                    update: function (element, valueAccessor) {
                        ko.utils.unwrapObservable(valueAccessor());
                        for (var index = 0, length = element.childNodes.length; index < length; index++) {
                            var node = element.childNodes[index];
                            if (node.nodeType === 1) {
                                var id = node.id.replace("row_", "");
                                var tolltipDelete = $("#delete_" + id);
                                if (tolltipDelete.length > 0)
                                    tolltipDelete.tooltip({ delay: { show: 100, hide: 100 } });

                                var tolltipEdit = $("#edit_" + id);
                                if (tolltipEdit.length > 0)
                                    tolltipEdit.tooltip({ delay: { show: 100, hide: 100 } });
                            }
                        }
                    }
                }

                ko.applyBindings(new ConfigViewModel());

                function ConfigDetail(id, configid, sortorder, phidgettype, phidgetstyletype, description, location, responsetype, issystem, canedit) {
                    var self = this;

                    self.id = id;
                    self.configid = configid;
                    self.sortorder = sortorder;
                    self.phidgettype = phidgettype;
                    self.phidgetstyletype = phidgetstyletype;
                    self.description = description;
                    self.location = location;
                    self.responsetype = responsetype;
                    self.issystem = issystem;
                    self.canedit = canedit;
                }

                function Config(id, description, isactive, isactiveeventlog, candelete) {
                    var self = this;

                    self.id = id;
                    self.description = description;
                    self.isactive = isactive;
                    self.isactiveeventlog = isactiveeventlog;
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
                    self.IsActiveEventLog = ko.observable(activeConfig.IsActiveEventLog);
                    self.activeEventLogDesc = (activeConfig.IsActiveEventLog === true)
                        ? self.activeEventLogDesc = ko.observable("On")
                        : self.activeEventLogDesc = ko.observable("Off");

                    self.selectedConfigDetail = ko.observable([]);
                    self.isLoadingConfigs = ko.observable(false);
                    self.totalConfigDetails = ko.observable(0);

                    createConfigDetailArray(lists.ConfigDetailList);
                    createConfigArray(lists.ConfigList);

                    function enableButtons(enable) {
                        cmdAdd.prop("hidden", enable);
                        cmdEdit.prop("hidden", enable);
                        cmdDelete.prop("hidden", enable);
                        cmdAddDetail.prop("disabled", !enable);
                        cmdActivate.prop("disabled", !enable);
                    }

                    function createConfigDetailArray(list) {
                        self.configDetails.removeAll();
                        $(list)
                            .each(function (index, value) {
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

                    self.columns = ko.computed(function () {
                        var arr = [];
                        arr.push({ title: "Phidget", sortKey: "phidgettype", cssClass: "" });
                        arr.push({ title: "Style", sortKey: "phidgetstyletype", cssClass: "" });
                        arr.push({ title: "Description", sortKey: "description", cssClass: "" });
                        arr.push({ title: "Location", sortKey: "location", cssClass: "" });
                        arr.push({ title: "Response", sortKey: "responsetype", cssClass: "" });
                        arr.push({ title: "System", sortKey: "issystem", cssClass: "col-issytem" });
                        return arr;
                    });

                    function pushConfig(value) {
                        self.configs.push(new Config(value.Id, value.Description, value.IsActive, value.IsActiveEventLog, value.CanDelete));
                    };

                    function pushConfigDetail(value) {
                        self.configDetails.push(new ConfigDetail(value.Id,
                            value.ConfigId,
                            value.SortOrder,
                            value.PhidgetType,
                            value.PhidgetStyleType,
                            value.Description,
                            value.Location,
                            value.ResponseType,
                            value.IsSystem,
                            value.CanEdit));
                    };

                    self.sort = function () {
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
                        var r = tblConfigDetail.find("#row_" + row.id);
                        $(r).addClass("highlight").siblings().removeClass("highlight");
                    };

                    self.editConfig = function (add) {
                        if (isBinding) return;

                        cmdAdd.tooltip("hide");
                        cmdEdit.tooltip("hide");
                        enableButtons(false);

                        var id = 0;   
                        var title = "<span class='glyphicon glyphicon-{glyphicon}' style='color: #fff'></span>" + " {action} Configuration";
                        title = add 
                            ? title.replace("{glyphicon}", "duplicate").replace("{action}", "Duplicate")
                            : title.replace("{glyphicon}", "pencil").replace("{action}", "Edit");

                        utilities.partialview.show({
                            url: site.url + "PhidgetConfig/GetConfigEditView/" + id,
                            type: add ? BootstrapDialog.TYPE_SUCCESS : BootstrapDialog.TYPE_PRIMARY,
                            title: title,
                            params: { selectedConfigid: self.selectedConfig() },
                            focus: "txtDescription",
                            buttonOKClass: add ? "btn-success" : "btn-edit",
                            cancelled: function() {
                                self.enableDetail();
                                enableButtons(true);
                            },
                            callback: function(dialog) {
                                var config = self.getConfigFromDialog();
                                config.Id = id;

                                utilities.job.execute({
                                        url: site.url + "PhidgetConfig/Validate",
                                        type: "POST",
                                        params: { config: config, selectedConfigId: self.selectedConfig() }
                                    })
                                    .then(function(validateResult) {
                                        if (validateResult.ValidationMessages === null) {
                                            dialog.close();
                                            utilities.job.execute({
                                                    url: site.url + "PhidgetConfig/Save",
                                                    type: "POST",
                                                    waitMessage: "Saving...",
                                                    params:
                                                    {
                                                        config: config,
                                                        selectedConfigId: self.selectedConfig()
                                                    }
                                                })
                                                .then(function(saveResult) {
                                                    $.extend(lists, saveResult);
                                                    createConfigArray(lists.ConfigList);
                                                    createConfigDetailArray(lists.ConfigDetailList);
                                                    self.selectedConfig(saveResult.SelectedId);
                                                    activeConfig = saveResult.ConfigList
                                                        .filter(function(value) { return value.IsActive; })[0];
                                                    self.activeConfig(activeConfig.Id);
                                                    self.activeConfigDesc(activeConfig.Description);
                                                    if (activeConfig.IsActiveEventLog === true)
                                                        self.activeEventLogDesc("On");
                                                    else
                                                        self.activeEventLogDesc("Off");
                                                    self.enableDetail();
                                                    enableButtons(true);
                                                });
                                        } else {
                                            utilities.validation.show({
                                                container: "validation-container",
                                                messages: validateResult.ValidationMessages,
                                                beginWithLineBreak: true
                                            });
                                        }
                                    });
                                }
                            })
                        .catch(function() {
                            self.enableDetail();
                            enableButtons(true);
                        });
                    };

                    self.deleteConfig = function () {
                        if (isBinding) return;
                        if (cmdDelete.is("[disabled]")) return;
                        var id = self.selectedConfig();
                        if (id <= 0) return;

                        $("#delete").tooltip("hide");
                        enableButtons(false);
                           
                        var title = "<span class='glyphicon glyphicon-trash' style='color: #fff'></span>";
                        utilities.confirm.show({
                            type: BootstrapDialog.TYPE_DANGER,
                            title: title + " Delete Configuration",
                            message: "Permanently delete the configuration <i><b>" +
                                self.selectedConfigDesc() + "</b></i>?\n\n" +
                                "<b>Warning:</b> All configuration detail will be removed!",
                            buttonOK: "Yes, Delete",
                            buttonOKClass: "btn-danger"
                        })
                        .then(function (confirm) {
                            if (confirm) {
                                utilities.job.execute({
                                    url: site.url + "PhidgetConfig/Delete",
                                    type: "POST",
                                    params: { id: id },
                                    waitMessage: "Deleting..."
                                })
                                .then(function(result) {
                                    lists.ConfigList = result.ConfigList;
                                    createConfigArray(lists.ConfigList);
                                    self.enableDetail();
                                    var activeId = lists.ConfigList
                                        .filter(function(value) { return value.IsActive === true })[0]
                                        .Id;
                                    self.selectedConfig(activeId);
                                    enableButtons(true);
                                })
                                .catch(function() {
                                    dialog.close();
                                    enableButtons(true);
                                });
                            } else {
                                enableButtons(true);
                            }
                        });
                    };

                    self.editDetail = function (row) {
                        if (isBinding) return;

                        $("#edit_" + row.id).tooltip("hide");
                        cmdAddDetail.tooltip("hide");
                        enableButtons(false);

                        var id = (row.id !== undefined) ? row.id : 0;
                        var add = (id === 0);
                        if (!add) { self.highlightRow(row); }
                       
                        var title = "<span class='glyphicon glyphicon-pencil' style='color: #fff'></span>";
                        title = add
                            ? title + " Edit Configuration Item"
                            : title + " Add Configuration Item";

                        if (add) {
                            self.selectedConfigDetail([]);
                        } else {
                            var configDetail = self.getConfigDetail(id);
                            self.selectedConfigDetail(configDetail);
                        }

                        utilities.partialview.show({
                            url: "PhidgetConfig/GetConfigDetailEditView/" + id,
                            type: add ? BootstrapDialog.TYPE_SUCCESS : BootstrapDialog.TYPE_PRIMARY,
                            title: title,
                            params: { id: id, configId: self.selectedConfig() },
                            focus: "txtDescription",
                            buttonOKClass: add ? "btn-success" : "btn-primary",
                            cancelled: function () { enableButtons(true); },
                            callback: function(dialog) {
                                var configdetail = self.getConfigDetailFromDialog();
                                configdetail.ConfigId = self.selectedConfig();
                                utilities.job.execute({
                                        url: site.url + "PhidgetConfig/ValidateDetail",
                                        type: "POST",
                                        params: { configdetail: configdetail }
                                    })
                                    .then(function(validateResult) {
                                        if (validateResult.ValidationMessages === null) {
                                            dialog.close();
                                            utilities.job.execute({
                                                    url: "PhidgetConfig/SaveDetail",
                                                    type: "POST",
                                                    params: { configdetail: configdetail }
                                                })
                                                .then(function(saveResult) {
                                                    lists.ConfigDetailList = saveResult.ConfigDetailList;
                                                    createConfigDetailArray(lists.ConfigDetailList);
                                                    self
                                                        .selectedConfigDetail(self.getConfigDetail(saveResult
                                                            .SelectedId));
                                                    self.highlightRow(self.selectedConfigDetail());
                                                    self.enableDetail();
                                                    self.sort();
                                                    enableButtons(true);
                                                });
                                        } else {
                                            utilities.validation.show({
                                                container: "validation-container",
                                                messages: validateResult.ValidationMessages,
                                                beginWithLineBreak: true
                                            });
                                        }
                                    })
                                    .catch(function() {
                                        enableButtons(true);
                                    });
                                }
                        });
                    };

                    self.deleteDetail = function (row) {
                        var id = (row.id !== undefined ? row.id : 0);

                        $("#delete_" + row.id).tooltip("hide");

                        enableButtons(false);

                        if (id > 0) self.highlightRow(row);
                        else return;

                        var title = "<span class='glyphicon glyphicon-trash' style='color: #fff'></span>";                        
                        var cd = self.getConfigDetail(id);

                        utilities.confirm.show({
                            type: BootstrapDialog.TYPE_DANGER,
                            title: title + " Delete Configuration Detail",
                            message: "Delete <b>" + cd.phidgettype + "</b> detail item for actvity <i><b>" + cd.description + "</b></i>?",
                            buttonOK: "Yes, Delete",
                            buttonOKClass: "btn-danger"
                        })
                        .then(function(confirm) {
                            if (confirm) {
                                utilities.job.execute({
                                    url: site.url + "PhidgetConfig/DeleteDetail",
                                    type: "POST",
                                    params: {
                                        id: id,
                                        configId: self.selectedConfig(),
                                        isActive: self.selectedConfig() === self.activeConfig()
                                    },
                                    waitMessage: "Deleting..."
                                })
                                .then(function(result) {
                                    lists.ConfigDetailList = result.ConfigDetailList;
                                    createConfigDetailArray(lists.ConfigDetailList);
                                    self.enableDetail();
                                    enableButtons(true);
                                })
                                .catch(function() {
                                    self.enableDetail();
                                    enableButtons(true);
                                });
                            } else {
                                enableButtons(true);
                            }
                        });
                    };

                    self.activateConfig = function (id) {
                        var title = "<span class='glyphicon glyphicon-ok' style='color: #fff'></span>";
                        if (id <= 0) return;

                        cmdActivate.tooltip("hide");
                        enableButtons(false);

                        utilities.confirm.show({
                            type: BootstrapDialog.TYPE_PRIMARY,
                            title: title + " Activate Configuration",
                            message: "Activate the configuration <b>" + self.selectedConfigDesc() + "</b>?",
                        })
                        .then(function(confirm) {
                            if (confirm) {
                                var configId = self.selectedConfig();

                                utilities.job.execute({
                                    url: site.url + "PhidgetConfig/Activate/",
                                    params: { configId: configId },
                                    waitMessage: "Activating..."
                                })
                                .then(function(result) {
                                    $.extend(lists, result);
                                    createConfigArray(lists.ConfigList);
                                    createConfigDetailArray(lists.ConfigDetailList);
                                    activeConfig = result.ConfigList
                                        .filter(function(value) { return value.IsActive; })[0];
                                    self.activeConfig(activeConfig.Id);
                                    self.selectedConfig(activeConfig.Id);
                                    self.activeConfigDesc(activeConfig.Description);
                                    if (activeConfig.IsActiveEventLog === true)
                                        self.activeEventLogDesc("On");
                                    else
                                        self.activeEventLogDesc("Off");
                                    self.enableDetail();
                                    dialog.close();
                                    enableButtons(true);
                                })
                                .catch(function() {
                                    enableButtons(true);
                                });
                            } else {
                                enableButtons(true);
                            }
                        });
                    };

                    self.getConfigFromDialog = function () {
                        var description = $.trim($("#txtDescription").val());
                        var isactiveeventlog = $.trim($("#chkIsActiveEventLog").is(":checked"));

                        return {
                            Description: description,
                            IsActiveEventLog: isactiveeventlog,
                            IsActive: self.selectedConfig() === self.activeConfig()
                        };
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
                        var location = $.trim($("#txtLocation").val());
                        var phidgettypeid = $("#ddlPhidgetTypes").val();
                        var phidgetstyletypeid = $("#ddlPhidgetStyleTypes").val();
                        var responsetypeid = $("#ddlResponseTypes").val();

                        return {
                            Id: self.selectedConfigDetail().id, Description: description, Location: location, PhidgetTypeId: phidgettypeid, PhidgetStyleTypeId: phidgetstyletypeid, ResponseTypeId: responsetypeid, IsActive: self.selectedConfig() === self.activeConfig()
                        };
                    };

                    self.enableDetail = function () {
                        if (self.isLoadingConfigs()) return;

                        var configId = self.selectedConfig();

                        // COMMENTED - allow configuration to be activated at any time

                        // count of details
                        //var detailCount = self.configDetails().filter(function (value) {
                        //    return value.configid === configId;
                        //}).length;

                        // activate  button
                        //if (self.activeConfig() === configId || detailCount === 0) {
                        //    $("#activate").prop("disabled", true);
                        //} else {
                        //    $("#activate").prop("disabled", false);
                        //}

                        // COMMENTED - allow configuration to be activated at any time

                        // delete button
                        cmdDelete.prop("disabled", !self.canDeleteConfig(configId));
                    };

                    self.canDeleteConfig = function (id) {
                        return self.configs()
                            .filter(function (value) { return value.id === id; })
                            [0].candelete;
                    };

                    self.selectedConfigDesc = function () {
                        return self.configs()
                            .filter(function (value) { return value.id === self.selectedConfig(); })
                            [0].description;
                    };
                }
            })
            .catch(function (data) {
                $("#loading-container").hide();
                $("#error-container")
                    .html("<div><h2>Data load error:</h2></div>")
                    .append("<div>" + data.ErrorMessage + "</div>")
                    .append("<div><h3>Please try refreshing the page</h3></div>");
                $("#error-container").show();
            });
        }          
    }
})(jQuery);