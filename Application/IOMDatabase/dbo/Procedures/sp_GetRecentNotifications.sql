CREATE PROCEDURE [dbo].[sp_GetRecentNotifications] @userDetailsId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @currentDateTime DATETIME = GETUTCDATE();

    SELECT *
    FROM dbo.[Notification]
    WHERE ToUserId = @userDetailsId
      AND NoteDate <= @currentDateTime
      AND NoteDate
        > DATEADD(MINUTE
              , -5
              , @currentDateTime)
      AND IsDisplayed = 0;
END
