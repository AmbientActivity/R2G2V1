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
                type: "GET",
                dataType: "json",
                title: null,
                waitMessage: null,
                successVerbage: null,
                params: {}
            };

            if ((typeof options !== "undefined") && (options !== null)) {
                if (options.controller === null) reject("Controller name cannot be null");
                if (options.action === null) reject("Action name cannot be null");
                if (options.title === null) reject("Title cannot be null");
            }

            $.extend(config, options);

            return new Promise(function (resolve, reject) {
                if (config.waitMessage !== null) {
                    utilities.inprogress.show({ message: config.waitMessage })
                        .then(function(dialog) {
                            execute(dialog)
                                .then(function(data) {
                                    resolve(data);
                                })
                                .catch(function() {
                                    reject();
                                });
                        });
                } else {
                    execute()
                        .then(function (data) {
                            resolve(data);
                        })
                        .catch(function () {
                            reject();
                        });
                }
            });

            function execute(inProgressDialog) {
                return new Promise(function (resolve, reject) {
                    $.ajax({
                        type: config.type,
                        url: site.url + config.controller + "/" + config.action,
                        dataType: config.dataType,
                        data: config.params,
                        success: function (data) {
                            if (typeof inProgressDialog !== "undefined")
                                inProgressDialog.close();

                            if (data.Success) {
                                if (config.successVerbage !== null) {
                                    BootstrapDialog.show({
                                        title: "Success",
                                        closable: false,
                                        type: BootstrapDialog.TYPE_SUCCESS,
                                        message: config.successVerbage,
                                        buttons: [
                                            {
                                                label: "Close",
                                                action: function (dlg) {
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
                                            action: function (dlg) {
                                                dlg.close();
                                            }
                                        }
                                    ]
                                });
                                reject(data);
                            }
                        },
                        error: function (data) {
                            if (typeof inProgressDialog !== "undefined")
                                inProgressDialog.close();

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
                });
            }
        }
    }
})(jQuery);;