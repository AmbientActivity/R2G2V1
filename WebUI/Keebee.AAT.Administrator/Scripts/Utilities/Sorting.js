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
                primarySortKey: "",
                descending: true
            };

            $.extend(config, options);
             
            var sortPrimary = function (a, b, descending) {
                var valuea;
                var valueb;

                if (isNaN(a[config.primarySortKey])) {
                    valuea = a[config.primarySortKey].toString().toLowerCase();
                    valueb = b[config.primarySortKey].toString().toLowerCase();
                } else {
                    valuea = a[config.primarySortKey];
                    valueb = b[config.primarySortKey];
                }

                if (!descending) {
                    return valuea === valueb
                        ? 0
                        : (valuea < valueb ? -1 : 1);
                } else {
                    return valuea === valueb
                        ? 0
                        : (valuea > valueb ? -1 : 1);
                }
            }

            $(config.columns).each(function (index, value) {
                if (value.sortKey === config.sortKey) {
                    config.fileArray.sort(function (a, b) {
                        if (!value.boolean) {
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
                    if (boola === boolb) {
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