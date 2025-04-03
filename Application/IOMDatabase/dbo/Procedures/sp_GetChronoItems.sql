CREATE PROCEDURE [dbo].[sp_GetChronoItems]
    @userId int = 0,
    @date varchar(50)
AS
     SELECT 
		  th.Id		
		, th.HistoryDate
		, th.TaskHistoryTypeId
		, tc.Comment

		-- This is a quick fix. Clean this if given opportunity.
		, CASE 
			WHEN th.TaskHistoryTypeId IN (8) THEN 'ApprovedEOD'
			WHEN th.TaskHistoryTypeId IN (9) THEN 'Task'
			ELSE tht.[Name] END 
			AS TaskTypeName	
		, th.UserDetailsId
		, CASE 
			WHEN th.TaskHistoryTypeId = 9 THEN 'AVAIL'
			ELSE iomt.[Description] END 
			AS TaskDescription
		, CASE 
			WHEN th.TaskHistoryTypeId = 9 THEN 'AVAIL'
			ELSE iomt.[Name] END 
			AS TaskName
		, StartTime = th.ActivateTime
		, CASE WHEN th.TaskHistoryTypeId = 7 THEN null ELSE DATEADD(MINUTE, th.Duration, th.ActivateTime) END AS EndTime

		, CASE WHEN th.TaskHistoryTypeId = 7 
			THEN 0.0 
			ELSE CONVERT(DECIMAL(10, 2), (th.Duration / 60)) 
			END As Duration
	FROM TaskHistory th
	LEFT JOIN IOMTasks iomt on th.TaskId = iomt.Id
	LEFT JOIN TaskComment tc ON th.Id = tc.TaskHistoryId
	INNER JOIN TaskHistoryType tht ON th.TaskHistoryTypeId = tht.Id
	WHERE th.UserDetailsId = @userId AND th.HistoryDate = @date
	ORDER BY th.ActivateTime ASC
RETURN 0
