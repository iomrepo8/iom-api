CREATE VIEW [dbo].[vw_ActiveUsers]
    AS 
    
    SELECT 
		  ud.Id AS UserDetailsId
		, au.Id AS NetUserId
		, au.Email
		, au.UserName
		, ud.FirstName + ' ' + ud.LastName AS FullName
		, ud.FirstName
		, ud.LastName
		, ud.[Role]
		, ur.Name as RoleName
		, ud.[Image]
		, ud.StaffId
		, SUBSTRING(
					(
					SELECT ','+ CONVERT(VARCHAR, a.[Id]) AS [text()] 
					FROM dbo.Accounts a 
					INNER JOIN dbo.AccountMember aat ON a.Id = aat.AccountId
					WHERE aat.UserDetailsId = ud.Id AND a.IsActive = 1 AND ISNULL(aat.IsDeleted, 0) = 0
					ORDER BY a.[Name]
					FOR XML PATH ('')), 2, 1000) AS AccountIds	
		, SUBSTRING((SELECT ','+ CONVERT(VARCHAR, t.[Id]) AS [text()]
					FROM dbo.Teams t 
					INNER JOIN dbo.TeamMember ts ON t.Id = ts.TeamId
					WHERE ts.UserDetailsId = ud.Id AND t.IsActive = 1 AND ISNULL(ts.IsDeleted, 0) = 0
					AND t.AccountId IN (SELECT aa.AccountId FROM dbo.AccountMember aa WHERE ISNULL(aa.IsDeleted, 0) = 0 AND aa.UserDetailsId = ud.Id)
					FOR XML PATH ('')
					), 2, 1000) TeamIds
	FROM dbo.UserDetails ud 
	INNER JOIN dbo.AspNetUsers au ON ud.UserId = au.Id
	LEFT JOIN AspNetRoles ur ON ud.[Role] = ur.RoleCode
	WHERE ISNULL(ud.IsDeleted, 0) = 0 AND ISNULL(ud.IsLocked, 0) =  0

GO


