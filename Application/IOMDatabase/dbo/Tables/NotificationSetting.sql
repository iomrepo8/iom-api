CREATE TABLE [dbo].[NotificationSetting]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(30) NOT NULL, 
    [Action] VARCHAR(250) NULL, 
    [Subject] VARCHAR(250) NULL, 
    [Message] VARCHAR(MAX) NULL, 
    [Type] VARCHAR(50) NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [CreatedBy] INT NULL, 
    [LastUpdated] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [UpdatedBy] INT NULL, 
    
)
