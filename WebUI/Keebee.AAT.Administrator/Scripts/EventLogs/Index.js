/*!
 * 1.0 Keebee AAT Copyright © 2016
 * EventLogs/Index.js
 * Author: John Charlton
 * Date: 2016-08
 */

; (function ($) {
    eventlogs.index = {
        init: function () {
            var sortDescending = true;
            var currentSortKey = "filename";
            var primarySortKey = "filename";
            var isBinding = true;

            var lists = {
                EventLogList: []
            };

            utilities.job.execute({
                url: site.url + "EventLogs/GetData/"
            })
            .then(function (data) {
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
                        var noRowsMessage = $("#no-rows-message");

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
                                colScrollbar.attr("style", "width: 1%; border-bottom: 1.5px solid #ddd;");
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
                        arr.push({ title: "Filename", sortable: true, sortKey: "filename", numeric: false, cssClass: "col-filename-el" });
                        arr.push({ title: "Type", sortable: true, sortKey: "filetype", numeric: false, cssClass: "col-filetype" });
                        arr.push({ title: "Path", sortable: true, sortKey: "path", numeric: false, cssClass: "col-path" });
                        arr.push({ title: "Size", sortable: true, sortKey: "filesize", numeric: true, cssClass: "col-filesize" });
                        return arr;
                    });

                    function pushEventLog(value) {
                        self.eventLogs.push(new EventLog(value.StreamId, value.IsFolder, value.Filename, value.FileType, value.FileSize, value.Path));
                    };

                    self.selectedEventLog(self.eventLogs()[0]);

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
                        self.eventLogs(utilities.sorting.sortArray(
                            {
                                fileArray: self.eventLogs(),
                                columns: self.columns(),
                                sortKey: sortKey,
                                primarySortKey: primarySortKey,
                                descending: sortDescending,
                                boolean: isboolean
                            }));
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