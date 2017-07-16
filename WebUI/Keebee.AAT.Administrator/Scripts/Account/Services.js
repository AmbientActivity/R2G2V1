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
            var lnkResetCaregiver = $("#reset-caregiver");
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
                                        title: "Success",
                                        message: "Your password has successfully been changed.",
                                        buttonOKClass: "btn-success"
                                    });
                                });
                            } else {
                                utilities.validation.show({
                                    container: "validation-container",
                                    messages: [validateResult.ValidationMessage],
                                    beginWithLineBreak: true
                                });
                            }
                        })
                        .catch(function (error) {
                            utilities.alert.show({
                                type: BootstrapDialog.TYPE_DANGER,
                                message: "An unexpected error occurred.\n" + error
                            });
                        });
                    }
                });
            });

            lnkResetCaregiver.click(function() {
                utilities.confirm.show({
                    type: BootstrapDialog.TYPE_PRIMARY,
                    title: "Password Reset",
                    message: "Reset caregiver pawword?"
                })
                .then(function (confirm) {
                    if (confirm) {
                        utilities.job.execute({
                            url: site.url + "Account/ResetCaregiver"
                        })
                        .then(function() {
                            utilities.alert.show({
                                type: BootstrapDialog.TYPE_SUCCESS,
                                title: "Success",
                                message: "Caregiver password has successfully been reset.",
                                buttonOKClass: "btn-success"
                            });
                        })
                        .catch(function(error) {
                            utilities.alert.show({
                                type: BootstrapDialog.TYPE_DANGER,
                                message: "An unexpected error occurred.\n" + error
                            });
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
                        utilities.validation.show({
                            container: "validation-container",
                            messages: [validateResult.ValidationMessage]
                        });

                        cmdLogin.prop("disabled", false);
                    }
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