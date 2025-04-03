$(function () {
    "use strict";

    $(document).ajaxStop($.unblockUI); 

    $.extend($.fn, {
        ValidateForm: function (options) {
            /* TODO: for additional options */
            var defaults = {},
                options = $.extend(defaults, options),
                result = true;

            $(this).validator('validate');
            $(this).find('.form-group:visible').each(function () {
                if ($(this).hasClass('has-error')) {
                    result = false;
                }
            });
            return result;
        },

        GetUrlParams: function (name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, '\\$&');
            var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, '+'));
        }
    });

    $.extend($, {
        displayMessage: function (options) {
            Notify(options.message, options.callback, options.close_callback, options.style);
        },
        displayResponseMessage: function (serviceResult, options) {
            var result = serviceResult.d || serviceResult,
                isSuccessful = result.isSuccessful || false,
                style = (isSuccessful ? (result.type || "success") : "danger").toLowerCase(),
                showGrowl = true,
                message = result.message || (isSuccessful ? "Successfully processed request." : "Oops. There was an error in processing your request please try again.");

            options = $.extend({
                style: style,
                message: message,
                callback: null,
                close_callback: null
            }, options || {});

            if (options.noGrowlOnSuccess === true && style === "success") {
                showGrowl = false;
            }

            if (showGrowl) {
                $.displayMessage(options);
            }

            if (style == "error" && options.throwError) {
                throw new Error(options.message);
            }
        }
    });
});
