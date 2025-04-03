CREATE PROCEDURE [dbo].[sp_GetUserShiftData] 
	@userDetailsId INT,
	@userIds VARCHAR(250) = '',
	@accountIds VARCHAR(250) = '',
	@teamIds VARCHAR(250) = '',
	@tagIds VARCHAR(250) = '',
	@roles VARCHAR(250) = ''
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @assignedAccounts TABLE(AccountId INT);
	DECLARE	@assigedTeams TABLE(TeamId INT, AccountId INT);
	DECLARE	@assigedUsers TABLE(UserDetailsId INT, TeamId INT, AccountId INT, TagId INT, RoleCode VARCHAR(3), IsAmDeleted BIT, IsTmDeleted BIT);

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

	INSERT INTO @assigedUsers(UserDetailsId, TeamId, AccountId, TagId, RoleCode, IsAmDeleted, IsTmDeleted)
		(SELECT u.UserDetailsId, tm.TeamId, am.AccountId, ut.TagId, u.[Role], am.IsDeleted, tm.IsDeleted
		 FROM dbo.vw_AllUsers u			
			LEFT JOIN dbo.AccountMember am ON am.UserDetailsId = u.UserDetailsId
			LEFT JOIN dbo.TeamMember tm ON tm.UserDetailsId = u.UserDetailsId
			LEFT JOIN dbo.UserTag ut ON ut.UserDetailsId = u.UserDetailsId
		  WHERE ISNULL(u.IsDeleted, 0) = 0
			AND (@role = 'SA' AND u.[Role] IN ('TM', 'LA', 'AG')
			OR (@role = 'AM' AND u.[Role] IN ('TM', 'LA', 'AG') 
				AND ISNULL(tm.IsDeleted, 0) = 0 AND ISNULL(am.IsDeleted, 0) = 0
				AND EXISTS(SELECT * FROM @assignedAccounts a WHERE a.AccountId = am.AccountId) 
				OR u.UserDetailsId = @userDetailsId)
			OR (@role = 'CL' AND u.[Role] IN ('TM', 'LA', 'AG') 
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

	IF LEN(@roles) > 0
		DELETE FROM @assigedUsers 
			WHERE NOT EXISTS(SELECT * FROM STRING_SPLIT(@roles, ',') s WHERE s.value = RoleCode)
	
	IF LEN(@tagIds) > 0
		DELETE FROM @assigedUsers 
			WHERE NOT EXISTS(SELECT * FROM STRING_SPLIT(@tagIds, ',') s WHERE s.value = TagId)

	SELECT 
          RTRIM(LTRIM(u.FullName)) AS Fullname
		, u.NetUserId
		, u.UserDetailsId AS UserDetailsId
		, CAST(COALESCE(u.IsLocked, 0) AS BIT) AS IsLocked
		, RTRIM(LTRIM(u.FullName)) AS Fullname
		, COALESCE(u.Email, '') AS Email 
		, COALESCE(u.[Role], '') AS RoleCode 
		, COALESCE(u.[RoleName], '') AS RoleName
		, u.StaffId
		, ISNULL(u.TimeZoneId, 0) as TimeZoneId
		, (SELECT 
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
		, (SELECT wd.Id
				, wd.DayIndex
				, wd.[Day]
			FROM dbo.UserWorkDay wd
			WHERE wd.UserDetailsId = u.UserDetailsId
			FOR JSON PATH) [WorkDays]
        , COALESCE(sd.Id, '') AS ShiftDetailsId
        , COALESCE(sd.ShiftStart, '9:00:00') AS ShiftStart
		, COALESCE(sd.ShiftEnd, '18:00:00') AS ShiftEnd
		, COALESCE(sd.LunchBreak, '1') AS LunchBreak
		, COALESCE(sd.PaidBreaks, '2') AS PaidBreaks
		, 5 AS DaysPerWeek
		, DATEDIFF(MINUTE, 
			COALESCE(sd.ShiftStart, CAST('9:00:00' AS TIME)), 
			COALESCE(sd.ShiftEnd, CAST('18:00:00' AS TIME)))
			- (60 + (COALESCE(sd.PaidBreaks, 2) * 15))				
				AS DailyHours

	FROM dbo.vw_AllUsers u
	LEFT JOIN dbo.UserShiftDetail sd 
		ON sd.UserDetailsId = u.UserDetailsId
	WHERE EXISTS(SELECT 1 FROM @assigedUsers au
				WHERE UserDetailsId = u.UserDetailsId)
		AND u.IsDeleted <> 1
		AND CAST(COALESCE(u.IsLocked, 0) AS BIT) <> 1
	ORDER BY u.IsLocked, u.Fullname

	--SELECT DISTINCT(UserDetailsId), RoleCode FROM @assigedUsers
		
	FOR JSON AUTO
END
