$(document).ready(function () {
    "use strict";

    $.blockUI();
    var sqldbDateFormat = 'YYYY-MM-DD',
        accountId = $.fn.GetUrlParams('Account'),
        accName = $.fn.GetUrlParams('Name'),
        teamModulePermission = {};

    $('#daterange-btn').daterangepicker(
        daterangepickerOptionsL,
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

            loadTeamsList(start, end);
        }
    );

    $('#selAccount').select2({
        placeholder: 'ALL',
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

    if (accountId) {
        $('#selAccount').append(new
            Option(`${accName}`, accountId, true, true)
        ).trigger('change');
    }

    $('#selAccount').change(loadTeamsList);

    function initHandlers() {
        $("#teamGrid").off().on('click', 'a', function () {
            var $a = $(this),
                teamId = $(this).attr("value"),
                teamName = $(this).attr("data-teamname");

            if ($a.is(".edit")) {
                showTeamModal(teamId);
            }
            else if ($a.is(".delete")) {
                $.get("../content/Html/TeamsModals.html", function (html) {
                    var deleteTeam = $(html).find('#deleteTeam'), result = {};

                    $('.modal-content.sm').find('div').remove();
                    $('.modal-content.sm').append(deleteTeam);

                    $('#teamName').text(teamName);

                    $.ajax({
                        type: "GET",
                        url: `../../teams/is_deletable?teamId=${teamId}`,
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
                                        data: teamId,
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

                                            loadTeamsList();
                                        }
                                    });
                                });
                            });
                        }
                    });
                });
            }
            else if ($a.is(".accountlink")) {
                var accountId = $a.attr('data-accid'),
                    accname = $a.text();

                window.location = `../../Account/Manager?AccountId=${accountId}`;
            }
        });

        $('#addTeam').off().on('click', function () {
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

            if (teamId) {
                $('#AccountName').attr('disabled');
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
                $('#AccountName').removeAttr('disabled');

                $('#AccountName').select2({
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
                            AccountId: $('#AccountName').val(),
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
                            loadTeamsList();
                        }
                    });
                });
            });
        });
    }

    function loadTeamsList(startDate, endDate) {

        if (!startDate || !endDate) {
            startDate = $('#daterange-btn').data('daterangepicker').startDate;
            endDate = $('#daterange-btn').data('daterangepicker').endDate;
        }

        var _grid = $("#teamGrid"),
            result = {},
            gridOptions = {
                width: "100%",
                height: "auto",
                pageSize: 30,
                sorting: true,
                paging: true,
                fields: [
                    {
                        name: "Name", type: "text", width: "12%", title: "Team",
                        sorting: true,
                        sorter: function (val1, val2) {
                            var data1 = $(val1).text();
                            var data2 = $(val2).text();
                            return data1.localeCompare(data2);
                        }
                    },
                    {
                        name: "AccountName", type: "text", width: "12%", title: "Account",
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');

                            if (value)
                                td.append(`<a href="#" class='accountlink' data-accid='${item.AccountId}'>${value}</a>`)

                            return td;
                        }
                    },
                    { name: "AgentCount", type: "number", width: "7%", title: "Agents" },
                    { name: "ActiveTime", type: "number", width: "7%", title: "Hours" },
                    { name: "control", type: "text", width: "7%", align: 'center', title: "Actions" }
                ],

                onRefreshed: function (args) {
                    var items = args.grid.option("data");
                    var total = {
                        "Name": '',
                        "ActiveTime": 0,
                        "AgentCount": 0,
                        IsTotal: true
                    };

                    items.forEach(function (item) {
                        total.AgentCount += item.AgentCount;
                        total.ActiveTime += item.ActiveTime;
                    });

                    total.Name = `${items.length} Teams`
                    total.AgentCount = `${total.AgentCount} Agents`
                    total.ActiveTime = `${total.ActiveTime.toFixed(2)} Hours`

                    var $totalRow = $("<tr>").addClass('total-row');

                    args.grid._renderCells($totalRow, total);

                    args.grid._content.append($totalRow);
                },

                onRefreshing: function (args) {
                    $.each(args.grid.data, function (index, data) {

                        var editBtn = `<a href="#" value="${data.Id}" data-teamname="${data.Name}" class="button edit"><i class="glyphicon glyphicon-pencil"></i></a>`,
                            deleteBtn = `<a href="#" value="${data.Id}" data-teamname="${data.Name}" class="button delete"><i class="glyphicon glyphicon-trash"></i></a>`;

                        if (!data.control) {
                            data.control = `${teamModulePermission.canEdit ? editBtn : ''} ${teamModulePermission.canDelete ? deleteBtn : ''}`
                        }

                        if (!data.Name.includes('<a href="')) {
                            data.Name = `<a href="/Team/Manager?teamId=${data.Id}" value="${data.Id}" class="namelink" >${data.Name}</a>`;
                        }
                    });
                }
            },
            /* Set start date value */
            startDate = startDate.format(sqldbDateFormat),
            /* Set end date value */
            endDate = endDate.format(sqldbDateFormat),

            accountIdArray = $('#selAccount').select2('data'),
            accountIds = $.map(accountIdArray, function (val, index) {
                return `accountIds[${index}]=${val.id}`;
            });

        var urlParams = `startDate=${startDate}&endDate=${endDate}`;

        urlParams += (accountIds.length > 0 ? `&${accountIds.join('&')}` : '')

        $.ajax({
            type: 'GET',
            url: `../teams/list?${urlParams}`,
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

                $(_grid).jsGrid("reset");
                $(_grid).jsGrid(gridOptions);
            }
        });
    }

    function initPageUserAccess() {
        if (teamModulePermission.canAdd) {
            $('#addBtnContainer').show();
        }
        else {
            $('#addBtnContainer').hide();
        }
    }
    
    UpdateUserInfo(function (data) {

        teamModulePermission = data.Permissions.find(function (item) {
            return item.ModuleCode == "m-teams";
        });
        
        initPageUserAccess();
        initHandlers();
        loadTeamsList();
    });
});