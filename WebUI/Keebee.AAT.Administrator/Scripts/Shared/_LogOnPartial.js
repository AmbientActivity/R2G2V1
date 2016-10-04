/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Maintenance/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

; (function ($) {

    shared._logonpartial = {
        init: function () {
            var lnkChangePassword = $("#change-password");

            lnkChangePassword.click(function() {
                var title = "<span class='glyphicon glyphicon-pencil'></span>";
                var message;

                $.ajax({
                    type: "GET",
                    async: false,
                    url: site.url + "Home/GetChangePasswordView",
                    success: function(data) {
                        message = data;
                    }
                });

                BootstrapDialog.show({
                    title: title + "Change Password",
                    message: message,
                    onshown: function() {
                        $("#txtOldPassword").focus();
                        $("#txtOldPassword").val("");
                    },
                    closable: false,
                    buttons: [
                        {
                            label: "Cancel",
                            action: function(dialog) {
                                dialog.close();
                            }
                        }, {
                            label: "OK",
                            cssClass: "btn-primary",
                            action: function(dialog) {
                                var result = attemptToChangePassword();

                                if (result.Success) {
                                    dialog.close();
                                    $("body").css("cursor", "default");
                                        BootstrapDialog.show({
                                            title: "Success",
                                            closable: false,
                                            type: BootstrapDialog.TYPE_SUCCESS,
                                            message: "Password successfully changed",
                                            buttons: [
                                            {
                                                label: "Close",
                                                action: function (d) {
                                                    d.close();
                                                }
                                            }]
                                        });
                                } else {
                                    $("#validation-container").show();
                                    $("#validation-container").html("");
                                    $("body").css("cursor", "default");
                                    var html = "<ul><li>" + result.ErrorMessage + "</li></ul>";
                                    $("#validation-container").append(html);
                                    $("body").css("cursor", "default");
                                }
                            }
                        }
                    ]
                });
            });

            function attemptToChangePassword() {
                var result;
                var jsonData = getChangePasswordDetailFromDialog();

                $.ajax({
                    type: "GET",
                    async: false,
                    traditional: true,
                    dataType: "json",
                    data: jsonData,
                    url: site.url + "Home/AttemptToChangePassword",
                    success: function (data) {
                        result = data;

                    },
                    error: function (data) {
                        result = data;
                    }
                });

                return result;
            }

            function getChangePasswordDetailFromDialog() {
                var oldpassword = $.trim($("#txtOldPassword").val());
                var newpassword = $.trim($("#txtNewPassword").val());
                var retypepassword = $.trim($("#txtRetypeNewPassword").val());

                return {
                    OldPassword: oldpassword, NewPassword: newpassword, RetypedNewPassword: retypepassword
                };
            };
        }
    }
})(jQuery);