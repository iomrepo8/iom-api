CREATE VIEW [dbo].[vw_AttendanceDefaultView]
    AS 
SELECT TOP (100) PERCENT UserDetailsId, CONVERT(DECIMAL(10, 2), SUM(Duration) / 60) AS 'TotalActiveTime'
        , HistoryDate
        , CASE WHEN CONVERT(DECIMAL(10, 2), SUM(th.Duration) / 60) > 8 
                THEN 8 
                ELSE CONVERT(DECIMAL(10, 2), SUM(th.Duration) / 60) 
                END AS RegularHours
        , CASE WHEN CONVERT(DECIMAL(10, 2), SUM(th.Duration) / 60) > 8 
                THEN CONVERT(DECIMAL(10, 2), SUM(th.Duration) / 60) - 8 
                ELSE 0 
                END AS OTHours
FROM            dbo.TaskHistory AS th
WHERE        (TaskHistoryTypeId IN (1, 2, 8, 9))
GROUP BY UserDetailsId, HistoryDate
ORDER BY UserDetailsId, HistoryDate
