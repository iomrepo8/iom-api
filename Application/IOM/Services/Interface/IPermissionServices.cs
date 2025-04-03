using System.Collections.Generic;
using IOM.Models.ApiControllerModels;

namespace IOM.Services.Interface
{
    public interface IPermissionServices
    {
        IList<NetRolePermission> GetRolePermissions();
        void SaveRolePermissions(NetRolePermission rolePermissions);
    }
}