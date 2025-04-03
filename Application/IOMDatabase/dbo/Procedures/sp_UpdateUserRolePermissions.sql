
-- =============================================
-- Author:		Jonathan Solatorio
-- Create date: Nov 15, 2019
-- Description:	Updates user permissions by role base on RolePermissions table data.
-- Params: @roleId = Id of role to update.
-- =============================================

CREATE PROCEDURE [dbo].[sp_UpdateUserRolePermissions]
	@roleId varchar(128)
AS
		
	WITH updateData AS (
		SELECT ud.Id, ud.UserId, rp.CanView, rp.CanAdd, rp.CanEdit, rp.CanDelete, rp.ModuleCode
		FROM UserDetails ud
		JOIN AspNetRoles r ON ud.[Role] = r.RoleCode
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
