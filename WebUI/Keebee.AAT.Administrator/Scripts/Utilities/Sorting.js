/*!
 * 1.0 Keebee AAT Copyright © 2017
 * Utilities/Serting.js
 * Author: John Charlton
 * Date: 2017-06
 */

/* specify the sortKey and a primarySortKey (used as a secondary sort if different from the sortKey) */

; (function ($) {
    utilities.sorting = {
        sortArray: function (options) {
            var config = {
                array: [],
                columns: [],
                sortKey: "",
                primarySortKey: "",
                descending: true,
                observable: false
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

            var sortPrimaryObservable = function (a, b, descending) {
                var valuea;
                var valueb;

                if (isNaN(a[config.primarySortKey]())) {
                    valuea = a[config.primarySortKey]().toString().toLowerCase();
                    valueb = b[config.primarySortKey]().toString().toLowerCase();
                } else {
                    valuea = a[config.primarySortKey]();
                    valueb = b[config.primarySortKey]();
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
                    config.array.sort(function (a, b) {
                        if (!value.boolean) {
                            if (!config.observable) return sortTextOrNumber(a, b);
                            else return sortTextOrNumberObservable(a, b);
                        } else {
                            if (!config.observable) return sortBoolean(a, b);
                            else return sortBooleanObservable(a, b);
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
                var valuea = null;
                var valueb = null;

                if (isNaN(a[config.sortKey])) {
                    if (a[config.sortKey] !== null)
                        valuea = a[config.sortKey].toString().toLowerCase();

                    if (b[config.sortKey] !== null)
                        valueb = b[config.sortKey].toString().toLowerCase();
                } else {
                    valuea = a[config.sortKey];
                    valueb = b[config.sortKey];
                }

                if (config.descending) {
                    if (valuea > valueb) {
                        return -1;
                    } 
                    else if (valuea < valueb) {
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

            function sortBooleanObservable(a, b) {
                var boola = a[config.sortKey]();
                var boolb = b[config.sortKey]();

                if (config.descending) {
                    if (boola === boolb) {
                        return sortPrimaryObservable(a, b, false);
                    } else {
                        if (boola === true) {
                            return -1;
                        } else if (boolb === true) {
                            return 1;
                        } else {
                            return sortPrimaryObservable(a, b, false);
                        }
                    }
                } else {
                    if (boola === boolb) {
                        return sortPrimaryObservable(a, b, false);
                    } else {
                        if (boola === false) {
                            return -1;
                        } else if (boolb === false) {
                            return 1;
                        } else {
                            return sortPrimaryObservable(a, b, false);
                        }
                    }
                }
            }

            function sortTextOrNumberObservable(a, b) {
                var valuea = null;
                var valueb = null;

                if (isNaN(a[config.sortKey]())) {
                    if (a[config.sortKey]() !== null)
                        valuea = a[config.sortKey]().toString().toLowerCase();

                    if (b[config.sortKey]() !== null)
                        valueb = b[config.sortKey]().toString().toLowerCase();
                } else {
                    valuea = a[config.sortKey]();
                    valueb = b[config.sortKey]();
                }

                if (config.descending) {
                    if (valuea > valueb) {
                        return -1;
                    }
                    else if (valuea < valueb) {
                        return 1;
                    } else {
                        return sortPrimaryObservable(a, b, false);
                    }
                } else {
                    if (valuea < valueb) {
                        return -1;
                    } else if (valuea > valueb) {
                        return 1;
                    } else {
                        return sortPrimaryObservable(a, b, false);
                    }
                }
            }

            return config.array;
        }
    }
})(jQuery);