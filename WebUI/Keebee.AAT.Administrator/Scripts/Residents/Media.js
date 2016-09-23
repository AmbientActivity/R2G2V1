/*!
 * Residents/Media.js
 * Author: John Charlton
 * Date: 2015-05
 */

function CuteWebUI_AjaxUploader_OnPostback() {
    $.blockUI({ message: "<h4>Saving...</h4>" });
    document.forms[0].submit();
}

function CuteWebUI_AjaxUploader_OnError(msg) {
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_DANGER,
        title: "File Error",
        message: msg
    });

    EnableScreen();
    return false;
}

function CuteWebUI_AjaxUploader_OnTaskError(obj, msg, reason) {
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_DANGER,
        title: "Task Error",
        message: "Error uploading file <b>" + obj.FileName + "</b>.\nMessage: " + msg
    });

    EnableScreen();
    return false;
}

function CuteWebUI_AjaxUploader_OnStop() {
    EnableScreen();
}

function CuteWebUI_AjaxUploader_OnSelect() {
    DisableScreen();
}

function EnableScreen() {
    $("#lnkGoBack").removeAttr("hidden");
    $("#lblGoBackDisabled").attr("hidden", "hidden");
    $("#txtSearchFilename").removeAttr("disabled");
    $("#uploadbutton").removeAttr("disabled");
    $("select").removeAttr("disabled");
    $("#main-menu").removeAttr("hidden");
}

function DisableScreen() {
    $("#lnkGoBack").attr("hidden", "hidden");
    $("#lblGoBackDisabled").removeAttr("hidden");
    $("#txtSearchFilename").attr("disabled", "disabled");
    $("#uploadbutton").attr("disabled", "disabled");
    $("select").attr("disabled", "disabled");
    $("#main-menu").attr("hidden", "hidden");
}

; (function ($) {

    var HIGHLIGHT_ROW_COLOUR = "#e3e8ff";

    residents.media = {
        init: function (values) {
            var config = {
                residentid: 0
            }

            $.extend(config, values);

            // buttons
            var divUpload = $(".upload-action-container");
            var divAddPublic = $(".add-public-action-container");

            var _sortDescending = false;
            var _currentSortKey = "filename";

            var lists = {
                FileList: [],
                MediaPathTypeList: [],
                MediaSourceTypeList: []
            };

            loadData();

            ko.applyBindings(new FileViewModel());

            function loadData() {
                var mediaPathTypeId = $("#mediaPathTypeId").val();

                $.ajax({
                    type: "GET",
                    url: site.url + "Residents/GetDataMedia/" + config.residentid
                        + "?mediaPathTypeId=" + mediaPathTypeId,
                    dataType: "json",
                    traditional: true,
                    async: false,
                    success: function (data) {
                        $.extend(lists, data);
                    }
                });
            }

            function MediaPathType(id, description) {
                var self = this;

                self.id = id;
                self.description = description;
            }

            function MediaSourceType(id, description) {
                var self = this;

                self.id = id;
                self.description = description;
            }

            function File(streamid, filename, filetype, filesize, path, ispublic) {
                var self = this;

                self.streamid = streamid;
                self.filename = filename;
                self.filetype = filetype;
                self.filesize = filesize;
                self.path = path;
                self.ispublic = ispublic;
            }

            function FileViewModel() {
                var tblFile = $("#tblFile");

                var self = this;

                self.files = ko.observableArray([]);
                self.mediaPathTypes = ko.observableArray([]);
                self.mediaSourceTypes = ko.observableArray();
                self.selectedFile = ko.observable();
                self.isLoadingMediaSourceTypes = ko.observable(false);
                self.selectedMediaPathType = ko.observable($("#mediaPathTypeId").val());
                self.selectedMediaSourceType = ko.observable($("#mediaSourceTypeId").val());
                self.filenameSearch = ko.observable("");
                self.totalFiles = ko.observable(0);

                createFileArray(lists.FileList);
                createMediaPathTypeArray(lists.MediaPathTypeList);
                createMediaSourceTypeArray(lists.MediaSourceTypeList);

                function createFileArray(list) {
                    self.files.removeAll();
                    $(list).each(function (index, value) {
                        pushFile(value);
                    });
                };

                function createMediaPathTypeArray(list) {
                    self.mediaPathTypes.removeAll();
                    $(list).each(function (index, value) {
                        pushMediaPathType(value);
                    });
                };

                function createMediaSourceTypeArray(list) {
                    self.isLoadingMediaSourceTypes(true);
                    self.mediaSourceTypes.removeAll();
                    $(list).each(function (index, value) {
                        pushMediaSourceType(value);
                    });
                    self.isLoadingMediaSourceTypes(false);
                };

                self.columns = ko.computed(function () {
                    var arr = [];
                    arr.push({ title: "Name", sortable: true, sortKey: "filename" });
                    arr.push({ title: "Type", sortable: true, sortKey: "filetype" });
                    arr.push({ title: "Size", sortable: true, sortKey: "filesize" });
                    return arr;
                });

                function pushMediaPathType(value) {
                    self.mediaPathTypes.push(new MediaPathType(value.Id, value.Description));
                }

                function pushMediaSourceType(value) {
                    self.mediaSourceTypes.push(new MediaSourceType(value.Id, value.Description));
                }

                function pushFile(value) {
                    self.files.push(new File(value.StreamId, value.IsFolder, value.Filename, value.FileType, value.FileSize, value.Path, value.IsPublic));
                }

                self.selectedFile(self.files()[0]);

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
                            self.files.sort(function (a, b) {
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

                self.filteredFiles = ko.computed(function () {
                    return ko.utils.arrayFilter(self.files(), function (f) {
                        var ispublic = (self.selectedMediaSourceType() === 0);
                        return (
                            self.filenameSearch().length === 0 ||
                            f.filename.toLowerCase().indexOf(self.filenameSearch().toLowerCase()) !== -1) &&
                            f.ispublic === ispublic;
                    });
                });

                self.filesTable = ko.computed(function () {
                    var filteredFiles = self.filteredFiles();
                    self.totalFiles(filteredFiles.length);

                    return filteredFiles;
                });

                // ------------------

                self.doPostBack = function () {
                    $("#mediaPathTypeId").val(self.selectedMediaPathType());
                    $("#mediaSourceTypeId").val(self.selectedMediaSourceType());
                    document.forms[0].submit();
                }

                self.showDeleteDialog = function (row) {
                    self.highlightRow(row);
                    self.showFileDeleteDialog(row);
                };

                self.showPreview = function (row) {
                    self.highlightRow(row);
                    self.showFeatureNotDoneYetDialog();
                };

                self.showAddPublicDialog = function () {
                    self.showFeatureNotDoneYetDialog();
                };

                self.showFileDeleteDialog = function (row) {
                    var id = (typeof row.streamid !== "undefined" ? row.streamid : 0);
                    if (id <= 0) return;

                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_DANGER,
                        title: "Delete File?",
                        message: "Delete the file <i><b>" + row.filename + "." + row.filetype.toLowerCase() + "</b></i>?",
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
                                    var result = self.deleteFile(id);
                                    lists.FileList = result.FileList;
                                    createFileArray(lists.FileList);
                                    self.sort({ afterSave: true });
                                    dialog.close();
                                    $("body").css("cursor", "default");
                                }
                            }
                        ]
                    });
                };

                self.showFeatureNotDoneYetDialog = function () {
                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_INFO,
                        title: "Under Development",
                        message: "This feature has not been implemented yet.",
                        closable: false,
                        buttons: [
                            {
                                label: "Close",
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }
                        ]
                    });
                };

                self.getFile = function (streamid) {
                    var file = null;

                    ko.utils.arrayForEach(self.files(), function (item) {
                        if (item.streamid === streamid) {
                            file = item;
                        }
                    });

                    return file;
                };

                self.highlightRow = function (row) {
                    if (row == null) return;

                    var rows = tblFile.find("tr:gt(0)");
                    rows.each(function () {
                        $(this).css("background-color", "#ffffff");
                    });

                    var r = tblFile.find("#row_" + row.streamid);
                    r.css("background-color", HIGHLIGHT_ROW_COLOUR);
                    tblFile.attr("tr:hover", HIGHLIGHT_ROW_COLOUR);
                };

                self.getFileDetailFromDialog = function () {
                    var firstname = $.trim($("#txtFirstName").val());
                    var lastname = $.trim($("#txtLastName").val());
                    var gender = $.trim($("#ddlGenders").val());

                    return {
                        Id: self.selectedFile().id, FirstName: firstname, LastName: lastname, Gender: gender
                    };
                };

                self.enableDetail = ko.computed(function () {
                    if (self.isLoadingMediaSourceTypes()) return;

                    var ispublic = self.selectedMediaSourceType() === 0;

                    //  buttons
                    if (ispublic) {
                        divAddPublic.removeAttr("hidden");
                        divUpload.attr("hidden", "hidden");
                    } else {
                        divAddPublic.attr("hidden", "hidden");
                        divUpload.removeAttr("hidden");
                    }
                });

                self.saveFile = function () {
                    var filedetail = self.getFileDetailFromDialog();
                    var jsonData = JSON.stringify(filedetail);
                    var result;

                    $("body").css("cursor", "wait");

                    $.ajax({
                        type: "POST",
                        async: false,
                        url: site.url + "Files/Save/",
                        data: { file: jsonData },
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

                self.deleteFile = function (streamid) {
                    $("body").css("cursor", "wait");

                    var result;
                    var mediaPathTypeId = $("#mediaPathTypeId").val();

                        $.ajax({
                            type: "POST",
                            async: false,
                            url: site.url + "Residents/DeleteFile/",
                            data:
                            {
                                streamId: streamid,
                                residentId: config.residentid,
                                mediaPathTypeId: mediaPathTypeId
                            },
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


