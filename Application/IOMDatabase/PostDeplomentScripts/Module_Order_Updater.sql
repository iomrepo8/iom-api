/**
  Module names
 */
UPDATE dbo.AspNetModules
    SET [ModuleCode] = CASE
        WHEN [Name] = 'Dashboard' THEN 'm-dashboard'
        WHEN [Name] = 'Notifications' THEN 'm-notifications'
        WHEN [Name] = 'Accounts' THEN 'm-accounts'
        WHEN [Name] = 'Teams' THEN 'm-teams'
        WHEN [Name] = 'Users' THEN 'm-users'
        WHEN [Name] = 'Task Dashboard' THEN 'm-taskdashboard'
        WHEN [Name] = 'Timekeeping' THEN 'm-timekeeps'
        WHEN [Name] = 'Support' THEN 'm-support'
        WHEN [Name] = 'Settings' THEN 'm-settings'
        WHEN [Name] = 'Tagging' THEN 'm-tagging'
        WHEN [Name] = 'IPWhitelist' THEN 's-ipwhitelist'
        WHEN [Name] = 'Email Notification Settings' THEN 's-emailnotification'
        WHEN [Name] = 'Seats' THEN 'm-seat'
        
        WHEN [Name] = 'User Permissions' THEN 's-permissions'
        WHEN [Name] = 'System Logs' THEN 's-syslogs'

        WHEN [Name] = 'EOD Edits' THEN 's-eodedits'
        WHEN [Name] = 'Management' THEN 's-management'
        WHEN [Name] = 'Report' THEN 's-report'
        WHEN [Name] = 'Attendance' THEN 's-attendance'
        END,
        
    [Order] = CASE
        WHEN ModuleCode = 'm-dashboard' THEN 1
        WHEN ModuleCode = 'm-notifications' THEN 2
        WHEN ModuleCode = 'm-seat' THEN 3
        WHEN ModuleCode = 'm-accounts' THEN 4
        WHEN ModuleCode = 'm-teams' THEN 5
        WHEN ModuleCode = 'm-users' THEN 6
        WHEN ModuleCode = 'm-taskdashboard' THEN 7
        WHEN ModuleCode = 'm-timekeeps' THEN 8
        WHEN ModuleCode = 'm-support' THEN 9
        WHEN ModuleCode = 'm-settings' THEN 10
        WHEN ModuleCode = 'm-tagging' THEN 11

        WHEN ParentModuleCode = 'm-timekeeps' THEN 8
        WHEN ParentModuleCode = 'm-settings' THEN 10
    END,

    SubModuleOrder = CASE
        WHEN ModuleCode = 's-management' THEN 1
        WHEN ModuleCode = 's-report' THEN 2
        WHEN ModuleCode = 's-attendance' THEN 3
        WHEN ModuleCode = 's-eodedits' THEN 4
        
        WHEN ModuleCode = 's-ipwhitelist' THEN 1
        WHEN ModuleCode = 's-syslogs' THEN 2
        WHEN ModuleCode = 's-permissions' THEN 3
        WHEN ModuleCode = 's-emailnotification' THEN 4
    END,
    
    ParentModuleCode = CASE
        WHEN ModuleCode = 's-ipwhitelist' THEN 'm-settings'
        WHEN ModuleCode = 's-syslogs' THEN 'm-settings'
        WHEN ModuleCode = 's-permissions' THEN 'm-settings'
        WHEN ModuleCode = 's-emailnotification' THEN 'm-settings'
        
        WHEN ModuleCode = 's-management' THEN 'm-timekeeps'
        WHEN ModuleCode = 's-report' THEN 'm-timekeeps'
        WHEN ModuleCode = 's-attendance' THEN 'm-timekeeps'
        WHEN ModuleCode = 's-eodedits' THEN 'm-timekeeps'
    END
GO