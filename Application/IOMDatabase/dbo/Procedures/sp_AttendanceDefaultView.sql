
CREATE PROCEDURE sp_AttendanceDefaultView 
(
	@dateFrom varchar(50)
	,@dateTo varchar(50)
) AS 
BEGIN
	DECLARE @cols AS NVARCHAR(MAX),
			@query  AS NVARCHAR(MAX);

	SELECT @cols = STUFF((SELECT ',' + QUOTENAME(dv.HistoryDate) 
						FROM dbo.vw_AttendanceDefaultView dv
						WHERE HistoryDate >= @dateFrom
						AND HistoryDate <= @dateTo
						GROUP BY dv.HistoryDate
						ORDER BY dv.HistoryDate
				FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)') 
			,1,1,'')

	SET @query = 'SELECT UserUniqueId, UserDetailId, Fullname, FirstName, LastName, [Role], AccountIds, AccountNames, ' + @cols + ' FROM 
					(
					SELECT *
					FROM dbo.vw_AttendanceDefaultView dv	
					INNER JOIN (SELECT
								u.UserId AS UserUniqueId ,u.Id AS UserDetailId ,RTRIM(LTRIM(u.[Name])) AS Fullname
							,FirstName = RTRIM(LTRIM(u.FirstName))  ,LastName = RTRIM(LTRIM(u.LastName)) ,u.[Role]
							, SUBSTRING(
										(
										SELECT '','' + a.[Name] AS [text()] 
										FROM dbo.Accounts a 
										INNER JOIN dbo.AccountAgTs aat ON a.Id = aat.AccountId
										WHERE aat.UserDetailsId = u.Id
										ORDER BY a.[Name]
										FOR XML PATH ('''')), 2, 1000) AS AccountNames
							, SUBSTRING(
										(
										SELECT '',''+ CONVERT(VARCHAR, a.[Id]) AS [text()] 
										FROM dbo.Accounts a 
										INNER JOIN dbo.AccountAgTs aat ON a.Id = aat.AccountId
										WHERE aat.UserDetailsId = u.Id
										ORDER BY a.[Name]
										FOR XML PATH ('''')), 2, 1000) AS AccountIds	
						FROM UserDetails u
						WHERE u.IsDeleted = 0 AND (u.[Role] = ''AG'' OR u.[Role] = ''TS'')) ui ON dv.UserDetailsId = ui.UserDetailId
					) x
				pivot 
				(
					SUM(TotalActiveTime)
					for HistoryDate in (' + @cols + ')
				) p ORDER BY UserDetailsId'

	EXECUTE(@query);
END