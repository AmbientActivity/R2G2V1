/*!
 * Profiles/Edit.js
 * Author: John Charlton
 * Date: 2015-05
 */

; (function ($) {

    var HIGHLIGHT_ROW_COLOUR = "#e3e8ff";

    profiles.edit = {
        init: function (config) {
            var profileid = config.id;
            var _sortDescending = false;
            var _currentSortKey = "filename";

            var lists = {
                FileList: [],
                MediaTypeList: []
            };

            loadData();

            ko.applyBindings(new FileViewModel());

            function loadData() {
                $.ajax({
                    type: "GET",
                    url: site.url + "Profiles/GetDataEdit/" + profileid,
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

            function MediaType(description) {
                var self = this;
                self.description = description;
            }

            function FileViewModel() {
                var tblFile = $("#tblFile");

                var self = this;

                self.files = ko.observableArray([]);
                self.mediaTypes = ko.observableArray([]);
                self.selectedFile = ko.observable();
                self.selectedMediaType = ko.observable("");
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
                        pushMediaType(value);
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
                };

                function pushMediaType(value) {
                    self.mediaTypes.push(new MediaType(value));
                };

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
                            f.filename.toLowerCase().indexOf(self.filenameSearch().toLowerCase()) !== -1) &&
                        (self.selectedMediaType().length === 0 ||
                            f.path.toLowerCase().indexOf(self.selectedMediaType().toLowerCase()) !== -1);
                    });
                });

                self.filesTable = ko.computed(function () {
                    var filteredFiles = self.filteredFiles();
                    self.totalFiles(filteredFiles.length);

                    return filteredFiles;
                });

                self.showEditDialog = function (row) {
                    var id = row.id;

                    if (id > 0) {
                        self.highlightRow(row);
                    }

                    self.showFileEditDialog(row);
                };

                self.editProfile = function (row) {
                    var id = row.profileid;

                    if (id > 0) {
                        self.highlightRow(row);
                    }
                };

                self.deleteSelectedFile = function (row) {
                    deleteFile(row.id);
                };

                self.showDeleteDialog = function (row) {
                    self.highlightRow(row);
                    self.showFileDeleteDialog(row);
                };

                self.showFileDeleteDialog = function (row) {
                    var id = (typeof row.id !== "undefined" ? row.id : 0);
                    if (id <= 0) return;
                    var r = self.getFile(id);
                    var messageGender;

                    if (r.gender === "M") messageGender = "his";
                    else messageGender = "her";

                    BootstrapDialog.show({
                        type: BootstrapDialog.TYPE_DANGER,
                        title: "Delete File?",
                        message: "Permanently delete the file <i><b>" + r.firstname + " " + r.lastname + "</b></i>?\n\n" +
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
                                    var result = self.deleteFile(row.id);
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

                self.showFileEditDialog = function (row) {
                    var id = (typeof row.id !== "undefined" ? row.id : 0);
                    var title = "<span class='glyphicon glyphicon-pencil'></span>";
                    var message;

                    if (id > 0) {
                        title = title + " Edit File";
                        var file = self.getFile(id);
                        self.selectedFile(file);
                    } else {
                        title = title + " Add File";
                        self.selectedFile([]);
                    }

                    $.ajax({
                        type: "GET",
                        async: false,
                        url: site.url + "Files/GetFileEditView/" + id,
                        success: function (data) {
                            message = data;
                        }
                    });

                    BootstrapDialog.show({
                        title: title,
                        message: message,
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
                            }, {
                                label: "OK",
                                cssClass: "btn-primary",
                                action: function (dialog) {
                                    var result = self.saveFile();

                                    if (result.ErrorMessages === null) {
                                        lists.FileList = result.FileList;
                                        createFileArray(lists.FileList);
                                        self.selectedFile(self.getFile(result.SelectedId));
                                        self.sort({ afterSave: true });
                                        self.highlightRow(self.selectedFile());
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

                self.getFile = function (fileid) {
                    var file = null;

                    ko.utils.arrayForEach(self.files(), function (item) {
                        if (item.id === fileid) {
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

                self.deleteFile = function (id) {
                    $("body").css("cursor", "wait");

                    var result;

                    $.ajax({
                        type: "POST",
                        async: false,
                        url: site.url + "Files/Delete/",
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