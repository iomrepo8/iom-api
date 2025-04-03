using IOM.Models.ApiControllerModels;
using IOM.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using IOM.Services.Interface;
using IOM.Utilities;

namespace IOM.Controllers
{
    [RoutePrefix("seat")]
    [System.Web.Http.Authorize]
    public class SeatController : ApiController
    {
        private readonly IRepositoryService _repositoryService;
        private readonly string _urlHost = ConfigurationManager.AppSettings["SiteUri"];

        public SeatController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        
        [HttpGet]
        [Route("get")]
        public ApiResult GetVacantSeats([FromUri] int accountid)
        {
            ApiResult result = new ApiResult();

            try
            {
                result.data = _repositoryService.GetAvailableAccountSeats(accountid);
                result.status = "OK";
                result.isSuccessful = true;
            }
            catch (Exception e)
            {
                result.status = "Error";
                result.message = e.Message;
                result.isSuccessful = false;
            }
            
            return result;
        }

        [HttpGet]
        [Route("dashboard")]
        public ApiResult GetDashboardSeats([FromUri] int[] accountids)
        {
            ApiResult result = new ApiResult();

            try
            {
                result.data = _repositoryService.GetDashboardSeats(accountids);
                result.status = "OK";
                result.isSuccessful = true;
            }
            catch (Exception e)
            {
                result.status = "Error";
                result.message = e.Message;
                result.isSuccessful = false;
            }

            return result;
        }

        [HttpPost]
        [Route("occupy")]
        public async Task<IHttpActionResult> OccupySeat([FromUri] int accountid, [FromUri] int sequence, [FromUri] int userid, [FromUri] string occupytype, CancellationToken cancellationToken)
        {
            var siteUrl = _urlHost + "/seats";
            var result = await _repositoryService
                .OccupySeat(accountid, userid, sequence, occupytype, siteUrl, User.Identity.GetUserId(), cancellationToken).ConfigureAwait(false);
            
            return Ok(result);
        }

        [HttpPost]
        [Route("vacant")]
        public async Task<ApiResult> VacantSeat([FromUri] int accountid, [FromUri] int sequence, [FromUri] int userid)
        {
            var result = new ApiResult();
            var msg = string.Empty;

            try
            {
                var siteUrl = _urlHost + "/seats";

                if (_repositoryService.VacanSeat(accountid, userid, sequence, siteUrl, User.Identity.GetUserId(), out msg))
                {
                    result.status = "OK";
                    result.isSuccessful = true;
                }
                else
                {
                    result.status = "Error";
                    result.isSuccessful = false;
                    result.message = msg;
                }
            }
            catch (Exception e)
            {
                result.status = "Error";
                result.message = e.Message;
                result.isSuccessful = false;
            }

            return result;
        }

        [HttpPost]
        [Route("status")]
        public async Task<ApiResult> UpdateStatus([FromUri] int accountid, [FromUri] int sequence, [FromUri] int userid, [FromUri] string occupytype)
        {
            var result = new ApiResult();
            var msg = string.Empty;

            try
            {
                var siteUrl = _urlHost + "/seats";

                if (_repositoryService.UpdateStatus(accountid, userid, sequence, occupytype, siteUrl, User.Identity.GetUserId(), out msg))
                {
                    result.status = "OK";
                    result.isSuccessful = true;
                }
                else
                {
                    result.status = "Error";
                    result.isSuccessful = false;
                    result.message = msg;
                }
            }
            catch (Exception e)
            {
                result.status = "Error";
                result.message = e.Message;
                result.isSuccessful = false;
            }

            return result;
        }


    }
}
