CREATE PROCEDURE [dbo].[sp_GetSentEODList]
		@dateFrom varchar(50)
	,	@dateTo varchar(50)
	,	@withActionOnly bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		
	SELECT DISTINCT
			eod.Id
		,	eod.EODDate
		,	eod.UserId
		,	ISNULL(ud.Id, 0) as UserDetailsId
		,	ud.[Role] as UserRole
		,	ud.[Name] as EODOwner
		,	eod.Note
		,	eod.IsConfirmed
		,	CASE 
				WHEN eod.ConfirmedUTCDate IS NOT NULL THEN 'Confirmed'
				ELSE 'No Action'
			END as EODAction
		,	eod.ConfirmedUTCDate
		,	approver.FirstName + ' ' + approver.LastName AS ApproverFullname
		,   ISNULL(a.Id, 0) as AccountId
		,   ISNULL(a.Name, '') as AccountName
		,	ISNULL(t.Id, 0) as TeamId
		,	ISNULL(t.Name, '') as TeamName 
	FROM dbo.EODReports eod
		LEFT join dbo.EODTaskItems i ON eod.Id = i.EODReportId
		left JOIN IOMTeamTask tt ON i.TaskId = tt.Id
		LEFT JOIN IOMTasks iomt on tt.TaskId = iomt.Id
        LEFT JOIN Teams t ON tt.TeamId = t.Id
        LEFT JOIN Accounts a ON t.AccountId = a.Id
		LEFT join dbo.UserDetails ud ON eod.UserId = ud.UserId
		LEFT join dbo.UserDetails approver ON eod.ConfirmedBy = approver.Id
	WHERE 
			(i.IsEdited = 1 OR i.IsRemoved = 1 OR	i.IsInserted = 1)
		AND	eod.EODDate >= @dateFrom
		AND eod.EODDate <= @dateTo
		AND CASE 
				WHEN @withActionOnly = 1 AND eod.ConfirmedUTCDate IS NULL THEN 1
				WHEN @withActionOnly = 0 THEN 1 
				ELSE 0 
			END = 1
	ORDER BY eod.IsConfirmed ASC

END
