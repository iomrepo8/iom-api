using IOM.Models.ApiControllerModels;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using IOM.Services.Interface;

namespace IOM.Controllers.WebApi
{
    [Authorize]
    [RoutePrefix("notification")]
    public class NotificationApiController : ApiController
    {
        private readonly IRepositoryService _repositoryService;
        private readonly INotificationServices _notificationServices;

        public NotificationApiController(IRepositoryService repositoryService, INotificationServices notificationServices)
        {
            _repositoryService = repositoryService;
            _notificationServices = notificationServices;
        }
        
        [HttpPut]
        [Route("read")]
        public ApiResult MarkAsRead([FromUri] int notificationId, CancellationToken cancellationToken)
        {
            ApiResult result = new ApiResult();

            _notificationServices.MarkAsRead(notificationId, cancellationToken);

            return result;
        }

        [HttpPut]
        [Route("batch_read")]
        public async Task<ApiResult> BatchMarkAsRead([FromBody] int[] NotificationIds, CancellationToken cancellationToken)
        {
            ApiResult result = new ApiResult();

            try
            {
                await _notificationServices.BatchMarkAsRead(NotificationIds, cancellationToken).ConfigureAwait(false);
                result.isSuccessful = true;
            }
            catch (System.Exception e)
            {
                result.message = e.Message;
                result.isSuccessful = false;
            }

            return result;
        }

        [HttpPut]
        [Route("unread")]
        public async Task<ApiResult> MarkAsUnreadAsync([FromUri] int notificationId, CancellationToken cancellationToken)
        {
            ApiResult result = new ApiResult();

            await _notificationServices.MarkAsUnread(notificationId, cancellationToken).ConfigureAwait(false);

            return result;
        }

        [HttpGet]
        [Route("list")]
        public ApiResult GetNotifications([FromUri] string startDate, [FromUri] string endDate)
        {
            ApiResult result = new ApiResult();

            var user = _repositoryService.GetCurrentUserInfo(User.Identity.Name);
            result.data = _notificationServices
                .GetNotifications(startDate, endDate, user.UserDetailsId);

            return result;
        }

        [HttpGet]
        [Route()]
        public async Task<ApiResult> IsThereNotification(CancellationToken cancellationToken)
        {
            ApiResult result = new ApiResult();

            var user = _repositoryService.GetCurrentUserInfo(User.Identity.Name);
            result.data = await _notificationServices
                .IsThereNotificationAsync(user.UserDetailsId, cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
    }
}