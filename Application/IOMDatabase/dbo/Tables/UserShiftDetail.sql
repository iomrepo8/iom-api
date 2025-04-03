CREATE TABLE [dbo].[UserShiftDetail]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserDetailsId] INT NOT NULL, 
    [ShiftStart] TIME NULL, 
    [ShiftEnd] TIME NULL, 
    [LunchBreak] FLOAT NOT NULL DEFAULT 60, 
    [PaidBreaks] TINYINT NOT NULL DEFAULT 3, 
    [CreatedDate] DATETIME NULL DEFAULT getutcdate(), 
    [UpdatedDate] DATETIME NULL DEFAULT getutcdate(), 
    [UpdatedBy] INT NULL, 
    CONSTRAINT [FK_UserShiftDetail_UserDetails] FOREIGN KEY (UserDetailsId) REFERENCES [UserDetails](Id)
)