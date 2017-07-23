/*!
 * 1.0 Keebee AAT Copyright © 2016
 * SystemProfile/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

; (function ($) {
    systemprofile.index = {
        init: function (options) {
            var isBinding = true;

            var config = {
                selectedMediaPathTypeId: 0
            };

            $.extend(config, options);

            // buttons
            var cmdDelete = $("#delete");

            // video player
            var videoPlayer = $("#video-player");

            var sortDescending = false;
            var currentSortKey = "filename";
            var primarySortKey = "filename";

            var lists = {
                FileList: [],
                MediaPathTypeList: []
            };

            utilities.job.execute({
                url: site.url + "SystemProfile/GetData"
            })
            .then(function (data) {
                $.extend(lists, data);

                $("#error-container").hide();
                $("#loading-container").hide();
                $("#table-header").show();
                $("#table-detail").show();
                $("#add").prop("disabled", false);

                ko.bindingHandlers.tableUpdated = {
                    update: function (element, valueAccessor, allBindings) {
                        ko.unwrap(valueAccessor());
                        $("#txtSearchFilename").focus();
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
                                var tooltipElement = $("#thumb_" + id);
                                if (tooltipElement.length > 0)
                                    tooltipElement.tooltip({ delay: { show: 100, hide: 100 } });
                            }
                        }
                        var table = element.parentNode;
                        formatTable(table);
                    }
                }

                function formatTable(table) {
                    var noRowsMessage = $("#no-rows-message");

                    var description = lists.MediaPathTypeList.filter(function (value) {
                        return value.Id === config.selectedMediaPathTypeId;
                    })[0].ShortDescription;

                    var colThumbnail = $("#col-thumbnail");
                    colThumbnail.html("<div class='virtualPlaceholderImage'></div>");

                    var tableDetailElement = $("#table-detail");
                    var tableHeaderElement = $("#table-header");

                    if (table.rows.length > 0) {
                        tableHeaderElement.show();
                        tableDetailElement.show();
                        noRowsMessage.hide();

                        // determine if there is table overflow (to cause a scrollbar)
                        // if so, unhide the scrollbar header column
                        // and adjust the width of the filename column
                        var colScrollbar = $("#col-scrollbar");

                        if (table.clientHeight > site.getMaxClientHeight) {
                            colScrollbar.prop("hidden", false);
                            colScrollbar.attr("style", "width: 1%; border-bottom: 1.5px solid #ddd;");
                            tableDetailElement.addClass("container-height");
                        } else {
                            colScrollbar.prop("hidden", true);
                            tableDetailElement.removeClass("container-height");
                        }

                    } else {
                        tableHeaderElement.hide();
                        tableDetailElement.hide();
                        noRowsMessage.html("<h2>No " + description.toLowerCase() + " found</h2>");
                        noRowsMessage.show();
                    }
                }

                ko.applyBindings(new FileViewModel());

                function FileViewModel() {
                    var tblFile = $("#tblFile");
                    var self = this;

                    self.files = ko.observableArray([]);
                    self.mediaPathTypes = ko.observableArray([]);
                    self.filenameSearch = ko.observable("");
                    self.totalFilteredFiles = ko.observable(0);
                    self.selectAllIsSelected = ko.observable(false);
                    self.selectedIds = ko.observable([]);

                    // for audio previewing
                    self.currentStreamId = ko.observable(0);
                    self.currentRowId = ko.observable(0);

                    createFileArray({ list: lists.FileList, insert: false });
                    createMediaPathTypeArray(lists.MediaPathTypeList);

                    // media path type
                    self.selectedMediaPathTypeId = ko.observable(config.selectedMediaPathTypeId);
                    self.selectedMediaPathType = ko.observable(self.mediaPathTypes().filter(function (value) {
                        return value.id === config.selectedMediaPathTypeId;
                    })[0]);

                    $("#delete").tooltip({ delay: { show: 100, hide: 100 } });

                    function createFileArray(params) {
                        var cfg = {
                            list: [],
                            insert: false
                        };

                        $.extend(cfg, params);

                        if (cfg.insert === false) {
                            self.files.removeAll();
                            $(cfg.list)
                                .each(function (index, value) {
                                    // do a "push"
                                    self.files.push({
                                        id: value.Id,
                                        streamid: value.StreamId,
                                        filename: value.Filename,
                                        filetype: value.FileType,
                                        filesize: value.FileSize,
                                        islinked: value.IsLinked,
                                        dateadded: value.DateAdded,
                                        path: value.Path,
                                        thumbnail: value.Thumbnail,
                                        mediapathtypeid: value.MediaPathTypeId,
                                        isselected: false,
                                        isplaying: false,
                                        ispaused: false
                                    });
                                });
                        } else {
                            $(cfg.list)
                                .each(function (index, value) {
                                    // do an "unshift"
                                    self.files.unshift({
                                        id: value.Id,
                                        streamid: value.StreamId,
                                        filename: value.Filename,
                                        filetype: value.FileType,
                                        filesize: value.FileSize,
                                        islinked: value.IsLinked,
                                        dateadded: value.DateAdded,
                                        path: value.Path,
                                        thumbnail: value.Thumbnail,
                                        mediapathtypeid: value.MediaPathTypeId,
                                        isselected: false,
                                        isplaying: false,
                                        ispaused: false
                                    });
                                });
                        }
                    };

                    function createMediaPathTypeArray(list) {
                        self.mediaPathTypes.removeAll();
                        $(list).each(function (index, value) {
                            self.mediaPathTypes.push(
                            {
                                id: value.Id,
                                responsetypeid: value.ResponseTypeId,
                                category: value.Category,
                                description: value.Description,
                                shortdescription: value.ShortDescription
                            });
                        });
                    };

                    self.columns = ko.computed(function () {
                        var arr = [];
                        arr.push({ title: "Filename", sortKey: "filename", cssClass: "col-filename-sp", visible: true });
                        arr.push({ title: "Type", sortKey: "filetype", cssClass: "col-filetype-sp", visible: true });
                        arr.push({ title: "Size", sortKey: "filesize", cssClass: "col-filesize-sp", visible: true });
                        arr.push({ title: "Added", sortKey: "dateadded", cssClass: "col-date", visible: false });
                        return arr;
                    });

                    self.sort = function (header) {
                        if (isBinding) return;

                        var sortKey = header.sortKey;

                        if (sortKey !== currentSortKey) {
                            sortDescending = false;
                        } else {
                            sortDescending = !sortDescending;
                        }

                        currentSortKey = sortKey;

                        var isboolean = false;
                        if (typeof header.boolean !== "undefined") {
                            isboolean = header.boolean;
                        }
                        self.files(utilities.sorting.sortArray({
                            array: self.files(),
                            columns: self.columns(),
                            sortKey: sortKey,
                            primarySortKey: primarySortKey,
                            descending: sortDescending,
                            boolean: isboolean
                        }));
                    };

                    self.selectedMediaPathTypeId.subscribe(function (id) {
                        if (typeof id === "undefined") return;

                        self.checkSelectAll(false);
                        self.selectAllRows();

                        self.selectedMediaPathType(self.mediaPathTypes().filter(function (value) {
                            return value.id === id;
                        })[0]);

                        self.displayNoRowsMessage();
                        self.clearStreams();
                    });

                    self.displayNoRowsMessage = function () {
                        // if no rows in table, display 'no rows found' message
                        // ko.bindingHandlers do not fire if no rows exist (after changing the media path type)
                        var noRowsMessage = $("#no-rows-message");
                        var tableDetailElement = $("#table-detail");
                        var tableHeaderElement = $("#table-header");

                        if (self.filteredFiles().length === 0) {
                            tableHeaderElement.hide();
                            tableDetailElement.hide();
                            noRowsMessage.html("<h2>No " +
                                self.selectedMediaPathType().shortdescription.toLowerCase() +
                                " found</h2>");
                            noRowsMessage.show();
                        } else {
                            tableHeaderElement.show();
                            tableDetailElement.show();
                            noRowsMessage.hide();
                        }
                    };

                    self.filteredFiles = ko.computed(function () {
                        return ko.utils.arrayFilter(self.files(), function (f) {
                            return (
                                self.filenameSearch().length === 0 ||
                                f.filename.toLowerCase().indexOf(self.filenameSearch().toLowerCase()) !== -1) &&
                                f.mediapathtypeid === self.selectedMediaPathType().id;
                        });
                    });

                    self.filteredFilesBySelection = ko.computed(function () {
                        return ko.utils.arrayFilter(self.files(), function (f) {
                            return (f.mediapathtypeid === self.selectedMediaPathType().id);
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

                    self.addFromSharedLibray = function () {
                        self.clearStreams();
                        var mediaPathTypeDesc = self.selectedMediaPathType().shortdescription;

                        sharedlibraryadd.view.show({
                            profileId: 0,
                            mediaPathTypeId: self.selectedMediaPathType().id,
                            mediaPathTypeDesc: mediaPathTypeDesc,
                            mediaPathTypeCategory: self.selectedMediaPathType().category
                        })
                        .then(function (streamIds) {
                            utilities.job.execute({
                                url: site.url + "SystemProfile/AddSharedMediaFiles",
                                type: "POST",
                                waitMessage: "Adding...",
                                params: {
                                    streamIds: streamIds,
                                    mediaPathTypeId: self.selectedMediaPathType().id,
                                    responseTypeId: self.selectedMediaPathType().responsetypeid
                                }
                            })
                            .then(function (addResult) {
                                createFileArray({ list: addResult.FileList, insert: true });
                                self.selectedIds([]);
                                self.checkSelectAll(false);
                                self.marqueeRows(addResult.FileList);
                                self.enableDetail();
                            })
                            .catch(function() {
                                self.enableDetail();
                            });
                        })
                        .catch(function () {
                            self.enableDetail();
                        });
                    };

                    self.marqueeRows = function (fileList) {
                        $(fileList).each(function (index, value) {
                            var id = value.Id;
                            $("#row_" + id).addClass("row-added");
                            setTimeout(function () {
                                $("#row_" + id).removeClass("row-added");
                            }, 1500);
                        });
                    };

                    self.deleteSelected = function () {
                        self.clearStreams();

                        utilities.confirm.show({
                            title: "Delete Files?",
                            message: "Delete all selected files?",
                            type: BootstrapDialog.TYPE_DANGER,
                            buttonOK: "Yes, Delete",
                            buttonOKClass: "btn-danger"
                        }).then(function (confirm) {
                            if (confirm) {
                                utilities.job.execute({
                                    url: site.url + "SystemProfile/DeleteSelected",
                                    type: "POST",
                                    waitMessage: "Deleting...",
                                    params: {
                                        ids: self.selectedIds(),
                                        mediaPathTypeId: self.selectedMediaPathType().id,
                                        responseTypeId: self.selectedMediaPathType().responsetypeid
                                    }
                                })
                                .then(function (deleteResult) {
                                    self.removeFiles(deleteResult.DeletedIds);
                                    self.selectedIds([]);
                                    self.checkSelectAll(false);
                                    self.enableDetail();
                                })
                                .catch(function () {
                                    self.enableDetail();
                                });
                            }
                        });
                    };

                    self.showPreview = function (row) {
                        var pathCategory = self.selectedMediaPathType().category;

                        if (pathCategory.includes("Video"))
                            self.previewVideo(row);
                    };

                    self.previewVideo = function (row) {
                        $("#thumb_" + row.id).tooltip("hide");

                        utilities.video.show({
                            src: site.getApiUrl + "videos/" + row.streamid,
                            player: videoPlayer,
                            filename: row.filename,
                            fileType: row.filetype.toLowerCase()
                        }).then(function () {
                            $("#modal-video").on("hidden.bs.modal", function () {
                                videoPlayer.attr("src", "");
                            });
                        });
                    };

                    self.clearStreams = function () {
                        videoPlayer.attr("src", "");
                        self.currentRowId(0);
                        self.currentStreamId(0);

                        var currentlyPlaying = self.filteredFiles()
                            .filter(function (value) {
                                return value.ispaused || value.isplaying;
                            })[0];

                        if (currentlyPlaying !== null && typeof currentlyPlaying !== "undefined") {
                            currentlyPlaying.ispaused = false;
                            currentlyPlaying.isplaying = false;
                            self.setGlyph(currentlyPlaying.id, false, false);
                        }
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
                            self.clearSelectedId(row.id);

                        self.highlightSelectedRows();
                        self.checkSelectAll(self.selectedIds().length === self.filteredFiles().length);
                        self.enableDetail();

                        return true;
                    };

                    self.clearSelectedId = function (id) {
                        for (var i = self.selectedIds().length - 1; i >= 0; i--) {
                            if (self.selectedIds()[i] === id) {
                                self.selectedIds().splice(i, 1);
                            }
                        }
                    }

                    self.highlightSelectedRows = function () {
                        var rows = tblFile.find("tr");
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

                    self.checkSelectAll = function (checked) {
                        self.selectAllIsSelected(checked);
                        $("#chk_all").prop("checked", checked);
                    };

                    self.enableDetail = function () {
                        var selected = self.files()
                            .filter(function (value) { return value.isselected; });

                        cmdDelete.prop("disabled", true);
                        if (selected.length > 0) {
                            if (selected.length < self.filteredFilesBySelection().length) {
                                cmdDelete.prop("disabled", false);
                            }
                        }
                    };

                    self.removeFiles = function (ids) {
                        $(ids).each(function (index, value) {
                            var idx = self.files().findIndex(function (row) {
                                return row.id === value;
                            });
                            self.files.splice(idx, 1);
                        });
                    }
                };
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


