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
                        var errorName = error.name;

                        if (errorName === "InvalidFileExtensionError") {
                            utilities.alert.show({
                                type: BootstrapDialog.TYPE_WARNING,
                                title: "Invalid File Extension",
                                message: "Can only upload files of type jpg, png, gif or bmp",
                                buttonOKClass: "btn-danger"
                            });
                        } else {
                            utilities.alert.show({
                                type: BootstrapDialog.TYPE_DANGER,
                                title: "Upload Error",
                                message: errorName + ": " + error.message,
                                buttonOKClass: "btn-danger"
                            });
                        }
                    }
                });
            });
        }
    }
})(jQuery);