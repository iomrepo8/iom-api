using IOM.DbContext;
using IOM.Helpers;
using IOM.Hubs.Models;
using IOM.Models.ApiControllerModels;
using IOM.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using IOM.Services.Interface;
using Elmah.ContentSyndication;
using IOM.Exceptions;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public IList<TimekeepingReport> ReportGridData(int[] accountIds, int[] teamIds, 
            int[] userIds, int[] tags, string[] roles, string startDate, string endDate, string username, bool hasLogHoursOnly)
        {
            var userInfo = GetCurrentUserInfo(username);
            var userIdsParams = string.Join(",", userIds);
            var accountIdsParams = string.Join(",", accountIds);
            var teamIdsParams = string.Join(",", teamIds);

            using (var ctx = Entities.Create())
            {
                var jsonResult = string.Join("", ctx.sp_GetTimekeepingReportGridData(userInfo.UserDetailsId,
                    startDate, endDate, userIdsParams, accountIdsParams, teamIdsParams).ToList());
                var dataQuery = (new JavaScriptSerializer())
                    .Deserialize<IList<TimekeepingReport>>(jsonResult);

                if(dataQuery == null)
                {
                    return new List<TimekeepingReport>();
                }

                if (roles.Length > 0)
                {
                    dataQuery = dataQuery.Where(a => roles.Contains(a.RoleCode)).ToList();
                }

                if (tags.Length > 0)
                {
                    dataQuery = dataQuery.Where(a => a.Tags.Any(b => tags.Contains(b.Id))).ToList();
                }

                if (hasLogHoursOnly)
                {
                    dataQuery = dataQuery.Where(a => a.TaskActiveTime > 0).ToList();
                }

                dataQuery = dataQuery.OrderByDescending(e => e.TaskActiveTime).ToList();

                return dataQuery;
            }
        }

        public void CollectTimeLog()
        {
            using (var ctx = Entities.Create())
            {
                ctx.sp_CollectTimeLogs();
            }
        }

        public IList<TimekeepingManagement> ManagementData(string[] roles, int[] accountIds, int[] teamIds, int[] tagIds,
                                             int[] userIds, string startDate, string endDate, string username, bool includeInactive, bool hasLiveHoursOnly,
                                             EmployeeStatusFilter statusfilter)
        {
            var userInfo = GetCurrentUserInfo(username);
            var userIdsParams = string.Join(",", userIds);
            var accountIdsParams = string.Join(",", accountIds);
            var teamIdsParams = string.Join(",", teamIds);
            var rolesParams = string.Join(",", roles);
            var tagIdsParams = string.Join(",", tagIds);

            using (var ctx = Entities.Create())
            {
                var jsonResult = string.Join("", ctx.sp_GetTimekeepingManagementGridData(userInfo.UserDetailsId,
                        startDate, endDate, userIdsParams, accountIdsParams, teamIdsParams, tagIdsParams, rolesParams)
                    .ToList());

                if (!string.IsNullOrEmpty(userInfo.UserTimezone) && userInfo.UserTimezone.Equals(UserTimeZones.TST))
                {
                    jsonResult = string.Join("", ctx.sp_TimekeepingManagementPhUsers(userInfo.UserDetailsId,
                        startDate, endDate, userIdsParams, accountIdsParams, teamIdsParams, tagIdsParams, rolesParams)
                    .ToList());
                }

                if (jsonResult.Length > 0)
                {
                    var dataQuery = (new JavaScriptSerializer())
                    .Deserialize<IList<TimekeepingManagement>>(jsonResult);

                    if (!includeInactive)
                    { 
                        dataQuery = dataQuery.Where(a => (a.IsLocked ?? false) == false).ToList();
                    }

                    if (hasLiveHoursOnly)
                    {
                        dataQuery = dataQuery.Where(a => a.TaskActiveTime > 0).ToList();
                    }

                    switch (statusfilter)
                    {
                        case EmployeeStatusFilter.All:
                            break;
                        case EmployeeStatusFilter.Present:
                            dataQuery = dataQuery.Where(a => a.Status != "Out" && a.Status != "Active").ToList();
                            break;
                        case EmployeeStatusFilter.Active:
                            dataQuery = dataQuery.Where(a => a.Status == "Active").ToList();
                            break;
                        default:
                            break;
                    }

                    dataQuery = dataQuery.OrderByDescending(e => e.TaskActiveTime).ToList();

                    return dataQuery;
                }

                return new List<TimekeepingManagement>();
            }
        }

        public object GetStatusViewData(string fromDate, string toDate, string userId, int[] userIds, string[] roles)
        {
            using (var ctx = Entities.Create())
            {
                var result = ctx.sp_GetAttendanceStatusView(fromDate, toDate, userId).AsQueryable();

                if (roles.Length > 0)
                { 
                    result = result.Where(a => roles.Contains(a.Role));
                }

                if (userIds.Length > 0)
                {
                    result = result.Where(a => userIds.Contains(a.UserDetailsId));
                }

                return result.ToList();
            }
        }

        public object GetAttendanceStatusOptions()
        {
            using (var ctx = Entities.Create())
            {
                return ctx.AttendanceStatusOptions.ToList();
            }
        }

        public void SetAttendanceStatusOptions(string mode, string statusname, int statusid)
        {
            using (var ctx = Entities.Create())
            {
                var status = ctx.AttendanceStatusOptions.Where(a => a.Id == statusid).FirstOrDefault();

                switch (mode)
                {
                    case "add":
                        ctx.AttendanceStatusOptions.Add(new AttendanceStatusOption()
                        {
                            Status_Name = statusname
                        });
                        break;
                    case "delete":
                        if (status != null)
                        {
                            ctx.AttendanceStatusOptions.Remove(status);
                        }
                        break;
                    case "edit":
                        status.Status_Name = statusname;
                        break;
                }

                ctx.SaveChanges();
            }
        }

        public void SetAttendanceStatus(string datestatus, string oldVal, string newVal, int statusOwner, string userId)
        {
            using (var ctx = Entities.Create())
            {
                var date_now = DateTimeUtility.Instance.DateTimeNow();

                ctx.AttendanceStatusUpdates.Add(new AttendanceStatusUpdate()
                {
                    CreatedBy = userId,
                    CreatedDate = date_now,
                    NewStatus = newVal,
                    OldStatus = oldVal,
                    StatusDate = datestatus,
                    UserDetailsId = statusOwner
                });

                ctx.SaveChanges();
            }
        }

        public void SaveAttendanceData(IList<AttendanceDay> attendanceData, string username)
        {
            using (var ctx = Entities.Create())
            {
                foreach (var item in attendanceData)
                {
                    if (item.Id > 0)
                    {
                        var existingData = ctx.Attendances.SingleOrDefault(a => a.Id == item.Id);

                        existingData.WorkedDay = item.WorkedDay;
                    }
                    else
                    {
                        ctx.Attendances.Add(new Attendance
                        {
                            UserDetailsId = item.UserDetailsId,
                            AttendanceDate = (DateTime)item.SDate,
                            WorkedDay = item.WorkedDay,
                            WorkedHours = CalculateWorkedHours((DateTime) item.StartTime, (DateTime) item.EndTime, item.MgtEdit),
                            CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                            CreatedBy = username
                        }); ;
                    }
                }

                ctx.SaveChanges();
            }
        }

        public void SaveIndividualAttendanceData(AttendanceIndividual attendanceIndividual, string username)
        {
            using (var ctx = Entities.Create())
            {
                string timeFormat = "HH:mm";
                double timeDifferenceInHours = 0.0;

                DateTime initialStartVar = DateTime.Now;
                DateTime initialEndVar = DateTime.Now;

                if (DateTime.TryParseExact(attendanceIndividual?.Start, timeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime startresult) 
                    && DateTime.TryParseExact(attendanceIndividual?.End, timeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime endresult))
                {
                    DateTime tempAttDate = (DateTime)attendanceIndividual.SDate;
                    DateTime attDate = new DateTime(tempAttDate.Year, tempAttDate.Month, tempAttDate.Day, tempAttDate.Hour, tempAttDate.Minute, tempAttDate.Second, tempAttDate.Millisecond);
                    initialStartVar = new DateTime(tempAttDate.Year, tempAttDate.Month, tempAttDate.Day, startresult.Hour, startresult.Minute, startresult.Second, startresult.Millisecond);
                    initialEndVar = new DateTime(tempAttDate.Year, tempAttDate.Month, tempAttDate.Day, endresult.Hour, endresult.Minute, endresult.Second, endresult.Millisecond);
                    /*initialStartVar = startresult;
                    initialEndVar = endresult;*/
                    TimeSpan timeDifference = initialEndVar - initialStartVar;
                    timeDifferenceInHours = timeDifference.TotalHours;
                }

                var existingAttendanceIfAny = ctx.Attendances.SingleOrDefault(a => a.UserDetailsId == attendanceIndividual.UserDetailsId && a.AttendanceDate == attendanceIndividual.SDate);
                if (existingAttendanceIfAny != null)
                {
                    var existingData = ctx.Attendances.SingleOrDefault(a => a.Id == existingAttendanceIfAny.Id);
                    existingData.UpdatedDate = DateTimeUtility.Instance.DateTimeNow();
                    existingData.UpdatedBy = username;
                    existingData.TotalHours = timeDifferenceInHours;
                    existingData.StartTime = initialStartVar;
                    existingData.EndTime = initialEndVar;

                }
                else
                {
                    ctx.Attendances.Add(new Attendance
                    {
                        UserDetailsId = attendanceIndividual.UserDetailsId,
                        WorkedHours = timeDifferenceInHours,
                        AttendanceDate = (DateTime)attendanceIndividual.SDate,
                        /*WorkedDay = item.WorkedDay,*/
                        TotalHours = timeDifferenceInHours,
                        CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                        CreatedBy = username,
                        StartTime = initialStartVar,
                        EndTime = initialEndVar
                    }); 
                }
                ctx.SaveChanges();
            }
        }

        public void SaveAttendanceRow(AttendanceRowModel attendanceRowModel, string username)
        {
            using(var ctx = Entities.Create())
            {
                var attendanceCapturedIfAny = ctx.Attendances.FirstOrDefault(a => a.AttendanceDate == attendanceRowModel.AttendanceDate && a.UserDetailsId == attendanceRowModel.UserDetailsId);

                if (attendanceCapturedIfAny != null)
                {
                    var existingRowIfAny = ctx.AttendanceRows.SingleOrDefault(a => a.AttendanceTag == attendanceRowModel.AttendanceTag && a.AttendanceId == attendanceCapturedIfAny.Id);

                    if (existingRowIfAny != null)
                    {
                        var existingRow = ctx.AttendanceRows.SingleOrDefault(a => a.Id == existingRowIfAny.Id);
                        existingRow.Hours = attendanceRowModel.Hours;
                        existingRow.UpdatedBy = username;
                        existingRow.UpdatedDate = DateTimeUtility.Instance.DateTimeNow();
                    }
                    else
                    {
                        ctx.AttendanceRows.Add(new AttendanceRow
                        {
                            Hours = attendanceRowModel.Hours,
                            AttendanceTag = attendanceRowModel.AttendanceTag,
                            AttendanceId = attendanceCapturedIfAny.Id,
                            CreatedBy = username,
                            CreatedDate = DateTimeUtility.Instance.DateTimeNow()
                        });
                    }
                    ctx.SaveChanges();

                }
                else
                {
                    throw new ParentRecordNotFoundException("Add an attendance record first");
                }




                


            }
        }

        public object WeekHours(int userId, string startDate, string endDate)
        {
            var sDate = DateTime.Parse(startDate);
            var eDate = DateTime.Parse(endDate);

            var userInfo = GetUserDetails(userId);

            using (var ctx = Entities.Create())
            {
                var attendanceQuery = ctx.Attendances
                                .Where(a => a.UserDetailsId == userId && a.AttendanceDate >= sDate && a.AttendanceDate <= eDate)
                                .Select(e => new AttendanceDay
                                {
                                    Id = e.Id,
                                    WorkedDay = e.WorkedDay,
                                    /*WorkedHours = e.WorkedHours,*/
                                    SDate = e.AttendanceDate
                                }).ToList();

                return new
                {
                    UserId = userId,
                    userInfo.FullName,
                    AttendanceData = attendanceQuery
                };
            }
        }

        public decimal GetTodayHourCount(string username)
        {
            var userInfo = GetCurrentUserInfo(username);

            var currentDate = DateTimeUtility.Instance.DateTimeNow().Date;

            using (var ctx = Entities.Create())
            {
                var dataQuery = ctx.fn_TimeKeepingMgtData(
                    currentDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    currentDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
                    .ToList().AsQueryable();

                if (userInfo.RoleCode == Globals.AGENT_RC)
                {
                    dataQuery = dataQuery.Where(d => d.UserDetailId == userInfo.UserDetailsId);
                }
                else if (userInfo.RoleCode == Globals.ACCOUNT_MANAGER_RC 
                    || userInfo.RoleCode == Globals.CLIENT_RC)
                {
                    var accountIds = (from am in ctx.AccountMembers
                                      join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                      where am.UserDetailsId == userInfo.UserDetailsId && u.Role == userInfo.RoleCode
                                      select am.AccountId).ToList();

                    dataQuery = dataQuery.Where(e => accountIds.Any(t => t == e.AccountId));
                }
                else if (userInfo.RoleCode == Globals.LEAD_AGENT_RC 
                    || userInfo.RoleCode == Globals.TEAM_MANAGER_RC)
                {
                    var teamIds = ctx.TeamMembers
                        .Where(tm => tm.UserDetailsId == userInfo.UserDetailsId)
                        .Select(e => e.TeamId);

                    dataQuery = dataQuery.Where(e => teamIds.Any(t => t == e.TeamId));
                }

                IList<fn_TimeKeepingMgtData_Result> compiledList = new List<fn_TimeKeepingMgtData_Result>();

                foreach (var item in dataQuery)
                {
                    var existingItem = compiledList.SingleOrDefault(e => e.UserDetailId == item.UserDetailId);

                    if (existingItem != null)
                    {
                        existingItem.TaskActiveTime = item.TaskActiveTime;
                    }
                    else
                    {
                        compiledList.Add(item);
                    }
                }

                var activeTime = compiledList.ToList().Sum(e => e.TaskActiveTime) ?? 0;

                return (activeTime);
            }
        }

        public void UpdateUsersActiveHours()
        {
            using (var ctx = Entities.Create())
            {
                var datenow = DateTimeUtility.Instance.DateTimeNow();
                ctx.sp_UpdateUserActiveHours(datenow);
            }
        }

        public void AutoOutThreeAMUTC()
        {
            using (var ctx = Entities.Create())
            {
                var datenow = DateTimeUtility.Instance.DateTimeNow();
                ctx.sp_KickOutAllUsers(datenow);
            }
        }

        public object GetTkManagementData(TkManagementRequest request, string netUserId)
        {

            using (Entities ctx = Entities.Create())
            {
                var username = (from u in ctx.UserDetails
                                join au in ctx.AspNetUsers on u.UserId equals au.Id
                                where u.UserId == netUserId
                                select au.UserName).FirstOrDefault();

                var mgtData = ManagementData(
                request.Roles, request.AccountIds, request.TeamIds,
                request.TagIds, request.UserIds, request.StartDate,
                request.EndDate, username, request.IncludeInactive, request.HasLiveHours, request.StatusFilter);

                return new
                {
                    Management = mgtData,
                    TotalTaskTime = mgtData.AsEnumerable().Sum(e => e.TaskActiveTime),
                    TotalActiveHours = mgtData.AsEnumerable().Sum(e => e.TaskActiveTime),
                    TotalUsers = mgtData.Count
                };
            }
        }

        public double CalculateWorkedHours(DateTime? start, DateTime? end, double mgtEdit)
        {
            if (start.HasValue && end.HasValue)
            {
                var workedHours = (end - start).Value.TotalHours;
                return workedHours + mgtEdit;
            }
            else
            {
                return mgtEdit;
            }
            
        }

        public void UpdateWorkedHours(DateTime? attendanceDate, int userDetailsId, string editorId)
        {
            using(Entities ctx = Entities.Create())
            {
                double returnWorkedHours = 0.0;
                double? deduction = 0.0;
                var attendanceRecord = ctx.Attendances.Where(a => a.AttendanceDate == attendanceDate && a.UserDetailsId == userDetailsId).FirstOrDefault();

                double span = (attendanceRecord.EndTime - attendanceRecord.StartTime).Value.TotalHours;

                var mgtEditId = ctx.Tags.Where(t => t.Name == "MGT Edit").Select(t => t.Id).FirstOrDefault();
                var mgtEditHrs = ctx.UserTags.Where(t => t.TagId == mgtEditId && t.UserDetailsId == userDetailsId && t.AttendanceDate == attendanceDate).Select(t => t.Hours).FirstOrDefault();

                var lunchId = ctx.Tags.Where(t => t.Name == "Lunch").Select(t => t.Id).FirstOrDefault();
                var lunch = ctx.UserTags.Where(t => t.TagId == lunchId && t.UserDetailsId == userDetailsId && t.AttendanceDate == attendanceDate).FirstOrDefault();

                if (lunch == null)
                {
                    deduction += 1;
                }
                else
                {
                    deduction += lunch.Hours.Value;
                }

                var break1Id = ctx.Tags.Where(t => t.Name == "Break 1").Select(t => t.Id).FirstOrDefault();
                var break1Hrs = ctx.UserTags.Where(t => t.TagId == break1Id && t.UserDetailsId == userDetailsId && t.AttendanceDate == attendanceDate).Select(t => t.Hours).FirstOrDefault();
                
                if(break1Hrs > 0.25)
                {
                    deduction += break1Hrs - 0.25;
                }

                var break2Id = ctx.Tags.Where(t => t.Name == "Break 2").Select(t => t.Id).FirstOrDefault();
                var break2Hrs = ctx.UserTags.Where(t => t.TagId == break2Id && t.UserDetailsId == userDetailsId && t.AttendanceDate == attendanceDate).Select(t => t.Hours).FirstOrDefault();

                if(break2Hrs > 0.25)
                {
                    deduction += break2Hrs - 0.25;
                }

                var bioId = ctx.Tags.Where(t => t.Name == "Bio OB").Select(t => t.Id).FirstOrDefault();
                var bioHrs = ctx.UserTags.Where(t => t.TagId == bioId && t.UserDetailsId == userDetailsId && t.AttendanceDate == attendanceDate).Select(t => t.Hours).FirstOrDefault();

                if(bioHrs > 0.17)
                {
                    deduction += bioHrs - 0.17;
                }


                var tagList = ctx.UserTags.Where(t => t.UserDetailsId == userDetailsId && 
                    t.AttendanceDate == attendanceDate && (t.TagId != mgtEditId && t.TagId != break1Id && t.TagId != break2Id
                    && t.TagId != lunchId)).ToList();
                
                foreach(var tag in tagList)
                {
                    deduction += tag.Hours;
                }
                var workedHrs = (span - deduction) + mgtEditHrs;
                attendanceRecord.WorkedHours = workedHrs.Value;

                
                
                ctx.SaveChanges();

                if(attendanceDate.HasValue)
                {
                    AddAttendanceOTAfterUpdate(attendanceDate, userDetailsId, (decimal)workedHrs, editorId);
                }
                else
                {
                    attendanceDate = new DateTime(2015, 12, 25);
                    AddAttendanceOTAfterUpdate(attendanceDate, userDetailsId, (decimal)workedHrs, editorId);
                }
                
            }
        }

        public object AddAttendanceOTAfterUpdate(DateTime? attendanceDate, int userDetailsId, decimal workedHrs, string editorId)
        {
            using(Entities ctx = Entities.Create())
            {
                ctx.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                var existingOT = ctx.AttendanceOTUpdates.SingleOrDefault(a => a.UserDetailsId == userDetailsId
                        && a.HistoryDate == attendanceDate);

                if (existingOT != null)
                {
                    existingOT.HistoryDate = (DateTime)attendanceDate;
                    existingOT.RegularHours = (decimal?)workedHrs;
                    existingOT.LastUpdated = DateTime.UtcNow;
                    
                    existingOT.UserDetailsId = userDetailsId;


                    ctx.SaveChanges();
                    return existingOT;
                }
                else
                {
                    var newOTUpdate = new AttendanceOTUpdate
                    {
                        RegularHours = (decimal?)workedHrs,
                        OTHours = -(decimal?)workedHrs,
                        HistoryDate = (DateTime)attendanceDate,
                        DateCreated = DateTime.UtcNow,
                        
                        UserDetailsId = userDetailsId
                    };
                    ctx.AttendanceOTUpdates.Add(newOTUpdate);
                    ctx.SaveChanges();
                    
                    return newOTUpdate;
                }

                
            }
        }

    }
}
