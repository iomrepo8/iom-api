CREATE TABLE [dbo].[TeamDayOff]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TeamId] INT NOT NULL, 
    [Day] VARCHAR(50) NOT NULL, 
    [CreateDate] DATETIME NULL, 
    [CreatedBy] VARCHAR(50) NULL, 
    [LastUpdateDate] DATETIME NULL, 
    [UpdatedBy] VARCHAR(50) NULL, 
    [IsDeleted] BIT NULL, 
    CONSTRAINT [FK_TeamDayOff_Teams] FOREIGN KEY (TeamId) REFERENCES Teams([Id])	
)
