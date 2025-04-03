CREATE VIEW [dbo].[vw_TaskClocker]
AS
SELECT u.UserId AS UserUniqueId
	,u.Id AS UserDetailsId
	,u.StaffId
	,RTRIM(LTRIM(u.[Name])) AS Fullname
	,RTRIM(LTRIM(u.FirstName)) AS FirstName
	,RTRIM(LTRIM(u.LastName)) AS LastName
	,u.[Role]
	,r.[Name] AS RoleName
	,u.[Image]
	,au.Email
	,SUBSTRING((
			SELECT ',' + CONVERT(VARCHAR, a.[Id]) AS [text()]
			FROM dbo.Accounts a
			INNER JOIN dbo.AccountMember aat ON a.Id = aat.AccountId
			WHERE aat.UserDetailsId = u.Id
				AND a.IsActive = 1
				AND ISNULL(aat.IsDeleted, 0) = 0
			ORDER BY a.[Name]
			FOR XML PATH('')
			), 2, 1000) AS AccountIds
	,SUBSTRING((
			SELECT ',' + CONVERT(VARCHAR, t.[Id]) AS [text()]
			FROM dbo.Teams t
			INNER JOIN dbo.TeamMember ts ON t.Id = ts.TeamId
			WHERE ts.UserDetailsId = u.Id
				AND t.IsActive = 1
				AND ISNULL(ts.IsDeleted, 0) = 0
				AND t.AccountId IN (
					SELECT aa.AccountId
					FROM dbo.AccountMember aa
					WHERE ISNULL(aa.IsDeleted, 0) = 0
						AND aa.UserDetailsId = u.Id
					)
			FOR XML PATH('')
			), 2, 1000) AS TeamIds
FROM UserDetails u
INNER JOIN dbo.AspNetUsers au ON u.UserId = au.Id
INNER JOIN dbo.AspNetRoles r ON u.[Role] = r.RoleCode

WHERE u.IsDeleted = 0
	AND u.[Role] IN (
		'AG'
		,'LA'
		,'TM'
		)
	AND ISNULL(u.islocked, 0) = 0
GO


