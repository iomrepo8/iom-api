using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using IOM.Models.ApiControllerModels.Settings;
using IOM.Services.Interface;

namespace IOM.Controllers
{
    [Authorize]
    [RoutePrefix("settings")]
    public class SettingsController : ApiController
    {
        private readonly IRepositoryService _repositoryService;

        public SettingsController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        
        [HttpGet]
        [Route("locations")]
        public ApiResult Locations()
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetLocations()
            };

            return result;
        }

        [HttpPost]
        [Route("save_location")]
        public ApiResult SaveLocation(LocationModel locationModel)
        {
            var result = new ApiResult();

            if (locationModel is null) throw new ArgumentNullException(nameof(locationModel));

            _repositoryService.SaveLocation(locationModel, User.Identity.Name);
            result.message = locationModel.Id > 0 ?
                Resources.LocationSuccessUpdate : Resources.LocationSuccessAdd;

            return result;
        }

        [HttpDelete]
        [Route("location/delete/{id}")]
        public ApiResult DeleteLocation(int id)
        {
            var result = new ApiResult();

            _repositoryService.DeleteLocation(id, User.Identity.Name);

            return result;
        }

        [HttpGet]
        [Route("ip-whitelist")]
        public async Task<ApiResult> IPWhitelist()
        {
            var result = new ApiResult
            {
                data = await _repositoryService.GetIPWhitelist().ConfigureAwait(false)
            };

            return result;
        }

        [HttpPost]
        [Route("save_ip")]
        public async Task<ApiResult> SaveIP(IPWhitelist ipAddress)
        {
            if (ipAddress is null) throw new ArgumentNullException(nameof(ipAddress));

            return await _repositoryService.SaveIP(ipAddress, User.Identity.Name)
                .ConfigureAwait(false);
        }

        [HttpDelete]
        [Route("ip/delete/{id}")]
        public async Task<ApiResult> DeleteIPWhitelist(int id)
        {
            var result = new ApiResult();

            await _repositoryService.DeleteIPWhitelist(id, User.Identity.Name)
                .ConfigureAwait(false);

            return result;
        }
        
        [HttpGet]
        [Route("email-notification")]
        public async Task<ApiResult> UserNotificationSettings(int userDetailsId)
        {
            var result = new ApiResult
            {
                data = await _repositoryService.GetUserNotificationSettings(userDetailsId)
                    .ConfigureAwait(false)
            };

            return result;
        }
        
        [HttpPut]
        [Route("email-notification")]
        public async Task<ApiResult> UpdateUserNotificationSettings(IList<UserNotificationSettingResult> notificationSettings)
        {
            var result = new ApiResult();

            await _repositoryService.UpdateUserNotificationSettings(notificationSettings).ConfigureAwait(false);

            result.message = Resources.EmailNotificationSuccessUpdate;
            return result;
        }
        
        [HttpPost]
        [Route("notifications")]
        public async Task<ApiResult> UpdateNotifications(NotificationSettingModel notificationSetting, CancellationToken cancellationToken)
        {
            var result = new ApiResult();
            try
            {
                await _repositoryService.UpdateNotificationSettingAsync(notificationSetting, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return result;
        }
        
        [HttpGet]
        [Route("notifications")]
        public async Task<ApiResult> Notifications(CancellationToken cancellationToken)
        {
            var result = new ApiResult
            {
                data = await _repositoryService.GetNotificationSettingsAsync(cancellationToken)
                    .ConfigureAwait(false)
            };

            return result;
        }
        
        [HttpGet]
        [Route("notifications/role-recipients")]
        public async Task<ApiResult> NotificationsRoleRecipient(int notificationSettingsId, CancellationToken cancellationToken)
        {
            var result = new ApiResult
            {
                data = await _repositoryService.GetNotificationRoleRecipientsAsync(notificationSettingsId, cancellationToken)
                    .ConfigureAwait(false)
            };

            return result;
        }
    }
}