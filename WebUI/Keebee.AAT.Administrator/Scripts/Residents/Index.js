/*!
 * 1.0 Keebee AAT Copyright © 2016
 * Residents/Index.js
 * Author: John Charlton
 * Date: 2016-08
 */

; (function ($) {
    residents.index = {
        init: function (values) {

            var currentSortKey = "firstname";
            var primarySortKey = "firstname";
            var sortDescending = false;
            var isBinding = true;

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

                    $("#loading-container").hide();
                    $("#table-header").show();
                    $("#table-detail").show();
                    $("#add-resident").removeAttr("disabled");

                    ko.bindingHandlers.tableUpdated = {
                        update: function (element, valueAccessor, allBindings) {
                            ko.unwrap(valueAccessor());
                            $("#txtSearchFirstName").focus();
                            isBinding = false;
                        }
                    }

                    ko.bindingHandlers.tableRender = {
                        update: function (element, valueAccessor) {
                            ko.utils.unwrapObservable(valueAccessor());
                            for (var index = 0, length = element.childNodes.length; index < length; index++) {
                                var node = element.childNodes[index];
                                if (node.nodeType === 1) {
                                    var id = node.id.replace("row_", "");
                                    var tooltipElement = $("#thumb_" + id);
                                    if (tooltipElement.length > 0)
                                        tooltipElement.tooltip({ delay: { show: 100, hide: 100 } });
                                }
                            }
                            // if there are no rows in the table, hide the table and display a message
                            var table = element.parentNode; // get the table element
                            var noMediaMessage = $("#no-records-message");

                            var tableDetailElement = $("#table-detail");
                            var tableHeaderElement = $("#table-header");

                            if (table.rows.length > 0) {
                                tableHeaderElement.show();
                                tableDetailElement.show();
                                noMediaMessage.hide();

                                // determine if there is table overflow (to cause a scrollbar)
                                // if so, increase the right margin of last column header 
                                var colRight = $("#sort-right");

                                if (table.clientHeight > site.getMaxClientHeight) {
                                    colRight.removeClass("col-date");
                                    colRight.addClass("col-date-scrollbar");
                                    tableDetailElement.addClass("container-height");
                                } else {
                                    colRight.removeClass("col-date-scrollbar");
                                    colRight.addClass("col-date");
                                    tableDetailElement.removeClass("container-height");
                                }

                            } else {
                                tableHeaderElement.hide();
                                tableDetailElement.hide();
                                noMediaMessage.html("<h2>No residents found</h2>");
                                noMediaMessage.show();
                            }
                        }
                    }

                    ko.applyBindings(new ResidentViewModel());

                    // select the resident whose media was being managed as the page was posted
                    if (parseInt(config.selectedid) > 0) { $("#row_" + config.selectedid).trigger("click"); }

                    // rest sorting to the way it was before the page was posted
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
                            arr.push({ title: "ID", sortKey: "id", cssClass: "col-id" });
                            arr.push({ title: "First Name", sortKey: "firstname", cssClass: "col-firstname" });
                            arr.push({ title: "Last Name", sortKey: "lastname", cssClass: "col-lastname" });
                            arr.push({ title: "Gender", sortKey: "gender", cssClass: "col-gender" });

                            if (config.isVideoCaptureServiceInstalled === "1")
                                arr.push({ title: "Capturable", sortKey: "allowvideocapturing", cssClass: "col-capturable", boolean: true });

                            //arr.push({ title: "Updated", sortKey: "dateupdated", id: "sort-right", cssClass: "col-date col-right" });
                            return arr;
                        });

                        function pushResident(value) {
                            self.residents.push(new Resident(value.Id, value.FirstName, value.LastName, value.Gender, value.GameDifficultyLevel, value.AllowVideoCapturing, value.ProfilePicture, value.ProfilePicturePlaceholder, value.DateUpdated));
                        };

                        self.selectedResident(self.residents()[0]);

                        self.sort = function (header) {
                            if (isBinding) return;

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

                            self.residents(utilities.sorting.sortArray(
                                {
                                    fileArray: self.residents(),
                                    columns: self.columns(),
                                    sortKey: sortKey,
                                    primaryKey: primarySortKey,
                                    descending: sortDescending
                                }));
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
                            $("#edit_" + row.id).tooltip("hide");

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
                            $("#thumb_" + row.id).tooltip("hide");

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
                            $("#delete_" + row.id).tooltip("hide");

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
                                                    reject(dialog);
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