CREATE TABLE [dbo].[UserDayOff]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserDetailsId] INT NOT NULL, 
    [Day] VARCHAR(50) NOT NULL, 
    [NumericDay] INT NOT NULL, 
    CONSTRAINT [FK_UserDayOff_ToTable] FOREIGN KEY (UserDetailsId) REFERENCES UserDetails(Id)
)
