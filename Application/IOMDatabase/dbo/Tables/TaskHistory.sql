CREATE TABLE [dbo].[TaskHistory]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TaskId] INT NULL, 
    [UserDetailsId] INT NOT NULL, 
    [HistoryDate] DATE NOT NULL DEFAULT GetDate(), 
    [Start] DATETIME NULL, 
    [Duration] DECIMAL(10, 2) NULL, 
    [IsActive] BIT NOT NULL DEFAULT 0, 
    [TaskHistoryTypeId] INT NOT NULL, 
    [ActivateTime] DATETIME NULL, 
    CONSTRAINT [FK_TaskHistory_ToTable] FOREIGN KEY (UserDetailsId) REFERENCES UserDetails(Id), 
    CONSTRAINT [FK_TaskHistory_ToTable_1] FOREIGN KEY (TaskHistoryTypeId) REFERENCES TaskHistoryType(Id)
)
