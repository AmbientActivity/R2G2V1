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

            var primarySortKey = "sortorder";
            var primarySortKeyConfig = "description";

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

            cmdDelete.attr("disabled", "disabled");

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
                    self.phidgettype = ko.observable(phidgettype);
                    self.phidgetstyletype = ko.observable(phidgetstyletype);
                    self.description = ko.observable(description);
                    self.location = ko.observable(location);
                    self.responsetype = ko.observable(responsetype);
                    self.issystem = ko.observable(issystem);
                    self.canedit = canedit;
                }

                function Config(id, description, isactive, isactiveeventlog, candelete) {
                    var self = this;

                    self.id = id;
                    self.description = ko.observable(description);
                    self.isactive = ko.observable(isactive);
                    self.isactiveeventlog = ko.observable(isactiveeventlog);
                    self.candelete = ko.observable(candelete);
                }

                function ConfigViewModel() {
                    var self = this;

                    self.configs = ko.observableArray([]);
                    self.configDetails = ko.observableArray([]);
                    self.activeConfigId = ko.observable(activeConfig.Id);
                    self.selectedConfigId = ko.observable(activeConfig.Id);
                    self.activeConfigDesc = ko.observable(activeConfig.Description);
                    self.IsActiveEventLog = ko.observable(activeConfig.IsActiveEventLog);
                    self.activeEventLogDesc = (activeConfig.IsActiveEventLog === true)
                        ? self.activeEventLogDesc = ko.observable("On")
                        : self.activeEventLogDesc = ko.observable("Off");

                    self.selectedConfigDetail = ko.observable({});
                    self.isLoadingConfigs = ko.observable(false);
                    self.totalConfigDetails = ko.observable(0);

                    createConfigDetailArray({ list: lists.ConfigDetailList, insert: false });
                    createConfigArray({ list: lists.ConfigList, insert: false });

                    function enableButtons(enable) {
                        cmdAdd.prop("hidden", enable);
                        cmdEdit.prop("hidden", enable);
                        cmdDelete.prop("hidden", enable);

                        if (enable) {
                            cmdAddDetail.removeAttr("disabled");
                            cmdActivate.removeAttr("disabled");
                        } else {
                            cmdAddDetail.attr("disabled", "disabled");
                            cmdActivate.attr("disabled", "disabled");
                        }
                    }

                    function createConfigDetailArray(params) {
                        var cfg = {
                            list: [],
                            insert: false
                        };

                        $.extend(cfg, params);

                        if (!cfg.insert)
                            self.configDetails.removeAll();

                        $(cfg.list).each(function (index, value) {
                            self.configDetails.push(new ConfigDetail(value.Id,
                                value.ConfigId,
                                value.SortOrder,
                                value.PhidgetType,
                                value.PhidgetStyleType,
                                value.Description,
                                value.Location,
                                value.ResponseType,
                                value.IsSystem,
                                value.CanEdit
                            ));
                        });
                    };

                    function createConfigArray(params) {
                        var cfg = {
                            list: [],
                            insert: false
                        };

                        $.extend(cfg, params);

                        self.isLoadingConfigs(true);

                        if (!cfg.insert)
                            self.configs.removeAll();

                        $(cfg.list).each(function (index, value) {
                            self.configs.push(new Config(
                                value.Id,
                                value.Description,
                                value.IsActive,
                                value.IsActiveEventLog,
                                value.CanDelete));
                            });
                        self.isLoadingConfigs(false);
                    };

                    self.columns = ko.computed(function () {
                        var arr = [];
                        arr.push({ title: "Phidget", sortKey: "phidgettype", cssClass: "", visible: true });
                        arr.push({ title: "Style", sortKey: "phidgetstyletype", cssClass: "", visible: true });
                        arr.push({ title: "Description", sortKey: "description", cssClass: "", visible: true });
                        arr.push({ title: "Location", sortKey: "location", cssClass: "", visible: true });
                        arr.push({ title: "Response", sortKey: "responsetype", cssClass: "", visible: true });
                        arr.push({ title: "System", sortKey: "issystem", cssClass: "col-issytem", visible: true });
                        arr.push({ title: "SortOrder", sortKey: "sortorder", cssClass: "", visible: false });
                        return arr;
                    });

                    self.sort = function () {
                        self.configDetails(utilities.sorting.sortArray(
                        {
                            array: self.configDetails(),
                            columns: self.columns(),
                            sortKey: primarySortKey,
                            primarySortKey: primarySortKey,
                            descending: false
                        }));
                    };

                    self.sortConfigs = function () {
                        self.configs(utilities.sorting.sortArray(
                        {
                            array: self.configs(),
                            columns: self.columns(),
                            sortKey: primarySortKeyConfig,
                            primarySortKey: primarySortKeyConfig,
                            descending: false
                        }));
                    };

                    self.filteredConfigDetails = ko.computed(function () {
                        return ko.utils.arrayFilter(self.configDetails(), function (c) {
                            return (c.configid === self.selectedConfigId());
                        });
                    });

                    self.configDetailsTable = ko.computed(function () {
                        var filteredConfigDetails = self.filteredConfigDetails();
                        self.totalConfigDetails(filteredConfigDetails.length);
                        return filteredConfigDetails;
                    });

                    self.editConfig = function (add) {
                        if (isBinding) return;

                        cmdAdd.tooltip("hide");
                        cmdEdit.tooltip("hide");
                        enableButtons(false);

                        var id = add ? 0 : self.selectedConfigId();   
                        var title = "<span class='glyphicon glyphicon-{glyphicon}' style='color: #fff'></span>" + " {action} Configuration";
                        title = add 
                            ? title.replace("{glyphicon}", "duplicate").replace("{action}", "Duplicate")
                            : title.replace("{glyphicon}", "pencil").replace("{action}", "Edit");

                        utilities.partialview.show({
                            url: site.url + "PhidgetConfig/GetConfigEditView/" + id,
                            type: add ? BootstrapDialog.TYPE_SUCCESS : BootstrapDialog.TYPE_PRIMARY,
                            title: title,
                            params: { selectedConfigid: self.selectedConfigId() },
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
                                        params: { config: config, selectedConfigId: self.selectedConfigId() }
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
                                                        selectedConfigId: self.selectedConfigId()
                                                    }
                                                })
                                                .then(function(saveResult) {                                                    
                                                    if (saveResult.ConfigList.length > 0) {
                                                        var c = saveResult.ConfigList[0];

                                                        if (add) {
                                                            createConfigArray({ list: saveResult.ConfigList, insert: true });
                                                            createConfigDetailArray({ list: saveResult.ConfigDetailList, insert: true });
                                                            self.sortConfigs();
                                                            self.sort();

                                                            if (c.Id === self.activeConfigId()) {
                                                                if (c.IsActiveEventLog === true)
                                                                    self.activeEventLogDesc("On");
                                                                else
                                                                    self.activeEventLogDesc("Off");
                                                            }
                                                        } else {
                                                            self.update(saveResult.ConfigList);
                                                        }

                                                        self.selectedConfigId(saveResult.ConfigList[0].Id);
                                                    }
                                                    self.enableDetail();
                                                    enableButtons(true);
                                                })
                                                .catch(function() {
                                                    self.enableDetail();
                                                    enableButtons(true);
                                                })
                                                .catch(function() {
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
                        });
                    };

                    self.deleteConfig = function () {
                        if (isBinding) return;
                        if (cmdDelete.is("[disabled]")) return;
                        var id = self.selectedConfigId();
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
                                .then(function(deleteResult) {
                                    createConfigArray({ list: deleteResult.ConfigList, insert: false });
                                    createConfigDetailArray({ list: deleteResult.ConfigDetailList, insert: false });
                                    self.enableDetail();
                                    var activeId = deleteResult.ConfigList
                                        .filter(function(value) { return value.IsActive === true })[0].Id;
                                    self.selectedConfigId(activeId);
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

                        var id = (typeof row.id !== "undefined") ? row.id : 0;
                        var add = (id === 0);

                        $("#edit_" + id).tooltip("hide");
                        cmdAddDetail.tooltip("hide");
                        enableButtons(false);  

                        var title = "<span class='glyphicon glyphicon-pencil' style='color: #fff'></span>";
                        title = add
                            ? title + " Add Configuration Item"
                            : title + " Edit Configuration Item";

                        if (add) {
                            self.selectedConfigDetail({});
                        } else {
                            var configDetail = self.getConfigDetail(id);
                            self.selectedConfigDetail(configDetail);
                        }

                        utilities.partialview.show({
                            url: "PhidgetConfig/GetConfigDetailEditView/" + id,
                            type: add ? BootstrapDialog.TYPE_SUCCESS : BootstrapDialog.TYPE_PRIMARY,
                            title: title,
                            params: { id: id, configId: self.selectedConfigId() },
                            focus: "txtDescription",
                            buttonOKClass: add ? "btn-success" : "btn-primary",
                            cancelled: function () { enableButtons(true); },
                            callback: function(dialog) {
                                var configdetail = self.getConfigDetailFromDialog();
                                configdetail.ConfigId = self.selectedConfigId();
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
                                                .then(function (saveResult) {
                                                    if (add) {
                                                        createConfigDetailArray({ list: saveResult.ConfigDetailList, insert: true });
                                                    } else {
                                                        self.updateDetail(saveResult.ConfigDetailList);
                                                    }

                                                    if (saveResult.ConfigDetailList.length > 0) {
                                                        self.marqueeRows(saveResult.ConfigDetailList);
                                                        self.selectedConfigDetail({});
                                                        self.unHighlightRows();
                                                    }

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

                        if (id === 0) return;

                        var title = "<span class='glyphicon glyphicon-trash' style='color: #fff'></span>";                        
                        var cd = self.getConfigDetail(id);

                        utilities.confirm.show({
                            type: BootstrapDialog.TYPE_DANGER,
                            title: title + " Delete Configuration Detail",
                            message: "Delete <b>" + cd.phidgettype() + "</b> detail item for actvity <i><b>" + cd.description() + "</b></i>?",
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
                                        configId: self.selectedConfigId(),
                                        isActive: self.selectedConfigId() === self.activeConfigId()
                                    },
                                    waitMessage: "Deleting..."
                                })
                                .then(function(deleteResult) {
                                    self.removeDetail(deleteResult.DeletedId);
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
                                var configId = self.selectedConfigId();

                                utilities.job.execute({
                                    url: site.url + "PhidgetConfig/Activate/",
                                    params: { configId: configId },
                                    waitMessage: "Activating..."
                                })
                                .then(function(activateResult) {
                                    if (activateResult.ConfigList.length > 0) {
                                        createConfigArray({ list: activateResult.ConfigList, insert: false });
                                        activeConfig = activateResult.ConfigList.filter(function(value) {
                                            return value.IsActive;
                                        })[0];
                                        self.activeConfigId(activeConfig.Id);
                                        self.selectedConfigId(activeConfig.Id);
                                        self.activeConfigDesc(activeConfig.Description);
                                        if (activeConfig.IsActiveEventLog === true)
                                            self.activeEventLogDesc("On");
                                        else
                                            self.activeEventLogDesc("Off");
                                    }
                                    
                                    self.enableDetail();
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
                            IsActive: self.selectedConfigId() === self.activeConfigId()
                        };
                    };

                    self.getConfig = function(configid) {
                        var config = null;

                        ko.utils.arrayForEach(self.configs(),
                            function(item) {
                                if (item.id === configid) {
                                    config = item;
                                }
                            });

                        return config;
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
                            Id: self.selectedConfigDetail().id,
                            Description: description,
                            Location: location,
                            PhidgetTypeId: phidgettypeid,
                            PhidgetStyleTypeId: phidgetstyletypeid,
                            ResponseTypeId: responsetypeid,
                            IsActive: self.selectedConfigId() === self.activeConfigId()
                        };
                    };

                    self.marqueeRows = function (configDetailList) {
                        $(configDetailList).each(function (index, value) {
                            var id = value.Id;
                            $("#row_" + id).addClass("row-added");
                            setTimeout(function () {
                                $("#row_" + id).removeClass("row-added");
                            }, 1500);
                        });
                    };

                    self.enableDetail = function () {
                        if (self.isLoadingConfigs()) return;

                        var configId = self.selectedConfigId();

                        if (self.canDeleteConfig(configId)) {
                            cmdDelete.removeAttr("disabled");
                        } else {
                            cmdDelete.attr("disabled", "disabled");
                        }
                    };

                    self.canDeleteConfig = function (id) {
                        return self.configs()
                            .filter(function(value) { return value.id === id; })
                            [0].candelete();
                    };

                    self.selectedConfigDesc = function () {
                        return self.configs()
                            .filter(function (value) { return value.id === self.selectedConfigId(); })
                            [0].description();
                    };

                    self.update = function (configList) {
                        $(configList).each(function (index, value) {
                            var c = self.getConfig(value.Id);

                            c.description(value.Description);
                            c.isActive(value.IsActive);
                            c.isactiveeventlog(value.IsActiveEventLog);
                            c.candelete(value.CanDelete);
                        });
                    }

                    self.updateDetail = function (configDetailList) {
                        $(configDetailList).each(function(index, value) {
                            var cd = self.getConfigDetail(value.Id);

                            cd.phidgettype(value.PhidgetType);
                            cd.phidgetstyletype(value.PhidgetStyleType);
                            cd.description(value.Description);
                            cd.location(value.Location);
                            cd.responsetype(value.ResponseType);
                            cd.issystem(value.IsSystem);
                            cd.sortorder = value.SortOrder;
                        });
                    }

                    self.removeDetail = function (id) {
                        var idx = self.configDetails().findIndex(function (value) {
                            return value.id === id;
                        });
                        self.configDetails.splice(idx, 1);
                    }
                }
            })
            .catch(function (error) {
                $("#loading-container").hide();
                $("#error-container")
                    .html("<div><h2>Data load error:</h2></div>")
                    .append("<div>" + error.message + "</div>")
                    .append("<div><h3>Please try refreshing the page</h3></div>");
                $("#error-container").show();
            });
        }          
    }
})(jQuery);