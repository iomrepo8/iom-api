using IOM.DbContext;
using IOM.Helpers;
using IOM.Models;
using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Services;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using IOM.Services.Interface;

namespace IOM.Controllers.WebApi
{
    [RoutePrefix("task")]
    [Authorize]
    public class TaskController : ApiController
    {
        private readonly IRepositoryService _repositoryService;

        public TaskController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        
        [Route("list")]
        [HttpGet]
        public ApiResult TaskLookup([FromUri] int[] teamId)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetTaskList(teamId)
            };

            return result;
        }

        [HttpGet]
        [Route("assigned")]
        public ApiResult TaskLookup([FromUri] int[] accountIds, [FromUri] int[] teamIds
                                   , [FromUri] string q = "")
        {
            var result = new ApiResult
            {
                data = _repositoryService.TaskLookup(User.Identity.Name, q, accountIds, teamIds)
            };

            return result;
        }

        [HttpGet]
        [Route("unassigned")]
        public ApiResult TaskUnassigned([FromUri] int teamId)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetUnassigned(teamId, User.Identity.Name)
            };

            return result;
        }

        [HttpGet]
        [Route("unassignedusertask")]
        public ApiResult Unassignedusertask([FromUri] string userid)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetUnassignedUserTask(userid)
            };

            return result;
        }

        [HttpGet]
        [Route("for")]
        public ApiResult TaskLookup([FromUri] int userId, [FromUri] string q = "")
        {
            var result = new ApiResult
            {
                data = _repositoryService.TaskLookup(userId, q)
            };

            return result;
        }

        [HttpPost]
        [Route("activate")]
        public async Task<ApiResult> SetActive([FromUri] int taskId, [FromUri] int taskTypeId)
        {
            var result = new ApiResult();

            var userInfo = _repositoryService.GetCurrentUserInfo(User.Identity.Name);

            var transData = _repositoryService.SetStatus(taskId, taskTypeId, userInfo.UserDetailsId);

            if (!transData.IsSuccessful)
            {
                result.message = transData.Error.ToString();
                result.isSuccessful = false;

                return result;
            }

            var task = (TaskHistory) transData.Data;

            if (taskId > 0)
            {
                await _repositoryService.BroadcastTaskActivation(taskId, userInfo.FullName, userInfo.UserDetailsId, User.Identity.Name).ConfigureAwait(false);
            }

            userInfo = _repositoryService.GetCurrentUserInfo(User.Identity.Name);
            userInfo.TaskHistoryId = task.Id;

            result.data = userInfo;

            return result;
        }

        [HttpPost]
        [Route("save_comment")]
        public async Task<ApiResult> SaveComment([FromBody] TaskComment taskComment)
        {
            var result = new ApiResult();

            var task = _repositoryService.SaveComment(taskComment, User.Identity.Name);

            result.data = task;

            return result;
        }

        [HttpGet]
        [Route("get")]
        public ApiResult GetTask([FromUri] int taskId)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetTask(taskId)
            };

            return result;
        }

        [HttpPost]
        [Route("save")]
        public ApiResult AddTask(TaskModel taskModel)
        {
            var result = new ApiResult();

            var userInfo = _repositoryService.GetCurrentUserInfo(User.Identity.Name);
            var allowedRoles = new string[] { Globals.SYSAD_RC, Globals.ACCOUNT_MANAGER_RC, Globals.TEAM_MANAGER_RC };

            if (Array.FindIndex(allowedRoles, a => a == userInfo.RoleCode) == -1)
            {
                result.message = Resources.User401;
                result.isSuccessful = false;
                return result;
            }

            var isNew = (taskModel.Id > 0);
            var transData = _repositoryService.SaveTask(taskModel, User.Identity.Name);

            if (!transData.IsSuccessful)
            {
                result.message = transData.Error.ToString();
                result.isSuccessful = false;

                return result;
            }

            var newTaskId = (int)transData.Data;
            result.message = isNew ? $"{Resources.TaskSuccessUpdate}|{newTaskId}" : string.Format("{0}|{1}", Resources.TaskSuccessAdd, newTaskId);

            return result;
        }

        [HttpPost]
        [Route("save_task_with_assignee")]
        public async Task<ApiResult> AddTaskWithAssignee(TaskModel taskModel)
        {
            var result = new ApiResult();

            var transData = _repositoryService.SaveTask(taskModel, User.Identity.Name);

            if (!transData.IsSuccessful)
            {
                result.message = transData.Error.ToString();
                result.isSuccessful = false;
                
                return result;
            }

            var newTaskId = (int)transData.Data;

            var data = await _repositoryService
                .UpdateTaskAssigneesAsync(taskModel.Users.UserIds, newTaskId, User.Identity.Name).ConfigureAwait(false);

            var addMsg = new StringBuilder();

            if (data.AddedAssigneeCount > 0)
            {
                addMsg.Append(string.Format(CultureInfo.InvariantCulture,
                    Resources.TaskAssigneeAddStatus, data.AddedAssigneeCount));
            }

            if (data.RemoveAssigneeCount > 0)
            {
                if (data.AddedAssigneeCount > 0)
                    addMsg.Append(Environment.NewLine);

                addMsg.Append(string.Format(CultureInfo.InvariantCulture,
                    Resources.TaskAssigneeRemoveStatus, data.RemoveAssigneeCount));
            }

            result.data = data;
            result.message
                = taskModel.Id > 0 ?
                    string.Format(CultureInfo.InvariantCulture, "{0}|{1}", Resources.TaskSuccessUpdate, newTaskId)
                    : string.Format(CultureInfo.InvariantCulture, "{0}|{1}", Resources.TaskSuccessAdd, newTaskId);

            return result;
        }

        [HttpPost]
        [Route("delete")]
        public ApiResult DeleteTask(TaskModel taskModel)
        {
            var result = new ApiResult();

            var userInfo = _repositoryService.GetCurrentUserInfo(User.Identity.Name);
            var allowedRoles = new string[] { Globals.SYSAD_RC, Globals.ACCOUNT_MANAGER_RC, Globals.TEAM_MANAGER_RC };

            if (Array.FindIndex(allowedRoles, a => a == userInfo.RoleCode) == -1)
            {
                result.message = Resources.User401;
                result.isSuccessful = false;
                return result;
            }

            _repositoryService.DeleteTask(taskModel);
            result.message = Resources.TaskSuccessDelete;

            return result;
        }

        [HttpPost]
        [Route("ActivateTask")]
        public ApiResult ActivateTask(TaskModel taskModel)
        {
            var result = new ApiResult();

            var userInfo = _repositoryService.GetCurrentUserInfo(User.Identity.Name);
            var allowedRoles = new string[] { Globals.SYSAD_RC, Globals.ACCOUNT_MANAGER_RC, Globals.TEAM_MANAGER_RC };

            if (Array.FindIndex(allowedRoles, a => a == userInfo.RoleCode) == -1)
            {
                result.message = Resources.User401;
                result.isSuccessful = false;
                return result;
            }


            _repositoryService.ActivateTask(taskModel);
            result.message = "Task Activated";

            return result;
        }

        [HttpPost]
        [Route("DeactivateTask")]
        public ApiResult DeactivateTask(TaskModel taskModel)
        {
            ApiResult result = new ApiResult();

            UserInfoModel userInfo = _repositoryService.GetCurrentUserInfo(User.Identity.Name);
            string[] allowedRoles = new string[] { Globals.SYSAD_RC, Globals.ACCOUNT_MANAGER_RC, Globals.TEAM_MANAGER_RC };

            if (Array.FindIndex(allowedRoles, a => a == userInfo.RoleCode) == -1)
            {
                result.message = Resources.User401;
                result.isSuccessful = false;
                return result;
            }

            _repositoryService.DeactivateTask(taskModel);
            result.message = "Task Deactivated.";

            return result;
        }

        [HttpPost]
        [Route("TaskNotification")]
        public ApiResult TaskNotification(TaskModel taskModel)
        {
            ApiResult result = new ApiResult();

            _repositoryService.TaskNotification(taskModel.Id, taskModel.NotificationEnabled);
            result.message = (taskModel.NotificationEnabled ? $"Enabled Notification for {taskModel.Name}" : $"Disabled Notification for {taskModel.Name}");

            return result;
        }

        [Route("taskdashboard")]
        [HttpGet]
        public ApiResult TaskDashboardData([FromUri] int[] groupids)
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.GetTaskDashboardData(User.Identity.Name, groupids);

            return result;
        }

        [Route("details")]
        [HttpGet]
        public ApiResult TaskDetails([FromUri] int taskid)
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.GetTaskDetails(User.Identity.Name, taskid);

            return result;
        }

        [Route("taskassignee")]
        [HttpGet]

        public ApiResult TaskDashboardData([FromUri] int taskid)
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.GetTaskAssignees(User.Identity.Name, taskid);

            return result;
        }

        [Route("Assign_TeamToTask")]
        [HttpPost]
        public ApiResult AssignTeamToTask(AssignTaskToTeamModel model)
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.AssignTeamsToTask(model.TeamIds, model.TaskId, User.Identity.Name);

            return result;
        }

        [Route("Assign_UsersToTask")]
        [HttpPost]
        public async Task<ApiResult> AssignUsersToTask(AssignTaskToUsersModel model)
        {
            ApiResult result = new ApiResult();

            result.data = await _repositoryService.UpdateTaskAssigneesAsync(model.UserIds, model.TaskId, User.Identity.Name).ConfigureAwait(false);
            return result;
        }

        [Route("Assign_TaskToGroup")]
        [HttpPost]
        public ApiResult AssignTaskToGroup(AssignTaskToGroupModel model)
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.AssignTaskToGroup(model.GroupIds, model.TaskId, User.Identity.Name);

            return result;
        }

        [Route("Update_UserTaskNotification")]
        [HttpPost]
        public ApiResult AssignTaskToGroup([FromUri] int taskid, [FromUri] string mode)
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.UpdateUserTaskNotifications(taskid, User.Identity.Name, mode);
            result.message = Resources.UserTaskNotificationUpdateSuccess;

            return result;
        }

        [HttpPost]
        [Route("update_single_teamtotask")]
        public ApiResult Update_Single_Teamtotask(TaskItemModel model, [FromUri] string mode)
        {
            ApiResult result = new ApiResult();

            _repositoryService.UpdateSingleTeamTask(model.TaskId, model.ItemId, mode, User.Identity.Name);
            result.message = Resources.TaskSuccessUpdate;

            return result;
        }

        [HttpPost]
        [Route("update_single_usertotask")]
        public async Task<ApiResult> Update_Single_Usertotask(TaskUserModel model, [FromUri] string mode)
        {
            ApiResult result = new ApiResult();

            await _repositoryService.UpdateSingleUserTask(model.TaskId, model.UserId, mode, User.Identity.Name).ConfigureAwait(false);

            if (mode == "add")
            {
                result.message = Resources.TaskSuccessUpdate;
            }
            else
            {
                result.message = Resources.TaskSuccessDelete;
            }

            return result;
        }

        [HttpPost]
        [Route("assign_users")]
        public async Task<ApiResult> AssignUsersAsync(TaskUsersModel taskUsersModel)
        {
            ApiResult result = new ApiResult();

            if (taskUsersModel is null) throw new ArgumentNullException(nameof(taskUsersModel));

            await _repositoryService.AssignUsersAsync(taskUsersModel, User.Identity.Name)
                .ConfigureAwait(true);
            result.message = "";

            return result;
        }

        [HttpPost]
        [Route("update_single_grouptotask")]
        public ApiResult Update_Single_Grouptotask(TaskItemModel model, [FromUri] string mode)
        {
            ApiResult result = new ApiResult();

            _repositoryService.UpdateSingleGroupToTask(model.TaskId, model.ItemId, mode, User.Identity.Name);
            result.message = Resources.TaskSuccessUpdate;
            return result;
        }
    }
}
