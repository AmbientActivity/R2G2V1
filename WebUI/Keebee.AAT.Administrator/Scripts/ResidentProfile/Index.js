/*!
 * ResidentProfile/Index.js
 * Author: John Charlton
 * Date: 2016-08
 */

function CuteWebUI_AjaxUploader_OnPostback() {
    utilities.inprogress.show({ message: "Saving..." });
    document.forms[0].submit();
}

function CuteWebUI_AjaxUploader_OnError(message) {
    utilities.alert.show({
        type: BootstrapDialog.TYPE_DANGER,
        title: "File Error",
        message: message
    });
    EnableScreen();
    return false;
}

function CuteWebUI_AjaxUploader_OnTaskError(obj, message, reason) {
    utilities.alert.show({
        type: BootstrapDialog.TYPE_DANGER,
        title: "Task Error",
        message: "Error uploading file <b>" + obj.FileName + "</b>.\nMessage: " + message
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
    $("#uploader-html-container").prop("hidden", true);
    $("#lnkGoBack").prop("hidden", false);
    $("#lblGoBackDisabled").prop("hidden", true);
    $("#txtSearchFilename").prop("disabled", false);
    $("#add").prop("disabled", false);
    $("select").prop("disabled", false);
    $("#main-menu").prop("hidden", false);
    $("#menu-login").prop("hidden", false);
}

function DisableScreen() {
    $("#uploader-html-container").prop("hidden", false);
    $("#lnkGoBack").prop("hidden", true);
    $("#lblGoBackDisabled").prop("hidden", false);
    $("#txtSearchFilename").prop("disabled", true);
    $("#add").prop("disabled", true);
    $("#delete").prop("hidden", true);
    $("select").prop("disabled", true);
    $("#main-menu").prop("hidden", true);
    $("#menu-login").prop("hidden", true);
}

; (function ($) {
    residentprofile.index = {
        init: function (options) {
            var config = {
                residentid: 0,
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
            
            $.get(site.url + "ResidentProfile/GetData/" + config.residentid
                    + "?mediaPathTypeId=" + $("#mediaPathTypeId").val())
            .done(function (data) {
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
                        for (var index = 0, length = element.childNodes.length; index < length; index++) {
                            var node = element.childNodes[index];
                            if (node.nodeType === 1) {
                                var id = node.id.replace("row_", "");

                                var tooltipElement = $("#thumb_" + id);
                                if (tooltipElement.length > 0)
                                    tooltipElement.tooltip({ delay: { show: 100, hide: 100 } });
                            }
                        }

                        // if there are no rows in the table, hide the table and display a message
                        var table = element.parentNode;
                        var noMediaMessage = $("#no-records-message");
                        var mediaPathTypeId = $("#mediaPathTypeId").val();

                        var description = lists.MediaPathTypeList.filter(function(value) {
                            return value.Id === Number(mediaPathTypeId);
                        })[0].ShortDescription;

                        var category = lists.MediaPathTypeList.filter(function (value) {
                            return value.Id === Number(mediaPathTypeId);
                        })[0].Category.toLowerCase();

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
                }

                ko.applyBindings(new FileViewModel());

                function FileViewModel() {
                    var tblFile = $("#tblFile");
                    var self = this;

                    self.files = ko.observableArray([]);
                    self.mediaPathTypes = ko.observableArray([]);
                    self.selectedMediaPathType = ko.observable(config.selectedMediaPathTypeId);
                    self.filenameSearch = ko.observable("");
                    self.totalFilteredFiles = ko.observable(0);
                    self.selectAllIsSelected = ko.observable(false);
                    self.selectedIds = ko.observable([]);
                    self.isSharable = ko.observable(false);
                    self.isAudio = ko.observable(false);

                    // for audio previewing
                    self.currentStreamId = ko.observable(0);
                    self.currentRowId = ko.observable(0);

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
                                thumbnail: value.Thumbnail,
                                path: value.Path,
                                mediapathtypeid: value.MediaPathTypeId,
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
                                    issharable: value.IsSharable
                                });
                        });

                        var mediaPathType = self.mediaPathTypes().filter(function (value) {
                            return value.id === self.selectedMediaPathType();
                        })[0];

                        self.isSharable(mediaPathType.issharable);
                        self.isAudio(mediaPathType.category.includes("Audio"));
                    };

                    function enableDetail() {
                        var selected = self.files()
                            .filter(function (value) { return value.isselected; });

                        cmdDelete.prop("disabled", (selected.length === 0));

                        if (self.isSharable())
                            cmdAdd.show();
                        else
                            cmdAdd.hide();

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

                    self.reloadUploaderHtml = function () {
                        var mediaPathTypeId = self.selectedMediaPathType();

                        $.get(site.url + "ResidentProfile/GetUploaderHtml?mediaPathTypeId=" + mediaPathTypeId)
                            .done(function (result) {
                                $("#uploader-html-container").html(result.UploaderHtml);
                        });
                    };

                    self.selectedMediaPathType.subscribe(function (id) {
                        if (typeof id === "undefined") return;
                        $("#mediaPathTypeId").val(id);
                        self.reloadUploaderHtml();
                        self.checkSelectAll(false);
                        self.selectAllRows();

                        self.isSharable(self.mediaPathType().issharable);
                        self.isAudio(self.mediaPathType().category.includes("Audio"));

                        enableDetail();

                        self.clearStreams();
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
                        self.totalFilteredFiles(filteredFiles.length);

                        // look for currently playing or paused audio and set flags
                        var currentlyStreaming = filteredFiles.filter(function (value) {
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

                    self.addFromSharedLibray = function () {
                        self.clearStreams();

                        utilities.sharedlibrary.show({
                            url: site.url + "ResidentProfile/GetSharedLibarayLinkView/",
                            mediaPathTypeDesc: self.mediaPathType().shortdescription,
                            params: { residentId: config.residentid, mediaPathTypeId: self.selectedMediaPathType() }
                        })
                        .then(function (result) {
                            result.dialog.close();
                            utilities.job.execute({
                                url: site.url + "ResidentProfile/AddSharedMediaFiles",
                                type: "POST",
                                waitMessage: "Adding...",
                                params: {
                                    streamIds: result.streamIds,
                                    residentId: config.residentid,
                                    mediaPathTypeId: self.selectedMediaPathType()
                                }
                            })
                            .then(function (saveResult) {
                                lists.FileList = saveResult.FileList;
                                createFileArray(lists.FileList);
                                self.sort({ afterSave: true });
                                self.selectedIds([]);
                                self.checkSelectAll(false);
                                enableDetail();
                            })
                            .catch(function () {
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
                                    url: site.url + "ResidentProfile/DeleteSelected",
                                    type: "POST",
                                    waitMessage: "Deleting...",
                                    params: {
                                        ids: self.selectedIds(),
                                        residentId: config.residentid,
                                        mediaPathTypeId: $("#mediaPathTypeId").val()
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

                    self.showPreview = function (row) {
                        var pathCategory = self.mediaPathType().category;

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
                            controller: "ResidentProfile",
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

                    self.audioEnded = function () {
                        self.setGlyph(self.currentRowId(), false, false);
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
                        self.checkSelectAll(self.selectedIds().length === self.filteredFiles().length);
                        enableDetail();

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

                    self.mediaPathType = function () {
                        return self.mediaPathTypes()
                            .filter(function (value) {
                                return value.id === self.selectedMediaPathType();
                            })[0];
                    }

                    self.resetFocus = function () {
                        enableDetail();
                    }
                };
            })
            .error(function (request) {
                $("#loading-container").hide();
                $("#error-container")
                    .html("<div><h2>Data load error:</h2></div>")
                    .append("<div>" + request.responseText + "</div>")
                    .append("<div><h3>Please try refreshing the page</h3></div>");
                $("#error-container").show();
            });
        }        
    }
})(jQuery);
