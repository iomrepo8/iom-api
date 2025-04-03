using IOM.DbContext;
using IOM.Models.ApiControllerModels;
using System.Collections.Generic;
using System.Linq;
using IOM.Services.Interface;

namespace IOM.Services
{
    public class PermissionServices : IPermissionServices
    {
        public IList<NetRolePermission> GetRolePermissions()
        {
            using (var ctx = Entities.Create())
            {
                IList<NetRolePermission> roles =
                    (from r in ctx.AspNetRoles
                     select new NetRolePermission
                     {
                         RoleId = r.Id,
                         RoleCode = r.RoleCode,
                         RoleName = r.Name,
                         IsAllUsers = r.IsAllUsers,
                         IsLocked = r.PermissionsLocked,
                         Modules = (from rp in ctx.RolePermissions
                                    join m in ctx.AspNetModules on rp.ModuleId equals m.Id
                                    from pm in ctx.AspNetModules.Where(pmod =>
                                        pmod.ModuleCode == m.ParentModuleCode).DefaultIfEmpty()
                                    where rp.RoleId == r.Id
                                    orderby m.Order, m.SubModuleOrder
                                    select new RoleModule
                                    {
                                        Id = rp.Id,
                                        ModuleId = m.Id,
                                        ModuleName = m.Name,
                                        ModuleCode = m.ModuleCode,
                                        Order = m.Order,
                                        ParentModule = pm.Name,
                                        canView = rp.CanView,
                                        canAdd = rp.CanAdd,
                                        canEdit = rp.CanEdit,
                                        canDelete = rp.CanDelete
                                    }).ToList()
                     }).ToList();

                return roles;
            }
        }

        public void SaveRolePermissions(NetRolePermission rolePermissions)
        {
            using (var ctx = Entities.Create())
            {
                var netRole = ctx.AspNetRoles.Single(e => e.Id == rolePermissions.RoleId);

                netRole.PermissionsLocked = rolePermissions.IsLocked;
                netRole.IsAllUsers = rolePermissions.IsAllUsers;

                foreach (var module in rolePermissions.Modules)
                {
                    var roleModule = ctx.RolePermissions.SingleOrDefault(e => e.Id == module.Id);

                    roleModule.CanView = module.canView ?? false;
                    roleModule.CanAdd = module.canAdd ?? false;
                    roleModule.CanEdit = module.canEdit ?? false;
                    roleModule.CanDelete = module.canDelete ?? false;
                }

                ctx.SaveChanges();

                if (rolePermissions.IsAllUsers)
                {
                    ctx.sp_UpdateUserRolePermissions(rolePermissions.RoleId);
                }
            }
        }
    }
}
