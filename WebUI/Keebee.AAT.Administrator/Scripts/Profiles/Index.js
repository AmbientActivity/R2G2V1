/*!
 * Profiles/Index.js
 * Author: John Charlton
 * Date: 2015-05
 */

 ; (function ($) {

    var HIGHLIGHT_ROW_COLOUR = "#e3e8ff";

    profiles.index = {
        init: function () {
            var _sortDescending = false;
            var _currentSortKey = "description";

            var lists = {
                ProfileList: [],
                ResidentArrayList: []
            };

            loadConfig();

            ko.applyBindings(new ProfileViewModel());

            function loadConfig() {
                $.ajax({
                    type: "GET",
                    url: site.url + "Profiles/GetData/",
                    dataType: "json",
                    traditional: true,
                    async: false,
                    success: function (data) {
                        $.extend(lists, data);
                    }
                });
            }

            function Profile(id, description, resident, gamedifficultylevel, datecreated, dateupdated) {
                var self = this;

                self.id = id;
                self.description = description;
                self.resident = getValue(lists.ResidentArrayList, resident, "Display", "Value");
                self.gamedifficultylevel = gamedifficultylevel;
                self.datecreated = datecreated;
                self.dateupdated = dateupdated;
            }

            function ProfileViewModel() {
                var tblProfile = $("#tblProfile");

                var self = this;

                self.profiles = ko.observableArray([]);
                self.selectedProfile = ko.observable();
                self.residentSearch = ko.observable("");
                self.totalProfiles = ko.observable(0);

                createProfileArray(lists.ProfileList);

                function createProfileArray(list) {
                    self.profiles.removeAll();
                    $(list).each(function (index, value) {
                        pushProfile(value);
                    });
                };

                function pushProfile(value) {
                    self.profiles.push(new Profile(value.Id, value.Description, value.Resident, value.GameDifficultyLevel, value.DateCreated, value.DateUpdated));
                };

                self.selectedProfile(self.profiles()[0]);

                self.sort = function (header) {
                    var afterSave = typeof header.afterSave != "undefined" ? header.afterSave : false;
                    var sortKey;

                    if (!afterSave) {
                        sortKey = header.sortKey;

                        if (sortKey !== _currentSortKey) {
                            _sortDescending = false;
                        } else {
                            _sortDescending = !_sortDescending;
                        }
                        _currentSortKey = sortKey;
                    } else {
                        sortKey = _currentSortKey;
                    }

                    $(self.columns()).each(function (index, value) {
                        if (value.sortKey === sortKey) {
                            self.profiles.sort(function (a, b) {
                                if (_sortDescending) {
                                    return a[sortKey].toString().toLowerCase() > b[sortKey].toString().toLowerCase()
                                        ? -1 : a[sortKey].toString().toLowerCase() < b[sortKey].toString().toLowerCase()
                                        || a.resident.toLowerCase() > b.resident.toLowerCase() ? 1 : 0;
                                } else {
                                    return a[sortKey].toString().toLowerCase() < b[sortKey].toString().toLowerCase()
                                        ? -1 : a[sortKey].toString().toLowerCase() > b[sortKey].toString().toLowerCase()
                                        || a.resident.toLowerCase() > b.resident.toLowerCase() ? 1 : 0;
                                }
                            });
                        }
                    });
                };

                self.filteredProfiles = ko.computed(function () {
                    return ko.utils.arrayFilter(self.profiles(), function (g) {
                        return (
                            (self.residentSearch().length === 0 || g.resident.toLowerCase().indexOf(self.residentSearch().toLowerCase()) !== -1)
                        );
                    });
                });

                self.profilesTable = ko.computed(function () {
                    var filteredProfiles = self.filteredProfiles();
                    self.totalProfiles(filteredProfiles.length);

                    return filteredProfiles;
                });

                self.showEditDialog = function (row) {
                    var id = row.id;

                    if (id > 0) {
                        self.highlightRow(row);
                    }

                    self.showMProfileEditDialog(row);
                };

                self.deleteSelectedProfile = function (row) {
                    deleteProfile(row.id);
                };

                self.showDeleteDialog = function (row) {
                    self.highlightRow(row);
                    self.showProfileDeleteDialog(row);
                };

                self.showProfileDeleteDialog = function (row) {
                    var id = (typeof row.id !== "undefined" ? row.id : 0);
                    if (id <= 0) return;
                    var m = self.getProfile(id);

                    BootstrapDialog.show({
                        title: "Delete Profile?",
                        message: "Are you sure?" +
                            "This will permanently delete the profile for resident <i><b>" + m.resident + " " + m.lastname + "</b></i>\n",
                        closable: false,
                        buttons: [
                            {
                                label: "No",
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }, {
                                label: "Yes",
                                cssClass: "btn-primary",
                                action: self.deleteProfile(row.id)
                            }
                        ]
                    });
                };

                self.showProfileEditDialog = function (row) {
                    var id = (typeof row.id !== "undefined" ? row.id : 0);
                    var title = "<span class='glyphicon glyphicon-pencil'></span>";
                    var message;

                    if (id > 0) {
                        title = title + " Edit Profile";
                        var profile = self.getProfile(id);
                        self.selectedProfile(profile);
                    } else {
                        title = title + " Add Profile";
                        self.selectedProfile([]);
                    }

                    $.ajax({
                        type: "GET",
                        async: false,
                        url: site.url + "Profiles/GetProfileEditView/" + id,
                        success: function (data) {
                            message = data;
                        }
                    });

                    BootstrapDialog.show({
                        title: title,
                        message: message,
                        closable: false,
                        buttons: [
                            {
                                label: "No",
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }, {
                                label: "Yes",
                                cssClass: "btn-primary",
                                action: self.saveProfile()
                            }
                        ]
                    });
                };

                self.getProfile = function (profileid) {
                    var profile = null;

                    ko.utils.arrayForEach(self.profiles(), function (item) {
                        if (item.id === profileid) {
                            profile = item;
                        }
                    });

                    return profile;
                };

                self.highlightRow = function (row) {
                    if (row == null) return;

                    var rows = tblProfile.find("tr:gt(0)");
                    rows.each(function () {
                        $(this).css("background-color", "#ffffff");
                    });

                    var r = tblProfile.find("#row_" + row.id);
                    r.css("background-color", HIGHLIGHT_ROW_COLOUR);
                    $("#tblProfile").attr("tr:hover", HIGHLIGHT_ROW_COLOUR);
                };

                self.getProfileDetailFromDialog = function () {
                    var description = $.trim($("#txtDescription").val());
                    var gamedifficultylevel = $.trim($("#txtGameDifficultyLevel").val());
                    var residentid = $.trim($("#ddlProfileInstruments").val());

                    return {
                        Id: self.selectedProfile().id, Description: description, ResidentId: residentid, GameDifficultyLevel: gamedifficultylevel
                    };
                };

                //---------------------------------------------- CONTROLLER (BEGIN) -------------------------------------------------------

                self.saveProfile = function () {
                    var profiledetail = self.getProfileDetailFromDialog();
                    var jsonData = JSON.stringify(profiledetail);
                    var result;

                    $("body").css("cursor", "wait");

                    $.ajax({
                        type: "POST",
                        async: false,
                        url: site.url + "Profiles/Save/",
                        data: { profile: jsonData },
                        dataType: "json",
                        traditional: true,
                        failure: function () {
                            $("body").css("cursor", "default");
                            $("#validation-container").html("");
                        },
                        success: function (data) {
                            if (data.Success) {
                                lists.ProfileList = data.ProfileList;
                                createProfileArray(lists.ProfileList);
                                self.selectedProfile(self.getProfile(data.SelectedId));
                                self.sort({ afterSave: true });
                                self.highlightRow(self.selectedProfile());
                                result = true;
                            } else {
                                if (data.ErrorMessages.length > 0) {
                                    $("#validation-container").show();
                                    $("#validation-container").html("");
                                    $("body").css("cursor", "default");
                                    var html = "<ul>";
                                    for (var i = 0; i < data.ErrorMessages.length; i++) {
                                        var message = data.ErrorMessages[i];
                                        html = html + "<li>" + message + "</li>";
                                    }
                                    html = html + "</ul>";
                                    $("#validation-container").append(html);
                                }
                                result = false;
                            }
                            $("body").css("cursor", "default");
                        }
                    });

                    return result;
                };

                self.deleteProfile = function (id) {
                    $("body").css("cursor", "wait");

                    $.ajax({
                        type: "POST",
                        url: site.url + "Profiles/Delete/",
                        data: { id: id },
                        dataType: "json",
                        traditional: true,
                        failure: function () {
                            $("body").css("cursor", "default");
                        },
                        success: function (data) {
                            if (data.Success) {
                                lists.ProfileList = data.ProfileList;
                                createProfileArray(lists.ProfileList);
                                self.sort({ afterSave: true });
                            }
                            $("body").css("cursor", "default");
                        }
                    });

                    return true;
                };

                //---------------------------------------------- CONTROLLER (END) -------------------------------------------------------
            };

            //---------------------------------------------- VIEW MODEL (END) -----------------------------------------------------

            ko.utils.stringStartsWith = function (string, startsWith) {
                string = string || "";
                if (startsWith.length > string.length) return false;
                return string.substring(0, startsWith.length) === startsWith;
            };

            function getValue(list, id, dataProfile, valueProfile) {
                var name = "";
                $(list).each(function (index, item) {
                    if (item[valueProfile] === id) {
                        name = item[dataProfile];
                        return name;
                    }
                });
                return name;
            }
        }
    }
})(jQuery);