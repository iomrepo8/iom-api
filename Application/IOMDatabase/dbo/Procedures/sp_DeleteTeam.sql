CREATE PROCEDURE [dbo].[sp_DeleteTeam]
    @teamId int = 0
AS  

	update TeamTasks set IsDeleted = 1, IsActive = 0 where TeamId = @teamId;

	update TeamDayOff set IsDeleted = 1 where TeamId = @teamId;

	update TeamMember set IsDeleted = 1 where TeamId = @teamId;

	update TeamHolidays set IsDeleted = 1 where TeamId = @teamId;

	update Teams set IsDeleted = 1, IsActive = 0 where Id = @teamId;

RETURN 0