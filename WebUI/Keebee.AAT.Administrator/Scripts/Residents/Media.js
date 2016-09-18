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
        title: "Tssk Error",
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
    $("#lblGoBackDisabled").attr("hidden", "hidden");
    $("#lnkGoBack").removeAttr("hidden");
    $("#txtSearchFilename").removeAttr("disabled");
    $("#uploadbutton").removeAttr("disabled");
    $("select").removeAttr("disabled");
    $("#main-menu").removeAttr("hidden");
}

function DisableScreen() {
    $("#lblGoBackDisabled").attr("hidden", "hidden");
    $("#lnkGoBack").removeAttr("hidden");
    $("#txtSearchFilename").removeAttr("disabled");
    $("#uploadbutton").removeAttr("disabled");
    $("select").removeAttr("disabled");
    $("#main-menu").removeAttr("hidden");
}

; (function ($) {

    var HIGHLIGHT_ROW_COLOUR = "#e3e8ff";

    residents.media = {
        init: function (values) {
            var config = {
                residentid: 0
            }

            $.extend(config, values);

            var _sortDescending = false;
            var _currentSortKey = "filename";

            var lists = {
                FileList: [],
                MediaTypeList: []
            };

            loadData();

            ko.applyBindings(new FileViewModel());

            function loadData() {
                var mediaType = $("#mediaType").val();
                $.ajax({
                    type: "GET",
                    url: site.url + "Residents/GetDataMedia/" + config.residentid + "?mediaType=" + mediaType,
                    dataType: "json",
                    traditional: true,
                    async: false,
                    success: function (data) {
                        $.extend(lists, data);
                    }
                });
            }

            function File(streamid, isfolder, filename, filetype, filesize, path) {
                var self = this;

                self.streamid = streamid;
                self.isfolder = isfolder;
                self.filename = filename;
                self.filetype = filetype;
                self.filesize = filesize;
                self.path = path;
            }

            function FileViewModel() {
                var tblFile = $("#tblFile");

                var self = this;

                self.files = ko.observableArray([]);
                self.mediaTypes = ko.observableArray(["images","videos"]);
                self.selectedFile = ko.observable();
                self.selectedMediaType = ko.observable($("#mediaType").val());
                self.filenameSearch = ko.observable("");
                self.totalFiles = ko.observable(0);

                createFileArray(lists.FileList);
                createMediaTypeArray(lists.MediaTypeList);

                function createFileArray(list) {
                    self.files.removeAll();
                    $(list).each(function (index, value) {
                        pushFile(value);
                    });
                };

                function createMediaTypeArray(list) {
                    self.mediaTypes.removeAll();
                    $(list).each(function (index, value) {
                        self.mediaTypes.push(value);
                    });
                };

                self.columns = ko.computed(function () {
                    var arr = [];
                    arr.push({ title: "Name", sortable: true, sortKey: "filename" });
                    arr.push({ title: "Type", sortable: true, sortKey: "filetype" });
                    arr.push({ title: "Size", sortable: true, sortKey: "filesize" });
                    return arr;
                });

                function pushFile(value) {
                    self.files.push(new File(value.StreamId, value.IsFolder, value.Filename, value.FileType, value.FileSize, value.Path));
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
                        return (self.filenameSearch().length === 0 ||
                            f.filename.toLowerCase().indexOf(self.filenameSearch().toLowerCase()) !== -1);
                    });
                });

                self.filesTable = ko.computed(function () {
                    var filteredFiles = self.filteredFiles();
                    self.totalFiles(filteredFiles.length);

                    return filteredFiles;
                });

                // ------------------

                self.doPostBack = function () {
                    $("#mediaType").val(self.selectedMediaType());
                    document.forms[0].submit();
                }

                self.showDeleteDialog = function (row) {
                    self.highlightRow(row);
                    self.showFileDeleteDialog(row);
                };

                self.showPreview = function (row) {
                    self.highlightRow(row);
                    self.showFilePreviewDialog(row);
                };

                self.showFileDeleteDialog = function (row) {
                    var id = (typeof row.streamid !== "undefined" ? row.streamid : 0);
                    if (id <= 0) return;

                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_DANGER,
                        title: "Delete File?",
                        message: "Delete the file <i><b>" + row.filename + "</b></i>?",
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

                self.showFilePreviewDialog = function (row) {
                    var id = (typeof row.streamid !== "undefined" ? row.streamid : 0);
                    if (id <= 0) return;

                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_INFO,
                        title: "File Preview",
                        message: "Feature not implemented yet.",
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
                    var mediatype = $("#mediaType").val();

                    $.ajax({
                        type: "POST",
                        async: false,
                        url: site.url + "Residents/DeleteFile/",
                        data: { streamId: streamid, residentId: config.residentid, mediaType: mediatype },
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


