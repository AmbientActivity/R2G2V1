/*!
 * 1.0 Keebee AAT Copyright © 2017
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

            var maxFileSize = 5000000; //5MB in bytes
            var allowedExts = ["jpg", "jpeg", "png", "gif"];
            var allowedTypes = ["image/jpg", "image/jpeg", "image/png", "image/gif"];

            // filter the file type "accepts"
            inputFileUpload.prop("accept", allowedTypes);

            // initialize the remove button
            cmdRemovePicture.prop("disabled", imgProfilePicture.attr("alt") === "notexists"); 

            $(function () {
                // Bootstrappable btn-group with checkables inside
                // - hide inputs and set bootstrap class
                $(".btn-group label.btn input[type=radio]")
                  .hide()
                  .filter(":checked").parent(".btn").addClass("active");
            });

            $("input:radio").change(function () {
                var id = $(this).attr("id");
                var val = $(this).val();

                if (val === "on") {
                    $("#radGender").val(id);
                }
            });

            cmdRemovePicture.click(function () {
                imgProfilePicture.attr("src", config.profilePicturePlaceholder);
                imgProfilePicture.attr("alt", "notexists");
                cmdRemovePicture.prop("disabled", true);
            });

            inputFileUpload.change(function () {
                $(this).simpleUpload("Residents/UploadProfilePicture",
                {
                    allowedExts: allowedExts,
                    allowedTypes: allowedTypes,
                    maxFileSize: maxFileSize,

                    start: function (file) {
                        progressBar.show();
                        $("button").prop("disabled", true);
                        $("input").prop("disabled", true);
                        $(".btn").addClass("disabled");
                    },

                    progress: function (progress) {
                        progressDesc.html("Progress: " + Math.round(progress) + "%");
                        progressBar.css("width", progress + "%");
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
                        var message;

                        if (error.name === "MaxFileSizeError") {
                            message = "<div>" + filename + " - Maximum file size exceeded</div>";
                            message = message.concat("<div><b>Maximum allowed size:</b> " + maxFileSize + " bytes</div>");
                        }

                        else if (error.name === "InvalidFileExtensionError") {
                            message = "<div>" + filename + " - Invalid file extension</div>";
                            message = message.concat("<div><b>Allowed extensions:</b> " + allowedExts.toString() + "</div>");
                        }

                        else if (error.name === "InvalidFileTypeError") {
                            message = "<div>" + filename + " - Invalid file type</div>";
                            message = message.concat("<div><b>Allowed types:</b> " + allowedTypes.toString() + "</div>");
                        }
                        else {
                            message = "<div><b>" + error.name + "</b>: " + error.message + "</div>";
                        }

                        utilities.alert.show({
                            type: BootstrapDialog.TYPE_WARNING,
                            title: "File Not Uploaded",
                            message: message
                        });
                    }
                });
            });
        }
    }
})(jQuery);