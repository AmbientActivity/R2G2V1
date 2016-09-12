/*!
 * EventLogs/Index.js
 * Author: John Charlton
 * Date: 2015-05
 */

; (function ($) {

    var HIGHLIGHT_ROW_COLOUR = "#e3e8ff";

    eventlogs.index = {
        init: function () {
            var _sortDescending = false;
            var _currentSortKey = "filename";

            var lists = {
                EventLogList: []
            };

            loadConfig();

            ko.applyBindings(new EventLogViewModel());

            function loadConfig() {
                $.ajax({
                    type: "GET",
                    url: site.url + "EventLogs/GetData/",
                    dataType: "json",
                    traditional: true,
                    async: false,
                    success: function (data) {
                        $.extend(lists, data);
                    }
                });
            }

            function EventLog(streamid, isfolder, filename, filetype, filesize, path) {
                var self = this;

                self.streamid = streamid;
                self.isfolder = isfolder;
                self.filename = filename;
                self.filetype = filetype;
                self.filesize = filesize;
                self.path = path;

                self.downloadUrl = "EventLogs/Download?streamId=" + streamid;
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
                    arr.push({ title: "Name", sortable: true, sortKey: "filename" });
                    arr.push({ title: "Path", sortable: true, sortKey: "path" });
                    arr.push({ title: "Type", sortable: true, sortKey: "filetype" });
                    arr.push({ title: "Size", sortable: true, sortKey: "filesize" });
                    return arr;
                });

                function pushEventLog(value) {
                    self.eventLogs.push(new EventLog(value.StreamId, value.IsFolder, value.Filename, value.FileType, value.FileSize, value.Path));
                };

                self.selectedEventLog(self.eventLogs()[0]);

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
                            self.eventLogs.sort(function (a, b) {
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

                self.getEventLog = function (streamid) {
                    var mediafile = null;

                    ko.utils.arrayForEach(self.eventLogs(), function (item) {
                        if (item.streamid === streamid) {
                            mediafile = item;
                        }
                    });

                    return mediafile;
                };

                self.highlightRow = function (row) {
                    if (row == null) return;

                    var rows = tblEventLog.find("tr:gt(0)");
                    rows.each(function () {
                        $(this).css("background-color", "#ffffff");
                    });

                    var r = tblEventLog.find("#row_" + row.streamid);
                    r.css("background-color", HIGHLIGHT_ROW_COLOUR);
                    $("#tblEventLog").attr("tr:hover", HIGHLIGHT_ROW_COLOUR);
                };

                self.download = function(row) {
                        $.ajax({
                            type: "GET",
                            url: site.url + "EventLogs/Download?streamId=" + row.streamid,
                            dataType: "json",
                            traditional: true,
                            async: false,
                            success: function (data) {
                                var d = data;
                            }
                        });
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