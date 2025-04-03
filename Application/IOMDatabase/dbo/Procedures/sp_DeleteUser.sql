CREATE PROCEDURE [dbo].[sp_DeleteUser]
    @userId int = 0
AS
    DECLARE @netUserId varchar(128);

    SELECT @netUserId = UserId FROM UserDetails WHERE Id = @userId;

	UPDATE UserDetails
	SET IsDeleted = 1
	WHERE Id = @userId;

	UPDATE AspNetUsers
	SET IsDeleted = 1
	WHERE Id = @netUserId;

RETURN 0

