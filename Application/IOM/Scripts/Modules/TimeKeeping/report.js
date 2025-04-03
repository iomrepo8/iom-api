$(document).ready(function () {
    "use strict";

    var sqldbDateFormat = 'YYYY-MM-DD',
        reportItems = {},
        totalActiveTime = 0;


    $(document).ready(function () {
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

                loadGridData(start, end);
            }
        );

        $("#refresh-btn").off().on('click', function () {
            loadGridData();
        });

        initFilters();

        $('#selTask').change(loadGridData);
        $('#selTeam').change(loadGridData);
        $('#selUsers').change(loadGridData);
        $('#selAccount').change(loadGridData);

        loadGridData();
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

        $('#selUsers').trigger('change');
        $('#selTask').trigger('change');
        $('#selAccount').trigger('change');
        $('#selTeam').trigger('change');
    }
    
    function initHandlers() {
        $("#reportGrid").off().on('click', 'a', function () {
            var $a = $(this);

            if ($a.is(".namelink")) {
                var userid = $a.attr('data-netuserid');

                window.location = `../Profile/Index/${userid}`;
            }
        });
    }

    function loadGridData(startDate, endDate) {

        if (!startDate || !endDate) {
            startDate = $('#daterange-btn').data('daterangepicker').startDate;
            endDate = $('#daterange-btn').data('daterangepicker').endDate;
        }

        var _grid = $("#reportGrid"),
            result = {},
            gridOptions = {
                width: "100%",
                height: "auto",
                pageSize: 30,
                sorting: true,
                paging: true,
                fields: [
                    {
                        name: "FullName", type: "text", width: "7%", title: "Name",
                        cellRenderer: function (value, item) {
                            var td = $('<td></td>');

                            if (value)
                                td.append(`<a href="#" class='namelink' data-netuserid='${item.NetUserId}'>${value}</a>`);

                            return td;
                        }
                    },
                    { name: "TaskName", type: "text", width: "7%", title: "Team - Task" },
                    { name: "TaskDescription", type: "text", width: "15%", title: "Task Description" },
                    { name: "TaskActiveTime", type: "text", width: "5%", title: "Active Time" }
                ]
            };

        /* Set dropdown filter array values */
        var taskIdArray = $('#selTask').select2('data')
            , accountIdArray = $('#selAccount').select2('data')
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
            }),
            taskIds = $.map(taskIdArray, function (val, index) {
                return `taskIds[${index}]=${val.id}`;
            });

             /* Set start date value */
             startDate = startDate.format(sqldbDateFormat),
             /* Set end date value */
             endDate = endDate.format(sqldbDateFormat);

        var urlParams = `startDate=${startDate}&endDate=${endDate}`;

        urlParams = urlParams + (taskIds.length > 0 ? `&${taskIds.join('&')}` : '');
        urlParams = urlParams + (accountIds.length > 0 ? `&${accountIds.join('&')}` : '');
        urlParams = urlParams + (teamIds.length > 0 ? `&${teamIds.join('&')}` : '');
        urlParams = urlParams + (userIds.length > 0 ? `&${userIds.join('&')}` : '');

        $.blockUI();
        $.ajax({
            type: 'GET',
            url: `../tkdata/report?${urlParams}`,
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

                reportItems = result.data.Report;
                totalActiveTime = result.data.TotalTime;

                $.extend(gridOptions, {
                    data: reportItems,
                    onRefreshed: function (args) {
                        var items = args.grid.option("data");
                        var total = {
                            "TaskActiveTime": 0,
                            IsTotal: true
                        };

                        items.forEach(function (item) {
                            total.TaskActiveTime += item.TaskActiveTime;
                        });

                        total.TaskActiveTime = `Overall Total: ${total.TaskActiveTime.toFixed(2)}`;

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

    function exportToCsv(filename, rows) {
        var processRow = function (row) {
            return `${processCellValue(row.FullName)},${processCellValue(row.TaskName)},${processCellValue(row.TaskDescription)},${processCellValue(row.TaskActiveTime)}\n`;
        };

        var processCellValue = function (data) {
            var innerValue = data === null ? '' : data.toString();
            if (data instanceof Date) {
                innerValue = data.toLocaleString();
            }

            var result = innerValue.replace(/"/g, '""');
            if (result.search(/("|,|\n)/g) >= 0) {
                result = '"' + result + '"';
            }

            return result;
        }

        var csvFile = `Name,Task Name,Task Description,Active Hours\n`;
        for (var i = 0; i < rows.length; i++) {
            csvFile += processRow(rows[i]);
        }

        csvFile += `,,, Overall Total Hours: ${totalActiveTime}`;

        var blob = new Blob([csvFile], { type: 'text/csv;charset=utf-8;' });
        if (navigator.msSaveBlob) { // IE 10+
            navigator.msSaveBlob(blob, filename);
        } else {
            var link = document.createElement("a");
            if (link.download !== undefined) { // feature detection
                // Browsers that support HTML5 download attribute
                var url = URL.createObjectURL(blob);
                link.setAttribute("href", url);
                link.setAttribute("download", filename);
                link.style.visibility = 'hidden';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }
        }
    }

    function exportToPDF(filename, data) {

        var doc = new jsPDF(),
            totalPagesExp = "{total_pages_count_string}";
        doc.setFontSize(11);
        doc.setFontStyle('bold');

        doc.autoTable({
            head: headRows(),
            body: bodyRows(data),
            columnStyles: {
                fullName: { cellWidth: 20 },
                taskName: { cellWidth: 33 },
                taskDescription: { cellWidth: 40 },
                taskActiveTime: { cellWidth: 7 }
            },
            didDrawPage: function (data) {
                
                var pageDetail = "Page " + doc.internal.getNumberOfPages();
                var overallTotal = `(Overall Total Hours: ${totalActiveTime})`

                // Total page number plugin only available in jspdf v1.0+
                if (typeof doc.putTotalPages === 'function') {
                    pageDetail = pageDetail + " of " + totalPagesExp;
                }
                doc.setFontSize(10);

                // jsPDF 1.4+ uses getWidth, <1.4 uses .width
                var pageSize = doc.internal.pageSize;
                var pageHeight = pageSize.height ? pageSize.height : pageSize.getHeight();

                doc.text(pageDetail, data.settings.margin.right, pageHeight - 7);
                doc.text(overallTotal, data.settings.margin.right, pageHeight - 12)
            },
            startY: 20,
            theme: 'grid'
        });

        if (typeof doc.putTotalPages === 'function') {
            doc.putTotalPages(totalPagesExp);
        }

        doc.save(filename);
    }

    function headRows() {
        return [{ fullName: 'Name', taskName: 'Task Name', taskDescription: 'Task Description', taskActiveTime: 'Active Hours' }];
    }

    function bodyRows(rows) {

        let body = [];

        $.each(rows, function (index, data) {
            body.push({
                fullName: data.FullName,
                taskName: data.TaskName,
                taskDescription: data.TaskDescription,
                taskActiveTime: data.TaskActiveTime.toString()
            });
        });

        return body;
    }

    $('.select2').select2({ multiple: true, placeholder: 'ALL'});
    $('#form0').removeData('validator');
    $('#form0').removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse('#form0');

    $('#exportCsv').off().on('click', function () {

        exportToCsv(`${getExportFilename()}.csv`, reportItems);
    });

    $('#exportPdf').off().on('click', function () {       

        exportToPDF(`${getExportFilename()}.pdf`, reportItems);
    });

    function getExportFilename() {
        var startDate = $('#daterange-btn').data('daterangepicker').startDate,
            endDate = $('#daterange-btn').data('daterangepicker').endDate,
            /* Set start date value */
            startDate = startDate.format(sqldbDateFormat),
            /* Set end date value */
            endDate = endDate.format(sqldbDateFormat);

        return `${startDate}_${endDate}_TKReport`;
    }

    initHandlers();
})