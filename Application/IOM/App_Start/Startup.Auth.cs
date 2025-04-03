using IOM.Models;
using IOM.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;

namespace IOM
{
    public partial class Startup
    {
        private string PublicClientId { get; set; }
        private OAuthAuthorizationServerOptions OAuthOptions { get; set; }

        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        private void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(30),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}