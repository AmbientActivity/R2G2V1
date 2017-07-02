﻿/*!
 * 1.0 Keebee AAT Copyright © 2016
 * Configs/Index.js
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

            $.get(site.url + "PhidgetConfig/GetData/")
                .done(function (data) {
                    $.extend(lists, data);
                    activeConfig = lists.ConfigList.filter(function (value) { return value.IsActive; })[0];

                    $("#loading-container").hide();
                    $("#tblConfigDetail").show();
                    $("#col-button-container").show();

                    cmdAdd.prop("hidden", true);
                    cmdEdit.prop("hidden", false);
                    cmdDelete.prop("hidden", false);
                    cmdActivate.prop("disabled", false);
                    cmdAddDetail.prop("disabled", false);

                    ko.bindingHandlers.tableUpdated = {
                        update: function (element, valueAccessor, allBindings) {
                            ko.unwrap(valueAccessor());
                            isBinding = false;
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

                        if (activeConfig.IsActiveEventLog === true)
                            self.activeEventLogDesc = ko.observable("On");
                        else
                            self.activeEventLogDesc = ko.observable("Off");

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

                        self.showEditDialog = function (add) {
                            if (isBinding) return;

                            $("body").css("cursor", "progress");
                            enableButtons(false);

                            var id = 0;

                            var title;
                            var type = BootstrapDialog.TYPE_PRIMARY;
                            var btnClass = "btn-primary";

                            if (add) {
                                title = "<span class='glyphicon glyphicon-duplicate' style='color: #fff'></span>" + " Duplicate Configuration";
                                type = BootstrapDialog.TYPE_SUCCESS;
                                btnClass = "btn-success";
                            } else {
                                id = self.selectedConfig();
                                title = "<span class='glyphicon glyphicon-pencil' style='color: #fff'></span>" + " Edit Configuration";
                            }

                            $.get(site.url + "PhidgetConfig/GetConfigEditView/" + id,
                                { selectedConfigid: self.selectedConfig() })
                                .done(function (message) {
                                    BootstrapDialog.show({
                                        type: type,
                                        title: title,
                                        message: $("<div></div>").append(message),
                                        onshown: function () {
                                            $("body").css("cursor", "default");
                                            $("#txtDescription").focus();
                                        },
                                        closable: false,
                                        buttons: [
                                            {
                                                label: "Cancel",
                                                action: function (dialog) {
                                                    dialog.close();
                                                    enableButtons(true);
                                                }
                                            }, {
                                                label: "OK",
                                                hotkey: 13,  // enter
                                                cssClass: btnClass,
                                                action: function (dialog) {
                                                    self.saveConfig(id).then(function (result) {
                                                        if (result.ErrorMessages === null) {
                                                            $.extend(lists, result);
                                                            createConfigArray(lists.ConfigList);
                                                            createConfigDetailArray(lists.ConfigDetailList);
                                                            self.selectedConfig(result.SelectedId);
                                                            activeConfig = result.ConfigList.filter(function (value) { return value.IsActive; })[0];
                                                            self.activeConfig(activeConfig.Id);
                                                            self.activeConfigDesc(activeConfig.Description);
                                                            if (activeConfig.IsActiveEventLog === true)
                                                                self.activeEventLogDesc("On");
                                                            else
                                                                self.activeEventLogDesc("Off");
                                                            self.enableDetail();
                                                            dialog.close();
                                                            enableButtons(true);
                                                        } else {
                                                            $("#validation-container").show();
                                                            $("#validation-container").html("");
                                                            var html = "<ul>";
                                                            for (var i = 0; i < result.ErrorMessages.length; i++) {
                                                                var msg = result.ErrorMessages[i];
                                                                html = html + "<li>" + msg + "</li>";
                                                            }
                                                            html = html + "</ul>";
                                                            $("#validation-container").append(html);
                                                        }
                                                    });
                                                }
                                            }
                                        ]
                                    });
                                });
                        };

                        self.showDeleteDialog = function () {
                            if (isBinding) return;
                            if (cmdDelete.is("[disabled]")) return;
                            var id = self.selectedConfig();
                            enableButtons(false);

                            var title = "<span class='glyphicon glyphicon-trash' style='color: #fff'></span>";
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
                                            enableButtons(true);
                                        }
                                    }, {
                                        label: "Yes, Delete",
                                        cssClass: "btn-danger",
                                        action: function (dialog) {
                                            self.deleteConfig(id).then(function (result) {
                                                lists.ConfigList = result.ConfigList;
                                                createConfigArray(lists.ConfigList);
                                                self.enableDetail();
                                                var activeId = lists.ConfigList
                                                    .filter(function(value) { return value.IsActive === true })[0].Id;
                                                self.selectedConfig(activeId);
                                                dialog.close();
                                                enableButtons(true);
                                            });
                                        }
                                    }
                                ]
                            });
                        };

                        self.showEditDetailDialog = function (row) {
                            if (isBinding) return;

                            $("body").css("cursor", "progress");
                            enableButtons(false);

                            var id = (row.id !== undefined ? row.id : 0);

                            if (id > 0) {
                                self.highlightRow(row);
                            }
                       
                            var title = "<span class='glyphicon glyphicon-pencil' style='color: #fff'></span>";
                            var type = BootstrapDialog.TYPE_PRIMARY;
                            var btnClass = "btn-primary";

                            if (id > 0) {
                                title = title + " Edit Configuration Item";
                                var configDetail = self.getConfigDetail(id);
                                self.selectedConfigDetail(configDetail);
                            } else {
                                title = title + " Add Configuration Item";
                                type = BootstrapDialog.TYPE_SUCCESS;
                                btnClass = "btn-success";
                                self.selectedConfigDetail([]);
                            }

                            $.get(site.url + "PhidgetConfig/GetConfigDetailEditView/" + id, 
                                { id: id, configId: self.selectedConfig() }, false)
                                .done(function (message) {
                                    BootstrapDialog.show({
                                        title: title,
                                        type: type,
                                        message: $("<div></div>").append(message),
                                        onshown: function () {
                                            $("body").css("cursor", "default");
                                            $("#txtDescription").focus();
                                        },
                                        closable: false,
                                        buttons: [
                                            {
                                                label: "Cancel",
                                                action: function (dialog) {
                                                    dialog.close();
                                                    enableButtons(true);
                                                }
                                            }, {
                                                label: "OK",
                                                hotkey: 13,  // enter
                                                cssClass: btnClass,
                                                action: function (dialog) {
                                                    self.saveConfigDetail().then(function (result) {
                                                        if (result.ErrorMessages === null) {
                                                            lists.ConfigDetailList = result.ConfigDetailList;
                                                            createConfigDetailArray(lists.ConfigDetailList);
                                                            self.selectedConfigDetail(self.getConfigDetail(result.SelectedId));
                                                            self.highlightRow(self.selectedConfigDetail());
                                                            self.enableDetail();
                                                            self.sort();
                                                            dialog.close();
                                                            enableButtons(true);
                                                        } else {
                                                            $("#validation-container").show();
                                                            $("#validation-container").html("");
                                                            var html = "<ul>";
                                                            for (var i = 0; i < result.ErrorMessages.length; i++) {
                                                                var msg = result.ErrorMessages[i];
                                                                html = html + "<li>" + msg + "</li>";
                                                            }
                                                            html = html + "</ul>";
                                                            $("#validation-container").append(html);
                                                        }
                                                    });
                                                }
                                            }
                                        ]
                                    });
                            });
                        };

                        self.showDeleteDetailDialog = function (row) {
                            var id = (row.id !== undefined ? row.id : 0);
                            enableButtons(false);

                            if (id > 0) {
                                self.highlightRow(row);
                            } else {
                                return;
                            }

                            var title = "<span class='glyphicon glyphicon-trash' style='color: #fff'></span>";                        
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
                                            enableButtons(true);
                                        }
                                    }, {
                                        label: "Yes, Delete",
                                        cssClass: "btn-danger",
                                        action: function (dialog) {
                                            self.deleteConfigDetail(row.id).then(function (result) {
                                                lists.ConfigDetailList = result.ConfigDetailList;
                                                createConfigDetailArray(lists.ConfigDetailList);
                                                self.enableDetail();
                                                dialog.close();
                                                enableButtons(true);
                                            });
                                        }
                                    }
                                ]
                            });
                        };

                        self.showActivatConfigDialog = function (id) {
                            var title = "<span class='glyphicon glyphicon-ok' style='color: #fff'></span>";
                            if (id <= 0) return;
                            enableButtons(false);

                            BootstrapDialog.show({
                                title: title + " Activate Configuration",
                                message: "Activate the configuration <b>" + self.selectedConfigDesc() + "</b>?",
                                closable: false,
                                buttons: [
                                    {
                                        label: "Cancel",
                                        action: function (dialog) {
                                            dialog.close();
                                            enableButtons(true);
                                        }
                                    }, {
                                        label: "OK",
                                        cssClass: "btn-primary",
                                        action: function (dialog) {
                                            self.activateConfig().then(function(result) {
                                                $.extend(lists, result);
                                                createConfigArray(lists.ConfigList);
                                                createConfigDetailArray(lists.ConfigDetailList);
                                                activeConfig = result.ConfigList.filter(function (value) { return value.IsActive; })[0];
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
                                            });
                                        }
                                    }
                                ]
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

                        self.saveConfig = function (id) {
                            return new Promise(function(resolve, reject) {
                                var config = self.getConfigFromDialog();
                                config.Id = id;

                                var jsonData = JSON.stringify(config);

                                $.post("PhidgetConfig/Save/",
                                    { config: jsonData, selectedConfigId: self.selectedConfig() })
                                    .done(function (result) {
                                        resolve(result);
                                    })
                                    .error(function (result) {
                                        reject(result);
                                    });
                            });
                        };

                        self.deleteConfig = function (id) {
                            return new Promise(function(resolve, reject) {
                                $.post(site.url + "PhidgetConfig/Delete/", { id: id })
                                    .done(function (result) {
                                        resolve(result);
                                    })
                                    .error(function (result) {
                                        reject(result);
                                    });
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
                            var location = $.trim($("#txtLocation").val());
                            var phidgettypeid = $("#ddlPhidgetTypes").val();
                            var phidgetstyletypeid = $("#ddlPhidgetStyleTypes").val();
                            var responsetypeid = $("#ddlResponseTypes").val();

                            return {
                                Id: self.selectedConfigDetail().id, Description: description, Location: location, PhidgetTypeId: phidgettypeid, PhidgetStyleTypeId: phidgetstyletypeid, ResponseTypeId: responsetypeid, IsActive: self.selectedConfig() === self.activeConfig()
                            };
                        };

                        self.saveConfigDetail = function () {
                            return new Promise(function(resolve, reject) {
                                var configdetail = self.getConfigDetailFromDialog();
                                configdetail.ConfigId = self.selectedConfig();
                                var jsonData = JSON.stringify(configdetail);

                                $.post(site.url + "PhidgetConfig/SaveDetail/", { configdetail: jsonData })
                                    .done(function (result) {
                                        resolve(result);
                                    })
                                    .error(function (result) {
                                        reject(result);
                                    });
                            });
                        };

                        self.deleteConfigDetail = function (id) {
                            return new Promise(function(resolve, reject) {
                                $.post(site.url + "PhidgetConfig/DeleteDetail/",
                                    {
                                        id: id,
                                        configId: self.selectedConfig(),
                                        isActive: self.selectedConfig() === self.activeConfig()
                                    })
                                    .done(function (result) {
                                        resolve(result);
                                    })
                                    .error(function (result) {
                                        reject(result);
                                    });
                            });
                        };

                        self.activateConfig = function () {
                            return new Promise(function(resolve, reject) {
                                var configId = self.selectedConfig();

                                $.get(site.url + "PhidgetConfig/Activate/", { configId: configId })
                                    .done(function (result) {
                                        resolve(result);
                                    })
                                    .error(function (result) {
                                        reject(result);
                                    });
                            });
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
            });
        }          
    }
})(jQuery);