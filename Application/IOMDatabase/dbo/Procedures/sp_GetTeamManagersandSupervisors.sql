CREATE PROCEDURE sp_GetTeamManagersandSupervisors 
	@teamId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @accountId int,
			@accountName VARCHAR(150),
			@teamName VARCHAR(150);

	SELECT 
		@accountId = t.AccountId,
		@accountName = a.[Name],
		@teamName = t.[Name]
	FROM dbo.Teams t
	INNER JOIN dbo.Accounts a ON t.AccountId = a.Id
	WHERE t.Id = @teamId;

	SELECT 
		u.UserDetailsId as UserId
		, u.Email
		, u.FullName AS [Name]
		, u.[Role]
		, AccountId = @accountId
		, AccountName = @accountName
		, TeamName = @teamName
	FROM 
		dbo.TeamMember tm
		INNER JOIN vw_ActiveUsers u ON u.UserDetailsId = tm.UserDetailsId
	WHERE 
			tm.TeamId = @teamId 
		AND u.[Role] IN ('TM', 'LA') 
		AND ISNULL(tm.IsDeleted, 0) = 0
		AND EXISTS(SELECT * FROM dbo.AccountMember am 
				   WHERE am.UserDetailsId = tm.UserDetailsId AND am.AccountId = @accountId
						AND ISNULL(am.IsDeleted, 0) = 0)
	UNION
	SELECT 
		u.UserDetailsId as UserId
		, u.Email
		, u.FullName AS [Name]
		, u.[Role]
		, AccountId = @accountId
		, AccountName = @accountName
		, TeamName = @teamName
	FROM 
		dbo.AccountMember am
		INNER JOIN vw_ActiveUsers u ON u.UserDetailsId = am.UserDetailsId
	WHERE 
			am.AccountId = @accountId
		AND u.[Role] IN ('AM') 
		AND ISNULL(am.IsDeleted, 0) = 0		
END
GO
