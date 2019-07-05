/*!
 * 1.0 Keebee AAT Copyright © 2017
 * Utilities/VideoViewer.js
 * Author: John Charlton
 * Date: 2017-06
 */

; (function ($) {

    utilities.video = {
        show: function (options) {
            var config = {
                src: null,
                player: null,
                filename: null,
                fileType: null
            };

            return new Promise(function (resolve) {

                if ((typeof options !== "undefined") && (options !== null)) {
                    if (options.src === null) reject("Source cannot be null");
                    if (options.player === null) reject("Player cannot be null");
                    if (options.filename === null) reject("Filename cannot be null");
                    if (options.fileType === null) reject("File Type cannot be null");
                }

                $.extend(config, options);

                var bootstrapDialog = new BootstrapDialog({
                    id: "modal-video",
                    type: BootstrapDialog.TYPE_INFO,
                    title: config.filename,
                    closable: true,
                    onshown: function () {
                        config.player.attr("type", "video/" + config.fileType);
                        config.player.prop("controls", true);
                        config.player.prop("controlsList", "nodownload");
                        config.player.prop("autoplay", true);
                        config.player.attr("src", config.src);
                        resolve();
                    }
                });

                bootstrapDialog.realize();
                bootstrapDialog.getModalDialog().css("width", "600px");

                var header = bootstrapDialog.getModalHeader();
                header.css({ backgroundColor: "#000", padding: "10px", border: "none" });
                header.find(".close").css({ color: "#f0f0f0", opacity: "1" });

                var body = bootstrapDialog.getModalBody();
                body.css({ "padding": "0" });
                body.append("<div class='container-video-preview'></div>").append(config.player);

                var content = bootstrapDialog.getModalContent();
                content.css({ backgroundColor: "#000" });

                bootstrapDialog.getModalFooter().hide();

                bootstrapDialog.open();
            });
        }
    }
})(jQuery);