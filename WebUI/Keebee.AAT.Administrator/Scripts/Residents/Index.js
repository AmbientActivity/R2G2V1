/*!
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

            loadConfig();

            ko.applyBindings(new ResidentViewModel());

            function loadConfig() {
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

            function Resident(id, profileid, firstname, lastname, gender, datecreated, dateupdated) {
                var self = this;

                self.id = id;
                self.profileid = profileid;
                self.firstname = firstname;
                self.lastname = lastname;
                self.gender = gender;
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
                    arr.push({ title: "RFID", sortKey: "id" });
                    arr.push({ title: "First Name", sortKey: "firstname" });
                    arr.push({ title: "Last Name", sortKey: "lastname" });
                    arr.push({ title: "Gender", sortKey: "gender" });
                    arr.push({ title: "Created", sortKey: "datecreated" });
                    arr.push({ title: "Updated", sortKey: "dateupdated" });
                    return arr;
                });

                function pushResident(value) {
                    self.residents.push(new Resident(value.Id, value.ProfileId, value.FirstName, value.LastName, value.Gender, value.DateCreated, value.DateUpdated));
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
                        self.highlightRow(row);
                    }

                    //self.showResidentEditDialog(row);
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
                    var m = self.getResident(id);

                    BootstrapDialog.show({
                        title: "Delete Resident?",
                        message: "Are you sure?" +
                            "This will permanently delete the resident for resident <i><b>" + m.resident + " " + m.lastname + "</b></i>\n",
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
                                action: self.deleteResident(row.id)
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
                                    $("body").css("cursor", "wait");
                                    var result = self.saveResident();
                                    $("body").css("cursor", "default");
                                    if (result.ErrorMessages === null) {
                                        lists.ResidentList = result.ResidentList;
                                        createResidentArray(lists.ResidentList);
                                        self.selectedResident(self.getResident(result.SelectedId));
                                        self.sort({ afterSave: true });
                                        self.highlightRow(self.selectedResident());
                                        dialog.close();
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
                    $("#tblResident").attr("tr:hover", HIGHLIGHT_ROW_COLOUR);
                };

                self.getResidentDetailFromDialog = function () {
                    var firstname = $.trim($("#txtFirstName").val());
                    var lastname = $.trim($("#txtLastName").val());
                    var gender = $.trim($("#ddlGenders").val());

                    return {
                        Id: self.selectedResident().id, FirstName: firstname, LastName: lastname, Gender: gender
                    };
                };

                //---------------------------------------------- CONTROLLER (BEGIN) -------------------------------------------------------

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

                    $.ajax({
                        type: "POST",
                        url: site.url + "Residents/Delete/",
                        data: { id: id },
                        dataType: "json",
                        traditional: true,
                        failure: function () {
                            $("body").css("cursor", "default");
                        },
                        success: function (data) {
                            if (data.Success) {
                                lists.ResidentList = data.ResidentList;
                                createResidentArray(lists.ResidentList);
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
        }
    }
})(jQuery);