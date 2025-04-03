$(document).ready(function () {
    "use strict";

    var userId = $('#userId').val(),
        netUserId = $('#netUserId').val(),
        userDetails = {},
        userModulePermission = {};

    function updateUserDetail(callback) {
        var result = {};

        $.blockUI();
        $.ajax({
            type: "GET",
            url: `../../user/details?userDetailId=${userId}`,
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
                userDetails = user;
                updateUserInformation(user);

                callback && callback();
            }
        });
    }

    function updateUserInformation(user) {

        switch (user.RoleCode) {
            case 'AM':
            case 'CL':
                $('#grpDetails, #grpTasks').hide();
                $('#accTeamLegend').text('Assign Accounts');
                break;
            case 'SA':
                $('#grpDetails, #grpTasks, #grpAccountTeams').hide();
                break;
            default:
                $('#grpDetails, #grpTasks, #grpAccountTeams').show();
                $('#accTeamLegend').text('Account and Teams Assigned');
                break

        }

        $('#userId').val(user.UserDetailsId);
        $('#name').text(user.FullName);
        $('#email').text(user.Email);
        $('#hourlyRate').text(user.HourlyRate);
        $('#employeeStatus').text(user.EmployeeStatus);
        $('#shiftSchedule').text(user.ShiftName);
        $('#userRole').text(user.RoleName);

        $('.day').addClass('selected');

        if (user.WeekSchedule.length > 0) {
            $.each(user.WeekSchedule, function (index, data) {
                $(`.${data.NumericDay}`).removeClass('selected');
            });
        }
        else {
            $('.day').removeClass('selected');
        }

        var hasTask = false;

        $('#accountTeam').html('');
        $('#tasks').html('');

        if (user.Accounts) {
            $.each(user.Accounts, function (index, account) {
                var accountTeamTR = $(`<tr></tr>`);
                accountTeamTR.append($(`<td><b>${account.Name}</b></td>`))

                if (account.Teams.length > 0) {
                    accountTeamTR.append($(`<td>${$.map(account.Teams, function (val, index) {
                        return val.Name
                    }).join(' • ')}</td>`));

                    $.each(account.Teams, function (index, team) {
                        if (team.Tasks) {
                            var addedteam = false;
                            hasTask = true;
                            $.each(team.Tasks, function (index, task) {
                                var taskTR = $(`<tr><td><i><b>${addedteam ? '' : team.Name}</b></i></td > <td>${task.Name}</td> <td>${task.Description}</td></tr >`);
                                addedteam = true;
                                $('#tasks').append(taskTR);
                            })
                        }
                    });
                }

                if (userDetails.RoleCode == 'AM') {
                    accountTeamTR.append('<td>N/A</td>');
                }

                $('#accountTeam').append(accountTeamTR);
            })
        }

        if (hasTask) {
            $('#tasks').show();
            $('#noTask').hide();
        }
        else {
            $('#tasks').hide();
            $('#noTask').show();
        }

        initAccountTeamGridEventHandlers();
        initPageUserAccess();
    }
    
    $('#btnEditName').off().on('click', function () {
        var result = {};

        $.get("../../content/Html/UserProfileModals.html", function (html) {
            var editName = $(html).find('#editName');

            $('.modal-content.md').find('div').remove();
            $('.modal-content.md').append(editName);

            if (userId) {
                $.ajax({
                    type: "GET",
                    url: `../../user/details?userDetailId=${userId}`,
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

                        $('#FirstName').val(user.FirstName);
                        $('#LastName').val(user.LastName);
                        $('#Email').val(user.Email);
                        $('#Role').append(new
                            Option(`${user.RoleName}`, user.RoleCode, true, true)
                        ).trigger('change');

                        $(".modal.md").modal();                        
                    }
                });

                $('.modal.md').off('shown.bs.modal').on('shown.bs.modal', function () {
                    result = {};

                    $('#Role').select2({
                        ajax: {
                            type: "GET",
                            url: `../../role/all`,
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

                    $('#btnSave').off().on('click', function () {
                        if (!($('.modal.md').find('.modal-body')).ValidateForm()) {
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

                        $('.modal.md').block(); 
                        $.ajax({
                            type: "POST",
                            url: `../../user/save_basic_info`,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            data: JSON.stringify({
                                UserDetailsId: userId,
                                FirstName: $('#FirstName').val(),
                                LastName: $('#LastName').val(),
                                Email: $('#Email').val(),
                                RoleCode: $('#Role').val()
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

                                updateUserDetail();
                                $(".modal.md").modal('hide');
                            }
                        });
                    });
                })
            }
        });
    }); 

    $('.changePassBtn').off().on('click', function () {
        var result = {};

        $.get("../../content/Html/UserProfileModals.html", function (html) {
            var changePassword = $(html).find('#changePassword');

            $('.modal-content.md').find('div').remove();
            $('.modal-content.md').append(changePassword);

            if (netUserId == UserInfo.NetUserId) {
                $('.modal-content.md').find('.forOwnProfile').show();
            }
            else {
                $('.modal-content.md').find('.forOwnProfile').hide();
            }

            $(".modal.md").modal();

            $('.modal.md').off('shown.bs.modal').on('shown.bs.modal', function () {
                result = {};

                $('#btnSave').off().on('click', function () {
                    if ($('.modal.md').find('.modal-body').ValidateForm()) {
                        $('.modal.md').block(); 
                        $.ajax({
                            type: "POST",
                            url: `../../user/change_password`,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            data: JSON.stringify({
                                UserId: netUserId,
                                CurrentPassword: $('#currentPassword').val(),
                                NewPassword: $('#newPassword').val(),
                                ConfirmPassword: $('#confirmPassword').val()
                            }),
                            success: function (response) {
                                result = response;
                            },
                            complete: function () {
                                $('.modal.md').unblock(); 

                                if (!result.isSuccessful) {
                                    if (result.message) Notify(result.message, null, null, 'danger');
                                    return false;
                                }
                               
                                $(".modal.md").modal('hide');

                                Notify('Password reset successful.', null, null, 'success');
                            }
                        });
                    }
                    
                });
            });
        });
    });

    $('#btnEditDetails').off().on('click', function () {
        $.get("../../content/Html/UserProfileModals.html", function (html) {
            var editEmpDetails = $(html).find('#editEmpDetails');

            $('.modal-content.md').find('div').remove();
            $('.modal-content.md').append(editEmpDetails);

            $('#EmployeeStatus').select2({
                ajax: {
                    type: "GET",
                    url: `../../user/emp-status`,
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
                ajax: {
                    type: "GET",
                    url: `../../user/emp-shift`,
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
                ajax: {
                    type: "GET",
                    url: `../../user/emp-week-schedule`,
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

            $(".modal.md").modal();

            $('.modal.md').off('shown.bs.modal').on('shown.bs.modal', function () {
                var result = {};

                if (userId) {
                    $.ajax({
                        type: "GET",
                        url: `../../user/details?userDetailId=${userId}`,
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

                            $('#HourlyRate').val(user.HourlyRate);

                            $('#EmployeeStatus').append(new
                                Option(`${user.EmployeeStatus}`, user.EmployeeStatusId, true, true)
                            ).trigger('change');

                            $('#Shift').append(new
                                Option(`${user.ShiftName}`, user.EmployeeShiftId, true, true)
                            ).trigger('change');
                        }
                    });
                }

                $('#btnSave').off().on('click', function () {
                    if ($('.modal.md').find('.modal-body').ValidateForm()) {
                        var weekSched = $.map($('#SelectedWeekdays').select2('data'), function (val, index) {
                            return {
                                NumericDay : val.id
                            };
                        }),
                            data = {
                                UserDetailsId: userId,
                                EmployeeStatusId: $('#EmployeeStatus').val(),
                                HourlyRate: $('#HourlyRate').val(),
                                EmployeeShiftId: $('#Shift').val(),
                                WeekSchedule: weekSched
                            };

                        $('.modal.md').block(); 
                        $.ajax({
                            type: "POST",
                            url: `../../user/save_emp_details`,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            data: JSON.stringify(data),
                            success: function (response) {
                                result = response;
                            },
                            complete: function () {
                                $('.modal.md').unblock(); 

                                if (!result.isSuccessful) {
                                    if (result.message) Notify(result.message, null, null, 'danger');
                                    return false;
                                }
                                var user = result.data;

                                updateUserInformation(user);

                                $(".modal.md").modal('hide');

                                Notify('Employment details successfully updated.', null, null, 'success');
                            }
                        });
                    }
                });
            });
        });
    });

    function initAccountTeamGridEventHandlers() {
        $('#btnEditAccount').off().on('click', function () {
            $.get("../../content/Html/UserProfileModals.html", function (html) {
                var editAccountTeam = $(html).find('#editAccount');

                $('.modal-content.md').find('div').remove();
                $('.modal-content.md').append(editAccountTeam);

                $('#Account').select2({
                    placeholder: 'Select an Account',
                    ajax: {
                        type: "GET",
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

                $(".modal.md").modal();

                $('.modal.md').off('shown.bs.modal').on('shown.bs.modal', function () {
                    var result = {};

                    if (userDetails.Accounts.length > 0) {
                        $('#Account').append(new
                            Option(userDetails.Accounts[0].Name, userDetails.Accounts[0].Id, true, true)
                        ).trigger('change');
                    }

                    $('#btnSave').off().on('click', function () {
                        if ($('.modal.md').find('.modal-body').ValidateForm()) {
                            $('.modal.md').block();
                            $.ajax({
                                type: "POST",
                                url: `../../user/change_account`,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                data: JSON.stringify({
                                    AccountId: $('#Account').val(),
                                    UserDetailsId: userDetails.UserDetailsId
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

                                    updateUserDetail();
                                    $(".modal.md").modal('hide');
                                }
                            });
                        }
                    });
                });
            });
        });

        $('#btnEditTeam').off().on('click', function () {

            if (userDetails.Accounts.length == 0) {
                Notify('Please set first an Account.', null, null, 'danger');
                return;
            }

            $.get("../../content/Html/UserProfileModals.html", function (html) {
                var editAccountTeam = $(html).find('#editTeam');

                $('.modal-content.md').find('div').remove();
                $('.modal-content.md').append(editAccountTeam);

                $('#Teams').select2({
                    placeholder: 'ALL',
                    ajax: {
                        type: "GET",
                        url: `../../teams/assigned?p=1`,
                        data: function (params) {
                            return {
                                q: params.term, // search term
                                page: params.page,
                                accountIds: userDetails.Accounts[0].Id
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

                $(".modal.md").modal();

                $('.modal.md').off('shown.bs.modal').on('shown.bs.modal', function () {
                    var result = {};

                    $.each(userDetails.Accounts[0].Teams, function (index, val) {
                        $('#Teams').append(new
                            Option(val.Name, val.Id, true, true)
                        )
                    });

                    $('#Teams').trigger('change');

                    $('#btnSave').off().on('click', function () {
                        if ($('.modal.md').find('.modal-body').ValidateForm()) {
                            $('.modal.md').block();
                            $.ajax({
                                type: "POST",
                                url: `../../user/change_teams`,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                data: JSON.stringify({
                                    UserDetailsId: userDetails.UserDetailsId,
                                    TeamIds: $.map($('#Teams').select2('data'), function (val, index) {
                                        return val.id
                                    })
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

                                    updateUserDetail();
                                    $(".modal.md").modal('hide');
                                }
                            });
                        }
                    });
                });
            });
        });
    }

    function initPageUserAccess() {
        if (userModulePermission.canEdit) {

            if (userDetails.RoleCode == 'AG' || userDetails.RoleCode == 'LA') {
                $('#btnEditAccount, #btnEditTeam').show();
            }
            else {
                $('#btnEditAccount, #btnEditTeam').hide();
            }

            $('#btnEditDetails, #btnUploadImage, #btnEditName, .changePassBtn').show();
        }
        else {
            $('#btnEditAccount, #btnEditTeam, #btnEditDetails, #btnUploadImage, #btnEditName, .changePassBtn').hide();
        }
    }

    UpdateUserInfo(function (data) {

        userModulePermission = data.Permissions.find(function (item) {
            return item.ModuleCode == "m-users";
        });

        updateUserDetail(initPageUserAccess);
    });
});