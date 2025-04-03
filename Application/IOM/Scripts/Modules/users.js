$(document).ready(function () {
    "use strict";
    $.blockUI();

    var userModulePermission = {};

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
                        roles = $.map($('#selRole').select2('data'), function (val, index) { return val.id; });

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
                url: `../role/all`,
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
                /* ?=p1: To set a default values for array of ids */
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

        $('#selRole').change(loadUsersList);
        $('#selAccount').change(loadUsersList);
        $('#selTeam').change(loadUsersList);
        $('#selUsers').change(loadUsersList);
    }

    function initHandlers() {
        $("#userGrid").off().on('click', 'a', function () {
            var $a = $(this),
                userId = $(this).attr("value"),
                users_name = $a.attr('data-name'),
                isLocked = $a.attr('data-islocked'),
                netUserId = $a.attr('data-netuserid'),
                email = $a.attr('data-email');

            if ($a.is(".edit")) {
                var roleCode = $a.attr('data-role');

                $.get("../content/Html/UsersModals.html", function (html) {

                    var editUser = $(html).find('#editUser'), result = {};

                    $('.modal-content.md').find('div').remove();
                    $('.modal-content.md').append(editUser);

                    var title = 'Edit User';
                    $('.modal-title').text(title);

                    setEditUiAccess(roleCode, $('.modal-content.md').find('.modal-body'));

                    switch (roleCode) {
                        case 'AG':
                        case 'LA':
                            if ($('#Role').select2().length > 0) {
                                $('#Role').select2('destroy');
                            }

                            $('#Role').select2({
                                ajax: {
                                    type: "GET",
                                    url: `../role/all`,
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

                            if ($('#Account').select2().length > 0) {
                                $('#Account').select2('destroy');
                            }

                            $('#Account').select2({
                                placeholder: 'Select an Account',
                                ajax: {
                                    type: "GET",
                                    /* ?=p1: To set a default values for array of ids */
                                    url: `../../accounts/assigned?p=1`,
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

                            if ($('#EmployeeStatus').select2().length > 0) {
                                $('#EmployeeStatus').select2('destroy');
                            }

                            $('#EmployeeStatus').select2({
                                ajax: {
                                    type: "GET",
                                    url: `../user/emp-status`,
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

                            if ($('#Shift').select2().length > 0) {
                                $('#Shift').select2('destroy');
                            }

                            $('#Shift').select2({
                                ajax: {
                                    type: "GET",
                                    url: `../user/emp-shift`,
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

                            if ($('#SelectedWeekdays').select2().length > 0) {
                                $('#SelectedWeekdays').select2('destroy');
                            }

                            $('#SelectedWeekdays').select2({
                                ajax: {
                                    type: "GET",
                                    url: `../user/emp-week-schedule`,
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
                            break
                    }

                    if (userId) {
                        $.ajax({
                            type: "GET",
                            url: `../user/details?userDetailId=${userId}`,
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

                                var user = result.data;

                                $('#userDetailId').val(user.UserDetailsId);
                                $('#FirstName').val(user.FirstName);
                                $('#LastName').val(user.LastName);
                                $('#Email').val(user.Email);
                                $('#HourlyRate').val(user.HourlyRate);

                                $('#Role').append(new
                                    Option(`${user.RoleName}`, user.RoleCode, true, true)
                                ).trigger('change');

                                if (user.Accounts.length > 0) {
                                    var account = user.Accounts[0];

                                    $('#Account').append(new
                                        Option(`${account.Name}`, account.Id, true, true)
                                    ).trigger('change');
                                }

                                $('#EmployeeStatus').append(new
                                    Option(`${user.EmployeeStatus}`, user.EmployeeStatusId, true, true)
                                ).trigger('change');

                                $('#Shift').append(new
                                    Option(`${user.ShiftName}`, user.EmployeeShiftId, true, true)
                                ).trigger('change');

                                if (user.WeekSchedule) {

                                    $.each(user.WeekSchedule, function (index, data) {
                                        var option = new Option(`${weekdays[data.NumericDay]}`, data.NumericDay, true, true);
                                        $('#SelectedWeekdays').append(option).trigger('change');
                                    });
                                }

                                $(".modal.md").modal();
                            }
                        });
                    }

                    $('.modal.md').off('shown.bs.modal').on('shown.bs.modal', function () {
                        result = {};

                        $('#btnSave').off().on('click', function () {
                            if (!($('.modal.md').find('div.basic')).ValidateForm()) {
                                return;
                            }

                            var invalid = validate({ EmailAddress: $('#Email').val() }, {
                                EmailAddress: {
                                    presence: true,
                                    // must be an email (duh)
                                    email: true
                                }
                            });

                            if (invalid) {
                                $('#Email').closest('div.email').addClass('has-error has-danger');
                                return;
                            }

                            var weekSched = [];

                            if (roleCode == 'AG' || roleCode == 'LA') {
                                if (!($('.modal.md').find('div.employment')).ValidateForm()) {
                                    return;
                                }

                                weekSched = $.map($('#SelectedWeekdays').select2('data'), function (val, index) {
                                    return {
                                        NumericDay: val.id
                                    };
                                });
                            }

                            $('.modal.md').block();
                            $.ajax({
                                type: "POST",
                                url: `../user/save_detail`,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                data: JSON.stringify({
                                    AccountId: $('#Account').val(),
                                    UserDetailsId: $('#userDetailId').val(),
                                    FirstName: $('#FirstName').val(),
                                    LastName: $('#LastName').val(),
                                    Email: $('#Email').val(),
                                    RoleCode: $('#Role').val(),
                                    EmployeeStatusId: $('#EmployeeStatus').val(),
                                    HourlyRate: $('#HourlyRate').val(),
                                    EmployeeShiftId: $('#Shift').val(),
                                    WeekSchedule: weekSched
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
                                    loadUsersList();
                                }
                            });
                        });
                    });
                });
            }
            else if ($a.is(".lock")) {
                $.get("../content/Html/UsersModals.html", function (html) {
                    var lockUser = $(html).find('#lockUser'), result = {};

                    $('.modal-content.sm').find('div').remove();
                    $('.modal-content.sm').append(lockUser);

                    var title = 'Confirm';
                    $('.modal-title').text(title);
                    $('#userName').text(users_name);

                    $('#lockHeader').removeClass('modal-info modal-danger')
                        .addClass(isLocked == 'true' ? 'modal-info' : 'modal-danger');

                    $('#lock_action').text(isLocked == 'true' ? 'Unlocked' : 'Locked');

                    $('#deny_grant').text(isLocked == 'true' ? 'granted' : 'denied')
                        .removeClass('text-red text-green')
                        .addClass(isLocked == 'true' ? 'text-blue' : 'text-red');

                    $('#lockBtn').removeClass('btn-info btn-danger')
                        .addClass(isLocked == 'true' ? 'btn-info' : 'btn-danger');
                    $('#lockBtn').text(isLocked == 'true' ? 'Unlock' : 'Lock');

                    $(".modal.sm").modal();

                    $('.modal.sm').off('shown.bs.modal').on('shown.bs.modal', function () {
                        result = {};

                        $('#lockBtn').off().on('click', function () {
                            $('.modal.sm').block();
                            $.ajax({
                                type: "POST",
                                url: `../user/lock?userId= ${userId}`,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                data: {},
                                success: function (response) {
                                    result = response;
                                },
                                complete: function () {
                                    $('.modal.sm').unblock();

                                    $.displayResponseMessage(result);
                                    if (!result.isSuccessful) {
                                        return false;
                                    }

                                    loadUsersList();
                                    $(".modal.sm").modal('hide');
                                }
                            });
                        });
                    });
                });
            }
            else if ($a.is(".delete")) {
                $.get("../content/Html/UsersModals.html", function (html) {
                    var deleteUser = $(html).find('#deleteUser'), result = {};

                    $('.modal-content.sm').find('div').remove();
                    $('.modal-content.sm').append(deleteUser);

                    var title = 'Confirm Delete User';
                    $('.modal-title').text(title);
                    $('#userName').text(users_name);

                    $(".modal.sm").modal();

                    $('.modal.sm').off('shown.bs.modal').on('shown.bs.modal', function () {
                        result = {};

                        $('#deleteBtn').off().on('click', function () {
                            $('.modal.sm').block();
                            $.ajax({
                                type: "POST",
                                url: `../user/delete?userId= ${userId}`,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                data: {},
                                success: function (response) {
                                    result = response;
                                },
                                complete: function () {
                                    $('.modal.sm').unblock();

                                    $.displayResponseMessage(result);
                                    if (!result.isSuccessful) {
                                        return false;
                                    }

                                    loadUsersList();
                                    $(".modal.sm").modal('hide');
                                }
                            });
                        });
                    });
                });
            }
            else if ($a.is(".resetPW")) {
                $.get("../content/Html/UsersModals.html", function (html) {
                    var sendResetPassword = $(html).find('#sendResetPassword'), result = {};

                    $('.modal-content.sm').find('div').remove();
                    $('.modal-content.sm').append(sendResetPassword);

                    $('#Email').val(email);

                    $(".modal.sm").modal();

                    $('.modal.sm').off('shown.bs.modal').on('shown.bs.modal', function () {
                        result = {};

                        $('#sendBtn').off().on('click', function () {
                            $('.modal.sm').block();
                            $.ajax({
                                type: "POST",
                                url: `../user/send_reset_pass?email= ${email}`,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                data: {},
                                success: function (response) {
                                    result = response;
                                },
                                complete: function () {
                                    $('.modal.sm').unblock();

                                    $.displayResponseMessage(result);
                                    if (!result.isSuccessful) {
                                        return false;
                                    }

                                    loadUsersList();
                                    $(".modal.sm").modal('hide');
                                }
                            });
                        });
                    });
                });
            }
            else if ($a.is(".permission")) {
                var uname = $a.attr('data-uname'),
                    urole = $a.attr('data-urole');

                $.get("../content/Html/UsersModals.html", function (html) {
                    var userPermission = $(html).find('#userPermission'), result = {};

                    $('.modal-content.md').find('div').remove();
                    $('.modal-content.md').append(userPermission);

                    $(".modal.md").modal();
                    $('.modal.md').off('shown.bs.modal').on('shown.bs.modal', function () {
                        $("#userId").val(userId);
                        $('#netuserid').val(netUserId);

                        EditPermission.init(function (data) {
                            $('#userPermission').find('input[type=checkbox]').iCheck({
                                checkboxClass: 'icheckbox_square-green'
                            });

                            if (data.IsLocked) {
                                $('#userPermission').find('input[type=checkbox]').iCheck('disable');
                                $('#userPermissionsBtns').hide();

                                $('#info').append(`<li class='form-group has-warning'><span class='help-block'><i class='fa fa-bell-o'></i>  Permission settings is locked and not editable.</span></li>`)
                            }
                        });

                        $('#uname').text(uname);
                        $('#urole').text(urole);
                    });
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

            function setEditUiAccess(role, el) {
                switch (role) {
                    case 'SA':
                    case 'AM':
                    case 'CL':
                        $(el).find('.form-group').hide();
                        $('.name, .email').show();
                        break;
                    case 'LA':
                    case 'AG':
                        $(el).find('.form-group').hide();
                        $('.name, .email, .role, .account, .employeeStatus, .hourlyRate, .shift, .weekdaysShed').show();
                        break;
                }
            }
        });

        $('#add-btn').off().on('click', function () {
            showAddUserModal();
        });

        $("#refresh-btn").off().on('click', function () {
            loadUsersList();
        });
    }

    function showAddUserModal() {
        $.get("../content/Html/UsersModals.html", function (html) {

            var addUser = $(html).find('#addUser');

            $('.modal-content.md').find('div').remove();
            $('.modal-content.md').append(addUser);

            var title = 'Create New User';
            $('.modal-title').text(title);

            initForms();

            $(".modal.md").modal();

            $('.modal.md').off('shown.bs.modal').on('shown.bs.modal', function () {
                $('#btnSave').off().on('click', function () {
                    var roleCode = $('#aRole').val();

                    if (!($('.modal.md').find('div.modal-body')).ValidateForm()) {
                        return;
                    }

                    var invalid = validate({ EmailAddress: $('#Email').val() }, {
                        EmailAddress: {
                            presence: true,
                            // must be an email (duh)
                            email: true
                        }
                    });

                    if (invalid) {
                        $('#Email').closest('div.email').addClass('has-error has-danger');
                        return;
                    }

                    saveUser(roleCode);
                });
            });
        });
    }

    function saveUser(roleCode) {
        var result = {};
        
        var userdata = {
            UserDetailsId: $('#userDetailId').val(),
            FirstName: $('#FirstName').val(),
            LastName: $('#LastName').val(),
            Email: $('#Email').val(),
            RoleCode: $('#aRole').val()
        }, weekSched = null;

        switch (roleCode) {
            case 'LA':
            case 'AG':
                weekSched = $.map($('#SelectedWeekdays').select2('data'), function (val, index) {
                    return val.id;
                });

                userdata = $.extend(userdata, {
                    AccountId: $('#Account').val(),
                    EmployeeStatusId: $('#EmployeeStatus').val(),
                    HourlyRate: $('#HourlyRate').val(),
                    EmployeeShiftId: $('#Shift').val(),
                    WeekSchedule: weekSched.join(',')
                });
                break;
        }
        $('.modal.md').block(); 

        $.ajax({
                type: "POST",
                url: `../user/add`,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(userdata),
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
                    loadUsersList();
                }
            });        
    }

    function initForms() {
        if ($('#aRole').select2().length > 0) {
            $('#aRole').select2('destroy');
        }

        $('#aRole').select2({
            placeholder: 'Select an Role',
            ajax: {
                type: "GET",
                url: `../role/all`,
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

        $('#aRole').change(function () {
            var role = $(this).val();

            switch (role) {
                case 'SA':
                case 'AM':
                case 'CL':
                    $('.employment').hide();
                    break;
                case 'LA':
                case 'AG':
                    $('.employment').show();
                    break;
            }

            return;
        });

        $('input').iCheck('check');

        if ($('#EmployeeStatus').select2().length > 0) {
            $('#EmployeeStatus').select2('destroy');
        }

        $('#EmployeeStatus').select2({
            placeholder: 'Select an Employee Status',
            ajax: {
                type: "GET",
                url: `../user/emp-status`,
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

        $('#Shift').select2({
            placeholder: 'Select a Shift',
            ajax: {
                type: "GET",
                url: `../user/emp-shift`,
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

        $('#SelectedWeekdays').select2({
            placeholder: 'Select a Week Schedule',
            ajax: {
                type: "GET",
                url: `../user/emp-week-schedule`,
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

        $('#Account').select2({
            placeholder: 'Select an Account',
            ajax: {
                type: "GET",
                /* ?=p1: To set a default values for array of ids */
                url: `../../accounts/assigned?p=1`,
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
    }

    function loadUsersList() {

        var _grid = $("#userGrid"),
            result = {},
            gridOptions = {
                width: "100%",
                height: "auto",
                pageSize: 30,
                sorting: true,
                paging: true,
                fields: [
                    {
                        name: "FullName", type: "text", width: "13%", title: "Name",
                        sorting: true,
                        sorter: function (val1, val2) {
                            return val1.localeCompare(val2);
                        },
                        cellRenderer: function (value, item) {
                            var lockIcon = item.IsLocked ? '<span class="text-yellow" title="User account is locked!">&nbsp;<i class="glyphicon glyphicon-warning-sign"></i></span>' : '';
                            var onlineIcon = item.IsOnline ? '<span class="text-green" title="User is online.">&nbsp;<i class="fa fa-circle text-gree"></i></span>' : '';
                            var td = $('<td></td>');

                            td.append(`<a href="/Profile/Index/${item.NetUserId}" value = "${item.UserDetailsId}" class="namelink" >${value}</a>${lockIcon} ${onlineIcon}`);

                            return td;
                        }
                    },
                    { name: "RoleName", type: "text", width: "7%", title: "Role" },
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
                    { name: "Email", type: "text", width: "13%", title: "Email" },
                    {
                        name: "control", type: "text", width: "6%", align: 'center', title: "Actions", visible: UserInfo.RoleCode == UserRoleCode.SystemAdmin,
                        cellRenderer: function (value, item) {
                            if (item.IsTotal) {
                                return $('<td></td>');
                            }

                            var editBtn = `<a href="#" value="${item.UserDetailsId}" data-role="${item.RoleCode}" class="button edit" title="Edit"><i class="glyphicon glyphicon-pencil"></i></a>`,
                                deleteBtn = `<a href="#" value="${item.UserDetailsId}" data-name="${item.FullName}" class="button delete" title="Delete"><i class="glyphicon glyphicon-trash"></i></a>`,
                                lockBtn = `<a href="#" value="${item.UserDetailsId}" data-name="${item.FullName}" data-islocked="${item.IsLocked ? true : false}" class="button lock" title="Lock"><i class="glyphicon glyphicon-lock"></i></a>`,
                                resetPWBtn = `<a href="#" value="${item.UserDetailsId}" data-email="${item.Email}" class="button resetPW" title="Reset Password"><i class="fa fa-key"></i></a>`,
                                permissionBtn = `<a href="#" value="${item.UserDetailsId}" data-netuserid="${item.NetUserId}" 
                                              data-uname="${item.FullName}" data-urole="${item.RoleName}" class="button permission" title="Manage Permission"><i class="glyphicon glyphicon-wrench"></i></a>`,
                                td = $('<td></td>');
                            
                            var controls = `${userModulePermission.canEdit ? lockBtn : ''} ${userModulePermission.canEdit ? editBtn : ''} ${userModulePermission.canDelete ? deleteBtn : ''} ${userModulePermission.canEdit ? resetPWBtn : ''} ${userModulePermission.canEdit ? permissionBtn : ''}`;
                            var dataControl = item.UserDetailsId == UserInfo.UserDetailsId || item.RoleCode == 'SA' ? '' : controls;

                            td.append(dataControl);

                            return td;
                        }
                    }
                ]
            };

        var accountIdArray = $('#selAccount').select2('data')
            , teamIdArray = $('#selTeam').select2('data')
            , roleArray = $('#selRole').select2('data')
            , userIdArray = $('#selUsers').select2('data');

        var accountIds = $.map(accountIdArray, function (val, index) {
            return `accountIds[${index}]=${val.id}`;
        }),
        teamIds = $.map(teamIdArray, function (val, index) {
            return `teamIds[${index}]=${val.id}`;
        }),
        roles = $.map(roleArray, function (val, index) {
            return `roles[${index}]=${val.id}`;
        }),
        userIds = $.map(userIdArray, function (val, index) {
            return `userIds[${index}]=${val.id}`;
        });

        var urlParams = 'q=1';
        urlParams = urlParams + (accountIds.length > 0 ? `&${accountIds.join('&')}` : '');
        urlParams = urlParams + (teamIds.length > 0 ? `&${teamIds.join('&')}` : '');
        urlParams = urlParams + (roles.length > 0 ? `&${roles.join('&')}` : '');
        urlParams = urlParams + (userIds.length > 0 ? `&${userIds.join('&')}` : '');

        $.ajax({
            type: 'GET',
            url: `../user/list?${urlParams}`,
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
                    data: result.data.UserList,
                    onRefreshed: function (args) {
                        var items = args.grid.option("data");
                        var total = {
                            'FullName': '',
                            'AccountName': '',
                            'TeamName': '',
                            IsTotal: true
                        };

                        items.forEach(function (item) {
                            total.TaskActiveTime += item.TaskActiveTime;
                        });

                        total.FullName = `${result.data.UserCount} Users`;
                        total.AccountName = `${result.data.AccountCount} Accounts`;
                        total.TeamName = `${result.data.TeamCount} Teams`;

                        var $totalRow = $("<tr>").addClass('total-row');

                        args.grid._renderCells($totalRow, total);

                        args.grid._content.append($totalRow);
                    }
                });

                $(_grid).jsGrid("reset");
                $(_grid).jsGrid(gridOptions);
            }
        });
    }

    function initPageUserAccess() {

        if (userModulePermission.canAdd) {
            $('.add').show();
        }
        else {
            $('.add').hide();
        }
        
        switch (UserInfo.RoleCode) {
            case UserRoleCode.Agent:
                $('.roles').hide();
                $('.accounts, .teams .refresh').show();
                break;
            case UserRoleCode.AccountManager:
                $('.roles').hide();
                $('.accounts, .teams .refresh').show();
                break;
            case UserRoleCode.SystemAdmin:
                $('.roles, .add, .accounts, .teams .refresh').show()
                break;
            case UserRoleCode.TeamSupervisor:
                $('.roles').hide();
                $('.accounts, .teams .refresh').show()
                break;
        }
    }

    UpdateUserInfo(function (data) {        
        userModulePermission = data.Permissions.find(function (item) {
            return item.ModuleCode == "m-users";
        });

        initPageUserAccess();

        initFilters();
        initHandlers();
        loadUsersList();
    });
});