/*!
 * 1.0 Keebee AAT Copyright © 2017
 * AmbientInvitation/Index.js
 * Author: John Charlton
 * Date: 2017-08
 */

; (function ($) {

    ambientinvitation.index = {
        init: function () {
            var isBinding = true;

            var primarySortKey = "id";
            var cmdAdd = $("#add");

;            var lists = {
                AmbientInvitationDetailList: []
            };

            cmdAdd.attr("disabled", "disabled");

            utilities.job.execute({
                url: "AmbientInvitations/GetData/"
            })
            .then(function (data) {
                $.extend(lists, data);

                $("#error-container").hide();
                $("#loading-container").hide();
                $("#table-panel").show();

                cmdAdd.removeAttr("disabled");

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

                        var table = element.parentNode;
                        var noMediaMessage = $("#no-rows-message");
                        var tablePanel = $("#table-panel");

                        if (table.rows.length > 1) {
                            noMediaMessage.hide();
                            tablePanel.show();
                        } else {
                            noMediaMessage.show();
                            tablePanel.hide();
                        }
                    }
                }

                ko.applyBindings(new AmbientInvitationViewModel());

                function AmbientInvitation(id, message, isexecuterandom) {
                    var self = this;

                    self.id = id;
                    self.message = ko.observable(message);
                    self.isexecuterandom = ko.observable(isexecuterandom);
                }

                function AmbientInvitationViewModel() {
                    var self = this;

                    self.ambientInvitations = ko.observableArray([]);

                    self.selectedAmbientInvitation = ko.observable({});
                    self.isLoading = ko.observable(false);
                    self.totalAmbientInvitations = ko.observable(0);

                    createAmbientInvitationArray({ list: lists.AmbientInvitationList, insert: false });

                    function enableButtons(enable) {
                        if (enable) {
                            cmdAdd.removeAttr("disabled");
                        } else {
                            cmdAdd.attr("disabled", "disabled");
                        }
                    }

                    function createAmbientInvitationArray(params) {
                        var cfg = {
                            list: [],
                            insert: false
                        };

                        $.extend(cfg, params);

                        self.isLoading(true);

                        if (!cfg.insert)
                            self.ambientInvitations.removeAll();

                        $(cfg.list).each(function (index, value) {
                            self.ambientInvitations.push(new AmbientInvitation(
                                value.Id,
                                value.Message,
                                value.IsExecuteRandom));
                        });

                        self.isLoading(false);
                    };

                    self.columns = ko.computed(function () {
                        var arr = [];
                        arr.push({ title: "Message", sortKey: "message", cssClass: "col-message", visible: true });
                        arr.push({ title: "Random", sortKey: "executerandom", cssClass: "col-executerandom", visible: true });
                        return arr;
                    });

                    self.sort = function () {
                        self.ambientInvitations(utilities.sorting.sortArray(
                        {
                            array: self.ambientInvitations(),
                            columns: self.columns(),
                            sortKey: primarySortKey,
                            primarySortKey: primarySortKey,
                            descending: false
                        }));
                    };

                    self.ambientInvitationsTable = ko.computed(function () {
                        self.totalAmbientInvitations(self.ambientInvitations().length);
                        return self.ambientInvitations();
                    });

                    // to prevent dialog from opening twice
                    var isEditLoading = false;
                    self.edit = function (row) {
                        if (isBinding) return;

                        if (isEditLoading) return;
                        isEditLoading = true;

                        var id = (typeof row.id !== "undefined") ? row.id : 0;
                        var add = (id === 0);

                        $("#edit_" + id).tooltip("hide");
                        cmdAdd.tooltip("hide");
                        enableButtons(false);

                        var title = "<span class='glyphicon glyphicon-pencil' style='color: #fff'></span>";
                        title = add
                            ? title + " Add Ambient Invitation"
                            : title + " Edit Ambient Invitation";

                        if (add) {
                            self.selectedAmbientInvitation({});
                        } else {
                            var ambientInvitation = self.getAmbientInvitation(id);
                            self.selectedAmbientInvitation(ambientInvitation);
                        }

                        utilities.partialview.show({
                            url: "AmbientInvitations/GetEditView/" + id,
                            type: add ? BootstrapDialog.TYPE_SUCCESS : BootstrapDialog.TYPE_PRIMARY,
                            title: title,
                            params: { id: id },
                            focus: "txtMessage",
                            buttonOKClass: add ? "btn-success" : "btn-primary",
                            cancelled: function () {
                                enableButtons(true);
                                isEditLoading = false;
                            },
                            callback: function (dialog) {
                                var invitation = self.getInvitationFromDialog();
                                utilities.job.execute({
                                    url: "AmbientInvitations/Validate",
                                    type: "POST",
                                    params: { ambientInvitation: invitation }
                                })
                                .then(function (validateResult) {
                                    isEditLoading = false;
                                    if (validateResult.ValidationMessages === null) {
                                        dialog.close();
                                        utilities.job.execute({
                                            url: "AmbientInvitations/Save",
                                            type: "POST",
                                            params: { ambientInvitation: invitation }
                                        })
                                            .then(function (saveResult) {
                                                if (add) {
                                                    createAmbientInvitationArray({
                                                        list: saveResult.AmbientInvitationList,
                                                        insert: true
                                                    });
                                                } else {
                                                    self.update(saveResult.AmbientInvitationList);
                                                }

                                                if (saveResult.AmbientInvitationList.length > 0) {
                                                    self.marqueeRows(saveResult.AmbientInvitationList);
                                                    self.selectedAmbientInvitation({});
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
                                .catch(function () {
                                    enableButtons(true);
                                    isEditLoading = false;
                                });
                            }
                        });
                    };

                    self.delete = function (row) {
                        var id = (row.id !== undefined ? row.id : 0);

                        $("#delete_" + row.id).tooltip("hide");

                        enableButtons(false);

                        if (id === 0) return;

                        var title = "<span class='glyphicon glyphicon-trash' style='color: #fff'></span>";
                        var ai = self.getAmbientInvitation(id);

                        utilities.confirm.show({
                            type: BootstrapDialog.TYPE_DANGER,
                            title: title + " Delete Ambient Invitation",
                            message: "Delete ambient invitation <i><b>" + ai.message() + "</b></i>?",
                            buttonOK: "Yes, Delete",
                            buttonOKClass: "btn-danger"
                        })
                        .then(function (confirm) {
                            if (confirm) {
                                utilities.job.execute({
                                    url: "AmbientInvitations/Delete",
                                    type: "POST",
                                    params: {
                                        id: id
                                    },
                                    waitMessage: "Deleting..."
                                })
                                .then(function (deleteResult) {
                                    self.remove(deleteResult.DeletedId);
                                    self.enableDetail();
                                    enableButtons(true);
                                })
                                .catch(function () {
                                    self.enableDetail();
                                    enableButtons(true);
                                });
                            } else {
                                enableButtons(true);
                            }
                        });
                    };

                    self.getAmbientInvitation = function (ambientInvitationId) {
                        var ambientInvitation = null;

                        ko.utils.arrayForEach(self.ambientInvitations(), function (item) {
                            if (item.id === ambientInvitationId) {
                                ambientInvitation = item;
                            }
                        });

                        return ambientInvitation;
                    };

                    self.getInvitationFromDialog = function () {
                        var message = $.trim($("#txtMessage").val());
                        var isexecuterandom = $.trim($("#chkIsExecuteRandom").is(":checked"));

                        return {
                            Id: self.selectedAmbientInvitation().id,
                            Message: message,
                            IsExecuteRandom: isexecuterandom
                        };
                    };

                    self.marqueeRows = function (ambientInvitationList) {
                        $(ambientInvitationList).each(function (index, value) {
                            var id = value.Id;
                            $("#row_" + id).addClass("row-added");
                            setTimeout(function () {
                                $("#row_" + id).removeClass("row-added");
                            }, 1500);
                        });
                    };

                    self.enableDetail = function () {
                        if (self.isLoading()) return;
                    };

                    self.update = function (ambientInvitationList) {
                        $(ambientInvitationList).each(function (index, value) {
                            var ai = self.getAmbientInvitation(value.Id);

                            ai.message(value.Message);
                            ai.isexecuterandom(value.IsExecuteRandom);
                        });
                    }

                    self.remove = function (id) {
                        var idx = self.ambientInvitations().findIndex(function (value) {
                            return value.id === id;
                        });
                        self.ambientInvitations.splice(idx, 1);
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