$(document).ready(function () {
    "use strict";

    $.blockUI();
    var teamId = $.fn.GetUrlParams('teamId'),
        modulePermission = {},
        userModulePermission = {},
        calendar = null,
        holidays = [];

    function initHandlers() {
        $('.refresh').off().on('click', function () {
            loadTeamDetails();
        });

        $("#supervisorGrid").off().on('click', 'a', function () {
            var $a = $(this),
                supId = $(this).attr("value"),
                supName = $(this).attr("data-name");

            if ($a.is(".delete")) {
                $.get("../Content/Html/TeamsManagerModals.html", function (html) {
                    var deleteMod = $(html).find('#delete'), result = {};

                    $('.modal-content.sm').find('div').remove();
                    $('.modal-content.sm').append(deleteMod);

                    $('.modal-title').text('Confirm Remove Supervisor');

                    $('#deleteMsg').html(`<b>${supName}</b> will be removed as a supervisor of this team.`)

                    $(".modal.sm").modal();

                    $('.modal.sm').off('shown.bs.modal').on('shown.bs.modal', function () {
                        $('#deleteBtn').off().on('click', function () {
                            $('.modal.sm').block();
                            $.ajax({
                                type: "POST",
                                url: `../../teams/remove_supervisor`,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                data: JSON.stringify({
                                    TeamId: teamId,
                                    UserId: supId,
                                }),
                                success: function (response) {
                                    result = response;
                                },
                                complete: function () {
                                    $('.modal.sm').unblock();
                                    $.displayResponseMessage(result);

                                    if (!result.isSuccessful) {
                                        return false;
                                    }

                                    $(".modal.sm").modal('hide');

                                    loadTeamDetails();
                                }
                            });
                        });
                    });
                });
            }

            if ($a.is('.namelink')) {
                var userid = $a.attr('data-netuserid');
                gotoProfile(userid);
            }
        });

        $("#agentGrid").off().on('click', 'a', function () {
            var $a = $(this),
                agentId = $(this).attr("value"),
                agentName = $(this).attr("data-name");

            if ($a.is(".delete")) {
                $.get("../Content/Html/TeamsManagerModals.html", function (html) {
                    var deleteAgent = $(html).find('#delete'), result = {};

                    $('.modal-content.sm').find('div').remove();
                    $('.modal-content.sm').append(deleteAgent);

                    $('.modal-title').text('Confirm Remove Agent');

                    $('#deleteMsg').html(`<b>${agentName}</b> will be removed as an agent from this team.`)

                    $(".modal.sm").modal();

                    $('.modal.sm').off('shown.bs.modal').on('shown.bs.modal', function () {
                        $('#deleteBtn').off().on('click', function () {
                            $('.modal.sm').block();
                            $.ajax({
                                type: "POST",
                                url: `../../teams/remove_agent`,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                data: JSON.stringify({
                                    TeamId: teamId,
                                    UserId: agentId,
                                }),
                                success: function (response) {
                                    result = response;
                                },
                                complete: function () {
                                    $('.modal.sm').unblock();
                                    $.displayResponseMessage(result);

                                    if (!result.isSuccessful) {
                                        return false;
                                    }

                                    $(".modal.sm").modal('hide');
                                    loadTeamDetails();
                                }
                            });
                        });
                    });
                });
            }

            if ($a.is('.namelink')) {
                var userid = $a.attr('data-netuserid');
                gotoProfile(userid);
            }
        });
        
        $("#taskGrid").off().on('click', 'a', function () {
            var $a = $(this),
                taskId = $(this).attr("value"),
                taskName = $(this).attr("data-name");

            if ($a.is(".delete")) {
                $.get("../Content/Html/TeamsManagerModals.html", function (html) {
                    var deleteTask = $(html).find('#delete'), result = {};

                    $('.modal-content.sm').find('div').remove();
                    $('.modal-content.sm').append(deleteTask);

                    $('.modal-title').text('Confirm Remove Task');

                    $('#deleteMsg').html(`<b>${taskName}</b> task will be removed from this team.`)

                    $(".modal.sm").modal();

                    $('.modal.sm').off('shown.bs.modal').on('shown.bs.modal', function () {
                        $('#deleteBtn').off().on('click', function () {
                            $('.modal.sm').block();
                            $.ajax({
                                type: "POST",
                                url: `../../task/delete`,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                data: JSON.stringify({
                                    Id: taskId,
                                    TeamId: teamId
                                }),
                                success: function (response) {
                                    result = response;
                                },
                                complete: function () {
                                    $('.modal.sm').unblock();
                                    $.displayResponseMessage(result);

                                    if (!result.isSuccessful) {
                                        return false;
                                    }

                                    $(".modal.sm").modal('hide');
                                    loadTeamDetails();
                                }
                            });
                        });
                    });
                });
            }

            if ($a.is('.edit')) {
                showTaskModal(taskId);
            }
        });

        $('#supAdd').off().on('click', function () {
            $.get("../Content/Html/TeamsManagerModals.html", function (html) {
                var addSupervisor = $(html).find('#addAgentSupervisor'), result = {};

                $('.modal-content.md').find('div').remove();
                $('.modal-content.md').append(addSupervisor);

                $('#fieldLabel').text('Supervisor');
                $('.modal-title').text('Add Supervisor');

                $('#Field').select2({
                    ajax: {
                        type: "GET",
                        url: `../teams/supervisors?teamId=${teamId}`,
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

                $(".modal.md").modal();

                $('.modal.md').off('shown.bs.modal').on('shown.bs.modal', function () {
                    $('#btnAdd').off().on('click', function () {
                        if (!($('.modal.md').find('.modal-body')).ValidateForm()) {
                            return;
                        }

                        $('.modal.md').block();

                        $.ajax({
                            type: "POST",
                            url: `../../teams/add_supervisor`,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            data: JSON.stringify({
                                TeamId: teamId,
                                UserId: $('#Field').val(),
                            }),
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
                                loadTeamDetails();
                            }
                        });
                    });
                });
            });
        });

        $('#agentAdd').off().on('click', function () {
            $.get("../Content/Html/TeamsManagerModals.html", function (html) {
                var addAgent = $(html).find('#addAgentSupervisor'), result = {};

                $('.modal-content.md').find('div').remove();
                $('.modal-content.md').append(addAgent);

                $('#fieldLabel').text('Agent');
                $('.modal-title').text('Add Agent');

                $('#Field').select2({
                    placeholder: 'Select an Agent',
                    ajax: {
                        type: "GET",
                        url: `../teams/agents?teamId=${teamId}`,
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

                $(".modal.md").modal();

                $('.modal.md').off('shown.bs.modal').on('shown.bs.modal', function () {
                    $('#btnAdd').off().on('click', function () {
                        if (!($('.modal.md').find('.modal-body')).ValidateForm()) {
                            return;
                        }

                        $('.modal.md').block();

                        $.ajax({
                            type: "POST",
                            url: `../../teams/add_agent`,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            data: JSON.stringify({
                                TeamId: teamId,
                                UserId: $('#Field').val(),
                            }),
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
                                loadTeamDetails();
                            }
                        });
                    });
                });
            });
        });

        $('#taskAdd').off().on('click', function () {
            showTaskModal();
        });

        $('#tmAccount').click(function () {
            var accountId = $(this).attr('data-acc_id'),
                accname = $(this).text();

            window.location = `../../Team?Account=${accountId}&Name=${accname}`;

            return false;
        });

        $('#dhSave').click(function () {
            $.blockUI();

            var dayOffs = [], holidays = [], result = {};
                        
            dayOffs = $.map($('.dayoffitems input:checked'), function (val, index) {
                return {
                    Day: $(val).data('name')
                }
            });

            holidays = $.map(calendar.getEvents(), function (val, index) {
                return {
                    HolidayDate: `${val.start.getFullYear()}-${val.start.getMonth() + 1}-${val.start.getDate()}`,
                    Title: val.title,
                    Description: val.extendedProps.description
                };
            });

            var doData = {
                TeamId : teamId,
                DayOffs: dayOffs,
                Holidays: holidays 
            }

            $.ajax({
                type: "POST",
                url: `../teams/update_dayoff_holidays`,
                contentType: "application/json; charset=utf-8",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(doData),
                success: function (response) {
                    result = response;
                },
                complete: function () {
                    $.displayResponseMessage(result);

                    if (!result.isSuccessful) {                        
                        return false;
                    }

                    loadTeamDetails();
                }
            });
        });

        $('#holidayList').click(function () {

            var yearList = $.map(holidays,
                function (value, index) {
                    return new Date(value.HolidayDate).getFullYear();
                }
            );
            
            yearList = yearList.filter(
                (v, i, a) => a.findIndex(s => s === v) === i
            );

            if (yearList.length > 0) {
                $.get("../Content/Html/TeamsManagerModals.html", function (html) {
                    var holidaysBox = $(html).find('#holidayListBox');

                    $('.modal-content.md').find('div').remove();
                    $('.modal-content.md').append(holidaysBox);

                    var currentYear = (new Date()).getFullYear();

                    if (yearList.findIndex(y => y == currentYear) == -1) {
                        currentYear = yearList[yearList.length - 1];
                    }

                    reloadHolidayList(currentYear, yearList);

                    $('#yearValue').text(currentYear);

                    $('.fc-next-button').click(function () {
                        var yearVal = $('#yearValue').text();

                        var index = yearList.findIndex(y => y == yearVal);

                        if (index < (yearList.length - 1)) {
                            var value = yearList[index + 1];
                            $('#yearValue').text(value);

                            reloadHolidayList(value, yearList);
                        }
                    });

                    $('.fc-prev-button').click(function () {
                        var yearVal = $('#yearValue').text();

                        var index = yearList.findIndex(y => y == yearVal);

                        if (index > 0) {
                            var value = yearList[index - 1];
                            $('#yearValue').text(yearList[index - 1]);

                            reloadHolidayList(value, yearList);
                        }
                    });

                    $(".modal.md").modal();
                });
            }
            else {
                Notify('No holiday data has been set.', null, null, 'danger');
            }

        });
    }

    function reloadHolidayList(year, yearList) {
        var holidaysMapped = {};
        var _grid = $("#holidaysGrid"),
            gridOptions = {
                width: "100%",
                height: "auto",
                pageSize: 30,
                sorting: false,
                paging: true,
                fields: [
                    {
                        name: "HolidayDate", type: "text", width: "8%", title: "Holiday Date",
                        sorter: function (val1, val2) {
                            var date1 = new Date(val1);
                            var date2 = new Date(val2);

                        },
                    },
                    { name: "Title", type: "text", width: "10%", title: "Title" },
                    { name: "Description", type: "text", width: "15%", title: "Description" }
                ],
                onRefreshing: function (args) {
                    $.each(args.grid.data, function (index, data) {
                        data.number = index + 1;
                    });
                }
            }

        holidaysMapped = $.map(holidays,
            function (value, index) {
                return {
                    Title: value.Title,
                    Description: value.Description,
                    HolidayDate: new Date(value.HolidayDate).toDateString(),
                    UnformatDate: value.HolidayDate
                };
            }
        );

        $("#years").select2({
            data: yearList
        });

        $('#years').change(function () {
            reloadHolidayList($(this).val(), yearList)
        });

        holidaysMapped = holidaysMapped.filter((v) => v.HolidayDate.includes(year));

        $.extend(gridOptions, {
            data: holidaysMapped
        });

        $(_grid).jsGrid("reset");
        $(_grid).jsGrid(gridOptions);
    }

    function gotoProfile(options) {
        window.location = `../Profile/Index/${options}`;
    }

    function showTaskModal(taskId) {
        $.get("../Content/Html/TeamsManagerModals.html", function (html) {
            var addTask = $(html).find('#addTask'), result = {};

            $('.modal-content.md').find('div').remove();
            $('.modal-content.md').append(addTask);

            if (taskId) {
                $('.modal-title').text('Update Task');

                $('.modal.md').block();
                $.ajax({
                    type: "GET",
                    url: `../../task/get?taskId=${taskId}`,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        result = response;
                    },
                    complete: function () {
                        $('.modal.md').unblock();

                        if (!result.isSuccessful) {
                            if (result.message) Notify(result.message, null, null, 'danger');
                            return false;
                        }

                        var task = result.data;

                        $('#taskId').val(task.Id);
                        $('#Name').val(task.Name);

                        $('#Description').val(task.Description);

                        $(".modal.md").modal();
                    }
                });
            }

            $(".modal.md").modal();

            $('.modal.md').off('shown.bs.modal').on('shown.bs.modal', function () {
                $('#btnAdd').off().on('click', function () {
                    if (!($('.modal.md').find('.modal-body')).ValidateForm()) {
                        return;
                    }

                    $('.modal.md').block();

                        $.ajax({
                            type: "POST",
                            url: `../../task/save`,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            data: JSON.stringify({
                                Id: $('#taskId').val() || 0,
                                TeamId: teamId,
                                Name: $('#Name').val(),
                                Description: $('#Description').val(),
                            }),
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
                                loadTeamDetails();
                            }
                    });
                });
            });
        });
    }

    function loadTeamDetails() {
        var result = {};
        $.blockUI();
        $.ajax({
            type: 'GET',
            url: `../teams/details?teamId=${teamId}`,
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

                if (!result.data) {
                    Notify('Something went wrong. Please contact your system administrator.', null, null, 'danger')
                    return false;
                }

                holidays = result.data.Holidays;

                $('.teamName').text(result.data.Name);
                $('.teamDesc').text(result.data.Description);
                $('.teamAccount').text(result.data.AccountName);
                $('.teamShift').text(result.data.ShiftName);
                $('#tmAccount').attr('data-acc_id', result.data.AccountId);

                loadSupervisors(result.data.Supervisors);
                loadTasks(result.data.Tasks);
                loadAgents(result.data.Agents);
                loadDayOffHolidayData({
                    dayOffs: result.data.DayOffs,
                    holidays: result.data.Holidays
                });
            }
        });
    }

    function loadDayOffHolidayData(data){
        $('input').iCheck({
            checkboxClass: 'icheckbox_square-green'
        });

        if (UserInfo.RoleCode == 'SA' || UserInfo.RoleCode == 'AM') {
            $('.dayoffitems input[type=checkbox]').iCheck('enable');
            $('#dhSave').show();
        } else {
            $('.dayoffitems input[type=checkbox]').iCheck('disable');
            $('#dhSave').hide();
        }

        $.each(data.dayOffs, function (index, val) {
            $('.dayoffitems').find(`input[data-name=${val.Day}]`).iCheck('check');
            $('.dayoffitems').find(`input[data-name=${val.Day}]`).iCheck('update');
        });

        data.holidays = $.map(data.holidays, function (val, index) {
            return {
                title: val.Title,
                description: val.Description,
                start: val.HolidayDate.replace('T00:00:00', ''),
                end: val.HolidayDate.replace('T00:00:00', '')
            }
        });

        var calEl = $('#holidaysCal')[0];

        if (calendar) {
            $.each(calendar.getEvents(), function (index, data) {
                data.remove();
            });

            calendar.addEventSource(data.holidays);

            return;
        }
        
        calendar = new FullCalendar.Calendar(calEl, {
            plugins: ['interaction', 'dayGrid'],
            header: {
                left: 'prev,next today',
                right: 'title'
            },
            selectable: true,
            editable: true,
            selectMirror: true,
            select: function (arg) {
                var existingEvent = calendar.getEvents()
                    .find(e => e.start == arg.start.toString());

                if (!existingEvent) {
                    if (UserInfo.RoleCode == 'SA' || UserInfo.RoleCode == 'AM') {
                        calEventModal({
                            Title: '',
                            Description: '',
                            Calendar: calendar,
                            Arg: arg
                        });
                    }
                }
            },
            eventClick: function (info) {
                calEventModal({
                    Title: info.event.title,
                    Description: info.event.extendedProps.description,
                    Calendar: calendar,
                    Event: info.event
                });
            },
            events: data.holidays
        });

        calendar.render();
    }

    function calEventModal(options) {
        $.get("../Content/Html/TeamsManagerModals.html", function (html) {
            var holiday = $(html).find('#holiday'), result = {};

            $('.modal-content.md').find('div').remove();
            $('.modal-content.md').append(holiday);

            if (options.Event) {

                $('#btnAdd').hide();

                if ((UserInfo.RoleCode == 'SA' || UserInfo.RoleCode == 'AM')) {
                    $('#title')[0].removeAttribute('disabled', '');
                    $('#description')[0].removeAttribute('disabled', '');
                    $('#btnRemove, #btnAdd').show();
                    $('#btnAdd').text('Save');
                }
                else {
                    $('#title')[0].setAttribute('disabled', '');
                    $('#description')[0].setAttribute('disabled', '');
                }

                $('.modal-title').text('Holiday Details')
            }

            $('#title').val(options.Title);
            $('#description').text(options.Description);

            $(".modal.md").modal();

            $('.modal.md').off('shown.bs.modal').on('shown.bs.modal', function () {
                $('#btnAdd').off().on('click', function () {

                    if (!($('.modal.md').find('.modal-body')).ValidateForm()) {
                        return;
                    }

                    var evt = null;

                    if (options.Event) {
                        evt = options.Event;

                        evt.setProp('title', $('#title').val());
                        evt.setExtendedProp('description', $('#description').val());
                    }
                    else {
                        evt = options.Arg

                        options.Calendar.addEvent({
                            title: $('#title').val(),
                            start: evt.start,
                            end: evt.start,
                            allDay: evt.allDay,
                            description: $('#description').val()
                        });
                    }

                    options.Calendar.unselect();
                    $(".modal.md").modal('hide');
                });

                $('#btnRemove').off().on('click', function () {

                    if (!($('.modal.md').find('.modal-body')).ValidateForm()) {
                        return;
                    }

                    options.Event.remove();

                    options.Calendar.unselect();
                    $(".modal.md").modal('hide');
                });
            });
        });
    }

    function loadSupervisors(data) {
        var _grid = $("#supervisorGrid"),
            gridOptions = {
                width: "100%",
                height: "auto",
                pageSize: 30,
                sorting: true,
                paging: true,
                fields: [
                    {
                        name: "FullName", type: "text", width: "12%", title: "Name",
                        sorting: true,
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');

                            if (userModulePermission.canView) {
                                td.append(`<a href="#" class='${userModulePermission.canView ? 'namelink' : ''}' data-netuserid='${item.NetUserId}'>${value}</a>`);
                            } else {
                                td.append(value);
                            }                            

                            return td;
                        }
                        
                    },
                    {
                        name: "Email", type: "text", width: "12%", title: "Email",
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');

                            if (value) {
                                if (userModulePermission.canView) {
                                    td.append(`<a href="#" class='${userModulePermission.canView ? 'namelink' : ''}' data-netuserid='${item.NetUserId}'>${value}</a>`);
                                } else {
                                    td.append(value);
                                }                                
                            }

                            return td;
                        }
                    },
                    { name: "control", type: "text", width: "7%", align: 'center', title: "Actions" }
                ],

                onRefreshed: function (args) {
                    var items = args.grid.option("data");
                    var total = {
                        "FullName": 0,
                        IsTotal: true
                    };
                    
                    total.FullName = `${items.length} Supervisors`

                    var $totalRow = $("<tr>").addClass('total-row');
                    args.grid._renderCells($totalRow, total);
                    args.grid._content.append($totalRow);
                },

                onRefreshing: function (args) {
                    $.each(args.grid.data, function (index, data) {

                        var deleteBtn = `<a href="#" value="${data.NetUserId}" data-name="${data.FullName}" class="button delete"><i class="glyphicon glyphicon-trash"></i></a>`;

                        if (!data.control) {
                            data.control = `${modulePermission.canEdit ? deleteBtn : ''}`
                        }
                    });
                }
            };

        $.extend(gridOptions, {
            data: data
        });

        $(_grid).jsGrid("reset");
        $(_grid).jsGrid(gridOptions);
    }

    function loadTasks(data) {
        var _grid = $("#taskGrid"),
            gridOptions = {
                width: "100%",
                height: "auto",
                pageSize: 30,
                sorting: true,
                paging: true,
                fields: [
                    {
                        name: "number", type: "number", width: "1%", title: "No.",
                        sorting: true
                    },
                    {
                        name: "Name", type: "text", width: "12%", title: "Task",
                        sorting: true
                    },
                    { name: "Description", type: "text", width: "12%", title: "Description" },
                    { name: "control", type: "text", width: "7%", align: 'center', title: "Actions" }
                ],

                onRefreshed: function (args) {
                    var items = args.grid.option("data");
                    var total = {
                        "Name": 0,
                        IsTotal: true
                    };

                    total.Name = `${items.length} Tasks`

                    var $totalRow = $("<tr>").addClass('total-row');
                    args.grid._renderCells($totalRow, total);
                    args.grid._content.append($totalRow);
                },

                onRefreshing: function (args) {
                    $.each(args.grid.data, function (index, data) {
                        data.number = index + 1;
                        var deleteBtn = `<a href="#" value="${data.Id}" data-name="${data.Name}" class="button delete"><i class="glyphicon glyphicon-trash"></i></a>`;
                        var editBtn = `<a href="#" value="${data.Id}" data-name="${data.Name}" class="button edit"><i class="glyphicon glyphicon-pencil"></i></a>`;

                        if (!data.control) {
                            data.control = `${modulePermission.canEdit ? editBtn : ''} ${modulePermission.canEdit ? deleteBtn : ''}`;
                        }
                    });
                }
            };

        $.extend(gridOptions, {
            data: data
        });

        $(_grid).jsGrid("reset");
        $(_grid).jsGrid(gridOptions);
    }

    function loadAgents(data) {
        var _grid = $("#agentGrid"),
            gridOptions = {
                width: "100%",
                height: "auto",
                pageSize: 30,
                sorting: true,
                paging: true,
                fields: [
                    {
                        name: "FullName", type: "text", width: "12%", title: "Name",
                        sorting: true,
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');

                            if (userModulePermission.canView) {
                                td.append(`<a href="#" class='${userModulePermission.canView ? 'namelink' : ''}' data-netuserid='${item.NetUserId}'>${value}</a>`);
                            }
                            else {
                                td.append(value);
                            }

                            return td;
                        }
                    },
                    {
                        name: "Email", type: "text", width: "12%", title: "Email",
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');

                            if (value) {
                                if (userModulePermission.canView) {
                                    td.append(`<a href="#" class='${userModulePermission.canView ? 'namelink' : ''}' data-netuserid='${item.NetUserId}'>${value}</a>`);
                                }
                                else {
                                    td.append(value);
                                }
                            }

                            return td;
                        }
                    },
                    { name: "control", type: "text", width: "7%", align: 'center', title: "Actions" }
                ],

                onRefreshed: function (args) {
                    var items = args.grid.option("data");
                    var total = {
                        "FullName": 0,
                        IsTotal: true
                    };

                    total.FullName = `${items.length} Agents`

                    var $totalRow = $("<tr>").addClass('total-row');
                    args.grid._renderCells($totalRow, total);
                    args.grid._content.append($totalRow);
                },

                onRefreshing: function (args) {
                    $.each(args.grid.data, function (index, data) {

                        var deleteBtn = `<a href="#" value="${data.NetUserId}" data-name="${data.FullName}" class="button delete"><i class="glyphicon glyphicon-trash"></i></a>`;

                        if (!data.control) {
                            data.control = `${modulePermission.canEdit ? deleteBtn : ''}`
                        }
                    });
                }
            };

        $.extend(gridOptions, {
            data: data
        });

        $(_grid).jsGrid("reset");
        $(_grid).jsGrid(gridOptions);
    }

    function initPageUserAccess() {

        if (modulePermission.canEdit) {
            $('.add').show();
        }
        else {
            $('.add').hide();
        }
    }

    UpdateUserInfo(function (data) {
        modulePermission = data.Permissions.find(function (item) {
            return item.ModuleCode == "m-teams";
        });

        userModulePermission = data.Permissions.find(function (item) {
            return item.ModuleCode == "m-users";
        });

        initPageUserAccess();

        initHandlers();
        loadTeamDetails();
    });
});
