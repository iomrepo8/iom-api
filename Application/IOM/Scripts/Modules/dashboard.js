$(document).ready(function () {
    "use strict";

    var userPermissions = {};
    
    function loadDashboardWidgetData() {
        var result = {};

        $.blockUI();
        $.ajax({
            type: 'GET',
            url: `../dashboard/data`,
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

                /* Accounts widgets */
                $('#accountCount').text(result.data.AccountCount);
                $('#accountCount').append(`<sup style="font-size: 18px"> ${result.data.AccountCount == 1 ? 'Account' : 'Accounts'}</sup>`);

                /* Teams widgets */
                $('#teamCount').text(result.data.TeamCount);
                $('#teamCount').append(`<sup style="font-size: 18px"> ${result.data.TeamCount == 1 ? 'Team' : 'Teams'}</sup>`);

                /* Users widgets */
                $('#userCount').text(result.data.UserCount);
                $('#userCount').append(`<sup style="font-size: 18px"> ${result.data.UserCount == 1 ? 'User' : 'Users'}</sup>`);

                var pContent = `${result.data.OnlineUserCount == 0 ? 'No' : result.data.OnlineUserCount} ${result.data.OnlineUserCount == 1 ? 'User' : 'Users'} Online`

                $('#userCountDetail').text(`${pContent}`);

                var hours = Math.floor(result.data.HourCount),
                    minutes = (((result.data.HourCount - hours) * 60) | 0);

                /* Hours widgets */
                $('#hourCount').append(`${hours}<sup style="font-size: 18px"> Hr</sup> ${minutes } <sup style="font-size: 18px"> Min</sup>`);
            }
        });
    }

    function initPageAccess(data) {

        if (data.find(function (item) { return item.ModuleCode == 'm-accounts' }).canView) {
            $('#accountWidget').show();
        }
        else {
            $('#accountWidget').hide();
        }

        if (data.find(function (item) { return item.ModuleCode == 'm-teams' }).canView) {
            $('#teamWidget').show();
        }
        else {
            $('#teamWidget').hide();
        }

        if (data.find(function (item) { return item.ModuleCode == 'm-users' }).canView) {
            $('#userWidget').show();
        }
        else {
            $('#userWidget').hide();
        }

        if (data.find(function (item) { return item.ModuleCode == 'm-timekeeps' }).canView) {
            $('#timekeepingWidget').show();
        }
        else {
            $('#timekeepingWidget').hide();
        }
    }

    UpdateUserInfo(function (data) {
        userPermissions = data.Permissions;

        initPageAccess(userPermissions);

        loadDashboardWidgetData();
    });
});