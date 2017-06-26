/*!
 * 1.0 Keebee AAT Copyright © 2016
 * Residents/Index.js
 * Author: John Charlton
 * Date: 2016-08
 */

; (function ($) {
    residents.index = {
        init: function (values) {
            var currentSortKey = "id";
            var sortDescending = false;

            var config = {
                selectedid: 0,
                idsearch: "",
                firstname: "",
                lastname: "",
                sortcolumn: "",
                sortdescending: 0,
                isVideoCaptureServiceInstalled: 0
            }

            $.extend(config, values);

            if (config.sortcolumn.length > 0)
                currentSortKey = config.sortcolumn;

            var lists = {
                ResidentList: []
            };

            $.get(site.url + "Residents/GetData/")
                .done(function (data) {
                    $.extend(lists, data);

                    ko.applyBindings(new ResidentViewModel());

                    // pre-select the resident whose media was just being managed
                    if (parseInt(config.selectedid) > 0) { $("#row_" + config.selectedid).trigger("click"); }

                    // pre-sort the list after media was managed
                    sortDescending = (config.sortdescending === "0");
                    if (config.sortcolumn.length > 0)
                        $("#resident-col-" + config.sortcolumn).trigger("click");

                    function Resident(id, firstname, lastname, gender, gamedifficultylevel, allowvideocapturing, profilepicture, profilepictureplaceholder, dateupdated) {
                        var self = this;

                        self.id = id;
                        self.firstname = firstname;
                        self.lastname = lastname;
                        self.gender = gender;
                        self.allowvideocapturing = allowvideocapturing;
                        self.profilepicture = profilepicture;
                        self.profilepictureplaceholder = profilepictureplaceholder;
                        self.dateupdated = dateupdated;
                    }

                    function ResidentViewModel() {
                        var tblResident = $("#tblResident");

                        var self = this;

                        self.residents = ko.observableArray([]);
                        self.selectedResident = ko.observable();
                        self.idSearch = ko.observable(config.idsearch);
                        self.firstNameSearch = ko.observable(config.firstname);
                        self.lastNameSearch = ko.observable(config.lastname);
                        self.isVideoCaptureServiceInstalled = config.isVideoCaptureServiceInstalled;
                        self.profilePicture = config.ProfilePicture;

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
                            arr.push({ title: "ID", sortable: true, sortKey: "id", numeric: true, cssClass: "col-id" });
                            arr.push({ title: "First Name", sortable: true, sortKey: "firstname", numeric: false, cssClass: "col-firstname" });
                            arr.push({ title: "Last Name", sortable: true, sortKey: "lastname", numeric: false, cssClass: "col-lastname" });
                            arr.push({ title: "Gender", sortable: true, sortKey: "gender", numeric: false, cssClass: "col-gender" });

                            if (config.isVideoCaptureServiceInstalled === "1")
                                arr.push({ title: "Capturable", sortable: true, sortKey: "allowvideocapturing", numeric: false, cssClass: "col-capturable" });

                            arr.push({ title: "Updated", sortable: true, sortKey: "dateupdated", numeric: true, cssClass: "col-date" });
                            return arr;
                        });

                        function pushResident(value) {
                            self.residents.push(new Resident(value.Id, value.FirstName, value.LastName, value.Gender, value.GameDifficultyLevel, value.AllowVideoCapturing, value.ProfilePicture, value.ProfilePicturePlaceholder, value.DateUpdated));
                        };

                        self.selectedResident(self.residents()[0]);

                        self.sort = function (header) {
                            var afterSave = typeof header.afterSave != "undefined" ? header.afterSave : false;
                            var sortKey;

                            if (!afterSave) {
                                sortKey = header.sortKey;

                                if (sortKey !== currentSortKey) {
                                    sortDescending = false;
                                } else {
                                    sortDescending = !sortDescending;
                                }
                                currentSortKey = sortKey;
                            } else {
                                sortKey = currentSortKey;
                            }

                            $(self.columns()).each(function (index, value) {
                                if (value.sortKey === sortKey) {
                                    self.residents.sort(function (a, b) {
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
                                                    || a.firstname.toLowerCase() > b.firstname.toLowerCase() ? 1 : 0;
                                            } else {
                                                return a[sortKey].toString().toLowerCase() < b[sortKey].toString().toLowerCase()
                                                    ? -1 : a[sortKey].toString().toLowerCase() > b[sortKey].toString().toLowerCase()
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
                                    (self.idSearch().length === 0 || r.id.toString().indexOf(self.idSearch().toString()) !== -1)
                                );
                            });
                        });

                        self.residentsTable = ko.computed(function () {
                            var filteredResidents = self.filteredResidents();
                            self.totalResidents(filteredResidents.length);

                            return filteredResidents;
                        });

                        self.showEditDialog = function (row) {
                            var id = (typeof row.id !== "undefined" ? row.id : 0);
                            var title = "<span class='glyphicon glyphicon-pencil' style='color: #fff'></span>";

                            if (id > 0) {
                                self.highlightRow(row);
                            }

                            if (id > 0) {
                                title = title + " Edit Resident";
                                var resident = self.getResident(id);
                                self.selectedResident(resident);
                            } else {
                                title = title + " Add Resident";
                                self.selectedResident([]);
                            }

                            $.get(site.url + "Residents/GetResidentEditView/" + id)
                                .done(function (message) {
                                    BootstrapDialog.show({
                                        title: title,
                                        message: $("<div></div>").append(message),
                                        onshown: function () {
                                            $("#txtFirstName").focus();
                                        },
                                        closable: false,
                                        buttons: [
                                            {
                                                label: "Cancel",
                                                action: function (dialog) {
                                                    dialog.close();
                                                }
                                            },{
                                                 label: "Save",
                                                 cssClass: "btn-primary",
                                                 action: function (dialog) {
                                                     self.saveResident().then(function (result) {
                                                         if (result.ValidationMessages === null) {
                                                             lists.ResidentList = result.ResidentList;
                                                             createResidentArray(lists.ResidentList);
                                                             self.selectedResident(self.getResident(result.SelectedId));
                                                             self.sort({ afterSave: true });
                                                             self.highlightRow(self.selectedResident());
                                                             dialog.close();
                                                         } else {
                                                             $("#validation-container").show();
                                                             $("#validation-container").html("");
                                                             var html = "<ul>";
                                                             for (var i = 0; i < result.ValidationMessages.length; i++) {
                                                                 var msg = result.ValidationMessages[i];
                                                                 html = html + "<li>" + msg + "</li>";
                                                             }
                                                             html = html + "</ul>";
                                                             $("#validation-container").append(html);
                                                         }
                                                     });
                                                 }
                                             }
                                        ]
                                    });
                            });                         
                        };

                        self.editProfile = function (row) {
                            var id = row.id;

                            var sortdescending = 0;
                            if (sortDescending) sortdescending = "1";

                            if (id > 0) {
                                window.location = site.url + "ResidentProfile?id=" + id
                                    + "&idsearch=" + self.idSearch()
                                    + "&firstname=" + self.firstNameSearch()
                                    + "&lastname=" + self.lastNameSearch()
                                    + "&sortcolumn=" + currentSortKey
                                    + "&sortdescending=" + sortdescending;
                            }
                        };

                        self.showDeleteDialog = function (row) {
                            return new Promise(function(resolve, reject) {
                                self.highlightRow(row);

                                var id = (typeof row.id !== "undefined" ? row.id : 0);
                                if (id <= 0) return false;
                                var r = self.getResident(id);
                                var messageGender;

                                if (r.gender === "M") messageGender = "his";
                                else messageGender = "her";

                                var fullName = r.firstname;
                                if (r.lastname != null) fullName = fullName + " " + r.lastname;

                                BootstrapDialog.show({
                                    type: BootstrapDialog.TYPE_DANGER,
                                    title: "Delete Resident?",
                                    message: "Permanently delete the resident <i><b>" + fullName + "</b></i>?\n\n" +
                                        "<b>Warning:</b> All " + messageGender + " personal media files will be removed!",
                                    closable: false,
                                    buttons: [
                                        {
                                            label: "Cancel",
                                            action: function (dialog) {
                                                dialog.close();
                                                reject();
                                            }
                                        }, {
                                            label: "Yes, Delete",
                                            cssClass: "btn-danger",
                                            action: function (dialog) {
                                                utilities.job.execute(
                                                {
                                                    controller: "Residents",
                                                    action: "Delete",
                                                    type: "POST",
                                                    params: { id: id },
                                                    title: "Delete Resident"
                                                })
                                                .then(function (result) {
                                                    dialog.close();
                                                    lists.ResidentList = result.ResidentList;
                                                    createResidentArray(lists.ResidentList);
                                                    self.sort({ afterSave: true });
                                                    resolve(result);
                                                })
                                                .catch(function () {
                                                    dialog.close();
                                                    reject();
                                                });
                                            }
                                        }
                                    ]
                                });
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

                        self.showTooltipProfile = function (row) {
                            var p = tblResident.find("#profile_" + row.id);
                            p.tooltip();
                        };

                        self.showTooltipEdit = function (row) {
                            var e = tblResident.find("#edit_" + row.id);
                            e.tooltip();
                        };

                        self.showTooltipDelete = function (row) {
                            var e = tblResident.find("#delete_" + row.id);
                            e.addClass("text-danger");
                            e.tooltip();
                        };

                        self.highlightRow = function (row) {
                            var r = tblResident.find("#row_" + row.id);
                            $(r).addClass("highlight").siblings().removeClass("highlight");
                        };

                        self.getResidentDetailFromDialog = function () {
                            var firstname = $.trim($("#txtFirstName").val());
                            var lastname = $.trim($("#txtLastName").val());
                            var gender = $.trim($("#ddlGenders").val());
                            var gamedifficultylevel = $.trim($("#ddlGameDifficultyLevels").val());
                            var allowVideoCapturing = $.trim($("#chkAllowVideoCapturing").is(":checked"));
                            var profilePicture = null;

                            if ($("#profile-picture").attr("alt") === "exists") {
                                profilePicture = $("#profile-picture").attr("src").replace("data:image/jpg;base64,", "");
                            }

                            return {
                                Id: self.selectedResident().id, FirstName: firstname, LastName: lastname, Gender: gender, GameDifficultyLevel: gamedifficultylevel, AllowVideoCapturing: allowVideoCapturing, ProfilePicture: profilePicture
                            };
                        };

                        self.saveResident = function () {
                            return new Promise(function(resolve, reject) {
                                var residentdetail = self.getResidentDetailFromDialog();
                                var jsonData = JSON.stringify(residentdetail);

                                utilities.job.execute(
                                {
                                    type: "POST",
                                    controller: "Residents",
                                    action: "Save",
                                    title: "Save Resident",
                                    params: { resident: jsonData }
                                })
                                .then(function(result) {
                                    resolve(result);
                                })
                                .catch(function(result) {
                                    reject(result);
                                });
                            });
                        };
                    };
            });       
        }
    }
})(jQuery);