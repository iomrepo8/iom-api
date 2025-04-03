CREATE FUNCTION [dbo].[fn_GetAssignedTeams]
(
    @userDetailsId INT
)
RETURNS TABLE AS RETURN
(
	WITH userDetails AS (
		SELECT * FROM vw_ActiveUsers u WHERE u.UserDetailsId = @userDetailsId
	)

	SELECT tm.TeamId, t.AccountId 
	FROM dbo.TeamMember tm
		INNER JOIN dbo.Teams t ON tm.TeamId = t.Id
	WHERE tm.UserDetailsId = @userDetailsId 
		AND ISNULL(tm.IsDeleted, 0) = 0
		AND ISNULL(t.IsDeleted, 0) = 0 
		AND t.IsActive = 1
	UNION
	SELECT t.Id AS TeamId, t.AccountId 
	FROM dbo.Teams t
	WHERE t.IsActive = 1 AND ISNULL(t.IsDeleted, 0) = 0 
		AND EXISTS(SELECT 1 FROM userDetails WHERE [Role] = 'SA')
)
