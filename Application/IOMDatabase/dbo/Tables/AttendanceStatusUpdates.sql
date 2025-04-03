CREATE TABLE [dbo].[AttendanceStatusUpdates]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CreatedDate] DATETIME NOT NULL, 
    [CreatedBy] NVARCHAR(150) NOT NULL, 
    [OldStatus] VARCHAR(100) NULL, 
    [NewStatus] VARCHAR(100) NOT NULL, 
    [UserDetailsId] INT NOT NULL, 
    [StatusDate] VARCHAR(50) NULL
)
