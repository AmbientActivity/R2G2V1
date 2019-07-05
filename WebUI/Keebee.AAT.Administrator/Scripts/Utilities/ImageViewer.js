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

            return new Promise(function () {
                if ((typeof options !== "undefined") && (options !== null)) {
                    if (options.streamId === null) reject("StreamId cannot be null");
                    if (options.filename === null) reject("Filename cannot be null");
                    if (options.fileType === null) reject("File Type cannot be null");
                    if (options.controller === null) reject("Controller name cannot be null");
                }

                $.extend(config, options);

                var bootstrapDialog = new BootstrapDialog({
                    id: "modal-viewer",
                    type: BootstrapDialog.TYPE_INFO,
                    title: config.filename + "." + config.fileType.toLowerCase(),
                    closable: true
                });

                utilities.job.execute({
                    url: config.controller + "/GetImageViewerView?streamId=" + config.streamId +
                        "&fileType=" + config.fileType
                })
                .then(function (result) {
                    bootstrapDialog.realize();
                    var header = bootstrapDialog.getModalHeader();
                    header.css({ backgroundColor: "#000", padding: "10px", border: "none" });
                    header.find(".close").css({ color: "#f0f0f0", opacity: "1" });

                    var body = bootstrapDialog.getModalBody();
                    body.css({ "padding": "0" });
                    body.append("<div></div>").append(result.Html);

                    var footer = bootstrapDialog.getModalFooter();
                    footer.css({
                        padding: "5px",
                        backgroundColor: "#000",
                        color: "#f0f0f0",
                        border: "none",
                        display: "block",
                        "text-align": "center"
                    });
                    footer.find(".bootstrap-dialog-footer")
                        .css({ display: "inline" })
                        .append("<span>Click image to close</span>");

                    bootstrapDialog.open();

                })
                .catch(function(error) {
                    utilities.alert.show({
                        title: "Error",
                        type: BootstrapDialog.TYPE_DANGER,
                        message: "The following unexpected error occured:\n" + error
                    });
                });
            });
        }
    }
})(jQuery);