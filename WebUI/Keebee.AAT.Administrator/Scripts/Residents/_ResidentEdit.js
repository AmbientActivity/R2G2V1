/*!
 * 1.0 Keebee AAT Copyright © 2016
 * Residents/_ResidentEdit.js
 * Author: John Charlton
 * Date: 2016-08
 */


; (function ($) {

    residents.edit = {
        init: function (options) {

            var config = {
                profilePicturePlaceholder: ""
            };

            $.extend(config, options);

            var imgProfilePicture = $("#profile-picture");
            var inputFileUpload = $("#fileupload");
            var cmdRemovePicture = $("#remove-picture");
            var progressDesc = $("#progress");
            var progressBar = $("#progressBar");

            // initialize the remove button
            if (imgProfilePicture.attr("alt") === "notexists")
                cmdRemovePicture.prop("disabled", true);
            else
                cmdRemovePicture.prop("disabled", false);

            cmdRemovePicture.click(function () {
                imgProfilePicture.attr("src", config.profilePicturePlaceholder);
                imgProfilePicture.attr("alt", "notexists");
                cmdRemovePicture.prop("disabled", true);
            });

            inputFileUpload.change(function () {
                $(this).simpleUpload(site.url + "Residents/UploadProfilePicture",
                {
                    allowedExts: ["jpg", "jpeg", "png", "gif, bmp"],
                    allowedTypes: ["image/jpg", "image/jpeg", "image/png", "image/gif", "image/bmp"],
                    maxFileSize: 5000000, //5MB in bytes

                    start: function (file) {
                        progressBar.show();
                        $("button").prop("disabled", true);
                        $("input").prop("disabled", true);
                        $(".btn").addClass("disabled");
                    },

                    progress: function (progress) {
                        progressDesc.html("Progress: " + Math.round(progress) + "%");
                        progressBar.css("width", progress/2 + "%");
                    },

                    success: function (data) {
                        imgProfilePicture.attr("src", "data:image/jpg;base64," + data);
                        imgProfilePicture.attr("alt", "exists");
                        cmdRemovePicture.removeAttr("disabled");
                        progressDesc.html("");
                        progressBar.width(0);
                        progressBar.hide();
                        $("button").prop("disabled", false);
                        $("input").prop("disabled", false);
                        $(".btn").removeClass("disabled");
                    },

                    error: function (error) {
                        var errorName = error.name;

                        if (errorName === "InvalidFileExtensionError") {
                            BootstrapDialog.show({
                                title: "Invalid File Extension",
                                type: BootstrapDialog.TYPE_WARNING,
                                message: "Can only upload files of type jpg, png, gif or bmp",
                                buttons: [
                                    {
                                        label: "OK",
                                        action: function(dialog) {
                                            dialog.close();
                                        }
                                    }
                                ]
                            });
                        } else {
                            BootstrapDialog.show({
                                title: "Upload Error",
                                type: BootstrapDialog.TYPE_DANGER,
                                message: errorName + ": " + error.message,
                                buttons: [
                                    {
                                        label: "OK",
                                        action: function (dialog) {
                                            dialog.close();
                                        }
                                    }
                                ]
                            });
                        }
                    }
                });
            });
        }
    }
})(jQuery);