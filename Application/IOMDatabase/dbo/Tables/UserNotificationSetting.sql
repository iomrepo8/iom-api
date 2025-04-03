CREATE TABLE [dbo].[UserNotificationSetting]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserDetailsId] INT NOT NULL, 
    [NotificationTypeId] INT NOT NULL, 
    [CreatedDate] DATETIME NOT NULL DEFAULT getutcdate(), 
    [IsAllowed] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_UserNotificationSetting_UserDetail] FOREIGN KEY ([UserDetailsId]) REFERENCES [UserDetails]([Id]), 
    CONSTRAINT [FK_UserNotificationSetting_NotificationType] FOREIGN KEY ([NotificationTypeId]) REFERENCES [NotificationType]([Id])
)
