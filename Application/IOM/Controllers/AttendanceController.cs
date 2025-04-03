using IOM.Models;
using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Globalization;
using System.Web.Http;
using IOM.Services.Interface;

namespace IOM.Controllers
{
    [Authorize]
    [RoutePrefix("attendance")]
    public class AttendanceController : ApiController
    {
        private readonly IRepositoryService _repositoryService;
        public AttendanceController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        
        [HttpPost]
        [Route("overtime_edit")]
        public ApiResult SaveAttendance([FromBody] OvertimeData attendanceData)
        {
            var result = new ApiResult();

            if (attendanceData is null) throw new ArgumentNullException(nameof(attendanceData));

            _repositoryService.SaveAttendanceOTUpdate(attendanceData, User.Identity.Name);

            return result;
        }

        [HttpPost]
        [Route("default")]
        public ApiResult AttendanceDefaultView(AttendanceDataRequestModel model)
        {
            var result = new ApiResult
            {
                data = _repositoryService
                    .AttendanceDefault(model.UserIds, model.AccountIds, model.TeamIds, model.Roles, model.StartDate, model.EndDate,
                        User.Identity.Name, model.Tags, model.HasLiveHours)
            };

            return result;
        }

        [HttpGet]
        [Route("rows")]
        public ApiResult AttendanceRowViewController([FromUri] int attendanceId)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetAttendanceRows(attendanceId)
            };

            return result;
        }

        [HttpGet]
        [Route("att_chrono")]
        public ApiResult AttendanceChronoView([FromUri] int userId, [FromUri] string date)
        {
            var result = new ApiResult();

            var userInfo = _repositoryService.GetUserDetails(userId);

            if (date is null) throw new ArgumentNullException(nameof(date));

            result.data =
                new
                {
                    ChronoData = _repositoryService.GetChronoData(userId, date),
                    UserData = new
                    {
                        FullName = userInfo.FullName
                    }
                };

            return result;
        }

        [HttpGet]
        [Route("eod_chrono")]
        public ApiResult GetChronoDetails([FromUri] int eodId)
        {
            var result = new ApiResult();

            var eodReport = _repositoryService.GetEODReportDetails(eodId);

            var eodStatus = _repositoryService.GetEODStatus(eodId);
            var note = "";

            switch (eodStatus.EODEnumStatus)
            {
                case EODStatus.Denied:
                    note = Resources.EODDeniedStatusNote;
                    break;
                case EODStatus.Pending:
                    note = Resources.EODPendingStatusNote;
                    break;
            }

            var stringDate = eodReport.EODDate.ToString(Resources.DateFormat, CultureInfo.InvariantCulture);

            result.data =
                new
                {
                    ChronoData = _repositoryService.GetChronoData(eodReport.SenderDetails.UserDetailsId,
                        stringDate),
                    UserData = new
                    {
                        FullName = eodReport.SenderDetails.FullName
                    },
                    Note = note,
                    EODDate = stringDate,
                    EODReportStatus = eodStatus.EODEnumStatus.ToString()
                };

            return result;
        }

        [HttpPost]
        [Route("overtime")]
        public ApiResult AttendanceOTView(AttOTDataRequestModel model)
        {
            var result = new ApiResult
            {
                data = _repositoryService
                    .AttendanceOT(model.UserIds, model.AccountIds, model.TeamIds, model.Roles, model.StartDate, model.EndDate, User.Identity.Name)
            };

            return result;
        }

        [HttpPost]
        [Route("att_status")]
        public ApiResult AttendanceStatusView(AttStatusRequestDataModel model)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetStatusViewData(model.StartDate, model.EndDate, User.Identity.GetUserId(), model.UserIds, model.Roles)
            };

            return result;
        }

        [HttpGet]
        [Route("att_status_updates")]
        public ApiResult LatestAttendanceStatus([FromUri] string startDate, [FromUri] string endDate)
        {
            var result = new ApiResult
            {
                data = _repositoryService
                    .GetAttendanceStatusUpdates(startDate, endDate, User.Identity.GetUserId())
            };

            return result;
        }

        [HttpPost]
        [Route("set_att_status")]
        public ApiResult SetAttendanceStatus(string datestatus, string oldVal, string newVal, int statusOwner)
        {
            var result = new ApiResult();


            _repositoryService.SetAttendanceStatus(datestatus, oldVal, newVal, statusOwner, User.Identity.GetUserId());
            result.message = Resources.AttStatusUpdateSuccess;

            return result;
        }

        [HttpPost]
        [Route("att_status/options")]
        public ApiResult UpdateStatusOptions([FromUri] string mode, [FromUri] string statusname,
            [FromUri] int id = 0)
        {
            var result = new ApiResult();

            _repositoryService.SetAttendanceStatusOptions(mode, statusname, id);

            return result;
        }

        [HttpGet]
        [Route("att_status/options")]
        public ApiResult GetStatusOptions()
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetAttendanceStatusOptions()
            };

            return result;
        }
    }
}