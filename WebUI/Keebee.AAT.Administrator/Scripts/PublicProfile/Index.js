/*!
 * 1.0 Keebee AAT Copyright © 2016
 * PublicProfile/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

function CuteWebUI_AjaxUploader_OnPostback() {
    utilities.inprogress.show({ message: "Saving..." });
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
    publicprofile.index = {
        init: function (values) {

            var config = {
                selectedMediaPathTypeId: 0
            };

            $.extend(config, values);

            // buttons
            var cmdDelete = $("#delete");
            var cmdAdd = $("#add");

            // audio player
            var audioPlayer = $("#audio-player");
            var audioPlayerElement = document.getElementById("audio-player");

            // video player
            var videoPlayer = $("#video-player");

            var sortDescending = true; // will get flippedd to false upon initial load
            var currentSortKey = "filename";
            var isBinding = true;

            var lists = {
                FileList: [],
                MediaPathTypeList: []
            };

            $.get(site.url + "PublicProfile/GetData?" + "mediaPathTypeId=" + $("#mediaPathTypeId").val())
                .done(function(data) {
                    $.extend(lists, data);

                    $("#loading-container").hide();
                    $("#table-header").show();
                    $("#table-detail").show();
                    cmdAdd.removeAttr("disabled");
                    $("#uploadbutton").removeAttr("disabled");

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
                            var table = element.parentNode; // get the table element
                            var noRowsMessage = $("#no-rows-message");
                            var mediaPathTypeId = $("#mediaPathTypeId").val();

                            var description = lists.MediaPathTypeList.filter(function (value) {
                                return value.Id === Number(mediaPathTypeId);
                            })[0].ShortDescription;

                            var tableDetailElement = $("#table-detail");
                            var tableHeaderElement = $("#table-header");

                            if (table.rows.length > 1) {
                                tableHeaderElement.show();
                                tableDetailElement.show();
                                noRowsMessage.hide();

                                // determine if there is table overflow (to cause a scrollbar)
                                // if so, increase the right margin of last column header 
                                var colRight = $("#sort-right");

                                if (table.clientHeight > site.getMaxClientHeight) {
                                    colRight.addClass("table-scrollbar");
                                    tableDetailElement.addClass("container-height");
                                } else {
                                    colRight.removeClass("table-scrollbar");
                                    tableDetailElement.removeClass("container-height");
                                }

                            } else {
                                tableHeaderElement.hide();
                                tableDetailElement.hide();
                                noRowsMessage.html("<h2>No " + description.toLowerCase() + " found</h2>");
                                noRowsMessage.show();
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

                        // for audio previewing
                        self.isAudio = ko.observable(false);
                        self.currentStreamId = ko.observable(0);
                        self.currentRowId = ko.observable(0);

                        createFileArray(lists.FileList);
                        createMediaPathTypeArray(lists.MediaPathTypeList);
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
                                        shortdescription: value.ShortDescription
                                    });
                                });

                            var pathType = self.mediaPathTypes()
                                .filter(function(value) {
                                    return value.id === self.selectedMediaPathType();
                                })[0];

                            self.isAudio(pathType.category.includes("Audio"));
                        };

                        function enableDetail() {
                            var selected = self.files()
                                .filter(function(value) { return value.isselected; });

                            cmdDelete.attr("disabled", "disabled");
                            if (selected.length > 0) {
                                if (selected.length < self.filteredFilesBySelection().length) {
                                    cmdDelete.removeAttr("disabled");
                                }
                            }

                            $("#txtSearchFilename").focus();
                        };

                        self.columns = ko.computed(function () {
                            var arr = [];
                            arr.push({ sortKey: "filename", numeric: false });
                            arr.push({ sortKey: "islinked", numeric: true });
                            return arr;
                        });

                        self.sort = function (header) {
                            if (isBinding && header.sortKey !== currentSortKey)
                                return;

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

                            $(self.columns())
                                .each(function(index, value) {
                                    if (value.sortKey === sortKey) {
                                        self.files.sort(function(a, b) {
                                            if (value.numeric) {
                                                if (sortDescending) {
                                                    return a[sortKey] > b[sortKey]
                                                        ? -1
                                                        : a[sortKey] < b[sortKey] || a.filename > b.filename ? 1 : 0;
                                                } else {
                                                    return a[sortKey] < b[sortKey]
                                                        ? -1
                                                        : a[sortKey] > b[sortKey] || a.filename > b.filename ? 1 : 0;
                                                }
                                            } else {
                                                if (sortDescending) {
                                                    return a[sortKey].toString().toLowerCase() >
                                                        b[sortKey].toString().toLowerCase()
                                                        ? -1
                                                        : a[sortKey].toString().toLowerCase() <
                                                        b[sortKey].toString().toLowerCase() ||
                                                        a.filename.toLowerCase() > b.filename.toLowerCase()
                                                        ? 1
                                                        : 0;
                                                } else {
                                                    return a[sortKey].toString().toLowerCase() <
                                                        b[sortKey].toString().toLowerCase()
                                                        ? -1
                                                        : a[sortKey].toString().toLowerCase() >
                                                        b[sortKey].toString().toLowerCase() ||
                                                        a.filename.toLowerCase() > b.filename.toLowerCase()
                                                        ? 1
                                                        : 0;
                                                }
                                            }
                                        });
                                    }
                                });
                        };

                        self.reloadUploaderHtml = function() {
                            var mediaPathTypeId = self.selectedMediaPathType();

                            $.get(site.url + "PublicProfile/GetUploaderHtml?mediaPathTypeId=" + mediaPathTypeId)
                                .done(function(result) {
                                    $("#uploader-html-container").html(result.UploaderHtml);
                                });
                        };

                        self.selectedMediaPathType.subscribe(function (id) {
                            if (typeof id === "undefined") return;
                            $("#mediaPathTypeId").val(id);
                            self.isAudio(self.mediaPathType().category.includes("Audio"));

                            self.reloadUploaderHtml();
                            self.checkSelectAll(false);
                            self.selectAllRows();
                            self.clearStreams();
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
                                        f.mediapathtypeid === self.selectedMediaPathType();
                                });
                        });

                        self.filteredFilesBySelection = ko.computed(function() {
                            return ko.utils.arrayFilter(self.files(),
                                function(f) {
                                    return (f.mediapathtypeid === self.selectedMediaPathType());
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

                        self.showLinkFromSharedLibarayDialog = function() {
                            var title = "<span class='glyphicon glyphicon-link' style='color: #fff'></span>";
                            var mediaPathTypeDesc = self.mediaPathType().shortdescription;

                            $.get(site.url + "PublicProfile/GetSharedLibarayLinkView/",
                                    { mediaPathTypeId: self.selectedMediaPathType() })
                                .done(function(message) {
                                    if (message.length === 0) {
                                        var hasHave = "has";
                                        if (mediaPathTypeDesc.endsWith("s"))
                                            hasHave = "have";

                                        BootstrapDialog.show({
                                            title: title + " Add <b>" + mediaPathTypeDesc + "</b> From Shared Library",
                                            message: $("<div></div>")
                                                .append("All available " +
                                                    mediaPathTypeDesc +
                                                    " " +
                                                    hasHave +
                                                    " already been added to the public profile."),

                                            closable: false,
                                            buttons: [
                                                {
                                                    label: "OK",
                                                    cssClass: "btn-primary",
                                                    action: function(dialog) {
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
                                                    action: function(dialog) {
                                                        dialog.close();
                                                    }
                                                }, {
                                                    label: "OK",
                                                    cssClass: "btn-primary",
                                                    action: function(dialog) {
                                                        self.addSharedFiles();
                                                        dialog.close();
                                                    }
                                                }
                                            ]
                                        });
                                    }
                                });
                        };

                        self.showDeleteSelectedDialog = function() {
                            BootstrapDialog.show({
                                type: BootstrapDialog.TYPE_DANGER,
                                title: "Delete Files?",
                                message: "Delete all selected files?",
                                closable: false,
                                buttons: [
                                    {
                                        label: "Cancel",
                                        action: function(dialog) {
                                            dialog.close();
                                        }
                                    }, {
                                        label: "Yes, Delete",
                                        cssClass: "btn-danger",
                                        action: function(dialog) {
                                            self.deleteSelected();
                                            dialog.close();
                                        }
                                    }
                                ]
                            });
                        };

                        self.showPreview = function(row) {
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

                            $.get(site.url +
                                    "PublicProfile/GetImageViewerView?streamId=" +
                                    row.streamid +
                                    "&fileType=" +
                                    row.filetype)
                                .done(function(message) {
                                    BootstrapDialog.show({
                                        type: BootstrapDialog.TYPE_INFO,
                                        title: row.filename + "." + row.filetype.toLowerCase(),
                                        message: $("<div></div>").append(message),
                                        closable: false,
                                        buttons: [
                                            {
                                                label: "Close",
                                                action: function(dialog) {
                                                    dialog.close();
                                                    enableDetail();
                                                }
                                            }
                                        ]
                                    });
                                })
                                .error(function(message) {
                                    BootstrapDialog.show({
                                        type: BootstrapDialog.TYPE_DANGER,
                                        title: "Error",
                                        message: $("<div></div>").append(message),
                                        closable: false,
                                        buttons: [
                                            {
                                                label: "Close",
                                                action: function(dialog) {
                                                    dialog.close();
                                                    enableDetail();
                                                }
                                            }
                                        ]
                                    });
                                });
                        };

                        self.previewVideo = function (row) {
                            $("#thumb_" + row.id).tooltip("hide");

                            var src = site.getApiUrl + "videos/" + row.streamid;
                            var filetype = row.filetype.toLowerCase();

                            BootstrapDialog.show({
                                type: BootstrapDialog.TYPE_INFO,
                                title: row.filename + "." + row.filetype.toLowerCase(),
                                width: "auto",
                                message: $("<div></div>").append(videoPlayer),
                                onshown: function() {
                                    videoPlayer.attr("src", src);
                                },
                                closable: false,
                                buttons: [
                                    {
                                        label: "Close",
                                        action: function(dialog) {
                                            videoPlayer.attr("src", "");
                                            videoPlayer.attr("type", "video/" + filetype);
                                            dialog.close();
                                            enableDetail();
                                        }
                                    }
                                ]
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

                        self.deleteSelected = function() {
                            var ids = self.selectedIds();
                            var mediaPathTypeId = $("#mediaPathTypeId").val();

                            self.clearStreams();
                            utilities.inprogress.show()
                                .then(function(dialog) {
                                    $.post(site.url + "PublicProfile/DeleteSelected/",
                                        {
                                            ids: ids,
                                            mediaPathTypeId: mediaPathTypeId
                                        })
                                        .done(function(result) {
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
                                        .error(function(result) {
                                            dialog.close();
                                            enableDetail();

                                            BootstrapDialog.show({
                                                type: BootstrapDialog.TYPE_DANGER,
                                                title: "Delete Error",
                                                message: "Unexpected Error\n" + result
                                            });
                                        });
                                });
                        };

                        self.addSharedFiles = function() {
                            var ids = [];
                            $("input[name='shared_files']:checked")
                                .each(function(item, value) {
                                    ids.push(value.id);
                                });

                            var mediaPathTypeId = $("#mediaPathTypeId").val();

                            self.clearStreams();

                            $.post(site.url + "PublicProfile/AddSharedMediaFiles/",
                                    { streamIds: ids, mediaPathTypeId: mediaPathTypeId })
                                .done(function(result) {
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
                                .error(function(result) {
                                    enableDetail();

                                    BootstrapDialog.show({
                                        type: BootstrapDialog.TYPE_DANGER,
                                        title: "Error Adding Shared Files",
                                        message: "Unexpected Error\n" + result
                                    });
                                });
                        };

                        self.checkSelectAll = function(checked) {
                            self.selectAllIsSelected(checked);
                            $("#chk_all").prop("checked", checked);
                        };

                        self.mediaPathType = function() {
                            return self.mediaPathTypes()
                                .filter(function(value) {
                                    return value.id === self.selectedMediaPathType();
                                })[0];
                        };
                    };
                });
        }
    }
})(jQuery);


