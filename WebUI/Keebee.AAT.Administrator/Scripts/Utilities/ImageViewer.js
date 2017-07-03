/*!
 * 1.0 Keebee AAT Copyright © 2017
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

            var imageViewer = new BootstrapDialog({
                id: "modal-viewer",
                type: BootstrapDialog.TYPE_INFO,
                title: config.filename + "." + config.fileType.toLowerCase(),
                closable: true
            });

            return new Promise(function(resolve) {
                $.get(site.url +
                        config.controller + "/GetImageViewerView?streamId=" + config.streamId +
                        "&fileType=" + config.fileType)
                    .done(function(message) {

                        imageViewer.realize();
                        var header = imageViewer.getModalHeader();
                        header.css({ backgroundColor: "#000", padding: "10px", border: "none" });
                        header.find(".close").css({ color: "#f0f0f0", opacity: "1" });

                        var body = imageViewer.getModalBody();
                        body.css({ "padding": "0" });
                        body.append("<div></div>").append(message);

                        var footer = imageViewer.getModalFooter();
                        footer.css({ padding: "5px", backgroundColor: "#000", border: "none", display: "block", "text-align": "center" });
                        footer.find(".bootstrap-dialog-footer")
                            .css({ display: "inline" })
                            .append("<span>Click image to close</span>");

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