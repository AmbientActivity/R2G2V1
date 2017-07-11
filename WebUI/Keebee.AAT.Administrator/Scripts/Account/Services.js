/*!
 * 1.0 Keebee AAT Copyright © 2016
 * Utilities/Account.js
 * Author: John Charlton
 * Date: 2016-10
 */

; (function ($) {

    account.services = {
        init: function() {
            var lnkChangePassword = $("#change-password");
            var cmdLogin = $("#login");
            var cmdLogOff = $("#logoff");

            $("#txtPassword").focus();

            $("#txtPassword").keyup(function (e) {
                e.stopImmediatePropagation(); // stop from firing twice
                if (e.keyCode === 13) {
                    login();
                }
            });

            // make sure password always has focus
            $("#ddlUsernames").change(function (e) {
                $("#txtPassword").focus();
            });

            cmdLogin.click(function (e) {
                e.stopImmediatePropagation(); // stop from firing twice
                login();
            });

            cmdLogOff.click(function () {
                $.get(site.url + "Account/LogOff")
                    .done(function () {
                        location.reload();
                });
            });

            lnkChangePassword.click(function () {
                utilities.partialview.show({
                    url: site.url + "Account/GetChangePasswordView",
                    type: BootstrapDialog.TYPE_PRIMARY,
                    title: "<span class='glyphicon glyphicon-pencil' style='color: #fff'></span> Change Password",
                    focus: "txtOldPassword",
                    cancelled: function() {},
                    callback: function(dialog) {
                        var jsonData = getChangePasswordDetailFromDialog();

                        utilities.job.execute({
                            url: site.url + "Account/ValidatePasswordChange",
                            type: "POST",
                            params: jsonData
                        })
                        .then(function (validateResult) {
                            if (validateResult.ValidationMessage === null) {
                                dialog.close();

                                utilities.job.execute({
                                    url: site.url + "Account/ChangePassword",
                                    type: "POST",
                                    params: jsonData
                                })
                                .then(function() {
                                    dialog.close();
                                    utilities.alert.show({
                                        type: BootstrapDialog.TYPE_SUCCESS,
                                        title: "Password Change",
                                        message: "Password successfully changed",
                                    });
                                });
                            } else {
                                $("#validation-container").show();
                                $("#validation-container").html("");
                                var html = "</br><ul><li>" + validateResult.ValidationMessage + "</li></ul>";
                                $("#validation-container").append(html);
                            }
                        });
                    }
                });
            });

            function getChangePasswordDetailFromDialog() {
                var oldpassword = $.trim($("#txtOldPassword").val());
                var newpassword = $.trim($("#txtNewPassword").val());
                var retypepassword = $.trim($("#txtRetypeNewPassword").val());

                return {
                    OldPassword: oldpassword, NewPassword: newpassword, RetypedNewPassword: retypepassword
                };
            };

            function login() {
                $("body").css("cursor", "progress");
                cmdLogin.prop("disabled", true);

                var jsonData = getCredentials();

                utilities.job.execute({
                    url: site.url + "Account/ValidateUser",
                    type: "POST",
                    params: jsonData
                })
                .then(function (validateResult) {
                    $("body").css("cursor", "default");
                    if (validateResult.ValidationMessage === null) {
                        utilities.job.execute({
                            url: site.url + "Account/LoginUser",
                            type: "POST",
                            params: jsonData
                        })
                        .then(function () {
                            showSpinner();
                            window.location.href = site.url + "Home";
                        })
                        .catch(function (loginResult) {
                            showError(loginResult.ErrorMessage);
                        });
                    } else {
                        $("#login-container").show();
                        $("#validation-container").show();

                        $("#validation-container").html("");
                        var html = "<br/><ul><li>" + validateResult.ValidationMessage + "</li></ul>";
                        $("#validation-container").append(html);

                        cmdLogin.prop("disabled", false);
                    }
                })
                .catch(function (validateResult) {
                    showError(validateResult.ErrorMessage);
                });
            }

            function showSpinner() {
                // hide login containers
                $("#login-container").hide();
                $("#validation-container").hide();
                $("#error-container").hide();

                $("#spinner-container").show();
            }

            function showError(message) {
                $("#validation-container").hide();
                $("#spinner-container").hide();
                $("#error-container").html("");
                $("#error-container")
                    .append("<div><h3>Login Error</h3><div>")
                    .append("<div>" + message + "</div>");
                $("#error-container").show();
            }

            function getCredentials() {
                var username = $.trim($("#ddlUsernames").val());
                var password = $.trim($("#txtPassword").val());

                return {
                    Username: username, Password: password
                };
            };
        }
    }
})(jQuery);