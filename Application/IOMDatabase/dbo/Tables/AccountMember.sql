CREATE TABLE [dbo].[AccountMember]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserDetailsId] INT NOT NULL, 
    [AccountId] INT NOT NULL, 
    [Created] DATETIME NULL DEFAULT getutcdate(), 
    [IsDeleted] BIT NOT NULL DEFAULT ((0)), 
    [IsNameMasked] BIT NOT NULL DEFAULT ((0)),
    CONSTRAINT [FK_AccountMember_UserDetails] FOREIGN KEY (UserDetailsId) REFERENCES UserDetails(Id), 
    CONSTRAINT [FK_AccountMember_Account] FOREIGN KEY (AccountId) REFERENCES Accounts(Id)
)
