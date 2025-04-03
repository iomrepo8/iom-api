$(document).ready(function () {
    "use strict";

    var ModuleAccessSettings = [
        { ModuleCode: 'm-accounts', canView: true, canAdd: true, canEdit: true, canDelete: true },
        { ModuleCode: 'm-dashboard', canView: true, canAdd: false, canEdit: false, canDelete: false },
        { ModuleCode: 'm-support', canView: true, canAdd: false, canEdit: false, canDelete: false },
        { ModuleCode: 'm-teams', canView: true, canAdd: true, canEdit: true, canDelete: true },
        { ModuleCode: 'm-timekeeps', canView: true, canAdd: false, canEdit: true, canDelete: false },
        { ModuleCode: 'm-users', canView: true, canAdd: true, canEdit: true, canDelete: true },
        { ModuleCode: 'm-permissions', canView: true, canAdd: false, canEdit: true, canDelete: false },
        { ModuleCode: 'm-syslogs', canView: true, canAdd: false, canEdit: false, canDelete: false }
    ],
        rolePermissions = null, rolePermissionModule = {};
        

    function collectRolePermissionData(el, rolePermission) {
        $.each($(el).find('input[type=checkbox]'), function (index, values) {
            var access = $(values).attr('data-access'),
                moduleCode = $(values).attr('data-modulecode'),
                chkboxVal = $(values).prop('checked');

            rolePermission.Modules.find(e => e.ModuleCode == moduleCode)[access] = chkboxVal
        });
    }

    function initHandlers() {
        $('.refresh').click(function () {
            $('.save, .cancel, .toggleLock, .toggleAllusers').hide();
            $('.edit').show();

            loadRolePermissionData();
        });

        $('.edit').click(function () {
            var id = $(this).attr('id');

            switch (id) {
                case 'agEdit':
                    $('#agRoleGrid').find('input[type=checkbox]').iCheck('enable');
                    $('#agents').find('.toggleLock, .toggleAllusers, .save, .cancel').show();
                    $(this).hide();
                    break;
                case 'tsEdit':
                    $('#tsRoleGrid').find('input[type=checkbox]').iCheck('enable');
                    $('#teamSupervisor').find('.toggleLock, .toggleAllusers, .save, .cancel').show();
                    $(this).hide();
                    break;
                case 'amEdit':
                    $('#amRoleGrid').find('input[type=checkbox]').iCheck('enable');
                    $('#accountManager').find('.toggleLock, .toggleAllusers, .save, .cancel').show();
                    $(this).hide();
                    break;
                case 'cpEdit':
                    $('#cpRoleGrid').find('input[type=checkbox]').iCheck('enable');
                    $('#clientPoc').find('.toggleLock, .toggleAllusers, .save, .cancel').show();
                    $(this).hide();
                    break;
            }
        });

        $('.cancel').click(function () {
            var id = $(this).attr('id');

            switch (id) {
                case 'agCancel':
                    loadGrid($('#agRoleGrid'), rolePermissions.find(e => e.RoleCode == 'AG').Modules);
                    $('#agRoleGrid').find('input[type=checkbox]').iCheck('disable');
                    $('#agents').find('.toggleLock, .toggleAllusers, .save, .cancel').hide();
                    $('#agEdit').show();
                    break;
                case 'tsCancel':
                    loadGrid($('#tsRoleGrid'), rolePermissions.find(e => e.RoleCode == 'LA').Modules);
                    $('#tsRoleGrid').find('input[type=checkbox]').iCheck('disable');
                    $('#teamSupervisor').find('.toggleLock, .toggleAllusers, .save, .cancel').hide();
                    $('#tsEdit').show();
                    break;
                case 'amCancel':
                    loadGrid($('#amRoleGrid'), rolePermissions.find(e => e.RoleCode == 'AM').Modules);
                    $('#amRoleGrid').find('input[type=checkbox]').iCheck('disable');
                    $('#accountManager').find('.toggleLock, .toggleAllusers, .save, .cancel').hide();
                    $('#amEdit').show();
                    break;
                case 'cpCancel':
                    loadGrid($('#cpRoleGrid'), rolePermissions.find(e => e.RoleCode == 'CL').Modules);
                    $('#cpRoleGrid').find('input[type=checkbox]').iCheck('disable');
                    $('#clientPoc').find('.toggleLock, .toggleAllusers, .save, .cancel').hide();
                    $('#cpEdit').show();
                    break;
            }

            $('input[type=checkbox]').iCheck({
                checkboxClass: 'icheckbox_square-green'
            });
        });

        $('.save').click(function () {
            var id = $(this).attr('id');

            switch (id) {
                case 'agSave':
                    var agRolePermissions = rolePermissions.find(e => e.RoleCode == 'AG');
                    agRolePermissions.IsLocked = $('#agLockIcon').hasClass('fa-lock');
                    agRolePermissions.IsAllUsers = $('#agAllusers').hasClass('fa-toggle-on');
                    collectRolePermissionData('#agRoleGrid', agRolePermissions);

                    saveRolePermission(agRolePermissions, function () {
                        $('#agRoleGrid').find('input[type=checkbox]').iCheck('disable');
                        $('#agents').find('.toggleLock, .toggleAllusers, .save, .cancel').hide();
                        $('#agEdit').show();
                    });
                    
                    break;
                case 'tsSave':
                    var tsRolePermissions = rolePermissions.find(e => e.RoleCode == 'LA');
                    tsRolePermissions.IsLocked = $('#tsLockIcon').hasClass('fa-lock');
                    tsRolePermissions.IsAllUsers = $('#tsAllusers').hasClass('fa-toggle-on');
                    collectRolePermissionData('#tsRoleGrid', tsRolePermissions);

                    saveRolePermission(tsRolePermissions, function () {
                        $('#agRoleGrid').find('input[type=checkbox]').iCheck('disable');
                        $('#teamSupervisor').find('.toggleLock, .toggleAllusers, .save, .cancel').hide();
                        $('#tsEdit').show();
                    });

                    break;
                case 'amSave':
                    var amRolePermissions = rolePermissions.find(e => e.RoleCode == 'AM');
                    amRolePermissions.IsLocked = $('#amLockIcon').hasClass('fa-lock');
                    amRolePermissions.IsAllUsers = $('#amAllusers').hasClass('fa-toggle-on');
                    collectRolePermissionData('#amRoleGrid', amRolePermissions);

                    saveRolePermission(amRolePermissions, function () {
                        $('#amRoleGrid').find('input[type=checkbox]').iCheck('disable');
                        $('#accountManager').find('.toggleLock, .toggleAllusers, .save, .cancel').hide();
                        $('#amEdit').show();
                    });

                    break;
                case 'cpSave':
                    var cpRolePermissions = rolePermissions.find(e => e.RoleCode == 'CL');
                    cpRolePermissions.IsLocked = $('#cpLockIcon').hasClass('fa-lock');
                    cpRolePermissions.IsAllUsers = $('#cpAllusers').hasClass('fa-toggle-on');
                    collectRolePermissionData('#cpRoleGrid', cpRolePermissions);

                    saveRolePermission(cpRolePermissions, function () {
                        $('#cpRoleGrid').find('input[type=checkbox]').iCheck('disable');
                        $('#clientPoc').find('.toggleLock, .toggleAllusers, .save, .cancel').hide();
                        $('#cpEdit').show();
                    });

                    break;
            }
        });
               
        $('.toggleLock').bstooltip({
            html: true,
            title: "<i class='fa fa-toggle-off text-muted'></i> User permissions are editable.<br/><i class='fa fa-toggle-on text-primary'></i> User permissions are locked."
        });

        $('.toggleAllusers').bstooltip({
            html: true,
            title: "<i class='fa fa-toggle-off text-muted'></i> Apply only to new users.<br/><i class='fa fa-toggle-on text-primary'></i> Applied to all users."
        });

        $('.sLock').click(function () {
            var id = $(this).attr('id');

            switch (id) {
                case 'agLock':
                    setLockClass({ iconLock: '#agLockIcon', switchEl: '#agLock', isLocked: $(this).hasClass('fa-toggle-off') });
                    break;
                case 'tsLock':
                    setLockClass({ iconLock: '#tsLockIcon', switchEl: '#tsLock', isLocked: $(this).hasClass('fa-toggle-off') });
                    break;
                case 'amLock':
                    setLockClass({ iconLock: '#amLockIcon', switchEl: '#amLock', isLocked: $(this).hasClass('fa-toggle-off') });
                    break;
                case 'cpLock':
                    setLockClass({ iconLock: '#cpLockIcon', switchEl: '#cpLock', isLocked: $(this).hasClass('fa-toggle-off') });
                    break;
            }
        });

        $('.sAllUser').click(function () {
            var id = $(this).attr('id');

            switch (id) {
                case 'agAllusers':
                    setAllUsersClass({ allUsersIcon: '#agAllUsersIcon', switchAllUserEl: '#agAllusers', isAllUsers: $(this).hasClass('fa-toggle-off') });
                    break;
                case 'tsAllusers':
                    setAllUsersClass({ allUsersIcon: '#tsAllUsersIcon', switchAllUserEl: '#tsAllusers', isAllUsers: $(this).hasClass('fa-toggle-off') });
                    break;
                case 'amAllusers':
                    setAllUsersClass({ allUsersIcon: '#amAllUsersIcon', switchAllUserEl: '#amAllusers', isAllUsers: $(this).hasClass('fa-toggle-off') });
                    break;
                case 'cpAllusers':
                    setAllUsersClass({ allUsersIcon: '#cpAllUsersIcon', switchAllUserEl: '#cpAllusers', isAllUsers: $(this).hasClass('fa-toggle-off') });
                    break;
            }
        });
    }

    function saveRolePermission(rolePermission, callback) {

        $.get("../Content/Html/RolePermissionsModals.html", function (html) {
            var confirmUpdate = $(html).find('#updateWarning'), result = {}, msg = '';

            $('.modal-content.sm').find('div').remove();
            $('.modal-content.sm').append(confirmUpdate);

            msg = rolePermission.IsAllUsers ?
                'This settings will be applied to <b>All Users</b> of this role.'
                : 'This settings will only be applied to <b>New Users</b> of this role.';

            $('#msg').append(msg);

            $(".modal.sm").modal();

            $('.modal.sm').off('shown.bs.modal').on('shown.bs.modal', function () {
                $('#confirm').click(function () {
                    $('.modal.sm').block();

                    $.ajax({
                        type: 'POST',
                        url: `../role/update_permissions`,
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify(rolePermission),
                        success: function (response) {
                            result = response
                        },
                        complete: function () {
                            $('.modal.sm').unblock(); 
                            $(".modal.sm").modal('hide');
                            $.displayResponseMessage(result);

                            if (!result.isSuccessful) {
                                return false;
                            }

                            callback && callback();
                        }
                    });
                });
            });
        });
    }

    function tdCheckboxRenderer(value, item, access) {
        var td = $('<td></td>'), checkbox = $(`<input type='checkbox' data-access='${access}' data-modulecode='${item.ModuleCode}'></input>`);

        var modAccSetting = ModuleAccessSettings.find(e => e.ModuleCode == item.ModuleCode);

        if (modAccSetting && modAccSetting[access]) {
            if (value) {
                checkbox.attr({ checked: true });
            }

            td.append(checkbox);
        }

        return td;
    }

    function loadGrid(grid, data) {
        var gridOptions = {
            width: "100%",
            height: "auto",
            pageSize: 15,
            sorting: false,
            paging: true,
            fields: [
                { name: "ModuleName", type: "text", width: "12%", title: "Module" }
                , { name: "ModuleCode", type: "text", width: "7%", title: "Module Code" }
                , {
                    name: "canView", type: "text", width: "5%", title: "View", align: 'center',
                    cellRenderer: function (value, item) {
                        return tdCheckboxRenderer(value, item, 'canView');
                    }
                }
                , {
                    name: "canAdd", type: "text", width: "5%", title: "Add", align: 'center',
                    cellRenderer: function (value, item) {
                        return tdCheckboxRenderer(value, item, 'canAdd');
                    }
                }
                , {
                    name: "canEdit", type: "text", width: "5%", title: "Edit", align: 'center',
                    cellRenderer: function (value, item) {
                        return tdCheckboxRenderer(value, item, 'canEdit');
                    }
                }
                , {
                    name: "canDelete", type: "text", width: "5%", title: "Delete", align: 'center',
                    cellRenderer: function (value, item) {
                        return tdCheckboxRenderer(value, item, 'canDelete');
                    }
                }
            ]
        };

        $.extend(gridOptions, {
            data: data
        });

        $(grid).jsGrid(gridOptions);
    }

    function setLockClass(role) {
        if (role.isLocked) {
            $(role.iconLock).removeClass('fa-unlock');
            $(role.iconLock).addClass('fa-lock');
            $(role.switchEl).removeClass('fa-toggle-off');
            $(role.switchEl).addClass('fa-toggle-on');
        } else {
            $(role.iconLock).removeClass('fa-lock');
            $(role.iconLock).addClass('fa-unlock');
            $(role.switchEl).removeClass('fa-toggle-on');
            $(role.switchEl).addClass('fa-toggle-off');
        }
    }

    function setAllUsersClass(role) {
        if (role.isAllUsers) {
            $(role.allUsersIcon).removeClass('fa-close');
            $(role.allUsersIcon).addClass('fa-check');
            $(role.switchAllUserEl).removeClass('fa-toggle-off');
            $(role.switchAllUserEl).addClass('fa-toggle-on');
        }
        else {
            $(role.allUsersIcon).removeClass('fa-check');
            $(role.allUsersIcon).addClass('fa-close');
            $(role.switchAllUserEl).removeClass('fa-toggle-on');
            $(role.switchAllUserEl).addClass('fa-toggle-off');
        }
    }

    function loadRolePermissionData() {
        var result = {};

        $.blockUI();

        $.ajax({
            type: 'GET',
            url: `../role/permissions`,
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

                rolePermissions = result.data;

                $.each(result.data, function (index, value) {
                    switch (value.RoleCode) {
                        case 'AG':
                            setLockClass({
                                iconLock: '#agLockIcon',
                                switchEl: '#agLock',
                                isLocked: value.IsLocked
                            });
                            setAllUsersClass({
                                isAllUsers: value.IsAllUsers,
                                allUsersIcon: '#agAllUsersIcon',
                                switchAllUserEl: '#agAllusers'
                            });

                            loadGrid($('#agRoleGrid'), value.Modules);
                            break;
                        case 'LA':
                            setLockClass({
                                iconLock: '#tsLockIcon',
                                switchEl: '#tsLock',
                                isLocked: value.IsLocked
                            });
                            setAllUsersClass({
                                isAllUsers: value.IsAllUsers,
                                allUsersIcon: '#tsAllUsersIcon',
                                switchAllUserEl: '#tsAllusers'
                            });

                            loadGrid($('#tsRoleGrid'), value.Modules);
                            break;
                        case 'AM':
                            setLockClass({
                                iconLock: '#amLockIcon',
                                switchEl: '#amLock',
                                isLocked: value.IsLocked
                            });
                            setAllUsersClass({
                                isAllUsers: value.IsAllUsers,
                                allUsersIcon: '#amAllUsersIcon',
                                switchAllUserEl: '#amAllusers'
                            });

                            loadGrid($('#amRoleGrid'), value.Modules);
                            break;
                        case 'CL':
                            setLockClass({
                                iconLock: '#cpLockIcon',
                                switchEl: '#cpLock',
                                isLocked: value.IsLocked
                            });
                            setAllUsersClass({
                                isAllUsers: value.IsAllUsers,
                                allUsersIcon: '#cpAllUsersIcon',
                                switchAllUserEl: '#cpAllusers'
                            });

                            loadGrid($('#cpRoleGrid'), value.Modules);
                            break;
                        case 'SA':
                            loadGrid($('#saRoleGrid'), value.Modules);
                            break;
                    }
                });

                $('input').iCheck({
                    checkboxClass: 'icheckbox_square-green'
                });

                $('input').iCheck('disable');
            }
        });
    }

    function initPageUserAccess() {
        if (rolePermissionModule.canEdit) {
            $('.btn-toolbar').show();
        }
        else {
            $('.btn-toolbar').hide();
        }
    }

    UpdateUserInfo(function (data) {
        rolePermissionModule = data.Permissions.find(function (item) {
            return item.ModuleCode == "m-permissions";
        });

        loadRolePermissionData();
        initHandlers();
        initPageUserAccess();
    });
});