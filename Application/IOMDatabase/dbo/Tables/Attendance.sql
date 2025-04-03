CREATE TABLE [dbo].[Attendance]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserDetailsId] INT NOT NULL, 
    [AttendanceDate] DATE NOT NULL, 
    [WorkedHours] FLOAT NOT NULL, 
    [WorkedDay] FLOAT NOT NULL, 
    [CreatedDate] DATETIME NULL, 
    [CreatedBy] NVARCHAR(128) NULL, 
    [UpdatedDate] DATETIME NULL, 
    [UpdatedBy] NVARCHAR(128) NULL, 
    [TotalHours] FLOAT NULL, 
    [StartTime] DATETIME NULL, 
    [EndTime] DATETIME NULL, 
    CONSTRAINT [FK_Attendance_UserDetails] FOREIGN KEY (UserDetailsId) REFERENCES UserDetails(Id)
)
