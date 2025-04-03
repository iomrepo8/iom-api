CREATE TABLE [dbo].[Notification]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ToUserId] INT NOT NULL, 
    [NoteDate] DATETIME NOT NULL, 
    [IsRead] BIT NOT NULL DEFAULT 0, 
    [IsArchived] BIT NOT NULL DEFAULT 0, 
    [Message] NVARCHAR(MAX) NULL, 
    [ReadDate] DATETIME NULL, 
    [ArchiveDate] DATETIME NULL, 
    [LastUpdateBy] INT NULL, 
    [Title] VARCHAR(120) NULL, 
    [Icon] VARCHAR(100) NULL, 
    [NoteType] VARCHAR(50) NULL,
    [IsDisplayed] BIT DEFAULT ((0)) NOT NULL,
    [SenderId] INT NULL, 
    CONSTRAINT [FK_Notification_UserDetails] FOREIGN KEY ([ToUserId]) REFERENCES dbo.[UserDetails](Id), 
    CONSTRAINT [FK_Notification_Sender] FOREIGN KEY ([SenderId]) REFERENCES dbo.[UserDetails](Id)
)
