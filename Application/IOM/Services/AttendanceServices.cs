using IOM.DbContext;
using IOM.Helpers;
using IOM.Models.ApiControllerModels;
using IOM.Utilities;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using IOM.Services.Interface;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public void SaveAttendanceOTUpdate(OvertimeData attendanceData, string username)
        {
            using (var ctx = Entities.Create())
            {
                var date_now = DateTimeUtility.Instance.DateTimeNow();
                var userInfo = GetCurrentUserInfo(username);
                var historyDate = DateTime.Parse(attendanceData.HistoryDate, CultureInfo.InvariantCulture);

                var existing = ctx.AttendanceOTUpdates.SingleOrDefault(a => a.UserDetailsId == attendanceData.UserDetailsId
                        && a.HistoryDate == historyDate);

                if (existing != null)
                {
                    existing.RegularHours = attendanceData.RegularHours;
                    existing.OTHours = attendanceData.OvertimeHours;
                    existing.LastUpdated = date_now;
                    existing.LastUpdatedBy = userInfo.UserDetailsId;
                }
                else
                {
                    ctx.AttendanceOTUpdates.Add(new AttendanceOTUpdate
                    {
                        UserDetailsId = attendanceData.UserDetailsId,
                        RegularHours = attendanceData.RegularHours,
                        OTHours = attendanceData.OvertimeHours,
                        HistoryDate = DateTime.Parse(attendanceData.HistoryDate, CultureInfo.InvariantCulture),
                        CreatedBy = userInfo.UserDetailsId,
                        DateCreated = date_now
                    });
                }

                ctx.SaveChanges();
            }
        }

        public object AttendanceOT(int[] userIds, int[] accountIds, int[] teamIds, string[] roles, string startDate, string endDate, string username)
        {
            var userInfo = GetCurrentUserInfo(username);

            using (var ctx = Entities.Create())
            {
                IList<sp_GetAttendanceDefault_Result> attendanceData = ctx.sp_GetAttendanceDefault(startDate, endDate).ToList();
                var taskClockers = ctx.vw_TaskClocker.ToList();

                if (userInfo.RoleCode == Globals.AGENT_RC)
                {
                    taskClockers = taskClockers.Where(r => r.UserDetailsId == userInfo.UserDetailsId).ToList();
                }
                else if (userInfo.RoleCode == Globals.LEAD_AGENT_RC)
                {
                    IEnumerable<vw_TaskClocker> agents = taskClockers.Where(a => a.Role == Globals.AGENT_RC).ToList();

                    taskClockers = taskClockers.Where(a => a.UserDetailsId == userInfo.UserDetailsId).ToList();
                    taskClockers.AddRange(agents);
                }
                else if (userInfo.RoleCode == Globals.TEAM_MANAGER_RC)
                {
                    taskClockers = taskClockers.Where(a => (a.Role == Globals.LEAD_AGENT_RC
                            || a.Role == Globals.AGENT_RC || a.UserDetailsId == userInfo.UserDetailsId)).ToList();
                }

                /**
                 * User's filter
                 */
                if (userIds.Length > 0)
                    taskClockers = taskClockers.Where(r => userIds.Contains(r.UserDetailsId)).ToList();

                /**
                 * Filter result to user's assigned accounts only
                 */
                var accountsList = GetAssignedAccountsRaw(username, false);
                var teamList = GetAssignedTeamsRaw(username, false);

                var tags = Array.Empty<string>();
   
                var assignedUsers = UserList(userIds, roles, accountIds,
                    teamIds, tags, username, false).Select(a => a.UserDetailsId).ToList();

                if (userInfo.RoleCode != Globals.SYSAD_RC)
                {
                    taskClockers = taskClockers.Where(a => assignedUsers.Contains(a.UserDetailsId)).ToList();
                }

                IList<AttendanceDefaultView> resultList = new List<AttendanceDefaultView>();

                foreach (var item in taskClockers)
                {
                    var userAttendanceData = attendanceData.Where(a => a.UserDetailsId == item.UserDetailsId).ToList();

                    IList<BaseModel> accounts = new List<BaseModel>();

                    if (item.AccountIds != null)
                    {
                        var accIds = item.AccountIds.Split(',');
                        foreach (var accId in accIds)
                        {
                            if (accountIds.Length > 0)
                            {
                                if (accountIds.Contains(Convert.ToInt32(accId, CultureInfo.InvariantCulture)))
                                {
                                    var acc = accountsList
                                        .FirstOrDefault(a => a.Id == Convert.ToInt32(accId, CultureInfo.InvariantCulture));

                                    if (acc != null)
                                    {
                                        accounts.Add(new BaseModel
                                        {
                                            Id = acc.Id,
                                            Name = acc.Name
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var acc = accountsList
                                    .FirstOrDefault(a => a.Id == Convert.ToInt32(accId, CultureInfo.InvariantCulture));

                                if (acc != null)
                                {
                                    accounts.Add(new BaseModel
                                    {
                                        Id = acc.Id,
                                        Name = acc.Name
                                    });
                                }
                            }
                        }
                    }

                    IList<BaseModel> teams = new List<BaseModel>();

                    if (item.TeamIds != null)
                    {
                        var tIds = item.TeamIds.Split(',');
                        foreach (var tId in tIds)
                        {
                            if (teamIds.Length > 0)
                            {
                                if (teamIds.Contains(Convert.ToInt32(tId, CultureInfo.InvariantCulture)))
                                {
                                    var team = teamList.FirstOrDefault(t => t.Id == Convert.ToInt32(tId, CultureInfo.InvariantCulture));
                                    if (team != null)
                                    {
                                        teams.Add(new BaseModel
                                        {
                                            Id = team.Id,
                                            Name = team.Name
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var team = teamList.FirstOrDefault(t => t.Id == Convert.ToInt32(tId, CultureInfo.InvariantCulture));
                                if (team != null)
                                {
                                    teams.Add(new BaseModel
                                    {
                                        Id = team.Id,
                                        Name = team.Name
                                    });
                                }
                            }
                        }
                    }
                    var data = new AttendanceDefaultView
                    {
                        UserDetailsId = item.UserDetailsId,
                        NetUserId = item.UserUniqueId,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        FullName = item.Fullname,
                        StaffId = item.StaffId,
                        RoleCode = item.Role,
                        Accounts = accounts,
                        Teams = teams,
                        TotalActiveTime = userAttendanceData.Sum(a => a.TotalActiveTime),
                        TotalOTTime = 0,
                        TotalRegTime = 0

                    };

                    resultList.Add(data);

                    foreach (var attData in userAttendanceData)
                    {
                        var attDefaultDat = new AttendanceDefaultData
                        {
                            TotalActiveTime = attData.TotalActiveTime,
                            HistoryDate = attData.HistoryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                            RegularHours = attData.RegularHours,
                            OTHours = attData.OTHours,
                            UpdatedRegHours = attData.UpdateRegHours,
                            UpdatedOTHours = attData.UpdateOTHours
                        };

                        if (IsWeekend(attData.HistoryDate))
                        {
                            attDefaultDat.OTHours = attData.TotalActiveTime;
                            attDefaultDat.RegularHours = 0;

                            data.TotalOTTime += attData.UpdateOTHours ?? attData.TotalActiveTime;
                            data.TotalRegTime += 0;
                        }
                        else
                        {
                            data.TotalOTTime += attData.UpdateOTHours ?? attData.OTHours;
                            data.TotalRegTime += attData.UpdateRegHours ?? attData.RegularHours;
                        }

                        data.Attendance.Add(attDefaultDat);
                    }
                }

                if (roles.Length > 0)
                {
                    resultList = resultList.Where(a => roles.Contains(a.RoleCode)).ToList();
                }

                return resultList.Where(a => GetAccessedRolesByRole(userInfo.RoleCode).Contains(a.RoleCode)).ToList();
            }
        }

        public object GetAttendanceStatusUpdates(string startDate, string endDate, string userId)
        {
            using (var ctx = Entities.Create())
            {
                return ctx.sp_GetLatestAttendanceStatusUpdates(startDate, endDate, userId, null);
            }
        }

        public object GetAttendanceRows(int AttendanceId)
        {
            using(var ctx = Entities.Create())
            {
                var attendanceRows = ctx.AttendanceRows.Where(a => a.AttendanceId == AttendanceId).ToList();

                return attendanceRows;
            }
        }

        public IList<AttendanceDefaultView> AttendanceDefault(int[] userIds, int[] accountIds, int[] teamIds, string[] roles,
            string startDate, string endDate , string username, string[] tags, bool hasLiveHoursOnly)
        {
            var userInfo = GetCurrentUserInfo(username);
            var userIdsParams = string.Join(",", userIds);
            var accountIdsParams = string.Join(",", accountIds);
            var teamIdsParams = string.Join(",", teamIds);

            using (var ctx = Entities.Create())
            {
                var jsonResult = string.Join("", ctx.sp_GetAttendanceDefaultGridData(userInfo.UserDetailsId,
                    startDate, endDate, userIdsParams, accountIdsParams, teamIdsParams).ToList());
                var dataQuery = (new JavaScriptSerializer())
                    .Deserialize<IList<AttendanceDefaultView>>(jsonResult);

                if (dataQuery == null)
                {
                    return new List<AttendanceDefaultView>();
                }

                if (tags.Length > 0)
                {
                    dataQuery = dataQuery.Where(a => a.Tags.Where(b => tags.Contains(b.Name)).Any()).ToList();
                }

                if (roles.Length > 0)
                {
                    dataQuery = dataQuery.Where(a => roles.Contains(a.RoleCode)).ToList();
                }

                if (hasLiveHoursOnly) 
                {
                    dataQuery = dataQuery.Where(a => a.TotalActiveTime > 0).ToList();
                }

                dataQuery = dataQuery.OrderByDescending(e => e.TotalActiveTime).ToList();

                return dataQuery;

            }
        }

        public IList<TaskChronoItem> GetChronoData(int userId, string date)
        {
            DateTime chronoDate = DateTime.Parse(date, CultureInfo.InvariantCulture);
            using (var ctx = Entities.Create())
            {
                var query = ctx.sp_GetChronoItems(userId, date);

                IList<TaskChronoItem> resultList = new List<TaskChronoItem>();

                query.ForEach(q =>
                {
                    resultList.Add(new TaskChronoItem
                    {
                        Id = q.Id,
                        Duration = q.Duration,
                        HistoryDate = q.HistoryDate,
                        TaskName = q.TaskName,
                        UserDetailsId = q.UserDetailsId,
                        TaskTypeName = q.TaskTypeName,
                        TaskDescription = q.TaskDescription,
                        StartTime = q.StartTime,
                        EndTime = q.EndTime,
                        TaskComment = q.Comment
                    });
                });

                return resultList;
            }
        }

        public bool IsWeekend(DateTime date)
        {
            DayOfWeek dateOfWeek = date.DayOfWeek;

            if (dateOfWeek == DayOfWeek.Saturday || dateOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }

            return false;
        }

    }
}