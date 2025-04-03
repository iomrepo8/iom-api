using IOM.DbContext;
using IOM.Helpers;
using IOM.Models;
using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using IOM.Services.Interface;
using SendGrid.Helpers.Mail;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public IList<LookUpModel> GetTaskList(int[] teamIds)
        {
            using (var ctx = Entities.Create())
            {
                var dataQuery = ctx.TeamTasks.AsQueryable();

                /* Filter teams */
                if (teamIds != null && teamIds.Length > 0) dataQuery = dataQuery.Where(d => teamIds.Contains(d.TeamId));

                return dataQuery.Select(d => new LookUpModel
                {
                    Id = d.Id,
                    Text = d.Name,
                    Description = d.Description
                }).ToList();
            }
        }

        public object TaskLookup(string username, string query, int[] filterAccIds, int[] filterTeamIds)
        {
            var userInfo = GetCurrentUserInfo(username);
            query = query.ToLower();

            using (var ctx = Entities.Create())
            {
                IList<LookUpModel> tasks
                    = (from ut in ctx.UserTasks
                       join t in ctx.IOMTasks on ut.TaskId equals t.Id
                       where ut.UserId == userInfo.NetUserId && t.IsActive == true
                       select new LookUpModel
                       {
                           Id = t.Id,
                           Text = t.Name,
                           StringId = t.TaskNumber,
                           Description = t.Description
                       }).ToList();

                tasks = tasks.Where(t => t.Text.ToLower().Contains(query))
                    .IOMDistinctBy(t => t.Id)
                    .ToList();

                return tasks;
            }
        }

        public IList<BaseModel> GetUnassigned(int teamId, string username)
        {
            var userInfo = GetCurrentUserInfo(username);

            using (var ctx = Entities.Create())
            {
                var assignedTask = ctx.IOMTeamTasks.Where(t => t.TeamId == teamId)
                    .Select(d => d.TaskId).ToList();

                return ctx.IOMTasks
                    .Where(a => a.IsDeleted != true && a.IsActive == true && !assignedTask.Contains(a.Id))
                    .Select(a => new BaseModel
                    {
                        Id = a.Id,
                        Name = a.TaskNumber + " | " + a.Name,
                        Description = a.Description
                    }).ToList();
            }
        }

        public IList<BaseModel> GetUnassignedUserTask(string userid)
        {
            using (var ctx = Entities.Create())
            {
                var assignedTask = ctx.UserTasks.Where(a => a.UserId == userid).Select(d => d.TaskId).ToList();

                return ctx.IOMTasks
                    .Where(a => a.IsDeleted != true && a.IsActive == true && !assignedTask.Contains(a.Id))
                    .Select(a => new BaseModel
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description
                    }).ToList();
            }
        }

        public TaskComment SaveComment(TaskComment taskComment, string username)
        {
            var userInfo = GetCurrentUserInfo(username);

            using (var ctx = Entities.Create())
            {
                TaskComment existingComment = ctx.TaskComments.SingleOrDefault(t => t.TaskHistoryId == taskComment.TaskHistoryId);

                if (existingComment != null)
                {
                    existingComment.Comment = taskComment.Comment;
                    existingComment.UpdateDate = DateTimeUtility.Instance.DateTimeNow();
                }
                else
                {
                    existingComment = new TaskComment
                    {
                        Comment = taskComment.Comment,
                        TaskHistoryId = taskComment.TaskHistoryId,
                        CreatedBy = userInfo.UserDetailsId,
                        CreateDate = DateTimeUtility.Instance.DateTimeNow()
                    };

                    ctx.TaskComments.Add(existingComment);
                }

                ctx.SaveChanges();

                return existingComment;
            }
        }

        public object TaskLookup(int userId, string query)
        {
            var userInfo = GetUserDetails(userId);
            query = query.ToLower();

            using (var ctx = Entities.Create())
            {
                IList<TaskModel> tasks = (from ut in ctx.UserTasks
                                          join t in ctx.IOMTasks on ut.TaskId equals t.Id
                                          where ut.UserId == userInfo.NetUserId && t.IsActive == true
                                          select new TaskModel
                                          {
                                              Id = t.Id,
                                              Name = t.Name,
                                              Description = t.Description,
                                              TaskNumber = t.TaskNumber
                                          }).ToList();

                return tasks.Distinct().Select(d => new LookUpModel
                {
                    Id = d.Id,
                    Text = d.Name,
                    Description = d.Description,
                    StringId = d.TaskNumber
                }).ToList();
            }
        }

        public TaskDetails GetTaskData(int taskId)
        {
            using (var ctx = Entities.Create())
            {

                return (
                        from t in ctx.IOMTasks
                        where t.Id == taskId
                        select new TaskDetails
                        {
                            Id = taskId,
                            TaskName = t.Name,
                            TaskDescription = t.Description,
                            TaskNumber = t.TaskNumber
                        }
                    ).FirstOrDefault();
            }
        }

        public object GetTask(int taskId)
        {
            using (var ctx = Entities.Create())
            {
                return (from t in ctx.IOMTasks
                        where t.Id == taskId
                        select new TaskModel
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Description = t.Description,
                        }).SingleOrDefault();
            }
        }

        public void DeleteTask(TaskModel taskModel)
        {
            try
            {
                using (var ctx = Entities.Create())
                {
                    var task = ctx.IOMTasks
                        .Where(e => e.Id == taskModel.Id)
                        .SingleOrDefault();

                    if (task == null)
                    {
                        throw new Exception(Resources.TaskNotFound)
                        {
                            Source = ExceptionType.Thrown.ToString()
                        };
                    }

                    var taskHistory = ctx.TaskHistories.Where(t => t.TaskId == task.Id).ToList().Count;

                    if (taskHistory > 0)
                    {
                        task.IsDeleted = true;
                    }
                    else
                    {
                        ctx.UserTasks.RemoveRange(ctx.UserTasks.Where(a => a.TaskId == task.Id).ToList());
                        ctx.IOMTeamTasks.RemoveRange(ctx.IOMTeamTasks.Where(a => a.TaskId == task.Id).ToList());
                        ctx.TaskGroupItems.RemoveRange(ctx.TaskGroupItems.Where(a => a.TaskId == task.Id).ToList());

                        ctx.IOMTasks.Remove(task);
                    }

                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public TransactionResult SaveTask(TaskModel taskModel, string userid)
        {
            IOMTask newTask = new IOMTask();
            taskModel.Name = taskModel.Name.Trim();

            if (taskModel.Name == null || taskModel.Name.Trim().Length == 0)
            {
                return new TransactionResult(error: Resources.TaskNameMissingErr);
            }

            if (taskModel.TaskNumber == null || taskModel.TaskNumber.Trim().Length == 0)
            {
                return new TransactionResult(error: Resources.TaskNumberMissingErr);
            }

            if (taskModel.TaskNumber.Length > 12)
            {
                return new TransactionResult(Resources.TaskNumberExceedsCharacterLimit);
            }

            if (taskModel.Name.Length > 150)
            {
                return new TransactionResult(Resources.TaskNameTooLongErr);
            }

            if (taskModel.Description.Length > 250)
            {
                return new TransactionResult(Resources.TaskDescriptionTooLongErr);
            }

            using (var ctx = Entities.Create())
            {
                var task = ctx.IOMTasks
                    .Where(e => e.TaskNumber.Trim().Length > 0 && e.Id != taskModel.Id
                        && e.TaskNumber.Trim().ToLower() == taskModel.TaskNumber.Trim().ToLower())
                    .SingleOrDefault();

                if (task != null)
                {
                    return new TransactionResult(Resources.TaskNameExist);
                }

                if (taskModel.Id > 0)
                {
                    var existingTask = ctx.IOMTasks.Where(t => t.Id == taskModel.Id).SingleOrDefault();

                    existingTask.Name = taskModel.Name;
                    existingTask.Description = taskModel.Description;
                    existingTask.TaskNumber = taskModel.TaskNumber;
                    existingTask.ClickUpRef = taskModel.ClickUpLink;
                    existingTask.Manual = taskModel.Manual;
                    existingTask.Trigger = taskModel.Trigger;
                }
                else
                {
                    newTask = ctx.IOMTasks.Add(new IOMTask
                    {
                        TaskNumber = taskModel.TaskNumber,
                        Name = taskModel.Name,
                        Description = taskModel.Description,
                        ClickUpRef = taskModel.ClickUpLink,
                        Manual = taskModel.Manual,
                        Trigger = taskModel.Trigger,
                        CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                        CreatedBy = userid,
                        IsActive = true
                    });

                    taskModel.Id = newTask.Id;
                }

                ctx.SaveChanges();

                if (taskModel.Id == 0) taskModel.Id = newTask.Id;
            }

            return new TransactionResult
            {
                Data = taskModel.Id
            };
        }

        public void DeactivateTask(TaskModel taskModel)
        {
            using (var ctx = Entities.Create())
            {
                var task = ctx.IOMTasks
                    .Where(e => e.Id == taskModel.Id)
                    .SingleOrDefault();

                if (task == null)
                {
                    throw new Exception(Resources.TaskNotFound)
                    {
                        Source = ExceptionType.Thrown.ToString()
                    };
                }

                var activeRecord = ctx.TaskHistories.Where(a => a.IsActive == true && a.TaskId == taskModel.Id).FirstOrDefault();
                if (activeRecord != null)
                {
                    throw new Exception(Resources.TaskDeactivateInUseTask)
                    {
                        Source = ExceptionType.Thrown.ToString()
                    };
                }

                task.IsActive = false;

                ctx.SaveChanges();
            }
        }

        public void ActivateTask(TaskModel taskModel)
        {
            using (var ctx = Entities.Create())
            {
                var task = ctx.IOMTasks
                    .Where(e => e.Id == taskModel.Id)
                    .SingleOrDefault();

                if (task == null)
                {
                    throw new Exception(Resources.TaskNotFound)
                    {
                        Source = ExceptionType.Thrown.ToString()
                    };
                }

                task.IsActive = true;

                ctx.SaveChanges();
            }
        }

        public void TaskNotification(int taskId, bool isEnabled)
        {
            using (var ctx = Entities.Create())
            {
                var task = ctx.TeamTasks
                    .Where(e => e.Id == taskId)
                    .FirstOrDefault();

                if (task == null)
                {
                    throw new Exception(Resources.TaskNotFound)
                    {
                        Source = ExceptionType.Thrown.ToString()
                    };
                }

                task.EnableNotification = isEnabled;

                ctx.SaveChanges();
            }
        }

        public TaskAssigneesModel GetTaskAssignees(string username, int taskid)
        {
            TaskAssigneesModel result = new TaskAssigneesModel();

            using (var ctx = Entities.Create())
            {
                var task = ctx.IOMTasks.Where(a => a.Id == taskid).FirstOrDefault();

                if (task != null)
                {
                    result.TaskId = task.Id;
                    result.Name = task.Name;
                    result.Description = task.Description;
                    result.IsActive = task.IsActive;
                    result.TaskNumber = task.TaskNumber;

                    result.Users = (from ut in ctx.UserTasks
                                    join au in ctx.AspNetUsers on ut.UserId equals au.Id
                                    join ud in ctx.UserDetails on au.Id equals ud.UserId
                                    where ut.TaskId == taskid && ud.IsDeleted != true &&
                                    ut.GroupId == null && ut.TeamId == null
                                    select new UserBasicInfo()
                                    {
                                        Name = ud.Name,
                                        UserDetailsId = ud.Id,
                                        NetUserId = ud.UserId,
                                        IsLocked = ud.IsLocked.HasValue ? ud.IsLocked.Value : false
                                    }).ToList();

                    result.Teams = (from tt in ctx.IOMTeamTasks
                                    join t in ctx.Teams on tt.TeamId equals t.Id
                                    where tt.TaskId == taskid && t.IsDeleted != true
                                    select new TeamBasicInfo()
                                    {
                                        TeamId = t.Id,
                                        Name = t.Name,
                                        Description = t.Description,
                                        IsActive = t.IsActive
                                    }).ToList();

                    result.Groups = (from tg in ctx.TaskGroupItems
                                     join g in ctx.TaskGroups on tg.GroupId equals g.Id
                                     where tg.TaskId == taskid && g.IsDeleted != true
                                     select new GroupTaskAssignees()
                                     {
                                         GroupId = g.Id,
                                         Name = g.Name,
                                         Description = g.Description,
                                         IsActive = g.IsActive
                                     }).ToList();
                }
            }

            return result;
        }

        public TaskDashboardDataModel GetTaskDetails(string username, int taskid)
        {
            TaskDashboardDataModel task = new TaskDashboardDataModel();
            UserInfoModel userInfo = GetCurrentUserInfo(username);

            using (var ctx = Entities.Create())
            {
                var rawGroups = ctx.TaskGroups.ToList();
                var rawUserLookUp = ctx.vw_ActiveUsers.ToList();
                var rawUserTaskNotifications = ctx.UserTaskNotifications.ToList();
                var rawIOMTaskAssignees = ctx.UserTasks.ToList();
                var rawTeams = ctx.Teams.Where(a => a.IsDeleted != true).ToList();
                var rawAccounts = ctx.Accounts.ToList();

                List<TaskGroupModel> iomtaskgrp = new List<TaskGroupModel>();
                List<IOMTaskAssignee> iomTaskAssignees = new List<IOMTaskAssignee>();
                List<TeamModel> teams = new List<TeamModel>();

                task = ctx.IOMTasks.Where(b => b.Id == taskid).Select(c => new TaskDashboardDataModel
                {
                    IsActive = true,
                    Created = c.CreatedDate,
                    Id = c.Id,
                    NotificationEnabled = false,
                    Name = c.Name,
                    Description = c.Description,
                    TaskNumber = c.TaskNumber,
                    ClickUpLink = c.ClickUpRef,
                    Manual = c.Manual,
                    Trigger = c.Trigger
                }).FirstOrDefault();

                iomtaskgrp = (from ti in ctx.TaskGroupItems
                              join tg in ctx.TaskGroups on ti.GroupId equals tg.Id
                              where ti.TaskId == taskid
                              select new TaskGroupModel
                              {
                                  IsActive = true,
                                  Created = ti.CreatedDate,
                                  Id = ti.Id,
                                  GroupId = ti.GroupId,
                                  NotificationEnabled = false,
                                  Name = tg.Name,
                                  Description = tg.Description
                              }).ToList();

                iomTaskAssignees = rawIOMTaskAssignees.Where(a => a.TaskId == taskid).Select(b => new IOMTaskAssignee()
                {
                    NetUserId = b.UserId,
                    Name = rawUserLookUp.Where(r => r.NetUserId == b.UserId).Select(r => r.FullName).SingleOrDefault(),
                    UserdetailsId = rawUserLookUp.Where(r => r.NetUserId == b.UserId).Select(r => r.UserDetailsId).SingleOrDefault(),
                    Accounts = rawUserLookUp.Where(r => r.NetUserId == b.UserId).Select(r => r.AccountIds).SingleOrDefault(),
                    AccountsAsString = FromIdToString(rawAccounts, rawUserLookUp.Where(r => r.NetUserId == b.UserId).Select(r => r.AccountIds).SingleOrDefault()),
                    Teams = rawUserLookUp.Where(r => r.NetUserId == b.UserId).Select(r => r.TeamIds).SingleOrDefault(),
                    TeamsAsString = FromIdToString(rawTeams, rawUserLookUp.Where(r => r.NetUserId == b.UserId).Select(r => r.TeamIds).SingleOrDefault()),
                    RoleCode = rawUserLookUp.Where(r => r.NetUserId == b.UserId).Select(r => r.Role).SingleOrDefault(),
                    Role = rawUserLookUp.Where(r => r.NetUserId == b.UserId).Select(r => r.RoleName).SingleOrDefault(),
                    ProfileImage = rawUserLookUp.Where(r => r.NetUserId == b.UserId).Select(r => r.Image).SingleOrDefault()
                }).Where(a => a.Name != null).GroupBy(a => a.NetUserId).Select(g => g.First()).ToList();

                teams = (from tt in ctx.IOMTeamTasks
                         join t in ctx.Teams on tt.TeamId equals t.Id
                         where t.IsDeleted != true && tt.TaskId == taskid
                         select new TeamModel()
                         {
                             Id = t.Id,
                             Name = t.Name,
                             Description = t.Description
                         }).ToList();

                var userNotif = rawUserTaskNotifications.Where(a => a.TaskId == taskid && a.UserId == userInfo.NetUserId).FirstOrDefault();

                task.Assignees = iomTaskAssignees;
                task.Groups = iomtaskgrp;
                task.Teams = teams;
                task.UserEnabledNotification = (userNotif != null);
            }

            return task;
        }

        public async Task AssignUsersAsync(TaskUsersModel taskUsersModel, string username)
        {
            UserInfoModel userInfo = GetCurrentUserInfo(username);

            using (var ctx = Entities.Create())
            {
                if (taskUsersModel.UserIds != null)
                {
                    foreach (var item in taskUsersModel.UserIds)
                    {
                        ctx.UserTasks.Add(new UserTask
                        {
                            UserId = item,
                            TaskId = taskUsersModel.TaskId,
                            CreatedBy = username,
                            CreatedDate = DateTime.UtcNow
                        });


                        #region notification
                        List<int> notifRecipientIds = new List<int>();
                        var assignedUserInfo = GetUserInfoById(item);
                        var task = GetTaskData(taskUsersModel.TaskId);

                        //A notification will be sent to USER, (if agent) their Lead Agent &
                        //Team Manager, and account manager whenever a task is assigned.
                        // get managers
                        var notifTitle = $"A Task was assigned to {assignedUserInfo.FullName} by {userInfo.RoleName}";
                        var notifMsg = $"Please be advised that {userInfo.FullName} had assigned the Task <b>{task.TaskName}</b> with Task ID <b>{task.TaskNumber}</b> to {assignedUserInfo.FullName}.";

                        // create task for the assigned user
                        ctx.Notifications.Add(new Notification()
                        {
                            ToUserId = assignedUserInfo.UserDetailsId,
                            NoteDate = DateTime.UtcNow,
                            Icon = "fa-bell",
                            Title = notifTitle,
                            NoteType = NoteType.ReminderAgentStatus.ToString(),
                            Message = notifMsg
                        });

                        var Accounts = GetAccounts(assignedUserInfo.UserDetailsId);
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

                        //// remove duplicates
                        notifRecipientIds = notifRecipientIds.Distinct().ToList();

                        ctx.Notifications.AddRange(notifRecipientIds.Select(a => new Notification()
                        {
                            ToUserId = a,
                            NoteDate = DateTime.UtcNow,
                            Icon = "fa-bell",
                            Title = notifTitle,
                            NoteType = NoteType.ReminderAgentStatus.ToString(),
                            Message = notifMsg
                        }).ToList());
                        #endregion
                    }

                    await ctx.SaveChangesAsync().ConfigureAwait(true);
                }
            }
        }

        public List<TaskDashboardDataModel> GetTaskDashboardData(string username, int[] groupids)
        {
            List<TaskDashboardDataModel> result = new List<TaskDashboardDataModel>();
            UserInfoModel userInfo = GetCurrentUserInfo(username);

            using (var ctx = Entities.Create())
            {
                List<IOMTask> iomTaskList = new List<IOMTask>();

                var rawTaskGroupList = ctx.TaskGroupItems.ToList();

                var rawGroups = ctx.TaskGroups.ToList();
                var rawUserLookUp = ctx.vw_TaskClocker.ToList();
                var rawUserTaskNotifications = ctx.UserTaskNotifications.ToList();
                var rawTeams = ctx.Teams.Where(a => a.IsDeleted != true).ToList();
                var rawTeamTask = ctx.IOMTeamTasks.ToList();
                var rawAccounts = ctx.Accounts.ToList();

                var rawIOMTaskAssignees = ctx.UserTasks.ToList();

                // get list of task group the task belongs
                var taskgroup = new List<int>();

                if (groupids?.Length > 0)
                {
                    taskgroup = rawTaskGroupList.Where(a => groupids.Contains(a.GroupId)).Select(a => a.TaskId).ToList();
                }

                if (taskgroup.Count > 0)
                {
                    iomTaskList = ctx.IOMTasks.Where(a => taskgroup.Contains(a.Id) && a.IsDeleted != true).ToList();
                }
                else
                {
                    iomTaskList = ctx.IOMTasks.Where(a => a.IsDeleted != true).ToList();
                }

                foreach (var iomtask in iomTaskList)
                {
                    List<TaskGroupModel> iomtaskgrp = new List<TaskGroupModel>();
                    List<IOMTaskAssignee> iomTaskAssignees = new List<IOMTaskAssignee>();

                    iomtaskgrp = rawTaskGroupList.Where(b => b.TaskId == iomtask.Id).Select(c => new TaskGroupModel
                    {
                        IsActive = true,
                        Created = c.CreatedDate,
                        Id = c.Id,
                        GroupId = c.GroupId,
                        NotificationEnabled = false,
                        Name = rawGroups.Where(g => g.Id == c.GroupId).Select(d => d.Name).SingleOrDefault(),
                        Description = rawGroups.Where(g => g.Id == c.GroupId).Select(d => d.Description).SingleOrDefault()
                    }).ToList();

                    iomTaskAssignees = rawIOMTaskAssignees.Where(a => a.TaskId == iomtask.Id).Select(b => new IOMTaskAssignee()
                    {
                        NetUserId = b.UserId,
                        Name = rawUserLookUp.Where(r => r.UserUniqueId == b.UserId).Select(r => r.Fullname).SingleOrDefault(),
                        UserdetailsId = rawUserLookUp.Where(r => r.UserUniqueId == b.UserId).Select(r => r.UserDetailsId).SingleOrDefault(),
                        Accounts = rawUserLookUp.Where(r => r.UserUniqueId == b.UserId).Select(r => r.AccountIds).SingleOrDefault(),
                        AccountsAsString = FromIdToString(rawAccounts, rawUserLookUp.Where(r => r.UserUniqueId == b.UserId).Select(r => r.AccountIds).SingleOrDefault()),
                        Teams = rawUserLookUp.Where(r => r.UserUniqueId == b.UserId).Select(r => r.TeamIds).SingleOrDefault(),
                        TeamsAsString = FromIdToString(rawTeams, rawUserLookUp.Where(r => r.UserUniqueId == b.UserId).Select(r => r.TeamIds).SingleOrDefault()),
                        RoleCode = rawUserLookUp.Where(r => r.UserUniqueId == b.UserId).Select(r => r.Role).SingleOrDefault(),
                        Role = rawUserLookUp.Where(r => r.UserUniqueId == b.UserId).Select(r => r.RoleName).SingleOrDefault(),
                        ProfileImage = rawUserLookUp.Where(r => r.UserUniqueId == b.UserId).Select(r => r.Image).SingleOrDefault()
                    }).Where(a => a.Name != null).GroupBy(a => a.NetUserId).Select(g => g.First()).ToList();

                    var userNotif = rawUserTaskNotifications.Where(a => a.TaskId == iomtask.Id && a.UserId == userInfo.NetUserId).FirstOrDefault();

                    result.Add(new TaskDashboardDataModel()
                    {
                        Id = iomtask.Id,
                        Name = iomtask.Name,
                        TaskNumber = iomtask.TaskNumber,
                        Description = iomtask.Description,
                        IsActive = iomtask.IsActive,
                        Assignees = iomTaskAssignees,
                        Groups = iomtaskgrp,
                        UserEnabledNotification = (userNotif != null)
                    });
                }
            }

            return result;
        }

        public async Task<TaskAssigneeUpdate> UpdateTaskAssigneesAsync(List<string> usersid, int taskId, string username)
        {
            using (var ctx = Entities.Create())
            {
                TaskAssigneeUpdate taskAssigneeUpdate = new TaskAssigneeUpdate();

                using (var trans = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        UserInfoModel userInfo = GetCurrentUserInfo(username);
                        var users = ctx.UserDetails.ToList();
                        var taskDetails = ctx.IOMTasks.Where(a => a.Id == taskId).FirstOrDefault();

                        var savedRecords = (from ut in ctx.UserTasks
                                            join u in ctx.vw_ActiveUsers on ut.UserId equals u.NetUserId
                                            where ut.TaskId == taskId
                                            select new
                                            {
                                                u.FullName,
                                                u.UserDetailsId,
                                                u.NetUserId,
                                                ut.TaskId
                                            }).ToList();

                        var currentDate = DateTimeUtility.Instance.DateTimeNow();
                        TimeSpan ts = new TimeSpan(00, 00, 0);
                        currentDate = currentDate.Date + ts;

                        var toRemoveRecords = savedRecords.Where(a => !usersid.Contains(a.NetUserId)).ToList();

                        IList<UserTask> toRemoveUserTask = new List<UserTask>();

                        foreach (var item in toRemoveRecords)
                        {
                            var activeTask
                                    = ctx.TaskHistories.Where(a => a.TaskHistoryTypeId == 1
                                    && a.UserDetailsId == item.UserDetailsId
                                    && a.TaskId == taskId && a.IsActive == true)
                                    .OrderByDescending(o => o.HistoryDate)
                                    .FirstOrDefault();

                            if (activeTask != null && activeTask.HistoryDate.CompareTo(currentDate) == 0)
                            {
                                taskAssigneeUpdate.FailRemovalErrors
                                    .Add(string.Format(CultureInfo.InvariantCulture,
                                        Resources.TaskAssigneeRemovalError, item.FullName));
                            }
                            else
                            {
                                IList<UserTask> userTasks = ctx.UserTasks
                                    .Where(t => t.TaskId == taskId && t.UserId == item.NetUserId)
                                    .ToList();

                                toRemoveUserTask = toRemoveUserTask.Concat(userTasks).ToList();
                            }
                        }

                        if (toRemoveUserTask.Count > 0)
                        {
                            ctx.UserTasks.RemoveRange(toRemoveUserTask);

                            var toRemoveUserIds = toRemoveUserTask.Select(a => a.UserId).ToList();
                            var toRemoveUsers = users.Where(a => toRemoveUserIds.Contains(a.UserId)).ToList();

                            ctx.Notifications.AddRange(toRemoveUsers.Select(a => new Notification()
                            {
                                ToUserId = a.Id,
                                NoteDate = DateTime.UtcNow,
                                Icon = "fa-bell",
                                Title = $"A Task was removed to {a.Name} by {userInfo.RoleName}",
                                NoteType = NoteType.ReminderAgentStatus.ToString(),
                                Message = $"Please be advised that {userInfo.FullName} was removed from Task <b>{taskDetails.Name}</b> with Task ID <b>{taskDetails.TaskNumber}</b> to {a.Name}."
                            }).ToList());
                        }

                        List<string> noDuplist = usersid.Where(i => !savedRecords.Any(s => s.NetUserId == i))
                            .Distinct().ToList();

                        if (noDuplist.Count > 0)
                        {
                            foreach (var userid in noDuplist)
                            {
                                ctx.UserTasks.Add(new UserTask()
                                {
                                    CreatedBy = username,
                                    CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                                    TaskId = taskId,
                                    UserId = userid
                                });
                                taskAssigneeUpdate.AddedAssigneeCount++;

                                var userDetails = users.Where(a => a.UserId == userid).FirstOrDefault();

                                ctx.Notifications.Add(new Notification()
                                {
                                    ToUserId = userDetails.Id,
                                    NoteDate = DateTime.UtcNow,
                                    Icon = "fa-bell",
                                    Title = $"A Task was assigned to {userDetails.Name} by {userInfo.RoleName}",
                                    NoteType = NoteType.ReminderAgentStatus.ToString(),
                                    Message = $"Please be advised that {userInfo.FullName} had assigned the Task <b>{taskDetails.Name}</b> with Task ID <b>{taskDetails.TaskNumber}</b> to {userDetails.Name}."
                                });
                            }
                        }

                        var sysAdmins = GetAllSystemAdmins();
                        ctx.Notifications.AddRange(sysAdmins.Select(a => new Notification()
                        {
                            ToUserId = a.UserDetailsId,
                            NoteDate = DateTime.UtcNow,
                            Icon = "fa-bell",
                            Title = $"Task {taskDetails.Name} was updated by {userInfo.RoleName}",
                            NoteType = NoteType.ReminderAgentStatus.ToString(),
                            Message = $"Please be advised that user {userInfo.FullName} updated task assignes of <b>{taskDetails.Name}</b> with Task ID <b>{taskDetails.TaskNumber}</b> from task dashboard"
                        }).ToList());

                        await ctx.SaveChangesAsync().ConfigureAwait(false);
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
                return taskAssigneeUpdate;
            }
        }

        public bool AssignTaskToGroup(List<int> groupids, int taskId, string username)
        {
            using (var ctx = Entities.Create())
            {
                using (var trans = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var savedRecords = ctx.TaskGroupItems.Where(a => a.TaskId == taskId);
                        ctx.TaskGroupItems.RemoveRange(savedRecords);

                        var currentDate = DateTimeUtility.Instance.DateTimeNow();
                        TimeSpan ts = new TimeSpan(12, 00, 0);
                        currentDate = currentDate.Date + ts;

                        var userTaskRange = ctx.UserTasks.Where(t => t.TaskId == taskId
                                        && savedRecords.Any(s => s.GroupId == t.GroupId));

                        ctx.UserTasks.RemoveRange(userTaskRange);

                        var toremoveRec = savedRecords.ToList().Where(a => !groupids.Contains(a.GroupId)).ToList();

                        if (toremoveRec.Count > 0)
                        {
                            var activeTask = ctx.TaskHistories.Where(a => a.TaskHistoryTypeId == 1 && a.TaskId == taskId && a.IsActive == true).FirstOrDefault();

                            if (activeTask != null && activeTask.HistoryDate.CompareTo(currentDate) == 0)
                            {
                                throw new Exception(Resources.TaskIsActiveError)
                                {
                                    Source = ExceptionType.Thrown.ToString()
                                };
                            }
                        }

                        ctx.TaskGroupItems.RemoveRange(savedRecords);
                        ctx.SaveChanges();

                        List<int> noDuplist = groupids.Distinct().ToList();

                        if (noDuplist.Count > 0)
                        {
                            foreach (var groupId in noDuplist)
                            {
                                ctx.TaskGroupItems.Add(new TaskGroupItem()
                                {
                                    CreatedBy = username,
                                    CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                                    TaskId = taskId,
                                    GroupId = groupId
                                });

                                var existingUserTask = ctx.UserTasks.Where(t => t.GroupId == groupId
                                    && t.TaskId == taskId).Select(d => d.UserId).ToList();

                                var taskGroupUsers = (from utg in ctx.UserTaskGroups
                                                      join u in ctx.vw_TaskClocker on utg.UserId equals u.UserUniqueId
                                                      where utg.GroupId == groupId
                                                      select u.UserUniqueId).ToList();

                                taskGroupUsers.Where(t => !existingUserTask.Any(e => e == t)).ToList().ForEach(d =>
                                {
                                    ctx.UserTasks.Add(new UserTask
                                    {
                                        UserId = d,
                                        TaskId = taskId,
                                        GroupId = groupId,
                                        CreatedDate = DateTime.UtcNow,
                                        CreatedBy = username
                                    });
                                });
                            }

                            ctx.SaveChanges();
                        }

                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return true;
        }

        public bool AssignTeamsToTask(List<int> teamsid, int taskId, string username)
        {

            using (var ctx = Entities.Create())
            {
                using (var transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        //clear previous records
                        var savedTeamTask = ctx.IOMTeamTasks.Where(a => a.TaskId == taskId);

                        var currentDate = DateTimeUtility.Instance.DateTimeNow();
                        TimeSpan ts = new TimeSpan(12, 00, 0);
                        currentDate = currentDate.Date + ts;

                        var toremoveRec = savedTeamTask.ToList().Where(a => !teamsid.Contains(a.TeamId)).ToList();

                        if (toremoveRec.Count > 0)
                        {
                            var activeTask = ctx.TaskHistories.Where(a => a.TaskHistoryTypeId == 1 && a.TaskId == taskId && a.IsActive == true).FirstOrDefault();

                            if (activeTask != null && activeTask.HistoryDate.CompareTo(currentDate) == 0)
                            {
                                throw new Exception(Resources.TaskIsActiveError)
                                {
                                    Source = ExceptionType.Thrown.ToString()
                                };
                            }
                        }

                        ctx.IOMTeamTasks.RemoveRange(savedTeamTask);

                        var savedusertask = ctx.UserTasks.Where(a => a.TaskId == taskId && a.TeamId > 0);
                        ctx.UserTasks.RemoveRange(savedusertask);
                        ctx.SaveChanges();

                        List<int> noDuplist = teamsid.Distinct().ToList();

                        var userdetails = ctx.UserDetails.ToList();

                        if (noDuplist.Count > 0)
                        {
                            foreach (var teamid in noDuplist)
                            {
                                ctx.IOMTeamTasks.Add(new IOMTeamTask()
                                {
                                    CreatedBy = username,
                                    CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                                    TaskId = taskId,
                                    TeamId = teamid
                                });

                                // gather all users assigned to the team and assign the task to them
                                var teamMembers = ctx.TeamMembers.Where(a => a.TeamId == teamid && a.IsDeleted != true).ToList();

                                ctx.UserTasks.AddRange(teamMembers.Select(tm => new UserTask()
                                {
                                    CreatedBy = username,
                                    CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                                    TaskId = taskId,
                                    UserId = userdetails.Where(a => a.Id == tm.UserDetailsId).Select(a => a.UserId).FirstOrDefault(),
                                    TeamId = teamid
                                }));
                            }

                            ctx.SaveChanges();
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }


            return true;
        }

        public bool UpdateUserTaskNotifications(int taskId, string username, string mode)
        {
            UserInfoModel userInfo = GetCurrentUserInfo(username);
            using (var ctx = Entities.Create())
            {
                if (mode == "add")
                {
                    ctx.UserTaskNotifications.Add(new UserTaskNotification()
                    {
                        TaskId = taskId,
                        DateCreated = DateTimeUtility.Instance.DateTimeNow(),
                        UserId = userInfo.NetUserId
                    });
                }
                else
                {
                    var tasknotification = ctx.UserTaskNotifications.Where(a => a.UserId == userInfo.NetUserId && a.TaskId == taskId).FirstOrDefault();

                    if (tasknotification != null)
                    {
                        ctx.UserTaskNotifications.Remove(tasknotification);
                    }
                }

                ctx.SaveChanges();
            }

            return true;
        }

        public bool UpdateSingleTeamTask(int taskId, int teamId, string mode, string username)
        {
            using (var ctx = Entities.Create())
            {
                if (mode == "add")
                {
                    ctx.IOMTeamTasks.Add(new IOMTeamTask()
                    {
                        TaskId = taskId,
                        CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                        TeamId = teamId,
                        CreatedBy = username
                    });

                    var existingUserTask = ctx.UserTasks.Where(t => t.TeamId == teamId
                        && t.TaskId == taskId).Select(d => d.UserId).ToList();

                    var teamMembers = (from tm in ctx.TeamMembers
                                       join u in ctx.vw_TaskClocker on tm.UserDetailsId equals u.UserDetailsId
                                       where tm.IsDeleted != true && tm.TeamId == teamId
                                       select u.UserUniqueId).ToList();

                    teamMembers.Where(t => !existingUserTask.Any(e => e == t)).ToList().ForEach(d =>
                    {
                        ctx.UserTasks.Add(new UserTask
                        {
                            UserId = d,
                            TaskId = taskId,
                            TeamId = teamId,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = username
                        });
                    });
                }
                else
                {
                    var teamtask = ctx.IOMTeamTasks.Where(a => a.TeamId == teamId && a.TaskId == taskId).FirstOrDefault();

                    if (teamtask != null)
                    {
                        ctx.IOMTeamTasks.Remove(teamtask);
                    }

                    var userTaskRange = ctx.UserTasks.Where(t => t.TaskId == taskId && t.TeamId == teamId);

                    ctx.UserTasks.RemoveRange(userTaskRange);
                }

                ctx.SaveChanges();
            }

            return true;
        }

        public async Task<bool> UpdateSingleUserTask(int taskId, string netUserId, string mode, string username)
        {
            using (var ctx = Entities.Create())
            {

                if (mode == "add")
                {
                    ctx.UserTasks.Add(new UserTask()
                    {
                        TaskId = taskId,
                        CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                        UserId = netUserId,
                        CreatedBy = username
                    });

                }
                else
                {
                    var currentDate = DateTimeUtility.Instance.DateTimeNow();
                    TimeSpan ts = new TimeSpan(00, 00, 0);
                    currentDate = currentDate.Date + ts;

                    var user = ctx.UserDetails.SingleOrDefault(u => u.UserId == netUserId);

                    var activeTask = ctx.TaskHistories.Where(a => a.TaskHistoryTypeId == 1
                                    && a.UserDetailsId == user.Id
                                    && a.TaskId == taskId && a.IsActive == true)
                                    .OrderByDescending(o => o.HistoryDate)
                                    .FirstOrDefault();

                    if (activeTask != null && activeTask.HistoryDate.CompareTo(currentDate) == 0)
                    {
                        throw new Exception(string.Format(CultureInfo.InvariantCulture,
                                        Resources.TaskAssigneeRemovalError, $"{user.FirstName} {user.LastName}"))
                        {
                            Source = ExceptionType.Thrown.ToString()
                        };
                    }
                    else
                    {
                        var usertask = ctx.UserTasks.Where(a => a.UserId == netUserId && a.TaskId == taskId)
                        .FirstOrDefault();

                        if (usertask != null)
                        {
                            ctx.UserTasks.Remove(usertask);
                        }
                    }
                }

                #region notification

                //  start send email to user
                //var userInfo2 = GetUserDetails(user.Id);
                var task = GetTaskData(taskId);

                var removeUserInfo = GetUserInfoById(netUserId);

                if (mode == "add")
                {

                    var emailmsgUser = string.Format("Please be advised that you were assigned to task {0}",
                    task.TaskName);

                    string emailBodyUser = EmailBody.TaskUpdate(emailmsgUser);

                    Task.Run(() => TaskUserEmail(new IdentityMessage()
                    {
                        Subject = "Assigned Task",
                        Body = emailBodyUser
                    },
                    removeUserInfo.Email)).Wait();

                }

                // end send user email

                var userInfo = GetCurrentUserInfo(username);
                List<int> notifRecipientIds = new List<int>();

                //A notification will be sent to USER, (if agent) their Lead Agent &
                //Team Manager, and account manager whenever a task is assigned.
                // get managers
                var notifTitle = string.Empty;
                var notifMsg = string.Empty;

                if (mode == "add")
                {
                    notifTitle = $"A Task was assigned to {removeUserInfo.FullName} by {userInfo.RoleName}";
                    notifMsg = $"Please be advised that {userInfo.FullName} had assigned the Task <b>{task.TaskName}</b> with Task ID <b>{task.TaskNumber}</b> to {removeUserInfo.FullName}.";
                }
                else
                {
                    notifTitle = $"A Task was removed to {removeUserInfo.FullName} by {userInfo.RoleName}";
                    notifMsg = $"Please be advised that {userInfo.FullName} was removed from Task <b>{task.TaskName}</b> with Task ID <b>{task.TaskNumber}</b> to {removeUserInfo.FullName}.";
                }

                ctx.Notifications.Add(new Notification()
                {
                    ToUserId = removeUserInfo.UserDetailsId,
                    NoteDate = DateTime.UtcNow,
                    Icon = "fa-bell",
                    Title = notifTitle,
                    NoteType = NoteType.ReminderAgentStatus.ToString(),
                    Message = notifMsg
                });

                var Accounts = GetAccounts(removeUserInfo.UserDetailsId);
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

                //// remove duplicates
                notifRecipientIds = notifRecipientIds.Distinct().ToList();

                ctx.Notifications.AddRange(notifRecipientIds.Select(a => new Notification()
                {
                    ToUserId = a,
                    NoteDate = DateTime.UtcNow,
                    Icon = "fa-bell",
                    Title = notifTitle,
                    NoteType = NoteType.ReminderAgentStatus.ToString(),
                    Message = notifMsg
                }).ToList());
                #endregion

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }

            return true;
        }

        public async Task TaskUserEmail(IdentityMessage message,
            string userEmail)
        {

            var recipient = userEmail;

            await SendTaskUpdateEmailUser(message, recipient).ConfigureAwait(false);
        }

        public async Task SendTaskUpdateEmailUser(IdentityMessage message,
            string recipient)
        {

            var emailSubject = message.Subject;
            var emailBody = message.Body;

            var emailRecipient = recipient;

            await SendGridMailServices.Instance.SendAsync(new IdentityMessage
            {
                Subject = emailSubject,
                Body = emailBody,
                Destination = emailRecipient
            }).ConfigureAwait(false);

        }

        public bool UpdateSingleGroupToTask(int taskId, int groupId, string mode, string username)
        {
            using (var ctx = Entities.Create())
            {
                if (mode == "add")
                {
                    ctx.TaskGroupItems.Add(new TaskGroupItem()
                    {
                        TaskId = taskId,
                        CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                        GroupId = groupId,
                        CreatedBy = username
                    });

                    var existingUserTask = ctx.UserTasks.Where(t => t.GroupId == groupId
                        && t.TaskId == taskId).Select(d => d.UserId).ToList();

                    var taskGroupUsers = (from utg in ctx.UserTaskGroups
                                          join u in ctx.vw_TaskClocker on utg.UserId equals u.UserUniqueId
                                          where utg.GroupId == groupId
                                          select u.UserUniqueId).ToList();

                    taskGroupUsers.Where(t => !existingUserTask.Any(e => e == t)).ToList().ForEach(d =>
                    {
                        ctx.UserTasks.Add(new UserTask
                        {
                            UserId = d,
                            TaskId = taskId,
                            GroupId = groupId,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = username
                        });
                    });
                }
                else
                {
                    var taskgroup = ctx.TaskGroupItems.Where(a => a.GroupId == groupId && a.TaskId == taskId)
                        .FirstOrDefault();

                    if (taskgroup != null)
                    {
                        ctx.TaskGroupItems.Remove(taskgroup);
                    }

                    var userTaskRange = ctx.UserTasks.Where(t => t.TaskId == taskId && t.GroupId == groupId);

                    ctx.UserTasks.RemoveRange(userTaskRange);
                }

                ctx.SaveChanges();
            }

            return true;
        }

        /* PLEASE REFACTOR THIS MESS */
        public void SetStatus_(int? taskId, int taskTypeId, int userDetailsId)
        {
            using (var ctx = Entities.Create())
            {
                var date_now = DateTimeUtility.Instance.DateTimeNow().Date;
                var endDate = DateTimeUtility.Instance.DateTimeNow();
                var userInfo = GetUserDetails(userDetailsId);

                var activeTask = ctx.TaskHistories.SingleOrDefault(t =>
                            t.IsActive == true &&
                            t.HistoryDate == date_now &&
                            t.UserDetailsId == userDetailsId);

                taskId = taskId > 0 ? taskId : null;

                if (activeTask != null)
                {
                    var start = activeTask.Start;

                    var duration = (decimal)(endDate - start.Value).TotalMinutes;
                    var prevDuration = activeTask.Duration ?? 0;

                    activeTask.Duration = duration + prevDuration;

                    if (taskId == activeTask.TaskId && activeTask.TaskHistoryTypeId == taskTypeId)
                    {
                        activeTask.Start = endDate;
                    }
                    else
                    {
                        activeTask.IsActive = false;

                        if (taskId == null)
                        {
                            if (taskTypeId == 4 && ctx.TaskHistories.Any(t => t.TaskHistoryTypeId == taskTypeId
                             && t.HistoryDate == date_now
                             && t.UserDetailsId == userDetailsId))
                            {
                                if (ctx.TaskHistories.Any(t => t.TaskHistoryTypeId == 5
                                 && t.HistoryDate == date_now
                                 && t.UserDetailsId == userDetailsId))
                                {
                                    throw new Exception(Resources.BreakLimit);
                                }
                                else
                                {
                                    taskTypeId = 5;
                                }

                                ctx.TaskHistories.Add(new TaskHistory
                                {
                                    TaskId = taskId,
                                    UserDetailsId = userDetailsId,
                                    HistoryDate = date_now,
                                    Start = endDate,
                                    IsActive = true,
                                    TaskHistoryTypeId = taskTypeId,
                                    ActivateTime = DateTimeUtility.Instance.DateTimeNow()
                                });
                            }
                            else
                            {
                                var prevTask = ctx.TaskHistories.FirstOrDefault(t =>
                                    t.TaskHistoryTypeId == taskTypeId &&
                                    t.HistoryDate == date_now &&
                                    t.UserDetailsId == userDetailsId);

                                if (prevTask != null)
                                {
                                    prevTask.IsActive = true;
                                    prevTask.Start = endDate;
                                    prevTask.ActivateTime = DateTimeUtility.Instance.DateTimeNow();
                                }
                                else
                                {
                                    ctx.TaskHistories.Add(new TaskHistory
                                    {
                                        TaskId = taskId,
                                        UserDetailsId = userDetailsId,
                                        HistoryDate = date_now,
                                        Start = endDate,
                                        IsActive = true,
                                        TaskHistoryTypeId = taskTypeId,
                                        ActivateTime = DateTimeUtility.Instance.DateTimeNow()
                                    });
                                }
                            }
                        }
                        else
                        {
                            var prevTask = ctx.TaskHistories.FirstOrDefault(t =>
                                t.TaskId == taskId &&
                                t.HistoryDate == date_now &&
                                t.UserDetailsId == userDetailsId &&
                                t.TaskHistoryTypeId == taskTypeId);

                            if (prevTask != null)
                            {
                                prevTask.IsActive = true;
                                prevTask.Start = endDate;
                            }
                            else
                            {
                                ctx.TaskHistories.Add(new TaskHistory
                                {
                                    TaskId = taskId,
                                    UserDetailsId = userDetailsId,
                                    HistoryDate = date_now,
                                    Start = endDate,
                                    IsActive = true,
                                    TaskHistoryTypeId = taskTypeId,
                                    ActivateTime = DateTimeUtility.Instance.DateTimeNow()
                                });
                            }
                        }
                    }
                }
                else
                {
                    ctx.TaskHistories.Add(new TaskHistory
                    {
                        TaskId = taskId,
                        UserDetailsId = userDetailsId,
                        HistoryDate = date_now,
                        Start = endDate,
                        IsActive = true,
                        TaskHistoryTypeId = taskTypeId,
                        ActivateTime = DateTimeUtility.Instance.DateTimeNow()
                    });
                }

                var user = ctx.AspNetUsers.SingleOrDefault(e => e.Id == userInfo.NetUserId);
                user.StatusUpdateDT = endDate;

                ctx.SaveChanges();
            }
        }

        public TransactionResult SetStatus(int? taskId, int taskTypeId, int userDetailsId)
        {
            using (var ctx = Entities.Create())
            {
                var dateNow = DateTimeUtility.Instance.DateTimeNow().Date;
                var endDate = DateTimeUtility.Instance.DateTimeNow();
                var userInfo = GetUserDetails(userDetailsId);

                taskId = taskId > 0 ? taskId : null;

                switch (taskTypeId)
                {
                    case 1:
                        var userTask = ctx.UserTasks
                            .FirstOrDefault(a => a.TaskId == taskId && a.UserId == userInfo.NetUserId);

                        if (userTask == null)
                        {
                            throw new Exception(Resources.InvalidTaskToActivate);
                        }
                        break;
                    case 4:
                        if (ctx.TaskHistories.Any(t =>
                                t.TaskHistoryTypeId == 4 &&
                                t.Duration > 0 && 
                                t.HistoryDate == dateNow &&
                                t.UserDetailsId == userDetailsId))
                        {
                            if (ctx.TaskHistories.Any(t =>
                                    t.TaskHistoryTypeId == 5 &&
                                    t.Duration > 0 && 
                                    t.HistoryDate == dateNow &&
                                    t.UserDetailsId == userDetailsId))
                            {
                                return new TransactionResult(Resources.BreakLimit);
                            }

                            taskTypeId = 5;
                        }
                        break;
                }

                ctx.TaskHistories.Where(t =>
                                        t.IsActive == true &&
                                        t.HistoryDate == dateNow &&
                                        t.UserDetailsId == userDetailsId).ToList()
                    .ForEach(t =>
                    {
                        t.IsActive = false;
                    });

                ctx.SaveChanges();

                var task = new TaskHistory
                {
                    TaskId = taskId,
                    UserDetailsId = userDetailsId,
                    HistoryDate = dateNow,
                    Start = endDate,
                    IsActive = true,
                    TaskHistoryTypeId = taskTypeId,
                    ActivateTime = DateTimeUtility.Instance.DateTimeNow()
                };

                ctx.TaskHistories.Add(task);

                var user = ctx.AspNetUsers.SingleOrDefault(e => e.Id == userInfo.NetUserId);
                user.StatusUpdateDT = endDate;

                ctx.SaveChanges();

                return new TransactionResult
                {
                    Data = task
                };
            }
        }

        public async Task BroadcastTaskActivation(int taskId, string userFullName, int userId, string username)
        {
            using (var ctx = Entities.Create())
            {
                List<SendGrid.Helpers.Mail.EmailAddress> emailRecipients = new List<SendGrid.Helpers.Mail.EmailAddress>();
                var task = ctx.IOMTasks.Where(a => a.Id == taskId).FirstOrDefault();
                var usersWithNotifications = (from n in ctx.UserTaskNotifications
                                              join u in ctx.vw_ActiveUsers on n.UserId equals u.NetUserId
                                              where n.TaskId == taskId
                                              select new
                                              {
                                                  UserDetailsId = u.UserDetailsId,
                                                  NetUserId = u.NetUserId,
                                                  Email = u.Email,
                                                  FullName = u.FullName,
                                                  RoleCode = u.Role
                                              }).ToList();
                var userlist = ctx.vw_ActiveUsers.ToList();

                var timeUtc = DateTime.UtcNow;
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

                string dateStr = easternTime.ToShortDateString();
                string timeStr = easternTime.ToShortTimeString();
                string accountName = string.Empty;
                string teamName = string.Empty;

                string notificationMsg = string.Empty;
                UserInfoModel userInfo = GetCurrentUserInfo(username);

                if (task != null)
                {
                    switch (userInfo.RoleCode)
                    {
                        case Globals.AGENT_RC:
                            usersWithNotifications = usersWithNotifications
                                .Where(u => u.RoleCode != Globals.AGENT_RC)
                                .ToList();
                            break;
                        case Globals.LEAD_AGENT_RC:
                            usersWithNotifications = usersWithNotifications
                                .Where(u => u.RoleCode != Globals.AGENT_RC || u.RoleCode != Globals.LEAD_AGENT_RC)
                                .ToList();
                            break;
                        case Globals.TEAM_MANAGER_RC:
                            usersWithNotifications = usersWithNotifications
                                .Where(u => u.RoleCode != Globals.AGENT_RC || u.RoleCode != Globals.LEAD_AGENT_RC
                                         || u.RoleCode != Globals.TEAM_MANAGER_RC)
                                .ToList();
                            break;
                    }

                    foreach (var item in usersWithNotifications)
                    {
                        if (item.NetUserId == userInfo.NetUserId) continue;

                        if (string.IsNullOrEmpty(notificationMsg))
                        {
                            notificationMsg
                                = $@"Please note that {userFullName} has selected {task.Name} on {dateStr} at {timeStr}";
                        }

                        ctx.Notifications.Add(new Notification()
                        {
                            ToUserId = item.UserDetailsId,
                            NoteDate = easternTime,
                            Icon = "fa-bell",
                            Title = $"A Task was selected by {userFullName}",
                            NoteType = NoteType.TaskNotification.ToString(),
                            Message = notificationMsg
                        });

                        emailRecipients.Add(new SendGrid.Helpers.Mail.EmailAddress()
                        {
                            Email = item.Email,
                            Name = item.FullName
                        });
                    }

                    ctx.SaveChanges();

                    // Send Email
                    string emailBody = GenerateTaskNotificationEmailBody(userFullName, task.Name, teamName, accountName, easternTime);

                    await SendGridMailServices.Instance.SendMultipleAsync(new IdentityMessage()
                    {
                        Subject = $"Task Notification - {accountName}",
                        Body = emailBody
                    }, emailRecipients).ConfigureAwait(false);
                }

            }
        }

        public string GenerateTaskNotificationEmailBody(string name, string taskName, string tname, string actname, DateTime est)
        {
            var body = ReadTemplate("TaskActivationNotification.html");

            var dateStr = est.ToShortDateString();
            var timeStr = est.ToShortTimeString();

            var emailContent = body
                .Replace("{{user}}", name)
                .Replace("{{taskname}}", taskName)
                .Replace("{{accountname}}", actname)
                .Replace("{{teamname}}", tname)
                .Replace("{{date}}", dateStr)
                .Replace("{{time}}", timeStr);

            return emailContent;
        }

        private static string ReadTemplate(string file)
        {
            using (var reader = new StreamReader(HostingEnvironment.MapPath("~\\Templates\\" + file)))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// from "1, 2, ,3, 4" to "team1, team2, team3, team4"
        /// </summary>
        /// <param name="teamList"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public string FromIdToString(List<Team> teamList, string ids)
        {
            if (ids == null) return string.Empty;

            var idsLists = ids.Trim().Split(',').ToList();
            var teamNames = teamList.Where(a => idsLists.IndexOf(a.Id.ToString()) > 0).Select(a => a.Name).ToList();
            return string.Join(" • ", teamNames);
        }

        public string FromIdToString(List<Account> accounts, string ids)
        {
            if (ids == null) return string.Empty;

            var idsLists = ids.Trim().Split(',').ToList();
            var accountNames = accounts.Where(a => idsLists.IndexOf(a.Id.ToString()) > 0).Select(a => a.Name).ToList();
            return string.Join(" • ", accountNames);
        }
    }
}
