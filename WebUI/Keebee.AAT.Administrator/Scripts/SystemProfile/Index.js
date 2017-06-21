/*!
 * 1.0 Keebee AAT Copyright © 2016
 * SystemProfile/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

; (function ($) {
    systemprofile.index = {
        init: function () {

            // buttons
            var cmdDelete = $("#delete");

            var sortDescending = false;
            var currentSortKey = "filename";

            var lists = {
                FileList: [],
                MediaPathTypeList: []
            };

            $.get(site.url + "SystemProfile/GetData")
                .done(function (data) {
                    $.extend(lists, data);

                    ko.applyBindings(new FileViewModel());

                    function FileViewModel() {
                        var tblFile = $("#tblFile");

                        var self = this;

                        self.files = ko.observableArray([]);
                        self.mediaPathTypes = ko.observableArray([]);
                        self.selectedMediaPathType = ko.observable();
                        self.filenameSearch = ko.observable("");
                        self.totalFilteredFiles = ko.observable(0);
                        self.selectAllIsSelected = ko.observable(false);
                        self.selectedIds = ko.observable([]);
                        self.isPreviewable = ko.observable(false);

                        createFileArray(lists.FileList);
                        createMediaPathTypeArray(lists.MediaPathTypeList);

                        function createFileArray(list) {
                            self.files.removeAll();
                            $(list).each(function (index, value) {
                                self.files.push({
                                    id: value.Id,
                                    streamid: value.StreamId,
                                    filename: value.Filename,
                                    filetype: value.FileType,
                                    filesize: value.FileSize,
                                    path: value.Path,
                                    islinked: value.IsLinked,
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
                                    ispreviewable: value.IsPreviewable
                                });
                            });
                            self.selectedMediaPathType(self.mediaPathTypes()[0].id);
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

                        self.selectedMediaPathType.subscribe(function (id) {
                            if (typeof id === "undefined") return;

                            self.checkSelectAll(false);
                            self.selectAllRows();

                            var ispreviewable = self.mediaPathTypes().filter(function (value) {
                                return value.id === self.selectedMediaPathType();
                            })[0].ispreviewable;

                            self.isPreviewable(ispreviewable);
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

                            self.selectedIds([]);
                            $.each(self.filteredFiles(), function (item, value) {
                                value.isselected = false;

                                var chk = tblFile.find("#chk_" + value.id);
                                chk.prop("checked", false);
                            });
                        });

                        self.showLinkFromSharedLibarayDialog = function () {
                            var title = "<span class='glyphicon glyphicon-link' style='color: #fff'></span>";
                            var mediaPathTypeDesc = self.mediaPathType().shortdescription;

                            $.get(site.url + "SystemProfile/GetSharedLibarayLinkView/", 
                                { mediaPathTypeId: self.selectedMediaPathType() })
                                .done(function (message) {
                                    if (message.length === 0) {
                                        var hasHave = "has";
                                        if (mediaPathTypeDesc.endsWith("s"))
                                            hasHave = "have";

                                        BootstrapDialog.show({
                                            title: title + " Add <b>" + mediaPathTypeDesc + "</b> From Shared Library",
                                            message: $("<div></div>").append("All available " + mediaPathTypeDesc + " " + hasHave + " already been added to the system profile."),

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

                        self.showDeleteSelectedDialog = function () {
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

                        self.showPreview = function (row) {
                            self.showImagePreview(row);
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
                            self.enableDetail();

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
                            self.checkSelectAll(self.selectedIds().length === self.filteredFiles().length);
                            self.enableDetail();

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

                            BootstrapDialog.show({
                                type: BootstrapDialog.TYPE_INFO,
                                title: "Delete Files",
                                message: "One moment...",
                                closable: false,
                                onshown: function (dialog) {
                                    $.post(site.url + "SystemProfile/DeleteSelected/",
                                        {
                                            ids: ids,
                                            mediaPathTypeId: self.selectedMediaPathType()
                                        })
                                        .done(function (result) {
                                            dialog.close();
                                            if (result.Success) {
                                                lists.FileList = result.FileList;
                                                createFileArray(lists.FileList);
                                                self.sort({ afterSave: true });
                                                self.selectedIds([]);
                                                self.checkSelectAll(false);
                                                self.enableDetail();
                                            } else {
                                                self.enableDetail();

                                                BootstrapDialog.show({
                                                    type: BootstrapDialog.TYPE_DANGER,
                                                    title: "Delete Error",
                                                    message: result.ErrorMessage
                                                });
                                            }
                                        })
                                        .error(function (result) {
                                            dialog.close();
                                            self.enableDetail();

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

                            $.post(site.url + "SystemProfile/AddSharedMediaFiles/",
                                {
                                    streamIds: ids,
                                    mediaPathTypeId: self.selectedMediaPathType()
                                })
                                .done(function (result) {
                                    if (result.Success) {
                                        lists.FileList = result.FileList;
                                        createFileArray(lists.FileList);
                                        self.sort({ afterSave: true });
                                        self.selectedIds([]);
                                        self.checkSelectAll(false);
                                        self.enableDetail();
                                    } else {
                                        self.enableDetail();

                                        BootstrapDialog.show({
                                            type: BootstrapDialog.TYPE_DANGER,
                                            title: "Error Adding Shared Files",
                                            message: result.ErrorMessage
                                        });
                                    }
                                })
                                .error(function (result) {
                                    self.enableDetail();

                                    BootstrapDialog.show({
                                        type: BootstrapDialog.TYPE_DANGER,
                                        title: "Error Adding Shared Files",
                                        message: "Unexpected Error\n" + data
                                    });
                                });
                        };

                        self.showImagePreview = function (row) {
                            $.get(site.url + "SystemProfile/GetImageViewerView?streamId=" + row.streamid + "&fileType=" + row.filetype)
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

                        self.enableDetail = function () {
                            var selected = self.files()
                                .filter(function (value) { return value.isselected; });

                            cmdDelete.attr("disabled", "disabled");
                            if (selected.length > 0) {
                                if (selected.length < self.filteredFilesBySelection().length) {
                                    cmdDelete.removeAttr("disabled");
                                }
                            }
                        };
                    };
            });
        }
    }
})(jQuery);


