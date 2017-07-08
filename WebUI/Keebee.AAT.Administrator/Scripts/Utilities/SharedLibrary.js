/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Utilities/SharedLibrary.js
 * Author: John Charlton
 * Date: 2017-06
 */

; (function ($) {

    utilities.sharedlibrary = {
        show: function (options) {
            var config = {
                controller: null,
                mediaPathTypeDesc: null,
                params: {}
            };

            if ((typeof options !== "undefined") && (options !== null)) {
                if (options.controller === null) reject("Controller cannot be null");
                if (options.mediaPathTypeDesc === null) reject("MediaPathType cannot be null");
            }

            $.extend(config, options);

            return new Promise(function (resolve, reject) {
                var title = "<span class='glyphicon glyphicon-link' style='color: #fff'></span>";

                $.get(site.url + config.controller + "/GetSharedLibarayLinkView/", config.params)
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
                                id: "shared-library-modal",
                                title: title + " Add <b>" + config.mediaPathTypeDesc + "</b> From Shared Library",
                                message: $("<div></div>").append(message),

                                closable: false,
                                buttons: [
                                    {
                                        label: "Cancel",
                                        action: function(dialog) {
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

                                            dialog.close();
                                        }
                                    }
                                ]
                            });
                            sharedLibraryDialog.realize();
                            sharedLibraryDialog.open();
                            $(document).on("hidden.bs.modal", "#shared-library-modal", function () {
                                $(document).off("hidden.bs.modal", "#shared-library-modal");
                                resolve({ streamIds: ids });
                            });
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
                                        dialog.close();
                                        reject();
                                    }
                                }
                            ]
                        });
                    });
            });
        }
    }
})(jQuery);;