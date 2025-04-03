$(document).ready(function () {
    "use strict";

    $.blockUI();
    var accountId = $.fn.GetUrlParams('AccountId'),
        modulePermission = {},
        userModulePermission = {};

    function initHandlers() {
        $('.refresh').off().on('click', function () {
            loadAccountDetails();
        });

        $("#agentGrid, #supervisorGrid, #accountMgrGrid, #clientPOCGrid, #teamGrid").off().on('click', 'a', function () {
            var $a = $(this),
                id = $(this).attr("value"),
                isAgent = $a.hasClass('agent'),
                isSupervisor = $a.hasClass('supervisor'),
                isTeam = $a.hasClass('team'),
                agentName = $(this).attr("data-name");

            if ($a.is(".delete")) {

                if (isAgent || isSupervisor) {
                    $.get("../Content/Html/TeamsManagerModals.html", function (html) {
                        var deleteAgent = $(html).find('#delete'), result = {},
                            url = isAgent ? `../../teams/remove_agent` : `../../teams/remove_supervisor`,
                            teamId = $a.attr("data-teamid");;

                        $('.modal-content.sm').find('div').remove();
                        $('.modal-content.sm').append(deleteAgent);

                        $('.modal-title').text(isAgent ? 'Confirm Remove Agent' : 'Confirm Remove Supervisor');

                        $('#deleteMsg').html(`${isAgent ? 'Agent' : 'Supervisor'} <b>${agentName}</b> will be removed from his team.`)

                        $(".modal.sm").modal();

                        $('.modal.sm').off('shown.bs.modal').on('shown.bs.modal', function () {
                            $('#deleteBtn').off().on('click', function () {
                                $('.modal.sm').block();
                                $.ajax({
                                    type: "POST",
                                    url: url,
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    data: JSON.stringify({
                                        TeamId: teamId,
                                        UserId: id,
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
                                        loadAccountDetails();
                                    }
                                });
                            });
                        });
                    });
                }
                else if (isTeam) {
                    var teamName = $a.data('teamname');

                    $.get("../content/Html/TeamsModals.html", function (html) {
                        var deleteTeam = $(html).find('#deleteTeam'), result = {};

                        $('.modal-content.sm').find('div').remove();
                        $('.modal-content.sm').append(deleteTeam);

                        $('#teamName').text(teamName);

                        $.ajax({
                            type: "GET",
                            url: `../../teams/is_deletable?teamId=${id}`,
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

                                $(".modal.sm").modal();

                                if (result.data.IsDeletable == true) {
                                    $('#deleteMsg').show();
                                    $('#deleteBtn').show();
                                    $('#forbidDelete').hide();
                                }
                                else {
                                    $('#forbidDelete').show();
                                    $('#deleteBtn').hide();
                                }

                                $('.modal.sm').off('shown.bs.modal').on('shown.bs.modal', function () {
                                    $('#deleteBtn').off().on('click', function () {
                                        $('.modal.sm').block();
                                        $.ajax({
                                            type: "POST",
                                            url: `../../teams/delete`,
                                            contentType: "application/json; charset=utf-8",
                                            dataType: "json",
                                            data: id,
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

                                                loadAccountDetails();
                                            }
                                        });
                                    });
                                });
                            }
                        });
                    });
                }
            }

            if ($a.is('.edit')) {
                showTeamModal(id);
            }

            if ($a.is('.namelink')) {
                var userid = $a.attr('data-netuserid');
                gotoProfile(userid);
            }

            if ($a.is(".teamlink")) {
                var teamId = $a.attr('data-teamid');

                window.location = `../../Team/Manager?teamId=${teamId}`;
            }
        });

        $('#agentAdd').off().on('click', function () {
            $.get("../Content/Html/AccountsManagerModals.html", function (html) {
                var addAgent = $(html).find('#addAgent'), result = {};

                $('.modal-content.md').find('div').remove();
                $('.modal-content.md').append(addAgent);

                $('.modal-title').text('Add Agent');

                $('#selTeam').select2({
                    placeholder: 'Select a Team',
                    ajax: {
                        type: "GET",
                        url: `../teams/assigned?p=1`,
                        data: function (params) {
                            return {
                                q: params.term, // search term
                                page: params.page,
                                accountIds: accountId
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

                $('#Agent').select2({
                    placeholder: 'Select an Agent',
                    ajax: {
                        type: "GET",
                        url: `../teams/agents`,
                        data: function (params) {
                            return {
                                q: params.term, // search term
                                page: params.page,
                                teamId: $('#selTeam').val()
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

                $('#selTeam').change(function () {
                    $('#Agent').removeAttr('disabled');
                });

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
                                TeamId: $('#selTeam').val(),
                                UserId: $('#Agent').val(),
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
                                loadAccountDetails();
                            }
                        });
                    });
                });
            });
        });

        $('#supAdd').click(function () {
            $.get("../Content/Html/AccountsManagerModals.html", function (html) {
                var addAgent = $(html).find('#addAgent'), result = {},
                    teamId = null;

                $('.modal-content.md').find('div').remove();
                $('.modal-content.md').append(addAgent);

                $('.modal-title').text('Add Supervisor');
                $('#fieldLabel').text('Supervisor');

                $('#selTeam').select2({
                    placeholder: 'Select a Team',
                    ajax: {
                        type: "GET",
                        url: `../teams/assigned?p=1`,
                        data: function (params) {
                            return {
                                q: params.term, // search term
                                page: params.page,
                                accountIds: accountId
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

                $('#Agent').select2({
                    ajax: {
                        type: "GET",
                        url: `../teams/supervisors`,
                        data: function (params) {
                            return {
                                q: params.term, // search term
                                page: params.page,
                                teamId: teamId
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

                $('#selTeam').change(function () {
                    $('#Agent').removeAttr('disabled');
                    teamId = $(this).val();
                });

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
                                UserId: $('#Agent').val(),
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
                                loadAccountDetails();
                            }
                        });
                    });
                });
            });
        });

        $('#amAdd').click(function () {
            $.get("../Content/Html/AccountsManagerModals.html", function (html) {
                var addAccountManager = $(html).find('#addAccountManager'), result = {};

                $('.modal-content.md').find('div').remove();
                $('.modal-content.md').append(addAccountManager);

                $('#AccountManager').select2({
                    ajax: {
                        type: "GET",
                        url: `../../accounts/managers`,
                        data: function (params) {
                            return {
                                key: params.term || '',
                                accountId: accountId
                            };
                        },
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        processResults: function (data, index) {
                            return {
                                results: $.map(data.data, function (val) {
                                    return { id: val.Description, text: val.Text };
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
                            url: `../../accounts/add_manager`,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            data: JSON.stringify({
                                AccountId: accountId,
                                UserId: $('#AccountManager').val(),
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
                                loadAccountDetails();
                            }
                        });
                    });
                });
            });
        });

        $('#cpAdd').click(function () {
            $.get("../Content/Html/AccountsManagerModals.html", function (html) {
                var addClientPOC = $(html).find('#addClientPOC'), result = {};

                $('.modal-content.md').find('div').remove();
                $('.modal-content.md').append(addClientPOC);
                
                $('#ClientPOC').select2({
                    ajax: {
                        type: "GET",
                        url: `../../accounts/client_pocs`,
                        data: function (params) {
                            return {
                                key: params.term || '',
                                accountId: accountId
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
                    $('#btnAdd').off().on('click', function () {
                        if (!($('.modal.md').find('.modal-body')).ValidateForm()) {
                            return;
                        }

                        $('.modal.md').block();

                        $.ajax({
                            type: "POST",
                            url: `../../accounts/add_client_poc`,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            data: JSON.stringify({
                                AccountId: accountId,
                                UserDetailsId: $('#ClientPOC').val(),
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
                                loadAccountDetails();
                            }
                        });
                    });
                });
            });
        });

        $('#teamAdd').click(function () {
            showTeamModal();
        });
    }

    function showTeamModal(teamId) {
        $.get("../content/Html/TeamsModals.html", function (html) {
            var editTeam = $(html).find('#editTeam'), result = {};

            $('.modal-content.md').find('div').remove();
            $('.modal-content.md').append(editTeam);

            var title = teamId ? 'Update Team' : 'Create Team';
            $('.modal-title').text(title);

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

            $('.account').hide();

            if (teamId) {
                $.ajax({
                    type: "GET",
                    url: `../../teams/get?teamId=${teamId}`,
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

                        var team = result.data;

                        $('#teamId').val(team.Id);
                        $('#accountId').val(team.AccountId);

                        $('#TeamName').val(team.Name);
                        $('#Description').val(team.Description);

                        $('#AccountName').append(new
                            Option(`${team.AccountName}`, team.AccountId, true, true)
                        ).trigger('change');

                        $('#Shift').append(new
                            Option(`${team.ShiftName}`, team.ShiftId, true, true)
                        ).trigger('change');

                        $(".modal.md").modal();
                    }
                });
            }
            else {
                $(".modal.md").modal();
            }

            $('.modal.md').off('shown.bs.modal').on('shown.bs.modal', function () {

                result = {};

                $('#btnSave').off().on('click', function () {

                    if (!($('.modal.md').find('.modal-body')).ValidateForm()) {
                        return;
                    }

                    var action = teamId ? 'updated' : 'created';

                    $('.modal.md').block();

                    $.ajax({
                        type: "POST",
                        url: `../../teams/save`,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify({
                            Id: $('#teamId').val() || 0,
                            Name: $('#TeamName').val(),
                            AccountId: accountId,
                            Description: $('#Description').val(),
                            ShiftId: $('#Shift').val()
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
                            loadAccountDetails();
                        }
                    });
                });
            });
        });
    }

    function gotoProfile(options) {
        window.location = `../Profile/Index/${options}`;
    }
    
    function loadAccountDetails() {
        var result = {};
        $.blockUI();
        $.ajax({
            type: 'GET',
            url: `../accounts/details?accountId=${accountId}`,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                result = response
            },
            complete: function () {
                if (!result.isSuccessful) {
                    $.displayResponseMessage(result);
                    return false;
                }

                $('.accountName').text(result.data.Name);
                $('.contactPerson').text(result.data.ContactPerson);
                $('.contactEmail').text(result.data.EmailAddress);
                $('.officeAddress').text(result.data.OfficeAddress);
                $('.website').text(result.data.Website);

                $('#tmAccount').attr('data-acc_id', result.data.AccountId);

                loadTeams(result.data.Teams);
                loadAccMgrs(result.data.AccountManagers);
                loadClientPOCs(result.data.ClientPOCs);
                loadSupervisors(result.data.Supervisors);
                loadAgents(result.data.Agents);
            }
        });
    }
    
    function loadTeams(data) {
        var _grid = $("#teamGrid"),
            gridOptions = {
                width: "100%",
                height: "auto",
                pageSize: 12,
                sorting: true,
                paging: true,
                fields: [
                    { name: "number", type: "number", width: "2%", title: "No.", sorting: true },
                    {
                        name: "Name", type: "text", width: "12%", title: "Name",
                        sorting: true,
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');

                            if (item.Id) {
                                td.append(`<a href="#" class='teamlink' data-teamid=${item.Id}>${value}</a>`);
                            }
                            else {
                                td.append(value);
                            }
                            

                            return td;
                        }
                    },
                    { name: "Description", type: "text", width: "12%", title: "Description" },
                    {
                        name: "control", type: "text", width: "7%", align: 'center', title: "Actions",
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');

                            var editBtn = `<a href="#" value="${item.Id}" data-teamname="${item.Name}" 
                                    class="button edit team"><i class="glyphicon glyphicon-pencil"></i></a>`,
                                deleteBtn = `<a href="#" value="${item.Id}" data-teamname="${item.Name}" 
                                    class="button delete team"><i class="glyphicon glyphicon-trash"></i></a>`;

                            if (modulePermission.canEdit && !item.IsTotal) {
                                td.append(`${editBtn} ${deleteBtn}`);
                            }

                            return td;
                        }
                    }
                ],

                onRefreshed: function (args) {
                    var items = args.grid.option("data");
                    var total = {
                        "Name": 0,
                        IsTotal: true
                    };

                    total.Name = `${items.length} Teams`

                    var $totalRow = $("<tr>").addClass('total-row');
                    args.grid._renderCells($totalRow, total);
                    args.grid._content.append($totalRow);
                },

                onRefreshing: function (args) {
                    $.each(args.grid.data, function (index, data) {
                        data.number = index + 1;
                    });
                }
            };

        $.extend(gridOptions, {
            data: data
        });

        $(_grid).jsGrid("reset");
        $(_grid).jsGrid(gridOptions);
    }

    function loadAccMgrs(data) {
        var _grid = $("#accountMgrGrid"),
            gridOptions = {
                width: "100%",
                height: "auto",
                pageSize: 12,
                sorting: true,
                paging: true,
                fields: [
                    { name: "number", type: "number", width: "2%", title: "No.", sorting: true },
                    {
                        name: "FullName", type: "text", width: "12%", title: "Name",
                        sorting: true,
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');

                            if (userModulePermission.canView && item.NetUserId) {
                                td.append(`<a href="#" class='${userModulePermission.canView ? 'namelink' : ''}' data-netuserid='${item.NetUserId}'>${value}</a>`);
                            }
                            else {
                                td.append(value);
                            }

                            return td;
                        }
                    },
                    { name: "Email", type: "text", width: "10%", title: "Email", sorting: true }
                ],

                onRefreshed: function (args) {
                    var items = args.grid.option("data");
                    var total = {
                        "FullName": 0,
                        IsTotal: true
                    };

                    total.FullName = `${items.length} Managers`

                    var $totalRow = $("<tr>").addClass('total-row');
                    args.grid._renderCells($totalRow, total);
                    args.grid._content.append($totalRow);
                },

                onRefreshing: function (args) {
                    $.each(args.grid.data, function (index, data) {
                        data.number = index + 1;
                    });
                }
            };

        $.extend(gridOptions, {
            data: data
        });

        $(_grid).jsGrid("reset");
        $(_grid).jsGrid(gridOptions);
    }

    function loadClientPOCs(data) {
        var _grid = $("#clientPOCGrid"),
            gridOptions = {
                width: "100%",
                height: "auto",
                pageSize: 12,
                sorting: true,
                paging: true,
                fields: [
                    { name: "number", type: "number", width: "2%", title: "No.", sorting: true },
                    {
                        name: "FullName", type: "text", width: "12%", title: "Name",
                        sorting: true,
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');

                            if (userModulePermission.canView  && item.NetUserId) {
                                td.append(`<a href="#" class='${userModulePermission.canView ? 'namelink' : ''}' data-netuserid='${item.NetUserId}'>${value}</a>`);
                            }
                            else {
                                td.append(value);
                            }

                            return td;
                        }
                    },
                    { name: "Email", type: "text", width: "10%", title: "Email", sorting: true }
                ],

                onRefreshed: function (args) {
                    var items = args.grid.option("data");
                    var total = {
                        "FullName": 0,
                        IsTotal: true
                    };

                    total.FullName = `${items.length} POCs`

                    var $totalRow = $("<tr>").addClass('total-row');
                    args.grid._renderCells($totalRow, total);
                    args.grid._content.append($totalRow);
                },

                onRefreshing: function (args) {
                    $.each(args.grid.data, function (index, data) {
                        data.number = index + 1;
                    });
                }
            };

        $.extend(gridOptions, {
            data: data
        });

        $(_grid).jsGrid("reset");
        $(_grid).jsGrid(gridOptions);
    }

    function loadSupervisors(data) {
        var _grid = $("#supervisorGrid"),
            gridOptions = {
                width: "100%",
                height: "auto",
                pageSize: 12,
                sorting: true,
                paging: true,
                fields: [
                    { name: "number", type: "number", width: "2%", title: "No.", sorting: true },
                    {
                        name: "FullName", type: "text", width: "12%", title: "Name",
                        sorting: true,
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');

                            if (userModulePermission.canView && item.NetUserId) {
                                td.append(`<a href="#" class='${userModulePermission.canView ? 'namelink' : ''}' data-netuserid='${item.NetUserId}'>${value}</a>`);
                            } else {
                                td.append(value);
                            }

                            return td;
                        }

                    },
                    {
                        name: "TeamName", type: "text", width: "12%", title: "Team",
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
                    { name: "Email", type: "text", width: "10%", title: "Email", sorting: true },
                    {
                        name: "control", type: "text", width: "7%", align: 'center', title: "Actions",
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');
                            var deleteBtn = `<a href="#" value="${item.NetUserId}" data-name="${item.FullName}" data-teamid="${item.TeamId}"
                                class="button delete supervisor"><i class="glyphicon glyphicon-trash"></i></a>`;

                            if (modulePermission.canEdit && !item.IsTotal) {
                                td.append(deleteBtn);
                            }

                            return td;
                        }
                    }
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
                        data.number = index + 1;
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
                pageSize: 12,
                sorting: true,
                paging: true,
                fields: [
                    { name: "number", type: "number", width: "2%", title: "No.", sorting: true },
                    {
                        name: "FullName", type: "text", width: "12%", title: "Name",
                        sorting: true,
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');

                            if (userModulePermission.canView && item.NetUserId) {
                                td.append(`<a href="#" class='${userModulePermission.canView ? 'namelink' : ''}' data-netuserid='${item.NetUserId}'>${value}</a>`);
                            }
                            else {
                                td.append(value);
                            }

                            return td;
                        }
                    },
                    {
                        name: "TeamName", type: "text", width: "12%", title: "Team",
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
                    { name: "Email", type: "text", width: "10%", title: "Email", sorting: true },
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
                        data.number = index + 1;

                        var deleteBtn = `<a href="#" value="${data.NetUserId}" data-name="${data.FullName}" data-teamid="${data.TeamId}"
                                    class="button delete agent"><i class="glyphicon glyphicon-trash"></i></a>`;

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
            return item.ModuleCode == "m-accounts";
        });

        userModulePermission = data.Permissions.find(function (item) {
            return item.ModuleCode == "m-users";
        });

        initPageUserAccess();

        initHandlers();
        loadAccountDetails();
    });
});