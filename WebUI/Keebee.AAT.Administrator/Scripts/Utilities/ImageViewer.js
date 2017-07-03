/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Utilities/ImageViewer.js
 * Author: John Charlton
 * Date: 2017-06
 */

; (function ($) {

    utilities.image = {
        show: function (options) {
            var config = {
                controller: null,
                streamId: null,
                filename: null,
                fileType: null
            };

            if ((typeof options !== "undefined") && (options !== null)) {
                if (options.streamId === null) reject("StreamId cannot be null");
                if (options.filename === null) reject("Filename cannot be null");
                if (options.fileType === null) reject("File Type cannot be null");
                if (options.controller === null) reject("Controller name cannot be null");
            }

            $.extend(config, options);

            return new Promise(function(resolve) {
                $.get(site.url +
                        config.controller +
                        "/GetImageViewerView?streamId=" +
                        config.streamId +
                        "&fileType=" +
                        config.fileType)
                    .done(function(message) {
                        var imageViewer = new BootstrapDialog({
                            type: BootstrapDialog.TYPE_INFO,
                            title: config.filename + "." + config.fileType.toLowerCase(),
                            message: $("<div></div>").append(message),
                            closable: false,
                            buttons: [
                                {
                                    label: "Close",
                                    cssClass: "btn-sm",
                                    action: function(dialog) {
                                        dialog.close();
                                        resolve();
                                    }
                                }
                            ]
                        });

                        imageViewer.realize();
                        var header = imageViewer.getModalHeader();
                        header.css({ "background-color": "#000" });
                        header.css({ "padding": "10px" });
                        header.css({ "border": "none" });

                        var body = imageViewer.getModalBody();
                        body.css({ "padding": "0" });

                        var footer = imageViewer.getModalFooter();
                        footer.css({ "padding": "5px"});
                        footer.css({ "background-color": "#000" });
                        footer.css({ "border": "none" });

                        imageViewer.open();
                    })
                    .error(function(message) {
                        BootstrapDialog.show({
                            type: BootstrapDialog.TYPE_DANGER,
                            title: "Error",
                            message: $("<div></div>").append(message),
                            closable: false,
                            buttons: [
                                {
                                    label: "Close",
                                    action: function(dialog) {
                                        dialog.close();
                                        resolve();
                                    }
                                }
                            ]
                        });
                    });
            });
        }
    }
})(jQuery);;