CREATE TABLE [dbo].[SystemLog]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [LogDate] DATETIME NOT NULL, 
    [ActorUserId] INT NOT NULL, 
    [RawUrl] VARCHAR(250) NULL, 
    [ActionType] VARCHAR(50) NULL, 
    [Description] VARCHAR(500) NULL, 
    [Entity] VARCHAR(100) NULL, 
    [BrowserUsed] VARCHAR(100) NULL, 
    [IPAddress] VARCHAR(50) NULL, 
    [ElapseTime] DECIMAL(7) NULL, 
    [ResponseStatusCode] INT NULL, 
    [UrlParams] NVARCHAR(500) NULL, 
    [RequestBody] NVARCHAR(MAX) NULL, 
    [Note] NVARCHAR(250) NULL, 
    [EODEmailReference] NVARCHAR(50) NULL 
)
