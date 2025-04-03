CREATE FUNCTION [dbo].[fn_GetAssignedAccounts]
(
    @userDetailsId INT
)
RETURNS TABLE AS RETURN
(
	WITH userDetails AS (
		SELECT * FROM vw_ActiveUsers u WHERE u.UserDetailsId = @userDetailsId
	)

	SELECT AccountId 
	FROM dbo.AccountMember am
	INNER JOIN dbo.Accounts a ON am.AccountId = a.Id
	WHERE am.UserDetailsId = @userDetailsId 
		AND ISNULL(am.IsDeleted, 0) = 0 
		AND ISNULL(a.IsDeleted, 0) = 0 
		AND a.IsActive = 1
	UNION
	SELECT a.Id AS AccountId 
	FROM dbo.Accounts a
	WHERE a.IsActive = 1 AND ISNULL(a.IsDeleted, 0) = 0 
		AND EXISTS(SELECT 1 FROM userDetails WHERE [Role] = 'SA')
)
