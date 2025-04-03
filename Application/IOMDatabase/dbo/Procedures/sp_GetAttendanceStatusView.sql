CREATE PROCEDURE [dbo].[sp_GetAttendanceStatusView]
	 @dateFrom varchar(50),
	 @dateTo varchar(50),
	 @userId varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE 
		@userdetailsid int,
		@role varchar(50) = '';

	select @role = ud.Role, @userdetailsid = ud.Id 
	from AspNetUsers asp
	left join  UserDetails ud on asp.Id = ud.UserId
	where asp.Id = @userId;

	SELECT 
			th.UserDetailsId 
		,	CONVERT(DECIMAL(10,2), SUM(th.Duration) / 60) as TotalHours
		,	th.HistoryDate
		,	u.UserId AS UserUniqueId
		,	RTRIM(LTRIM(u.[Name])) AS Fullname
		,	RTRIM(LTRIM(u.FirstName)) AS FirstName
		,	RTRIM(LTRIM(u.LastName)) AS LastName
		,	u.[Role]
	FROM 
		TaskHistory th
		LEFT JOIN UserDetails u ON th.UserDetailsId = u.Id
	WHERE 
			th.HistoryDate >= @dateFrom
		AND th.HistoryDate <= @dateTo
		AND th.TaskHistoryTypeId IN (1,8,9)
	GROUP BY 
		th.UserDetailsId, th.HistoryDate, 
		u.UserId, u.[Name], u.FirstName, u.LastName, u.[Role]
	ORDER BY HistoryDate
END