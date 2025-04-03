CREATE PROCEDURE [dbo].[sp_GetEODList]
    @userDetailsId INT = 0,
    @dateFrom VARCHAR(50),
    @dateTo VARCHAR(50),
	@userIds VARCHAR(250) = '',
	@accountIds VARCHAR(250) = '',
	@teamIds VARCHAR(250) = '',
    @tagIds VARCHAR(250) = '',
    @roles VARCHAR(250) = '',
	@withActionOnly bit = 0
AS
	DECLARE @assignedAccounts TABLE(AccountId INT);
	DECLARE	@assigedTeams TABLE(TeamId INT, AccountId INT);
	DECLARE	@assigedUsers TABLE(UserDetailsId INT, TeamId INT, AccountId INT, TagId INT, RoleCode VARCHAR(3), IsAmDeleted BIT, IsTmDeleted BIT);

	DECLARE @role VARCHAR(2);

	SELECT @role = ud.[Role] FROM dbo.UserDetails ud WHERE ud.Id = @userDetailsId;

    INSERT INTO @assignedAccounts(AccountId) 
		(SELECT AccountId FROM dbo.AccountMember am
		WHERE am.UserDetailsId = @userDetailsId AND ISNULL(am.IsDeleted, 0) = 0
		UNION
		SELECT a.Id AS AccountId FROM dbo.Accounts a
		WHERE a.IsActive = 1 AND ISNULL(a.IsDeleted, 0) = 0 AND @role = 'SA');

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
		INNER JOIN @assignedAccounts aa ON t.AccountId = aa.AccountId
		WHERE t.IsActive = 1 AND ISNULL(t.IsDeleted, 0) = 0 AND @role IN ('SA', 'AM'));

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

    IF LEN(@roles) > 0
        DELETE FROM @assigedUsers
        WHERE NOT EXISTS(SELECT * FROM STRING_SPLIT(@roles, ',') s WHERE s.value = RoleCode)
    
    IF LEN(@tagIds) > 0
        DELETE FROM @assigedUsers
        WHERE NOT EXISTS(SELECT * FROM STRING_SPLIT(@tagIds, ',') s WHERE s.value = TagId)
        
	;WITH editsData AS(
		SELECT 
			r.Id, r.EODDate, r.UserId
		FROM EODReports r
		INNER JOIN EODTaskItems ti ON r.Id = ti.EODReportId
		WHERE 
				r.EODDate >= @dateFrom
			AND r.EODDate <= @dateTo
			AND (ti.IsEdited = 1 OR ti.IsInserted = 1 OR ti.IsRemoved = 1)
		GROUP BY r.Id, r.EODDate, r.UserId
	)
	
	SELECT
		  r.Id
		, FORMAT(r.EODDate, 'yyyy-MM-dd')                                                                  AS EODDate		
		, COALESCE(r.IsConfirmed, 0)                                                                       AS IsConfirmed
		, COALESCE(u.FullName, '')                                                                         AS FullName
		, COALESCE(u.UserDetailsId, 0)                                                                     AS UserDetailsId
		, COALESCE(u.NetUserId, '')                                                                        AS NetUserId
		, COALESCE(u.[Role], '')                                                                           AS RoleCode
		, COALESCE(apvr.FullName, '')                                                                      AS ConfirmedByFullName
		, IIF((r.ConfirmedUTCDate IS NULL), NULL, 1)                                                       AS IsTouched
		, IIF(r.ConfirmedUTCDate IS NOT NULL, CONVERT(VARCHAR, COALESCE(r.ConfirmedUTCDate, ''), 100), '') AS DateTimeConfirmed
		, r.SentUTCDateTime
		, IIF(r.ConfirmedUTCDate IS NOT NULL, 'Confirmed', 'No Action')                                    as EODAction
		, r.ConfirmedUTCDate
		, (SELECT a.Id, a.[Name]
				FROM dbo.AccountMember am
				INNER JOIN dbo.Accounts a ON am.AccountId = a.Id
				INNER JOIN @assignedAccounts aa ON a.Id = aa.AccountId
				WHERE ISNULL(am.IsDeleted, 0) = 0 AND am.UserDetailsId = u.UserDetailsId
				GROUP BY a.Id, a.[Name]
			FOR JSON AUTO)                                                                                    [Accounts]
		, (SELECT 
				  COALESCE(t.Id, 0) AS Id
				, COALESCE(t.[Name], '') AS [Name] 
			FROM dbo.TeamMember tm
				INNER JOIN dbo.Teams t ON tm.TeamId = t.Id
				INNER JOIN @assigedTeams ast ON t.Id = ast.TeamId
			WHERE ISNULL(tm.IsDeleted, 0) = 0 AND tm.UserDetailsId = u.UserDetailsId
			FOR JSON PATH)                                                                                    [Teams]
		, (SELECT 
				COALESCE(t.Id, 0) AS Id
				, COALESCE(t.[Name], '') AS [Name] 
			FROM dbo.UserTag ut
				INNER JOIN dbo.Tag t ON ut.TagId = t.Id
			WHERE 
				ut.UserDetailsId = u.UserDetailsId				
			GROUP BY t.Id, t.[Name]
			FOR JSON PATH) [Tags]
	FROM dbo.EODReports r
	INNER JOIN vw_ActiveUsers u ON r.UserId = u.NetUserId
	INNER JOIN editsData e ON r.Id = e.Id
	LEFT JOIN vw_ActiveUsers apvr ON r.ConfirmedBy = apvr.UserDetailsId
	WHERE
		EXISTS(SELECT 1 FROM @assigedUsers au
				LEFT JOIN @assignedAccounts aa ON au.AccountId = aa.AccountId
				LEFT JOIN @assigedTeams st ON au.TeamId = st.TeamId
				WHERE UserDetailsId = u.UserDetailsId)
		AND r.EODDate >= @dateFrom
		AND r.EODDate <= @dateTo
		AND CASE 
				WHEN @withActionOnly = 1 AND r.ConfirmedUTCDate IS NULL THEN 1
				WHEN @withActionOnly = 0 THEN 1 
				ELSE 0 
			END = 1
	ORDER BY r.IsConfirmed
     FOR JSON AUTO
RETURN 0
