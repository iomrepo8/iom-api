using IOM.Models.ApiControllerModels;
using IOM.Services;
using System.Web.Http;
using IOM.Services.Interface;

namespace IOM.Controllers.WebApi
{
    [Authorize]
    [RoutePrefix("dashboard")]
    public class DashboardApiController : ApiController
    {
        private readonly IRepositoryService _repositoryService;
        public DashboardApiController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        [HttpGet]
        [Route("data")]
        public ApiResult DashboardData()
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetDashboardData(User.Identity.Name)
            };

            return result;
        }
    }
}