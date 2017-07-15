﻿/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Utilities/Validation.js
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
                callback: function () { }
            };

            $.extend(config, options);

            $("input[type=file]").unbind("change");

            $("input[type=file]").change(function () {
                var totalFiles;
                var pendingFile = 1;
                var filenames = [];

                $(this).simpleUpload(config.url, {
                    allowedExts: config.allowedExts,
                    allowedTypes: config.allowedTypes,
                    maxFileSize: 1000000000, //1GB in bytes

                    /*
                        * Each of these callbacks are executed for each file.
                        * To add callbacks that are executed only once, see init() and finish().
                        *
                        * "this" is an object that can carry data between callbacks for each file.
                        * Data related to the upload is stored in this.upload.
                        */
                    init: function (totalUploads) {
                        totalFiles = totalUploads;
                    },

                    start: function (file) {
                        //upload started
                        disableScreen();

                        this.block = $('<div class="block"></div>');
                        this.progressBar = $('<div class="progressBar"></div>');
                        this.cancelButton = $('<div class="cancelButton">x</div>');

                        var that = this;
                        this.cancelButton.click(function () {
                            that.upload.cancel();
                        });

                        this.block.html("");
                        this.block.append(this.progressBar).append(this.cancelButton);

                        this.block.append("<div>Uploading <b>" + file.name + "</b> (" + pendingFile + " of " + totalFiles + " file(s))...</div>");
                        $("#uploads").append(this.block);
                        pendingFile++;
                    },

                    progress: function (progress) {
                        this.progressBar.width(progress + "%");
                    },
                    success: function (data) {
                        //upload successful

                        this.progressBar.remove();

                        /*
                            * Just because the success callback is called doesn't mean your
                            * application logic was successful, so check application success.
                            *
                            * Data as returned by the server on...
                            * success:	{"success":true,"format":"..."}
                            * error:	{"success":false,"error":{"code":1,"message":"..."}}
                            */
                 
                        if (data.Success) {
                            if (data.Filename !== null)
                                filenames.push(data.Filename);

                            this.block.fadeOut(400, function () { });
                        } else {
                            //our application returned an error
                            var message = data.ErrorMessage;
                            var errorDiv = $('<div class="error"></div>').text(message);
                            this.block.append(errorDiv);
                        }
                    },

                    cancel: function () {
                        this.block.fadeOut(400, function () { });
                    },
                    finish: function () {
                        $("#uploads").html("");
                        enableScreen();
                        config.callback(filenames);
                    },
                    error: function (error) {
                        //upload failed
                        this.progressBar.remove();
                        var message = error.ErrorMessage;
                        var errorDiv = $('<div class="error"></div>').text(message);
                        this.block.append(errorDiv);
                    }
                });
            });

            function enableScreen() {
                $("#lnkGoBack").show();
                $("#lblGoBackDisabled").hide();
                $("#txtSearchFilename").prop("disabled", false);
                $("#add").prop("disabled", false);
                $("select").prop("disabled", false);
                $("#main-menu").show();
                $("#menu-login").show();
            }

            function disableScreen() {
                $("#lnkGoBack").hide();
                $("#lblGoBackDisabled").show();
                $("#txtSearchFilename").prop("disabled", true);
                $("#add").prop("disabled", true);
                $("select").prop("disabled", true);
                $("#main-menu").hide();
                $("#menu-login").hide();
            }
        }
    }
})(jQuery);