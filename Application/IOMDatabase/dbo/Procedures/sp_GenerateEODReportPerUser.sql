CREATE PROCEDURE [dbo].[sp_GenerateEODReportPerUser] 
	 @dateFrom VARCHAR(50)
	,@dateTo VARCHAR(50)
	,@userId VARCHAR(150)
AS
BEGIN
	DECLARE @role VARCHAR(10)
		,@roleDesc VARCHAR(50)
		,@name VARCHAR(200)
		,@userDetailsId INT;

	SELECT @role = [Role]
		,@name = Name
		,@userDetailsId = Id
	FROM UserDetails
	WHERE UserId = @userId;

	WITH totalHours
	AS (
		SELECT th.UserDetailsId
			,CONVERT(DECIMAL(10, 2), SUM(th.Duration) / 60) AS TotalActiveTime
			,ISNULL(th.TaskId, - 1) AS TaskId
			,th.TaskHistoryTypeId
		FROM TaskHistory th
		WHERE th.HistoryDate >= @dateFrom
			AND th.HistoryDate <= @dateTo
			AND th.TaskHistoryTypeId IN (1,8,9)
			AND th.UserDetailsId = @userDetailsId
		GROUP BY th.UserDetailsId
			,th.TaskId
			,th.TaskHistoryTypeId
		)
	SELECT
		--users data
		u.UserId AS 'UserUniqueId'
		,u.Id AS 'UserDetailId'
		,RTRIM(LTRIM(u.[Name])) AS 'Fullname'
		,FirstName = RTRIM(LTRIM(u.FirstName))
		,LastName = RTRIM(LTRIM(u.LastName))
		,CASE 
			WHEN u.[Role] = 'AM'
				THEN 'Account Manager'
			WHEN u.[Role] = 'AG'
				THEN 'Agent'
			WHEN u.[Role] = 'CL'
				THEN 'Client'
			WHEN u.[Role] = 'SA'
				THEN 'System Administrator'
			WHEN u.[Role] = 'LA'
				THEN 'Lead Agent'
			ELSE ''
			END AS UserRole
		-- Task data
		,RTRIM(LTRIM(ISNULL(iomt.[Name], 'AVAIL'))) AS 'TaskName'
		,REPLACE(REPLACE(ISNULL(iomt.[Description], 'AVAIL'), CHAR(13), ''), CHAR(10), '') AS 'TaskDescription'
		,th.TaskId AS 'TaskId'
		,iomt.IsActive AS 'IsTaskActive'
		,iomt.TaskNumber
		-- time data
		,th.UserDetailsId
		,th.TotalActiveTime
		,th.TaskHistoryTypeId
	FROM totalHours th
	LEFT JOIN IOMTasks iomt ON th.TaskId = iomt.Id
	LEFT JOIN UserDetails u ON th.UserDetailsId = u.Id
END
