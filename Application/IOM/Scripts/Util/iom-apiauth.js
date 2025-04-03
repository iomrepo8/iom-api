
$(function () {
    "use strict";

    var pathname = window.location.pathname.toLowerCase();

    if (pathname == '/user/login') {
        localStorage.clear();
    }

    $.ajaxSetup({
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", `Bearer ${localStorage.access_token}`);
        }
    });

    $.extend($.fn, {
        acqcred: function (options) {
            /* TODO: for additional options */
            var defaults = {},
                options = $.extend(defaults, options);

            $('form').submit(function () {

                let d = {
                    un: $(this).find('input[type=text]').val(),
                    pw: $(this).find('input[type=password]').val()
                };

                localStorage.uauthdata = JSON.stringify(d);
            });
        },

        authenticate: function (options) {
            let uauthdata = JSON.parse(localStorage.getItem('uauthdata'));

            if (!uauthdata) return;

            localStorage.clear();

            var defaults = {
                un: uauthdata.un,
                pw: uauthdata.pw,
                grant_type: 'password',
                callback: options.callback = typeof options.callback === 'function' ? options.callback : function () { },
            },
                options = $.extend(defaults, options);

            $.ajax({
                type: "POST",
                url: '/token',
                data: {
                    /* IOM-049 | BUG*/
                    //Username: options.un.split('@')[0],
                    Username: options.un,
                    Password: options.pw,
                    grant_type: options.grant_type
                },
                success: function (response) {
                    localStorage.removeItem('access_token');
                    localStorage.access_token = response.access_token;

                    options.callback();
                },
                statusCode: {
                    400: function () {
                        //TODO: Add proper action for invalid acquisition of access token.
                        //document.getElementById('logoutForm').submit();
                    }
                }
            });
        },

        signout: function (options) {
            /* TODO: for additional options */
            var defaults = {},
                options = $.extend(defaults, options);

            $('#logoutForm').submit(function () {
                localStorage.clear();
            });
        },

        heartbeat: function () {
            var autoOutHub = $.connection.autoOutHub;

            $.connection.hub.qs = { 'access_token': localStorage.access_token };

            autoOutHub.client.heartbeat = function (userOnlineCount) {
                //var pContent = `${userOnlineCount == 0 ? 'No' : userOnlineCount} ${userOnlineCount == 1 ? 'User' : 'Users'} Online`

                //if ($('#userCountDetail').length > 0) {
                //    $('#userCountDetail').text(`${pContent}`);
                //}
            };

            $.connection.hub.start();
        },

        updateActiveTime: function () {
            var autoOutHub = $.connection.autoOutHub;
            $.connection.hub.qs = { 'access_token': localStorage.access_token };

            setInterval(function () {
                $.connection.hub.start()
                    .done(function () {
                        autoOutHub.server.updateUserActiveTime();
                    });
            }, 5000);
        }
    });

    $.fn.authenticate({
        callback: function () {
            $.fn.signout();
            $.fn.heartbeat();
        }
    });

    $.fn.heartbeat();

    var isChromium = window.chrome;
    var winNav = window.navigator;
    var vendorName = winNav.vendor;
    var isOpera = typeof window.opr !== "undefined";
    var isIEedge = winNav.userAgent.indexOf("Edge") > -1;
    var isIOSChrome = winNav.userAgent.match("CriOS");

    if (!sessionStorage.isBrowserWarn || !sessionStorage.isBrowserWarn === 'true') {
        if (isIOSChrome) {
            // is Google Chrome on IOS
        } else if (!(
            isChromium !== null &&
            typeof isChromium !== "undefined" &&
            vendorName === "Google Inc." &&
            isOpera === false &&
            isIEedge === false)
        ) {
            sessionStorage.isBrowserWarn = 'true';
            alert("This application works best with Google Chrome. We recommend accessing the system using Google Chrome for best user experience.");
        }
    }    
});