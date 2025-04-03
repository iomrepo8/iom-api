
-- =============================================
-- Author:		Jonathan Solatorio
-- Create date: Nov 21, 2019
-- Description:	Updates user permissions base on RolePermissions table data.
-- Params: 
--			• @roleId = Id of role to base.
--          • @userId = Id of user to update permissions
-- =============================================

CREATE PROCEDURE [dbo].[sp_UpdateUserPermissions]
	@roleId varchar(128),
	@userId varchar(128)
AS
	WITH updateData AS (
		SELECT UserId = @userId, rp.CanView, rp.CanAdd, rp.CanEdit, rp.CanDelete, rp.ModuleCode
		FROM AspNetRoles r
		JOIN RolePermissions rp ON r.Id = rp.RoleId
		WHERE r.Id = @roleId
	)

	UPDATE up
		SET 
			  up.CanView = ud.CanView
			, up.CanAdd = ud.CanAdd
			, up.CanEdit = ud.CanEdit
			, up.CanDelete = ud.CanDelete
	FROM AspNetUserPermission up
	INNER JOIN updateData ud 
		ON up.UserId = ud.UserId 
		AND up.ModuleCode = ud.ModuleCode
RETURN 0
