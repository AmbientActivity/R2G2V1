/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Utilities/JobExecution.js
 * Author: John Charlton
 * Date: 2017-06
 */

; (function ($) {

    utilities.job = {
        execute: function (options) {
            var config = {
                controller: null,
                action: null,
                title: null,
                waitMessage: "Please wait...",
                successVerbage: null,
                params: {}
            };

            if (options.controller === null) reject("Controller name cannot be null");
            if (options.action === null) reject("Action name cannot be null");
            if (options.title === null) reject("Title cannot be null");
            if (options.waitMessage === null) reject("Wait Message cannot be null");

            $.extend(config, options);

            return new Promise(function(resolve, reject) {
                BootstrapDialog.show({
                    type: BootstrapDialog.TYPE_INFO,
                    title: config.title,
                    message: config.waitMessage,
                    closable: false,
                    onshown: function (dialog) {
                        $("body").css("cursor", "wait");
                   
                        $.get({
                            url: site.url + config.controller + "/" + config.action,
                            dataType: "json",
                            data: config.params,
                            success: function (data) {
                                $("body").css("cursor", "default");
                                dialog.close();

                                if (data.ErrorMessage === null) {
                                    if (config.successVerbage !== null) {
                                        BootstrapDialog.show({
                                            title: "Success",
                                            closable: false,
                                            type: BootstrapDialog.TYPE_SUCCESS,
                                            message: config.successVerbage,
                                            buttons: [
                                                {
                                                    label: "Close",
                                                    action: function(dlg) {
                                                        dlg.close();
                                                    }
                                                }
                                            ]
                                        });
                                    }
                                    resolve(data);
                                } else {
                                    BootstrapDialog.show({
                                        title: "Error",
                                        closable: false,
                                        type: BootstrapDialog.TYPE_DANGER,
                                        message: "The following error occured:\n" + data.ErrorMessage,
                                        buttons: [
                                            {
                                                label: "Close",
                                                action: function(dlg) {
                                                    dlg.close();
                                                }
                                            }
                                        ]
                                    });
                                    reject(data);
                                }
                            },
                            error: function (data) {
                                $("body").css("cursor", "default");
                                dialog.close();
                                BootstrapDialog.show({
                                    title: "Error",
                                    closable: false,
                                    type: BootstrapDialog.TYPE_DANGER,
                                    message: "The following error occured:\n" + data.ErrorMessage,
                                    buttons: [{
                                        label: "Close",
                                        action: function (dlg) {
                                            dlg.close();
                                        }
                                    }]
                                });
                                reject(data);
                            }
                        });
                    }
                });
            });
        }
    }
})(jQuery);;