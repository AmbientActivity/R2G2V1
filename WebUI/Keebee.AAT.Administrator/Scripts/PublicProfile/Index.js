/*!
 * 1.0 Keebee AAT Copyright © 2016
 * PublicProfile/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

; (function ($) {
    publicprofile.index = {
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
                url: site.url + "PublicProfile/GetData?" + "mediaPathTypeId=" + config.selectedMediaPathTypeId
            })
            .then(function(data) {
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
                    update: function (element, valueAccessor) {
                        ko.utils.unwrapObservable(valueAccessor());

                        var table = element.parentNode;

                        // get the currently selected media path type id from the dropdown
                        var mediaPathTypeSelector = $("#ddlMediaPathTypes");
                        var mediaPathTypeId = mediaPathTypeSelector.val();

                        var category = lists.MediaPathTypeList.filter(function (value) {
                            return value.Id === Number(mediaPathTypeId);
                        })[0].Category.toLowerCase();

                        formatTable(table, mediaPathTypeId, category);
                    }
                };

                ko.virtualElements.allowedBindings.audioRender = true;
                ko.bindingHandlers.audioRender = {
                    update: function (element, valueAccessor) {

                        // handler gets for all category types
                        // only do this logic if this is a thumbnail column
                        var mediaPathTypeSelector = $("#ddlMediaPathTypes");
                        var mediaPathTypeId = mediaPathTypeSelector.val();
                        var category = lists.MediaPathTypeList.filter(function (value) {
                            return value.Id === Number(mediaPathTypeId);
                        })[0].Category.toLowerCase();

                        if (category === "audio") {

                            // get thumbnail column, find anchor element, set tooltip
                            var audio = ko.virtualElements.firstChild(element);
                            var row = audio.parentNode;

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

                ko.virtualElements.allowedBindings.thumbnailRender = true;
                ko.bindingHandlers.thumbnailRender = {
                    update: function (element, valueAccessor) {
                        // handler gets for all category types
                        // only do this logic if this is a thumbnail column
                        var mediaPathTypeSelector = $("#ddlMediaPathTypes");
                        var mediaPathTypeId = mediaPathTypeSelector.val();
                        var category = lists.MediaPathTypeList.filter(function (value) {
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

                    var noMediaMessage = $("#no-records-message");

                    var description = lists.MediaPathTypeList.filter(function(value) {
                        return value.Id === Number(mediaPathTypeId);
                    })[0].ShortDescription;

                    var colThumbnail = $("#col-thumbnail");
                    if (category !== "audio")
                        colThumbnail.html("<div class='virtualPlaceholderImage'></div>");
                    else
                        colThumbnail.html("");
                            
                    var tableDetailElement = $("#table-detail");
                    var tableHeaderElement = $("#table-header");

                    if (table.rows.length > 0) {
                        tableHeaderElement.show();
                        tableDetailElement.show();
                        noMediaMessage.hide();

                        // determine if there is table overflow (to cause a scrollbar)
                        // if so, unhide the scrollbar header column
                        // and adjust the width of the filename column
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
                        noMediaMessage.html("<h2>No " + description.toLowerCase() + " found</h2>");
                        noMediaMessage.show();
                    }
                }

                ko.applyBindings(new FileViewModel(), document.getElementById("public-profile"));

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
                    self.isAudio = ko.observable(false);
                    self.currentStreamId = ko.observable(0);
                    self.currentRowId = ko.observable(0);

                    createFileArray(lists.FileList);
                    createMediaPathTypeArray(lists.MediaPathTypeList);

                    self.selectedMediaPathTypeId = ko.observable(config.selectedMediaPathTypeId);
                    self.selectedMediaPathType = ko.observable(self.mediaPathTypes().filter(function (value) {
                        return value.id === config.selectedMediaPathTypeId;
                    })[0]);

                    $("#delete").tooltip({ delay: { show: 100, hide: 100 } });

                    // when "Add" anchor is clicked, force click of the "file" element
                    $("#upload").click(function () {
                        $("#fileupload").trigger("click");
                    });

                    enableDetail();

                    function createFileArray(list) {
                        self.files.removeAll();
                        $(list)
                            .each(function(index, value) {
                                self.files.push({
                                    id: value.Id,
                                    streamid: value.StreamId,
                                    filename: value.Filename,
                                    filetype: value.FileType,
                                    filesize: value.FileSize,
                                    islinked: value.IsLinked,
                                    path: value.Path,
                                    thumbnail: value.Thumbnail,
                                    mediapathtypeid: value.MediaPathTypeId,
                                    isselected: false,
                                    isplaying: false,
                                    ispaused: false
                                });
                            });
                    };

                    function createMediaPathTypeArray(list) {
                        self.mediaPathTypes.removeAll();
                        $(list)
                            .each(function(index, value) {
                                self.mediaPathTypes.push(
                                {
                                    id: value.Id,
                                    category: value.Category,
                                    description: value.Description,
                                    shortdescription: value.ShortDescription,
                                    path: value.Path,
                                    allowedexts: value.AllowedExts,
                                    allowedtypes: value.AllowedTypes
                                });
                            });

                        var pathType = self.mediaPathTypes()
                            .filter(function(value) {
                                return value.id === config.selectedMediaPathTypeId;
                            })[0];

                        self.isAudio(pathType.category.includes("Audio"));
                    };

                    function enableDetail() {
                        var selected = self.files()
                            .filter(function(value) { return value.isselected; });

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
                        arr.push({ title: "Filename", sortKey: "filename", cssClass: "col-filename" });
                        arr.push({ title: "Type", sortKey: "filetype", cssClass: "col-filetype" });
                        arr.push({ sortKey: "islinked", cssClass: "col-islinked", boolean: true });
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

                        self.selectedMediaPathType(self.mediaPathTypes().filter(function(value) {
                            return value.id === id;
                        })[0]);

                        self.isAudio(self.selectedMediaPathType().category.includes("Audio"));
                        self.checkSelectAll(false);
                        self.selectAllRows();
                        self.clearStreams();
                        self.initUploader();
                    });

                    self.filteredFiles = ko.computed(function() {
                        return ko.utils.arrayFilter(self.files(),
                            function(f) {
                                return (
                                        self.filenameSearch().length === 0 ||
                                            f.filename
                                            .toLowerCase()
                                            .indexOf(self.filenameSearch().toLowerCase()) !==
                                            -1) &&
                                    f.mediapathtypeid === self.selectedMediaPathType().id;
                            });
                    });

                    self.filteredFilesBySelection = ko.computed(function() {
                        return ko.utils.arrayFilter(self.files(),
                            function(f) {
                                return (f.mediapathtypeid === self.selectedMediaPathType().id);
                            });
                    });

                    self.filesTable = ko.computed(function() {
                        var filteredFiles = self.filteredFiles();
                        self.totalFilteredFiles(filteredFiles.length);

                        // look for currently playing or paused audio and set flags
                        var currentlyStreaming = filteredFiles.filter(function(value) {
                            return value.id === self.currentRowId();
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

                    self.checkAllReset = ko.computed(function() {
                        $("#chk_all").prop("checked", false);

                        self.selectedIds([]);
                        $.each(self.filteredFiles(),
                            function(item, value) {
                                value.isselected = false;

                                var chk = tblFile.find("#chk_" + value.id);
                                chk.prop("checked", false);
                            });
                    });

                    self.initUploader = ko.computed(function () {
                        var mediaPathType = self.selectedMediaPathType();

                        utilities.upload.init({
                            url: site.url + "PublicProfile/UploadFile"
                                + "?mediaPath=" + mediaPathType.path
                                + "&mediaPathTypeId=" + mediaPathType.id
                                + "&mediaPathTypeCategory=" + mediaPathType.category,
                            allowedExts: mediaPathType.allowedexts.split(","),
                            allowedTypes: mediaPathType.allowedtypes.split(","),
                            callback: function (filenames) {
                                if (filenames !== null) {
                                    utilities.job.execute({
                                        url: site.url + "PublicProfile/AddFiles",
                                        type: "POST",
                                        waitMessage: "Saving...",
                                        params: {
                                            filenames: filenames,
                                            mediaPath: mediaPathType.path,
                                            mediaPathTypeId: mediaPathType.id
                                        }
                                    })
                                    .then(function(addResult) {
                                        lists.FileList = addResult.FileList;
                                        createFileArray(lists.FileList);
                                        self.sort({ afterSave: true });
                                        self.selectedIds([]);
                                        self.checkSelectAll(false);
                                        enableDetail();
                                    });
                                }
                            }
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
                            .then(function(result) {
                                utilities.job.execute({
                                    url: site.url + "PublicProfile/AddSharedMediaFiles",
                                    type: "POST",
                                    waitMessage: "Adding...",
                                    params: {
                                        streamIds: result.streamIds,
                                        mediaPathTypeId: self.selectedMediaPathType().id
                                    }
                                })
                                .then(function(saveResult) {
                                    lists.FileList = saveResult.FileList;
                                    createFileArray(lists.FileList);
                                    self.sort({ afterSave: true });
                                    self.selectedIds([]);
                                    self.checkSelectAll(false);
                                    enableDetail();
                                })
                                .catch(function() {
                                    enableDetail();
                                });
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
                                    url: site.url + "PublicProfile/DeleteSelected",
                                    type: "POST",
                                    waitMessage: "Deleting...",
                                    params: {
                                        ids: self.selectedIds(),
                                        mediaPathTypeId: self.selectedMediaPathType().id
                                    }
                                })
                                .then(function (result) {
                                    lists.FileList = result.FileList;
                                    createFileArray(lists.FileList);
                                    self.sort({ afterSave: true });
                                    self.selectedIds([]);
                                    self.checkSelectAll(false);
                                    enableDetail();
                                })
                                .catch(function () {
                                    enableDetail();
                                });
                            }
                        });
                    };

                    self.showPreview = function(row) {
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
                            controller: "PublicProfile",
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
                        }).then(function() {
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
                                self.setGlyph(row.id, true, true);
                            } else {
                                audioPlayerElement.pause();
                                self.setGlyph(row.id, false, true);
                            }
                        } else {
                            // end previous
                            self.audioEnded();

                            // play new selection
                            self.setGlyph(row.id, true, true);
                            audioPlayer.attr("type", "audio/" + row.filetype.toLowerCase());
                            audioPlayer.attr("src", site.getApiUrl + "audio/" + row.streamid);

                            // set current id's
                            self.currentStreamId(row.streamid);
                            self.currentRowId(row.id);
                        }

                        enableDetail();
                    };

                    self.audioEnded = function() {
                        self.setGlyph(self.currentRowId(), false, false);
                    };

                    self.setGlyph = function(rowid, paused, success) {
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

                    self.clearStreams = function() {
                        videoPlayer.attr("src", "");
                        audioPlayer.attr("src", "");
                        self.currentRowId(0);
                        self.currentStreamId(0);

                        var currentlyPlaying = self.filteredFiles()
                            .filter(function(value) {
                                return value.ispaused || value.isplaying;
                            })[0];

                        if (currentlyPlaying !== null && typeof currentlyPlaying !== "undefined") {
                            currentlyPlaying.ispaused = false;
                            currentlyPlaying.isplaying = false;
                            self.setGlyph(currentlyPlaying.id, false, false);
                        }
                    };

                    self.selectAllRows = function() {
                        self.selectedIds([]);

                        $.each(self.filteredFiles(),
                            function(item, value) {
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

                    self.selectFile = function(row) {
                        if (typeof row === "undefined") return false;
                        if (row === null) return false;

                        if (row.isselected)
                            self.selectedIds().push(row.id);
                        else
                            self.removeSelectedId(row.id);

                        self.highlightSelectedRows();
                        self.checkSelectAll(self.selectedIds().length === self.filteredFiles().length);
                        enableDetail();

                        return true;
                    };

                    self.removeSelectedId = function(id) {
                        for (var i = self.selectedIds().length - 1; i >= 0; i--) {
                            if (self.selectedIds()[i] === id) {
                                self.selectedIds().splice(i, 1);
                            }
                        }
                    }

                    self.highlightSelectedRows = function() {
                        var rows = tblFile.find("tr");
                        rows.each(function() {
                            $(this).removeClass("highlight");
                        });

                        var selected = self.files()
                            .filter(function(value) { return value.isselected; });

                        $.each(selected,
                            function(item, value) {
                                var r = tblFile.find("#row_" + value.id);
                                $(r).addClass("highlight");
                            });
                    };

                    self.checkSelectAll = function(checked) {
                        self.selectAllIsSelected(checked);
                        $("#chk_all").prop("checked", checked);
                    };

                    self.resetFocus = function () {
                        enableDetail();
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


