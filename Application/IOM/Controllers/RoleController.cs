using IOM.Models.ApiControllerModels;
using IOM.Services;
using System.Web.Http;
using IOM.Services.Interface;

namespace IOM.Controllers.WebApi
{
    [RoutePrefix("role")]
    [Authorize]
    public class RolesController : ApiController
    {
        private readonly IRoleServices _roleServices;
        private readonly IPermissionServices _permissionServices;
        public RolesController(IRoleServices roleServices, IPermissionServices permissionServices)
        {
            _roleServices = roleServices;
            _permissionServices = permissionServices;
        }
        
        [HttpGet]
        [Route("nonadmins")]
        public ApiResult NonAdminRoles([FromUri] string q = "")
        {
            var result = new ApiResult
            {
                data = _roleServices.GetAssocRoleList(false, q)
            };

            return result;
        }

        [HttpGet]
        [Route("all")]
        public ApiResult AllRoles([FromUri] string q = "")
        {
            var result = new ApiResult
            {
                data = _roleServices.GetAssocRoleList(true, q)
            };

            return result;
        }

        [HttpGet]
        [Route("permissions")]
        public ApiResult GetRolePermissions()
        {
            var result = new ApiResult
            {
                data = _permissionServices.GetRolePermissions()
            };

            return result;
        }

        [HttpPost]
        [Route("update_permissions")]
        public ApiResult SaveRolePermissions(NetRolePermission rolePermissions)
        {
            var result = new ApiResult();

            _permissionServices.SaveRolePermissions(rolePermissions);

            return result;
        }
    }
}