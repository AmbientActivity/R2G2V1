/*!
 * 1.0 Keebee AAT Copyright © 2015
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

                $.get(site.url + "SharedLibraryAdd/GetView", {
                    profileId: config.profileId,
                    mediaPathTypeId: config.mediaPathTypeId,
                    mediaPathTypeDesc: config.mediaPathTypeDesc,
                    mediaPathTypeCategory: config.mediaPathTypeCategory
                })
                .done(function (message) {
                    var addSharedDialog = new BootstrapDialog({
                        type: BootstrapDialog.TYPE_PRIMARY,
                        title: "<i class='fa fa-share-alt fa-md' style='color: #fff'></i> " +
                            "Add <b>" + config.mediaPathTypeDesc + "</b> from Shared Library",
                        message: $("<div></div>").append(message),
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
                            cssClass: "btn-edit",
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
                })
                .error(function (error) {
                    utilities.alert.show({
                        title: "Partial View Load Error",
                        type: BootstrapDialog.TYPE_DANGER,
                        message: "The following error occured:\n" + error.statusText
                    });
                });
            });
        }
    }
})(jQuery);