using IOM.Models;
using IOM.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IOM
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return SendMailAsync(message);
        }

        private async Task SendMailAsync(IdentityMessage message)
        {
            var smtpClient = new SmtpClient(ConfigurationManager.AppSettings["mailHost"],
                Convert.ToInt32(ConfigurationManager.AppSettings["mailPort"]))
            {
                UseDefaultCredentials = false,
                Credentials = null,
                Timeout = 0,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                DeliveryFormat = SmtpDeliveryFormat.SevenBit,
                PickupDirectoryLocation = null,
                EnableSsl = false,
                TargetName = null
            };
            using (MailMessage msg = new MailMessage
                   {
                       From = new MailAddress(ConfigurationManager.AppSettings["mailAccount"]),
                   })
            {
                msg.To.Add(new MailAddress(message.Destination));
                msg.Subject = message.Subject;
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message.Body, null, MediaTypeNames.Text.Html));

                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["mailAccount"], ConfigurationManager.AppSettings["mailPassword"]);
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;

                try
                {
                    await smtpClient.SendMailAsync(msg);
                }
                catch { }
            }
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true
            };
            
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser, string>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is: {0}"
            });

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            manager.EmailService = SendGridMailServices.Instance;

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                    {
                        TokenLifespan = TimeSpan.FromHours(7.0)
                    };
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
