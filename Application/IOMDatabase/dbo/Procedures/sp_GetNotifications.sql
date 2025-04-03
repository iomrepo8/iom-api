CREATE PROCEDURE [dbo].[sp_GetNotifications]
	@dateFrom varchar(50)
	,@dateTo varchar(50)
	,@userDetailsId INT
AS
	SELECT n.Id
		, n.NoteDate
		, n.[Message]
		, n.IsRead
		, n.IsArchived
		, n.Icon
		, n.Title
		, n.NoteType
	FROM dbo.[Notification] n
	WHERE n.ToUserId = @userDetailsId AND n.NoteDate >= @dateFrom
		AND n.NoteDate  <= DATEADD(SECOND, 59, DATEADD(MINUTE, 59, DATEADD(HOUR, 23, CAST(@dateTo AS DATETIME))))
	ORDER BY n.NoteDate DESC
