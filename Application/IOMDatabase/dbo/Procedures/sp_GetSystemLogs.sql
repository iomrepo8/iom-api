CREATE PROCEDURE [dbo].[sp_GetSystemLogs]
	 @dateFrom varchar(50)
	,@dateTo varchar(50)
AS
	SELECT sl.Id
		, StaffId =  CAST(ud.[StaffId] AS varchar(100))
		, sl.ActorUserId
        , Entity = UPPER(sl.Entity)
        , sl.LogDate
		, sl.RequestBody
        , ElapsedTime = CAST(sl.ElapseTime AS VARCHAR) + ' ms'		        
		, ActionType = TRIM(STUFF((SELECT UPPER(value) + ' ' FROM STRING_SPLIT(sl.ActionType, '_') FOR XML PATH('')), 1, 0, ''))        
        , EODEmailReference
		, ActorName = CAST(ud.[Name] AS varchar(100))
        , ActorRole = CAST(ar.[Name] AS varchar(100))
		, sl.IPAddress
        , (SELECT 
				COALESCE(a.Id, 0) AS Id
				, COALESCE(a.[Name], '') AS [Name]
			FROM dbo.AccountMember am
				INNER JOIN dbo.Accounts a ON am.AccountId = a.Id
			WHERE 
				am.UserDetailsId = ud.Id
				AND ISNULL(am.IsDeleted, 0) = 0
				AND ud.[Role] <> 'SA'
			GROUP BY a.Id, a.[Name]
			FOR JSON AUTO) [Accounts]
		, (SELECT 
				COALESCE(t.Id, 0) AS Id
				, COALESCE(t.[Name], '') AS [Name] 
			FROM dbo.TeamMember tm
				INNER JOIN dbo.Teams t ON tm.TeamId = t.Id
			WHERE 
				tm.UserDetailsId = ud.Id
				AND ISNULL(tm.IsDeleted, 0) = 0
				AND ud.[Role] IN ('CL', 'TM', 'LA', 'AG')
			GROUP BY t.Id, t.[Name]
			FOR JSON PATH) [Teams]
		
        FROM SystemLog sl
        JOIN UserDetails ud on sl.ActorUserId = ud.Id
        JOIN AspNetRoles ar on ud.[Role] = ar.RoleCode
        WHERE sl.LogDate >= @dateFrom
			 AND sl.LogDate <= @dateTo

		FOR JSON AUTO