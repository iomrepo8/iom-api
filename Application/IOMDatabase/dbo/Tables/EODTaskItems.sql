CREATE TABLE [dbo].[EODTaskItems]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TaskId] INT NOT NULL, 
    [TotalTaskHours] DECIMAL(10, 2) NOT NULL, 
    [AdjustedTotalHours] DECIMAL(10, 2) NOT NULL, 
    [EODReportId] INT NOT NULL, 
    [IsEdited] BIT NULL, 
    [IsRemoved] BIT NULL, 
    [IsInserted] BIT NULL
)
