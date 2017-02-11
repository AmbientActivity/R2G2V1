/*!
 * 1.0 Keebee AAT Copyright © 2016
 * SystemLibraries/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

function CuteWebUI_AjaxUploader_OnPostback() {
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_INFO,
        title: "Saving Media",
        message: "One moment...",
        closable: false
    });
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
    $("#uploader-html-container").attr("hidden", "hidden");
    $("#lnkGoBack").removeAttr("hidden");
    $("#lblGoBackDisabled").attr("hidden", "hidden");
    $("#txtSearchFilename").removeAttr("disabled");
    $("#uploadbutton").removeAttr("disabled");
    $("select").removeAttr("disabled");
    $("#main-menu").removeAttr("hidden");
    $("#menu-login").removeAttr("hidden");
}

function DisableScreen() {
    $("#uploader-html-container").removeAttr("hidden");
    $("#lnkGoBack").attr("hidden", "hidden");
    $("#lblGoBackDisabled").removeAttr("hidden");
    $("#txtSearchFilename").attr("disabled", "disabled");
    $("#uploadbutton").attr("disabled", "disabled");
    $("select").attr("disabled", "disabled");
    $("#main-menu").attr("hidden", "hidden");
    $("#menu-login").attr("hidden", "hidden");
}

; (function ($) {
    var HIGHLIGHT_ROW_COLOUR = "#e3e8ff";

    systemlibraries.index = {
        init: function (values) {

            var config = {
                selectedMediaPathTypeId: 0
            };

            $.extend(config, values);

            // buttons
            var cmdDelete = $("#delete");

            var _sortDescending = false;
            var _currentSortKey = "filename";

            var lists = {
                FileList: [],
                MediaPathTypeList: []
            };

            loadData();

            ko.applyBindings(new FileViewModel());

            function loadData() {
                var mediaPathTypeId = $("#mediaPathTypeId").val();

                $.ajax({
                    type: "GET",
                    url: site.url + "SystemLibraries/GetData?" + "mediaPathTypeId=" + mediaPathTypeId,
                    dataType: "json",
                    traditional: true,
                    async: false,
                    success: function (data) {
                        $.extend(lists, data);
                    }
                });
            }

            function FileViewModel() {
                var tblFile = $("#tblFile");

                var self = this;

                self.files = ko.observableArray([]);
                self.mediaPathTypes = ko.observableArray([]);
                self.selectedMediaPathType = ko.observable(config.selectedMediaPathTypeId);
                self.filenameSearch = ko.observable("");
                self.totalFilteredFiles = ko.observable(0);
                self.selectAllIsSelected = ko.observable(false);
                self.selectedStreamIds = ko.observable([]);
                self.isPreviewable = ko.observable(false);
                self.isSharable = ko.observable(false);

                createFileArray(lists.FileList);
                createMediaPathTypeArray(lists.MediaPathTypeList);
                enableDetail();

                function createFileArray(list) {
                    self.files.removeAll();
                    $(list).each(function (index, value) {
                        self.files.push({

                            streamid: value.StreamId,
                            filename: value.Filename,
                            filetype: value.FileType,
                            filesize: value.FileSize,
                            path: value.Path,
                            mediapathtypeid: value.MediaPathTypeId,
                            numlinked: value.NumLinkedProfiles,
                            isselected: false
                        });
                    });
                };

                function createMediaPathTypeArray(list) {
                    self.mediaPathTypes.removeAll();
                    $(list).each(function (index, value) {
                        self.mediaPathTypes.push(
                        {
                            id: value.Id,
                            description: value.Description,
                            shortdescription: value.ShortDescription,
                            ispreviewable: value.IsPreviewable,
                            islinkable: value.IsLinkable
                        });
                    });

                    var mediaPathType = self.mediaPathTypes().filter(function (value) {
                        return value.id === self.selectedMediaPathType();
                    })[0];

                    self.isPreviewable(mediaPathType.ispreviewable);
                    self.isSharable(mediaPathType.islinkable);
                };

                function enableDetail() {
                    var selected = self.files()
                        .filter(function (data) { return data.isselected; });

                    cmdDelete.attr("disabled", "disabled");
                    if (selected.length > 0) {
                        if (selected.length < self.filteredFilesBySelection().length) {
                            cmdDelete.removeAttr("disabled");
                        }
                    }
                };

                self.columns = ko.computed(function () {
                    var arr = [];
                    arr.push({ title: "Name", sortable: true, sortKey: "filename", numeric: false, cssClass: "" });
                    arr.push({ title: "Type", sortable: true, sortKey: "filetype", numeric: false, cssClass: "col-filetype" });
                    arr.push({ title: "Size", sortable: true, sortKey: "filesize", numeric: true, cssClass: "col-filesize" });
                    return arr;
                });

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
                                if (value.numeric) {
                                    if (_sortDescending) {
                                        return a[sortKey] > b[sortKey]
                                              ? -1 : a[sortKey] < b[sortKey] || a.filename > b.filename ? 1 : 0;
                                    } else {
                                        return a[sortKey] < b[sortKey]
                                            ? -1 : a[sortKey] > b[sortKey] || a.filename > b.filename ? 1 : 0;
                                    }
                                } else {
                                    if (_sortDescending) {
                                        return a[sortKey].toString().toLowerCase() > b[sortKey].toString().toLowerCase()
                                            ? -1 : a[sortKey].toString().toLowerCase() < b[sortKey].toString().toLowerCase()
                                            || a.filename.toLowerCase() > b.filename.toLowerCase() ? 1 : 0;
                                    } else {
                                        return a[sortKey].toString().toLowerCase() < b[sortKey].toString().toLowerCase()
                                            ? -1 : a[sortKey].toString().toLowerCase() > b[sortKey].toString().toLowerCase()
                                            || a.filename.toLowerCase() > b.filename.toLowerCase() ? 1 : 0;
                                    }
                                }
                            });
                        }
                    });
                };

                self.reloadUploaderHtml = function () {
                    var mediaPathTypeId = self.selectedMediaPathType();

                    $.ajax({
                        type: "GET",
                        url: site.url + "SystemLibraries/GetUploaderHtml?mediaPathTypeId=" + mediaPathTypeId,
                        traditional: true,
                        async: true,
                        dataType: "json",
                        success: function (data) {
                            $("#uploader-html-container").html(data.UploaderHtml);
                            $("#uploadbutton").text(data.AddButtonText);
                        }
                    });
                };

                self.selectedMediaPathType.subscribe(function (id) {
                    if (typeof id === "undefined") return;
                    $("#mediaPathTypeId").val(id);
                    self.reloadUploaderHtml();
                    self.checkSelectAll(false);
                    self.selectAllRows();

                    var mediaPathType = self.mediaPathTypes().filter(function (value) {
                        return value.id === self.selectedMediaPathType();
                    })[0];

                    self.isPreviewable(mediaPathType.ispreviewable);
                    self.isSharable(mediaPathType.islinkable);
                });

                self.filteredFiles = ko.computed(function () {
                    return ko.utils.arrayFilter(self.files(), function (f) {
                        return (
                            self.filenameSearch().length === 0 ||
                            f.filename.toLowerCase().indexOf(self.filenameSearch().toLowerCase()) !== -1) &&
                            f.mediapathtypeid === self.selectedMediaPathType();
                    });
                });

                self.filteredFilesBySelection = ko.computed(function () {
                    return ko.utils.arrayFilter(self.files(), function (f) {
                        return (f.mediapathtypeid === self.selectedMediaPathType());
                    });
                });

                self.filesTable = ko.computed(function () {
                    var filteredFiles = self.filteredFiles();
                    self.totalFilteredFiles(filteredFiles.length);

                    return filteredFiles;
                });

                self.checkAllReset = ko.computed(function () {
                    $("#chk_all").prop("checked", false);

                    self.selectedStreamIds([]);
                    $.each(self.filteredFiles(), function (item, value) {
                        value.isselected = false;

                        var chk = tblFile.find("#chk_" + value.id);
                        chk.prop("checked", false);
                    });
                });

                // ------------------

                self.showDeleteSelectedDialog = function () {
                    self.showSelectedFileDeleteDialog();
                };

                self.showPreview = function (row) {
                    self.showImagePreview(row);
                };

                self.showLinkedProfilesDialog = function (row) {
                    self.showProfilesLinkedDialog(row);
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
                                }
                            }
                        ]
                    });
                };

                self.showProfilesLinkedDialog = function (row) {
                    var message;
                    var title = "<span class='glyphicon glyphicon-link' style='color: #fff'></span>";
                    var mediaPathTypeDesc = self.mediaPathType().shortdescription;

                    $.ajax({
                        type: "GET",
                        async: false,
                        data: { streamId: row.streamid },
                        url: site.url + "SystemLibraries/GetLinkedResidentsView/",
                        success: function (data) {
                            message = data;
                        }
                    });

                    BootstrapDialog.show({
                        title: title + " Profiles linked to the <b>" + mediaPathTypeDesc + "</b> file: \n" +
                            "<b>" + row.filename + "</b>",
                        message: $("<div></div>").append(message),

                        closable: false,
                        buttons: [
                            {
                                label: "OK",
                                cssClass: "btn-primary",
                                action: function (dialog) {
                                    dialog.close();
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

                self.selectAllRows = function () {
                    self.selectedStreamIds([]);

                    $.each(self.filteredFiles(), function (item, value) {
                        if (self.selectAllIsSelected())
                            self.selectedStreamIds().push(value.id);
                        else
                            self.selectedStreamIds().pop(value.id);

                        value.isselected = self.selectAllIsSelected();
                        var chk = tblFile.find("#chk_" + value.id);
                        chk.prop("checked", self.selectAllIsSelected());
                    });

                    self.highlightSelectedRows();
                    enableDetail();

                    return true;
                };

                self.selectFile = function (row) {
                    if (typeof row === "undefined") return false;
                    if (row === null) return false;

                    if (row.isselected)
                        self.selectedStreamIds().push(row.streamid);
                    else
                        self.removeSelectedStreamId(row.streamid);

                    self.highlightSelectedRows();
                    self.checkSelectAll(self.selectedStreamIds().length === self.filteredFiles().length);
                    enableDetail();

                    return true;
                };

                self.removeSelectedStreamId = function (id) {
                    for (var i = self.selectedStreamIds().length - 1; i >= 0; i--) {
                        if (self.selectedStreamIds()[i] === id) {
                            self.selectedStreamIds().splice(i, 1);
                        }
                    }
                }

                self.highlightSelectedRows = function () {
                    var rows = tblFile.find("tr:gt(0)");
                    rows.each(function () {
                        $(this).css("background-color", "#ffffff");
                    });

                    var selected = self.files()
                        .filter(function (data) { return data.isselected; });

                    $.each(selected, function (item, value) {
                        var r = tblFile.find("#row_" + value.id);
                        r.css("background-color", HIGHLIGHT_ROW_COLOUR);
                        tblFile.attr("tr:hover", HIGHLIGHT_ROW_COLOUR);
                    });

                    return true;
                };

                self.deleteSelected = function () {
                    $("body").css("cursor", "wait");

                    var streamIds = self.selectedStreamIds();
                    var mediaPathTypeId = $("#mediaPathTypeId").val();

                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_INFO,
                        title: "Delete Files",
                        message: "One moment...",
                        closable: false,
                        onshown: function (dialog) {

                            $.ajax({
                                type: "POST",
                                async: true,
                                traditional: true,
                                url: site.url + "SystemLibraries/DeleteSelected/",
                                data:
                                {
                                    streamIds: streamIds,
                                    mediaPathTypeId: mediaPathTypeId,
                                    isSharable: self.isSharable()
                                },
                                dataType: "json",
                                success: function (data) {
                                    dialog.close();
                                    $("body").css("cursor", "default");
                                    if (data.Success) {
                                        lists.FileList = data.FileList;
                                        createFileArray(lists.FileList);
                                        self.sort({ afterSave: true });
                                        self.selectedStreamIds([]);
                                        self.checkSelectAll(false);
                                        enableDetail();
                                    } else {
                                        $("body").css("cursor", "default");
                                        enableDetail();

                                        BootstrapDialog.show({
                                            type: BootstrapDialog.TYPE_DANGER,
                                            title: "Delete Error",
                                            message: data.ErrorMessage
                                        });
                                    }
                                },
                                error: function (data) {
                                    dialog.close();
                                    $("body").css("cursor", "default");
                                    enableDetail();

                                    BootstrapDialog.show({
                                        type: BootstrapDialog.TYPE_DANGER,
                                        title: "Delete Error",
                                        message: "Unexpected Error\n" + data
                                    });
                                }
                            });
                        }
                    });
                };

                self.showImagePreview = function (row) {
                    $("body").css("cursor", "wait");
                    var message;

                    $.ajax({
                        type: "GET",
                        async: false,
                        url: site.url + "SystemLibraries/GetImageViewerView?streamId=" + row.streamid + "&fileType=" + row.filetype,
                        success: function (data) {
                            message = data;
                        },
                        error: function () {
                            message = "An error occured";
                        }
                    });

                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_INFO,
                        title: "Image Viewer - " + row.filename + "." + row.filetype.toLowerCase(),
                        message: $("<div></div>").append(message),
                        closable: false,
                        onshown: function () { $("body").css("cursor", "default"); },
                        buttons: [{
                            label: "Close",
                            action: function (dialog) {
                                dialog.close();
                            }
                        }]
                    });
                };

                self.checkSelectAll = function (checked) {
                    self.selectAllIsSelected(checked);
                    $("#chk_all").prop("checked", checked);
                };

                self.mediaPathType = function () {
                    return self.mediaPathTypes()
                        .filter(function (value) {
                            return value.id === self.selectedMediaPathType();
                        })[0];
                }
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


