// GLOBAL options

var accountModulePermission = {}, rolePermissionModule = {};

var weekdays = {
    1: 'Sunday',
    2: 'Monday',
    3: 'Tuesday',
    4: 'Wednesday',
    5: 'Thursday',
    6: 'Friday',
    7: 'Saturday'
};

var dateformatShort = 'MMDDYY',
    dateformatLong = 'MMMM D, YYYY',
    UserInfo = {},
    UserStatus,
    daterangepickerOptions,
    daterangepickerOptionsL,
    UserRoleCode = {
        Agent: 'AG',
        AccountManager: 'AM',
        SystemAdmin: 'SA',
        TeamSupervisor: 'LA'
    };

var utcDate = new Date();
var estParam = new Date(utcDate.toLocaleString('en-US', { timeZone: 'America/New_York' }));
var momentEST = moment(estParam.toISOString());

var daterangepickerOptions = {
    startDate: momentEST, //default: TODAY
    endDate: momentEST,
    autoApply: true,
    opens: 'center'
};

var daterangepickerOptionsL = {
        startDate: momentEST,
        endDate: momentEST,
        autoApply: true,
        opens: 'left'
},

    singleDatePicker = {
        singleDatePicker: true,
        startDate: momentEST,
        endDate: momentEST,
        autoApply: true,
        opens: 'left'
    };

function AppInfo(callback) {
    callback = callback = typeof callback === 'function' ? callback : function () { };

    var result = {};

    $.ajax({
        type: 'GET',
        url: `../../app/info`,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            result = response
        },
        complete: function () {
            localStorage.app_info = JSON.stringify(result.data);
            callback(result.data);
        }
    });
}

AppInfo();

function UpdateUserInfo(callback) {

    callback = callback = typeof callback === 'function' ? callback : function () { };

    var result = {};

    $.ajax({
        type: 'GET',
        url: `../../user/info`,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            result = response
        },
        complete: function () {
            UserInfo = result.data;

            localStorage.user_info = JSON.stringify(result.data);

            callback(UserInfo);
            setUserStatus();

            $.each(JSON.parse(localStorage.user_info).Permissions, function (index, data) {
                if (data.canView) {
                    $(`.${data.ModuleCode}`).show();
                }
                else {
                    $(`.${data.ModuleCode}`).hide();
                }
            });
            
            $('#noteUl li').remove();

            if (UserInfo.Notifications.length > 0) {
                $('#noteDropdown').attr('data-toggle', 'dropdown');

                $('#noteDropdown').data('toggle', 'dropdown');
                $('#noteCount').text(UserInfo.Notifications.length);
                $('#noteHeader').text(`You have ${UserInfo.Notifications.length} notifications`);

                $.each(UserInfo.Notifications, function (index, val) {
                    var textClass = val.IsRead ? "" : "text-bold";
                    var li = $(`<li class="${textClass}"><a href="#" class="notiflink" data-notifid="${val.Id}" data-msg="${val.Message}" data-title="${val.Title}"><i class="fa ${val.Icon} text-aqua"></i> ${val.Title}</a></li>`);

                    $('#noteUl').append(li);
                });
            } else {
                $('#noteDropdown').removeAttr('data-toggle');
                $('#noteCount').text('');
            }

            $('#MenuActiveTime').text(UserInfo.ActiveTimeOnTask.toFixed(2));
            $('#MenuTaskName').text(`${UserInfo.TaskTypeId == 1 ? UserInfo.TaskName : ''}`);

            initNotificationEventHandlers();
        }
    });
}

function initNotificationEventHandlers() {
    $('.notiflink').click(function () {
        var $a = $(this);

        var noteId = $a.data('notifid'),
            message = $a.data('msg'),
            title = $a.data('title');

        $.ajax({
            type: 'POST',
            url: `../../notification/read?notificationId=${noteId}`,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                result = response
            },
            complete: function () {
                $.get("../content/Html/NotificationModals.html", function (html) {
                    var notificationDetails = $(html).find('#notificationDetails');

                    $('.modal-content.md').find('div').remove();
                    $('.modal-content.md').append(notificationDetails);

                    $('.modal-title').text(title);

                    $('#msg').html(message);

                    $(".modal.md").modal();
                });

                UpdateUserInfo();
            }
        });
    });
}

function changeStatus(status) {
    var userPanel = '.main-sidebar .sidebar .user-panel .info a.status';
    var userpanelHtml;

    switch (status) {
        case 'Active':
            userpanelHtml = '<i class="fa fa-circle text-lime"></i>&nbsp;Active';
            break;
        case 'Break':
            userpanelHtml = '<i class="fa fa-minus-circle text-red"></i>&nbsp;Break';
            break;
        case 'Meeting':
            userpanelHtml = '<i class="fa fa-exclamation-circle text-orange"></i>&nbsp;Meeting';
            break;
        case 'Out':
            userpanelHtml = '<i class="fa fa-circle text-gray"></i>&nbsp;Out';
            break;
        default:
            userpanelHtml = $(userPanel).html();
    }

    $(userPanel).html(userpanelHtml);

    userPanel = '.main-header .navbar .navbar-custom-menu .nav .status-menu a.status';
    $(userPanel).html(userpanelHtml);
}

function setUserStatus() {
    $('#status_icon').html('');

    var htmlContent = '';

    switch (UserInfo.TaskTypeId) {
        case 1:
            htmlContent = '<i class="fa fa-circle text-lime"></i> Active';
            UserStatus = 'Active';

            $('#statusActive_li, #statusMeeting_li, #statusBreak_li, #statusOut_li').removeClass('disabled');
            break;
        case 2:
            htmlContent = '<i class="fa fa-exclamation-circle text-orange"></i> Meeting';
            UserStatus = 'Meeting';

            $('#statusActive_li, #statusBreak_li, #statusOut_li').removeClass('disabled');
            $('#statusMeeting_li').addClass('disabled');
            break;
        case 3:
            htmlContent = '<i class="fa fa-minus-circle text-red"></i> Lunch Break';
            UserStatus = 'Lunch Break';

            $('#statusActive_li, #statusMeeting_li, #statusOut_li').removeClass('disabled');
            $('#statusBreak_li').addClass('disabled');
            break;
        case 4:
        case 5:
            htmlContent = '<i class="fa fa-minus-circle text-red"></i> Break';
            UserStatus = 'Break';

            $('#statusActive_li, #statusMeeting_li, #statusOut_li').removeClass('disabled');
            $('#statusBreak_li').addClass('disabled');
            break;
        case 6:
            htmlContent = '<i class="fa fa-minus-circle text-red"></i> Bio Break';
            UserStatus = 'Bio Break';

            $('#statusActive_li, #statusMeeting_li, #statusOut_li').removeClass('disabled');
            $('#statusBreak_li').addClass('disabled');
            break;
        default:
            htmlContent = `<i class="fa fa-circle text-gray"></i> Out`;
            UserStatus = 'Out';

            $('#statusActive_li, #statusMeeting_li').removeClass('disabled');
            $('#statusBreak_li, #statusOut_li').addClass('disabled');
            break;
    }

    changeStatus(UserStatus);

    $('#status_icon').html(htmlContent);
}

function updateBundle() {
    UpdateUserInfo(function () {
        var permission = JSON.parse(localStorage.user_info).Permissions;

        $.each(permission, function (index, value) {
            if (!value.canView) {
                $(`.${value.ModuleCode}`).hide();
            }
        });

        rolePermissionModule = JSON.parse(localStorage.user_info).Permissions.find(function (item) {
            return item.ModuleCode == "m-permissions";
        });

    });

    setUserStatus();
}

$(document).ready(function () {

    if (getSetting('sidebar-collapse') === 'true') {
        $('body').addClass('sidebar-collapse');
    }

    var bootstrapTooltip = $.fn.tooltip.noConflict();
    $.fn.bstooltip = bootstrapTooltip;

    //workaround for bootstrap autofocus issue on modals
    $('.modal').on('shown.bs.modal', function () {
        $('input[autofocus]').focus();
    });

    $('.sidebar-toggle').on('click', function () {
        var setting = 'sidebar-collapse';

        if ($('body').hasClass(setting)) {
            saveSetting(setting, '');
        } else {
            saveSetting(setting, 'true');
        }
    });

    $('.sidebar .sidebar-menu .treeview-menu li a').on('click', function () {
        //reload current page
        location.reload();
    });

    $('#statusActive').off().on('click', function () {
        $.get("../../content/Html/TimeKeepingModals.html", function (html) {

            var setActiveData = $(html).find('#setActiveDlg'), result = {};

            $('.modal-content.md').find('div').remove();
            $('.modal-content.md').append(setActiveData);

            $('#setActiveDlgTitle').text(UserInfo.IsOut ? 'Confirm Active' : 'Confirm Task');
            $('#btnChangeStatus').text(UserInfo.IsOut ? 'Change Status' : 'Update Task');

            $('#setActiveDlgMsg').html('');

            $('#setActiveDlgMsg').append(UserInfo.TaskTypeId == 1 ?
                'Your status is currently <b>Active</b>, task will be updated instead.' :
                `Status will be changed from <s><b class="text-muted">${UserStatus}</b></s> to <b>Active</b> with the selected task below:`);

            $('#taskList').html('');

            $.ajax({
                type: "GET",
                url: `../../task/assigned?p=1`,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    result = response.data;
                },
                complete: function () {
                    $.each(result, function (i, d) {
                        $('#taskList').append(`<option value="${d.Id}">${d.Text}</option>`);
                    });

                    $('#taskList').trigger('change');
                }
            });

            $('#taskList').off().on('change', function () {
                if ($(this).val()) {
                    $('#btnChangeStatus').removeAttr('disabled');
                }
                else {
                    $('#btnChangeStatus').attr('disabled', true);
                }
            });


            $(".modal.md").modal();

            $('#btnChangeStatus').off().on('click', function () {
                var result = {};

                if (!$('#taskList').val()) {
                    return false;
                }

                $.ajax({
                    type: 'POST',
                    url: `../../task/activate?taskTypeId=1&taskId=${$('#taskList').val()}`,
                    dataType: "json",
                    data: {},
                    contentType: 'application/json',
                    success: function (response) {
                        result = response
                    },
                    complete: function () {
                        if (!result.isSuccessful) {
                            if (result.message) Notify(result.message, null, null, 'danger');
                            return false;
                        }

                        $(".modal.md").modal('hide');

                        Notify('Status successfully changed.', null, null, 'success');
                        UpdateUserInfo();

                        location.reload();
                    }
                });

                return false;
            });

        });
    });

    $('#statusMeeting, #statusOut').off().on('click', function () {
        $('.modal-content.sm').html('');

        var statusType = $(this).prop('id') === 'statusOut' ? { status: 'Out', id: 7 } : { status: 'Meeting', id: 2 };

        $.get("../../content/Html/TimeKeepingModals.html", function (html) {
            var setMeetingData = $(html).find('#setStatusDlg');

            $('.modal-content.sm').find('div').remove();
            $('.modal-content.sm').append(setMeetingData);

            $('#dlgTitle').html(`Confirm ${statusType.status}`);

            $('#prevStatus').text(UserStatus);
            $('#toStatus').text(statusType.status);

            $(".modal.sm").modal();

            $('#setStatusBtn').off().on('click', function () {
                var result = {};

                $.ajax({
                    type: 'POST',
                    url: `../../task/activate?taskTypeId=${statusType.id}&taskId=0`,
                    dataType: "json",
                    data: {},
                    contentType: 'application/json',
                    success: function (response) {
                        result = response
                    },
                    complete: function () {
                        if (!result.isSuccessful) {
                            if (result.message) Notify(result.message, null, null, 'danger');
                            return false;
                        }

                        $(".modal.sm").modal('hide');
                        Notify('Status successfully changed.', null, null, 'success');
                        UpdateUserInfo();
                    }
                });

                return false;
            });
        });
    });

    $('#statusBreak').on('click', function () {
        $('.modal-content.sm').html('');

        $.get("../../content/Html/TimeKeepingModals.html", function (html) {
            var setMeetingData = $(html).find('#setBreakDlg');

            $('.modal-content.sm').find('div').remove();
            $('.modal-content.sm').append(setMeetingData);

            $('#prevBreakStatus').text(UserStatus);

            $(".modal.sm").modal();

            if ($('#breakList').data('select2')) {
                $('#breakList').select2('destroy');
            }

            $('#breakList').off().on('change', function () {
                if ($(this).val()) {
                    $('#setBreakBtn').removeAttr('disabled');
                }
                else {
                    $('#setBreakBtn').attr('disabled', true);
                }
            });

            $('#setBreakBtn').off().on('click', function () {
                var result = {};

                $.ajax({
                    type: 'POST',
                    url: `../../task/activate?taskTypeId=${$("#breakList").val()}&taskId=0`,
                    dataType: "json",
                    data: {},
                    contentType: 'application/json',
                    success: function (response) {
                        result = response
                    },
                    complete: function () {
                        if (!result.isSuccessful) {
                            if (result.message) Notify(result.message, null, null, 'danger');
                            return false;
                        }

                        $(".modal.sm").modal('hide');
                        Notify('Status successfully changed.', null, null, 'success');
                        UpdateUserInfo();
                    }
                });

                return false;
            });
        });
    });

    updateBundle();
});

var confirmDialog = function (opt) {
    var cb = (typeof opt.cb === "function" ? opt.cb : function () { });
    $("#modal-md-confirm").modal();
    $("#modal-md-confirm").find(".modal-dialog").css("left", "-9px");
    $("#confirmMessage").text(opt.msg || "Are you sure you want to proceed?");
    $("#btnConfirmSave").off().on('click', function () {
        cb();
        $("#modal-md-confirm").modal('hide');
    });
    $("#btnConfirmCancel").off().on('click', function () {
        $("#modal-md-confirm").modal('hide');
    });
};