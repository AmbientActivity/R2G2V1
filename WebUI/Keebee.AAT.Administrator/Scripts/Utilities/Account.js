/*!
 * 1.0 Keebee AAT Copyright © 2016
 * Utilities/Account.js
 * Author: John Charlton
 * Date: 2016-10
 */

; (function ($) {

    utilities.account = {
        init: function () {
            var lnkChangePassword = $("#change-password");
            var cmdLogin = $("#login");
            var cmdLogOff = $("#logoff");

            $("#error-container").hide();

            cmdLogin.click(function (e) {
                e.stopImmediatePropagation(); // stop from firing twice

                $("#login-container").hide();
                $("body").css("cursor", "progress");
                cmdLogin.prop("disabled", true);

                var jsonData = getCredentials();

                $.get(site.url + "Account/AttemptToLogin", jsonData)
                    .done(function (result) {
                        cmdLogin.prop("disabled", false);
                        $("body").css("cursor", "default");
                        if (result.Success) {
                            $("#login-container").hide();
                            $("#validation-container").hide();
                            $("#error-container").hide();

                            $("#load-message").html("<h3>Loggin in...</h3>");
                            $("#loading-container").show();
                            window.location.href = site.url + "Home";
                        } else {
                            $("#login-container").show();
                            $("#validation-container").show();

                            $("#validation-container").html("");
                            var html = "<br/><ul><li>" + result.ErrorMessage + "</li></ul>";
                            $("#validation-container").append(html);
                        }
                    })
                    .error(function (result) {
                        $("#validation-container").hide();
                        $("#loading-container").hide();
                        $("#error-container").html("");
                        $("#error-container")
                            .append("<div><h3>Login Error</h3><div>")
                            .append("<div>" + result + "</div>");
                        $("#error-container").show();
                    });
            });

            function getCredentials() {
                var username = $.trim($("#ddlUsernames").val());
                var password = $.trim($("#txtPassword").val());

                return {
                    Username: username, Password: password
                };
            };

            cmdLogOff.click(function () {
                $.get(site.url + "Account/LogOff")
                    .done(function () {
                        location.reload();
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
                                                var html = "<ul><li>" + result.ErrorMessage + "</li></ul>";
                                                $("#validation-container").append(html);
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