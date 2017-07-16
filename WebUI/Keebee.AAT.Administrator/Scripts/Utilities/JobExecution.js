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
                url: null,
                type: "GET",
                dataType: "json",
                waitMessage: null,
                successVerbage: null,
                params: {}
            };

            return new Promise(function (resolve, reject) {

                if ((typeof options !== "undefined") && (options !== null)) {
                    if (options.url === null) reject("URL cannot be null");
                }

                $.extend(config, options);

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
                        url: config.url,
                        dataType: config.dataType,
                        data: config.params,
                        success: function (data) {
                            if (typeof inProgressDialog !== "undefined")
                                inProgressDialog.close();

                            if (data.Success) {
                                if (config.successVerbage !== null) {
                                    utilities.alert.show({
                                        title: "Success",
                                        type: BootstrapDialog.TYPE_SUCCESS,
                                        message: config.successVerbage,
                                        buttonOKClass: "btn-success"
                                    });
                                }
                                resolve(data);

                            } else {
                                utilities.alert.show({
                                    title: "Error",
                                    type: BootstrapDialog.TYPE_DANGER,
                                    message: "The following error occured:\n" + data.ErrorMessage
                                });
                                reject(data);
                            }
                        },
                        error: function (request) {
                            if (typeof inProgressDialog !== "undefined")
                                inProgressDialog.close();

                            utilities.alert.show({
                                title: "Error",
                                type: BootstrapDialog.TYPE_DANGER,
                                message: "The following error occured:\n" + request.statusText
                            });
                            reject();
                        }
                    });
                });
            }
        }
    }
})(jQuery);