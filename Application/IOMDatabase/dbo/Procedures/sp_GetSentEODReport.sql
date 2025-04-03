CREATE PROCEDURE [dbo].[sp_GetSentEODReport]
	@eodId int
AS
	SELECT 
			e.Id
		,	e.UserId
		,	e.Note
		,	e.EODDate
		,	e.SentUTCDateTime
		,	e.IsConfirmed as EODIsConfirmed
		,	e.IsEdited as HasEditItems
		,	e.ConfirmedUTCDate
		,	e.ConfirmedBy
		,	ud.Name as LastUpdateName
		,	ud.[Role] as LastUpdateRole
		,	uud.Id as UserDetailsId
		,	ei.TotalTaskHours
		,	ei.AdjustedTotalHours
		,	ei.IsEdited
		,	ei.IsRemoved
		,	ei.IsInserted
		,	ei.TaskId as TaskId
		,	CASE WHEN ei.TaskId = -1 THEN 'AVAIL' ELSE iomt.[Name] END as TaskName
		,	CASE WHEN ei.TaskId = -1 THEN 'AVAIL' ELSE iomt.[Description] END as TaskDescription
	FROM EODReports e
		LEFT JOIN EODTaskItems ei ON e.Id = ei.EODReportId
		LEFT JOIN IOMTasks iomt on ei.TaskId = iomt.Id
		LEFT JOIN UserDetails ud ON e.ConfirmedBy = ud.Id
		LEFT JOIN UserDetails uud ON e.UserId = uud.UserId
	WHERE e.Id = @eodId
	ORDER BY TaskId DESC

RETURN 0