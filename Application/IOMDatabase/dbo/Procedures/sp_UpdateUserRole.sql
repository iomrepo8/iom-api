CREATE PROCEDURE [dbo].[sp_UpdateUserRole]
	@userId varchar(128),
	@roleCode varchar(5)
AS
	UPDATE AspNetUserRoles SET RoleId = (SELECT Id FROM AspNetRoles WHERE RoleCode = @roleCode)
	WHERE UserId = @userId;
RETURN 0
