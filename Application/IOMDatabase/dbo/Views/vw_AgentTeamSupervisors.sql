CREATE VIEW [dbo].[vw_AgentTeamSupervisors]
    AS 
	
	SELECT
				u.UserId AS UserUniqueId 
			,	u.Id AS UserDetailsId 
			,	RTRIM(LTRIM(u.[Name])) AS Fullname
			,	FirstName = RTRIM(LTRIM(u.FirstName))  
			,	LastName = RTRIM(LTRIM(u.LastName)) 
			,	u.[Role]
			,	ur.Name as RoleName
			,	u.[Image]
			,	SUBSTRING
				(
					(
					SELECT ','+ CONVERT(VARCHAR, a.[Id]) AS [text()] 
					FROM dbo.Accounts a 
					INNER JOIN dbo.AccountMember aat ON a.Id = aat.AccountId
					WHERE aat.UserDetailsId = u.Id AND a.IsActive = 1 AND ISNULL(aat.IsDeleted, 0) = 0
					ORDER BY a.[Name]
					FOR XML PATH ('')), 2, 1000) AS AccountIds	
		, SUBSTRING((SELECT ','+ CONVERT(VARCHAR, t.[Id]) AS [text()]
					FROM dbo.Teams t 
					INNER JOIN dbo.TeamMember ts ON t.Id = ts.TeamId
					WHERE ts.UserDetailsId = u.Id AND t.IsActive = 1 AND ISNULL(ts.IsDeleted, 0) = 0
					AND t.AccountId IN (SELECT aa.AccountId FROM dbo.AccountMember aa WHERE ISNULL(aa.IsDeleted, 0) = 0 AND aa.UserDetailsId = u.Id)
					FOR XML PATH ('')
					), 2, 1000) AS TeamIds
					
	FROM UserDetails u
	LEFT JOIN AspNetRoles ur ON u.[Role] = ur.RoleCode

	WHERE u.IsDeleted = 0 AND (u.[Role] = 'AG' OR u.[Role] = 'TS' OR u.[Role] = 'LA') and ISNULL(u.islocked, 0) = 0
