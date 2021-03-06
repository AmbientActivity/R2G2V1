﻿/*!
 * 1.0 Keebee AAT Copyright © 2017
 * Utilities/PartialView.js
 * Author: John Charlton
 * Date: 2017-06
 */

; (function ($) {

    sharedlibraryadd.view = {
        show: function (options) {
            var config = {
                profileId: 0,
                mediaPathTypeId: 0,
                mediaPathTypeDesc: "",
                mediaPathTypeCategory: ""
            };

            return new Promise(function(resolve, reject) {
                $.extend(config, options);
                utilities.job.execute({
                    url: "SharedLibraryAdd/GetView",
                    params: {
                        profileId: config.profileId,
                        mediaPathTypeId: config.mediaPathTypeId,
                        mediaPathTypeDesc: config.mediaPathTypeDesc,
                        mediaPathTypeCategory: config.mediaPathTypeCategory
                    }
                })
                .then(function (result) {
                    var addSharedDialog = new BootstrapDialog({
                        type: BootstrapDialog.TYPE_SUCCESS,
                        title: "<i class='fa fa-share-alt fa-md' style='color: #fff'></i> " +
                            "Add <b>" + config.mediaPathTypeDesc + "</b> from Shared Library",
                        message: $("<div></div>").append(result.Html),
                        onshown: function () {
                            $("#txtSharedSearchFilename").focus();
                        },
                        closable: false,
                        buttons: [{
                            label: "Cancel",
                            action: function (dialog) {
                                dialog.close();
                                reject();
                            }
                        }, {
                            label: "Add",
                            id: "btnAdd",
                            hotkey: 13, // enter
                            cssClass: "btn-add",
                            action: function (dialog) {
                                var streamIds = [];
                                $("input:checkbox:checked", $("#tblSharedFile")).each(function (idx, value) {
                                    var id = value.id.replace("shared_chk_", "");
                                    streamIds.push(id);
                                });
                                dialog.close();
                                resolve(streamIds);
                            }
                        }]
                    });

                    addSharedDialog.realize();
                    addSharedDialog.getModalContent().css("width", "700px");
                    addSharedDialog.open();
                });
            });
        }
    }
})(jQuery);