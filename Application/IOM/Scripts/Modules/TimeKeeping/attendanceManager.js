$(document).ready(function () {
    "use strict";

    var sqldbDateFormat = 'YYYY-MM-DD',
        headerDateFormat = 'MM/DD/YYYY',
        timekeepingModulePermission = {};


    $(document).ready(function () {
        
        $('#daterange-btn').daterangepicker(
            singleDatePicker,
            function (start, end) {
                
                var weekStart = start.weekday(0),
                    weekend = end.weekday(6);

                $('#daterange-btn span').html('<b>From</b>: ' + start.format(dateformatLong) + ' <b>To</b>: ' + end.format(dateformatLong));

                loadGridData(weekStart, weekend);
            }
        );

        $('#daterange-btn span').html('<b>From</b>: ' + momentEST.weekday(0).format(dateformatLong) + ' <b>To</b>: ' + momentEST.weekday(6).format(dateformatLong));

        $("#refresh-btn").off().on('click', function () {
            loadGridData();
        });
    });

    function initFilters() {
        $('#selUsers').select2({
            placeholder: 'ALL',
            ajax: {
                type: "GET",
                /* ?=p1 : To set a default values for array of ids */
                url: `../../user/assigned?p=1`,
                data: function (params) {
                    var accountIds = $.map($('#selAccount').select2('data'), function (val, index) { return val.id; }),
                        teamIds = $.map($('#selTeam').select2('data'), function (val, index) { return val.id; }),
                        roles = ['AG', 'LA'];

                    return {
                        q: params.term, // search term
                        page: params.page,
                        accountIds: accountIds,
                        teamIds: teamIds,
                        roles: roles
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

        $('#selAccount').select2({
            placeholder: 'ALL',
            ajax: {
                type: "GET",
                /* ?=p1 : To set a default values for array of ids */
                url: `../../accounts/assigned?p=1`,
                data: function (params) {
                    var userIds = $.map($('#selUsers').select2('data'), function (val, index) { return val.id; }),
                        teamIds = $.map($('#selTeam').select2('data'), function (val, index) { return val.id; });

                    return {
                        q: params.term, // search term
                        page: params.page,
                        userIds: userIds,
                        teamIds: teamIds
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

        $('#selTeam').select2({
            placeholder: 'ALL',
            ajax: {
                type: "GET",
                /* ?=p1 : To set a default values for array of ids */
                url: `../../teams/assigned?p=1`,
                data: function (params) {
                    var userIds = $.map($('#selUsers').select2('data'), function (val, index) { return val.id; }),
                        accountIds = $.map($('#selAccount').select2('data'), function (val, index) { return val.id; });

                    return {
                        q: params.term, // search term
                        page: params.page,
                        userIds: userIds,
                        accountIds: accountIds
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

        $('#selUsers').change(loadGridData);
        $('#selAccount').change(loadGridData);
        $('#selTeam').change(loadGridData);
    }

    function initHandlers() {
        $('#attendanceGrid').off().on('click', 'a', function () {
            var $a = $(this);

            if ($a.is(".accountlink")) {
                var accountId = $a.attr('data-accid');

                window.location = `../../Account/Manager?AccountId=${accountId}`;
            }
            else if ($a.is(".teamlink")) {
                var teamId = $a.attr('data-teamid');

                window.location = `../../Team/Manager?teamId=${teamId}`;
            }
            else if ($a.is(".namelink")) {
                var userid = $a.attr('data-netuserid');

                window.location = `../Profile/Index/${userid}`;
            }
            else if ($a.is('.editHours')) {
                var userid = $a.data('userid'),
                    startDate = $a.data('startdate'),
                    endDate = $a.data('enddate');

                $.get("../content/Html/TimeKeepingModals.html", function (html) {
                    var editHoursDlg = $(html).find('#editHoursDlg'), result = {};

                    $('.modal-content.md').find('div').remove();
                    $('.modal-content.md').append(editHoursDlg);

                    $.ajax({
                        type: "GET",
                        url: `../../tkdata/week_attendance?userId=${userid}&startdate=${startDate}&enddate=${endDate}`,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            result = response;
                        },
                        complete: function () {
                            if (!result.isSuccessful) {
                                if (result.message) Notify(result.message, null, null, 'danger');
                                return false;
                            }

                            $('.dayAttendance').select2({
                                data: [
                                    {
                                        id: '0',
                                        text: '0.00'
                                    },
                                    {
                                        id: '0.25',
                                        text: '0.25'
                                    },
                                    {
                                        id: '0.5',
                                        text: '0.50'
                                    },
                                    {
                                        id: '0.75',
                                        text: '0.75'
                                    },
                                    {
                                        id: '1',
                                        text: '1.00'
                                    }
                                ]
                            });

                            $('#name').text(result.data.FullName);

                            var momSDate = moment(startDate, sqldbDateFormat),
                                momEDate = moment(endDate, sqldbDateFormat);

                            var trDate = $('<tr class=""></tr>');

                            for (var i = 0; i < 7; i++) {
                                var formattedDate = momSDate.format(sqldbDateFormat);
                                var att = result.data.AttendanceData.find(a => a.SDate.includes(formattedDate)),
                                    attValue = att ? parseFloat(att.WorkedDay) : 0.0,
                                    fontColor = attValue < 1 && attValue > 0 ? 'orange' : (attValue == 0 ? 'red' : 'normal');
                                
                                trDate.append($(`<td>${momSDate.format(headerDateFormat)}</td>`));

                                $(`.day${i}`).val(attValue);

                                $(`.day${i}`).attr('data-attid', att ? att.Id : '');
                                $(`.day${i}`).attr('data-attdate', formattedDate);

                                momSDate.add(1, 'd');
                            }

                            $('.dayAttendance').trigger('change');

                            $('#attFixHead').append(trDate);                            
                                                        
                            $(".modal.md").modal();
                        }
                    });

                    $('.modal.md').off('shown.bs.modal').on('shown.bs.modal', function () {
                        result = {};

                        $('#saveBtn').off().on('click', function () {                            

                            var data = [];

                            $.each($('.dayAttendance'), function (index, value) {

                                data.push({
                                    Id: $(value).data('attid'),
                                    SDate: $(value).data('attdate'),
                                    WorkedDay: $(value).val(),
                                    UserDetailsId: userid
                                });
                            });

                            $('.modal.md').block();
                            $.ajax({
                                type: "POST",
                                url: `../tkdata/save_attendance`,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                data: JSON.stringify(data),
                                success: function (response) {
                                    result = response;
                                },
                                complete: function () {
                                    $('.modal.md').unblock();

                                    $.displayResponseMessage(result);
                                    if (!result.isSuccessful) {
                                        return false;
                                    }

                                    $(".modal.md").modal('hide');
                                    loadGridData();
                                }
                            });
                        });
                    });
                });

                return false;
            }
        });
    }

    function loadGridData(startDate, endDate) {

        if (!startDate || !endDate) {
            startDate = $('#daterange-btn').data('daterangepicker').startDate.weekday(0);
            endDate = $('#daterange-btn').data('daterangepicker').endDate.weekday(6);
        }
        
        /* Set start date value */
        var sStartDate = startDate.format(sqldbDateFormat),
            /* Set end date value */
            sEndDate = endDate.format(sqldbDateFormat);

        var baseDate = startDate.clone();

        var _grid = $("#attendanceGrid"),
            result = {},
            gridOptions = {
                width: "100%",
                height: "auto",
                pageSize: 20,
                sorting: true,
                paging: true,
                fields: [
                    {
                        name: "FullName", type: "text", width: "10%", title: "Name",
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');

                            if (value)
                                td.append(`<a href="#" class='namelink' data-netuserid='${item.NetUserId}'>${value}</a>`);

                            return td;
                        }
                    },
                    {
                        name: "AccountName", type: "text", width: "10%", title: "Account",
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');
                            var aData = '';

                            $.each(item.Accounts, function (index, value) {
                                aData += `• <a href="#" class='accountlink' data-accid='${value.Id}'>${value.Name}</a> `;
                            });

                            if (value != '')
                                td.append(`${aData} `);

                            return td;
                        }
                    },
                    {
                        name: "TeamName", type: "text", width: "10%", title: "Team",
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');
                            var aData = '';

                            $.each(item.Teams, function (index, value) {
                                aData += `• <a href="#" class='teamlink' data-teamid='${value.Id}'>${value.Name}</a> `;
                            });

                            if (value != '')
                                td.append(`${aData} `);

                            return td;
                        }
                    },
                    {
                        name: "Sunday", type: "number", width: "7%", title: "Sunday", align: 'center',
                        headerTemplate: function () {
                            var thSpan = $('<span>Sunday</span>');
                            thSpan.append(`<hr style='margin:0'>${baseDate.weekday(0).format(headerDateFormat)}`);
                            return thSpan;
                        },
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>'),
                                historyDate = baseDate.weekday(0).format(sqldbDateFormat),
                                data = item.Attendance.find(a => a.SDate.includes(historyDate)),
                                fontColor = 'normal';

                            if (data) {
                                if (parseFloat(data.WorkedDay) < 1 && parseFloat(data.WorkedDay) > 0) {
                                    fontColor = 'orange';
                                }
                                else if (parseFloat(data.WorkedDay) == 0.0 && !item.DayOffs.find(d => d.NumericDay == 0)) {
                                    fontColor = 'red';
                                }

                                td.append(parseFloat(data.WorkedDay).toFixed(2));
                            }
                            else if (!item.DayOffs.find(d => d.NumericDay == 0)) {
                                fontColor = 'red';
                                td.append('0.00');
                            }
                            else {
                                td.append('0.00');
                            }

                            td.addClass(fontColor);

                            return td;
                        }
                    },
                    {
                        name: "Monday", type: "number", width: "7%", title: "Monday", align: 'center',
                        headerTemplate: function () {
                            var thSpan = $('<span>Monday</span>');
                            thSpan.append(`<hr style='margin:0'>${baseDate.weekday(1).format(headerDateFormat)}`);
                            return thSpan;
                        },
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>'),
                                historyDate = baseDate.weekday(1).format(sqldbDateFormat),
                                data = item.Attendance.find(a => a.SDate.includes(historyDate)),
                                fontColor = 'normal';

                            if (data) {
                                if (parseFloat(data.WorkedDay) < 1 && parseFloat(data.WorkedDay) > 0) {
                                    fontColor = 'orange';
                                }
                                else if (parseFloat(data.WorkedDay) == 0.0 && !item.DayOffs.find(d => d.NumericDay == 1)) {
                                    fontColor = 'red';
                                }

                                td.append(parseFloat(data.WorkedDay).toFixed(2));
                            }
                            else if (!item.DayOffs.find(d => d.NumericDay == 1)) {
                                fontColor = 'red';
                                td.append('0.00');
                            }
                            else {
                                td.append('0.00');
                            }

                            td.addClass(fontColor);

                            return td;
                        }
                    },
                    {
                        name: "Tuesday", type: "number", width: "7%", title: "Tuesday", align: 'center',
                        headerTemplate: function () {
                            var thSpan = $('<span>Tuesday</span>');
                            thSpan.append(`<hr style='margin:0'>${baseDate.weekday(2).format(headerDateFormat)}`);
                            return thSpan;
                        },
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>'),
                                historyDate = baseDate.weekday(2).format(sqldbDateFormat),
                                data = item.Attendance.find(a => a.SDate.includes(historyDate)),
                                fontColor = 'normal';

                            if (data) {
                                if (parseFloat(data.WorkedDay) < 1 && parseFloat(data.WorkedDay) > 0) {
                                    fontColor = 'orange';
                                }
                                else if (parseFloat(data.WorkedDay) == 0.0 && !item.DayOffs.find(d => d.NumericDay == 2)) {
                                    fontColor = 'red';
                                }

                                td.append(parseFloat(data.WorkedDay).toFixed(2));
                            }
                            else if (!item.DayOffs.find(d => d.NumericDay == 2)) {
                                fontColor = 'red';
                                td.append('0.00');
                            }
                            else {
                                td.append('0.00');
                            }

                            td.addClass(fontColor);

                            return td;
                        }
                    },
                    {
                        name: "Wednesday", type: "number", width: "7%", title: "Wednesday", align: 'center',
                        headerTemplate: function () {
                            var thSpan = $('<span>Wednesday</span>');
                            thSpan.append(`<hr style='margin:0'>${baseDate.weekday(3).format(headerDateFormat)}`);
                            return thSpan;
                        },
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>'),
                                historyDate = baseDate.weekday(3).format(sqldbDateFormat),
                                data = item.Attendance.find(a => a.SDate.includes(historyDate)),
                                fontColor = 'normal';

                            if (data) {
                                if (parseFloat(data.WorkedDay) < 1 && parseFloat(data.WorkedDay) > 0) {
                                    fontColor = 'orange';
                                }
                                else if (parseFloat(data.WorkedDay) == 0.0 && !item.DayOffs.find(d => d.NumericDay == 3)) {
                                    fontColor = 'red';
                                }

                                td.append(parseFloat(data.WorkedDay).toFixed(2));
                            }
                            else if (!item.DayOffs.find(d => d.NumericDay == 3)) {
                                fontColor = 'red';
                                td.append('0.00');
                            }
                            else {
                                td.append('0.00');
                            }

                            td.addClass(fontColor);

                            return td;
                        }
                    },
                    {
                        name: "Thursday", type: "number", width: "7%", title: "Thursday", align: 'center',
                        headerTemplate: function () {
                            var thSpan = $('<span>Thursday</span>');
                            thSpan.append(`<hr style='margin:0'>${baseDate.weekday(4).format(headerDateFormat)}`);
                            return thSpan;
                        },
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>'),
                                historyDate = baseDate.weekday(4).format(sqldbDateFormat),
                                data = item.Attendance.find(a => a.SDate.includes(historyDate)),
                                fontColor = 'normal';

                            if (data) {
                                if (parseFloat(data.WorkedDay) < 1 && parseFloat(data.WorkedDay) > 0) {
                                    fontColor = 'orange';
                                }
                                else if (parseFloat(data.WorkedDay) == 0.0 && !item.DayOffs.find(d => d.NumericDay == 4)) {
                                    fontColor = 'red';
                                }

                                td.append(parseFloat(data.WorkedDay).toFixed(2));
                            }
                            else if (!item.DayOffs.find(d => d.NumericDay == 4)) {
                                fontColor = 'red';
                                td.append('0.00');
                            }
                            else {
                                td.append('0.00');
                            }

                            td.addClass(fontColor);
                            td.append(data);

                            return td;
                        }
                    },
                    {
                        name: "Friday", type: "number", width: "7%", title: "Friday", align: 'center',
                        headerTemplate: function () {
                            var thSpan = $('<span>Friday</span>');
                            thSpan.append(`<hr style='margin:0'>${baseDate.weekday(5).format(headerDateFormat)}`);
                            return thSpan;
                        },
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>'),
                                historyDate = baseDate.weekday(5).format(sqldbDateFormat),
                                data = item.Attendance.find(a => a.SDate.includes(historyDate)),
                                fontColor = 'normal';

                            if (data) {
                                if (parseFloat(data.WorkedDay) < 1 && parseFloat(data.WorkedDay) > 0) {
                                    fontColor = 'orange';
                                }
                                else if (parseFloat(data.WorkedDay) == 0.0 && !item.DayOffs.find(d => d.NumericDay == 5)) {
                                    fontColor = 'red';
                                }

                                td.append(parseFloat(data.WorkedDay).toFixed(2));
                            }
                            else if (!item.DayOffs.find(d => d.NumericDay == 5)) {
                                fontColor = 'red';
                                td.append('0.00');
                            }
                            else {
                                td.append('0.00');
                            }

                            td.addClass(fontColor);

                            return td;
                        }
                    },
                    {
                        name: "Saturday", type: "number", width: "7%", title: "Saturday", align: 'center',
                        headerTemplate: function () {
                            var thSpan = $('<span>Saturday</span>');
                            thSpan.append(`<hr style='margin:0'>${baseDate.weekday(6).format(headerDateFormat)}`);
                            return thSpan;
                        },
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>'),
                                historyDate = baseDate.weekday(6).format(sqldbDateFormat),
                                data = item.Attendance.find(a => a.SDate.includes(historyDate)),
                                fontColor = 'normal';

                            if (data) {
                                if (parseFloat(data.WorkedDay) < 1 && parseFloat(data.WorkedDay) > 0) {
                                    fontColor = 'orange';
                                }
                                else if (parseFloat(data.WorkedDay) == 0.0 && !item.DayOffs.find(d => d.NumericDay == 6)) {
                                    fontColor = 'red';
                                }

                                td.append(parseFloat(data.WorkedDay).toFixed(2));
                            }
                            else if (!item.DayOffs.find(d => d.NumericDay == 6)) {
                                fontColor = 'red';
                                td.append('0.00');
                            }
                            else {
                                td.append('0.00');
                            }

                            td.addClass(fontColor);

                            return td;
                        }
                    },
                    {
                        name: "WorkedDays", type: "text", width: "7%", title: "Worked", align: 'center',
                        cellRenderer: function (value, item) {
                            var td = $('<td class="bg_yellow"></td>'),
                                fontColor = 'normal';

                            if (value) {
                                if (parseFloat(value) < 1 && parseFloat(value) > 0) {
                                    fontColor = 'orange';
                                }
                                else if (parseFloat(value) == 0.0) {
                                    fontColor = 'red';
                                }

                                td.append(parseFloat(value).toFixed(2));
                            }
                            else {
                                fontColor = 'red';
                                td.append('0.00');
                            }

                            td.addClass(fontColor);

                            return td;
                        }
                    },
                    {
                        name: "Absence", type: "text", width: "7%", title: "Absence", align: 'center',
                        cellRenderer: function (value, item) {
                            var td = $('<td class="bg_yellow"></td>'),
                                fontColor = 'normal';

                            if (value) {
                                if (parseFloat(value) < 1 && parseFloat(value) > 0) {
                                    fontColor = 'orange';
                                }
                                else if (parseFloat(value) > 1.0) {
                                    fontColor = 'red';
                                }
                                else if (parseFloat(value) < 0) {
                                    value = 0;
                                }

                                td.append(parseFloat(value).toFixed(2));
                            }
                            else {
                                fontColor = 'normal';
                                td.append('0.00');
                            }

                            td.addClass(fontColor);

                            return td;
                        }
                    },
                    {
                        name: "Action", type: "text", width: "5%", title: "Action", align: 'center',
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');
                            var editIcon = '<i class="glyphicon glyphicon-pencil"></i>',
                                data = `<a href='#' class='button editHours' title='Edit Hours' 
                                        data-userid=${item.UserDetailsId}                                        
                                        data-startdate='${sStartDate}'
                                        data-enddate='${sEndDate}'
                                        ">${timekeepingModulePermission.canEdit ? editIcon : ''}</a>`;

                            return $(`<td>${data}</td>`);
                        }
                    }

                ],
                onRefreshing: function (args) {
                    $.each(args.grid.data, function (index, data) {
                        var sun = data.Attendance.find(a => a.SDate.includes(baseDate.weekday(0).format(sqldbDateFormat))),
                            mon = data.Attendance.find(a => a.SDate.includes(baseDate.weekday(1).format(sqldbDateFormat))),
                            tue = data.Attendance.find(a => a.SDate.includes(baseDate.weekday(2).format(sqldbDateFormat))),
                            wed = data.Attendance.find(a => a.SDate.includes(baseDate.weekday(3).format(sqldbDateFormat))),
                            thu = data.Attendance.find(a => a.SDate.includes(baseDate.weekday(4).format(sqldbDateFormat))),
                            fri = data.Attendance.find(a => a.SDate.includes(baseDate.weekday(5).format(sqldbDateFormat))),
                            sat = data.Attendance.find(a => a.SDate.includes(baseDate.weekday(6).format(sqldbDateFormat)));

                        data.Sunday = sun ? sun.WorkedDay : 0.0;
                        data.Monday = mon ? mon.WorkedDay : 0.0;
                        data.Tuesday = tue ? tue.WorkedDay : 0.0;
                        data.Wednesday = wed ? wed.WorkedDay : 0.0;
                        data.Thursday = thu ? thu.WorkedDay : 0.0;
                        data.Friday = fri ? fri.WorkedDay : 0.0;
                        data.Saturday = sat ? sat.WorkedDay : 0.0;
                    });
                }
            };

        /* Set dropdown filter array values */
        var accountIdArray = $('#selAccount').select2('data')
            , teamIdArray = $('#selTeam').select2('data')
            , userIdArray = $('#selUsers').select2('data');

        var accountIds = $.map(accountIdArray, function (val, index) {
            return `accountIds[${index}]=${val.id}`;
        }),
            teamIds = $.map(teamIdArray, function (val, index) {
                return `teamIds[${index}]=${val.id}`;
            }),
            userIds = $.map(userIdArray, function (val, index) {
                return `userIds[${index}]=${val.id}`;
            });


        var urlParams = `startDate=${sStartDate}&endDate=${sEndDate}`;

        urlParams = urlParams + (accountIds.length > 0 ? `&${accountIds.join('&')}` : '');
        urlParams = urlParams + (teamIds.length > 0 ? `&${teamIds.join('&')}` : '');
        urlParams = urlParams + (userIds.length > 0 ? `&${userIds.join('&')}` : '');

        $.blockUI();
        $.ajax({
            type: 'GET',
            url: `../tkdata/attendance?${urlParams}`,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                result = response
            },
            complete: function () {
                if (!result.isSuccessful || !_grid) {
                    if (result.message) Notify(result.message, null, null, 'danger');
                    return false;
                }
                
                $.extend(gridOptions, {
                    data: result.data
                });

                $(_grid).jsGrid("reset");
                $(_grid).jsGrid(gridOptions);
            }
        });
    }

    UpdateUserInfo(function (data) {

        timekeepingModulePermission = data.Permissions.find(function (item) {
            return item.ModuleCode == "m-timekeeps";
        });

        initFilters();
        loadGridData();
        initHandlers();

    });
})