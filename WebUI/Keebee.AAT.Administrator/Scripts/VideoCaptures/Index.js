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
            var primarySortKey = "filename";
            var isBinding = true;

            var lists = {
                VideoCaptureList: []
            };

            $.get(site.url + "VideoCaptures/GetData/")
                .done(function (data) {
                    $.extend(lists, data);

                    $("#loading-container").hide();
                    $("#table-header").show();
                    $("#table-detail").show();

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
                            var noRowsMessage = $("#no-records-message");

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
                                    colScrollbar.attr("style", "width: 1%;");
                                    tableDetailElement.addClass("container-height");
                                } else {
                                    colScrollbar.prop("hidden", true);
                                    tableDetailElement.removeClass("container-height");
                                }

                            } else {
                                tableHeaderElement.hide();
                                tableDetailElement.hide();
                                noRowsMessage.show();
                            }
                        }
                    }

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
                            arr.push({ title: "Filename", sortable: true, sortKey: "filename", numeric: false, cssClass: "col-filename-vc" });
                            arr.push({ title: "Path", sortable: true, sortKey: "path", numeric: false, cssClass: "col-path" });
                            arr.push({ title: "Size", sortable: true, sortKey: "filesize", numeric: true, cssClass: "col-filesize" });
                            return arr;
                        });

                        function pushVideoCapture(value) {
                            self.videoCaptures.push(new VideoCapture(value.Filename, value.Path, value.FileSize));
                        };

                        self.selectedVideoCapture(self.videoCaptures()[0]);

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
                            self.videoCaptures(utilities.sorting.sortArray(
                                {
                                    fileArray: self.videoCaptures(),
                                    columns: self.columns(),
                                    sortKey: sortKey,
                                    primarySortKey: primarySortKey,
                                    descending: sortDescending,
                                    boolean: isboolean
                                }));
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