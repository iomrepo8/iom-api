using System.Net.Http;
using System.Threading.Tasks;
using IOM.Models.ApiControllerModels;
using IOM.Services;
using System.Web.Http;
using IOM.Services.Interface;
using IOM.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace IOM.Controllers.WebApi
{
    [Authorize]
    [RoutePrefix("auth")]
    public class AuthController : ApiController
    {
        private readonly IRepositoryService _repositoryService;
        
        public AuthController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        
        [HttpPost]
        [Route("logout")] 
        public ApiResult Logout()
        {
            var result = new ApiResult();
            var userInfo = _repositoryService.GetCurrentUserInfo(User.Identity.Name);

            _repositoryService.SetStatus(null, 7, userInfo.UserDetailsId);

            return result;
        }
        
        [HttpPost]
        [Route("enable-2fa")]
        public async Task<ApiResult> EnableTFA()
        {
            var result = new ApiResult();
            
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            await userManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true).ConfigureAwait(false);

            return result;
        }
        
        [HttpPost]
        [Route("verify-email-otp")]
        public async Task<ApiResult> VerifyEmailOtp(OtpCode code)
        {
            var result = new ApiResult();
            
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var verified = await userManager
                .VerifyTwoFactorTokenAsync(User.Identity.GetUserId(), TwoFactorProvider.EmailCode.ToString(), code.Code)
                .ConfigureAwait(false);

            if (!verified)
            {
                result.isSuccessful = false;
                result.message = $"{code.Code} is not a valid Security Code.";
                result.status = AuthFilter.Unauthorized.ToString();
                
                return result;
            }

            await userManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false)
                .ConfigureAwait(false);

            result.message = "Security Code verified successfully.";

            return result;
        }

    }
}