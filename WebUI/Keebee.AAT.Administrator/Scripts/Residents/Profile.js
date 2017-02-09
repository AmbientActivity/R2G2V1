/*!
 * Residents/Profile.js
 * Author: John Charlton
 * Date: 2016-08
 */

function CuteWebUI_AjaxUploader_OnPostback() {
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_INFO,
        title: "Saving Media",
        message: "Ome moment...",
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
    $("#delete").attr("hidden", "hidden");
    $("select").attr("disabled", "disabled");
    $("#main-menu").attr("hidden", "hidden");
    $("#menu-login").attr("hidden", "hidden");
}

; (function ($) {
    var HIGHLIGHT_ROW_COLOUR = "#e3e8ff";

    residents.profile = {
        init: function (values) {
            var config = {
                residentid: 0,
                selectedMediaPathTypeId: 0
            };

            $.extend(config, values);

            // buttons
            var cmdDelete = $("#delete");
            var cmdAddShared = $("#add-shared");

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
                    url: site.url + "Residents/GetDataProfile/" + config.residentid
                        + "?mediaPathTypeId=" + mediaPathTypeId,
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
                self.totalFiles = ko.observable(0);
                self.selectAllIsSelected = ko.observable(false);
                self.selectedIds = ko.observable([]);
                self.isPreviewable = ko.observable(false);
                self.isSharable = ko.observable(false);

                createFileArray(lists.FileList);
                createMediaPathTypeArray(lists.MediaPathTypeList);
                enableDetail();

                function createFileArray(list) {
                    self.files.removeAll();
                    $(list).each(function (index, value) {
                        self.files.push({
                            id: value.Id,
                            streamid: value.StreamId,
                            filename: value.Filename,
                            filetype: value.FileType,
                            filesize: value.FileSize,
                            isshared: value.IsShared,
                            path: value.Path,
                            mediapathtypeid: value.MediaPathTypeId,
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
                                issharable: value.IsSharable
                            });
                    });

                    var mediaType = self.mediaPathTypes().filter(function (value) {
                        return value.id === self.selectedMediaPathType();
                    })[0];

                    self.isPreviewable(mediaType.ispreviewable);
                    self.isSharable(mediaType.issharable);
                };

                function enableDetail() {
                    var selected = self.files()
                        .filter(function (data) { return data.isselected; });

                    if (selected.length > 0)
                        cmdDelete.removeAttr("disabled");
                    else
                        cmdDelete.attr("disabled", "disabled");

                    if (self.isSharable())
                        cmdAddShared.removeAttr("disabled");
                    else
                        cmdAddShared.attr("disabled", "disabled");
                };

                self.columns = ko.computed(function () {
                    var arr = [];
                    arr.push({ title: "Name", sortable: true, sortKey: "filename", numeric: false, cssClass: "" });
                    arr.push({ title: "Type", sortable: true, sortKey: "filetype", numeric: false, cssClass: "col-filetype" });
                    arr.push({ title: "Size", sortable: true, sortKey: "filesize", numeric: true, cssClass: "col-filesize" });
                    arr.push({ title: "Shared", sortable: true, sortKey: "isshared", numeric: true, cssClass: "col-isshared" });
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

                self.reloadUploaderHtml = function() {
                    var mediaPathTypeId = self.selectedMediaPathType();

                    $.ajax({
                        type: "GET",
                        url: site.url + "Residents/GetUploaderHtml?mediaPathTypeId=" + mediaPathTypeId,
                        traditional: true,
                        async: true,
                        dataType: "json",
                        success: function(data) {
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

                    self.isPreviewable(self.mediaPathType().ispreviewable);
                    self.isSharable(self.mediaPathType().issharable);

                    enableDetail();
                });

                self.filteredFiles = ko.computed(function () {
                    return ko.utils.arrayFilter(self.files(), function (f) {
                        return (
                            self.filenameSearch().length === 0 ||
                                f.filename.toLowerCase().indexOf(self.filenameSearch().toLowerCase()) !== -1)
                         && f.mediapathtypeid === self.selectedMediaPathType();;
                    });
                });

                self.filesTable = ko.computed(function () {
                    var filteredFiles = self.filteredFiles();
                    self.totalFiles(filteredFiles.length);

                    return filteredFiles;
                });

                // ------------------

                self.showAddFromSharedLibarayDialog = function () {
                    self.showSharedLibrayAddDialog();
                };

                self.showDeleteSelectedDialog = function (row) {
                    self.showSelectedFileDeleteDialog(row);
                };

                self.showPreview = function (row) {
                    self.showImagePreview(row);
                };

                self.showSharedLibrayAddDialog = function () {
                    var message;
                    var residentId = config.residentid;
                    var title = "<span class='glyphicon glyphicon-cd' style='color: #fff'></span>";
                    var mediaPathTypeDesc = self.mediaPathType().shortdescription;

                    $.ajax({
                        type: "GET",
                        async: false,
                        data: 
                        {
                            residentId: residentId,
                            mediaPathTypeId: self.selectedMediaPathType()
                        },
                        url: site.url + "Residents/GetSharedLibarayAddView/",
                        success: function (data) {
                            message = data;
                        }
                    });

                    BootstrapDialog.show({
                        title: title + " Add <b>" + mediaPathTypeDesc + "</b> From Shared Library",
                        message: $("<div></div>").append(message),
                        
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
                                    self.addSharedFiles();
                                    dialog.close();
                                }
                            }
                        ]
                    });
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
                                    dialog.close();
                                    self.deleteSelected();
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
                    self.selectedIds([]);

                    $.each(self.filteredFiles(), function (item, value) {

                        if (self.selectAllIsSelected()) 
                            self.selectedIds().push(value.id);
                        else
                            self.selectedIds().pop(value.id);

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
                        self.selectedIds().push(row.id);
                    else
                        self.removeSelectedId(row.id);

                    self.highlightSelectedRows();
                    enableDetail();

                    self.checkSelectAll(self.selectedIds().length === self.filteredFiles().length);

                    return true;
                };

                self.removeSelectedId = function (id) {
                    for (var i = self.selectedIds().length - 1; i >= 0; i--) {
                        if (self.selectedIds()[i] === id) {
                            self.selectedIds().splice(i, 1);
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

                self.deleteSelected = function() {
                    $("body").css("cursor", "wait");

                    var ids = self.selectedIds();
                    var residentId = config.residentid;
                    var mediaPathTypeId = $("#mediaPathTypeId").val();

                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_INFO,
                        title: "Delete Files",
                        message: "One moment...",
                        closable: false,
                        onshown: function(dialog) {

                            $.ajax({
                                type: "POST",
                                async: true,
                                traditional: true,
                                url: site.url + "Residents/DeleteSelectedMediaFiles/",
                                data:
                                {
                                    ids: ids,
                                    residentId: residentId,
                                    mediaPathTypeId: mediaPathTypeId
                                },
                                dataType: "json",
                                success: function(data) {
                                    dialog.close();
                                    $("body").css("cursor", "default");
                                    if (data.Success) {
                                        lists.FileList = data.FileList;
                                        createFileArray(lists.FileList);
                                        self.sort({ afterSave: true });
                                        self.selectedIds([]);
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
                                error: function(data) {
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

                self.addSharedFiles = function () {
                    $("body").css("cursor", "wait");

                    var ids = [];
                    $("input[name='shared_files']:checked").each(function(item, value) {
                        ids.push(value.id);
                    });
                    var residentId = config.residentid;
                    var mediaPathTypeId = $("#mediaPathTypeId").val();

                    $.ajax({
                        type: "POST",
                        async: true,
                        traditional: true,
                        url: site.url + "Residents/AddSharedMediaFiles/",
                        data:
                        {
                            streamIds: ids,
                            residentId: residentId,
                            mediaPathTypeId: mediaPathTypeId
                        },
                        dataType: "json",
                        success: function(data) {
                            $("body").css("cursor", "default");
                            if (data.Success) {
                                lists.FileList = data.FileList;
                                createFileArray(lists.FileList);
                                self.sort({ afterSave: true });
                                self.selectedIds([]);
                                self.checkSelectAll(false);
                                enableDetail();
                            } else {
                                $("body").css("cursor", "default");
                                enableDetail();

                                BootstrapDialog.show({
                                    type: BootstrapDialog.TYPE_DANGER,
                                    title: "Error Adding Shared Files",
                                    message: data.ErrorMessage
                                });
                            }
                        },
                        error: function(data) {
                            $("body").css("cursor", "default");
                            enableDetail();

                            BootstrapDialog.show({
                                type: BootstrapDialog.TYPE_DANGER,
                                title: "Error Adding Shared Files",
                                message: "Unexpected Error\n" + data
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
                        url: site.url + "Residents/GetImageViewerView?streamId=" + row.streamid + "&fileType=" + row.filetype,
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
                        onshown: function() { $("body").css("cursor", "default"); },
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

                self.mediaPathType = function() {
                    return self.mediaPathTypes()
                        .filter(function(value) {
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
