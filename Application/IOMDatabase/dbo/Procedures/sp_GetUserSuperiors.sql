CREATE PROCEDURE [dbo].[sp_GetUserSuperiors] @userId INT, @notificationTypeId INT
AS
DECLARE @userRole VARCHAR(2);

SELECT @userRole = [Role]
FROM dbo.UserDetails
WHERE Id = @userId;

SELECT ud.Id
     , ud.FirstName
     , ud.LastName
     , au.Email
     , ud.[Role]
     , n.IsAllowed
FROM dbo.AspNetUsers au
         INNER JOIN dbo.UserDetails ud ON au.Id = ud.UserId
         LEFT JOIN dbo.UserNotificationSetting n ON ud.Id = n.UserDetailsId
WHERE ud.[Role] = 'SA'
  AND ISNULL(ud.IsDeleted, 0) <> 1
  AND ISNULL(ud.IsLocked, 0) <> 1
  AND n.NotificationTypeId = @notificationTypeId

UNION

SELECT DISTINCT (ud.Id)
              , ud.FirstName
              , ud.LastName
              , au.Email
              , ud.[Role]
              , n.IsAllowed
FROM dbo.AccountMember a
         INNER JOIN dbo.UserDetails ud ON a.UserDetailsId = ud.Id
         INNER JOIN dbo.AspNetUsers au ON ud.UserId = au.Id
         CROSS APPLY (SELECT am.*
                      FROM dbo.AccountMember am
                               INNER JOIN dbo.Accounts ac ON am.AccountId = ac.Id
                      WHERE am.UserDetailsId = @userId
                        AND a.AccountId = am.AccountId
                        AND ISNULL(am.IsDeleted, 0) <> 1
                        AND ISNULL(ac.IsDeleted, 0) <> 1) x
         LEFT JOIN dbo.UserNotificationSetting n ON ud.Id = n.UserDetailsId
WHERE ud.[Role] = 'AM'
  AND ISNULL(a.IsDeleted, 0) <> 1
  AND n.NotificationTypeId = @notificationTypeId

UNION

SELECT DISTINCT (ud.Id)
              , ud.FirstName
              , ud.LastName
              , au.Email
              , ud.[Role]
              , n.IsAllowed
FROM dbo.TeamMember t
         INNER JOIN dbo.UserDetails ud ON t.UserDetailsId = ud.Id
         INNER JOIN dbo.AspNetUsers au ON ud.UserId = au.Id
         CROSS APPLY (SELECT tm.*
                      FROM dbo.TeamMember tm
                               INNER JOIN dbo.Teams te ON tm.TeamId = te.Id
                      WHERE tm.UserDetailsId = @userId
                        AND t.TeamId = tm.TeamId
                        AND ISNULL(te.IsDeleted, 0) <> 1
                        AND ISNULL(tm.IsDeleted, 0) <> 1) x
         LEFT JOIN dbo.UserNotificationSetting n ON ud.Id = n.UserDetailsId
WHERE ISNULL(t.IsDeleted, 0) <> 1
  AND (
        (@userRole = 'AG' AND ud.[Role] IN ('LA', 'TM'))
        OR (@userRole = 'LA' AND ud.[Role] IN ('TM'))
        OR (@userRole = 'TM' AND ud.[Role] IN (''))
    )
  AND n.NotificationTypeId = @notificationTypeId
