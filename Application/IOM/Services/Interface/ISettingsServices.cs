using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IOM.Models;
using IOM.Models.ApiControllerModels;
using IOM.Models.ApiControllerModels.Settings;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        IList<LocationModel> GetLocations();
        void SaveLocation(LocationModel location, string username);
        Task DeleteIPWhitelist(int id, string name);
        Task<ApiResult> SaveIP(IPWhitelist ipAddress, string username);
        Task<IList<IPWhitelist>> GetIPWhitelist();
        void DeleteLocation(int id, string name);
        Task<IList<UserNotificationSettingResult>> GetUserNotificationSettings(int userDetailsId);
        Task UpdateUserNotificationSettings(IList<UserNotificationSettingResult> notificationSettings);
        Task<IList<NotificationSettingModel>> GetNotificationSettingsAsync(CancellationToken cancellationToken);

        Task<NotificationSettingModel> GetNotificationSettingsAsync(NotificationAction notificationAction,
            CancellationToken cancellationToken);

        Task<IList<NotificationRecipientRoleModel>> GetNotificationRoleRecipientsAsync(int notificationSettingsId, CancellationToken cancellationToken);
        Task UpdateNotificationSettingAsync(NotificationSettingModel notificationSetting, CancellationToken cancellationToken);
    }
}