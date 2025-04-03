CREATE TABLE [dbo].[EODReports]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] NVARCHAR(128) NOT NULL, 
    [Note] NVARCHAR(MAX) NULL, 
    [EODDate] DATE NOT NULL, 
    [SentUTCDateTime] DATETIME NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [ClientOffset] FLOAT NULL, 
    [IsConfirmed] BIT NULL, 
    [IsEdited] BIT NULL, 
    [ConfirmedUTCDate] DATETIME NULL, 
    [ConfirmedBy] INT NULL,
    [Recipients] [nvarchar](max) NULL
)
