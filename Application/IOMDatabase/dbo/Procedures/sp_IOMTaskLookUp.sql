CREATE PROCEDURE sp_IOMTaskLookUp
	
AS
BEGIN
	select 
			IOMTasks.Id as TaskId
		,	IOMTasks.Name as TaskName
		,	IOMTasks.Description as TaskDescription
		,	ISNULL(a.Id, 0) as AccountId
		,	ISNULL(a.Name, '') as AccountName
		,	ISNULL(t.Id, 0) as TeamId
		,	ISNULL(t.Name, '') as TeamName
		,	IOMTasks.IsActive as IsActiveTask
	from IOMTasks
		LEFT JOIN IOMTeamTask tt on IOMTasks.Id = tt.TaskId
		LEFT JOIN Teams t on tt.TeamId = t.Id
		LEFT JOIN Accounts a on t.AccountId = a.Id 
	where ISNULL(IOMTasks.IsActive, 0) = 1 AND ISNULL(IOMTasks.IsDeleted, 0) = 0
END
GO