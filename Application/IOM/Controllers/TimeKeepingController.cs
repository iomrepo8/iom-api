using IOM.Helpers;
using IOM.Models;
using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Services;
using IOM.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IOM.Services.Interface;
using Microsoft.Ajax.Utilities;
using SendGrid.Helpers.Mail;

namespace IOM.Controllers.WebApi
{
    [Authorize]
    [RoutePrefix("tkdata")]
    public class TimeKeepingController : ApiController
    {
        private readonly IRepositoryService _repositoryService;

        public TimeKeepingController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        
        [HttpPost]
        [Route("report")]
        public ApiResult ReportData(TkReportDataRequestModel model)
        {
            var result = new ApiResult();

            if (model != null)
            {
                var reportData = _repositoryService.ReportGridData(
                    model.AccountIds, model.TeamIds, model.UserIds, model.TagIds, model.Roles, model.StartDate, model.EndDate,
                    User.Identity.Name, model.HasLiveHours);

                result.data = new
                {
                    Report = reportData,
                    TotalTime = reportData.AsEnumerable().Sum(e => e.TaskActiveTime)
                };
            }

            return result;
        }

        [HttpPost]
        [Route("mgt")]
        public ApiResult MgtData(TimeKeepingDataRequestModel model)
        {
            var result = new ApiResult();

	    if (model != null)
            {            var mgtData = _repositoryService.ManagementData(
                model.Roles, model.AccountIds, model.TeamIds,
                model.TagIds, model.UserIds, model.StartDate,
                model.EndDate, User.Identity.Name, model.IncludeInactive, model.HasLiveHours, model.StatusFilter);

                result.data = new
                {
                    Management = mgtData,
                    TotalTaskTime = mgtData.AsEnumerable().Sum(e => e.TaskActiveTime),
                    TotalActiveHours = mgtData.AsEnumerable().Sum(e => e.TaskActiveTime),
                    TotalUsers = mgtData.Count
                };
            }

            return result;
        }

        [HttpGet]
        [Route("week_attendance")]
        public ApiResult WeekHours([FromUri] int userId, [FromUri] string startDate, [FromUri] string endDate)
        {
            var result = new ApiResult();

            if (startDate is null) throw new ArgumentNullException(nameof(startDate));
            if (endDate is null) throw new ArgumentNullException(nameof(endDate));

            result.data = _repositoryService
                .WeekHours(userId, startDate, endDate);

            return result;
        }

        [HttpPost]
        [Route("save_attendance")]
        public ApiResult SaveAttendance(IList<AttendanceDay> attendanceData)
        {
            var result = new ApiResult();

            _repositoryService.SaveAttendanceData(attendanceData, User.Identity.Name);

            return result;
        }

        [HttpPost]
        [Route("insert_individual_attendance")]
        public ApiResult SaveIndividualAttendance(AttendanceIndividual attendanceDataIndiv)
        {
            /*string timeFormat = "HH:mm";

            if(DateTime.TryParseExact(attendanceDataIndiv?.Start, timeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime startresult))
            {
                System.Diagnostics.Debug.WriteLine("Start time: " + startresult);
            }
            
            if (DateTime.TryParseExact(attendanceDataIndiv?.End, timeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime endresult))
            {
                System.Diagnostics.Debug.WriteLine("End time: " + endresult);
            }

            TimeSpan timeDifference = endresult - startresult;
            double timeDifferenceInHours = timeDifference.TotalHours;
            System.Diagnostics.Debug.WriteLine("Difference: " + timeDifferenceInHours);
            System.Diagnostics.Debug.WriteLine("User: " + attendanceDataIndiv?.UserDetailsId);
            System.Diagnostics.Debug.WriteLine("Date: " + attendanceDataIndiv?.SDate);*/

            var result = new ApiResult();
            _repositoryService.SaveIndividualAttendanceData(attendanceDataIndiv, User.Identity.Name);

            return result;
        }

        [HttpPost]
        [Route("insert_attendance_row")]
        public ApiResult SaveAttendanceRowController(AttendanceRowModel attendanceRowModel)
        {
            var result = new ApiResult();
            _repositoryService.SaveAttendanceRow(attendanceRowModel, User.Identity.Name);

            return result;
        }

        [HttpGet]
        [Route("eodreport")]
        public ApiResult EOD([FromUri] string startDate, [FromUri] string endDate)
        {
            var result = new ApiResult();
            result.isSuccessful = true;

            result.data = _repositoryService.GenerateEOD(startDate, endDate, User.Identity.Name);

            return result;
        }

        [HttpPost]
        [Route("sendeodreport")]
        public async Task<ApiResult> sendeod([FromBody] EODRecipients eodModel, [FromUri] string startDate,
            [FromUri] string endDate, [FromUri] int clientOffset)
        {
            var result = new ApiResult();
            result.isSuccessful = true;
            var specialRecipients = new List<Recipient>();

            foreach (var recipient in eodModel.Recipients)
            {
                specialRecipients.Add(recipient);
            }

            DateTime eodDate;
            if (!DateTime.TryParse(startDate, out eodDate))
            {
                result.isSuccessful = false;
                result.message = $"{startDate} is not a valid date";

                return result;
            }

            // for future proofing only
            DateTime endEODDate;
            if (!DateTime.TryParse(endDate, out endEODDate))
            {
                result.isSuccessful = false;
                result.message = $"{endDate} is not a valid date";

                return result;
            }

            var userInfo = _repositoryService.GetCurrentUserInfo(User.Identity.Name);

            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var token = userManager.GeneratePasswordResetToken(userInfo.NetUserId);
            var uri = Request.Headers.Referrer;

            eodModel.Accounts = _repositoryService.GetAccounts(userInfo.UserDetailsId);
            eodModel.Teams = _repositoryService.GetTeams(userInfo.UserDetailsId);

            var chronoRoute = string.Format(CultureInfo.InvariantCulture,
                AppConfigurations.ChronoDetailRoute, token, userInfo.UserDetailsId, startDate);

            var chronoDetailUrl = $"{uri.Scheme}://{uri.Authority}{chronoRoute}";
            var usersRecipientSuperiors =
                _repositoryService.GetUserEODRecipients(eodModel.UserDetailsId, NotificationType.EodReport);

            foreach (var recipient in usersRecipientSuperiors)
            {
                eodModel.Recipients.Add(recipient);
            }

            var newEodId = await _repositoryService.SendEOD(eodModel, startDate, endDate, clientOffset,
                userInfo.NetUserId, chronoDetailUrl).ConfigureAwait(false);

            if (newEodId <= 0)
            {
                result.message = "An error occured while sending EOD report.";
                result.isSuccessful = false;
            }
            else
            {
                var eodIsEdited = false;

                // check for edited items
                foreach (var item in eodModel.EODTaskList)
                {
                    if (item.IsAdjusted || item.IsInserted || item.IsRemoved)
                    {
                        eodIsEdited = true;
                        break;
                    }
                }

                if (eodIsEdited)
                {
                    var returnUrl = eodModel.ConfirmationURL.ReturnURLWithParams;

                    for (var i = 0; i < eodModel.ConfirmationURL.Params.Count; i++)
                    {
                        var param = eodModel.ConfirmationURL.Params[i];
                        returnUrl = returnUrl.Replace($"{{{i}}}", param);
                    }

                    returnUrl = returnUrl.Replace("{key}", newEodId.ToString());


                    _repositoryService.GetUserEODRecipients(eodModel.UserDetailsId,
                        NotificationType.EodEditRequest).ForEach(r => { specialRecipients.Add(r); });

                    var recipientEmails = specialRecipients
                        .Select(a =>
                            new EmailAddress()
                            {
                                Email = a.Email,
                                Name = a.Name
                            }).ToList();

                    await _repositoryService.BroadcastEditedEOD(newEodId, userInfo.FullName,
                        userInfo.UserDetailsId, returnUrl, endEODDate, recipientEmails).ConfigureAwait(false);
                }
            }

            return result;
        }

        [HttpGet]
        [Route("GetSentEOD/{eodid}")]
        public async Task<ApiResult> GetSentEOD(int eodId)
        {
            var result = new ApiResult
            {
                isSuccessful = true
            };

            string eodOwnerId = _repositoryService.GetReportDetail(eodId).UserId;

            var userInfo = _repositoryService.GetUserInfoById(eodOwnerId);

            IEnumerable<UsersRawModel> assignedUsers = _repositoryService
                .GetAssignedUsersRaw(User.Identity.Name);

            if (!assignedUsers.Any(a => a.NetUserId == eodOwnerId))
            {
                result.code = APIResultCode.AccessDenied;
                result.isSuccessful = false;
                result.message = Resources.AccessDenied;
            }
            else
            {
                result.data = new
                {
                    EodData = _repositoryService.GetSentEOD(eodId),
                    Owner = new
                    {
                        Name = userInfo.FullName,
                        NetUserId = userInfo.NetUserId
                    }
                };
            }

            return result;
        }

        [HttpPost]
        [Route("eodlist")]
        public async Task<ApiResult> GetSentEODList(EodListDataRequestModel model)
        {
            ApiResult result = new ApiResult();
            result.isSuccessful = true;

            string[] allowedRoles =
                {Globals.LEAD_AGENT_RC, Globals.ACCOUNT_MANAGER_RC, Globals.SYSAD_RC, Globals.TEAM_MANAGER_RC};
            var userInfo = _repositoryService.GetUserInfoById(User.Identity.GetUserId());

            if (allowedRoles.ToList().IndexOf(userInfo.RoleCode) == -1)
            {
                result.message = Resources.EODNotAuthorized;
                result.isSuccessful = false;
                return result;
            }

            result.data = _repositoryService.GetSentEODList(model.StartDate, model.EndDate, model.WithActionOnly,
                model.UserIds, model.AccountIds, model.TeamIds, model.TagIds, model.Roles, User.Identity.Name);

            return result;
        }

        [HttpPost]
        [Route("ApproveEOD/{eodid}/{senderUserId}")]
        public async Task<ApiResult> ApproveEOD(int eodid, string senderUserId, [FromBody] string eodURL)
        {
            var result = new ApiResult();
            result.isSuccessful = true;
            result.data = true;

            string[] allowedRoles = {Globals.ACCOUNT_MANAGER_RC, Globals.TEAM_MANAGER_RC, Globals.SYSAD_RC};
            var userInfo = _repositoryService.GetUserInfoById(User.Identity.GetUserId());

            if (allowedRoles.ToList().IndexOf(userInfo.RoleCode) == -1)
            {
                result.message = Resources.EODNotAuthorized;
                result.isSuccessful = false;
                return result;
            }

            var eodStatus = _repositoryService.GetEODStatus(eodid);

            string date = "", time = "";

            switch (eodStatus.EODEnumStatus)
            {
                case EODStatus.Approved:

                    date = eodStatus.ConfirmedUTCDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    time = eodStatus.ConfirmedUTCDate.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);

                    result.message = string.Format(CultureInfo.InvariantCulture, Resources.EODAlreadyApproved,
                        eodStatus.ConfirmedByFullname, date, time);
                    result.isSuccessful = false;
                    break;
                case EODStatus.Denied:
                    date = eodStatus.ConfirmedUTCDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    time = eodStatus.ConfirmedUTCDate.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture);

                    result.message = string.Format(CultureInfo.InvariantCulture, Resources.EODAlreadyDenied,
                        eodStatus.ConfirmedByFullname, date, time);
                    result.isSuccessful = false;
                    break;
                case EODStatus.Pending:
                    await _repositoryService
                        .ApproveEOD(eodid, senderUserId, userInfo.UserDetailsId, userInfo.FullName, eodURL)
                        .ConfigureAwait(false);
                    result.message = Resources.EODApproved;
                    break;
            }

            return result;
        }

        [HttpPost]
        [Route("DenyEOD/{eodid}")]
        public async Task<ApiResult> DenyEOD(int eodid, [FromBody] string eodURL)
        {
            var result = new ApiResult();
            result.isSuccessful = true;
            result.data = true;

            string[] allowedRoles = {Globals.ACCOUNT_MANAGER_RC, Globals.TEAM_MANAGER_RC, Globals.SYSAD_RC};
            var userInfo = _repositoryService.GetUserInfoById(User.Identity.GetUserId());

            if (allowedRoles.ToList().IndexOf(userInfo.RoleCode) == -1)
            {
                result.message = Resources.EODNotAuthorized;
                result.isSuccessful = false;
                return result;
            }

            await _repositoryService.DenyEOD(eodid, userInfo.UserDetailsId, userInfo.FullName, eodURL)
                .ConfigureAwait(false);
            result.message = Resources.EODDenied;

            return result;
        }
    }
}
