using IOM.Models.ApiControllerModels;
using IOM.Services;
using System.Web.Http;
using IOM.Services.Interface;

namespace IOM.Controllers.WebApi
{
    [Authorize]
    [RoutePrefix("syslogs")]
    public class SystemLogApiController : ApiController
    {
        private readonly IRepositoryService _repositoryService;
        public SystemLogApiController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        
        [HttpPost]
        [Route("data")]
        public ApiResult SystemLogData(SysLogRequestDataModel model)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetSystemLogData(model)
            };

            return result;
        }

        [HttpGet]
        [Route("entities")]
        public ApiResult Entities([FromUri] string q = "")
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetEntities(q)
            };

            return result;
        }
    }
}