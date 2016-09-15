﻿/*!
 * Residents/Index.js
 * Author: John Charlton
 * Date: 2015-05
 */

; (function ($) {

    var HIGHLIGHT_ROW_COLOUR = "#e3e8ff";

    residents.index = {
        init: function () {
            var _sortDescending = false;
            var _currentSortKey = "id";

            var lists = {
                ResidentList: []
            };

            loadData();

            ko.applyBindings(new ResidentViewModel());

            function loadData() {
                $.ajax({
                    type: "GET",
                    url: site.url + "Residents/GetData/",
                    dataType: "json",
                    traditional: true,
                    async: false,
                    success: function (data) {
                        $.extend(lists, data);
                    }
                });
            }

            function Resident(id, profileid, firstname, lastname, gender, gamedifficultylevel, hasprofile, datecreated, dateupdated) {
                var self = this;

                self.id = id;
                self.profileid = profileid;
                self.firstname = firstname;
                self.lastname = lastname;
                self.gender = gender;
                self.gamedifficultylevel = gamedifficultylevel;
                self.hasprofile = hasprofile;
                self.datecreated = datecreated;
                self.dateupdated = dateupdated;
            }

            function ResidentViewModel() {
                var tblResident = $("#tblResident");

                var self = this;

                self.residents = ko.observableArray([]);
                self.selectedResident = ko.observable();
                self.firstNameSearch = ko.observable("");
                self.lastNameSearch = ko.observable("");
                self.rfidSearch = ko.observable("");
                self.totalResidents = ko.observable(0);

                createResidentArray(lists.ResidentList);

                function createResidentArray(list) {
                    self.residents.removeAll();
                    $(list).each(function (index, value) {
                        pushResident(value);
                    });
                };
                
                self.columns = ko.computed(function () {
                    var arr = [];
                    arr.push({ title: "Profile", sortable: false  });
                    arr.push({ title: "RFID", sortable: true, sortKey: "id" });
                    arr.push({ title: "First Name", sortable: true, sortKey: "firstname" });
                    arr.push({ title: "Last Name", sortable: true, sortKey: "lastname" });
                    arr.push({ title: "Gender", sortable: true, sortKey: "gender" });
                    arr.push({ title: "Game Difficulty", sortable: true, sortKey: "gamedifficultylevel" });
                    arr.push({ title: "Created", sortable: true, sortKey: "datecreated" });
                    arr.push({ title: "Updated", sortable: true, sortKey: "dateupdated" });
                    return arr;
                });

                function pushResident(value) {
                    self.residents.push(new Resident(value.Id, value.ProfileId, value.FirstName, value.LastName, value.Gender, value.GameDifficultyLevel, value.HasProfile, value.DateCreated, value.DateUpdated));
                };

                self.selectedResident(self.residents()[0]);

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
                            self.residents.sort(function (a, b) {
                                if (_sortDescending) {
                                    return a[sortKey].toString().toLowerCase() > b[sortKey].toString().toLowerCase()
                                        ? -1 : a[sortKey].toString().toLowerCase() < b[sortKey].toString().toLowerCase()
                                        || a.lastname.toLowerCase() > b.lastname.toLowerCase() ? 1 : 0;
                                } else {
                                    return a[sortKey].toString().toLowerCase() < b[sortKey].toString().toLowerCase()
                                        ? -1 : a[sortKey].toString().toLowerCase() > b[sortKey].toString().toLowerCase()
                                        || a.lastname.toLowerCase() > b.lastname.toLowerCase() ? 1 : 0;
                                }
                            });
                        }
                    });
                };

                self.filteredResidents = ko.computed(function () {
                    return ko.utils.arrayFilter(self.residents(), function (r) {
                        return (
                            (self.firstNameSearch().length === 0 || r.firstname.toLowerCase().indexOf(self.firstNameSearch().toLowerCase()) !== -1)
                            &&
                            (self.lastNameSearch().length === 0 || r.lastname.toLowerCase().indexOf(self.lastNameSearch().toLowerCase()) !== -1)
                            &&
                            (self.rfidSearch().length === 0 || r.id.toString().indexOf(self.rfidSearch().toString()) !== -1)
                        );
                    });
                });

                self.residentsTable = ko.computed(function () {
                    var filteredResidents = self.filteredResidents();
                    self.totalResidents(filteredResidents.length);

                    return filteredResidents;
                });

                self.showEditDialog = function (row) {
                    var id = row.id;

                    if (id > 0) {
                        self.highlightRow(row);
                    }

                    self.showResidentEditDialog(row);
                };

                self.editProfile = function (row) {
                    var id = row.profileid;

                    if (id > 0) {
                        window.location = site.url + "Profiles/Edit/" + id;
                    }
                };

                self.deleteSelectedResident = function (row) {
                    deleteResident(row.id);
                };

                self.showDeleteDialog = function (row) {
                    self.highlightRow(row);
                    self.showResidentDeleteDialog(row);
                };

                self.showResidentDeleteDialog = function (row) {
                    var id = (typeof row.id !== "undefined" ? row.id : 0);
                    if (id <= 0) return;
                    var r = self.getResident(id);
                    var messageGender;

                    if (r.gender === "M") messageGender = "his";
                    else messageGender = "her";

                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_DANGER,
                        title: "Delete Resident?",
                        message: "Permanently delete the resident <i><b>" + r.firstname + " " + r.lastname + "</b></i>?\n\n" +
                            "<b>Warning:</b> All " + messageGender + " personal media files will be removed!",
                        closable: false,
                        buttons: [
                            {
                                label: "Cancel",
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }, {
                                label: "Yes, Delete",
                                cssClass: "btn-danger",
                                action: function (dialog) {
                                    var result = self.deleteResident(row.id);
                                    lists.ResidentList = result.ResidentList;
                                    createResidentArray(lists.ResidentList);
                                    self.sort({ afterSave: true });
                                    dialog.close();
                                    $("body").css("cursor", "default");
                                }
                            }
                        ]
                    });
                };

                self.showResidentEditDialog = function (row) {
                    var id = (typeof row.id !== "undefined" ? row.id : 0);
                    var title = "<span class='glyphicon glyphicon-pencil'></span>";
                    var message;

                    if (id > 0) {
                        title = title + " Edit Resident";
                        var resident = self.getResident(id);
                        self.selectedResident(resident);
                    } else {
                        title = title + " Add Resident";
                        self.selectedResident([]);
                    }

                    $.ajax({
                        type: "GET",
                        async: false,
                        url: site.url + "Residents/GetResidentEditView/" + id,
                        success: function (data) {
                            message = data;
                        }
                    });

                    BootstrapDialog.show({
                        title: title,
                        message: message,
                        onshown: function() {
                            $("#txtFirstName").focus();
                        },
                        closable: false,
                        buttons: [
                            {
                                label: "Cancel",
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }, {
                                label: "OK",
                                cssClass: "btn-primary",
                                action: function (dialog) {
                                    var result = self.saveResident();
                                    
                                    if (result.ErrorMessages === null) {
                                        lists.ResidentList = result.ResidentList;
                                        createResidentArray(lists.ResidentList);
                                        self.selectedResident(self.getResident(result.SelectedId));
                                        self.sort({ afterSave: true });
                                        self.highlightRow(self.selectedResident());
                                        dialog.close();
                                        $("body").css("cursor", "default");
                                    } else {
                                        $("#validation-container").show();
                                        $("#validation-container").html("");
                                        $("body").css("cursor", "default");
                                        var html = "<ul>";
                                        for (var i = 0; i < result.ErrorMessages.length; i++) {
                                            var message = result.ErrorMessages[i];
                                            html = html + "<li>" + message + "</li>";
                                        }
                                        html = html + "</ul>";
                                        $("#validation-container").append(html);
                                        $("body").css("cursor", "default");
                                    }
                                }
                            }
                        ]
                    });
                };

                self.getResident = function (residentid) {
                    var resident = null;

                    ko.utils.arrayForEach(self.residents(), function (item) {
                        if (item.id === residentid) {
                            resident = item;
                        }
                    });

                    return resident;
                };

                self.highlightRow = function (row) {
                    if (row == null) return;

                    var rows = tblResident.find("tr:gt(0)");
                    rows.each(function () {
                        $(this).css("background-color", "#ffffff");
                    });

                    var r = tblResident.find("#row_" + row.id);
                    r.css("background-color", HIGHLIGHT_ROW_COLOUR);
                    tblResident.attr("tr:hover", HIGHLIGHT_ROW_COLOUR);
                };

                self.getResidentDetailFromDialog = function () {
                    var firstname = $.trim($("#txtFirstName").val());
                    var lastname = $.trim($("#txtLastName").val());
                    var gender = $.trim($("#ddlGenders").val());
                    var gamedifficultylevel = $.trim($("#ddlGameDifficultyLevels").val());

                    return {
                        Id: self.selectedResident().id, FirstName: firstname, LastName: lastname, Gender: gender, GameDifficultyLevel: gamedifficultylevel
                    };
                };

                self.saveResident = function () {
                    var residentdetail = self.getResidentDetailFromDialog();
                    var jsonData = JSON.stringify(residentdetail);
                    var result;

                    $("body").css("cursor", "wait");

                    $.ajax({
                        type: "POST",
                        async: false,
                        url: site.url + "Residents/Save/",
                        data: { resident: jsonData },
                        dataType: "json",
                        traditional: true,
                        failure: function () {
                            $("body").css("cursor", "default");
                            $("#validation-container").html("");
                        },
                        success: function (data) {
                            result = data;
                        },
                        error: function (data) {
                            result = data;
                        }
                    });

                    return result;
                };

                self.deleteResident = function (id) {
                    $("body").css("cursor", "wait");

                    var result;

                    $.ajax({
                        type: "POST",
                        async: false,
                        url: site.url + "Residents/Delete/",
                        data: { id: id },
                        dataType: "json",
                        traditional: true,
                        failure: function () {
                            $("body").css("cursor", "default");
                            $("#validation-container").html("");
                        },
                        success: function (data) {
                            result = data;
                        },
                        error: function (data) {
                            result = data;
                        }
                    });

                    return result;
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