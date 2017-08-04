/*!
 * 1.0 Keebee AAT Copyright © 2016
 * SharedLibrary/_LinkedProfiles.js
 * Author: John Charlton
 * Date: 2017-07
 */

;
(function ($) {
    sharedlibrary.linkedprofiles = {
        init: function (options) {

            var config = {
                streamId: 0
            };

            $.extend(config, options);

            var cmdClose = $("#btnClose");

            var lists = {
                Profiles: []
            };

            utilities.job.execute({
                url: "SharedLibrary/GetDataLinkedProfiles",
                params: {
                    streamId: config.streamId
                }
            })
            .then(function (data) {
                $.extend(lists, data);

                $("#linked-error-container").hide();
                $("#linked-loading-container").hide();
                $("#linked-table-header").show();
                $("#linked-table-detail").show();

                ko.bindingHandlers.tableRender = {
                    update: function (element, valueAccessor) {
                        ko.utils.unwrapObservable(valueAccessor());

                        // if there are no rows in the table, hide the table and display a message
                        var table = element.parentNode;
                        var noRowsMessage = $("#linked-no-rows-message");

                        var tableDetailElement = $("#linked-table-detail");
                        var tableHeaderElement = $("#linked-table-header");

                        if (table.rows.length > 0) {
                            tableHeaderElement.show();
                            tableDetailElement.show();
                            noRowsMessage.hide();

                            // determine if there is table overflow (to cause a scrollbar)
                            if (table.clientHeight > site.getMaxClientHeight) {
                                tableDetailElement.addClass("container-height");
                            } else {
                                tableDetailElement.removeClass("container-height");
                            }

                        } else {
                            tableHeaderElement.hide();
                            tableDetailElement.hide();
                            noRowsMessage.html("<h2>No linked profiles</h2>");
                            noRowsMessage.show();
                        }
                    }
                }

                ko.applyBindings(new LinkedProfileViewModel(), document.getElementById("linked-profiles"));

                function LinkedProfileViewModel() {
                    var self = this;

                    self.profiles = ko.observableArray([]);
                    self.totalProfiles = ko.observableArray(0);

                    createProfileArray(lists.Profiles);

                    function createProfileArray(list) {
                        self.profiles.removeAll();
                        $(list)
                            .each(function (index, value) {
                                var name = value.FirstName;
                                if (value.LastName !== null) {
                                    name = name + " " + value.LastName;
                                }
                                self.profiles.push({
                                    name: name,
                                    profilepicture: value.ProfilePicture,
                                    profilepictureplaceholder: value.ProfilePicturePlaceholder
                                });
                            });
                    };

                    self.linkedcolumns = ko.computed(function () {
                        var arr = [];
                        arr.push({ title: "Name", sortKey: "name", cssClass: "col-profilename" });
                        return arr;
                    });

                    self.profilesTable = ko.computed(function () {
                        self.totalProfiles(self.profiles().length);

                        return self.profiles();
                    });
                };
            })
            .catch(function (error) {
                $("#linked-loading-container").hide();
                $("#linked-error-container")
                    .html("<div><h2>Data load error:</h2></div>")
                    .append("<div>" + error.message + "</div>")
                    .append("<div><h3>Please try refreshing the page</h3></div>");
                $("#linked-error-container").show();
            });
        }
    }
})(jQuery);