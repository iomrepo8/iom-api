/*
Post-Deployment Script Template                            
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.        
 Use SQLCMD syntax to include a file in the post-deployment script.            
 Example:      :r .\myfile.sql                                
 Use SQLCMD syntax to reference a variable in the post-deployment script.        
 Example:      :setvar TableName MyTable
               SELECT * FROM [$(TableName)]                    
--------------------------------------------------------------------------------------
*/

-- ADD NEW MODULE
IF NOT EXISTS(SELECT * FROM dbo.AspNetModules WHERE ModuleCode = 'm-notifications')
	INSERT INTO dbo.AspNetModules(ModuleCode, [Name], DateCreated, IsActive, [Order])
	VALUES ('m-notifications', 'Notifications', GETUTCDATE(), 1, 10)
GO

-- GENERATE MODULE PERMISSION PER ROLE
;WITH updateData AS (
	SELECT 
		RoleId = r.Id, ModuleId = m.Id, ModuleCode = m.ModuleCode,
		RoleName = r.Name, ModuleName = m.Name,
		r.RoleCode,
		CanView = 1, CanAdd = 1, CanEdit = 1, CanDelete = 1
		, CreatedBy = 'Jonathan', DateCreated = GETDATE()
	FROM AspNetRoles r, AspNetModules m
	WHERE r.RoleCode = 'SA'
	UNION
	SELECT 
		RoleId = r.Id, ModuleId = m.Id, ModuleCode = m.ModuleCode,
		RoleName = r.Name, ModuleName = m.Name, r.RoleCode
		, CanView = CASE WHEN m.ModuleCode = 'm-syslogs' OR m.ModuleCode = 'm-permissions' THEN 0 ELSE 1 END
		, CanAdd = 0
		, CanEdit = CASE WHEN m.ModuleCode = 'm-accounts' OR m.ModuleCode = 'm-teams' THEN 1 ELSE 0 END
		, CanDelete = 0
		, CreatedBy = 'Jonathan', DateCreated = GETDATE()
	FROM AspNetRoles r, AspNetModules m
	WHERE r.RoleCode = 'AM'	
	UNION
	SELECT 
		RoleId = r.Id, ModuleId = m.Id, ModuleCode = m.ModuleCode,
		RoleName = r.Name, ModuleName = m.Name, r.RoleCode
		, CanView = CASE WHEN ModuleCode = 'm-syslogs' OR ModuleCode = 'm-permissions' THEN 0 ELSE 1 END
		, CanAdd = 0
		, CanEdit = CASE WHEN ModuleCode = 'm-teams' THEN 1 ELSE 0 END
		, CanDelete = 0
		, CreatedBy = 'Jonathan', DateCreated = GETDATE()
	FROM AspNetRoles r, AspNetModules m
	WHERE r.RoleCode IN ('CL', 'LA', 'TM')
	UNION
	SELECT 
		RoleId = r.Id, ModuleId = m.Id, ModuleCode = m.ModuleCode,
		RoleName = r.Name, ModuleName = m.Name, r.RoleCode
		, CanView = CASE WHEN ModuleCode = 'm-syslogs' OR ModuleCode = 'm-permissions' THEN 0 ELSE 1 END
		, CanAdd = 0 , CanEdit = 0 , CanDelete = 0
		, CreatedBy = 'Jonathan', DateCreated = GETDATE()
	FROM AspNetRoles r, AspNetModules m
	WHERE r.RoleCode = 'AG'
)

INSERT INTO dbo.RolePermissions(RoleId, ModuleId, IsLocked, ModuleCode, CanView, CanAdd, CanEdit, CanDelete, CreatedBy, DateCreated)
SELECT fd.RoleId, fd.ModuleId, 0, fd.ModuleCode, fd.CanView, fd.CanAdd, fd.CanEdit, fd.CanDelete, CreatedBy = 'Jonathan', DateCreated = GETDATE() 
FROM updateData fd
WHERE NOT EXISTS(SELECT * FROM RolePermissions rp WHERE rp.RoleId = fd.RoleId AND rp.ModuleId = fd.ModuleId)
GO

--GENERATE USER PERMISSIONS
INSERT INTO AspNetUserPermission
SELECT  au.UserId, am.ModuleCode, 1, 1, 1, 1, 'System', GETUTCDATE(), 'System', GETUTCDATE(), 1  
FROM UserDetails au, AspNetModules am
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserPermission _ab WHERE _ab.UserId = au.UserId AND _ab.ModuleCode = am.ModuleCode)
AND au.Role IN ('SA','AM','TM','CL','LA','AG')
GO

UPDATE AspNetModules 
  SET [Order] = 
			CASE 
				WHEN ModuleCode = 'm-dashboard' THEN 1				
				WHEN ModuleCode = 'm-notifications' THEN 2
				WHEN ModuleCode = 'm-accounts' THEN 3
				WHEN ModuleCode = 'm-teams' THEN 4
				WHEN ModuleCode = 'm-users' THEN 5 
				WHEN ModuleCode = 'm-taskdashboard' THEN 6
				WHEN ModuleCode = 'm-timekeeps' THEN 7
				WHEN ModuleCode = 'm-syslogs' THEN 8
				WHEN ModuleCode = 'm-permissions' THEN 9
				WHEN ModuleCode = 'm-support' THEN 10
				WHEN ModuleCode = 'm-tagging' THEN 11

				WHEN ParentModuleCode = 'm-timekeeps' THEN 7
					 END,

	SubModuleOrder = CASE 
				WHEN ModuleCode = 's-management' THEN 1
				WHEN ModuleCode = 's-report' THEN 2
				WHEN ModuleCode = 's-attendance' THEN 3
				WHEN ModuleCode = 's-eodedits' THEN 4
				END
GO

;WITH notiUpdate AS(
	SELECT 
		Id,
		Title,
		NewNoteType = CASE 
			WHEN NoteType = 'AgentStatus' THEN 'ReminderAgentStatus'
			WHEN NoteType = 'AttendanceReminder' THEN 'ReminderAttendance'
			WHEN NoteType IS NULL THEN 'ReminderAgentStatus'
			WHEN [Title] = 'EOD Confirmation' THEN 'EODConfirm'
			WHEN [Title] LIKE 'EOD Edit Request%' THEN 'EODEdit'
			ELSE NoteType
			END
	FROM dbo.[Notification]
)

UPDATE n 
SET n.NoteType = nu.NewNoteType
FROM [Notification] n 
	INNER JOIN notiUpdate nu ON n.Id = nu.Id
GO