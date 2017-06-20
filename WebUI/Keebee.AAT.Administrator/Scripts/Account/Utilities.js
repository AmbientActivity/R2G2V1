/*!
 * 1.0 Keebee AAT Copyright © 2016
 * Account/Utilities.js
 * Author: John Charlton
 * Date: 2016-10
 */

; (function ($) {

    account.utilities = {
        init: function () {
            var lnkChangePassword = $("#change-password");
            var cmdLogin = $("#login");
            var cmdLogOff = $("#logoff");

            function attemptToLogin() {
                return new Promise(function(resolve, reject) {
                    var jsonData = getLoginDetailFromDialog();

                    $.get(site.url + "Account/Login", jsonData)
                        .done(function (result) {
                            resolve(result);
                        })
                        .error(function (result) {
                            reject(result);
                        });
                });
            }

            function getLoginDetailFromDialog() {
                var username = $.trim($("#ddlUsernames").val());
                var password = $.trim($("#txtPassword").val());

                return {
                    Username: username, Password: password
                };
            };

            cmdLogin.click(function () {
                $.get(site.url + "Account/GetLoginView")
                    .done(function (message) {
                        BootstrapDialog.show({
                            title: "R2G2 Login",
                            message: $("<div></div>").append(message),
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
                                    hotkey: 13,  // enter
                                    action: function (dialog) {
                                        $("body").css("cursor", "wait");
                                        attemptToLogin().then(function (result) {
                                            if (result.Success) {
                                                dialog.close();
                                                location.reload();
                                                $("body").css("cursor", "default");
                                            } else {
                                                $("#validation-container").show();
                                                $("#validation-container").html("");
                                                $("body").css("cursor", "default");
                                                var html = "<br/><ul><li>" + result.ErrorMessage + "</li></ul>";
                                                $("#validation-container").append(html);
                                                $("body").css("cursor", "default");
                                            }
                                        });
                                    }
                                }
                            ]
                        });
                    });
            });

            cmdLogOff.click(function () {
                $.ajax({
                    type: "GET",
                    url: site.url + "Account/LogOff",
                    success: function () {
                        location.reload();
                    }
                });
            });

            lnkChangePassword.click(function () {
                var title = "<span class='glyphicon glyphicon-pencil'></span>";

                $.get(site.url + "Account/GetChangePasswordView")
                    .done(function (message) {
                        BootstrapDialog.show({
                            title: title + " Change Password",
                            message: $("<div></div>").append(message),
                            onshown: function () {
                                $("#txtOldPassword").focus();
                                $("#txtOldPassword").val("");
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
                                        attemptToChangePassword().then(function(result) {
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
                                        });
                                    }
                                }
                            ]
                        });
                    });
            });

            function attemptToChangePassword() {
                return new Promise(function(resolve, reject) {
                    var jsonData = getChangePasswordDetailFromDialog();

                    $.get(site.url + "Account/AttemptToChangePassword", jsonData)
                        .done(function(result) {
                            resolve(result);
                         })
                         .error(function (result) {
                            reject(result);
                        });
                });
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