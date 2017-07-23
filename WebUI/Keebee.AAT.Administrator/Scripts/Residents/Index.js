/*!
 * 1.0 Keebee AAT Copyright © 2016
 * Residents/Index.js
 * Author: John Charlton
 * Date: 2016-08
 */

; (function ($) {
    residents.index = {
        init: function (options) {
            var isBinding = true;   // <-- for initial page load only - used to stop the sort from being executed
            var isRendering = true; // <-- for initial page load only - used to set focus to 'First Name' search input

            // buttons
            var cmdAdd = $("#add");

            var sortDescending = false;
            var currentSortKey = "firstname";
            var primarySortKey = "firstname";

            var config = {
                selectedid: 0,
                idsearch: "",
                firstname: "",
                lastname: "",
                sortcolumn: "",
                sortdescending: 0,
                isVideoCaptureServiceInstalled: 0
            }

            $.extend(config, options);

            if (config.sortcolumn.length > 0)
                currentSortKey = config.sortcolumn;

            var lists = {
                ResidentList: []
            };

            utilities.job.execute({
                url: site.url + "Residents/GetData/"
            })
            .then(function (data) {
                $.extend(lists, data);

                $("#error-container").hide();
                $("#loading-container").hide();
                $("#table-header").show();
                $("#table-detail").show();
                cmdAdd.prop("disabled", false);

                ko.bindingHandlers.tableUpdated = {
                    update: function (element, valueAccessor, allBindings) {
                        ko.unwrap(valueAccessor());
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

                                var tooltipProfile = $("#profile_" + id);
                                if (tooltipProfile.length > 0) {
                                    tooltipProfile.tooltip({ delay: { show: 100, hide: 100 } });
                                }

                                var tooltipEdit = $("#edit_" + id);
                                if (tooltipEdit.length > 0) {
                                    tooltipEdit.tooltip({ delay: { show: 100, hide: 100 } });
                                }

                                var tooltipDelete = $("#delete_" + id);
                                if (tooltipDelete.length > 0) {
                                    tooltipDelete.tooltip({ delay: { show: 100, hide: 100 } });
                                }

                                var tooltipProfilePicture = $("#profilepicture_" + id);
                                if (tooltipProfilePicture.length > 0) {
                                    tooltipProfilePicture.tooltip({ delay: { show: 100, hide: 100 } });
                                }
                            }
                        }
                        var table = element.parentNode;
                        formatTable(table);
                    }
                }

                function formatTable(table) {
                    var noMediaMessage = $("#no-rows-message");

                    var tableDetailElement = $("#table-detail");
                    var tableHeaderElement = $("#table-header");

                    $("#col-glyphicon_1").html("<div class='virtualPlaceholderGlyphicon'></div>");
                    $("#col-glyphicon_2").html("<div class='virtualPlaceholderGlyphicon'></div>");
                    $("#col-glyphicon_3").html("<div class='virtualPlaceholderGlyphicon'></div>");


                    if (table.rows.length > 0) {
                        tableHeaderElement.show();
                        tableDetailElement.show();
                        noMediaMessage.hide();

                        // determine if there is table overflow (to cause a scrollbar)
                        // if so, unhide the scrollbar header column
                        var colScrollbar = $("#col-scrollbar");

                        if (table.clientHeight > site.getMaxClientHeight) {
                            colScrollbar.prop("hidden", false);
                            colScrollbar.attr("style", "width: 1%; border-bottom: 1.5px solid #ddd;");
                            tableDetailElement.addClass("container-height");
                        } else {
                            colScrollbar.prop("hidden", true);
                            tableDetailElement.removeClass("container-height");
                        }
                        // if there are no rows in the table, hide the table and display a message
                    } else {                         
                        tableHeaderElement.hide();
                        tableDetailElement.hide();
                        noMediaMessage.show();
                    }
                    if (isRendering) $("#txtSearchFirstName").focus();
                    isRendering = false;
                }

                ko.applyBindings(new ResidentViewModel());

                // select the resident whose media was being managed as the page was posted
                if (parseInt(config.selectedid) > 0) { $("#row_" + config.selectedid).trigger("click"); }

                // rest sorting to the way it was before the page was posted
                sortDescending = (config.sortdescending === "0");
                if (config.sortcolumn.length > 0)
                    $("#resident-col-" + config.sortcolumn).trigger("click");

                function Resident(id, firstname, lastname, gender, gamedifficultylevel, allowvideocapturing, profilepicture, profilepictureplaceholder, datecreated, dateupdated) {
                    var self = this;

                    self.id = ko.observable(id);
                    self.firstname = ko.observable(firstname);
                    self.lastname = ko.observable(lastname);
                    self.gender = ko.observable(gender);
                    self.gamedifficultylevel = ko.observable(gamedifficultylevel);
                    self.allowvideocapturing = ko.observable(allowvideocapturing);
                    self.profilepicture = ko.observable(profilepicture);
                    self.profilepictureplaceholder = ko.observable(profilepictureplaceholder);
                    self.datecreated = ko.observable(datecreated);
                    self.dateupdated = ko.observable(dateupdated);
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

                    createResidentArray({ list: lists.ResidentList, insert: false });

                    function createResidentArray(params) {
                        var cfg = {
                            list: [],
                            insert: false
                        };

                        $.extend(cfg, params);

                        if (cfg.insert === false) {
                            self.residents.removeAll();
                            $(cfg.list)
                                .each(function(index, value) {
                                    self.residents.push(new Resident(
                                        value.Id,
                                        value.FirstName,
                                        value.LastName,
                                        value.Gender,
                                        value.GameDifficultyLevel,
                                        value.AllowVideoCapturing,
                                        value.ProfilePicture,
                                        value.ProfilePicturePlaceholder,
                                        value.DateCreated,
                                        value.DateUpdated
                                    ));
                                });
                        } else {
                            $(cfg.list)
                                .each(function (index, value) {
                                    // do an "unshift"
                                    self.residents.unshift(new Resident(
                                    value.Id,
                                    value.FirstName,
                                    value.LastName,
                                    value.Gender,
                                    value.GameDifficultyLevel,
                                    value.AllowVideoCapturing,
                                    value.ProfilePicture,
                                    value.ProfilePicturePlaceholder,
                                    value.DateCreated,
                                    value.DateUpdated
                                ));
                            });
                        }
                    };

                    self.columns = ko.computed(function () {
                        var arr = [];
                            
                        arr.push({ title: "First Name", sortKey: "firstname", visible: true, cssClass: "col-firstname" });
                        arr.push({ title: "Last Name", sortKey: "lastname", visible: true, cssClass: "col-lastname" });
                        arr.push({ title: "ID", sortKey: "id", visible: true, cssClass: "col-id" });
                        arr.push({ title: "Created", sortKey: "datecreated", visible: false, cssClass: "col-date" });
                        arr.push({ title: "Updated", sortKey: "dateupdated", visible: true, cssClass: "col-date" });
                        arr.push({ title: "Gender", sortKey: "gender", visible: true, cssClass: "col-gender" });

                        if (config.isVideoCaptureServiceInstalled === "1")
                            arr.push({ title: "Capturable", sortKey: "allowvideocapturing", visible: true, cssClass: "col-capturable", boolean: true });
                        return arr;
                    });

                    self.selectedResident(self.residents()[0]);

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

                        self.residents(utilities.sorting.sortArray(
                            {
                                array: self.residents(),
                                columns: self.columns(),
                                sortKey: sortKey,
                                primarySortKey: primarySortKey,
                                descending: sortDescending,
                                boolean: isboolean,
                                observable: true
                            }));
                    };

                    self.filteredResidents = ko.computed(function () {
                        return ko.utils.arrayFilter(self.residents(), function (r) {
                            return (
                                (self.firstNameSearch().length === 0 || r.firstname().toLowerCase().indexOf(self.firstNameSearch().toLowerCase()) !== -1)
                                &&
                                (self.lastNameSearch().length === 0 || (r.lastname() !== null &&
                                    r.lastname().toLowerCase().indexOf(self.lastNameSearch().toLowerCase()) !== -1))
                                &&
                                (self.idSearch().length === 0 || r.id().toString().indexOf(self.idSearch().toString()) !== -1)
                            );
                        });
                    });

                    self.residentsTable = ko.computed(function () {
                        var filteredResidents = self.filteredResidents();
                        self.totalResidents(filteredResidents.length);

                        return filteredResidents;
                    });

                    self.edit = function (row) {
                        cmdAdd.prop("disabled", true);

                        var id = (typeof row.id !== "undefined")
                            ? row.id()
                            : 0;

                        $("#edit_" + id).tooltip("hide");

                        var add = (id === 0);
                        if (!add) { self.highlightRow(row); }

                        var title = "<span class='glyphicon glyphicon-pencil' style='color: #fff'></span>";
                        title = add
                        ? title + " Add Resident"
                        : title + " Edit Resident";

                        if (add) {
                            self.selectedResident([]);
                        } else {
                            var r = self.getResident(id);
                            self.selectedResident(r);
                        }

                        utilities.partialview.show({
                            url: site.url + "Residents/GetResidentEditView",
                            params: { id: id },
                            type: add ? BootstrapDialog.TYPE_SUCCESS : BootstrapDialog.TYPE_PRIMARY,
                            focus: "txtFirstName",
                            title: title,
                            buttonOK: "Save",
                            buttonOKClass: add ? "btn-success" : "btn-edit",
                            cancelled: function () { cmdAdd.prop("disabled", false); },
                            callback: function(dialog) {
                                var resident = self.getResidentDetailFromDialog();

                                utilities.job.execute({
                                    url: site.url + "Residents/Validate",
                                    action: "Validate",
                                    type: "POST",
                                    params: { resident: resident }
                                })
                                .then(function(validateResult) {
                                    if (validateResult.ValidationMessages === null) {
                                        dialog.close();
                                        utilities.job.execute({
                                                url: site.url + "Residents/Save",
                                                type: "POST",
                                                title: "Save Resident",
                                                params: { resident: resident }
                                            })
                                            .then(function (saveResult) {
                                                if (add)
                                                    createResidentArray({ list: saveResult.ResidentList, insert: true });
                                                else
                                                    self.updateResident(saveResult.ResidentList);

                                                if (saveResult.ResidentList.length > 0) {
                                                    self.selectedResident(self.getResident(saveResult.ResidentList[0].Id));
                                                    self.highlightRow(self.selectedResident());
                                                }
                                                cmdAdd.prop("disabled", false);
                                            });
                                    } else {
                                        utilities.validation.show({
                                            container: "validation-container",
                                            messages: validateResult.ValidationMessages
                                        });
                                    }
                                })
                                .catch(function() {
                                    cmdAdd.prop("disabled", false);
                                });
                            }
                        });
                    };

                    self.editProfile = function (row) {
                        var id = row.id();

                        $("#thumb_" + id).tooltip("hide");

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

                    self.delete = function (row) {
                        cmdAdd.prop("disabled", true);

                        var id = (typeof row.id !== "undefined" ? row.id() : 0);
                        if (id <= 0) return;

                        $("#delete_" + id).tooltip("hide");
                        self.highlightRow(row);

                        // construct the confirm message
                        var resident = self.getResident(id);

                        // name
                        var fullName = (resident.lastname() == null)
                            ? resident.firstname()
                            : resident.firstname() + " " + resident.lastname();

                        // possesive
                        var messageGender = (resident.gender().toUpperCase() === "M")
                            ? "his"
                            : "her";

                        // show confirm
                        utilities.confirm.show({
                            type: BootstrapDialog.TYPE_DANGER,
                            title: "Delete Resident?",
                            message: "Permanently delete the resident <i><b>" +
                                fullName +
                                "</b></i>?\n\n" +
                                "<b>Warning:</b> All " +
                                messageGender +
                                " personal media files will be removed!",
                            buttonOK: "Yes, Delete",
                            buttonOKClass: "btn-danger"
                        })
                        .then(function(confirm) {
                            if (confirm) {
                                utilities.job.execute({
                                    url: site.url + "Residents/Delete",
                                    type: "POST",
                                    params: { id: id },
                                    title: "Delete Resident",
                                    waitMessage: "Deleting..."
                                })
                                .then(function (deleteResult) {
                                    self.removeResident(deleteResult.DeletedId);
                                    cmdAdd.prop("disabled", false);
                                })
                                .catch(function() {
                                    cmdAdd.prop("disabled", false);
                                });
                            } else {
                                cmdAdd.prop("disabled", false);
                            }
                        });
                    };

                    self.getResident = function (residentid) {
                        var resident = null;

                        ko.utils.arrayForEach(self.residents(), function (item) {
                            if (item.id() === residentid) {
                                resident = item;
                            }
                        });

                        return resident;
                    };

                    self.updateResident = function (residentList) {
                        $(residentList).each(function(index, value) {
                            var r = self.getResident(value.Id);

                            r.firstname(value.FirstName);
                            r.lastname(value.LastName);
                            r.gender(value.Gender);
                            r.gamedifficultylevel(value.GameDifficultyLevel);
                            r.allowvideocapturing(value.AllowVideoCapturing);
                            r.profilepicture(value.ProfilePicture);
                            r.profilepictureplaceholder(value.ProfilePicturePlaceholder);
                            r.dateupdated(value.DateUpdated);
                        });
                    };

                    self.highlightRow = function (row) {
                        var r = tblResident.find("#row_" + row.id());
                        $(r).addClass("highlight").siblings().removeClass("highlight");
                        $("#txtSearchFirstName").focus();
                    };

                    self.getResidentDetailFromDialog = function () {
                        var firstname = $.trim($("#txtFirstName").val());
                        var lastname = $.trim($("#txtLastName").val());
                        var gender = $("#radGender").val();
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

                    self.removeResident = function (id) {
                        var idx = self.residents().findIndex(function (value) {
                            return value.id() === id;
                        });
                        self.residents.splice(idx, 1);
                    }
                };
            })
            .catch(function (error) {
                $("#loading-container").hide();
                $("#error-container")
                    .html("<div><h2>Data load error:</h2></div>")
                    .append("<div>" + error.error + "</div>")
                    .append("<div><h3>Please try refreshing the page</h3></div>");
                $("#error-container").show();
            });
        }
    }
})(jQuery);