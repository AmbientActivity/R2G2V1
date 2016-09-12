/*!
 * MediaFiles/Index.js
 * Author: John Charlton
 * Date: 2015-05
 */

; (function ($) {

    var HIGHLIGHT_ROW_COLOUR = "#e3e8ff";

    mediafiles.index = {
        init: function () {
            var _sortDescending = false;

            var lists = {
                MediaFileList: []
            };

            loadConfig();

            ko.applyBindings(new MediaFileViewModel());

            function loadConfig() {
                $.ajax({
                    type: "GET",
                    url: site.url + "MediaFiles/GetData/",
                    dataType: "json",
                    traditional: true,
                    async: false,
                    success: function (data) {
                        $.extend(lists, data);
                    }
                });
            }

            function MediaFile(streamid, isfolder, filename, filetype, filesize, path) {
                var self = this;

                self.streamid = streamid;
                self.isfolder = isfolder;
                self.filename = filename;
                self.filetype = filetype;
                self.filesize = filesize;
                self.path = path;
            }

            function MediaFileViewModel() {
                var tblMediaFile = $("#tblMediaFile");

                var self = this;

                self.mediaFiles = ko.observableArray([]);
                self.selectedMediaFile = ko.observable();
                self.filenameSearch = ko.observable("");
                self.totalMediaFiles = ko.observable(0);

                createMediaFileArray(lists.MediaFileList);

                function createMediaFileArray(list) {
                    self.mediaFiles.removeAll();
                    $(list).each(function (index, value) {
                        pushMediaFile(value);
                    });
                };

                self.columns = ko.computed(function () {
                    var arr = [];
                    arr.push({ title: "StreamId", sortable: false });
                    arr.push({ title: "Name", sortable: true, sortKey: "filename" });
                    arr.push({ title: "Path", sortable: true, sortKey: "path" });
                    arr.push({ title: "Type", sortable: true, sortKey: "filetype" });
                    arr.push({ title: "Size", sortable: true, sortKey: "filesize" });
                    return arr;
                });

                function pushMediaFile(value) {
                    self.mediaFiles.push(new MediaFile(value.StreamId, value.IsFolder, value.Filename, value.FileType, value.FileSize, value.Path));
                };

                self.selectedMediaFile(self.mediaFiles()[0]);

                self.sort = function () {
                    $(self.columns()).each(function (index, value) {
                        if (value.sortKey === sortKey) {
                            self.mediaFiles.sort(function (a, b) {
                                if (_sortDescending) {
                                    return a[sortKey].toString().toLowerCase() > b[sortKey].toString().toLowerCase()
                                        ? -1 : a[sortKey].toString().toLowerCase() < b[sortKey].toString().toLowerCase()
                                        || a.filename.toLowerCase() > b.filename.toLowerCase() ? 1 : 0;
                                } else {
                                    return a[sortKey].toString().toLowerCase() < b[sortKey].toString().toLowerCase()
                                        ? -1 : a[sortKey].toString().toLowerCase() > b[sortKey].toString().toLowerCase()
                                        || a.filename.toLowerCase() > b.filename.toLowerCase() ? 1 : 0;
                                }
                            });
                        }
                    });
                };

                self.filteredMediaFiles = ko.computed(function () {
                    return ko.utils.arrayFilter(self.mediaFiles(), function (g) {
                        return (
                            (self.filenameSearch().length === 0 || g.filename.toLowerCase().indexOf(self.filenameSearch().toLowerCase()) !== -1)
                        );
                    });
                });

                self.mediaFilesTable = ko.computed(function () {
                    var filteredMediaFiles = self.filteredMediaFiles();
                    self.totalMediaFiles(filteredMediaFiles.length);

                    return filteredMediaFiles;
                });

                self.getMediaFile = function (mediafileid) {
                    var mediafile = null;

                    ko.utils.arrayForEach(self.mediaFiles(), function (item) {
                        if (item.id === mediafileid) {
                            mediafile = item;
                        }
                    });

                    return mediafile;
                };

                self.highlightRow = function (row) {
                    if (row == null) return;

                    var rows = tblMediaFile.find("tr:gt(0)");
                    rows.each(function () {
                        $(this).css("background-color", "#ffffff");
                    });

                    var r = tblMediaFile.find("#row_" + row.id);
                    r.css("background-color", HIGHLIGHT_ROW_COLOUR);
                    $("#tblMediaFile").attr("tr:hover", HIGHLIGHT_ROW_COLOUR);
                };
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