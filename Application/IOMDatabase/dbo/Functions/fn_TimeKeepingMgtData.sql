CREATE FUNCTION [dbo].[fn_TimeKeepingMgtData] (
	 @dateFrom varchar(50)
	,@dateTo varchar(50)
 )
RETURNS TABLE
AS
RETURN (
		WITH userInfo AS (
			SELECT
				-- Users data
				u.UserId AS 'UserUniqueId' ,u.Id AS 'UserDetailId' ,u.[Name] AS 'Fullname' ,u.FirstName ,u.LastName ,u.[Role], u.IsLocked
		
				-- Team data
				, t.Name AS 'TeamName' , t.Id AS 'TeamId' ,t.[Description] AS 'TeamDescription'
	
				-- Account data
				, a.Name AS 'AccountName' , a.Id AS 'AccountId' ,a.ContactPerson

				, RoleName = r.[Name]
		
			FROM UserDetails u
			INNER JOIN TeamMember tm ON u.Id = tm.UserDetailsId AND ISNULL(tm.IsDeleted, 0) = 0 AND ISNULL(u.IsDeleted, 0) = 0
			INNER JOIN AspNetRoles r ON u.[Role] = r.RoleCode
			INNER JOIN Teams t ON tm.TeamId = t.Id AND t.IsActive = 1
			INNER JOIN Accounts a ON t.AccountId = a.Id AND a.IsActive = 1
			WHERE u.[Role] = 'AG' 
				AND EXISTS(SELECT * FROM dbo.AccountMember am WHERE am.UserDetailsId = u.Id AND ISNULL(am.IsDeleted, 0) = 0)

			UNION

			SELECT
				-- Users data
				u.UserId AS 'UserUniqueId' ,u.Id AS 'UserDetailId' ,u.[Name] AS 'Fullname' ,u.FirstName ,u.LastName ,u.[Role], u.IsLocked
		
				-- Team data
				, t.Name AS 'TeamName' , t.Id AS 'TeamId' ,t.[Description] AS 'TeamDescription'
	
				-- Account data
				, a.Name AS 'AccountName' , a.Id AS 'AccountId' ,a.ContactPerson

				, RoleName = r.[Name]
		
			FROM UserDetails u
			JOIN TeamMember ts ON ts.UserDetailsId = u.Id AND ISNULL(ts.IsDeleted, 0) = 0 AND ISNULL(u.IsDeleted, 0) = 0
			JOIN AspNetRoles r ON u.[Role] = r.RoleCode
			LEFT JOIN Teams t ON ts.TeamId = t.Id AND t.IsActive = 1
			LEFT JOIN Accounts a ON t.AccountId = a.Id AND a.IsActive = 1
			WHERE u.[Role] IN ('LA', 'TM')
				AND EXISTS(SELECT * FROM dbo.AccountMember am WHERE am.UserDetailsId = u.Id AND ISNULL(am.IsDeleted, 0) = 0)

			UNION

			SELECT
				-- Users data
				u.UserId AS 'UserUniqueId' ,u.Id AS 'UserDetailId' ,u.[Name] AS 'Fullname' ,u.FirstName ,u.LastName ,u.[Role], u.IsLocked
		
				-- Team data
				, NULL AS 'TeamName' , NULL AS 'TeamId' ,NULL AS 'TeamDescription'
	
				-- Account data
				, a.Name AS 'AccountName' , a.Id AS 'AccountId' ,a.ContactPerson

				, RoleName = r.[Name]
		
			FROM dbo.UserDetails u
			JOIN dbo.AccountMember aa ON aa.UserDetailsId = u.Id AND ISNULL(aa.IsDeleted, 0) = 0 AND ISNULL(u.IsDeleted, 0) = 0
			JOIN dbo.AspNetRoles r ON u.[Role] = r.RoleCode
			LEFT JOIN dbo.Accounts a ON aa.AccountId = a.Id AND a.IsActive = 1
			WHERE u.[Role] IN ('LA', 'AG', 'TM')
		),
		
		time_details AS (
			SELECT t.UserDetailsId
				, 'TaskActiveTime' = 
					CASE WHEN t.TaskHistoryTypeId IN (1, 8, 9) THEN						
						CONVERT(DECIMAL(10,2), SUM(t.Duration) / 60.0)						
					ELSE 0 END

				, 'LunchActiveTime' = 
					CASE WHEN t.TaskHistoryTypeId = 3 THEN						
						CONVERT(DECIMAL(10,2), SUM(t.Duration) / 60.0)						
					ELSE 0 END

				, 'FirstBreakTime' = 
					CASE WHEN t.TaskHistoryTypeId = 4 THEN						
						CONVERT(DECIMAL(10,2), SUM(t.Duration) / 60.0)						
					ELSE 0 END

				, 'SecondBreakTime' = 
					CASE WHEN t.TaskHistoryTypeId = 5 THEN						
						CONVERT(DECIMAL(10,2), SUM(t.Duration) / 60.0)						
					ELSE 0 END

				, 'BioActiveTime' = 
					CASE WHEN t.TaskHistoryTypeId = 6 THEN						
						CONVERT(DECIMAL(10,2), SUM(t.Duration) / 60.0)						
					ELSE 0 END
				, 'AvailTime' = 
					CASE WHEN t.TaskHistoryTypeId = 9 THEN						
						CONVERT(DECIMAL(10,2), SUM(t.Duration) / 60.0)						
					ELSE 0 END

				, TeamId = te.Id, TeamName = te.[Name]
				, AccountId = a.Id, AccountName = a.[Name]
				, t.TaskId

			FROM TaskHistory t
				LEFT JOIN IOMTasks tt ON t.TaskId = tt.Id
				LEFT JOIN IOMTeamTask ttt on tt.Id = ttt.TaskId
				LEFT JOIN Teams te ON ttt.TeamId = te.Id
				LEFT JOIN Accounts a ON te.AccountId = a.Id
			WHERE t.HistoryDate >= @dateFrom
			 AND t.HistoryDate <= @dateTo
			GROUP BY t.TaskHistoryTypeId, t.UserDetailsId, te.Id, a.Id, te.[Name], a.[Name], t.TaskId
		),

		computed AS (
			SELECT UserDetailsId
				, TaskActiveTime = SUM(TaskActiveTime)
				, LunchActiveTime = SUM(LunchActiveTime)
				, FirstBreakTime = SUM(FirstBreakTime)
				, SecondBreakTime = SUM(SecondBreakTime)
				, BioActiveTime = SUM(BioActiveTime)
				, AvailTime = SUM(AvailTime)
			FROM time_details
			GROUP BY UserDetailsId
		),

		userStatus AS (
			SELECT t.UserDetailsId
				, 'Status' = CASE WHEN t.TaskHistoryTypeId = 1 THEN 'Active'
								  WHEN t.TaskHistoryTypeId = 3 THEN 'Lunch'
								  WHEN t.TaskHistoryTypeId = 4 THEN '1st Break'
								  WHEN t.TaskHistoryTypeId = 5 THEN '2nd Break'
								  WHEN t.TaskHistoryTypeId = 6 THEN 'Bio'
								  WHEN t.TaskHistoryTypeId = 9 THEN 'AVAIL'
								  ELSE 'Out' END
				, t.TaskHistoryTypeId
                , t.TaskId
                , TaskName = tt.[Name]
							 
			FROM  TaskHistory t
            LEFT JOIN IOMTasks tt ON t.TaskId = tt.Id
			WHERE t.HistoryDate >= @dateFrom
			 AND t.HistoryDate <= @dateTo
			 AND t.IsActive = 1
		),

		timekeepingDetails AS (
			SELECT th.UserDetailsId, EndTime = MAX(th.ActivateTime), StartTime = MIN(th.ActivateTime) FROM TaskHistory th
			WHERE th.HistoryDate >= @dateFrom
			 AND th.HistoryDate <= @dateTo
			GROUP BY th.UserDetailsId
		)

		SELECT DISTINCT(u.UserUniqueId), u.UserDetailId, u.FirstName, u.LastName, u.Fullname, u.[Role], u.RoleName, u.AccountId, u.AccountName, u.TeamId, u.TeamName, 'Status' = COALESCE(s.[Status], 'Out')	
			, _stat = s.[Status], u.IsLocked
			, 'TaskActiveTime' = COALESCE((c.TaskActiveTime), 0)
			, 'LunchActiveTime' = COALESCE((c.LunchActiveTime), 0)
			, 'FirstBreakTime' = COALESCE((c.FirstBreakTime), 0)
			, 'SecondBreakTime' = COALESCE((c.SecondBreakTime), 0)
			, 'BioActiveTime' = COALESCE((c.BioActiveTime), 0)
			, 'AvailTime' = COALESCE((c.AvailTime), 0)
            , TotalActiveHours = COALESCE((c.TaskActiveTime), 0)

			, 'IsActive' = CASE WHEN s.TaskHistoryTypeId in (1, 2, 9) THEN 1 ELSE 0 END
            , TaskId = s.TaskId
            , TaskName = s.TaskName
			, td.StartTime
			, td.EndTime
		FROM userInfo u
		LEFT JOIN computed c ON u.UserDetailId = c.UserDetailsId --AND u.AccountId = c.AccountId AND u.TeamId = c.TeamId
		LEFT JOIN userStatus s ON u.UserDetailId = s.UserDetailsId
		LEFT JOIN timekeepingDetails td ON u.UserDetailId = td.UserDetailsId
        WHERE u.AccountId IS NOT NULL OR u.TeamId IS NOT NULL			
		GROUP BY u.UserUniqueId, u.UserDetailId, u.FirstName, u.LastName, u.[Role], u.RoleName, u.Fullname, u.AccountId, u.AccountName, u.TeamId, u.TeamName, u.IsLocked, s.[Status], s.TaskHistoryTypeId
				, c.TaskActiveTime, c.LunchActiveTime, c.FirstBreakTime, c.SecondBreakTime, c.BioActiveTime, s.TaskId, s.TaskName, td.StartTime, td.EndTime, c.AvailTime
);