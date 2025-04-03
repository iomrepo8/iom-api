CREATE PROCEDURE [dbo].[sp_GetAttendanceDefault]
    @dateFrom varchar(50)
	,@dateTo varchar(50)
AS

SELECT a.*, ao.RegularHours AS 'UpdateRegHours', ao.OTHours AS 'UpdateOTHours'
FROM dbo.vw_AttendanceDefaultView a
LEFT JOIN dbo.AttendanceOTUpdates ao ON a.UserDetailsId = ao.UserDetailsId AND a.HistoryDate = ao.HistoryDate
WHERE a.HistoryDate >= @dateFrom
	AND a.HistoryDate <= @dateTo
