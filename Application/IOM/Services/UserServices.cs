using IOM.DbContext;
using IOM.Helpers;
using IOM.Models;
using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Utilities;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Security;
using IOM.Services.Interface;
using SendGrid.Helpers.Mail;
using NotificationType = IOM.Models.NotificationType;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public UserInfoModel GetCurrentUserInfo(string username)
        {
            using (var ctx = Entities.Create())
            {
                var user = (from au in ctx.AspNetUsers
                    join ud in ctx.UserDetails on au.Id equals ud.UserId
                    join r in ctx.AspNetRoles on ud.Role equals r.RoleCode
                    from t in ctx.TimeZones.Where(e => e.Id == ud.TimeZoneId).DefaultIfEmpty()
                    where au.UserName == username && ud.IsDeleted == false
                    select new
                    {
                        UserId = au.Id,
                        au.Email,
                        UserDetailsId = ud.Id,
                        ud.Role,
                        RoleName = r.Name,
                        ud.IsLocked,
                        ud.IsDeleted,
                        au.StatusUpdateDT,
                        ud.Image,
                        ud.FirstName,
                        ud.LastName,
                        ud.Name,
                        t.Value
                    }).FirstOrDefault();

                #region User Status

                if (user == null)
                {
                    return null;
                }

                var dateNow = DateTimeUtility.Instance.DateTimeNow().Date;

                var status = (from th in ctx.TaskHistories
                    join tht in ctx.TaskHistoryTypes on th.TaskHistoryTypeId equals tht.Id
                    from tc in ctx.TaskComments.Where(c => th.Id == c.TaskHistoryId).DefaultIfEmpty()
                    from it in ctx.IOMTasks.Where(t => th.TaskId.HasValue && t.Id == th.TaskId.Value)
                        .DefaultIfEmpty()
                    where th.UserDetailsId == user.UserDetailsId &&
                          th.HistoryDate == dateNow &&
                          th.IsActive == true
                    orderby th.Start descending
                    select new
                    {
                        TaskHistoryId = th.Id,
                        TaskTypeId = (int?) tht.Id,
                        TaskType = tht.Name,
                        th.TaskId,
                        it.Name,
                        th.Duration,
                        th.Start,
                        CurrentTaskComment = tc.Comment
                    }).FirstOrDefault();

                var activeTimeTaskType = new List<int> {1, 2};

                decimal? durationTotal = 0, activeTime = 0;

                (from t in ctx.TaskHistories
                    where t.HistoryDate == dateNow &&
                          t.UserDetailsId == user.UserDetailsId &&
                          activeTimeTaskType.Contains(t.TaskHistoryTypeId)
                    select new
                    {
                        t.Duration,
                        t.Start,
                        t.IsActive
                    }).ToList().ForEach(e =>
                {
                    durationTotal += e.Duration ?? 0;

                    if (e.IsActive)
                    {
                        activeTime += (e.Start.HasValue
                            ? (decimal) (DateTimeUtility.Instance.DateTimeNow() - e.Start).Value.TotalMinutes
                            : 0);
                    }
                });

                #endregion User Status

                #region Notifications

                var notifications = ctx.Notifications.Where(n => n.ToUserId == user.UserDetailsId && n.IsRead == false)
                    .Select(e => new NotificationModel
                    {
                        Id = e.Id,
                        NoteDate = e.NoteDate,
                        Message = e.Message,
                        IsRead = e.IsRead,
                        IsArChived = e.IsArchived,
                        Icon = e.Icon,
                        Title = e.Title
                    })
                    .OrderByDescending(d => d.NoteDate)
                    .ToList();

                #endregion

                var accountMember = GetMember(user.UserDetailsId, user.Role);

                var isNameMasked = accountMember?.IsNameMasked ?? false;

                var userInfo = new UserInfoModel
                {
                    UserDetailsId = user.UserDetailsId,
                    NetUserId = user.UserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.Name,
                    IsOut = status == null,
                    ActiveTimeOnTask = (durationTotal + activeTime) / 60,
                    RoleCode = user.Role,
                    RoleName = user.RoleName,
                    IsLocked = user.IsLocked,
                    IsDeleted = user.IsDeleted,
                    Image = user.Image,
                    Permissions = GetUserPermission(user.UserDetailsId),
                    StatusUpdate = user.StatusUpdateDT,
                    Notifications = notifications,
                    IsNameMasked = isNameMasked
                };

                if (status != null)
                {
                    userInfo.TaskHistoryId = status.TaskHistoryId;
                    userInfo.CurrentTaskComment = status?.CurrentTaskComment;
                    userInfo.Status = status?.TaskType;
                    userInfo.TaskId = status?.TaskId;
                    userInfo.TaskName = status?.Name;
                    userInfo.TaskTypeId = status?.TaskTypeId;
                }

                return userInfo;
            }
        }

        public string[] GetAccessedRolesByRole(string role)
        {
            string[] result =
            {
                Globals.AGENT_RC,
                Globals.LEAD_AGENT_RC,
                Globals.TEAM_MANAGER_RC,
                Globals.ACCOUNT_MANAGER_RC,
                Globals.CLIENT_RC,
                Globals.SYSAD_RC
            };

            switch (role)
            {
                case Globals.LEAD_AGENT_RC:
                    result = new[] {Globals.AGENT_RC, Globals.LEAD_AGENT_RC};
                    break;
                case Globals.TEAM_MANAGER_RC:
                    result = new[] {Globals.AGENT_RC, Globals.LEAD_AGENT_RC, Globals.TEAM_MANAGER_RC};
                    break;
            }

            return result;
        }

        public async Task<IList<NotificationModel>> GetUserNotifications(string username)
        {
            IList<NotificationModel> result = new List<NotificationModel>();

            using (var ctx = Entities.Create())
            {
                var userinfo = await (
                    from au in ctx.AspNetUsers
                    join ud in ctx.UserDetails on au.Id equals ud.UserId
                    where au.UserName == username
                    select new {userid = ud.Id}).FirstOrDefaultAsync().ConfigureAwait(false);

                if (userinfo != null)
                {
                    result = await ctx.Notifications.Where(n => n.ToUserId == userinfo.userid && n.IsRead == false)
                        .Select(e => new NotificationModel
                        {
                            Id = e.Id,
                            NoteDate = e.NoteDate,
                            Message = e.Message,
                            IsRead = e.IsRead,
                            IsArChived = e.IsArchived,
                            Icon = e.Icon,
                            Title = e.Title
                        })
                        .OrderByDescending(e => e.NoteDate)
                        .ToListAsync().ConfigureAwait(false);
                }
            }

            return result;
        }

        public object GetTaskClockers(int[] teamIds, int[] accountIds, string username)
        {
            UserInfoModel userInfo = GetCurrentUserInfo(username);

            var assignedUsers = UserList(userInfo.UserDetailsId);

            assignedUsers = assignedUsers.Where(u => u.RoleCode == Globals.AGENT_RC
                                                     || u.RoleCode == Globals.LEAD_AGENT_RC ||
                                                     u.RoleCode == Globals.TEAM_MANAGER_RC)
                .OrderBy(o => o.FullName)
                .ToList();

            if (teamIds.Length == 0)
            {
                teamIds = GetTeamsByAccounts(accountIds, username)
                    .Select(t => t.Id).ToArray();
            }

            IList<BaseLookUpModel> users = new List<BaseLookUpModel>();

            foreach (var item in assignedUsers)
            {
                bool include = false;

                item.Teams.ForEach(t =>
                {
                    if (teamIds.Contains(t.Id))
                    {
                        include = true;
                    }
                });

                if (include)
                {
                    users.Add(new BaseLookUpModel
                    {
                        Id = item.UserDetailsId,
                        Text = item.FullName
                    });
                }
            }

            users = users.IOMDistinctBy(u => u.Id).ToList();

            return users;
        }

        public object GetAGTSByAccounts(int[] accountIds, bool includeInactive, string username)
        {
            var assignedUsers = GetAssignedUsersRaw(username, includeInactive)
                .Where(u => u.RoleCode == Globals.AGENT_RC
                            || u.RoleCode == Globals.LEAD_AGENT_RC
                            || u.RoleCode == Globals.TEAM_MANAGER_RC)
                .Select(u => new BaseLookUpModel
                {
                    Id = u.Id,
                    Text = u.Name
                })
                .ToList();

            assignedUsers = assignedUsers.IOMDistinctBy(u => u.Id).ToList();

            return assignedUsers;
        }

        public UserInfoModel GetUserInfoById(string id)
        {
            using (var ctx = Entities.Create())
            {
                return (from ud in ctx.UserDetails
                    join au in ctx.AspNetUsers on ud.UserId equals au.Id
                    join r in ctx.AspNetRoles on ud.Role equals r.RoleCode
                    from s in ctx.EmployeeShifts.Where(e => e.Id == ud.EmployeeShiftId).DefaultIfEmpty()
                    from st in ctx.EmployeeStatus.Where(e => e.Id == ud.EmployeeStatusId).DefaultIfEmpty()
                    where au.Id == id
                    select new UserInfoModel
                    {
                        UserDetailsId = ud.Id,
                        NetUserId = au.Id,
                        FirstName = ud.FirstName,
                        LastName = ud.LastName,
                        Email = au.Email,
                        RoleCode = ud.Role,
                        RoleName = r.Name,
                        FullName = ud.Name,
                        Username = au.UserName
                    }).SingleOrDefault();
            }
        }

        public string GetUserName(int userDetailId)
        {
            using (var ctx = Entities.Create())
            {
                return (from ud in ctx.UserDetails
                    join au in ctx.AspNetUsers on ud.UserId equals au.Id
                    where ud.Id == userDetailId
                    select au.UserName).SingleOrDefault();
            }
        }

        public IList<UserLookUpModel> TaskAssignees(int taskId, string username)
        {
            var userInfo = GetCurrentUserInfo(username);
            var roleCodes = new List<string>
            {
                Globals.AGENT_RC,
                Globals.LEAD_AGENT_RC,
                Globals.TEAM_MANAGER_RC
            };

            using (var ctx = Entities.Create())
            {
                var dataQuery = UserList(userInfo.UserDetailsId);

                var assignedToTask = ctx.UserTasks.Where(u => u.TaskId == taskId).Select(d => d.UserId).ToList();

                dataQuery = dataQuery.Where(d => !assignedToTask.Contains(d.NetUserId)
                                                 && roleCodes.Contains(d.RoleCode))
                    .ToList();

                return dataQuery.Select(d => new UserLookUpModel
                {
                    Id = d.UserDetailsId,
                    NetUserId = d.NetUserId,
                    Text = d.FullName,
                    RoleCode = d.RoleCode
                }).ToList();
            }
        }

        public IList<UserLookUpModel> GetAssignedUsers(string username, bool includeInactive = false,
            string query = "")
        {
            List<UsersRawModel> dataQuery = GetAssignedUsersRaw(username, includeInactive, query: query).ToList();

            return dataQuery
                .Select(d => new UserLookUpModel
                {
                    Id = d.Id,
                    Text = d.Name,
                    RoleCode = d.RoleCode,
                    NetUserId = d.NetUserId,
                    TeamIds = d.TeamIds,
                    AccountIds = d.AccountIds,
                    StaffId = d.StaffId
                }).ToList();
        }

        public IList<UsersRawModel> GetAssignedUsersRaw(string username, bool includeInActive = true,
            string query = "")
        {
            var userInfo = GetCurrentUserInfo(username);
            query = query.ToUpperInvariant();

            IList<UsersRawModel> dataQuery = UserList(userInfo.UserDetailsId, includeInActive)
                .Select(e => new UsersRawModel
                {
                    Id = e.UserDetailsId,
                    Name = e.FullName,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    IsLocked = e.IsLocked,
                    RoleCode = e.RoleCode,
                    NetUserId = e.NetUserId,
                    StaffId = e.StaffId,
                    AccountIds = e.Accounts.Select(a => a.Id).ToList(),
                    TeamIds = e.Teams.Select(t => t.Id).ToList()
                }).ToList();

            dataQuery = dataQuery.Where(d => d.Name.ToUpperInvariant().Contains(query))
                .ToList();

            return dataQuery;
        }

        public IList<UserLookUpModelv2> GetUsersLookupv2(string username, bool includeInActive = true,
            string query = "")
        {
            using (var ctx = Entities.Create())
            {
                var rawUsers = ctx.vw_ActiveUsers.ToList();
                var rawUserTeams = (from t in ctx.Teams
                    join ut in ctx.TeamMembers on t.Id equals ut.TeamId
                    select new {ut.UserDetailsId, Id = ut.TeamId, t.Name}).ToList();

                var rawUserAccounts = (from a in ctx.Accounts
                    join ua in ctx.AccountMembers on a.Id equals ua.AccountId
                    select new {ua.UserDetailsId, Id = ua.AccountId, a.Name}).ToList();

                return rawUsers.Select(u => new UserLookUpModelv2()
                {
                    Id = u.UserDetailsId,
                    NetUserId = u.NetUserId,
                    RoleCode = u.Role,
                    Text = u.FullName,
                    Accounts = rawUserAccounts.Where(a => a.UserDetailsId == u.UserDetailsId)
                        .Select(r => new CommonItem1() {Id = r.Id, Name = r.Name}).ToList(),
                    Teams = rawUserTeams.Where(a => a.UserDetailsId == u.UserDetailsId)
                        .Select(r => new CommonItem1() {Id = r.Id, Name = r.Name}).ToList(),
                }).ToList();
            }
        }

        public int GetOnlineUserCount(string username)
        {
            var userInfo = GetCurrentUserInfo(username);
            var onlineUsers = UserList(userInfo.UserDetailsId).Where(e => e.IsOnline == 1);

            return onlineUsers.ToList().Count;
        }

        public void SetUserOnline(string username)
        {
            using (var ctx = Entities.Create())
            {
                if (ctx.AspNetUsers != null)
                {
                    var user = ctx.AspNetUsers.SingleOrDefault(u => u.UserName == username);

                    if (user != null)
                    {
                        user.IsLoggedIn = true;
                        user.StatusUpdateDT = DateTimeUtility.Instance.DateTimeNow();
                    }
                }

                ctx.SaveChanges();
            }
        }

        public void SetUserOffline(string username)
        {
            using (var ctx = Entities.Create())
            {
                var user = ctx.AspNetUsers.SingleOrDefault(u => u.UserName == username);

                if (user != null)
                {
                    user.IsLoggedIn = false;
                    user.StatusUpdateDT = DateTimeUtility.Instance.DateTimeNow();
                }

                ctx.SaveChanges();
            }
        }

        public IList<UserListModel> UserList(int[] userIds, string[] roles, int[] accountIds,
            int[] teamIds, string[] tags, string username, bool showInactive = false)
        {
            var userInfo = GetCurrentUserInfo(username);
            var userIdsParams = string.Join(",", userIds);
            var accountIdsParams = string.Join(",", accountIds);
            var teamIdsParams = string.Join(",", teamIds);

            var dateNow = DateTimeUtility.Instance.DateTimeNow()
                .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            using (var ctx = Entities.Create())
            {
                var jsonResult = string.Join("", ctx.sp_GetUserList(dateNow, userInfo.UserDetailsId,
                    userIdsParams, accountIdsParams, teamIdsParams, showInactive).ToList());
                
                if (string.IsNullOrEmpty(jsonResult))
                {
                    return new List<UserListModel>();
                }
                
                var dataQuery =
                    (new JavaScriptSerializer()).Deserialize<IList<UserListModel>>(jsonResult);

                if (tags.Length > 0)
                {
                    dataQuery = dataQuery.Where(a => a.Tags.Any(b => tags.Contains(b.Name))).ToList();
                }

                if (roles.Length > 0)
                {
                    dataQuery = dataQuery.Where(a => roles.Contains(a.RoleCode)).ToList();
                }

                return dataQuery;
            }
        }

        public IList<UserListModel> UserList(int userDetailsId, bool showInactive = false)
        {
            var dateNow = DateTimeUtility.Instance.DateTimeNow()
                .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            using (var ctx = Entities.Create())
            {
                var jsonResult = string.Join("", ctx.sp_GetUserList(dateNow, userDetailsId,
                    "", "", "", showInactive).ToList());
                var dataQuery = (new JavaScriptSerializer())
                    .Deserialize<IList<UserListModel>>(jsonResult);

                return dataQuery;
            }
        }

        public int GetUsersCount(string username)
        {
            var users = GetAssignedUsersRaw(username)
                .Where(t => t.IsLocked != true)
                .Select(e => e.Id);
            return users.Count();
        }

        public void SaveImageProperty(int userDetailsId, string imageUri, string username)
        {
            using (var ctx = Entities.Create())
            {
                var user = ctx.UserDetails.SingleOrDefault(u => u.Id == userDetailsId);
                if (user != null) user.Image = imageUri;

                ctx.UserImageFiles.Where(i => i.UserDetailsId == userDetailsId && i.IsActive == true)
                    .ToList()
                    .ForEach(i => { i.IsActive = false; });

                ctx.SaveChanges();

                ctx.UserImageFiles.Add(new UserImageFile
                {
                    UserDetailsId = userDetailsId,
                    ImageUri = imageUri,
                    IsActive = true,
                    CreateDate = DateTimeUtility.Instance.DateTimeNow(),
                    CreatedBy = username
                });

                ctx.SaveChanges();
            }
        }

        public void SavePermissions(UserPermissionModel userPermissions)
        {
            using (var ctx = Entities.Create())
            {
                var dateNow = DateTimeUtility.Instance.DateTimeNow();
                ctx.AspNetUserPermissions.Where(e => e.UserId == userPermissions.UserId)
                    .ToList()
                    .ForEach(d =>
                    {
                        var updateData = userPermissions.Modules.SingleOrDefault(e => e.ModuleCode == d.ModuleCode);

                        if (updateData != null)
                        {
                            d.CanView = updateData.canView;
                            d.CanAdd = updateData.canAdd;
                            d.CanEdit = updateData.canEdit;
                            d.CanDelete = updateData.canDelete;
                        }

                        d.DateUpdated = dateNow;
                    });

                ctx.SaveChanges();
            }
        }

        public UserDetailModel GetUserDetails(int userDetailsId, string username)
        {
            using (var ctx = Entities.Create())
            {
                var userInfo = GetCurrentUserInfo(username);

                var seats = ctx.Seats.Where(a => a.UserId == userDetailsId).AsEnumerable();
                int[] activeTaskTypeIds = {1, 8, 9};
                var dateNow = DateTimeUtility.Instance.DateTimeNow();
                var strDate = $"{dateNow.Year}-{dateNow.Month}-{dateNow.Day} 00:00";
                var compDate = DateTime.Parse(strDate, CultureInfo.InvariantCulture);

                var isOnline = ctx.TaskHistories.Where(t =>
                    t.UserDetailsId == userDetailsId && t.IsActive == true &&
                    activeTaskTypeIds.Contains(t.TaskHistoryTypeId) && t.HistoryDate == compDate).ToList();

                var userData = (from ud in ctx.UserDetails
                    join au in ctx.AspNetUsers on ud.UserId equals au.Id
                    join r in ctx.AspNetRoles on ud.Role equals r.RoleCode
                    from s in ctx.EmployeeShifts.Where(e => e.Id == ud.EmployeeShiftId).DefaultIfEmpty()
                    from st in ctx.EmployeeStatus.Where(e => e.Id == ud.EmployeeStatusId).DefaultIfEmpty()
                    from t in ctx.TimeZones.Where(e => e.Id == ud.TimeZoneId).DefaultIfEmpty()
                                where ud.Id == userDetailsId && ud.IsDeleted != true && au.IsDeleted != true
                    select new UserDetailModel
                    {
                        UserDetailsId = ud.Id,
                        NetUserId = au.Id,
                        FirstName = ud.FirstName,
                        LastName = ud.LastName,
                        Email = au.Email,
                        RoleCode = ud.Role,
                        RoleName = r.Name,
                        FullName = ud.Name,
                        EmployeeStatus = st.Name,
                        EmployeeStatusId = ud.EmployeeStatusId,
                        HourlyRate = ud.HourlyRate,
                        EmployeeShiftId = ud.EmployeeShiftId,
                        ShiftName = s.Name,
                        IsLocked = ud.IsLocked,
                        AccountId = ud.AccountId.Value,
                        Image = ud.Image,
                        StaffId = ud.StaffId,
                        Username = au.UserName,
                        IsUnrestrictedIp = ud.IsUnrestrictedIp,
                        TimeZoneId = t.Id,
                        Timezone = t.Value,
                        IpAddress = ud.IPAddress
                    }).SingleOrDefault();

                if (userData != null)
                {
                    userData.WeekSchedule = ctx.UserDayOffs.Where(u => u.UserDetailsId == userData.UserDetailsId)
                        .Select(e => new DayOff
                        {
                            Day = e.Day,
                            NumericDay = e.NumericDay
                        }).ToList();

                    var accounts = (from ac in ctx.AccountMembers
                        join a in ctx.Accounts on ac.AccountId equals a.Id
                        where ac.UserDetailsId == userData.UserDetailsId && a.IsActive == true
                                                                         && a.IsDeleted != true && ac.IsDeleted != true
                        select new ProfileAccountModel
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Seat = seats.Where(se => se.AccountId == a.Id).Select(ses => ses.SeatNumber)
                                .FirstOrDefault(),
                            SeatCode = a.SeatCode,
                            Teams = (from tm in ctx.TeamMembers
                                join t in ctx.Teams on tm.TeamId equals t.Id
                                where t.AccountId == a.Id && tm.UserDetailsId == userData.UserDetailsId
                                                          && t.IsActive == true && t.IsDeleted != true &&
                                                          tm.IsDeleted != true
                                select new TeamModel
                                {
                                    Id = t.Id,
                                    Name = t.Name
                                }).ToList()
                        }).AsEnumerable().DistinctBy(e => e.Id).ToList();

                    var tags = (from ut in ctx.UserTags
                        join t in ctx.Tags on ut.TagId equals t.Id
                        where ut.UserDetailsId == userData.UserDetailsId
                        select new BaseModel
                        {
                            Id = t.Id,
                            Name = t.Name
                        }).ToList();

                    if (userInfo.IsNameMasked && userInfo.UserDetailsId != userData.UserDetailsId)
                    {
                        userData.FullName = Resources.NameMasked;
                        userData.FirstName = Resources.NameMasked;
                        userData.LastName = Resources.NameMasked;
                        userData.Email = Resources.NameMasked;
                    }

                    userData.Accounts = accounts;
                    userData.Tags = tags;

                    IList<TaskModel> tasks = (from ut in ctx.UserTasks
                        join t in ctx.IOMTasks on ut.TaskId equals t.Id
                        where ut.UserId == userData.NetUserId && t.IsActive == true && t.IsDeleted != true
                        select new TaskModel
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Description = t.Description,
                            TaskNumber = t.TaskNumber
                        }).ToList();

                    userData.Tasks = tasks.IOMDistinctBy(t => t.Id).ToList();

                    var shiftDetails = ctx.UserShiftDetails.SingleOrDefault(d => d.UserDetailsId == userDetailsId);

                    if(shiftDetails != null)
                    {
                        userData.ShiftStartTime = shiftDetails.ShiftStart.Value;
                        userData.ShiftEndTime = shiftDetails.ShiftEnd.Value;
                    }

                    var isTaskActive = ctx.TaskHistories.SingleOrDefault(t => t.UserDetailsId == userDetailsId && t.IsActive == true
                        && t.HistoryDate == compDate
                        && (t.TaskHistoryTypeId == 1 || t.TaskHistoryTypeId == 8 || t.TaskHistoryTypeId == 9));

                    if(isTaskActive != null)
                    {
                        userData.IsOnline = true;
                    }
                }

                return userData;
            }
        }

        public UserDetailModel GetUserDetails(int userDetailId)
        {
            using (var ctx = Entities.Create())
            {
                var userData = (from ud in ctx.UserDetails
                    join au in ctx.AspNetUsers on ud.UserId equals au.Id
                    join r in ctx.AspNetRoles on ud.Role equals r.RoleCode
                    from s in ctx.EmployeeShifts.Where(e => e.Id == ud.EmployeeShiftId).DefaultIfEmpty()
                    from st in ctx.EmployeeStatus.Where(e => e.Id == ud.EmployeeStatusId).DefaultIfEmpty()
                    where ud.Id == userDetailId && ud.IsDeleted != true && au.IsDeleted != true
                    select new UserDetailModel
                    {
                        UserDetailsId = ud.Id,
                        NetUserId = au.Id,
                        FirstName = ud.FirstName,
                        LastName = ud.LastName,
                        Email = au.Email,
                        RoleCode = ud.Role,
                        RoleName = r.Name,
                        FullName = ud.Name,
                        EmployeeStatus = st.Name,
                        EmployeeStatusId = ud.EmployeeStatusId,
                        HourlyRate = ud.HourlyRate,
                        EmployeeShiftId = ud.EmployeeShiftId,
                        ShiftName = s.Name,
                        IsLocked = ud.IsLocked,
                        Image = ud.Image
                    }).SingleOrDefault();

                if (userData != null)
                {
                    userData.WeekSchedule = ctx.UserDayOffs.Where(u => u.UserDetailsId == userData.UserDetailsId)
                        .Select(e => new DayOff
                        {
                            Day = e.Day,
                            NumericDay = e.NumericDay
                        }).ToList();
                }

                return userData;
            }
        }

        public IList<LookUpModel> GetEmployeeStatuses(string query)
        {
            query = query.ToLower();

            using (var ctx = Entities.Create())
            {
                var dataQuery = from s in ctx.EmployeeStatus
                    select s;

                if (query != "")
                {
                    dataQuery = dataQuery.Where(e => e.Name.ToLower().Contains(query) ||
                                                     query.Contains(e.Name.ToLower()));
                }

                return dataQuery.Select(s =>
                    new LookUpModel
                    {
                        Id = s.Id,
                        Text = s.Name,
                        Description = s.Description
                    }).ToList();
            }
        }

        public IList<LookUpModel> GetEmployeeShifts(string query)
        {
            query = query.ToLower();

            using (var ctx = Entities.Create())
            {
                var dataQuery = from s in ctx.EmployeeShifts
                    select s;

                if (query != "")
                {
                    dataQuery = dataQuery.Where(e => e.Name.ToLower().Contains(query) ||
                                                     query.Contains(e.Name.ToLower()));
                }

                return dataQuery.Select(s =>
                    new LookUpModel
                    {
                        Id = s.Id,
                        Text = s.Name,
                        Description = s.Description
                    }).ToList();
            }
        }

        public IList<LookUpModel> GetEmployeeWeekSchedule(string query)
        {
            query = query.ToLower();
            var dataQuery = new List<LookUpModel>
            {
                new LookUpModel {Id = 1, Text = DayOfWeek.Sunday.ToString()},
                new LookUpModel {Id = 2, Text = DayOfWeek.Monday.ToString()},
                new LookUpModel {Id = 3, Text = DayOfWeek.Tuesday.ToString()},
                new LookUpModel {Id = 4, Text = DayOfWeek.Wednesday.ToString()},
                new LookUpModel {Id = 5, Text = DayOfWeek.Thursday.ToString()},
                new LookUpModel {Id = 6, Text = DayOfWeek.Friday.ToString()},
                new LookUpModel {Id = 7, Text = DayOfWeek.Saturday.ToString()}
            };

            if (query != "")
            {
                dataQuery = dataQuery.Where(e => e.Text.ToLower().Contains(query) ||
                                                 query.Contains(e.Text.ToLower())).ToList();
            }

            return dataQuery;
        }

        public void SaveBasicInfo(UserModel userModel)
        {
            using (var ctx = Entities.Create())
            {
                bool updatePermissions = false;

                if (userModel.UserDetailsId > 0)
                {
                    var existingUser = ctx.UserDetails.SingleOrDefault(u => u.Id == userModel.UserDetailsId);
                    var existingAUser = ctx.AspNetUsers.SingleOrDefault(u => u.Id == existingUser.UserId);

                    if (existingUser != null)
                    {
                        updatePermissions = existingUser.Role != userModel.RoleCode;

                        existingUser.FirstName = userModel.FirstName;
                        existingUser.LastName = userModel.LastName;
                        existingUser.Name = $"{userModel.FirstName} {userModel.LastName}";
                        existingUser.Role = userModel.RoleCode;
                        existingUser.StaffId = userModel.StaffId;
                        existingAUser.UserName = userModel.Username;
                        existingUser.IsUnrestrictedIp = userModel.IsUnrestrictedIp;
                        existingUser.TimeZoneId = userModel.TimeZoneId;
                        existingUser.IsLocked = userModel.IsLocked;
                    }

                    if (existingAUser != null)
                    {
                        existingAUser.Email = userModel.Email;

                        ctx.SaveChanges();

                        ctx.sp_UpdateUserRole(existingAUser.Id, userModel.RoleCode);

                        if (updatePermissions)
                        {
                            var role = ctx.AspNetRoles.SingleOrDefault(e => e.RoleCode == userModel.RoleCode);
                            ctx.sp_UpdateUserPermissions(role.Id, existingAUser.Id);

                            ctx.AccountMembers.Where(a => a.UserDetailsId == userModel.UserDetailsId)
                                .ForEach(am => { am.IsDeleted = true; });

                            ctx.TeamMembers.Where(t => t.UserDetailsId == userModel.UserDetailsId)
                                .ForEach(tm => { tm.IsDeleted = true; });

                            ctx.SaveChanges();
                        }
                    }
                }
            }
        }

        public void SaveUser(UserDetailModel userDetailModel, string username)
        {
            using (var ctx = Entities.Create())
            {
                if (userDetailModel.UserDetailsId > 0)
                {
                    var existingUser = ctx.UserDetails
                        .SingleOrDefault(u => u.Id == userDetailModel.UserDetailsId);
                    var existingAUser = ctx.AspNetUsers.Where(u => u.Id == existingUser.UserId).SingleOrDefault();

                    var updatePermissions = existingUser.Role != userDetailModel.RoleCode;

                    existingUser.FirstName = userDetailModel.FirstName;
                    existingUser.LastName = userDetailModel.LastName;
                    existingUser.Name = $"{userDetailModel.FirstName} {userDetailModel.LastName}";
                    existingAUser.Email = userDetailModel.Email;
                    existingUser.HourlyRate = userDetailModel.HourlyRate ?? 0;
                    existingUser.Role = userDetailModel.RoleCode;
                    existingUser.StaffId = userDetailModel.StaffId;

                    if (existingUser.Role == Globals.AGENT_RC || existingUser.Role == Globals.LEAD_AGENT_RC)
                    {
                        existingUser.EmployeeShiftId = userDetailModel.EmployeeShiftId;
                        existingUser.EmployeeStatusId = userDetailModel.EmployeeStatusId;
                        existingUser.HourlyRate = userDetailModel.HourlyRate;

                        var accRange = ctx.AccountMembers.Where(a => a.UserDetailsId == userDetailModel.UserDetailsId);
                        ctx.AccountMembers.RemoveRange(accRange);

                        var userSchedRange =
                            ctx.UserDayOffs.Where(s => s.UserDetailsId == userDetailModel.UserDetailsId);
                        ctx.UserDayOffs.RemoveRange(userSchedRange);

                        var taskGroupRange = ctx.UserTaskGroups.Where(t => t.UserId == existingUser.UserId);
                        ctx.UserTaskGroups.RemoveRange(taskGroupRange);

                        ctx.SaveChanges();

                        foreach (var sched in userDetailModel.WeekSchedule)
                        {
                            var day = ((DayOfWeek) sched.NumericDay).ToString();

                            ctx.UserDayOffs.Add(new UserDayOff
                            {
                                Day = day,
                                NumericDay = sched.NumericDay,
                                UserDetailsId = userDetailModel.UserDetailsId
                            });
                        }

                        userDetailModel.Accounts.ForEach(a =>
                        {
                            ctx.AccountMembers.Add(new AccountMember
                            {
                                UserDetailsId = userDetailModel.UserDetailsId,
                                AccountId = a.Id,
                                Created = DateTimeUtility.Instance.DateTimeNow()
                            });
                        });

                        userDetailModel.TaskGroups.ForEach(t =>
                        {
                            ctx.UserTaskGroups.Add(new UserTaskGroup
                            {
                                UserId = existingUser.UserId,
                                GroupId = t.Id,
                                CreatedBy = username,
                                CreatedDate = DateTime.UtcNow
                            });
                        });
                    }

                    ctx.SaveChanges();

                    ctx.sp_UpdateUserRole(existingAUser.Id, userDetailModel.RoleCode);

                    if (updatePermissions)
                    {
                        var role = ctx.AspNetRoles.SingleOrDefault(e => e.RoleCode == userDetailModel.RoleCode);

                        ctx.sp_UpdateUserPermissions(role.Id, existingAUser.Id);
                    }
                }
            }
        }

        public void SaveEmpDetails(UserDetailModel userModel)
        {
            using (var ctx = Entities.Create())
            {
                if (userModel.UserDetailsId > 0)
                {
                    var existingUser = ctx.UserDetails.Where(u => u.Id == userModel.UserDetailsId).SingleOrDefault();

                    existingUser.EmployeeStatusId = userModel.EmployeeStatusId;
                    existingUser.HourlyRate = userModel.HourlyRate;
                    existingUser.EmployeeShiftId = userModel.EmployeeShiftId;

                    var userSchedRange = ctx.UserDayOffs.Where(s => s.UserDetailsId == userModel.UserDetailsId);
                    ctx.UserDayOffs.RemoveRange(userSchedRange);

                    ctx.SaveChanges();

                    foreach (var sched in userModel.WeekSchedule)
                    {
                        var day = ((DayOfWeek) sched.NumericDay).ToString();

                        ctx.UserDayOffs.Add(new UserDayOff
                        {
                            Day = day,
                            NumericDay = sched.NumericDay,
                            UserDetailsId = userModel.UserDetailsId
                        });
                    }
                }

                ctx.SaveChanges();
            }
        }

        public async Task DeleteUserAsync(int userId, ApplicationUserManager userManager, IIdentity identity)
        {
            using (var ctx = Entities.Create())
            {
                var userToDeleteDetails = GetUserDetails(userId);

                var currentUserInfo = GetCurrentUserInfo(identity.Name);

                ctx.sp_DeleteUser(userId);

                #region Send email to deleted user

                var emailBody = EmailBody.UserDelete(new UserDetailModel
                {
                    FirstName = userToDeleteDetails.FirstName,
                    LastName = userToDeleteDetails.LastName,
                    FullName = userToDeleteDetails.FullName
                });

                await SendGridMailServices.Instance.SendAsync(new IdentityMessage
                {
                    Subject = Resources.ThankYou,
                    Body = emailBody,
                    Destination = userToDeleteDetails.Email
                }).ConfigureAwait(false);

                #endregion

                #region Send Email to Admins
                List<int> adminIds = new List<int>();

                // get managers
                var Accounts = GetAccounts(userId);
                var accIds = Accounts.Select(a => a.Id).ToList();
                //var Teams = _teamServices.GetAllTeamsByAccounts(accIds);
                var accMngrs = GetAccountManagers(accIds);
                var emailRecipients = GetAdminEmailRecipients(NotificationType.DeletedUser, out adminIds);

                emailRecipients.AddRange(accMngrs.Select(a => new EmailAddress()
                {
                    Email = a.Email,
                    Name = a.Name
                }));

                emailRecipients = emailRecipients.Distinct().ToList();

                var message = new IdentityMessage
                {
                    Subject = "iLucent [Deleted " + userToDeleteDetails.RoleName + "]",
                    Body = EmailBody.UserDeleteForAdmin(currentUserInfo.FullName,
                        userToDeleteDetails.FullName, userToDeleteDetails.Email, userToDeleteDetails.RoleName)
                };

                await SendGridMailServices.Instance.SendMultipleAsync(message, emailRecipients)
                    .ConfigureAwait(false);

                #endregion

                #region send notification to admins
                List<int> userNotificationIds = new List<int>();
                userNotificationIds.AddRange(accMngrs.Select(a => a.UserDetailsId).ToList());
                userNotificationIds.AddRange(adminIds.Select(a => a).ToList());

                userNotificationIds = userNotificationIds.Distinct().ToList();

                ctx.Notifications.AddRange(userNotificationIds.Select(a => new Notification()
                {
                    ToUserId = a,
                    NoteDate = DateTime.UtcNow,
                    Icon = "fa-bell",
                    Title = message.Subject,
                    NoteType = NotificationType.DeletedUser.ToString(),
                    Message = message.Body
                }).ToList());

                await ctx.SaveChangesAsync().ConfigureAwait(false);
                #endregion
            }
        }

        public async Task ChangePassword(ChangePasswordModel cpModel, ApplicationUserManager usermanager)
        {
            using (var ctx = Entities.Create())
            {
                IdentityResult result;

                var user = usermanager.FindById(cpModel.UserId);

                if (cpModel.CurrentPassword.Length == 0)
                {
                    var token = usermanager.GeneratePasswordResetToken(user.Id);
                    result = usermanager.ResetPassword(user.Id, token, cpModel.ConfirmPassword);
                }
                else
                {
                    result = usermanager.ChangePassword(cpModel.UserId, cpModel.CurrentPassword, cpModel.NewPassword);
                }

                if (!result.Succeeded)
                {
                    var errors = "";
                    foreach (var err in result.Errors)
                    {
                        errors +=
                            $"{err.Replace(Resources.NonLetterOrDigit, Resources.NonAlphanumric)}{Environment.NewLine}";
                    }

                    var exception = new Exception(errors)
                    {
                        Source = ExceptionType.Thrown.ToString()
                    };
                    throw exception;
                }

                var userDetail = ctx.UserDetails.SingleOrDefault(e => e.UserId == user.Id);
                if (userDetail != null)
                {
                    userDetail.TemporaryPassword = false;

                    await ctx.SaveChangesAsync().ConfigureAwait(false);

                    var emailBody = EmailBody.PasswordResetSuccess(new UserModel
                    {
                        FullName = userDetail.Name
                    });
                    await usermanager.SendEmailAsync(user.Id, Resources.PasswordResetSuccess, emailBody)
                        .ConfigureAwait(false);
                }
            }
        }

        public async Task CreateUserAsync(UserDetailModel userDetailModel,
            ApplicationUserManager userManager,
            IIdentity currentUser, CancellationToken cancellationToken)
        {
            using (var ctx = Entities.Create())
            {
                IdentityResult result;

                var userExist = ctx.AspNetUsers.FirstOrDefault(e => e.Email == userDetailModel.Email);
                if (userExist != null)
                {
                    if (userExist.IsDeleted == true)
                    {
                        userExist.IsDeleted = false;

                        var userDetail = ctx.UserDetails.FirstOrDefault(a => a.UserId == userExist.Id);

                        if (userDetail != null)
                        {
                            userDetail.IsDeleted = false;
                        }

                        await ctx.SaveChangesAsync().ConfigureAwait(false);
                        return;
                    }
                    else
                    {
                        throw new Exception(Resources.EmailExist)
                        {
                            Source = ExceptionType.Thrown.ToString()
                        };
                    }
                }

                /* IOM-049 | BUG*/
                //var username = userDetailModel.Email.Split('@')[0];
                var username = userDetailModel.Email;
                var user = new ApplicationUser {UserName = username, Email = userDetailModel.Email};

                userDetailModel.Password =
                    $@"{Membership.GeneratePassword(5, 1).ToUpper()}{
                        (new Random(DateTimeUtility.Instance.DateTimeNow().Millisecond)).Next(0, 9).ToString()}{
                            Membership.GeneratePassword(5, 1).ToLower()}";

                result = userManager.Create(user, userDetailModel.Password);

                if (result.Succeeded)
                {
                    var role = ctx.AspNetRoles.Where(r => r.RoleCode == userDetailModel.RoleCode).SingleOrDefault();

                    if (userManager.AddToRole(user.Id, role.Name).Succeeded)
                    {
                        var newUser = new UserDetail
                        {
                            UserId = user.Id,
                            FirstName = userDetailModel.FirstName,
                            LastName = userDetailModel.LastName,
                            Name = $"{userDetailModel.FirstName} {userDetailModel.LastName}",
                            CreatedBy = currentUser.Name,
                            Created = DateTimeUtility.Instance.DateTimeNow(),
                            Role = userDetailModel.RoleCode,
                            HourlyRate = userDetailModel.HourlyRate ?? 0,
                            StaffId = userDetailModel.StaffId
                        };

                        ctx.UserDetails.Add(newUser);
                        await ctx.SaveChangesAsync().ConfigureAwait(false);

                        if (newUser.Role != Globals.SYSAD_RC)
                        {
                            userDetailModel.Accounts.ForEach(a =>
                            {
                                ctx.AccountMembers.Add(new AccountMember
                                {
                                    UserDetailsId = newUser.Id,
                                    AccountId = a.Id,
                                    Created = DateTimeUtility.Instance.DateTimeNow()
                                });
                            });

                            newUser.EmployeeShiftId = userDetailModel.EmployeeShiftId;
                            newUser.EmployeeStatusId = userDetailModel.EmployeeStatusId;

                            var userSchedRange =
                                ctx.UserDayOffs.Where(s => s.UserDetailsId == userDetailModel.UserDetailsId);
                            ctx.UserDayOffs.RemoveRange(userSchedRange);

                            await ctx.SaveChangesAsync().ConfigureAwait(false);
                        }

                        ctx.RolePermissions.Where(r => r.RoleId == role.Id).ToList().ForEach(e =>
                        {
                            ctx.AspNetUserPermissions.Add(new AspNetUserPermission
                            {
                                UserId = user.Id,
                                ModuleCode = e.ModuleCode,
                                CanView = e.CanView,
                                CanAdd = e.CanAdd,
                                CanEdit = e.CanEdit,
                                CanDelete = e.CanDelete,
                                CreatedBy = "System",
                                DateCreated = DateTimeUtility.Instance.DateTimeNow()
                            });
                        });

                        await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                        userDetailModel.FullName = newUser.Name;
                        userDetailModel.RoleName = role.Name;

                        #region Send Emails to new user

                        var notification = await GetNotificationSettingsAsync(NotificationAction.AddUser, cancellationToken)
                            .ConfigureAwait(false);
                        
                        var emailSubject = notification.Subject;
                        var emailBody = notification.Message.Registration(user, new UserDetails
                        {
                            FirstName = userDetailModel.FirstName,
                            LastName = userDetailModel.LastName,
                            Name = userDetailModel.FullName,
                            TemporaryPassword = true
                        }, userDetailModel.Password);
                        await userManager.SendEmailAsync(user.Id, emailSubject, emailBody).ConfigureAwait(false);

                        #endregion Send Emails to new user

                        #region Send Email to Admins
                        
                        notification = await GetNotificationSettingsAsync(NotificationAction.AddUserAdmin, cancellationToken)
                            .ConfigureAwait(false);

                        var emailRecipients = GetAdminEmailRecipients(NotificationType.NewUserCreated, out var adminIds);

                        var message = new IdentityMessage
                        {
                            Subject = string.Format(notification.Subject, role.Name),
                            Body = notification.Message.RegistrationForAdmin(userDetailModel)
                        };

                        await SendGridMailServices.Instance.SendMultipleAsync(message, emailRecipients)
                            .ConfigureAwait(false);

                        #endregion

                        #region send notification to admins
                        ctx.Notifications.AddRange(adminIds.Select(a => new Notification()
                        {
                            ToUserId = a,
                            NoteDate = DateTime.UtcNow,
                            Icon = "fa-bell",
                            Title = message.Subject,
                            NoteType = NotificationType.NewUserCreated.ToString(),
                            Message = message.Body
                        }).ToList());

                        ctx.SaveChanges();
                        #endregion

                    }
                    else
                    {
                        throw new Exception(Resources.ErrorAddToRole)
                        {
                            Source = ExceptionType.Caught.ToString()
                        };
                    }
                }
                else
                {
                    var errors = "";
                    foreach (var err in result.Errors)
                    {
                        errors += $"{err}{Environment.NewLine}";
                    }

                    var exception = new Exception(errors)
                    {
                        Source = ExceptionType.Thrown.ToString()
                    };
                    throw exception;
                }
            }
        }

        public async Task ResetPassword(ResetPasswordModel resetPassword, ApplicationUserManager usermanager)
        {
            using (var ctx = Entities.Create())
            {
                var user = usermanager.FindByEmail(resetPassword.Email);

                var userDetail = ctx.UserDetails.Where(e => e.UserId == user.Id)
                    .Select(e => new UserModel
                    {
                        FullName = e.FirstName + " " + e.LastName
                    }).SingleOrDefault();

                var result = usermanager.ResetPassword(user.Id, resetPassword.Token, resetPassword.Password);

                if (!result.Succeeded)
                {
                    var errors = "";
                    foreach (var err in result.Errors)
                    {
                        errors +=
                            $"{err.Replace(Resources.NonLetterOrDigit, Resources.NonAlphanumric)}{Environment.NewLine}";
                    }

                    var exception = new Exception(errors)
                    {
                        Source = ExceptionType.Thrown.ToString()
                    };
                    throw exception;
                }

                var emailBody = EmailBody.PasswordResetSuccess(userDetail);
                await usermanager.SendEmailAsync(user.Id, Resources.PasswordResetSuccess, emailBody);
            }
        }

        public async Task SendResetPass(string email,
            ApplicationUserManager usermanager,
            Uri uri)
        {
            email = email.Trim();

            var user = usermanager.FindByEmail(email);

            if (user == null)
            {
                var exception = new Exception(Resources.EmailNotReg)
                {
                    Source = ExceptionType.Thrown.ToString()
                };
                throw exception;
            }

            var token = usermanager.GeneratePasswordResetToken(user.Id);
            var callbackUrl =
                $"{uri.Scheme}://{uri.Host}{ConfigurationManager.AppSettings["ResetPassRoute"]}?token={token}&userId={user.Id}&email={user.Email}";
            var emailBody = EmailBody.ForgotPassword(callbackUrl);

            await usermanager.SendEmailAsync(user.Id, Resources.PasswordReset, emailBody).ConfigureAwait(false);
        }

        public bool LockUser(IIdentity identity, int? userId)
        {
            using (var ctx = Entities.Create())
            {
                var currentUserInfo = GetCurrentUserInfo(identity.Name);

                var user = ctx.UserDetails.SingleOrDefault(e => e.Id == userId);

                var userLock = user.IsLocked.HasValue && user.IsLocked.Value;

                user.IsLocked = !userLock;

                #region send notification
                List<int> notifRecipientIds = new List<int>();

                // get super admins
                var emailRecipients = GetAdminEmailRecipients(NotificationType.SupportInquiry, out notifRecipientIds);

                // get managers
                var Accounts = GetAccounts(user.Id);
                var accIds = Accounts.Select(a => a.Id).ToList();
                var Teams = GetAllTeamsByAccounts(accIds);
                var accMngrs = GetAccountManagers(accIds);
                var sysAdmins = GetAllSystemAdmins();

                notifRecipientIds.AddRange(accMngrs.Select(a => a.UserDetailsId).ToList());
                notifRecipientIds.AddRange(sysAdmins.Select(a => a.UserDetailsId).ToList());

                foreach (var team in Teams)
                {
                    notifRecipientIds.AddRange(ctx.sp_GetTeamManagersandSupervisors(team.Id).Select(a => a.UserId)
                    .ToList());
                }

                // remove duplicates
                notifRecipientIds = notifRecipientIds.Distinct().ToList();

                ctx.Notifications.AddRange(notifRecipientIds.Select(a => new Notification()
                {
                    ToUserId = a,
                    NoteDate = DateTime.UtcNow,
                    Icon = "fa-bell",
                    Title = $"Notice: { user.Name } was deactivated by { currentUserInfo.RoleName }",
                    NoteType = NoteType.ReminderAgentStatus.ToString(),
                    Message = $"Please be advised that { currentUserInfo.FullName } had deactivated { user.Name }."
                }).ToList());
                #endregion

                ctx.SaveChanges();

                return user.IsLocked.Value;
            }
        }

        public void UpdateActiveTime(string username)
        {
            var userInfo = GetCurrentUserInfo(username);

            using (var ctx = Entities.Create())
            {
                var endDate = DateTimeUtility.Instance.DateTimeNow();
                var historyDate = DateTimeUtility.Instance.DateTimeNow().Date;

                var currentTask = ctx.TaskHistories.OrderByDescending(t => t.Start)
                    .FirstOrDefault(t => t.IsActive == true
                                         && t.UserDetailsId == userInfo.UserDetailsId
                                         && DbFunctions.TruncateTime(t.HistoryDate) == historyDate
                    );

                if (currentTask != null)
                {
                    var start = currentTask.Start ?? endDate;

                    var duration = (decimal) (endDate - start).TotalMinutes;

                    var prevDuration = currentTask.Duration ?? 0;

                    currentTask.Start = DateTimeUtility.Instance.DateTimeNow();
                    currentTask.Duration = duration + prevDuration;
                }

                var user = ctx.AspNetUsers.SingleOrDefault(e => e.Id == userInfo.NetUserId);
                user.StatusUpdateDT = endDate;

                ctx.SaveChanges();
            }
        }

        public IList<PermissionsModel> GetUserPermission(int userId)
        {
            using (var ctx = Entities.Create())
            {
                var userid = ctx.UserDetails.FirstOrDefault(x => x.Id == userId)?.UserId;

                var data = (from u in ctx.AspNetUserPermissions
                    join m in ctx.AspNetModules on u.ModuleCode equals m.ModuleCode
                    where u.UserId == userid
                    select new PermissionsModel
                    {
                        ModuleCode = u.ModuleCode,
                        ParentModule = m.ParentModuleCode,
                        IsSubModule = u.ModuleCode.Contains("s-"),
                        canAdd = u.CanAdd,
                        canView = u.CanView,
                        canEdit = u.CanEdit,
                        canDelete = u.CanDelete
                    }).ToList();

                return data;
            }
        }

        public NetRolePermission GetUserRolePermission(int userId)
        {
            using (var ctx = Entities.Create())
            {
                var user = GetUserDetails(userId);

                var role = ctx.AspNetRoles.SingleOrDefault(e => e.RoleCode == user.RoleCode);

                var userPermission = (from u in ctx.AspNetUserPermissions
                    where u.UserId == user.NetUserId
                    select new RoleModule
                    {
                        ModuleCode = u.ModuleCode,
                        canAdd = u.CanAdd,
                        canView = u.CanView,
                        canEdit = u.CanEdit,
                        canDelete = u.CanDelete
                    }).ToList();

                return new NetRolePermission
                {
                    RoleId = role.Id,
                    IsLocked = role.PermissionsLocked,
                    Modules = userPermission
                };
            }
        }

        public List<EmailAddress> GetAdminEmailRecipients(NotificationType notificationType, out List<int> adminUds)
        {
            adminUds = new List<int>();

            using (var ctx = Entities.Create())
            {
                var admins =  (from ud in ctx.UserDetails
                        join au in ctx.AspNetUsers on ud.UserId equals au.Id
                        join ns in ctx.UserNotificationSettings on ud.Id equals ns.UserDetailsId
                        where ud.Role == UserRole.SA.ToString() && ns.IsAllowed == true &&
                              ns.NotificationTypeId == (int)notificationType
                        select new UserBasicModel
                        {
                            Email = au.Email,
                            FirstName = ud.FirstName,
                            LastName = ud.LastName,
                            UserDetailsId = ud.Id
                        }).ToList();

                adminUds = admins.Select(a => a.UserDetailsId).ToList();

                return admins.Select(a => new EmailAddress()
                {
                    Email = a.Email,
                    Name = a.FirstName + " " + a.LastName
                }).ToList();
            }
        }

        public List<UserBasicModel> GetAllSystemAdmins()
        {
            using (var ctx = Entities.Create())
            {
                var admins = (from ud in ctx.UserDetails
                              join au in ctx.AspNetUsers on ud.UserId equals au.Id
                              where ud.Role == UserRole.SA.ToString()
                              select new UserBasicModel
                              {
                                  Email = au.Email,
                                  //FirstName = ud.FirstName + " " + ud.LastName,
                                  FirstName = ud.FirstName,
                                  LastName = ud.LastName,
                                  UserDetailsId = ud.Id
                              }).ToList();

                return admins;
            }
        }

        public async Task UpdateUsedIpAddressAsync(int userDetailsId, string ipAddress, CancellationToken cancellationToken)
        {
            using (var ctx = Entities.Create())
            {
                var user = ctx.UserDetails.SingleOrDefault(e => e.Id == userDetailsId);

                if (user != null) user.IPAddress = ipAddress;

                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
