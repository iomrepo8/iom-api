CREATE PROCEDURE [dbo].[sp_KickOutAllUsers](
	@dateNow AS DATETIME
)

AS		
    CREATE table #activeUserIds 
    (
        UserDetailsId int,
        HistoryDate date,
        [Start] datetime,
        Duration decimal,
        IsActive bit,
        TaskHistoryTypeId int,
        ActivateTime datetime
    );

    INSERT INTO #activeUserIds 
    SELECT 
            th.UserDetailsId
          , HistoryDate = @dateNow
          , [Start] = @dateNow
          , Duration = 0
          , IsActive = 0
          , TaskHistoryTypeId = 7
          , ActivateTime = @dateNow
    FROM TaskHistory th
    WHERE 
            th.IsActive = 1
        AND th.HistoryDate = DATEFROMPARTS(DATEPART(yy, @dateNow), DATEPART(mm, @dateNow), DATEPART(d, @dateNow));
    
    WITH activeUsers AS (
        SELECT th.Id
            , 'UserDetailsId' = ud.Id
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
    )

    UPDATE th 
	SET 
		th.IsActive = 0
	FROM 
        TaskHistory th 
		JOIN activeUsers u ON th.UserDetailsId = u.UserDetailsId
	WHERE th.HistoryDate = DATEFROMPARTS(DATEPART(yy, @dateNow), DATEPART(mm, @dateNow), DATEPART(d, @dateNow));

    INSERT INTO TaskHistory(UserDetailsId, HistoryDate, [Start], Duration, IsActive, TaskHistoryTypeId, ActivateTime)
    SELECT * FROM #activeUserIds;

    If(OBJECT_ID('tempdb..#activeUserIds') Is Not Null)
    Begin
        Drop Table #activeUserIds
    End

RETURN 0
