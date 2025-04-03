using IOM.DbContext;
using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using IOM.Services.Interface;

namespace IOM.Services
{
    public class TaskGroupServices : ITaskGroupServices
    {
        public object GetTaskGroupList(string userid)
        {
            using (var ctx = Entities.Create())
            {
                var taskGroupItemList = ctx.TaskGroupItems.AsEnumerable();

                return ctx.TaskGroups
                    .Where(a => a.IsDeleted != true)
                    .Select(a => new TaskGroupModel
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description,
                        IsActive = a.IsActive,
                        TaskItemCount = taskGroupItemList.Where(i => i.GroupId == a.Id).Count()
                    }).ToList();
            }
        }

        public object GetTaskGroup(int taskGroupId)
        {
            using (var ctx = Entities.Create())
            {
                return (from t in ctx.TaskGroups
                        where t.Id == taskGroupId
                        select new TaskGroupModel
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Description = t.Description,
                            IsActive = t.IsActive
                        }).SingleOrDefault();
            }
        }

        public void DeleteTaskGroup(TaskGroupModel tgModel)
        {
            using (var ctx = Entities.Create())
            {
                var task = ctx.TaskGroups
                    .Where(e => e.Id == tgModel.Id)
                    .SingleOrDefault();

                if (task == null)
                {
                    throw new Exception("item not found");
                }

                task.IsDeleted = true;

                ctx.SaveChanges();
            }
        }

        public void SaveTaskGroup(TaskGroupModel tgModel, string userid)
        {
            tgModel.Name = tgModel.Name.Trim();

            if (tgModel.Name == null || tgModel.Name.Trim().Length == 0)
            {
                throw new Exception(Resources.TaskGroupNameMissingErr)
                {
                    Source = ExceptionType.Thrown.ToString()
                };
            }

            using (var ctx = Entities.Create())
            {
                var task = ctx.TaskGroups
                    .Where(e => e.Name.ToLower() == tgModel.Name.ToLower() && e.Id != tgModel.Id)
                    .SingleOrDefault();

                if (task != null)
                {
                    throw new Exception("duplicate item.");
                }

                if (tgModel.Id > 0)
                {
                    var existingTask = ctx.TaskGroups.Where(t => t.Id == tgModel.Id).SingleOrDefault();

                    existingTask.Name = tgModel.Name;
                    existingTask.Description = tgModel.Description;
                }
                else
                {
                    ctx.TaskGroups.Add(new TaskGroup
                    {
                        Name = tgModel.Name,
                        Description = tgModel.Description,
                        CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                        CreatedBy = userid,
                        IsActive = true
                    });
                }

                ctx.SaveChanges();
            }
        }

        public void UpdateTaskGroupItems(List<int> taskids, int groupid, string username)
        {
            using (var ctx = Entities.Create())
            {
                using (var trans = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var savedRecords = ctx.TaskGroupItems.Where(a => a.GroupId == groupid);
                        ctx.TaskGroupItems.RemoveRange(savedRecords);
                        ctx.SaveChanges();

                        List<int> noDuplist = taskids.Distinct().ToList();

                        if (noDuplist.Count > 0)
                        {
                            foreach (var taskid in noDuplist)
                            {
                                ctx.TaskGroupItems.Add(new TaskGroupItem()
                                {
                                    CreatedBy = username,
                                    CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                                    TaskId = taskid,
                                    GroupId = groupid
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
        }

        public void UpdateUserTaskGroup(int groupId, string userId, string mode, string username)
        {
            using (var ctx = Entities.Create())
            {
                if (mode == "add")
                {
                    ctx.UserTaskGroups.Add(new UserTaskGroup()
                    {
                        CreatedBy = username,
                        CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                        GroupId = groupId,
                        UserId = userId
                    });

                    ctx.TaskGroupItems.Where(t => t.GroupId == groupId).ToList()
                        .ForEach(d => {
                            ctx.UserTasks.Add(new UserTask
                            {
                                UserId = userId,
                                GroupId = groupId,
                                TaskId = d.TaskId,
                                CreatedBy = username,
                                CreatedDate = DateTime.UtcNow
                            });
                        });
                }
                else
                {
                    var usergroup = ctx.UserTaskGroups.Where(a => a.UserId == userId 
                            && a.GroupId == groupId).FirstOrDefault();

                    if (usergroup != null) 
                    {
                        ctx.UserTaskGroups.Remove(usergroup);
                    }

                    var savedRecords = ctx.UserTasks.Where(t => t.UserId == userId && t.GroupId == groupId);

                    ctx.UserTasks.RemoveRange(savedRecords);
                }

                ctx.SaveChanges();
            }
        }

        public void UpdateUserTaskGroup(List<string> userids, int groupid, string username)
        {
            using (var ctx = Entities.Create())
            {
                using (var trans = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var savedRecords = ctx.UserTaskGroups.Where(a => a.GroupId == groupid);
                        ctx.UserTaskGroups.RemoveRange(savedRecords);
                        ctx.SaveChanges();

                        List<string> noDuplist = userids.Distinct().ToList();

                        if (noDuplist.Count > 0)
                        {
                            foreach (var userid in noDuplist)
                            {
                                ctx.UserTaskGroups.Add(new UserTaskGroup()
                                {
                                    CreatedBy = username,
                                    CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                                    UserId = userid,
                                    GroupId = groupid
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
        }

        public IList<TaskGroupModel> GetUnassigned(int teamId)
        {
            using (var ctx = Entities.Create())
            {
                var assignedTaskGroup = ctx.TeamTaskGroups.Where(t => t.TeamId == teamId)
                    .Select(d => d.GroupId).ToList();

                return ctx.TaskGroups
                    .Where(a => a.IsDeleted != true && !assignedTaskGroup.Contains(a.Id))
                    .Select(a => new TaskGroupModel
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description,
                        IsActive = a.IsActive
                    }).ToList();
            }
        }

        public void UpdateTaskGroupItems(int taskid, int groupid, string mode, string username)
        {
            using (var ctx = Entities.Create())
            {
                if (mode == "add")
                {
                    ctx.TaskGroupItems.Add(new TaskGroupItem()
                    {
                        CreatedBy = username,
                        CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                        GroupId = groupid,
                        TaskId = taskid
                    });
                }
                else
                {
                    var groupitem = ctx.TaskGroupItems.Where(a => a.TaskId == taskid && a.GroupId == groupid).FirstOrDefault();

                    if (groupid != null)
                    {
                        ctx.TaskGroupItems.Remove(groupitem);
                    }
                }

                ctx.SaveChanges();
            }
        }

        public void UpdateTeamTaskGroup(int groupid, int teamid, string mode, string username)
        {
            using (var ctx = Entities.Create())
            {
                if (mode == "add")
                {
                    ctx.TeamTaskGroups.Add(new TeamTaskGroup()
                    {
                        CreatedBy = username,
                        CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                        GroupId = groupid,
                        TeamId = teamid
                    });
                }
                else
                {
                    var teamtaskgroup = ctx.TeamTaskGroups.Where(a => a.TeamId == teamid && a.GroupId == groupid).FirstOrDefault();

                    if (teamtaskgroup != null)
                    {
                        ctx.TeamTaskGroups.Remove(teamtaskgroup);
                    }
                }

                ctx.SaveChanges();
            }
        }

        public void UpdateTeamTaskGroup(List<int> teamids, int groupid, string username)
        {
            using (var ctx = Entities.Create())
            {
                using (var trans = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var savedRecords = ctx.TeamTaskGroups.Where(a => a.GroupId == groupid);
                        ctx.TeamTaskGroups.RemoveRange(savedRecords);
                        ctx.SaveChanges();

                        List<int> noDuplist = teamids.Distinct().ToList();

                        if (noDuplist.Count > 0)
                        {
                            foreach (var teamid in noDuplist)
                            {
                                ctx.TeamTaskGroups.Add(new TeamTaskGroup()
                                {
                                    CreatedBy = username,
                                    CreatedDate = DateTimeUtility.Instance.DateTimeNow(),
                                    TeamId = teamid,
                                    GroupId = groupid
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
        }

        public TaskGroupDetails GetTaskGroupDetails(int groupid, string username)
        {
            TaskGroupDetails result = new TaskGroupDetails();

            using (var ctx = Entities.Create())
            {
                var taskgroup = ctx.TaskGroups.Where(a => a.Id == groupid).FirstOrDefault();

                if (taskgroup == null) return result;

                TaskGroupModel taskgroupinfo = new TaskGroupModel();
                taskgroupinfo.Id = taskgroup.Id;
                taskgroupinfo.Name = taskgroup.Name;
                taskgroupinfo.Description = taskgroup.Description;
                taskgroupinfo.IsActive = taskgroup.IsActive;
                result.TaskGroupInfo = taskgroupinfo;

                result.Users = (from ug in ctx.UserTaskGroups
                                join au in ctx.AspNetUsers on ug.UserId equals au.Id
                                join ud in ctx.UserDetails on au.Id equals ud.UserId
                                join rn in ctx.AspNetRoles on ud.Role equals rn.RoleCode
                                where ug.GroupId == groupid
                                select new UserBasicInfo()
                                {
                                    Name = ud.Name,
                                    UserDetailsId = ud.Id,
                                    NetUserId = ud.UserId,
                                    IsLocked = ud.IsLocked.HasValue ? ud.IsLocked.Value : false,
                                    Role = ud.Role,
                                    RoleName = rn.Name
                                }).ToList();

                result.Teams = (from ttg in ctx.TeamTaskGroups
                                join t in ctx.Teams on ttg.TeamId equals t.Id
                                where ttg.GroupId == groupid && t.IsDeleted != true
                                select new TeamBasicInfo()
                                {
                                    TeamId = t.Id,
                                    Name = t.Name,
                                    Description = t.Description,
                                    IsActive = t.IsActive
                                }).ToList();

                result.Task = (from ttg in ctx.TaskGroupItems
                                join t in ctx.IOMTasks on ttg.TaskId equals t.Id
                                where ttg.GroupId == groupid && t.IsDeleted != true
                                select new TaskModel()
                                {
                                    Id = t.Id,
                                    TaskNumber = t.TaskNumber,
                                    Name = t.Name,
                                    Description = t.Description,
                                    IsActive = t.IsActive
                                }).ToList();

            }

            return result;
        }
    }
}