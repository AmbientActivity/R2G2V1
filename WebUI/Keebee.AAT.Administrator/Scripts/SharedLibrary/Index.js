﻿/*!
 * 1.0 Keebee AAT Copyright © 2016
 * SharedLibrary/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

; (function ($) {
    sharedlibrary.index = {
        init: function (options) {
            
            var config = {
                selectedMediaPathTypeId: 0
            };

            $.extend(config, options);

            // buttons
            var cmdDelete = $("#delete");
            var cmdAdd = $("#add");

            // audio player
            var audioPlayer = $("#audio-player");
            var audioPlayerElement = document.getElementById("audio-player");

            // video player
            var videoPlayer = $("#video-player");

            var sortDescending = false;
            var currentSortKey = "filename";
            var primarySortKey = "filename";
            var isBinding = true;

            var lists = {
                FileList: [],
                MediaPathTypeList: []
            };

            utilities.job.execute({
                url: site.url + "SharedLibrary/GetData?mediaPathTypeId=" + config.selectedMediaPathTypeId
            })
            .then(function (data) {
                $.extend(lists, data);

                $("#error-container").hide();
                $("#loading-container").hide();
                $("#table-header").show();
                $("#table-detail").show();
                cmdAdd.prop("disabled", false);

                ko.bindingHandlers.tableUpdated = {
                        update: function (element, valueAccessor, allBindings) {
                        ko.unwrap(valueAccessor());
                        $("#txtSearchFilename").focus();
                        isBinding = false;
                    }
                }

                ko.bindingHandlers.tableRender = {
                    update: function(element, valueAccessor) {
                        ko.utils.unwrapObservable(valueAccessor());
                        for (var index = 0, length = element.childNodes.length; index < length; index++) {
                            var node = element.childNodes[index];
                            if (node.nodeType === 1) {
                                var id = node.id.replace("row_", "");
                                var colLink = $("#link_" + id);
                                if (colLink.length > 0)
                                    colLink.tooltip({ delay: { show: 100, hide: 100 } });
                            }
                        }
                    }
                }

                ko.virtualElements.allowedBindings.audioRender = true;
                ko.bindingHandlers.audioRender = {
                    update: function (element, valueAccessor) {

                        // handler gets for all category types
                        // only do this logic if this is an audio column
                        var mediaPathTypeSelector = $("#ddlMediaPathTypes");
                        var mediaPathTypeId = mediaPathTypeSelector.val();
                        var category = lists.MediaPathTypeList.filter(function(value) {
                            return value.Id === Number(mediaPathTypeId);
                        })[0].Category.toLowerCase();

                        if (category === "audio") {

                            // get expected number of rows
                            var listLength = lists.FileList.filter(function (value) {
                                return value.MediaPathTypeId === Number(mediaPathTypeId);
                            }).length;

                            // get audio column, row and table elements
                            var audio = ko.virtualElements.firstChild(element);
                            var row = audio.parentNode;
                            var table = row.parentNode;

                            // if last row do table formatting
                            if (row === table.rows[listLength - 1]) {
                                formatTable(table, mediaPathTypeId, category);
                            }
                        }
                    }
                };

                ko.virtualElements.allowedBindings.thumbnailRender = true;
                ko.bindingHandlers.thumbnailRender = {
                    update: function (element, valueAccessor) {

                        // handler gets for all category types
                        // only do this logic if this is a thumbnail column
                        var mediaPathTypeSelector = $("#ddlMediaPathTypes");
                        var mediaPathTypeId = mediaPathTypeSelector.val();
                        var category = lists.MediaPathTypeList.filter(function(value) {
                            return value.Id === Number(mediaPathTypeId);
                        })[0].Category.toLowerCase();

                        if (category !== "audio") {

                            // get thumbnail column, find anchor element, set tooltip
                            var thumbnail = ko.virtualElements.firstChild(element);
                            var row = thumbnail.parentNode;
                            while (thumbnail) {
                                if (thumbnail.className === "col-thumbnail") {
                                    var child = ko.virtualElements.firstChild(thumbnail);
                                    var a = ko.virtualElements.nextSibling(child);
                                    $(a).tooltip({ delay: { show: 100, hide: 100 } });
                                }
                                thumbnail = ko.virtualElements.nextSibling(thumbnail);
                            }

                            // get expected number of rows
                            var listLength = lists.FileList.filter(function (value) {
                                return value.MediaPathTypeId === Number(mediaPathTypeId);
                            }).length;

                            // if last row do table formatting
                            var table = row.parentNode;
                            if (row === table.rows[listLength - 1]) {
                                formatTable(table, mediaPathTypeId, category);
                            }
                        }
                    }
                }

                function formatTable(table, mediaPathTypeId, category) {
                    var noRowsMessage = $("#no-records-message");

                    var description = lists.MediaPathTypeList.filter(function (value) {
                        return value.Id === Number(mediaPathTypeId);
                    })[0].ShortDescription;

                    var colThumbnail = $("#col-thumbnail");
                    if (category !== "audio") {
                        colThumbnail.html("<div class='virtualPlaceholderImage'></div>");
                    } else {
                        colThumbnail.html("");
                    }

                    var tableDetailElement = $("#table-detail");
                    var tableHeaderElement = $("#table-header");

                    if (table.rows.length > 0) {
                        tableHeaderElement.show();
                        tableDetailElement.show();
                        noRowsMessage.hide();

                        // determine if there is table overflow (to cause a scrollbar)
                        // if so, unhide the scrollbar header column
                        var colScrollbar = $("#col-scrollbar");

                        if (table.clientHeight > site.getMaxClientHeight) {
                            colScrollbar.prop("hidden", false);
                            colScrollbar.attr("style", "width: 1%;");
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
                    self.selectedStreamIds = ko.observable([]);
                    self.isSharable = ko.observable(false);
                    self.isAudio = ko.observable(false);

                    // for audio previewing
                    self.currentStreamId = ko.observable(0);

                    createFileArray(lists.FileList);
                    createMediaPathTypeArray(lists.MediaPathTypeList);

                    $("#delete").tooltip({ delay: { show: 100, hide: 100 } });
                    $("#upload").click(function () {
                        $("#fileupload").trigger("click");
                    });

                    // media path type
                    self.selectedMediaPathTypeId = ko.observable(config.selectedMediaPathTypeId);
                    self.selectedMediaPathType = ko.observable(self.mediaPathTypes().filter(function (value) {
                        return value.id === config.selectedMediaPathTypeId;
                    })[0]);

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
                                thumbnail: value.Thumbnail,
                                isselected: false,
                                isplaying: false,
                                ispaused: false
                            });
                        });
                    };

                    function createMediaPathTypeArray(list) {
                        self.mediaPathTypes.removeAll();
                        $(list).each(function (index, value) {
                            self.mediaPathTypes.push(
                            {
                                id: value.Id,
                                category: value.Category,
                                description: value.Description,
                                shortdescription: value.ShortDescription,
                                issharable: value.IsSharable,
                                path: value.Path,
                                allowedexts: value.AllowedExts,
                                allowedtypes: value.AllowedTypes
                            });
                        });

                        var mediaPathType = self.mediaPathTypes().filter(function (value) {
                            return value.id === config.selectedMediaPathTypeId;
                        })[0];

                        self.isAudio(mediaPathType.category.includes("Audio"));
                        self.isSharable(mediaPathType.issharable);
                    };

                    function enableDetail() {
                        var selected = self.files()
                            .filter(function (value) { return value.isselected; });

                        cmdDelete.prop("disabled", true);
                        if (selected.length > 0) {
                            if (selected.length < self.filteredFilesBySelection().length) {
                                cmdDelete.prop("disabled", false);
                            }
                        }

                        $("#txtSearchFilename").focus();
                    };

                    self.columns = ko.computed(function () {
                        var arr = [];
                        arr.push({ title: "Filename", sortKey: "filename", cssClass: "col-filename-sl" });
                        arr.push({ title: "Type", sortKey: "filetype", cssClass: "col-filetype" });
                        arr.push({ title: "Size", sortKey: "filesize", cssClass: "col-filesize" });
                        return arr;
                    });

                    self.sort = function (header) {
                        if (isBinding) return;

                        var afterSave = typeof header.afterSave !== "undefined" ? header.afterSave : false;
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

                        var isboolean = false;
                        if (typeof header.boolean !== "undefined") {
                            isboolean = header.boolean;
                        }
                        self.files(utilities.sorting.sortArray(
                            {
                                fileArray: self.files(),
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

                        self.isAudio(self.selectedMediaPathType().category.includes("Audio"));
                        self.isSharable(self.selectedMediaPathType().issharable);
                        self.clearStreams();
                    });

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

                        // look for currently playing or paused audio and set flags
                        var currentlyStreaming = filteredFiles.filter(function (value) {
                            return value.streamid === self.currentStreamId();
                        })[0];

                        if (currentlyStreaming !== null && typeof currentlyStreaming !== "undefined") {
                            if (audioPlayerElement.paused) {
                                currentlyStreaming.ispaused = true;
                                currentlyStreaming.isplaying = false;
                            } else {
                                currentlyStreaming.isplaying = true;
                                currentlyStreaming.ispaused = false;
                            }
                        }

                        return filteredFiles;
                    });

                    self.checkAllReset = ko.computed(function () {
                        $("#chk_all").prop("checked", false);

                        self.selectedStreamIds([]);
                        $.each(self.filteredFiles(), function (item, value) {
                            value.isselected = false;

                            var chk = tblFile.find("#chk_" + value.streamid);
                            chk.prop("checked", false);
                        });
                    });

                    self.initUploader = ko.computed(function () {
                        var mediaPathType = self.selectedMediaPathType();

                        utilities.upload.init({
                            url: site.url + "SharedLibrary/UploadFile"
                                + "?mediaPath=" + mediaPathType.path
                                + "&mediaPathTypeId=" + mediaPathType.id
                                + "&mediaPathTypeCategory=" + mediaPathType.category,
                            allowedExts: mediaPathType.allowedexts.split(","),
                            allowedTypes: mediaPathType.allowedtypes.split(","),
                            callback: function (filenames) {
                                if (filenames !== null) {
                                    utilities.job.execute({
                                        url: site.url + "SharedLibrary/GetData",
                                        waitMessage: "Saving...",
                                        params: {
                                            mediaPathTypeId: mediaPathType.id
                                        }
                                    })
                                    .then(function (addResult) {
                                        lists.FileList = addResult.FileList;
                                        createFileArray(lists.FileList);
                                        self.sort({ afterSave: true });
                                        self.selectedStreamIds([]);
                                        self.checkSelectAll(false);
                                        enableDetail();
                                    });
                                }
                            }
                        });
                    });

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
                                    url: site.url + "SharedLibrary/DeleteSelected",
                                    type: "POST",
                                    waitMessage: "Deleting...",
                                    params: {
                                        streamIds: self.selectedStreamIds(),
                                        mediaPathTypeId: self.selectedMediaPathType().id,
                                        isSharable: self.isSharable()
                                    }
                                })
                                .then(function (result) {
                                    lists.FileList = result.FileList;
                                    createFileArray(lists.FileList);
                                    self.sort({ afterSave: true });
                                    self.selectedStreamIds([]);
                                    self.checkSelectAll(false);
                                    enableDetail();
                                })
                                .catch(function () {
                                    enableDetail();
                                });
                            }
                        });
                    };

                    self.showPreview = function (row) {
                        var pathCategory = self.selectedMediaPathType().category;

                        if (pathCategory.includes("Image"))
                            self.previewImage(row);
                        if (pathCategory.includes("Video"))
                            self.previewVideo(row);
                        if (pathCategory.includes("Audio"))
                            self.previewAudio(row);
                    };

                    self.previewImage = function (row) {
                        $("#thumb_" + row.id).tooltip("hide");
                        utilities.image.show({
                            controller: "SharedLibrary",
                            streamId: row.streamid,
                            filename: row.filename,
                            fileType: row.filetype
                        }).then(function () { enableDetail() });
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

                    self.previewAudio = function (row) {
                        if (self.currentStreamId() === row.streamid) {
                            // already paused or playing                            
                            if (audioPlayerElement.paused) {
                                audioPlayerElement.play();
                                self.setGlyph(row.streamid, true, true);
                            } else {
                                audioPlayerElement.pause();
                                self.setGlyph(row.streamid, false, true);
                            }
                        } else {
                            // end previous
                            self.audioEnded();

                            // play new selection
                            self.setGlyph(row.streamid, true, true);
                            audioPlayer.attr("type", "audio/" + row.filetype.toLowerCase());
                            audioPlayer.attr("src", site.getApiUrl + "audio/" + row.streamid);

                            // set current id's
                            self.currentStreamId(row.streamid);
                        }
                        enableDetail();
                    };

                    self.audioEnded = function () {
                        self.setGlyph(self.currentStreamId(), false, false);
                    };

                    self.setGlyph = function (rowid, paused, success) {
                        var glyphElement = tblFile.find("#audio_" + rowid);
                        var cssPlay = "glyphicon-play";
                        var cssPause = "glyphicon-pause";

                        if (success) {
                            cssPlay = cssPlay.concat(" play-paused");
                            cssPause = cssPause.concat(" play-paused");
                        } else {
                            glyphElement.removeClass("play-paused");
                        }

                        if (paused) {
                            glyphElement.removeClass(cssPlay);
                            glyphElement.addClass(cssPause);
                        } else {
                            glyphElement.removeClass(cssPause);
                            glyphElement.addClass(cssPlay);
                        }
                    };

                    self.clearStreams = function () {
                        videoPlayer.attr("src", "");
                        audioPlayer.attr("src", "");
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

                    self.showLinkedProfiles = function (row) {
                        utilities.partialview.show({
                            url: site.url + "SharedLibrary/GetLinkedResidentsView/",
                            title: "<span class='glyphicon glyphicon-link' style='color: #fff'></span> Linked Profiles",
                            params: { streamId: row.streamid },
                            buttonOK: "Close",
                            okOnly: true,
                            callback: function(dialog) {
                                dialog.close();
                            }
                        });
                    };

                    self.selectAllRows = function () {
                        self.selectedStreamIds([]);

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
                        var rows = tblFile.find("tr");
                        rows.each(function () {
                            $(this).removeClass("highlight");
                        });

                        var selected = self.files()
                            .filter(function (value) { return value.isselected; });

                        $.each(selected, function (item, value) {
                            var r = tblFile.find("#row_" + value.streamid);
                            $(r).addClass("highlight");
                        });
                    };

                    self.checkSelectAll = function (checked) {
                        self.selectAllIsSelected(checked);
                        $("#chk_all").prop("checked", checked);
                    };
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


