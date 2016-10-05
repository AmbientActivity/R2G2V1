/*!
 * 1.0 Keebee AAT Copyright © 2016
 * VideoCaptures/Index.js
 * Author: John Charlton
 * Date: 2016-10
 */

; (function ($) {

    var HIGHLIGHT_ROW_COLOUR = "#e3e8ff";

    videocaptures.index = {
        init: function () {
            var _sortDescending = false;
            var _currentSortKey = "filename";

            var lists = {
                VideoCaptureList: []
            };

            loadData();

            ko.applyBindings(new VideoCaptureViewModel());

            function loadData() {
                $.ajax({
                    type: "GET",
                    url: site.url + "VideoCaptures/GetData/",
                    dataType: "json",
                    traditional: true,
                    async: false,
                    success: function (data) {
                        $.extend(lists, data);
                    }
                });
            }

            function VideoCapture(filename, path, filesize) {
                var self = this;

                self.filename = filename;
                self.path = path;
                self.filesize = filesize;

                self.download = function (row) {
                    window.location = "VideoCaptures/Download?filename=" + row.filename;
                };
            }

            function VideoCaptureViewModel() {
                var tblVideoCapture = $("#tblVideoCapture");

                var self = this;

                self.videoCaptures = ko.observableArray([]);
                self.selectedVideoCapture = ko.observable();
                self.filenameSearch = ko.observable("");
                self.totalVideoCaptures = ko.observable(0);

                createVideoCaptureArray(lists.VideoCaptureList);

                function createVideoCaptureArray(list) {
                    self.videoCaptures.removeAll();
                    $(list).each(function (index, value) {
                        pushVideoCapture(value);
                    });
                };

                self.columns = ko.computed(function () {
                    var arr = [];
                    arr.push({ title: "Name", sortable: true, sortKey: "filename" });
                    arr.push({ title: "Path", sortable: true, sortKey: "path" });
                    arr.push({ title: "Size", sortable: true, sortKey: "filesize" });
                    return arr;
                });

                function pushVideoCapture(value) {
                    self.videoCaptures.push(new VideoCapture(value.Filename, value.Path, value.FileSize));
                };

                self.selectedVideoCapture(self.videoCaptures()[0]);

                self.sort = function (header) {
                    var sortKey = header.sortKey;

                    if (sortKey !== _currentSortKey) {
                        _sortDescending = false;
                    } else {
                        _sortDescending = !_sortDescending;
                    }

                    _currentSortKey = sortKey;

                    $(self.columns()).each(function (index, value) {
                        if (value.sortKey === sortKey) {
                            self.videoCaptures.sort(function (a, b) {
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

                self.filteredVideoCaptures = ko.computed(function () {
                    return ko.utils.arrayFilter(self.videoCaptures(), function (g) {
                        return (
                            (self.filenameSearch().length === 0 || g.filename.toLowerCase().indexOf(self.filenameSearch().toLowerCase()) !== -1)
                        );
                    });
                });

                self.videoCapturesTable = ko.computed(function () {
                    var filteredVideoCaptures = self.filteredVideoCaptures();
                    self.totalVideoCaptures(filteredVideoCaptures.length);

                    return filteredVideoCaptures;
                });

                self.highlightRow = function (row) {
                    if (row == null) return;

                    var rows = tblVideoCapture.find("tr:gt(0)");
                    rows.each(function () {
                        $(this).css("background-color", "#ffffff");
                    });

                    var r = tblVideoCapture.find("#row_" + row.streamid);
                    r.css("background-color", HIGHLIGHT_ROW_COLOUR);
                    $("#tblVideoCapture").attr("tr:hover", HIGHLIGHT_ROW_COLOUR);
                };
            };

            ko.utils.stringStartsWith = function (string, startsWith) {
                string = string || "";
                if (startsWith.length > string.length) return false;
                return string.substring(0, startsWith.length) === startsWith;
            };
        }
    }
})(jQuery);