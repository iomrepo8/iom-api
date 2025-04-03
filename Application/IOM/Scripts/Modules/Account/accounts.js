$(document).ready(function () {
    "use strict";

    $.blockUI();
    var sqldbDateFormat = 'YYYY-MM-DD', accountModulePermission = {};

    $('#daterange-btn').daterangepicker(
        daterangepickerOptions,
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

            loadAccountsList(start, end);
        }
    );

    function initHandlers() {
        $("#accountsGrid").off().on('click', 'a', function () {
            var $a = $(this),
                accountId = $(this).attr("value"),
                accountName = $(this).attr("data-accname");

            if ($a.is(".edit")) {
                showAccountModal(accountId);
            }
            else if ($a.is(".delete")) {
                $.get("../content/Html/AccountsModals.html", function (html) {
                    var deleteAccount = $(html).find('#deleteAccount'), result = {};

                    $('.modal-content.sm').find('div').remove();
                    $('.modal-content.sm').append(deleteAccount);

                    var title = 'Delete Account';
                    $('.modal-title').text(title);
                    $('#accountName').text(accountName);

                    $.ajax({
                        type: "GET",
                        url: `../../accounts/is_deletable?accountId=${accountId}`,
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
                                        url: `../../accounts/delete`,
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        data: accountId,
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
                                            loadAccountsList();
                                        }
                                    });
                                });
                            });
                        }
                    });
                });
            }
        });

        $('#addAccount').off().on('click', function () {
            showAccountModal();
        });
    }

    function showAccountModal(accountId) {
        $.get("../content/Html/AccountsModals.html", function (html) {
            var editAccount = $(html).find('#editAccount'), result = {};

            $('.modal-content.md').find('div').remove();
            $('.modal-content.md').append(editAccount);

            var title = accountId ? 'Update Account' : 'Add Account';

            $('.modal-title').text(title);

            if ($('#AccountManager').select2().length > 0) {
                $('#AccountManager').select2('destroy');
            }

            $('#AccountManager').select2({
                ajax: {
                    type: "GET",
                    url: `../../accounts/managers`,
                    data: function (params) {
                        return {
                            key: params.term || ''
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

            $('#ClientPOC').select2({
                ajax: {
                    type: "GET",
                    url: `../../accounts/client_pocs`,
                    data: function (params) {
                        return {
                            key: params.term || ''
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
                $.ajax({
                    type: "GET",
                    url: `../../accounts/get?accountId=${accountId}`,
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

                        var account = result.data;

                        $('#accountId').val(account.Id);
                        $('#AccountName').val(account.Name);
                        $('#ContactPerson').val(account.ContactPerson);
                        $('#EmailAddress').val(account.EmailAddress);
                        $('#OfficeAddress').val(account.OfficeAddress);
                        $('#Website').val(account.Website);

                        $.each(account.AccountManagers, function (index, data) {
                            var option = new Option(`${data.FirstName} ${data.LastName}`, data.NetUserId, true, true);
                            $('#AccountManager').append(option).trigger('change');
                        });

                        $.each(account.ClientPOCs, function (index, data) {
                            var option = new Option(`${data.FirstName} ${data.LastName}`, data.UserDetailsId, true, true);
                            $('#ClientPOC').append(option).trigger('change');
                        });

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

                    var invalid = validate({ EmailAddress: $('#EmailAddress').val() }, {
                        EmailAddress: {
                            presence: true,
                            // must be an email (duh)
                            email: true
                        }
                    });

                    if (invalid) {
                        $('#EmailAddress').closest('div.form-group').addClass('has-error has-danger');
                        return;
                    }

                    $('.modal.md').block(); 
                    var accntMgrs = $.map($('#AccountManager').select2('data'), function (val, index) {
                        return { NetUserId: val.id }
                    });
                    var clientPOCs = $.map($('#ClientPOC').select2('data'), function (val, index) {
                        return { UserDetailsId: val.id }
                    });

                    $.ajax({
                        type: "POST",
                        url: `../../accounts/save`,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify({
                            Id: $('#accountId').val() || 0,
                            Name: $('#AccountName').val(),
                            ContactPerson: $('#ContactPerson').val(),
                            EmailAddress: $('#EmailAddress').val(),
                            OfficeAddress: $('#OfficeAddress').val(),
                            Website: $('#Website').val(),
                            AccountManagers: accntMgrs,
                            ClientPOCs: clientPOCs
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
                            loadAccountsList();
                        }
                    });
                });
            });
        });
    }

    function loadAccountsList(startDate, endDate) {

        if (!startDate || !endDate) {
            startDate = $('#daterange-btn').data('daterangepicker').startDate;
            endDate = $('#daterange-btn').data('daterangepicker').endDate;
        }

        var _grid = $("#accountsGrid"),
            result = {},
            gridOptions = {  
                width: "100%",
                height: "auto",
                pageSize: 30,
                sorting: true,
                paging: true,
                fields: [
                    {
                        name: "Name", type: "text", width: "25%", title: "Name",
                        sorting: true,
                        sorter: function (val1, val2) {
                            var data1 = $(val1).text();
                            var data2 = $(val2).text();
                            return data1.localeCompare(data2);
                        },
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');

                            td.append(`<a href="/Account/Manager?AccountId=${item.Id}" value = "${item.Id}" class="namelink" >${value}</a>`);

                            return td;
                        }
                    },
                    { name: "AgentCount", type: "number", width: "7%", title: "Agents" },
                    { name: "TeamCount", type: "number", width: "7%", title: "Teams" },
                    { name: "HoursActive", type: "number", width: "7%", title: "Hours" },
                    { name: "control", type: "text", width: "5%", align: 'center', title: "Actions" }
                ],

                onRefreshed: function (args) {
                    var items = args.grid.option("data");
                    var total = {
                        "Name": '',
                        "TeamCount": 0,
                        "HoursActive": 0,
                        "AgentCount": 0,
                        IsTotal: true
                    };

                    items.forEach(function (item) {
                        total.AgentCount += item.AgentCount;
                        total.TeamCount += item.TeamCount;
                        total.HoursActive += item.HoursActive;
                    });

                    total.Name = `${items.length} Accounts`
                    total.AgentCount = `${total.AgentCount} Agents`
                    total.TeamCount = `${total.TeamCount} Teams`
                    total.HoursActive = `${total.HoursActive.toFixed(2)} Hours`


                    var $totalRow = $("<tr>").addClass('total-row');

                    args.grid._renderCells($totalRow, total);

                    args.grid._content.append($totalRow);

                    
                },

                onRefreshing: function (args) {
                    $.each(args.grid.data, function (index, data) {

                        var editBtn = `<a href="#" value="${data.Id}" data-accname="${data.Name}" class="button edit"><i class="glyphicon glyphicon-pencil"></i></a>`,
                            deleteBtn = `<a href="#" value="${data.Id}" data-accname="${data.Name}" class="button delete"><i class="glyphicon glyphicon-trash"></i></a>`;

                        if (!data.control) {

                            data.control = `${accountModulePermission.canEdit ? `${editBtn}` : ''} ${accountModulePermission.canDelete ? `${deleteBtn}` : ''}`
                        }
                    });
                }
            },
            /* Set start date value */
            startDate = startDate.format(sqldbDateFormat),
            /* Set end date value */
            endDate = endDate.format(sqldbDateFormat);

        var urlParams = `startDate=${startDate}&endDate=${endDate}`;

        $.ajax({
            type: 'GET',
            url: `../accounts/list?${urlParams}`,
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
        if (accountModulePermission.canAdd) {
            $('#addBtnContainer').show();
        }
        else {
            $('#addBtnContainer').hide();
        }
    }

    UpdateUserInfo(function (data) {
        accountModulePermission = data.Permissions.find(function (item) {
            return item.ModuleCode == "m-accounts";
        });

        initPageUserAccess();
        initHandlers();
        loadAccountsList();
    });
});