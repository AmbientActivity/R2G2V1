﻿/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Utilities/SharedLibrary.js
 * Author: John Charlton
 * Date: 2017-06
 */

; (function ($) {

    utilities.sharedlibrary = {
        show: function (options) {
            var config = {
                url: null,
                mediaPathTypeDesc: null,
                params: {}
            };

            return new Promise(function (resolve, reject) {

                if ((typeof options !== "undefined") && (options !== null)) {
                    if (options.controller === null) reject("Controller cannot be null");
                    if (options.mediaPathTypeDesc === null) reject("MediaPathType cannot be null");
                }

                $.extend(config, options);

                var title = "<i class='fa fa-share-alt fa-md' style='color: #fff'></i>";

                $.get(config.url, config.params)
                    .done(function(message) {
                        if (message.length === 0) {
                            var hasHave = "has";
                            if (config.mediaPathTypeDesc.endsWith("s"))
                                hasHave = "have";

                            BootstrapDialog.show({
                                title: title + " Add <b>" + config.mediaPathTypeDesc + "</b> From Shared Library",
                                message: $("<div></div>")
                                    .append("All available " +
                                        config.mediaPathTypeDesc +
                                        " " +
                                        hasHave +
                                        " already been added to the system profile."),
                                onshown: function() { $("body").css("cursor", "default"); },
                                closable: false,
                                buttons: [
                                    {
                                        label: "OK",
                                        cssClass: "btn-primary",
                                        action: function(dialog) {
                                            dialog.close();
                                            reject();
                                        }
                                    }
                                ]
                            });
                        } else {
                            var ids = [];
                            var sharedLibraryDialog = new
                            BootstrapDialog({
                                title: title + " Add <b>" + config.mediaPathTypeDesc + "</b> From Shared Library",
                                message: $("<div></div>").append(message),

                                closable: false,
                                buttons: [
                                    {
                                        label: "Cancel",
                                        action: function (dialog) {
                                            dialog.close();
                                            reject();
                                        }
                                    }, {
                                        label: "OK",
                                        cssClass: "btn-primary",
                                        action: function (dialog) {
                                            $("input[name='shared_files']:checked").each(function (item, value) {
                                                ids.push(value.id);
                                            });
                                            resolve({ dialog: dialog, streamIds: ids });
                                        }
                                    }
                                ]
                            });
                            sharedLibraryDialog.realize();
                            sharedLibraryDialog.open();
                        }
                    })
                    .error(function (result) {
                        BootstrapDialog.show({
                            title: title + "Error",
                            message: $("<div>An error occurred loading the shared library.</div>").append("<div>" + result + "</div>"),
                            type: BootstrapDialog.TYPE_DANGER,
                            closable: false,
                            buttons: [
                                 {
                                    label: "OK",
                                    cssClass: "btn-primary",
                                    action: function(dialog) {
                                        reject(dialog);
                                    }
                                }
                            ]
                        });
                    });
            });
        }
    }
})(jQuery);;