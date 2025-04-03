$(document).ready(function () {
    "use strict";
    
    $.blockUI();
    var sqldbDateFormat = 'YYYY-MM-DD';

    $('#daterange-btn').daterangepicker(
        {
            startDate: moment(),
            endDate: moment(),
            autoApply: true,
            opens: 'right'
        },
        function (start, end) {
            if (start.format(dateformatShort) === end.format(dateformatShort)) {
                if (start.format(dateformatShort) === moment().format(dateformatShort)) {
                    $('#daterange-btn span').html('<b>Date</b>: Today');
                } else {
                    $('#daterange-btn span').html('<b>Date</b>: ' + start.format(dateformatLong));
                }
            } else {
                $('#daterange-btn span').html('<b>From</b>: ' + start.format(dateformatLong) + ' <b>To</b>: ' + end.format(dateformatLong));
            }

            loadLogsData(start, end);
        }
    );

    function initHandlers() {
        $("#syslogGrid").off().on('click', 'a', function () {
            var $a = $(this);

            if ($a.is(".viewBody")) {
                showDetailsModal('body', function () {
                    var jsonData = JSON.parse($a.attr('data-reqbody'));

                    $('#detail').jsonPresenter('destroy')
                        .jsonPresenter({
                            json: jsonData
                        });

                    $(".modal.md").modal();
                });
            }
            else if ($a.is(".viewParams")) {
                showDetailsModal('params', function () {
                    var urlParamsData = $a.attr('data-urlparams'),
                        data = '';

                    urlParamsData = urlParamsData.replace('?', '');

                    $('#detail').html('');

                    $.each(urlParamsData.split('&'), function (index, value) {
                        data += `• ${value}<br/>`;
                    });

                    var pre = $('<pre></pre>').append(data);

                    $('#detail').append(pre);

                    $(".modal.md").modal();
                });
            }
        });
    }

    function showDetailsModal(type, callback) {

        callback = typeof callback === 'function' ? callback : function () { };

        $.get("../content/Html/SystemLogsModals.html", function (html) {
            var logDetail = $(html).find('#logDetail'), result = {};

            $('.modal-content.md').find('div').remove();
            $('.modal-content.md').append(logDetail);

            var title = type == 'body' ? 'Request Body' : 'Url Paramaters';
            $('.modal-title').text(title);

            callback();
        });
    }

    function initFilters() {
        $('#selData').select2({
            placeholder: 'ALL',
            ajax: {
                type: "GET",
                url: `../../syslogs/entities`,
                data: function (params) {
                    return {
                        q: params.term, // search term
                        page: params.page
                    };
                },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                processResults: function (data, index) {
                    return {
                        results: $.map(data.data, function (val) {
                            return { id: val.StringId, text: val.Text };
                        })
                    };
                }
            }
        });

        $('#selUser').select2({
            placeholder: 'ALL',
            ajax: {
                type: "GET",
                url: `../../user/assigned?p=1`,
                data: function (params) {
                    return {
                        q: params.term, // search term
                        page: params.page
                    };
                },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                processResults: function (data, index) {
                    return {
                        results: $.map(data.data, function (val) {
                            return { id: val.Id, text: val.Text };
                        })
                    };
                }
            }
        });

        $('#selUser').change(loadLogsData);
        $('#selData').change(loadLogsData);
    }

    function IsJsonString(str) {
        try {
            JSON.parse(str);
        } catch (e) {
            return false;
        }
        return true;
    }

    function loadLogsData(startDate, endDate) {

        if (!startDate || !endDate) {
            startDate = $('#daterange-btn').data('daterangepicker').startDate;
            endDate = $('#daterange-btn').data('daterangepicker').endDate;
        }

        var _grid = $("#syslogGrid"),
            result = {},
            gridOptions = {
                width: "100%",
                height: "auto",
                pageSize: 30,
                sorting: true,
                paging: true,
                fields: [
                      { name: "ActorName", type: "text", width: "7%", title: "User" }
                    , { name: "ActorRole", type: "text", width: "7%", title: "Role" }
                    , { name: "ActionType", type: "text", width: "7%", title: "Action" }
                    , { name: "Entity", type: "text", width: "7%", title: "Data" }
                    , { name: "ElapsedTime", type: "text", width: "7%", title: "Process Time" }
                    , {
                        name: "LogDate", type: "text", width: "7%", title: "Log Date",
                        cellRenderer: function (value, item) {
                            var d = new Date(value);

                            return $(`<td>${d.toLocaleString()}</td>`);
                        }
                    }
                    , {
                        name: "", type: "text", width: "5%", title: "Request Details",
                        cellRenderer: function (value, item) {
                            var buttons = '';

                            if (item.RequestBody && IsJsonString(item.RequestBody)) {
                                buttons += `<a href='#' class='viewBody' data-reqbody='${item.RequestBody}' title="Request Body"><i class="grid-icons glyphicon glyphicon-info-sign"></i></a> `
                            }

                            if (item.UrlParams) {
                                buttons += `<a href='#' class='viewParams' data-urlparams='${item.UrlParams}' title="Url Parameters"><i class="grid-icons glyphicon glyphicon-stats"></i></a>`
                            }

                            return $(`<td style='padding-left: 20px;'>${buttons}</td>`);
                        }
                    }
                ]
            },

            /* Set start date value */
            startDate = startDate.format(sqldbDateFormat),
            /* Set end date value */
            endDate = endDate.format(sqldbDateFormat) + ' 23:59:59';

        var userIdArray = $('#selUser').select2('data'),
            entitiesArray = $('#selData').select2('data');

        var userIds = $.map(userIdArray, function (val, index) {
            return `userIds[${index}]=${val.id}`;
        }),
            entities = $.map(entitiesArray, function (val, index) {
                return `entities[${index}]=${val.id}`;
            });

        var urlParams = `startDate=${startDate}&endDate=${endDate}`;

        urlParams = urlParams + (userIds.length > 0 ? `&${userIds.join('&')}` : '');
        urlParams = urlParams + (entities.length > 0 ? `&${entities.join('&')}` : '');

        $.ajax({
            type: 'GET',
            url: `../syslogs/data?${urlParams}`,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                result = response
            },
            complete: function () {
                if (!result.isSuccessful) {
                    if (result.message) Notify(result.message, null, null, 'danger');
                    return false;
                }

                $.extend(gridOptions, {
                    data: result.data
                });

                $(_grid).jsGrid(gridOptions);
            }
        });
    }

    initFilters();
    loadLogsData();
    initHandlers();
});