CREATE TABLE [dbo].[UserImageFile]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserDetailsId] INT NOT NULL, 
    [ImageUri] VARCHAR(500) NOT NULL, 
    [IsActive] BIT NOT NULL DEFAULT 0, 
    [CreateDate] DATETIME NULL, 
    [CreatedBy] VARCHAR(50) NULL, 
    CONSTRAINT [FK_UserImageFile_ToTable] FOREIGN KEY (UserDetailsId) REFERENCES UserDetails(Id)
)
