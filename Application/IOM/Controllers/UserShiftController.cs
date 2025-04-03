using System.Threading;
using System.Threading.Tasks;
using IOM.Models.ApiControllerModels;
using System.Web.Http;
using IOM.Services.Interface;
using IOM.Properties;

namespace IOM.Controllers.WebApi
{
    [Authorize]
    [RoutePrefix("usershift")]
    public class UserShiftController : ApiController
    {
        private readonly IRepositoryService _repositoryService;

        public UserShiftController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        [HttpPost]
        [Route("data")]
        public ApiResult GetUserShift(TkReportDataRequestModel model)
        {
            var result = new ApiResult();

            if (model != null)
            {
                result = new ApiResult()
                {
                    data = _repositoryService.GetUserShiftAsync(model.Roles, model.AccountIds,
                        model.TeamIds, model.TagIds, model.UserIds, User.Identity.Name),
                    message = Resources.UserShiftSuccessUpdate
                };
            }

            return result;
        }

        [HttpPost]
        [Route("save")]
        public async Task<ApiResult> SaveUserShift(UserShiftDataRequest model, CancellationToken cancellationToken)
        {
            var result = new ApiResult();
            if (model != null)
            {
                await _repositoryService.SaveUserShiftDataAsync(model, cancellationToken).ConfigureAwait(false);
            }

            return result;
        }

        [HttpGet]
        [Route("timezones")]
        public async Task<ApiResult> GetTimeZones(CancellationToken cancellationToken)
        {
            var result = new ApiResult()
            {
                data = await _repositoryService.GetTimeZones(cancellationToken).ConfigureAwait(false),
                message = Resources.UserShiftSuccessUpdate
            };

            return result;
        }
    }
}