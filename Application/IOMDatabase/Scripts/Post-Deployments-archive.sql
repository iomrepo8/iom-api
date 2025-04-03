--/* Fix module and role duplicate issue */
--DELETE from rolepermissions where roleid in (SELECT Id From AspNetRoles where Name = 'Client-POC')
--DELETE From AspNetRoles where Name = 'Client-POC'

--IF(NOT EXISTS(SELECT * FROM dbo.AspNetRoles WHERE RoleCode = 'CL'))
--	INSERT INTO dbo.AspNetRoles(Id, Name, RoleCode)
--	VALUES(NEWID(), 'CLient', 'CL')
--GO

--WITH RPClient AS (
--	SELECT 
--		RoleId = r.Id, ModuleId = m.Id, ModuleCode = m.ModuleCode,
--		RoleName = r.Name, ModuleName = m.Name,
--		r.RoleCode
--	FROM AspNetRoles r, AspNetModules m
--	WHERE r.RoleCode = 'CL'
--),

--finalData AS (	
--	SELECT 
--		  RoleId, ModuleId, IsLocked = 0, ModuleCode
--		, CanView = CASE WHEN ModuleId = 5 OR ModuleId = 3 THEN 0 ELSE 1 END
--		, CanAdd = 0
--		, CanEdit = CASE WHEN ModuleId = 6 THEN 1 ELSE 0 END
--		, CanDelete = 0
--		, CreatedBy = 'Jonathan', DateCreated = GETDATE()
--	FROM RPClient
--)

--INSERT INTO dbo.RolePermissions(RoleId, ModuleId, IsLocked, ModuleCode, CanView, CanAdd, CanEdit, CanDelete, CreatedBy, DateCreated)
--SELECT * FROM finalData fd
--WHERE NOT EXISTS(SELECT * FROM RolePermissions rp WHERE rp.RoleId = fd.RoleId AND rp.ModuleId = fd.ModuleId)
--GO

----INSERT TASKBOARD MODULE AND DEFAULT PERMISSIONS
--IF (NOT EXISTS(SELECT * FROM dbo.AspNetModules WHERE ModuleCode = 'm-taskdashboard'))
--	INSERT INTO dbo.AspNetModules(ModuleCode, [Name], CreatedBy, DateCreated, IsActive, [Order])
--	VALUES ('m-taskdashboard', 'Task Dashboard', 'system', GETUTCDATE(), 1, 9)
--GO

--IF (NOT EXISTS(SELECT * FROM dbo.AspNetModules WHERE ModuleCode = 'm-taskgroups'))
--	INSERT INTO dbo.AspNetModules(ModuleCode, [Name], CreatedBy, DateCreated, IsActive, [Order])
--	VALUES ('m-taskgroups', 'Task Groups', 'system', GETUTCDATE(), 1, 10)
--GO

--INSERT INTO AspNetUserPermission
--SELECT  au.UserId, am.ModuleCode, 1, 1, 1, 1, 'System', GETUTCDATE(), 'System', GETUTCDATE(), 1  
--FROM UserDetails au, AspNetModules am
--WHERE NOT EXISTS (SELECT 1 FROM AspNetUserPermission _ab WHERE _ab.UserId = au.UserId AND _ab.ModuleCode = am.ModuleCode)
--AND au.Role IN ('SA','AM','TM')
--GO

----Migration of task and team task
--BEGIN TRY
--	BEGIN TRANSACTION

--	DELETE from IOMTeamTask
--	DELETE from TaskGroupItems
--	DELETE FROM usertask
--	DELETE from UserTaskGroup
--	DELETE from IOMTasks

--	-- insert Task records	
--	SET IDENTITY_INSERT IOMTasks ON
--	INSERT IOMTasks(Id, TaskNumber, Name, Description, CreatedDate, CreatedBy, IsDeleted, IsActive)
--	SELECT DISTINCT Id as teamtaskid,'' as TaskNumber, Name, Description, Created as CreatedDate, 'SysteMigration', ISNULL(IsDeleted, 0), IsActive
--	FROM TeamTasks
--	WHERE ISNULL(IsDeleted, 0) = 0
--	SET IDENTITY_INSERT IOMTasks OFF

--	-- insert assignment of task to teams
--	INSERT IOMTeamTask(TeamId, TaskId, CreatedDate, CreatedBy)
--	SELECT TeamId, Id, Created as CreatedDate, 'SysteMigration'
--	FROM TeamTasks
--	WHERE ISNULL(IsDeleted, 0) = 0

--	;WITH updateData AS(
--		-- TEAM AGENTS
--		SELECT la.TeamId, ud.Id, la.IsDeleted
--		FROM dbo.LeadAgent la
--		INNER JOIN dbo.AspNetUsers au ON la.UserId = au.Id
--		INNER JOIN dbo.UserDetails ud ON au.Id = ud.UserId

--		UNION 

--		-- CLIENTS
--		SELECT c.TeamId, c.UserDetailsId, c.IsDeleted
--		FROM dbo.TeamClientPOC c

--		UNION 

--		-- AGENTS
--		SELECT ta.TeamId, ud.Id, ta.IsDeleted
--		FROM dbo.TeamAgents ta 
--		INNER JOIN dbo.AspNetUsers au ON ta.UserId = au.Id
--		INNER JOIN dbo.UserDetails ud ON au.Id = ud.UserId
--	)

--	INSERT INTO dbo.TeamMember(UserDetailsId, TeamId, IsDeleted, CreatedDateUtc)
--	SELECT u.Id, u.TeamId, u.IsDeleted, GETUTCDATE() 
--	FROM updateData u
--	WHERE NOT EXISTS(SELECT * FROM dbo.TeamMember tm WHERE tm.UserDetailsId = u.Id AND tm.TeamId = u.TeamId)


--	/*
--		Migration of data for AccountMembers
--	*/
--	;WITH updateData AS(
--		SELECT a.AccountId, ud.Id, a.IsDeleted 
--		FROM dbo.AccountAgTs a
--		INNER JOIN dbo.UserDetails ud ON ud.Id = a.UserDetailsId

--		UNION 

--		SELECT c.AccountId, c.UserDetailsId, c.IsDeleted
--		FROM dbo.ClientPOC c

--		UNION 

--		SELECT am.AccountId, ud.Id, am.IsDeleted
--		FROM dbo.AccountManagers am 
--		INNER JOIN dbo.AspNetUsers au ON am.UserId = au.Id
--		INNER JOIN dbo.UserDetails ud ON au.Id = ud.UserId
--	)

--	INSERT INTO dbo.AccountMember(UserDetailsId, AccountId, IsDeleted, Created)
--	SELECT u.Id, u.AccountId, u.IsDeleted, GETUTCDATE() FROM updateData u
--	WHERE NOT EXISTS(SELECT * FROM dbo.AccountMember am WHERE am.UserDetailsId = u.Id AND am.AccountId = u.AccountId)

--	COMMIT TRANSACTION
--END TRY
--BEGIN CATCH
--	ROLLBACK TRANSACTION

--	SELECT
--    ERROR_NUMBER() AS ErrorNumber,
--    ERROR_STATE() AS ErrorState,
--    ERROR_SEVERITY() AS ErrorSeverity,
--    ERROR_PROCEDURE() AS ErrorProcedure,
--    ERROR_LINE() AS ErrorLine,
--    ERROR_MESSAGE() AS ErrorMessage;
--END CATCH

--;WITH fillData AS (	
--	SELECT u.UserId, rp.ModuleCode, rp.CanView, rp.CanAdd, rp.CanEdit, rp.CanDelete
--	FROM [dbo].[RolePermissions] rp
--	INNER JOIN AspNetRoles r ON rp.RoleId = r.Id
--	INNER JOIN UserDetails u ON u.[Role] = r.RoleCode
--	WHERE RoleCode = 'CL'
--)

--INSERT INTO AspNetUserPermission(UserId, ModuleCode, CanView, CanAdd, CanEdit, CanDelete, CreatedBy, DateCreated)
--SELECT fd.UserId, fd.ModuleCode, fd.CanView, fd.CanAdd, fd.CanEdit, fd.CanDelete, 'System',  GETUTCDATE()
--FROM fillData fd
--WHERE NOT EXISTS(
--	SELECT 1 FROM AspNetUserPermission up WHERE up.UserId =  fd.UserId AND up.ModuleCode = fd.ModuleCode
--)