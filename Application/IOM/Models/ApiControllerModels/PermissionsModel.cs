using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class PermissionsModel : BaseModel
    {
        public string ModuleCode { get; set; }
        public string ParentModule { get; set; }
        public bool? IsSubModule { get; set; }
        public bool? canAdd { get; set; }
        public bool? canView { get; set; }
        public bool? canEdit { get; set; }
        public bool? canDelete { get; set; }
    }

    public class RoleModule : PermissionsModel
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int? Order { get; set; }
    }

    public class UserPermissionModel : PermissionsModel
    {
        public string UserId { get; set; }
        public IList<RoleModule> Modules { get; set; }
    }

    public class NetRolePermission
    {
        public string RoleId { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        /// <summary>
        /// True: User permission management in Users page is not editable.
        /// False: User permission management in Users page is editable.
        /// </summary>
        public bool IsLocked { get; set; }
        /// <summary>
        /// True: Upon update of roles permissions, all changes will be applied to all users.
        /// False: Upon update of roles permissions, new settings will only be applied to new users.
        /// </summary>
        public bool IsAllUsers { get; set; }
        public IList<RoleModule> Modules { get; set; }
    }
}