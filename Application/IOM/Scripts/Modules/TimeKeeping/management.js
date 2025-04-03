$(document).ready(function () {
    "use strict";

    var reportDateFormat = 'YYYY-MM-DD',
        _grid = null,
        gridOptions = null,
        isMinuteView = true,
        isActiveOnly = true,
        timekeepingData = [],
        startDate, endDate,
        timekeepingModulePermission = {};

    var sortOptions = [
        { text: 'None', id: '' },
        { text: 'Name', id: 'FullName' },
        { text: 'Role', id: 'RoleName' },
        { text: 'Account', id: 'AccountName' },
        { text: 'Team', id: 'TeamName' },
        { text: 'Status', id: 'Status' },
        { text: 'Active Hours', id: 'TotalActiveTime' }
    ];

    $('#selSort').select2({
        data: sortOptions
    });

    $('#daterange-btn').daterangepicker(daterangepickerOptionsL,
        function (start, end) {
            if (start.format(dateformatShort) === end.format(dateformatShort)) {
                if (start.format(dateformatShort) === momentEST.format(dateformatShort)) {
                    $('#daterange-btn span').html('<b>Date</b>: Today');
                } else {
                    $('#daterange-btn span').html('<b>Date</b>: ' + start.format(dateformatLong));
                }
            } else {
                $('#daterange-btn span').html('<b>From</b>: ' + start.format(dateformatLong) + ' <b>To</b>: ' + end.format(dateformatLong));
            }

            loadJsGridData(start, end);
        }
    );

    $("#refresh-btn").off().on('click', function () {
        loadJsGridData(startDate, endDate);
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

        $('#selRole').select2({
            placeholder: 'ALL',
            ajax: {
                type: "GET",
                url: `../../role/nonadmins`,
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
                            return { id: val.RoleCode, text: val.Name };
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
                    var teamIds = $.map($('#selTeam').select2('data'), function (val, index) { return val.id; });

                    return {
                        q: params.term, // search term
                        page: params.page,
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
                    var accountIds = $.map($('#selAccount').select2('data'), function (val, index) { return val.id; });

                    return {
                        q: params.term, // search term
                        page: params.page,
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

        $('#selTask').select2({
            placeholder: 'ALL',
            ajax: {
                type: "GET",
                /* ?=p1 : To set a default values for array of ids */
                url: `../../task/assigned?p=1`,
                data: function (params) {
                    var teamIds = $.map($('#selTeam').select2('data'), function (val, index) { return val.id; }),
                        accountIds = $.map($('#selAccount').select2('data'), function (val, index) { return val.id; });

                    return {
                        q: params.term, // search term
                        page: params.page,
                        teamIds: teamIds,
                        accountIds: accountIds
                    };
                },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                processResults: function (data, index) {
                    return {
                        results: $.map(data.data, function (val) {
                            return { id: val.Id, text: `${val.Description} - ${val.Text}` };
                        })
                    };
                }
            }
        });

        $('#selUsers').change(loadJsGridData);
        $('#selRole').change(loadJsGridData);
        $('#selAccount').change(loadJsGridData);
        $('#selTeam').change(loadJsGridData);
        $('#selTask').change(loadJsGridData);
    }
        
    function initColHeaderEventHandlers() {

        $('#mgtGrid').off().on('click', 'a', function () {
            var $a = $(this),
                userId = $(this).attr('value'),
                userStatus = $(this).attr('data-userstatus');

            if ($a.is('.editStatus')) {
                $.get("../content/Html/TimeKeepingModals.html", function (html) {
                    var changeStatus = $(html).find('#changeStatusDlg'), result = {};

                    $('.modal-content.md').find('div').remove();
                    $('.modal-content.md').append(changeStatus);

                    $('.modal-title').text('Edit User Status');

                    $('#taskList').select2({
                        ajax: {
                            type: "GET",
                            url: `../../task/for?p=1`,
                            data: function (params) {
                                return {
                                    q: params.term, // search term
                                    page: params.page,
                                    userId: userId
                                };
                            },
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            processResults: function (data, index) {
                                return {
                                    results: $.map(data.data, function (val) {
                                        return { id: val.Id, text: `${val.Description} - ${val.Text}` };
                                    })
                                };
                            }
                        }
                    });

                    setModalBehavior($('#statusList').val());

                    switch (userStatus) {
                        case 'Active':
                        case 'Meeting':
                            $('#statusList').val('Active');
                            break;

                        case 'Lunch':
                        case 'Break':
                        case 'Bio':
                            $('#statusList').val('Break');
                            $('#breakTypes').val(userStatus);
                            break;

                        case 'Out':
                            $('#statusList').val('Out');
                            break;
                    }

                    $('#statusList, #breakTypes, #taskList').off().on('change', function () {
                        eventChangeHandler(userStatus);
                    });

                    eventChangeHandler(userStatus);

                    $('#btnChangeStatus').off().on('click', function () {
                        var data = getChangeStatusData();

                        data = $.extend(data, {
                            userDetailId: userId
                        });

                        $.ajax({
                            type: 'POST',
                            url: `../user/change_status`,
                            dataType: "json",
                            data: JSON.stringify(data),
                            contentType: 'application/json',
                            success: function (response) {
                                result = response
                            },
                            complete: function () {
                                $.displayResponseMessage(result);

                                if (!result.isSuccessful) {
                                    return false;
                                }

                                $(".modal.md").modal('hide');
                                loadJsGridData();
                            }
                        });
                    });

                    $(".modal.md").modal();
                });
            }
            else if ($a.is(".accountlink")) {
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
        });
    }

    function getChangeStatusData() {
        var status = $('#statusList').val(),
            taskTypeId, taskId;

        if (status == 'Active') {
            taskTypeId = 1;
            taskId = $('#taskList').val();
        }
        else if (status == 'Out') {
            taskTypeId = 7;
        }
        else {
            taskTypeId = $('#breakTypes').find('option:selected').attr('data-typeid');
        }

        return {
            taskId : taskId,
            taskTypeId : taskTypeId
        }
    }

    function eventChangeHandler(userStatus) {        

        var status = $('#statusList').val();

        setModalBehavior(status);

        if (status != 'Active') {

            var prefStatus = status == 'Out' ? status : $('#breakTypes').val();

            setChangeBtnBehavior(userStatus, prefStatus);
        }
        else {
            if ($('#taskList').val()) {
                $('#btnChangeStatus').removeAttr('disabled');
            }
            else {
                $('#btnChangeStatus').attr('disabled', true);
            }
            
        }
    }

    function setChangeBtnBehavior(currentStatus, prefStatus) {
        if (currentStatus == prefStatus) {
            $('#btnChangeStatus').attr('disabled', true);
        }
        else {
            $('#btnChangeStatus').removeAttr('disabled');
        }
    }

    function setModalBehavior(status) {
        switch (status) {
            case 'Active':
                $('#taskListContainer').show();
                $('#breakTypesContainer').hide();
                break;
            case 'Break':
                $('#taskListContainer').hide();
                $('#breakTypesContainer').show();
                break;
            default:
                $('#taskListContainer').hide();
                $('#breakTypesContainer').hide();
        }
    }

    function getBallIcon(time, timeLimit) {
        var content = '';
        if (time > 0) {
            var formattedTime = getTimeFormat(time);

            if ((time * 60) > timeLimit) {
                content = `<i class="fa fa-circle text-red" data-toggle="tooltip" data-placement="top" data-html="true" title="${formattedTime}"></i>`;
            }
            else {
                content = `<i class="fa fa-circle text-green" data-toggle="tooltip" data-placement="top" data-html="true" title="${formattedTime}"></i>`;
            }
        }
        
        return content;
    }

    function getTimeFormat(time) {
        var hours = Math.floor(time),
            minutes = (((time - hours) * 60) | 0),
            formatted = `${hours} h ${minutes} m`;

        return formatted;
    }

    function initTableHeaderTooltips(el) {
        el.find('.toggleStatus').bstooltip({
            html: true,
            title: "<i class='fa fa-toggle-off text-muted'></i> Show Active Only<br/><i class='fa fa-toggle-on text-primary'></i> Show OUT Status"
        });
        el.find('.toggleTime').bstooltip({
            html: true,
            title: "<i class='fa fa-toggle-off text-muted'></i> Decimal view<br/><i class='fa fa-toggle-on text-primary'></i> Minute view"
        });
        el.find('.legendLunch').bstooltip({
            html: true,
            title: "<i class='fa fa-circle text-green'></i> within 60 min<br/><i class='fa fa-circle text-red'></i> over the limit"
        });
        el.find('.legendBreak').bstooltip({
            html: true,
            title: "<i class='fa fa-circle text-green'></i> within 15 min<br/><i class='fa fa-circle text-red'></i> over the limit"
        });
        el.find('.legendBio').bstooltip({
            html: true,
            title: "<i class='fa fa-circle text-green'></i> within 10 min<br/><i class='fa fa-circle text-red'></i> over the limit"
        });
    }
    
    var gridEvents = {
        ToggleStatus: function () {
            isActiveOnly = !isActiveOnly;

            let data = JSON.parse(JSON.stringify(timekeepingData));

            startDate = ($('#daterange-btn').data('daterangepicker').startDate).format(reportDateFormat);
            endDate = ($('#daterange-btn').data('daterangepicker').endDate).format(reportDateFormat);

            if (!isActiveOnly) {
                data.Management = data.Management.filter(d => d.Status != 'Out');
                data.TotalUsers = data.Management.length;
                data.TotalTaskTime = 0;

                $.each(data.Management, function (index, val) {
                    data.TotalTaskTime += val.TaskActiveTime;
                });
            }

            $.extend(gridOptions, {
                data: data.Management,
                onRefreshed: function (args) {
                    var time = data.TotalTaskTime, activeTime = '';

                    if (isMinuteView) {
                        var hours = Math.floor(time),
                            minutes = (((time - hours) * 60) | 0);

                        activeTime = `<b>${hours}</b> h <b>${minutes}</b>`;
                    }
                    else {
                        activeTime = `<b>${time}</b>`;
                    }

                    var total = {
                        "ActiveTime": activeTime,
                        "FullName": `${data.TotalUsers} User(s)`,
                        IsTotal: true
                    };

                    var $totalRow = $("<tr>").addClass('total-row');

                    args.grid._renderCells($totalRow, total);
                    args.grid._content.append($totalRow);
                }
            });

            _grid.jsGrid('reset');
            $(_grid).jsGrid(gridOptions);

            _grid.jsGrid("sort", { field: $("#selSort").val() || "FullName", order: $("#selSort").val() == 'TotalActiveTime' ? "desc" : 'asc' });

            initTableHeaderTooltips(_grid);
        },
        ToggleTime: function () {
            isMinuteView = !isMinuteView;

            _grid.jsGrid('render');
            _grid.jsGrid("sort", { field: $("#selSort").val() || "FullName", order: $("#selSort").val() == 'TotalActiveTime' ? "desc" : 'asc' });
            initTableHeaderTooltips(_grid);
        }
    }

    function loadJsGridData(startDate, endDate) {
        startDate = ($('#daterange-btn').data('daterangepicker').startDate).format(reportDateFormat);
        endDate = ($('#daterange-btn').data('daterangepicker').endDate).format(reportDateFormat);

        var eDate = new Date(endDate), curDate = (new Date()).setHours(0, 0, 0, 0);

        var ballIConsIsVisible = startDate == endDate, attendanceColVisibility = eDate < curDate;

        var statusVisible = ballIConsIsVisible && startDate == momentEST.format(reportDateFormat);

        if (statusVisible) $('#selTaskContainer').show(); else $('#selTaskContainer').hide();

        _grid = $("#mgtGrid");
        gridOptions = {
            width: "100%",
            height: "auto",
            pageSize: 30,
            sorting: true,
            paging: true,
            fields: [
                {
                    name: "FullName", type: "text", width: "10%", title: "Name",
                    cellRenderer: function (value, item) {
                        var td = $(`<td></td>`);

                        var hoverclass = ballIConsIsVisible && item.StartTime ? 'hoverpop' : '';

                        td.append(`<a href="#" class='namelink' data-starttime='${item.StartTime}' data-endtime='${item.EndTime}' data-netuserid='${item.NetUserId}'>${value}</a>`);

                        return td;
                    }
                },
                {
                    name: "AccountName", type: "text", width: "11%", title: "Account",
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
                    name: "TeamName", type: "text", width: "15%", title: "Team",
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
                { name: "RoleName", type: "text", width: "8%", title: "Role" },                
                { name: "TaskName", type: "text", width: "10%", title: "Current Task", visible: statusVisible },
                {
                    name: "Status", type: "text", width: "8%", title: "Status", visible: statusVisible,
                    headerTemplate: function () {
                        var thSpan = $('<span>Status </span>'),
                            sToggleClass = isActiveOnly ? 'fa-toggle-on text-primary' : 'fa-toggle-off text-muted',
                            sSpan = $('<span data-toggle="tooltip" class="headToggle toggleStatus"></span>'),
                            a = $(`<a href="#" class="fa ${sToggleClass}"></a>`);

                        a.off().on('click', gridEvents.ToggleStatus);

                        sSpan.append(a);
                        thSpan.append(sSpan);

                        return thSpan;
                    },
                    cellRenderer: function (value, item) {

                        if (!value) {
                            return $(`<td></td>`);
                        }

                        var editIcon = '<i class="glyphicon glyphicon-pencil"></i>',
                            data = `<a href='#' class='button pull-right editStatus' title='Edit Status' value=${item.UserDetailId} data-userstatus="${value}">${timekeepingModulePermission.canEdit ? editIcon : ''}</a>`;
                        return $(`<td>${value} ${data}</td>`);
                    }
                },
                {
                    name: "TotalActiveTime", type: "number", width: "8%", title: "Active",
                    headerTemplate: function () {
                        var thSpan = $('<span>Active </span>'),
                            atToggleClass = isMinuteView ? 'fa-toggle-on text-primary' : 'fa-toggle-off text-muted',
                            atSpan = $('<span data-toggle="tooltip" class="headToggle toggleTime" data-placement="top"></span>'),
                            a = $(`<a href="#" class="fa ${atToggleClass}"></a>`);

                        if (statusVisible) {
                            atSpan.attr('data-placement', 'top');
                        } else {
                            atSpan.attr('data-placement', 'left');
                        }

                        a.off().on('click', gridEvents.ToggleTime);

                        atSpan.append(a);
                        thSpan.append(atSpan);
                        return thSpan;
                    },
                    cellRenderer: function (value, item) {
                        if (item.ActiveTime) {
                            return $(`<td>${item.ActiveTime}</td>`);
                        }

                        var mEnddate = momentEST.format(reportDateFormat);

                        var hoverclass = item.StartTime ? 'hoverpop' : '',
                            endtime = item.Status == 'Out' || mEnddate != endDate ? item.EndTime : '';

                        var span = $(`<span class='${hoverclass}' data-starttime='${item.StartTime}' data-endtime='${ endtime }'></span>`);
                        
                        if (isMinuteView) {
                            var time = item.TaskActiveTime,
                                hours = Math.floor(time),
                                minutes = (((time - hours) * 60) | 0);

                            span.append(`<b>${hours}</b> h <b>${minutes}</b> m`);
                        }
                        else {
                            span.append(`<b>${parseFloat(item.TaskActiveTime).toFixed(2)}</b>`);
                        }

                        return $('<td></td>').append(span);
                    }
                },
                {
                    name: "", type: "text", width: "6%", title: "Lunch", visible: ballIConsIsVisible, sorting: false,
                    headerTemplate: function () {
                        return 'Lunch <span data-toggle="tooltip" class="headToggle legendLunch"><i class="fa fa-question-circle-o text-muted"></i></span>';
                    },
                    cellRenderer: function (value, item) {
                        if (item.Status == 'Lunch') {
                            var lunchTime = getTimeFormat(item.LunchTime);

                            return $(`<td>${lunchTime}</td>`);
                        }
                        else {
                            return $(`<td>${getBallIcon(item.LunchTime, 60)}</td>`);
                        }
                    }
                },
                {
                    name: "", type: "text", width: "6%", title: "Break", visible: ballIConsIsVisible, sorting: false,
                    headerTemplate: function () {
                        return 'Break <span data-toggle="tooltip" class="headToggle legendBreak"><i class="fa fa-question-circle-o text-muted"></i></span>';
                    },
                    cellRenderer: function (value, item) {
                        var td = $('<td></td>');

                        if (item.Status === '1st Break') {
                            td.append(`${getTimeFormat(item.FirstBreakTime)}  `);
                        }
                        else {
                            td.append(`${getBallIcon(item.FirstBreakTime, 15)}  `);
                        }

                        if (item.Status === '2nd Break') {
                            td.append(`${getTimeFormat(item.SecondBreakTime)}`);
                        }
                        else {
                            td.append(getBallIcon(item.SecondBreakTime, 15));
                        }

                        return td;
                    }
                },
                {
                    name: "", type: "text", width: "6%", title: "Bio", visible: ballIConsIsVisible, sorting: false,
                    headerTemplate: function () {
                        return 'Bio <span data-toggle="tooltip" class="headToggle legendBio" data-placement="left"><i class="fa fa-question-circle-o text-muted"></i></span>';
                    },
                    cellRenderer: function (value, item) {
                        if (item.Status === 'Bio') {
                            return $(`<td>${getTimeFormat(item.BioTime)}</td>`);
                        }
                        else {
                            /*Bio Time Data*/
                            return $(`<td>${getBallIcon(item.BioTime, 10)}</td>`);
                        }
                    }
                },
                {
                    name: "WorkedDay", type: "text", width: "7%", title: "Work Days", align: 'center', visible: attendanceColVisibility
                },
                {
                    name: "Absences", type: "text", width: "8%", title: "Absences", align: 'center', visible: attendanceColVisibility
                }

            ]
        };

        var result = {};

        /* Set dropdown filter array values */
        var accountIdArray = $('#selAccount').select2('data')
            , userIdArray = $('#selUsers').select2('data')
            , teamIdArray = $('#selTeam').select2('data')
            , roleArray = $('#selRole').select2('data')
            , taskArray = $('#selTask').select2('data');

        var accountIds = $.map(accountIdArray, function (val, index) {
            return `accountIds[${index}]=${val.id}`;
        }),
            userIds = $.map(userIdArray, function (val, index) {
                return `userIds[${index}]=${val.id}`;
            }),
            teamIds = $.map(teamIdArray, function (val, index) {
                return `teamIds[${index}]=${val.id}`;
            }),
            roles = $.map(roleArray, function (val, index) {
                return `roles[${index}]=${val.id}`;
            }),
            tasks = $.map(taskArray, function (val, index) {
                return `taskIds[${index}]=${val.id}`;
            });

        var urlParams = `startDate=${startDate}&endDate=${endDate}`;

        urlParams = urlParams + (accountIds.length > 0 ? `&${accountIds.join('&')}` : '');
        urlParams = urlParams + (teamIds.length > 0 ? `&${teamIds.join('&')}` : '');
        urlParams = urlParams + (roles.length > 0 ? `&${roles.join('&')}` : '');
        urlParams = urlParams + (userIds.length > 0 ? `&${userIds.join('&')}` : '');

        if (statusVisible) {
            urlParams += (tasks.length > 0 ? `&${tasks.join('&')}` : '');
        }

        $.blockUI();
        $.ajax({
            type: 'GET',
            url: `../tkdata/mgt?${urlParams}`,
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

                timekeepingData = JSON.parse(JSON.stringify(result.data));

                var tempTimekeeping = JSON.parse(JSON.stringify(result.data));

                if (!isActiveOnly) {
                    tempTimekeeping.Management = tempTimekeeping.Management.filter(d => d.Status != 'Out');
                    tempTimekeeping.TotalUsers = tempTimekeeping.Management.length;
                    tempTimekeeping.TotalTaskTime = 0;

                    $.each(tempTimekeeping.Management, function (index, val) {
                        tempTimekeeping.TotalTaskTime += val.TaskActiveTime;
                    });
                }

                $.extend(gridOptions, {
                    data: tempTimekeeping.Management,
                    onRefreshed: function (args) {

                        var time = tempTimekeeping.TotalTaskTime,
                            hours = Math.floor(time),
                            minutes = (((time - hours) * 60) | 0);

                        var activeTime = isMinuteView ? `<b>${hours} h ${minutes} m</b>` : `<b>${parseFloat(tempTimekeeping.TotalTaskTime).toFixed(2)}</b>`;

                        var total = {
                            "ActiveTime": activeTime,
                            "FullName": `${tempTimekeeping.TotalUsers} User(s)`,
                            IsTotal: true
                        };

                        var $totalRow = $("<tr>").addClass('total-row');

                        args.grid._renderCells($totalRow, total);
                        args.grid._content.append($totalRow);

                        initTimeKeepingDetailTooltip($('#mgtGrid'));
                    }
                });

                $(_grid).jsGrid("reset");
                $(_grid).jsGrid(gridOptions);

                initTableHeaderTooltips($('#mgtGrid'));

                initColHeaderEventHandlers();
            }
        });
    }

    function initTimeKeepingDetailTooltip(el) {

        $.each(el.find('.hoverpop'), function (index, value) {
            var elem = $(value);

            var starttime = elem.data('starttime');
            var endtime = elem.data('endtime');

            var startData = starttime != '' ? new Date(starttime).toLocaleString() : '';
            var endData = endtime != '' ? new Date(endtime).toLocaleString() : '';
            
            elem.bstooltip({
                html: true,
                title: `<div class='ttContainer'><small><table><tr><td>Start Time: </td><td>${startData}</td></tr><tr><td>End Time: </td><td>${endData}</td></tr></table></small></div>`
            });
        });
    }
       
    UpdateUserInfo(function (data) {

        timekeepingModulePermission = data.Permissions.find(function (item) {
            return item.ModuleCode == "m-timekeeps";
        });
        
        initFilters();
        loadJsGridData();

    });
});