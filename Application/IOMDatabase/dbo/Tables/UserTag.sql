CREATE TABLE [dbo].[UserTag]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserDetailsId] INT NOT NULL, 
    [TagId] INT NOT NULL, 
    [DateCreated] DATETIME NOT NULL DEFAULT GETUTCDATE(), 
    [AttendanceDate] DATE NULL, 
    [Hours] FLOAT NULL, 
    CONSTRAINT [FK_UserTag_Tag] FOREIGN KEY (TagId) REFERENCES Tag(Id), 
    CONSTRAINT [FK_UserTag_UserDetails] FOREIGN KEY (UserDetailsId) REFERENCES UserDetails(Id) 
)
