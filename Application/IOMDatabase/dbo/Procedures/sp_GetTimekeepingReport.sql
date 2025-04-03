CREATE PROCEDURE [dbo].[sp_GetTimekeepingReport]
     @dateFrom varchar(50)
	,@dateTo varchar(50)
AS
    SELECT 
			th.UserDetailsId 
		,	CONVERT(DECIMAL(10,2), SUM(th.Duration) / 60) as TotalActiveTime
		,	ISNULL(th.TaskId, -1) as TaskId
		,	th.TaskHistoryTypeId
		,	CASE WHEN th.TaskHistoryTypeId = 9 THEN CAST(1 as bit) ELSE tt.IsActive END as IsTaskActive
		,	RTRIM(LTRIM(ISNULL(tt.Name, 'AVAIL'))) AS TaskName
		,	REPLACE(REPLACE(ISNULL(tt.[Description], 'AVAIL'), CHAR(13), ''), CHAR(10), '') AS TaskDescription
	FROM TaskHistory th
		LEFT JOIN dbo.IOMTasks tt ON th.TaskId = tt.Id
	WHERE 
			HistoryDate >= @dateFrom
		AND HistoryDate <= @dateTo
		AND th.TaskHistoryTypeId IN (1,8,9)
	GROUP BY th.UserDetailsId, th.TaskId, th.TaskHistoryTypeId, tt.[Name], tt.[Description], tt.IsActive
