using IOM.DbContext;
using IOM.Models;
using IOM.Properties;
using IOM.Services;
using IOM.Utilities;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IOM.Services.Interface;

namespace IOM.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            _publicClientId = publicClientId ?? throw new ArgumentNullException("publicClientId");
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            var username = context.UserName;

            if (context.UserName.Contains("@"))
            {
                var userDetails = await userManager.FindByEmailAsync(context.UserName).ConfigureAwait(true);
                if (userDetails != null)
                {
                    username = userDetails.UserName;
                }
            }

            var user = await userManager.FindAsync(username, context.Password).ConfigureAwait(true);

            if (user == null)
            {
                context.SetError(Resources.UserInvalidKey, Resources.UserInvalidMsg);
                return;
            }

            if (context.UserName != "logicapp")
            {
                await userManager.SetTwoFactorEnabledAsync(user.Id, true).ConfigureAwait(false);
                var code = await userManager.GenerateTwoFactorTokenAsync(user.Id, "EmailCode").ConfigureAwait(false);
                var notificationResult = await userManager.NotifyTwoFactorTokenAsync(user.Id, "EmailCode", code)
                    .ConfigureAwait(false);

                using (var ctx = Entities.Create())
                {
                    var userDetail = ctx.UserDetails.SingleOrDefault(e => e.UserId == user.Id);

                    if (userDetail.IsDeleted)
                    {
                        context.SetError(Resources.UserDeletedkey, Resources.UserDeletedMsg);
                        return;
                    }

                    if (userDetail.IsLocked.HasValue && userDetail.IsLocked.Value)
                    {
                        context.SetError(Resources.UserLockedKey, Resources.UserLockedMsg);
                        return;
                    }

                    SaveToDb(new SystemLog
                    {
                        LogDate = DateTimeUtility.Instance.DateTimeNow(),
                        ActorUserId = userDetail.Id,
                        RawUrl = context.Request.Uri.AbsoluteUri,
                        ActionType = "Login",
                        Entity = "Authentication",
                        IPAddress = context.Request.LocalIpAddress
                    });
                }
            }

            var oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
                OAuthDefaults.AuthenticationType).ConfigureAwait(false);

            var properties = CreateProperties(user.UserName);
            var ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        private static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                {"userName", userName}
            };
            return new AuthenticationProperties(data);
        }
        
        private void SaveToDb(SystemLog systemLog)
        {
            using (var ctx = Entities.Create())
            {
                ctx.SystemLogs.Add(systemLog);

                ctx.SaveChanges();
            }
        }
    }
}