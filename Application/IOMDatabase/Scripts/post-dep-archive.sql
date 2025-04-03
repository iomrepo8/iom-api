--ADMIN
WITH rolePermissions AS (
	SELECT  ModuleCode = 'm-dashboard', Name = 'Dashboard', DateCreated = GETDATE(), DateUpdated = GETDATE(), CreatedBy = 'nere', UpdateBy = 'nere', IsActive = 1  
	UNION 
	SELECT  ModuleCode = 'm-accounts', Name = 'Accounts', DateCreated = GETDATE(), DateUpdated = GETDATE(), CreatedBy = 'nere', UpdateBy = 'nere', IsActive = 1  
	UNION
	SELECT  ModuleCode = 'm-teams', Name = 'Teams', DateCreated = GETDATE(), DateUpdated = GETDATE(), CreatedBy = 'nere', UpdateBy = 'nere', IsActive = 1  
	UNION 
	SELECT  ModuleCode = 'm-users', Name = 'Users', DateCreated = GETDATE(), DateUpdated = GETDATE(), CreatedBy = 'nere', UpdateBy = 'nere', IsActive = 1  
	UNION
	SELECT  ModuleCode = 'm-timekeeps', Name = 'Timekeeping', DateCreated = GETDATE(), DateUpdated = GETDATE(), CreatedBy = 'nere', UpdateBy = 'nere', IsActive = 1  
	UNION
	SELECT  ModuleCode = 'm-support', Name = 'Support', DateCreated = GETDATE(), DateUpdated = GETDATE(), CreatedBy = 'nere', UpdateBy = 'nere', IsActive = 1  
	UNION
	SELECT  ModuleCode = 'm-permissions', Name = 'User Permissions', DateCreated = GETDATE(), DateUpdated = GETDATE(), CreatedBy = 'jonathan', UpdateBy = 'jonathan', IsActive = 1 
	UNION
	SELECT  ModuleCode = 'm-syslogs', Name = 'System Logs', DateCreated = GETDATE(), DateUpdated = GETDATE(), CreatedBy = 'jonathan', UpdateBy = 'jonathan', IsActive = 1 
)

INSERT INTO AspNetModules(ModuleCode, Name, DateCreated, DateUpdated, CreatedBy, UpdateBy, IsActive)
SELECT * FROM rolePermissions rp
WHERE NOT EXISTS(SELECT * FROM AspNetModules m WHERE m.Name = rp.Name AND m.ModuleCode = rp.ModuleCode)
GO

INSERT INTO AspNetUserPermission
SELECT  au.UserId, am.ModuleCode, 1, 1, 1, 1, 'nere', GETDATE(), 'nere', GETDATE(), 1  
FROM UserDetails au, AspNetModules am
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserPermission _ab WHERE _ab.UserId = au.UserId AND _ab.ModuleCode = am.ModuleCode)
AND au.Role IN ('SA','AM','LA')
GO

INSERT INTO AspNetUserPermission
SELECT  au.UserId, am.ModuleCode, 1, 1, 1, 1, 'nere', GETDATE(), 'nere', GETDATE(), 1  
FROM UserDetails au, AspNetModules am
WHERE NOT EXISTS (SELECT 1 FROM AspNetUserPermission _ab WHERE _ab.UserId = au.UserId AND _ab.ModuleCode = am.ModuleCode)
AND au.Role = 'AG'
GO

-- ROLES 

/****** Script for SelectTopNRows command from SSMS  ******/
SET IDENTITY_INSERT TaskHistoryType ON;
GO
WITH taskTypeData AS (
		  SELECT 1 AS Id, 'Task' AS [Name], 'Project Task' AS [Description]
	UNION SELECT 2 AS Id, 'Meeting' AS [Name], 'Project Meeting' AS [Description]
	UNION SELECT 3 AS Id, 'Lunch Break' AS [Name], 'Lunch Break' AS [Description]
	UNION SELECT 4 AS Id, 'First Break' AS [Name], 'First Break' AS [Description]
	UNION SELECT 5 AS Id, 'Second Break' AS [Name], 'Second Break' AS [Description]
	UNION SELECT 6 AS Id, 'Bio Break' AS [Name], 'Bio Break' AS [Description]
	UNION SELECT 7 AS Id, 'Out' AS [Name], 'Out' AS [Description]
	UNION SELECT 8 AS Id, 'ApprovedEOD' as [Name], 'Approved EOD Updates' as [Description]
	UNION SELECT 9 AS Id, 'AVAIL' as [Name], 'AVAIL' as [Description]
)

INSERT INTO TaskHistoryType(Id, [Name], [Description])
SELECT * FROM taskTypeData tt
WHERE NOT EXISTS(SELECT * FROM TaskHistoryType th WHERE th.[Name] = tt.[Name])
GO
SET IDENTITY_INSERT TaskHistoryType OFF;
GO
UPDATE AspNetRoles SET RoleCode = 'AM' WHERE [Name] = 'Account Manager';
UPDATE AspNetRoles SET RoleCode = 'AG' WHERE [Name] = 'Agent';
UPDATE AspNetRoles SET RoleCode = 'SA' WHERE [Name] = 'System Administrator';
UPDATE AspNetRoles SET RoleCode = 'LA' WHERE [Name] = 'Lead Agent';
GO

-- EMPLOYEE STATUS TABLE

SET IDENTITY_INSERT EmployeeStatus ON;
GO
WITH emp_status AS (
          SELECT 1 AS Id, 'Probationary' AS [Name], 'Probationary' AS [Description]
	UNION SELECT 2 AS Id, 'Regular' AS [Name], 'Regular' AS [Description]
	UNION SELECT 3 AS Id, 'Contractual' AS [Name], 'Contractual' AS [Description]
	UNION SELECT 4 AS Id, 'Part-Time' AS [Name], 'Part-Time' AS [Description]
)

INSERT INTO EmployeeStatus(Id, [Name], [Description])
SELECT * FROM emp_status es
WHERE NOT EXISTS(SELECT * FROM EmployeeStatus e WHERE e.Name = es.Name)
GO
SET IDENTITY_INSERT EmployeeStatus OFF;
GO

-- UPDATE USER DETAILS with Employee Status Id

UPDATE UserDetails
SET EmployeeStatusId = CASE WHEN EmployeeStatus = 'Probationary' THEN 1
                            WHEN EmployeeStatus = 'Regular' THEN 2
                            WHEN EmployeeStatus = 'Contractual' THEN 3
                            WHEN EmployeeStatus = 'Part-Time' THEN 4
                            END
GO

-- SHIFT TABLE

SET IDENTITY_INSERT EmployeeShift ON;
GO
WITH eshift AS (
          SELECT 1 AS Id, 'Day Shift' AS [Name], 'DS - Day Shift' AS [Description]
	UNION SELECT 2 AS Id, 'Swing Shift' AS [Name], 'SS - Swing Shift' AS [Description]
	UNION SELECT 3 AS Id, 'Night Shift' AS [Name], 'NS - Night Shift' AS [Description]
)

INSERT INTO EmployeeShift(Id, [Name], [Description])
SELECT * FROM eshift es
WHERE NOT EXISTS(SELECT * FROM EmployeeShift e WHERE e.Name = es.Name)
GO
SET IDENTITY_INSERT EmployeeShift OFF;
GO

-- UPDATE USER DETAILS with Employee Shift Id

UPDATE UserDetails
SET EmployeeShiftId = CASE WHEN [Shift] = 'DS' THEN 1
                            WHEN [Shift] = 'SS' THEN 2
                            WHEN [Shift] = 'NS' THEN 3
                            END
GO

-- Update Account Managers with UserDetails Id

UPDATE am 
SET am.UserDetailId = ud.Id
FROM AccountManagers am 
	INNER JOIN UserDetails ud ON am.UserId = ud.UserId
GO


UPDATE UserDetails SET HourlyRate = 0 WHERE HourlyRate IS NUll
GO

-- Update Teams shift

UPDATE Teams
SET ShiftId = CASE WHEN [ShiftSchedule] = 'DS' THEN 1
                            WHEN [ShiftSchedule] = 'SS' THEN 2
                            WHEN [ShiftSchedule] = 'NS' THEN 3
                            END
GO

-- Permissions per Role default

WITH RPSysAd AS (
	SELECT 
		RoleId = r.Id, ModuleId = m.Id, ModuleCode = m.ModuleCode,
		RoleName = r.Name, ModuleName = m.Name,
		r.RoleCode
	FROM AspNetRoles r, AspNetModules m
	WHERE r.RoleCode = 'SA'
),

RPAccMngr AS (
	SELECT 
		RoleId = r.Id, ModuleId = m.Id, ModuleCode = m.ModuleCode,
		RoleName = r.Name, ModuleName = m.Name,
		r.RoleCode
	FROM AspNetRoles r, AspNetModules m
	WHERE r.RoleCode = 'AM'
),

RPSup AS (
	SELECT 
		RoleId = r.Id, ModuleId = m.Id, ModuleCode = m.ModuleCode,
		RoleName = r.Name, ModuleName = m.Name,
		r.RoleCode
	FROM AspNetRoles r, AspNetModules m
	WHERE r.RoleCode = 'LA'
),

RPAgent AS (
	SELECT 
		RoleId = r.Id, ModuleId = m.Id, ModuleCode = m.ModuleCode,
		RoleName = r.Name, ModuleName = m.Name,
		r.RoleCode
	FROM AspNetRoles r, AspNetModules m
	WHERE r.RoleCode = 'AG'
),

finalData AS (
	SELECT 
		  RoleId, ModuleId, IsLocked = 0, ModuleCode
		, CanView = 1, CanAdd = 1, CanEdit = 1, CanDelete = 1
		, CreatedBy = 'Jonathan', DateCreated = GETDATE()
	FROM RPSysAd
	UNION
	SELECT 
		  RoleId, ModuleId, IsLocked = 0, ModuleCode
		, CanView = CASE WHEN ModuleId = 5 OR ModuleId = 3 THEN 0 ELSE 1 END
		, CanAdd = 0
		, CanEdit = CASE WHEN ModuleId = 1 OR ModuleId = 6 THEN 1 ELSE 0 END
		, CanDelete = 0
		, CreatedBy = 'Jonathan', DateCreated = GETDATE()
	FROM RPAccMngr
	UNION
	SELECT 
		  RoleId, ModuleId, IsLocked = 0, ModuleCode
		, CanView = CASE WHEN ModuleId = 5 OR ModuleId = 3 THEN 0 ELSE 1 END
		, CanAdd = 0
		, CanEdit = CASE WHEN ModuleId = 6 THEN 1 ELSE 0 END
		, CanDelete = 0
		, CreatedBy = 'Jonathan', DateCreated = GETDATE()
	FROM RPSup
	UNION
	SELECT 
		  RoleId, ModuleId, IsLocked = 0, ModuleCode
		, CanView = CASE WHEN ModuleId = 5 OR ModuleId = 3 THEN 0 ELSE 1 END
		, CanAdd = 0
		, CanEdit = 0
		, CanDelete = 0
		, CreatedBy = 'Jonathan', DateCreated = GETDATE()
	FROM RPAgent
)

INSERT INTO dbo.RolePermissions(RoleId, ModuleId, IsLocked, ModuleCode, CanView, CanAdd, CanEdit, CanDelete, CreatedBy, DateCreated)
SELECT * FROM finalData fd
WHERE NOT EXISTS(SELECT * FROM RolePermissions rp WHERE rp.RoleId = fd.RoleId AND rp.ModuleId = fd.ModuleId)
GO

IF OBJECT_ID('sp_SettleActiveOfflineUsers', 'P') IS NOT NULL
DROP PROC sp_SettleActiveOfflineUsers
GO

  UPDATE AspNetModules 
  SET [Order] = CASE WHEN ModuleCode = 'm-dashboard' THEN 1
					 WHEN ModuleCode = 'm-accounts' THEN 2
					 WHEN ModuleCode = 'm-teams' THEN 3
					 WHEN ModuleCode = 'm-users' THEN 4
					 WHEN ModuleCode = 'm-timekeeps' THEN 5
					 WHEN ModuleCode = 'm-syslogs' THEN 6
					 WHEN ModuleCode = 'm-permissions' THEN 7
					 WHEN ModuleCode = 'm-support' THEN 8
					 END
GO
