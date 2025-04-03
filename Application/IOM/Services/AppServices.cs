using IOM.Models.ApiControllerModels;
using IOM.Utilities;
using SendGrid.Helpers.Mail;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using IOM.Models;
using IOM.Services.Interface;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public AppInfo GetAppInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();

            return new AppInfo
            {
                AppName = "IOM",
                AppEnvironment = ConfigurationManager.AppSettings["DeploymentEnvironment"],
                BuildVersion = assembly.GetName().Version.ToString(),
                SystemDate = DateTimeUtility.Instance.DateTimeNow()
            };
        }

        public async Task InquireSupportAsync(SupportEmail supportEmail, ApplicationUserManager userManager, string username)
        {
            var userInfo = GetCurrentUserInfo(username);

            var sender = new EmailAddress
            {
                Email = userInfo.Email,
                Name = userInfo.FullName
            };
            
            var emailRecipients = GetAdminEmailRecipients(NotificationType.SupportInquiry, out _);
            
            await SendGridMailServices.SupportInquiry(supportEmail, emailRecipients, sender)
                .ConfigureAwait(false);
        }
    }
}