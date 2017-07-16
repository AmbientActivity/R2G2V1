/*!
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
                maxFileBytes: 5000000, //5MB in bytes
                maxFileUploads: 10,
                callback: function () { }
            };

            $.extend(config, options);

            $("input[type=file]").unbind("change");

            $("input[type=file]").change(function () {
                var totalFiles;
                var pendingFile = 1;
                var successful = [];
                var rejected = [];

                $(this).simpleUpload(config.url, {
                    allowedExts: config.allowedExts,
                    allowedTypes: config.allowedTypes,
                    maxFileSize: config.maxFileBytes,

                    /*
                    * Each of these callbacks are executed for each file
                    * To add callbacks that are executed only once, see init() and finish().
                    *
                    * "this" is an object that can carry data between callbacks for each file.
                    * Data related to the upload is stored in this.upload.
                    */
                    init: function (totalUploads) {
                        totalFiles = totalUploads;
                        successful = [];
                        rejected = [];
                    },

                    start: function (file) {
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
                                successful.push(data.Filename);

                            this.block.fadeOut(400, function () { });
                        } else {
                            var message = data.ErrorMessage; // optionally display this
                        }
                    },
                    cancel: function () {
                        this.block.fadeOut(400, function () { });
                    },
                    finish: function () {
                        $("#uploads").html("");
                        enableScreen();
                        config.callback(successful, rejected);
                    },
                    error: function (error) {
                        var block = this.block;
                        var html = block[0].innerHTML;
                        // extract filename from inner html
                        var filename = html.substring(html.indexOf("<b>"), html.indexOf("</b>") + 4);
                        rejected.push(filename + " - " + error.message);
                        this.progressBar.remove();
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