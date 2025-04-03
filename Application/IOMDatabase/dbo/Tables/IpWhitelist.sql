CREATE TABLE [dbo].[IpWhitelist]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [IPAddress] VARCHAR(20) NOT NULL, 
    [Alias] VARCHAR(50) NOT NULL, 
    [CreatedBy] INT NOT NULL, 
    [UpdatedBy] INT NULL, 
    [IsDeleted] BIT NOT NULL DEFAULT 0, 
    [CreatedDate] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [UpdatedDate] DATETIME NULL
)
