/*!
 * 1.0 Keebee AAT Copyright © 2016
 * VideoCaptures/Index.js
 * Author: John Charlton
 * Date: 2016-10
 */

; (function ($) {
    videocaptures.index = {
        init: function () {
            var sortDescending = false;
            var currentSortKey = "filename";

            var lists = {
                VideoCaptureList: []
            };

            $.get(site.url + "VideoCaptures/GetData/")
                .done(function (data) {
                    $.extend(lists, data);

                    ko.applyBindings(new VideoCaptureViewModel());

                    function VideoCapture(filename, path, filesize) {
                        var self = this;

                        self.filename = filename;
                        self.path = path;
                        self.filesize = filesize;

                        self.download = function (row) {
                            window.location = "VideoCaptures/Download?path=" + row.path + "&filename=" + row.filename;
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
                            arr.push({ title: "Name", sortable: true, sortKey: "filename", numeric: false, cssClass: "" });
                            arr.push({ title: "Path", sortable: true, sortKey: "path", numeric: false, cssClass: "" });
                            arr.push({ title: "Size", sortable: true, sortKey: "filesize", numeric: true, cssClass: "col-filesize" });
                            return arr;
                        });

                        function pushVideoCapture(value) {
                            self.videoCaptures.push(new VideoCapture(value.Filename, value.Path, value.FileSize));
                        };

                        self.selectedVideoCapture(self.videoCaptures()[0]);

                        self.sort = function (header) {
                            var sortKey = header.sortKey;

                            if (sortKey !== currentSortKey) {
                                sortDescending = false;
                            } else {
                                sortDescending = !sortDescending;
                            }

                            currentSortKey = sortKey;

                            $(self.columns()).each(function (index, value) {
                                if (value.sortKey === sortKey) {
                                    self.videoCaptures.sort(function (a, b) {
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
                            var r = tblVideoCapture.find("#row_" + row.streamid);
                            $(r).addClass("highlight").siblings().removeClass("highlight");
                        };
                    };
            });      
        }
    }
})(jQuery);