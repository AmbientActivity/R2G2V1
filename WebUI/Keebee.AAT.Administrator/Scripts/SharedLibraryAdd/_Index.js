/*!
 * 1.0 Keebee AAT Copyright © 2016
 * SharedLibraryAdd/_Index.js
 * Author: John Charlton
 * Date: 2017-07
 */

;
(function($) {
    sharedlibraryadd.index = {
        init: function (options) {
            var isBinding = true;

            var config = {
                profileId: 0,
                mediaPathTypeId: 0,
                mediaPathTypeDesc: "",
                mediaPathTypeCategory: ""
            };

            $.extend(config, options);

            var cmdAdd = $("#btnAdd");

            // audio player
            var audioPlayer = $("#audio-player");
            var audioPlayerElement = document.getElementById("audio-player");

            // video player
            var videoPlayer = $("#video-player");

            var sortDescending = false;
            var currentSortKey = "filename";
            var primarySortKey = "filename";

            var lists = {
                FileList: []
            };

            utilities.job.execute({
                url: "SharedLibraryAdd/LoadData",
                params: {
                    profileId:  config.profileId,
                    mediaPathTypeId: config.mediaPathTypeId
                }
            })
            .then(function(data) {
                $.extend(lists, data);

                $("#shared-error-container").hide();
                $("#shared-loading-container").hide();
                $("#shared-table-header").show();
                $("#shared-table-detail").show();

                ko.bindingHandlers.tableUpdated = {
                    update: function(element, valueAccessor, allBindings) {
                        ko.unwrap(valueAccessor());
                        $("#txtSharedSearchFilename").focus();
                        isBinding = false;
                    }
                }

                ko.bindingHandlers.tableRender = {
                    update: function(element, valueAccessor) {
                        ko.utils.unwrapObservable(valueAccessor());
                        for (var index = 0, length = element.childNodes.length; index < length; index++) {
                            var node = element.childNodes[index];
                            if (node.nodeType === 1) {
                                var id = node.id.replace("shared_row_", "");
                                var tolltipThumb = $("#shared_thumb_" + id);
                                if (tolltipThumb.length > 0)
                                    tolltipThumb.tooltip({ delay: { show: 100, hide: 100 } });
                            }
                        }

                        // if there are no rows in the table, hide the table and display a message
                        var table = element.parentNode;
                        var noRowsMessage = $("#shared-no-rows-message");

                        var colThumbnail = $("#col-shared-thumbnail");
                        if (config.mediaPathTypeCategory.toLowerCase() !== "audio")
                            colThumbnail.html("<div class='virtualPlaceholderImage'></div>");
                        else
                            colThumbnail.html("");

                        var tableDetailElement = $("#shared-table-detail");
                        var tableHeaderElement = $("#shared-table-header");

                        if (table.rows.length > 0) {
                            tableHeaderElement.show();
                            tableDetailElement.show();
                            noRowsMessage.hide();

                            // determine if there is table overflow (to cause a scrollbar)
                            // if so, unhide the scrollbar header column
                            var colScrollbar = $("#col-shared-scrollbar");

                            if (table.clientHeight > site.getMaxClientHeightSharedLibraryAdd) {
                                colScrollbar.prop("hidden", false);
                                colScrollbar.attr("style", "width: 1%; border-bottom: 1.5px solid #ddd;");
                                tableDetailElement.addClass("container-height-sharedlibrary-add");
                            } else {
                                colScrollbar.prop("hidden", true);
                                tableDetailElement.removeClass("container-height-sharedlibrary-add");
                            }

                        } else {
                            tableHeaderElement.hide();
                            tableDetailElement.hide();
                            noRowsMessage.html("<h2>No " + config.mediaPathTypeDesc.toLowerCase() + " found</h2>");
                            noRowsMessage.show();
                        }
                    }
                }

                ko.applyBindings(new SharedFileViewModel(), document.getElementById("shared-library-add"));

                function SharedFileViewModel() {
                    var tblFile = $("#tblSharedFile");
                    var self = this;

                    self.files = ko.observableArray([]);
                    self.sharedFilenameSearch = ko.observable("");
                    self.sharedTotalFilteredFiles = ko.observable(0);
                    self.selectAllIsSelected = ko.observable(false);
                    self.selectedStreamIds = ko.observable([]);
                    self.isAudio = ko.observable(config.mediaPathTypeCategory.includes("Audio"));

                    // for audio previewing
                    self.currentStreamId = ko.observable(0);

                    createFileArray(lists.FileList);

                    enableDetail();

                    function createFileArray(list) {
                        self.files.removeAll();
                        $(list)
                            .each(function(index, value) {
                                self.files.push({
                                    streamid: value.StreamId,
                                    filename: value.Filename,
                                    filetype: value.FileType,
                                    path: value.Path,
                                    thumbnail: value.Thumbnail,
                                    isselected: false,
                                    isplaying: false,
                                    ispaused: false
                                });
                            });
                    };

                    function enableDetail() {
                        var numSelected = $("input:checkbox:checked", $("#tblSharedFile")).length;

                        cmdAdd.prop("disabled", numSelected === 0);

                        $("#txtSharedSearchFilename").focus();
                    };

                    self.sharedcolumns = ko.computed(function () {
                        var arr = [];
                        arr.push({ title: "Filename", sortKey: "filename", cssClass: "col-filename-sl" });
                        return arr;
                    });

                    self.sort = function(header) {
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
                            array: self.files(),
                            columns: self.sharedcolumns(),
                            sortKey: sortKey,
                            primarySortKey: primarySortKey,
                            descending: sortDescending,
                            boolean: isboolean
                        }));

                        enableDetail();
                    };

                    self.filteredFiles = ko.computed(function() {
                        return ko.utils.arrayFilter(self.files(),
                            function(f) {
                                return (
                                    self.sharedFilenameSearch().length === 0 ||
                                        f.filename.toLowerCase().indexOf(self.sharedFilenameSearch().toLowerCase()) !== -1);
                            });
                    });

                    self.sharedFilesTable = ko.computed(function () {
                        var filteredFiles = self.filteredFiles();
                        self.sharedTotalFilteredFiles(filteredFiles.length);

                        // look for currently playing or paused audio and set flags
                        var currentlyStreaming = filteredFiles.filter(function(value) {
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

                    self.checkAllReset = ko.computed(function() {
                        $("#shared_chk_all").prop("checked", false);

                        self.selectedStreamIds([]);
                        $.each(self.filteredFiles(),
                            function(item, value) {
                                value.isselected = false;

                                var chk = tblFile.find("#shared_chk_" + value.streamid);
                                chk.prop("checked", false);
                            });
                    });

                    self.showPreview = function(row) {
                        if (config.mediaPathTypeCategory.includes("Image"))
                            self.previewImage(row);
                        if (config.mediaPathTypeCategory.includes("Video"))
                            self.previewVideo(row);
                        if (config.mediaPathTypeCategory.includes("Audio"))
                            self.previewAudio(row);
                    };

                    self.previewImage = function(row) {
                        $("#shared_thumb_" + row.streamid).tooltip("hide");
                        utilities.image.show({
                                controller: "SharedLibrary",
                                streamId: row.streamid,
                                filename: row.filename,
                                fileType: row.filetype
                            })
                            .then(function() { enableDetail() });
                    };

                    self.previewVideo = function(row) {
                        $("#shared_thumb_" + row.streamid).tooltip("hide");

                        utilities.video.show({
                                src: site.getApiUrl + "videos/" + row.streamid,
                                player: videoPlayer,
                                filename: row.filename,
                                fileType: row.filetype.toLowerCase()
                            })
                            .then(function() {
                                $("#modal-video")
                                    .on("hidden.bs.modal",
                                        function() {
                                            videoPlayer.attr("src", "");
                                        });
                            });
                    };

                    self.previewAudio = function(row) {
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

                    self.audioEnded = function() {
                        self.setGlyph(self.currentStreamId(), false, false);
                    };

                    self.setGlyph = function(rowid, paused, success) {
                        var glyphElement = tblFile.find("#shared_audio_" + rowid);
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
                        self.selectedStreamIds([]);

                        $.each(self.filteredFiles(),
                            function(item, value) {
                                if (self.selectAllIsSelected())
                                    self.selectedStreamIds().push(value.streamid);
                                else
                                    self.selectedStreamIds().pop(value.streamid);

                                value.isselected = self.selectAllIsSelected();
                                var chk = tblFile.find("#shared_chk_" + value.streamid);
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
                            self.selectedStreamIds().push(row.streamid);
                        else
                            self.removeSelectedStreamId(row.streamid);

                        self.highlightSelectedRows();
                        self.checkSelectAll(self.selectedStreamIds().length === self.filteredFiles().length);
                        enableDetail();

                        return true;
                    };

                    self.highlightSelectedRows = function() {
                        var rows = tblFile.find("tr");
                        rows.each(function() {
                            $(this).removeClass("highlight");
                        });

                        var selected = self.files()
                            .filter(function(value) { return value.isselected; });

                        $.each(selected,
                            function(item, value) {
                                var r = tblFile.find("#shared_row_" + value.streamid);
                                $(r).addClass("highlight");
                            });
                    };

                    self.checkSelectAll = function(checked) {
                        self.selectAllIsSelected(checked);
                        $("#shared_chk_all").prop("checked", checked);
                    };

                    self.resetFocus = function () {
                        enableDetail();
                    }

                    self.removeSelectedStreamId = function (id) {
                        for (var i = self.selectedStreamIds().length - 1; i >= 0; i--) {
                            if (self.selectedStreamIds()[i] === id) {
                                self.selectedStreamIds().splice(i, 1);
                            }
                        }
                    }
                };
            })
            .catch(function(error) {
                $("#shared-loading-container").hide();
                $("#shared-error-container")
                    .html("<div><h2>Data load error:</h2></div>")
                    .append("<div>" + error.message + "</div>")
                    .append("<div><h3>Please try refreshing the page</h3></div>");
                $("#shared-error-container").show();
            });
        }
    }
})(jQuery);