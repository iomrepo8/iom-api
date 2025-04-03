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

UPDATE AspNetModules 
  SET [Order] = 
			CASE 
				WHEN ModuleCode = 'm-dashboard' THEN 1
				WHEN ModuleCode = 'm-accounts' THEN 2
				WHEN ModuleCode = 'm-teams' THEN 3
				WHEN ModuleCode = 'm-users' THEN 4 
				WHEN ModuleCode = 'm-taskdashboard' THEN 5
				WHEN ModuleCode = 'm-timekeeps' THEN 6
				WHEN ModuleCode = 'm-syslogs' THEN 7
				WHEN ModuleCode = 'm-permissions' THEN 8
				WHEN ModuleCode = 'm-support' THEN 9

				WHEN ParentModuleCode = 'm-timekeeps' THEN 6
					 END,

	SubModuleOrder = CASE 
				WHEN ModuleCode = 's-management' THEN 1
				WHEN ModuleCode = 's-report' THEN 2
				WHEN ModuleCode = 's-attendance' THEN 3
				WHEN ModuleCode = 's-eodedits' THEN 4
				END
GO

-- ADD NEW MODULE
IF NOT EXISTS(SELECT * FROM dbo.AspNetModules WHERE ModuleCode = 'm-settings')
	INSERT INTO dbo.AspNetModules(ModuleCode, [Name], DateCreated, IsActive, [Order])
	VALUES ('m-settings', 'Settings', GETUTCDATE(), 1, 10)
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

--Add default locations
IF NOT EXISTS (SELECT 1 FROM dbo.[Location] WHERE [Name] = 'PH-DV' 
	AND [Description] = 'Davao, Philippines')
BEGIN
	
INSERT INTO dbo.[Location]([Name], [Description], CreateDate, CreatedBy)
VALUES('PH-DV', 'Davao, Philippines', GETUTCDATE(), 'System')
END;

IF NOT EXISTS (SELECT 1 FROM dbo.[Location] WHERE [Name] = 'ES-SS' 
	AND [Description] = 'San Salvador, El Salvador')
BEGIN
	
INSERT INTO dbo.[Location]([Name], [Description], CreateDate, CreatedBy)
VALUES ('ES-SS', 'San Salvador, El Salvador', GETUTCDATE(), 'System')
END;
