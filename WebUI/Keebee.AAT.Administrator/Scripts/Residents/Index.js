/*!
 * 1.0 Keebee AAT Copyright © 2016
 * Residents/Index.js
 * Author: John Charlton
 * Date: 2016-08
 */

; (function ($) {

    var HIGHLIGHT_ROW_COLOUR = "#e3e8ff";

    residents.index = {
        init: function (values) {
            var _currentSortKey = "id";
            var _sortDescending = false;

            var config = {
                selectedid: 0,
                rfid: "",
                firstname: "",
                lastname: "",
                sortcolumn: "",
                sortdescending: 0
            }

            $.extend(config, values);

            if (config.sortcolumn.length > 0)
                _currentSortKey = config.sortcolumn;

            var lists = {
                ResidentList: []
            };

            loadData();

            ko.applyBindings(new ResidentViewModel());

            // pre-select the resident whose media was just being managed
            if (parseInt(config.selectedid) > 0) { $("#row_" + config.selectedid).trigger("click"); }

            // pre-sort the list after media was managed
            _sortDescending = (config.sortdescending === "0");
            if (config.sortcolumn.length > 0)
                $("#resident-col-" + config.sortcolumn).trigger("click");

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

            function Resident(id, firstname, lastname, gender, gamedifficultylevel, allowvideocapturing, hasprofile, datecreated, dateupdated) {
                var self = this;

                self.id = id;
                self.firstname = firstname;
                self.lastname = lastname;
                self.gender = gender;
                self.gamedifficultylevel = gamedifficultylevel;
                self.allowvideocapturing = allowvideocapturing;
                self.hasprofile = hasprofile;
                self.datecreated = datecreated;
                self.dateupdated = dateupdated;
            }

            function ResidentViewModel() {
                var tblResident = $("#tblResident");

                var self = this;

                self.residents = ko.observableArray([]);
                self.selectedResident = ko.observable();
                self.rfidSearch = ko.observable(config.rfid);
                self.firstNameSearch = ko.observable(config.firstname);
                self.lastNameSearch = ko.observable(config.lastname);

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
                    arr.push({ title: "RFID", sortable: true, sortKey: "id", numeric: true });
                    arr.push({ title: "First Name", sortable: true, sortKey: "firstname", numeric: false });
                    arr.push({ title: "Last Name", sortable: true, sortKey: "lastname", numeric: false });
                    arr.push({ title: "Gender", sortable: true, sortKey: "gender", numeric: false });
                    arr.push({ title: "Game Level", sortable: true, sortKey: "gamedifficultylevel", numeric: true });
                    arr.push({ title: "Capturable", sortable: true, sortKey: "allowvideocapturing", numeric: false });
                    arr.push({ title: "Created", sortable: true, sortKey: "datecreated", numeric: true });
                    arr.push({ title: "Updated", sortable: true, sortKey: "dateupdated", numeric: true });
                    return arr;
                });

                function pushResident(value) {
                    self.residents.push(new Resident(value.Id, value.FirstName, value.LastName, value.Gender, value.GameDifficultyLevel, value.AllowVideoCapturing, value.HasProfile, value.DateCreated, value.DateUpdated));
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
                                if (value.numeric) {
                                    if (_sortDescending) {
                                          return a[sortKey]> b[sortKey]
                                                ? -1: a[sortKey]< b[sortKey]|| a.filename > b.filename ? 1: 0;
                                                } else {
                                            return a[sortKey] < b[sortKey]
                                                ? -1: a[sortKey]> b[sortKey]|| a.filename > b.filename ? 1: 0;
                                        }
                                    } else {
                                        if(_sortDescending) {
                                            return a[sortKey].toString().toLowerCase() > b[sortKey].toString().toLowerCase()
                                                ? -1: a[sortKey].toString().toLowerCase() < b[sortKey].toString().toLowerCase()
                                                || a.firstname.toLowerCase() > b.firstname.toLowerCase() ? 1 : 0;
                                            } else {
                                            return a[sortKey].toString().toLowerCase() < b[sortKey].toString().toLowerCase()
                                                ? -1: a[sortKey].toString().toLowerCase() > b[sortKey].toString().toLowerCase()
                                                || a.firstname.toLowerCase() > b.firstname.toLowerCase() ? 1 : 0;
                                        }
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

                self.editMedia = function (row) {
                    var id = row.id;

                    var sortdescending = 0;
                    if (_sortDescending) sortdescending = "1";

                    if (id > 0) {
                        window.location = site.url + "Residents/Media/" + id
                            + "?rfid=" + self.rfidSearch()
                            + "&firstname=" + self.firstNameSearch()
                            + "&lastname=" + self.lastNameSearch()
                            + "&sortcolumn=" + _currentSortKey
                            + "&sortdescending=" + sortdescending;
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
                                    self.deleteResident(row.id);
                                    dialog.close();
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
                    var allowVideoCapturing = $.trim($("#chkAllowVideoCapturing").is(":checked"));

                    return {
                        Id: self.selectedResident().id, FirstName: firstname, LastName: lastname, Gender: gender, GameDifficultyLevel: gamedifficultylevel, AllowVideoCapturing: allowVideoCapturing
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

                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_INFO,
                        title: "Resident",
                        message: "Deleting resident...",
                        closable: false,
                        onshown: function(dialog) {
                            $.ajax({
                                type: "POST",
                                async: true,
                                traditional: true,
                                url: site.url + "Residents/Delete/",
                                data: { id: id },
                                dataType: "json",
                                success: function (data) {

                                    dialog.close();
                                    $("body").css("cursor", "default");
                                    if (data.Success) {
                                        lists.ResidentList = data.ResidentList;
                                        createResidentArray(lists.ResidentList);
                                        self.sort({ afterSave: true });
                                    } else {
                                        BootstrapDialog.show({
                                            type: BootstrapDialog.TYPE_DANGER,
                                            title: "Delete Error",
                                            message: data.ErrorMessage
                                        });
                                    }
                                },
                                error: function (data) {
                                    dialog.close();
                                    $("body").css("cursor", "default");
                                    BootstrapDialog.show({
                                        type: BootstrapDialog.TYPE_DANGER,
                                        title: "Delete Error",
                                        message: "Unexpected Error\n" + data
                                    });
                                }
                            });
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