using IOM.DbContext;
using IOM.Models.ApiControllerModels;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using IOM.Services.Interface;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public IList<SystemLogDataModel> GetSystemLogData(SysLogRequestDataModel model)
        {
            using (var ctx = Entities.Create())
            {
                var jsSerializer = new JavaScriptSerializer
                {
                    MaxJsonLength = 50000000
                };

                var jsonResult = string.Join("", ctx.sp_GetSystemLogs(model.StartDate, model.EndDate));
                var dataQuery = jsSerializer.Deserialize<IList<SystemLogDataModel>>(jsonResult);

                if (dataQuery != null)
                {
                    /* Filter users */
                    if (model.AccountIds.Length > 0) dataQuery = dataQuery.Where(a => a.Accounts.Where(b => model.AccountIds.Contains(b.Id)).Any()).ToList();

                    if (model.TeamIds.Length > 0) dataQuery = dataQuery.Where(a => a.Teams.Where(b => model.TeamIds.Contains(b.Id)).Any()).ToList();

                    if (model.UserIds.Length > 0) dataQuery = dataQuery.Where(d => model.UserIds.Contains(d.ActorUserId)).ToList();

                    if (model.Entities.Length > 0) dataQuery = dataQuery.Where(d => model.Entities.Contains(d.Entity)).ToList();

                    if (model.ActionTypes.Length > 0) dataQuery = dataQuery.Where(a => model.ActionTypes.Contains(a.ActionType)).ToList();

                    if (model.Roles.Length > 0) dataQuery = dataQuery.Where(a => model.Roles.Contains(a.ActorRole)).ToList();

                    dataQuery = dataQuery.OrderByDescending(d => d.LogDate).ToList();

                    return dataQuery.ToList();
                }

                return new List<SystemLogDataModel>();

            }
        }

        public object GetEntities(string query)
        {
            using (var ctx = Entities.Create())
            {
                var dataQuery = ctx.SystemLogs.AsQueryable();

                if (query.Length > 0)
                {
                    dataQuery = dataQuery.Where(d => d.Entity.Contains(query) || query.Contains(d.Entity))
                        .AsQueryable();
                }

                return dataQuery.DistinctBy(s => s.Entity)
                    .Select(e => new
                    {
                        Id = e.Entity.ToUpperInvariant(),
                        Text = e.Entity.ToUpperInvariant()
                    })
                    .ToList();
            }
        }

        public void SaveToDb(SystemLog systemLog)
        {
            using (var ctx = Entities.Create())
            {
                if (systemLog.ActionType == "activate")
                {
                    var myUri = new Uri(systemLog.RawUrl);
                    var taskId = HttpUtility.ParseQueryString(myUri.Query).Get("taskId");

                    if (taskId != null)
                    {
                        int.TryParse(taskId, out var _taskId);

                        if (_taskId > 0)
                        {
                            var taskDetails = GetTaskData(_taskId);
                            systemLog.Note = $"{taskDetails.TaskName} | {taskDetails.TaskDescription}";
                        }
                        else
                        {
                            systemLog.Note = $"Status is set to active";
                        }
                    }
                }

                ctx.SystemLogs.Add(systemLog);

                ctx.SaveChanges();
            }
        }
    }
}