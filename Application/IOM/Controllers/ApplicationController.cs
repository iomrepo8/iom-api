using IOM.Models.ApiControllerModels;
using IOM.Services;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IOM.Services.Interface;

namespace IOM.Controllers.WebApi
{
    [Authorize]
    [RoutePrefix("app")]
    public class ApplicationController : ApiController
    {
        private readonly IRepositoryService _repositoryService;

        public ApplicationController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        
        [AllowAnonymous]
        [HttpGet]
        [Route("info")]
        public ApiResult AppInfo()
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetAppInfo()
            };

            return result;
        }

        [HttpPost]
        [Route("inquire_support")]
        public async Task<ApiResult> InquireSupport(SupportEmail supportEmail)
        {
            var result = new ApiResult();

            if (supportEmail == null) throw new ArgumentNullException(nameof(supportEmail));

            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

            await _repositoryService.InquireSupportAsync(supportEmail, userManager, User.Identity.Name)
                .ConfigureAwait(false);

            return result;
        }
    }
}