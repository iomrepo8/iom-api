CREATE TABLE [dbo].[TeamClientPOC]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TeamId] INT NOT NULL, 
    [UserDetailsId] INT NOT NULL, 
    [IsDeleted] BIT NULL, 
    CONSTRAINT [FK_TeamClientPOC_Team] FOREIGN KEY (TeamId) REFERENCES Teams(Id), 
    CONSTRAINT [FK_TeamClientPOC_UserDetail] FOREIGN KEY (UserDetailsId) REFERENCES UserDetails(Id)
)
