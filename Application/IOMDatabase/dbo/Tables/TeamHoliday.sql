CREATE TABLE [dbo].[TeamHolidays]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TeamId] INT NOT NULL, 
    [HolidayDate] DATE NOT NULL, 
    [CreateDate] DATETIME NULL, 
    [CreatedBy] VARBINARY(50) NULL, 
    [LastUpdateDate] DATETIME NULL, 
    [UpdatedBy] VARCHAR(50) NULL, 
    [Description] VARCHAR(250) NULL, 
    [Title] VARCHAR(100) NULL, 
    [IsDeleted] BIT NULL, 
    CONSTRAINT [FK_TeamHolidays_Teams] FOREIGN KEY ([TeamId]) REFERENCES [Teams]([Id])
)
