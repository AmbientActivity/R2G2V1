/*!
 * 1.0 Keebee AAT Copyright © 2016
 * Home/Index.js
 * Author: John Charlton
 * Date: 2016-09
 */

; (function ($) {

    home.index = {
        init: function () {
            var cmdLogin = $("#login");
            var cmdLogOff = $("#logoff");

            function attemptToLogin() {
                var result;
                var jsonData = getLoginDetailFromDialog();

                $.ajax({
                    type: "GET",
                    async: false,
                    data: jsonData,
                    dataType: "json",
                    url: site.url + "Home/Login",
                    success: function (data) {
                        result = data;
                    },
                    error: function (data) {
                        result = data;
                    }
                });

                return result;
            }

            function getLoginDetailFromDialog() {
                var username = $.trim($("#ddlUsernames").val());
                var password = $.trim($("#txtPassword").val());

                return {
                    Username: username, Password: password
                };
            };

            function changePassword() {
                var result;
                var jsonData = getChangePasswordDetailFromDialog();

                $.ajax({
                    type: "GET",
                    async: false,
                    data: jsonData,
                    dataType: "json",
                    url: site.url + "Home/ChangePassword",
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
                    OldPassword: oldpassword, NewPassword: newpassword, RetypeNewPassword: retypepassword
                };
            };

            cmdLogin.click(function () {
                var message;

                $.ajax({
                    type: "GET",
                    async: false,
                    url: site.url + "Home/GetLoginView",
                    success: function (data) {
                        message = data;
                    }
                });

                BootstrapDialog.show({
                    title: "Login",
                    message: message,
                    onshown: function () {
                        $("#txtPassword").focus();
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
                                $("body").css("cursor", "wait");
                                var result = attemptToLogin();

                                if (result.Success) {
                                    dialog.close();
                                    location.reload();
                                    $("body").css("cursor", "default");
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

            cmdLogOff.click(function() {
                $.ajax({
                    type: "GET",
                    url: site.url + "Home/LogOff",
                    success: function() {
                        location.reload();
                    }
                });
            });
        }
    }
})(jQuery);