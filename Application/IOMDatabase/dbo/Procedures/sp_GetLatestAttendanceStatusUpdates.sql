CREATE PROCEDURE [dbo].[sp_GetLatestAttendanceStatusUpdates]
	 @dateFrom varchar(50),
	 @dateTo varchar(50),
	 @userId varchar(50),
	 @userDetailsId int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE 
		@role varchar(50) = '';

	select @role = ud.Role, @userdetailsid = ud.Id 
	from AspNetUsers asp
	left join  UserDetails ud on asp.Id = ud.UserId
	where asp.Id = @userId;

	SELECT 
			MAX(OldStatus) as OldStatus
		,	MAX(NewStatus) as NewStatus
		,	at.UserDetailsId
		,	at.StatusDate
		,	RTRIM(LTRIM(u.[Name])) AS Fullname
		,	RTRIM(LTRIM(u.FirstName)) AS FirstName
		,	RTRIM(LTRIM(u.LastName)) AS LastName
		,	u.[Role]
	FROM [dbo].[AttendanceStatusUpdates] at
		LEFT JOIN UserDetails u ON UserDetailsId = u.Id
		INNER JOIN	
		(
			SELECT MAX(l.Id) as maxid, UserDetailsId, StatusDate 
			FROM AttendanceStatusUpdates l 
			WHERE 
				l.StatusDate >= @dateFrom
				AND l.StatusDate <= @dateTo
			GROUP BY UserDetailsId, StatusDate
		) mx
			ON mx.maxid = at.Id
	WHERE 
		at.StatusDate >= @dateFrom
		AND at.StatusDate <= @dateTo
	GROUP BY
		at.UserDetailsId, at.StatusDate, u.[Name], u.FirstName, u.LastName, u.[Role]
END