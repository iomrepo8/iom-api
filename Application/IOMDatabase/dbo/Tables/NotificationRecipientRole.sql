CREATE TABLE [dbo].[NotificationRecipientRole]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [NotificationSettingId] INT NOT NULL,
    [RoleId] NVARCHAR(128) NOT NULL,
    [CreatedDate] DATETIME NULL DEFAULT GETUTCDATE(), 
    [CreatedBy] INT NULL, 
    [LastUpdated] DATETIME NULL DEFAULT GETUTCDATE(), 
    [UpdatedBy] INT NULL, 
    CONSTRAINT [FK_NotificationRecipientRole_NotificationSetting] FOREIGN KEY (NotificationSettingId) REFERENCES [NotificationSetting](Id), 
    CONSTRAINT [FK_NotificationRecipientRole_Role] FOREIGN KEY (RoleId) REFERENCES [AspNetRoles]([Id])

    
)
