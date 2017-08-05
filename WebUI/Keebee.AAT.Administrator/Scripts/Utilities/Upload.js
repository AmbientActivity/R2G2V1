/*!
 * 1.0 Keebee AAT Copyright © 2017
 * Utilities/Upload.js
 * Author: John Charlton
 * Date: 2017-06
 */

; (function ($) {

    utilities.upload = {
        init: function (options) {
            var config = {
                url: "",
                addUrl: "",
                allowedExts: [],
                allowedTypes: [],
                maxFileBytes: 5000000, //5MB in bytes
                maxFileUploads: 10,
                callback: function () { }
            };

            $.extend(config, options);

            $("input[type=file]").unbind("change");

            $("input[type=file]").change(function () {
                var totalFiles;
                var isTooManyFiles = false;
                var pendingFile = 1;
                var successful = [0];
                var rejected = [0];

                $(this).simpleUpload(config.url, {
                    allowedExts: config.allowedExts,
                    allowedTypes: config.allowedTypes,
                    maxFileSize: config.maxFileBytes,
                    //limit: config.maxFileUploads,
                    /*
                    * Each of these callbacks are executed for each file
                    * To add callbacks that are executed only once, see init() and finish().
                    *
                    * "this" is an object that can carry data between callbacks for each file.
                    * Data related to the upload is stored in this.upload.
                    */
                    init: function (totalUploads) {
                        totalFiles = totalUploads;

                        isTooManyFiles = (totalFiles > config.maxFileUploads);

                        successful = [];
                        rejected = [];
                    },

                    start: function (file) {
                        if (isTooManyFiles) {
                            this.upload.cancel();

                        } else {

                            disableScreen();

                            this.block = $('<div class="block"></div>');
                            this.progressBar = $('<div class="progressBar"></div>');
                            this.cancelButton = $('<div class="cancelButton">x</div>');

                            var that = this;
                            this.cancelButton.click(function() {
                                that.upload.cancel();
                            });

                            this.block.html("");
                            this.block.append(this.progressBar).append(this.cancelButton);

                            this.block.append("<div>Uploading <b>" +
                                file.name +
                                "</b> (" +
                                pendingFile +
                                " of " +
                                totalFiles +
                                " files)...</div>");
                            $("#uploads").append(this.block);
                            pendingFile++;
                        }
                    },

                    progress: function (progress) {
                        this.progressBar.width(progress + "%");
                    },
                    success: function (data) {
                        //upload successful

                        this.progressBar.remove();
                        this.block.html("");
                        this.block.append("<div><b>" + data.Filename + "</b> (Succeeded)</div>");
                        /*
                        * Just because the success callback is called doesn't mean your
                        * application logic was successful, so check application success.
                        *
                        * Data as returned by the server on...
                        * success:	{"success":true,"format":"..."}
                        * error:	{"success":false,"error":{"code":1,"message":"..."}}
                        */
                        if (typeof data.Success === "undefined") {
                            utilities.sessionexpired.show();
                        } else {
                            if (data.Success) {
                                if (data.Filename !== null)
                                    successful.push(data.Filename);

                                this.block.fadeOut(400, function () { });
                            } else {
                                var message = data.ErrorMessage; // optionally display this
                            }
                        }
                    },
                    cancel: function () {
                        this.block.fadeOut(400, function () { });
                    },
                    finish: function () {
                        $("#uploads").html("");
                        enableScreen();

                        if (isTooManyFiles) {
                            rejected = ["<p>Too many files were selected for a single upload.</p>" +
                                "<div><b>You selected:</b> " + totalFiles + "</div>" + 
                                "<div><b>Maximum allowed:</b> " + config.maxFileUploads + "</div>"];
                        }
                        config.callback(successful, rejected);
                    },
                    error: function (error) {
                        var message = "";
                        var html = this.block[0].innerHTML;
                        // extract filename
                        var filename = html.substring(html.indexOf("<b>") + 3, html.indexOf("</b>"));

                        if (error.name === "MaxFileSizeError") {
                            var maxSizeDesc = "";

                            if (config.maxFileBytes >= 1000 && config.maxFileBytes < 1000000)
                                maxSizeDesc = config.maxFileBytes / 1000 + " KB";
                            else if (config.maxFileBytes >= 1000000 && config.maxFileBytes < 1000000000)
                                maxSizeDesc = config.maxFileBytes / 1000000 + " MB";
                            else if (config.maxFileBytes >= 1000000000)
                                maxSizeDesc = config.maxFileBytes / 1000000000 + " GB";
                            else
                                maxSizeDesc = config.maxFileBytes + " Bytes";

                            message = "<div><b>File:</b> " + filename + "</div>";
                            message = message.concat("<div><b>Issue:</b> Maximum file size exceeded</div>");
                            message = message.concat("<div><b>Maximum:</b> " + maxSizeDesc + "</div>");
                        }

                        if (error.name === "InvalidFileExtensionError") {
                            message = "<div><b>File:</b> " + filename + "</div>";
                            message = message.concat("<div><b>Issue:</b> Invalid file extension</div>");
                            message = message.concat("<div><b>Accepted:</b> " + config.allowedExts.toString() + "</div>");
                        }

                        if (error.name === "InvalidFileTypeError") {
                            message = "<div><b>File:</b> " + filename + "</div>";
                            message = message.concat("<div><b>Issue:</b> Invalid file type</div>");
                            message = message.concat("<div><b>Accepted:</b> " + config.allowedTypes.toString() + "</div>");
                        }

                        rejected.push(message);
                        this.progressBar.remove();
                    }
                });
            });

            function enableScreen() {
                $("#lnkGoBack").show();
                $("#lblGoBackDisabled").hide();
                $("#txtSearchFilename").prop("disabled", false);
                $("#add").removeAttr("disabled");
                $("select").prop("disabled", false);
                $("#main-menu").show();
                $("#menu-login").show();
            }

            function disableScreen() {
                $("#lnkGoBack").hide();
                $("#lblGoBackDisabled").show();
                $("#txtSearchFilename").prop("disabled", true);
                $("#add").attr("disabled", "disabled");
                $("select").prop("disabled", true);
                $("#main-menu").hide();
                $("#menu-login").hide();
            }
        }
    }
})(jQuery);