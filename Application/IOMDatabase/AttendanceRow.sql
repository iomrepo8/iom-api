CREATE TABLE [dbo].[AttendanceRow]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Hours] FLOAT NOT NULL, 
    [AttendanceTag] NVARCHAR(50) NULL, 
    [CreatedDate] DATETIME NULL, 
    [CreatedBy] NVARCHAR(128) NULL, 
    [UpdatedDate] DATETIME NULL, 
    [UpdatedBy] NVARCHAR(128) NULL, 
    [AttendanceId] INT NOT NULL, 
    CONSTRAINT [FK_AttendanceRow_ToAttendance] FOREIGN KEY ([AttendanceId]) REFERENCES [Attendance]([Id])
)
