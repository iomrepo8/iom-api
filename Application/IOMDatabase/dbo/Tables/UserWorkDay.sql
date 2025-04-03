CREATE TABLE [dbo].[UserWorkDay]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserDetailsId] INT NOT NULL, 
    [DayIndex] TINYINT NOT NULL, 
    [Day] VARCHAR(10) NOT NULL, 
    [CreatedDate] DATETIME NOT NULL DEFAULT getutcdate(), 
    [UpdatedDate] DATETIME NOT NULL DEFAULT getutcdate(), 
    [UpdatedBy] INT NULL, 
    CONSTRAINT [FK_UserWorkDay_UserDetails] FOREIGN KEY (UserDetailsId) REFERENCES UserDetails(Id)
)
