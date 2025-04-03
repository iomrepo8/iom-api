CREATE PROCEDURE [dbo].[sp_GetTimekeepingReportGridData] 
	@userDetailsId INT,
	@dateFrom varchar(50),
	@dateTo varchar(50),
	@userIds VARCHAR(250) = '',
	@accountIds VARCHAR(250) = '',
	@teamIds VARCHAR(250) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @assignedAccounts TABLE(AccountId INT);
	DECLARE	@assigedTeams TABLE(TeamId INT, AccountId INT);
	DECLARE	@assigedUsers TABLE(UserDetailsId INT, TeamId INT, AccountId INT, RoleCode VARCHAR(3), IsAmDeleted BIT, IsTmDeleted BIT);

	DECLARE @role VARCHAR(2);
	SELECT @role = ud.[Role] FROM dbo.UserDetails ud WHERE ud.Id = @userDetailsId;

	INSERT INTO @assignedAccounts(AccountId)
	(SELECT AccountId FROM fn_GetAssignedAccounts(@userDetailsId));

	IF LEN(@accountIds) > 0
		DELETE FROM @assignedAccounts 
			WHERE NOT EXISTS(SELECT * FROM STRING_SPLIT(@accountIds, ',') s WHERE s.value = AccountId)

	INSERT INTO @assigedTeams(TeamId, AccountId)
		(SELECT tm.TeamId, t.AccountId 
		FROM dbo.TeamMember tm
			INNER JOIN dbo.Teams t ON tm.TeamId = t.Id
			INNER JOIN @assignedAccounts aa ON t.AccountId = aa.AccountId
		WHERE tm.UserDetailsId = @userDetailsId AND ISNULL(tm.IsDeleted, 0) = 0
		UNION
		SELECT t.Id AS TeamId, t.AccountId 
		FROM dbo.Teams t
		WHERE t.IsActive = 1 AND ISNULL(t.IsDeleted, 0) = 0 AND @role = 'SA'
		UNION
		SELECT t.Id, t.AccountId 
		FROM dbo.Teams t
			INNER JOIN @assignedAccounts aa ON t.AccountId = aa.AccountId
		WHERE @role = 'AM');

	IF LEN(@teamIds) > 0
		DELETE FROM @assigedTeams 
			WHERE NOT EXISTS(SELECT * FROM STRING_SPLIT(@teamIds, ',') s WHERE s.value = TeamId)

	INSERT INTO @assigedUsers(UserDetailsId, TeamId, AccountId, RoleCode, IsAmDeleted, IsTmDeleted)
		(SELECT u.UserDetailsId, tm.TeamId, am.AccountId, u.[Role], am.IsDeleted, tm.IsDeleted
		 FROM dbo.vw_AllUsers u			
			LEFT JOIN dbo.AccountMember am ON am.UserDetailsId = u.UserDetailsId
			LEFT JOIN dbo.TeamMember tm ON tm.UserDetailsId = u.UserDetailsId
		  WHERE ISNULL(u.IsDeleted, 0) = 0
			AND (@role = 'SA' AND u.[Role] IN ('SA', 'AM', 'CL', 'TM', 'LA', 'AG')
			OR (@role = 'AM' AND u.[Role] IN ('CL', 'TM', 'LA', 'AG') 
				AND ISNULL(tm.IsDeleted, 0) = 0 AND ISNULL(am.IsDeleted, 0) = 0
				AND EXISTS(SELECT * FROM @assignedAccounts a WHERE a.AccountId = am.AccountId) 
				OR u.UserDetailsId = @userDetailsId)
			OR (@role = 'CL' AND u.[Role] IN ('CL', 'TM', 'LA', 'AG') 
				AND ISNULL(tm.IsDeleted, 0) = 0 AND ISNULL(am.IsDeleted, 0) = 0
				AND (EXISTS(SELECT * FROM @assigedTeams t WHERE t.TeamId = tm.TeamId) AND EXISTS(SELECT * FROM @assignedAccounts a where a.AccountId = am.AccountId))
				OR u.UserDetailsId = @userDetailsId)
			OR (@role = 'TM' AND u.[Role] IN ('LA', 'AG') 
				AND ISNULL(tm.IsDeleted, 0) = 0 AND ISNULL(am.IsDeleted, 0) = 0
				AND (EXISTS(SELECT * FROM @assigedTeams t WHERE t.TeamId = tm.TeamId) AND EXISTS(SELECT * FROM @assignedAccounts a where a.AccountId = am.AccountId)) 
				OR u.UserDetailsId = @userDetailsId)
			OR (@role = 'LA' AND u.[Role] IN ('AG') 
				AND ISNULL(tm.IsDeleted, 0) = 0 AND ISNULL(am.IsDeleted, 0) = 0
				AND (EXISTS(SELECT * FROM @assigedTeams t WHERE t.TeamId = tm.TeamId) AND EXISTS(SELECT * FROM @assignedAccounts a where a.AccountId = am.AccountId)) 
				OR u.UserDetailsId = @userDetailsId)
				)
		);
			
	DELETE FROM @assigedUsers 
		WHERE 
		(
			(AccountId NOT IN(SELECT AccountId FROM @assignedAccounts) OR AccountId IS NULL)
			AND (TeamId NOT IN(SELECT TeamId FROM @assigedTeams) OR TeamId IS NULL)
		)
		AND UserDetailsId <> @userDetailsId

	IF LEN(@userIds) > 0
		DELETE FROM @assigedUsers 
			WHERE NOT EXISTS(SELECT * FROM STRING_SPLIT(@userIds, ',') s WHERE s.value = UserDetailsId)

	IF LEN(@accountIds) > 0
		DELETE FROM @assigedUsers
			WHERE AccountId NOT IN(SELECT AccountId FROM @assignedAccounts) 
				OR IsAmDeleted = 1
				OR AccountId IS NULL 

	IF LEN(@teamIds) > 0
		DELETE FROM @assigedUsers 
			WHERE TeamId NOT IN(SELECT TeamId FROM @assigedTeams) 
				OR IsTmDeleted = 1
				OR TeamId IS NULL
			
	;WITH reportData AS(
		SELECT 
				th.UserDetailsId 			
			,	CONVERT(DECIMAL(10,2), SUM(th.Duration) / 60) as TaskActiveTime
			,	ISNULL(th.TaskId, -1) as TaskId
			,	th.TaskHistoryTypeId
			,	CASE WHEN th.TaskHistoryTypeId IN (9) THEN CAST(1 as bit) ELSE tt.IsActive END as IsActiveTask
			,	RTRIM(LTRIM(ISNULL(tt.Name, 'AVAIL'))) AS TaskName
			,	REPLACE(REPLACE(ISNULL(tt.[Description], 'AVAIL'), CHAR(13), ''), CHAR(10), '') AS TaskDescription
			,	COALESCE(tt.TaskNumber, '') AS TaskNumber
			
		FROM TaskHistory th
			LEFT JOIN dbo.IOMTasks tt ON th.TaskId = tt.Id
		WHERE 
				HistoryDate >= @dateFrom
			AND HistoryDate <= @dateTo
			AND th.TaskHistoryTypeId IN (1,8,9)
			AND EXISTS(SELECT TOP(1) au.UserDetailsId FROM @assigedUsers au
						WHERE UserDetailsId = th.UserDetailsId)
		GROUP BY th.UserDetailsId
			, th.TaskId, th.TaskHistoryTypeId
			, tt.[Name], tt.[Description]
			, tt.IsActive, tt.TaskNumber
	)

	SELECT 
			r.* 
		,	COALESCE(u.FullName, '') AS FullName
		,   COALESCE(u.StaffId, '') AS StaffId
		,	COALESCE(u.[Role], '') AS RoleCode 
		,	COALESCE(u.[RoleName], '') AS RoleName
		,	(SELECT 
					COALESCE(a.Id, 0) AS Id
					, COALESCE(a.[Name], '') AS [Name]
				FROM dbo.AccountMember am
					INNER JOIN dbo.Accounts a ON am.AccountId = a.Id
					INNER JOIN @assignedAccounts aa ON a.Id = aa.AccountId
				WHERE 
					am.UserDetailsId = u.UserDetailsId 
					AND ISNULL(am.IsDeleted, 0) = 0
					AND u.[Role] <> 'SA'
				GROUP BY a.Id, a.[Name]
				FOR JSON AUTO) [Accounts]
			, (SELECT 
					COALESCE(t.Id, 0) AS Id
					, COALESCE(t.[Name], '') AS [Name] 
				FROM dbo.TeamMember tm
					INNER JOIN dbo.Teams t ON tm.TeamId = t.Id
					INNER JOIN @assigedTeams ast ON t.Id = ast.TeamId
				WHERE 
					tm.UserDetailsId = u.UserDetailsId
					AND ISNULL(tm.IsDeleted, 0) = 0
					AND u.[Role] IN ('CL', 'TM', 'LA', 'AG')
				GROUP BY t.Id, t.[Name]
				FOR JSON PATH) [Teams]
			, (SELECT 
					COALESCE(t.Id, 0) AS Id
					, COALESCE(t.[Name], '') AS [Name] 
				FROM dbo.UserTag ut
					INNER JOIN dbo.Tag t ON ut.TagId = t.Id
				WHERE 
					ut.UserDetailsId = u.UserDetailsId				
				GROUP BY t.Id, t.[Name]
				FOR JSON PATH) [Tags]
	FROM reportData r
		LEFT JOIN vw_AllUsers u ON r.UserDetailsId = u.UserDetailsId	
	FOR JSON AUTO
END
