CREATE PROCEDURE [dbo].[sp_CollectTimeLogs]
AS
		WITH attendanceData AS (
            SELECT 
                    t.UserDetailsId
                  , t.HistoryDate
                  , WorkHours = CONVERT(DECIMAL(10,2), SUM(COALESCE(t.Duration, 0)) / 60.0)
            FROM TaskHistory t
			JOIN UserDetails u ON t.UserDetailsId = u.Id
            WHERE 
				t.TaskHistoryTypeId = 1
            GROUP BY t.UserDetailsId, t.HistoryDate
		)

		INSERT INTO dbo.Attendance (UserDetailsId, AttendanceDate, WorkedHours, WorkedDay, CreatedDate, CreatedBy)

        SELECT 
			  ad.UserDetailsId
	        , AttendanceDate = ad.HistoryDate
	        , WorkedHours = ad.WorkHours
	        , WorkedDay = COALESCE(CASE 
			        WHEN WorkHours < 1.0 OR WorkHours IS NULL THEN 0
			        WHEN WorkHours > 1.0 AND WorkHours < 2.0 THEN 0.25
			        WHEN WorkHours > 2.1 AND WorkHours < 4.0 THEN 0.50
			        WHEN WorkHours > 4.1 AND WorkHours < 6.0 THEN 0.75
			        WHEN WorkHours > 6.0 THEN 1 END, 0.0)
			, GETDATE()
			, 'System'
        FROM attendanceData ad
		WHERE NOT EXISTS(
				SELECT * 
				FROM Attendance a 
				WHERE a.AttendanceDate = ad.HistoryDate AND a.UserDetailsId = ad.UserDetailsId)
RETURN 0
