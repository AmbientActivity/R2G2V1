/*!
 * 1.0 Keebee AAT Copyright © 2015
 * Utilities/Serting.js
 * Author: John Charlton
 * Date: 2017-06
 */

/* specify the sortKey and a primarySortKey (used as a secondary sort if different from the sortKey) */

; (function ($) {
    utilities.sorting = {
        sortArray: function (options) {
            var config = {
                fileArray: [],
                columns: [],
                sortKey: "",
                primaryKey: "",
                descending: true
            };

            $.extend(config, options);
             
            var sortPrimary = function (a, b, descending) {
                if (!descending) {
                    return a[config.primaryKey] === b[config.primaryKey]
                        ? 0
                        : (a[config.primaryKey] < b[config.primaryKey] ? -1 : 1);
                } else {
                    return a[config.primaryKey] === b[config.primaryKey]
                        ? 0
                        : (a[config.primaryKey] > b[config.primaryKey] ? -1 : 1);
                }
            }

            $(config.columns).each(function (index, value) {
                if (value.sortKey === config.sortKey) {
                    config.fileArray.sort(function (a, b) {
                        var isboolean = false;
                        if (typeof value.boolean !== "undefined") {
                            isboolean = value.boolean;
                        }
                        if (!isboolean) {
                            return sortTextOrNumber(a, b);
                        } else {
                            return sortBoolean(a, b);
                        } 
                    });
                }
            });

            function sortBoolean(a, b) {
                var boola = a[config.sortKey];
                var boolb = b[config.sortKey];

                if (config.descending) {
                    if (boola === boolb) {
                        return sortPrimary(a, b, false);
                    } else {
                        if (boola === true) {
                            return -1;
                        } else if (boolb === true) {
                            return 1;
                        } else {
                            return sortPrimary(a, b, false);
                        }
                    }
                } else {
                    if (boola === b[config.sortKey]) {
                        return sortPrimary(a, b, false);
                    } else {
                        if (boola === false) {
                            return -1;
                        } else if (boolb === false) {
                            return 1;
                        } else {
                            return sortPrimary(a, b, false);
                        }
                    }
                }
            }

            function sortTextOrNumber(a, b) {
                var valuea;
                var valueb;

                if (isNaN(a[config.sortKey])) {
                    valuea = a[config.sortKey].toString().toLowerCase();
                    valueb = b[config.sortKey].toString().toLowerCase();
                } else {
                    valuea = a[config.sortKey];
                    valueb = b[config.sortKey];
                }

                if (config.descending) {
                    if (valuea > valueb) {
                        return -1;
                    } else if (valuea < valueb) {
                        return 1;
                    } else {
                        return sortPrimary(a, b, false);
                    }
                } else {
                    if (valuea < valueb) {
                        return -1;
                    } else if (valuea > valueb) {
                        return 1;
                    } else {
                        return sortPrimary(a, b, false);
                    }
                }
            }

            return config.fileArray;
        }
    }
})(jQuery);;