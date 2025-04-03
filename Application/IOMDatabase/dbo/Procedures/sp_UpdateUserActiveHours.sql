CREATE PROCEDURE [dbo].[sp_UpdateUserActiveHours] (
	@dateNow AS DATETIME
)

AS
    WITH activeUsers AS (
        SELECT th.Id
            , 'UserDetailId' = ud.Id
            , 'NetUserId' = au.Id
            , th.[Start]
            , th.HistoryDate
		FROM 
            TaskHistory th 
		    INNER JOIN UserDetails ud ON th.UserDetailsId = ud.Id
		    INNER JOIN AspNetUsers au ON ud.UserId = au.Id

		WHERE 
				th.IsActive = 1
            AND th.HistoryDate = DATEFROMPARTS(DATEPART(yy, @dateNow), DATEPART(mm, @dateNow), DATEPART(d, @dateNow))
			AND DATEDIFF(minute, au.StatusUpdateDT, @dateNow) < 360
    )

    UPDATE th 
	SET 
        th.Duration = COALESCE(th.Duration, 0) + DATEDIFF(MINUTE, th.[Start], @dateNow),
		th.[Start] = @dateNow
	FROM 
        TaskHistory th 
		INNER JOIN activeUsers u ON th.Id = u.Id
    WHERE th.HistoryDate = DATEFROMPARTS(DATEPART(yy, @dateNow), DATEPART(mm, @dateNow), DATEPART(d, @dateNow))

RETURN 0
