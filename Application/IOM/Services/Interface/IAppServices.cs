using System.Threading.Tasks;
using IOM.Models.ApiControllerModels;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        AppInfo GetAppInfo();
        Task InquireSupportAsync(SupportEmail supportEmail, ApplicationUserManager userManager, string username);
    }
}