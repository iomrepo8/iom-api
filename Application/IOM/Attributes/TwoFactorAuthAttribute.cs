using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using IOM.Models.ApiControllerModels;
using IOM.Utilities;
using Microsoft.AspNet.Identity.Owin;

namespace IOM.Attributes
{
    internal sealed class TwoFactorAuthAttribute : IAuthorizationFilter
    {
        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            if (SkipAuthorization(actionContext)) await continuation().ConfigureAwait(false);

            #region Get userManager
            var userManager = HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>();
            if(userManager == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, 
                    new ApiResult
                    {
                        isSuccessful = false,
                        status = AuthFilter.Unauthorized.ToString(),
                        message = "Failed to authenticate user."
                    });;
                
                return actionContext.Response;
            }
            #endregion

            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            #region Get current user
            var user = await userManager.FindByNameAsync(principal?.Identity?.Name).ConfigureAwait(false);
            if(user == null)
            {
                actionContext.Response =
                    actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, 
                        new ApiResult
                        {
                            isSuccessful = false,
                            status = AuthFilter.Unauthorized.ToString(),
                            message = "Failed to authenticate user."
                        });
                return actionContext.Response;
            }
            #endregion
            
            #region Validate Two-Factor Authentication
            if (user.TwoFactorEnabled && actionContext.Request.RequestUri.LocalPath != "/auth/verify-email-otp")
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.PreconditionFailed,
                    new ApiResult
                    {
                        isSuccessful = false,
                        status = AuthFilter.Unauthorized2Fa.ToString(),
                        message = "User must be authenticated using Two-Factor Authentication."
                    });
                return actionContext.Response;
            }
            #endregion

            //If this line was reached it means the user is allowed to use this method, so just return continuation() which basically means continue processing 
            return await continuation().ConfigureAwait(false);
        }
        

        public bool AllowMultiple { get; }
        
        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            Contract.Assert(actionContext != null);

            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }

}