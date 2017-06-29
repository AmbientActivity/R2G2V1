/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Utilities/Serting.js
 * Author: John Charlton
 * Date: 2017-06
 */

/* column array must contain a field named 'filename' */

; (function ($) {
    utilities.sorting = {
        sortFies: function (options) {
            var config = {
                fileArray: [],
                columns: [],
                sortKey: "filename",
                descending: true
            };

            $.extend(config, options);
              
            $(config.columns).each(function (index, value) {
                if (value.sortKey === config.sortKey) {
                    config.fileArray.sort(function (a, b) {
                        if (value.numeric) {
                            return sortNumeric(a, b);
                        } else if (value.boolean) {
                            return sortBoolean(a, b);
                        } else {
                            return sortFilenames(a, b);
                        }
                    });
                }

            });

            function sortBoolean(a, b) {
                var sortByVal = function () {
                    return a.filename === b.filename ? 0 : (a.filename < b.filename ? -1 : 1);
                }

                if (config.descending) {
                    if (a[config.sortKey] === b[config.sortKey]) {
                        return sortByVal();
                    } else {
                        if (a[config.sortKey] === true) {
                            return -1;
                        } else if (b[config.sortKey] === true) {
                            return 1;
                        } else {
                            return sortByVal();
                        }
                    }
                } else {
                    if (a[config.sortKey] === b[config.sortKey]) {
                        return sortByVal();
                    } else {
                        if (a[config.sortKey] === false) {
                            return -1;
                        } else if (b[config.sortKey] === false) {
                            return 1;
                        } else {
                            return sortByVal();
                        }
                    }
                }
            }

            function sortNumeric(a, b) {
                if (config.descending) {
                    return a[config.sortKey] > b[config.sortKey]
                        ? -1
                        : a[config.sortKey] < b[config.sortKey] || a.filename > b.filename ? 1 : 0;
                } else {
                    return a[config.sortKey] < b[config.sortKey]
                        ? -1
                        : a[config.sortKey] > b[config.sortKey] || a.filename > b.filename ? 1 : 0;
                }
            }

            function sortFilenames(a, b) {
                if (config.descending) {
                    return a[config.sortKey].toString().toLowerCase() >
                        b[config.sortKey].toString().toLowerCase()
                        ? -1
                        : a[config.sortKey].toString().toLowerCase() <
                        b[config.sortKey].toString().toLowerCase() ||
                        a.filename.toLowerCase() > b.filename.toLowerCase()
                        ? 1
                        : 0;
                } else {
                    return a[config.sortKey].toString().toLowerCase() <
                        b[config.sortKey].toString().toLowerCase()
                        ? -1
                        : a[config.sortKey].toString().toLowerCase() >
                        b[config.sortKey].toString().toLowerCase() ||
                        a.filename.toLowerCase() > b.filename.toLowerCase()
                        ? 1
                        : 0;
                };
            }

            return config.fileArray;
        }
    }
})(jQuery);;