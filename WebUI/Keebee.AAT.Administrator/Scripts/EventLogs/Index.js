/*!
 * 1.0 Keebee AAT Copyright © 2016
 * EventLogs/Index.js
 * Author: John Charlton
 * Date: 2016-08
 */

; (function ($) {
    eventlogs.index = {
        init: function () {
            var sortDescending = false;
            var currentSortKey = "filename";

            var lists = {
                EventLogList: []
            };

            $.get(site.url + "EventLogs/GetData/")
                .done(function (data) {
                    $.extend(lists, data);

                    $("#loading-container").hide();
                    $("#tblEventLog").show();

                    ko.applyBindings(new EventLogViewModel());

                    function EventLog(streamid, isfolder, filename, filetype, filesize, path) {
                        var self = this;

                        self.streamid = streamid;
                        self.isfolder = isfolder;
                        self.filename = filename;
                        self.filetype = filetype;
                        self.filesize = filesize;
                        self.path = path;

                        self.download = function (row) {
                            window.location = "EventLogs/Download?streamId=" + row.streamid;
                        };
                    }

                    function EventLogViewModel() {
                        var tblEventLog = $("#tblEventLog");

                        var self = this;

                        self.eventLogs = ko.observableArray([]);
                        self.selectedEventLog = ko.observable();
                        self.filenameSearch = ko.observable("");
                        self.totalEventLogs = ko.observable(0);

                        createEventLogArray(lists.EventLogList);

                        function createEventLogArray(list) {
                            self.eventLogs.removeAll();
                            $(list).each(function (index, value) {
                                pushEventLog(value);
                            });
                        };

                        self.columns = ko.computed(function () {
                            var arr = [];
                            arr.push({ title: "Name", sortable: true, sortKey: "filename", numeric: false, cssClass: "" });
                            arr.push({ title: "Path", sortable: true, sortKey: "path", numeric: false, cssClass: "" });
                            arr.push({ title: "Type", sortable: true, sortKey: "filetype", numeric: false, cssClass: "col-filetype" });
                            arr.push({ title: "Size", sortable: true, sortKey: "filesize", numeric: true, cssClass: "col-filesize" });
                            return arr;
                        });

                        function pushEventLog(value) {
                            self.eventLogs.push(new EventLog(value.StreamId, value.IsFolder, value.Filename, value.FileType, value.FileSize, value.Path));
                        };

                        self.selectedEventLog(self.eventLogs()[0]);

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
                                    self.eventLogs.sort(function (a, b) {
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

                        self.filteredEventLogs = ko.computed(function () {
                            return ko.utils.arrayFilter(self.eventLogs(), function (g) {
                                return (
                                    (self.filenameSearch().length === 0 || g.filename.toLowerCase().indexOf(self.filenameSearch().toLowerCase()) !== -1)
                                );
                            });
                        });

                        self.eventLogsTable = ko.computed(function () {
                            var filteredEventLogs = self.filteredEventLogs();
                            self.totalEventLogs(filteredEventLogs.length);

                            return filteredEventLogs;
                        });

                        self.highlightRow = function (row) {
                            var r = tblEventLog.find("#row_" + row.streamid);
                            $(r).addClass("highlight").siblings().removeClass("highlight");
                        };
                    };
            });
        }
    }
})(jQuery);