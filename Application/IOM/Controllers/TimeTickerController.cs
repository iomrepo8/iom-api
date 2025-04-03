using IOM.Services;
using System.Threading.Tasks;
using System.Web.Http;
using IOM.Services.Interface;

namespace IOM.Controllers.WebApi
{
    [Authorize]
    [RoutePrefix("time")]
    public class TimeTickerController : ApiController
    {
        private readonly IRepositoryService _repositoryService;
        private readonly INotificationServices _notificationServices;

        public TimeTickerController(IRepositoryService repositoryService, INotificationServices notificationServices)
        {
            _repositoryService = repositoryService;
            _notificationServices = notificationServices;
        }
        [HttpPost]
        [Route("tick")]
        public void Tick()
        {
            Task.Run(() =>
                {
                    _repositoryService.UpdateUsersActiveHours();
                });
        }

        [HttpPost]
        [Route("auto_out")]
        public void AutoOut()
        {
            Task.Run(() =>
            {
                _repositoryService.AutoOutThreeAMUTC();
            });
        }

        [HttpPost]
        [Route("notify")]
        public void Notify()
        {
            Task.Run(() =>
                  {
                      _notificationServices.NotifyReminder();
                  });

        }

        [HttpPost]
        [Route("attendance_reminder")]
        public void AttendanceReminder()
        {
            Task.Run(() =>
            {
                _notificationServices.AttendanceReminder();
            });
        }

        [HttpPost]
        [Route("collect_time_log")]
        public void CollectTimeLog()
        {
            Task.Run(() =>
            {
                _repositoryService.CollectTimeLog();
            });
        }
    }
}