/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Site/Site.js
 * Author: John Charlton
 * Date: 2015-08
 */

var site = (function () {
    var keebeeUrl = "/Keebee.AAT.Administrator/";

    var self = {
        getUrl: function () {
            var baseUrl = location.href;
            var rootUrl = baseUrl.substring(0, baseUrl.indexOf("/", 7));

            if (rootUrl.indexOf("localhost") > 0) {
                return rootUrl + keebeeUrl;
            } else {
                return rootUrl + keebeeUrl;
            }
        }
    };
    return {
        url: self.getUrl()
    }
})();