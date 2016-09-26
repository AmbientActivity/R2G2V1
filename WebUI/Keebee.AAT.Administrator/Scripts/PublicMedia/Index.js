/*!
 * PublicMedia/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

function CuteWebUI_AjaxUploader_OnPostback() {
    $.blockUI({ message: "<h4>Saving...</h4>" });
    document.forms[0].submit();
}

function CuteWebUI_AjaxUploader_OnError(msg) {
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_DANGER,
        title: "File Error",
        message: msg
    });

    EnableScreen();
    return false;
}

function CuteWebUI_AjaxUploader_OnTaskError(obj, msg, reason) {
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_DANGER,
        title: "Task Error",
        message: "Error uploading file <b>" + obj.FileName + "</b>.\nMessage: " + msg
    });

    EnableScreen();
    return false;
}

function CuteWebUI_AjaxUploader_OnStop() {
    EnableScreen();
}

function CuteWebUI_AjaxUploader_OnSelect() {
    DisableScreen();
}

function EnableScreen() {
    $("#lnkGoBack").removeAttr("hidden");
    $("#lblGoBackDisabled").attr("hidden", "hidden");
    $("#txtSearchFilename").removeAttr("disabled");
    $("#uploadbutton").removeAttr("disabled");
    $("select").removeAttr("disabled");
    $("#main-menu").removeAttr("hidden");
}

function DisableScreen() {
    $("#lnkGoBack").attr("hidden", "hidden");
    $("#lblGoBackDisabled").removeAttr("hidden");
    $("#txtSearchFilename").attr("disabled", "disabled");
    $("#uploadbutton").attr("disabled", "disabled");
    $("select").attr("disabled", "disabled");
    $("#main-menu").attr("hidden", "hidden");
}

; (function ($) {

    var HIGHLIGHT_ROW_COLOUR = "#e3e8ff";

    publicmedia.index = {
        init: function () {

            var cmdDelete = $("#delete");

            var _sortDescending = false;
            var _currentSortKey = "filename";

            var lists = {
                FileList: [],
                MediaPathTypeList: [],
                ResponseTypeList: []
            };

            loadData();

            ko.applyBindings(new FileViewModel());

            function loadData() {
                var mediaPathTypeId = $("#mediaPathTypeId").val();

                $.ajax({
                    type: "GET",
                    url: site.url + "PublicMedia/GetData?" + "mediaPathTypeId=" + mediaPathTypeId,
                    dataType: "json",
                    traditional: true,
                    async: false,
                    success: function (data) {
                        $.extend(lists, data);
                    }
                });
            }

            function MediaPathType(id, description) {
                var self = this;

                self.id = id;
                self.description = description;
            }

            function ResponseType(id, description) {
                var self = this;

                self.id = id;
                self.description = description;
            }

            function File(streamid, filename, filetype, filesize, path, responsetypeid) {
                var self = this;

                self.streamid = streamid;
                self.filename = filename;
                self.filetype = filetype;
                self.filesize = filesize;
                self.path = path;
                self.responsetypeid = responsetypeid;
                self.isselected = false;
            }

            function FileViewModel() {
                var tblFile = $("#tblFile");

                var self = this;

                self.files = ko.observableArray([]);
                self.mediaPathTypes = ko.observableArray([]);
                self.responseTypes = ko.observableArray([]);
                self.selectedFile = ko.observable();
                self.isLoadingResponseTypes = ko.observable(false);
                self.selectedMediaPathType = ko.observable($("#mediaPathTypeId").val());
                self.selectedResponseType = ko.observable($("#responseTypeId").val());
                self.filenameSearch = ko.observable("");
                self.totalFiles = ko.observable(0);
                self.selectAllIsSelected = ko.observable(false);
                self.selectedStreamIds = ko.observable([]);

                createFileArray(lists.FileList);
                createMediaPathTypeArray(lists.MediaPathTypeList);
                createResponseTypeArray(lists.ResponseTypeList);

                function createFileArray(list) {
                    self.files.removeAll();
                    $(list).each(function (index, value) {
                        pushFile(value);
                    });
                };

                function createMediaPathTypeArray(list) {
                    self.mediaPathTypes.removeAll();
                    $(list).each(function (index, value) {
                        pushMediaPathType(value);
                    });
                };

                function createResponseTypeArray(list) {
                    self.isLoadingResponseTypes(true);
                    self.responseTypes.removeAll();
                    $(list).each(function (index, value) {
                        pushResponseType(value);
                    });
                    self.isLoadingResponseTypes(false);
                };

                self.columns = ko.computed(function () {
                    var arr = [];
                    arr.push({ title: "Name", sortable: true, sortKey: "filename" });
                    arr.push({ title: "Type", sortable: true, sortKey: "filetype" });
                    arr.push({ title: "Size", sortable: true, sortKey: "filesize" });
                    return arr;
                });

                function pushMediaPathType(value) {
                    self.mediaPathTypes.push(new MediaPathType(value.Id, value.Description));
                }

                function pushResponseType(value) {
                    self.responseTypes.push(new ResponseType(value.Id, value.Description));
                }

                function pushFile(value) {
                    self.files.push(new File(value.StreamId, value.Filename, value.FileType, value.FileSize, value.Path, value.ResponseTypeId));
                }

                self.selectedFile(self.files()[0]);

                self.sort = function (header) {
                    var afterSave = typeof header.afterSave != "undefined" ? header.afterSave : false;
                    var sortKey;

                    if (!afterSave) {
                        sortKey = header.sortKey;

                        if (sortKey !== _currentSortKey) {
                            _sortDescending = false;
                        } else {
                            _sortDescending = !_sortDescending;
                        }
                        _currentSortKey = sortKey;
                    } else {
                        sortKey = _currentSortKey;
                    }

                    $(self.columns()).each(function (index, value) {
                        if (value.sortKey === sortKey) {
                            self.files.sort(function (a, b) {
                                if (_sortDescending) {
                                    return a[sortKey].toString().toLowerCase() > b[sortKey].toString().toLowerCase()
                                        ? -1 : a[sortKey].toString().toLowerCase() < b[sortKey].toString().toLowerCase()
                                        || a.filename.toLowerCase() > b.filename.toLowerCase() ? 1 : 0;
                                } else {
                                    return a[sortKey].toString().toLowerCase() < b[sortKey].toString().toLowerCase()
                                        ? -1 : a[sortKey].toString().toLowerCase() > b[sortKey].toString().toLowerCase()
                                        || a.filename.toLowerCase() > b.filename.toLowerCase() ? 1 : 0;
                                }
                            });
                        }
                    });
                };

                self.filteredFiles = ko.computed(function () {
                    $("#responseTypeId").val(self.selectedResponseType());

                    return ko.utils.arrayFilter(self.files(), function (f) {
                        return (
                            self.filenameSearch().length === 0 ||
                            f.filename.toLowerCase().indexOf(self.filenameSearch().toLowerCase()) !== -1) &&
                            f.responsetypeid === self.selectedResponseType();
                    });
                });

                self.filesTable = ko.computed(function () {
                    var filteredFiles = self.filteredFiles();
                    self.totalFiles(filteredFiles.length);

                    return filteredFiles;
                });

                // ------------------

                self.doPostBack = function () {
                    $("#mediaPathTypeId").val(self.selectedMediaPathType());
                    $("#responseTypeId").val(self.selectedResponseType());
                    document.forms[0].submit();
                }

                self.showDeleteSelectedDialog = function () {
                    self.showSelectedFileDeleteDialog();
                };

                self.showPreview = function (row) {
                    self.highlightRow(row);
                    self.showFeatureNotDoneYetDialog();
                };

                self.showAddPublicDialog = function () {
                    self.showFeatureNotDoneYetDialog();
                };

                self.showSelectedFileDeleteDialog = function () {
                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_DANGER,
                        title: "Delete Files?",
                        message: "Delete all selected files?",
                        closable: false,
                        buttons: [
                            {
                                label: "Cancel",
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }, {
                                label: "Yes, Delete",
                                cssClass: "btn-danger",
                                action: function (dialog) {
                                    self.deleteSelected();
                                    dialog.close();
                                    $("body").css("cursor", "default");
                                }
                            }
                        ]
                    });
                };

                self.showFeatureNotDoneYetDialog = function () {
                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_INFO,
                        title: "Under Development",
                        message: "This feature has not been implemented yet.",
                        closable: false,
                        buttons: [
                            {
                                label: "Close",
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }
                        ]
                    });
                };

                self.getFile = function (streamid) {
                    var file = null;

                    ko.utils.arrayForEach(self.files(), function (item) {
                        if (item.streamid === streamid) {
                            file = item;
                        }
                    });

                    return file;
                };

                self.selectAllRows = function () {
                    $.each(self.filteredFiles(), function (item, value) {

                        if (self.selectAllIsSelected()) 
                            self.selectedStreamIds().push(value.streamid);
                        else
                            self.selectedStreamIds().pop(value.streamid);

                        value.isselected = self.selectAllIsSelected();
                        var chk = tblFile.find("#chk_" + value.streamid);
                        chk.prop("checked", self.selectAllIsSelected());
                    });

                    self.highlightSelectedRows();
                    self.enableDetail();

                    return true;
                };

                self.selectFile = function (row) {
                    if (typeof row === "undefined") return false;
                    if (row === null) return false;

                    if (row.isselected)
                        self.selectedStreamIds().push(row.streamid);
                    else
                        self.selectedStreamIds().pop(row.streamid);

                    self.highlightSelectedRows();
                    self.enableDetail();

                    return true;
                };

                self.highlightSelectedRows = function () {
                    var rows = tblFile.find("tr:gt(0)");
                    rows.each(function () {
                        $(this).css("background-color", "#ffffff");
                    });
                    
                    var selected = self.files()
                        .filter(function (data) { return data.isselected; });

                    $.each(selected, function (item, value) {
                        var r = tblFile.find("#row_" + value.streamid);
                        r.css("background-color", HIGHLIGHT_ROW_COLOUR);
                        tblFile.attr("tr:hover", HIGHLIGHT_ROW_COLOUR);
                    });

                    return true;
                };

                self.deleteSelected = function () {
                    $("body").css("cursor", "wait");

                    var streamIds = self.selectedStreamIds();
                    var mediaPathTypeId = $("#mediaPathTypeId").val();
                    var responseTypeId = $("#responseTypeId").val();

                    $.blockUI({ message: "<h4>Deleting files...</h4>" });

                    $.ajax({
                        type: "POST",
                        async: true,
                        traditional: true,
                        url: site.url + "PublicMedia/DeleteSelected/",
                        data:
                        {
                            streamIds: streamIds,
                            mediaPathTypeId: mediaPathTypeId,
                            responseTypeId: responseTypeId
                        },
                        dataType: "json",
                        success: function (data) {
                            $("body").css("cursor", "default");
                            $.unblockUI();
                            if (data.Success) {
                                lists.FileList = data.FileList;
                                createFileArray(lists.FileList);
                                self.sort({ afterSave: true });
                                self.enableDetail();
                            } else {
                                BootstrapDialog.show({
                                    type: BootstrapDialog.TYPE_DANGER,
                                    title: "Delete Error",
                                    message: data.ErrorMessage
                                });
                            }

                        },
                        error: function (data) {
                            $("body").css("cursor", "default");
                            $.unblockUI();
                            BootstrapDialog.show({
                                type: BootstrapDialog.TYPE_DANGER,
                                title: "Delete Error",
                                message: "Unexpected Error\n" + data
                            });
                        }
                    });
                };

                self.enableDetail = function () {
                    var selected = self.files()
                        .filter(function (data) { return data.isselected; });

                    if (selected.length > 0) {
                        if (selected.length < self.totalFiles()) {
                            cmdDelete.removeAttr("disabled");
                        }
                    } else
                        cmdDelete.attr("disabled", "disabled");
                };
            };

            //---------------------------------------------- VIEW MODEL (END) -----------------------------------------------------

            ko.utils.stringStartsWith = function (string, startsWith) {
                string = string || "";
                if (startsWith.length > string.length) return false;
                return string.substring(0, startsWith.length) === startsWith;
            };
        }
    }
})(jQuery);


