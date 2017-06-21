/*!
 * ResidentProfile/Index.js
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
    residentprofile.index = {
        init: function (values) {
            var config = {
                residentid: 0,
                selectedMediaPathTypeId: 0
            };

            $.extend(config, values);

            // buttons
            var cmdDelete = $("#delete");
            var cmdAddShared = $("#add-shared");

            var sortDescending = false;
            var currentSortKey = "filename";
            
            var lists = {
                FileList: [],
                MediaPathTypeList: []
            };

            $.get(site.url + "ResidentProfile/GetData/" + config.residentid
                    + "?mediaPathTypeId=" + $("#mediaPathTypeId").val())
                .done(function (data) {
                    $.extend(lists, data);

                    ko.applyBindings(new FileViewModel());

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
                                    islinked: value.IsLinked,
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
                                .filter(function (value) { return value.isselected; });

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
                            arr.push({ title: "Linked", sortable: true, sortKey: "islinked", numeric: true, cssClass: "col-islinked" });
                            return arr;
                        });

                        self.sort = function (header) {
                            var afterSave = typeof header.afterSave != "undefined" ? header.afterSave : false;
                            var sortKey;

                            if (!afterSave) {
                                sortKey = header.sortKey;

                                if (sortKey !== currentSortKey) {
                                    sortDescending = false;
                                } else {
                                    sortDescending = !sortDescending;
                                }
                                currentSortKey = sortKey;
                            } else {
                                sortKey = currentSortKey;
                            }

                            $(self.columns()).each(function (index, value) {
                                if (value.sortKey === sortKey) {
                                    self.files.sort(function (a, b) {
                                        if (value.numeric) {
                                            if (sortDescending) {
                                                return a[sortKey] > b[sortKey]
                                                        ? -1 : a[sortKey] < b[sortKey] || a.filename > b.filename ? 1 : 0;
                                            } else {
                                                return a[sortKey] < b[sortKey]
                                                    ? -1 : a[sortKey] > b[sortKey] || a.filename > b.filename ? 1 : 0;
                                            }
                                        } else {
                                            if (sortDescending) {
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

                            $.get(site.url + "ResidentProfile/GetUploaderHtml?mediaPathTypeId=" + mediaPathTypeId)
                                .done(function (result) {
                                    $("#uploader-html-container").html(result.UploaderHtml);
                                    $("#uploadbutton").text(result.AddButtonText);
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

                        self.showLinkFromSharedLibarayDialog = function () {
                            self.showSharedLibrayLinkDialog();
                        };

                        self.showDeleteSelectedDialog = function (row) {
                            self.showSelectedFileDeleteDialog(row);
                        };

                        self.showPreview = function (row) {
                            $.get(site.url + "ResidentProfile/GetImageViewerView?streamId=" + row.streamid + "&fileType=" + row.filetype)
                                .done(function (message) {
                                    BootstrapDialog.show({
                                        type: BootstrapDialog.TYPE_INFO,
                                        title: "Image Viewer - " + row.filename + "." + row.filetype.toLowerCase(),
                                        message: $("<div></div>").append(message),
                                        closable: false,
                                        buttons: [{
                                            label: "Close",
                                            action: function (dialog) {
                                                dialog.close();
                                            }
                                        }]
                                    });
                                })
                                .error(function (message) {
                                    BootstrapDialog.show({
                                        type: BootstrapDialog.TYPE_DANGER,
                                        title: "Error",
                                        message: $("<div></div>").append(message),
                                        closable: false,
                                        buttons: [{
                                            label: "Close",
                                            action: function (dialog) {
                                                dialog.close();
                                            }
                                        }]
                                    });
                            });
                        };

                        self.showSharedLibrayLinkDialog = function () {
                            var residentId = config.residentid;
                            var title = "<span class='glyphicon glyphicon-link' style='color: #fff'></span>";
                            var mediaPathTypeDesc = self.mediaPathType().shortdescription;

                            $.get(site.url + "ResidentProfile/GetSharedLibarayLinkView/",
                                { residentId: residentId, mediaPathTypeId: self.selectedMediaPathType() })
                                .done(function (message) {
                                    var hasHave = "has";
                                    if (mediaPathTypeDesc.endsWith("s"))
                                        hasHave = "have";

                                    if (message.length === 0) {
                                        BootstrapDialog.show({
                                            title: title + " Add <b>" + mediaPathTypeDesc + "</b> From Shared Library",
                                            message: $("<div></div>").append("All available " + mediaPathTypeDesc + " " + hasHave + " already been added to this profile."),

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
                                    } else {
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
                                    }
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
                                $(this).removeClass("highlight");
                            });

                            var selected = self.files()
                                .filter(function (value) { return value.isselected; });

                            $.each(selected, function (item, value) {
                                var r = tblFile.find("#row_" + value.id);
                                $(r).addClass("highlight");
                            });
                        };

                        self.deleteSelected = function () {
                            var ids = self.selectedIds();
                            var residentId = config.residentid;
                            var mediaPathTypeId = $("#mediaPathTypeId").val();

                            BootstrapDialog.show({
                                type: BootstrapDialog.TYPE_INFO,
                                title: "Delete Files",
                                message: "One moment...",
                                closable: false,
                                onshown: function (dialog) {
                                    $.post(site.url + "ResidentProfile/DeleteSelected/",
                                        {
                                            ids: ids,
                                            residentId: residentId,
                                            mediaPathTypeId: mediaPathTypeId
                                        })
                                        .done(function (result) {
                                            dialog.close();
                                            if (result.Success) {
                                                lists.FileList = result.FileList;
                                                createFileArray(lists.FileList);
                                                self.sort({ afterSave: true });
                                                self.selectedIds([]);
                                                self.checkSelectAll(false);
                                                enableDetail();
                                            } else {
                                                enableDetail();

                                                BootstrapDialog.show({
                                                    type: BootstrapDialog.TYPE_DANGER,
                                                    title: "Delete Error",
                                                    message: result.ErrorMessage
                                                });
                                            }
                                        })
                                        .error(function (result) {
                                            dialog.close();
                                            enableDetail();

                                            BootstrapDialog.show({
                                                type: BootstrapDialog.TYPE_DANGER,
                                                title: "Delete Error",
                                                message: "Unexpected Error\n" + result
                                            });
                                        });
                                }
                            });
                        };

                        self.addSharedFiles = function () {
                            var ids = [];
                            $("input[name='shared_files']:checked").each(function (item, value) {
                                ids.push(value.id);
                            });
                            var residentId = config.residentid;
                            var mediaPathTypeId = $("#mediaPathTypeId").val();

                            $.post(site.url + "ResidentProfile/AddFromSharedLibrary/", 
                                {
                                    streamIds: ids,
                                    residentId: residentId,
                                    mediaPathTypeId: mediaPathTypeId
                                })
                            .done(function (result) {
                                    if (result.Success) {
                                        lists.FileList = result.FileList;
                                        createFileArray(lists.FileList);
                                        self.sort({ afterSave: true });
                                        self.selectedIds([]);
                                        self.checkSelectAll(false);
                                        enableDetail();
                                    } else {
                                        enableDetail();

                                        BootstrapDialog.show({
                                            type: BootstrapDialog.TYPE_DANGER,
                                            title: "Error Adding Shared Files",
                                            message: result.ErrorMessage
                                        });
                                    }
                                })
                                .error(function (result) {
                                    enableDetail();

                                    BootstrapDialog.show({
                                        type: BootstrapDialog.TYPE_DANGER,
                                        title: "Error Adding Shared Files",
                                        message: "Unexpected Error\n" + result
                                    });
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
            });
        }        
    }
})(jQuery);
