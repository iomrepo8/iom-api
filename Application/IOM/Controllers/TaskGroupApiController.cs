using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Services;
using System;
using System.Web.Http;
using IOM.Services.Interface;

namespace IOM.Controllers
{
    [RoutePrefix("taskgroup")]
    [Authorize]
    public class TaskGroupApiController : ApiController
    {
        private readonly ITaskGroupServices _taskGroupServices;

        public TaskGroupApiController(ITaskGroupServices taskGroupServices)
        {
            _taskGroupServices = taskGroupServices;
        }
        [HttpGet]
        [Route("list")]
        public ApiResult GetTaskGroupList()
        {
            ApiResult result = new ApiResult();

            result.data = _taskGroupServices.GetTaskGroupList(User.Identity.Name);

            return result;
        }

        [HttpGet]
        [Route("get")]
        public ApiResult GetTaskGroup([FromUri] int groupid)
        {
            ApiResult result = new ApiResult();

            result.data = _taskGroupServices.GetTaskGroupDetails(groupid, User.Identity.Name);

            return result;
        }

        [HttpPost]
        [Route("save")]
        public ApiResult SaveTaskGroup(TaskGroupModel tgModel)
        {
            ApiResult result = new ApiResult();

            _taskGroupServices.SaveTaskGroup(tgModel, User.Identity.Name);

            return result;
        }

        [HttpPost]
        [Route("delete")]
        public ApiResult DeleteTaskGroup(TaskGroupModel tgModel)
        {
            ApiResult result = new ApiResult();

            _taskGroupServices.DeleteTaskGroup(tgModel);
            result.message = Resources.TaskGroupDeleted;

            return result;
        }

        [HttpPost]
        [Route("update_taskgroup")]
        public ApiResult UpdateTaskGroup(AssignItemsToGroupModel model)
        {
            ApiResult result = new ApiResult();

            _taskGroupServices.UpdateTaskGroupItems(model.Ids, model.GroupId, User.Identity.Name);
            result.message = Resources.TaskGroupUpdated;

            return result;
        }

        [HttpPost]
        [Route("update_usertaskgroup")]
        public ApiResult UpdateUserTaskGroup(AssignItemsStringToGroupModel model)
        {
            ApiResult result = new ApiResult();

            _taskGroupServices.UpdateUserTaskGroup(model.Ids, model.GroupId, User.Identity.Name);
            result.message = Resources.TaskGroupUpdated;

            return result;
        }

        [HttpPost]
        [Route("update_teamtaskgroup")]
        public ApiResult UpdateTeamaskGroup(AssignItemsToGroupModel model)
        {
            ApiResult result = new ApiResult();

            _taskGroupServices.UpdateTeamTaskGroup(model.Ids, model.GroupId, User.Identity.Name);
            result.message = Resources.TaskGroupUpdated;

            return result;
        }

        [HttpPost]
        [Route("add_teamtaskgroup")]
        public ApiResult AddTeamTaskGroup(TaskGroupItemModel teamTaskGroupModel)
        {
            ApiResult result = new ApiResult();

            if (teamTaskGroupModel == null) throw new ArgumentNullException(nameof(teamTaskGroupModel));

            _taskGroupServices.UpdateTeamTaskGroup(teamTaskGroupModel.TaskGroupId,
                teamTaskGroupModel.Id, "add", User.Identity.Name);
            result.message = Resources.TeamSuccessAdd;

            return result;
        }

        [HttpPost]
        [Route("remove_teamtaskgroup")]
        public ApiResult RemoveTeamTaskGroup(TaskGroupItemModel teamTaskGroupModel)
        {
            ApiResult result = new ApiResult();


            if (teamTaskGroupModel == null) throw new ArgumentNullException(nameof(teamTaskGroupModel));

            _taskGroupServices.UpdateTeamTaskGroup(teamTaskGroupModel.TaskGroupId,
                teamTaskGroupModel.Id, "remove", User.Identity.Name);

            return result;
        }

        [HttpPost]
        [Route("add_usertaskgroup")]
        public ApiResult AddUserTaskGroup(TaskGroupUserModel teamTaskGroupModel)
        {
            ApiResult result = new ApiResult();

            if (teamTaskGroupModel == null) throw new ArgumentNullException(nameof(teamTaskGroupModel));

            _taskGroupServices.UpdateUserTaskGroup(teamTaskGroupModel.TaskGroupId,
                teamTaskGroupModel.UserId, "add", User.Identity.Name);
            result.message = Resources.UserSuccessAdd;

            return result;
        }

        [HttpPost]
        [Route("remove_usertaskgroup")]
        public ApiResult RemoveUserTaskGroup(TaskGroupUserModel teamTaskGroupModel)
        {
            ApiResult result = new ApiResult();

            if (teamTaskGroupModel == null) throw new ArgumentNullException(nameof(teamTaskGroupModel));

            _taskGroupServices.UpdateUserTaskGroup(teamTaskGroupModel.TaskGroupId,
                teamTaskGroupModel.UserId, "remove", User.Identity.Name);
            result.message = Resources.UserSuccessRemove;

            return result;
        }

        [HttpPost]
        [Route("add_iomtasktaskgroup")]
        public ApiResult AddIOMTaskToTaskGroup(TaskGroupItemModel teamTaskGroupModel)
        {
            ApiResult result = new ApiResult();

            if (teamTaskGroupModel == null) throw new ArgumentNullException(nameof(teamTaskGroupModel));

            _taskGroupServices.UpdateTaskGroupItems(teamTaskGroupModel.Id,
                teamTaskGroupModel.TaskGroupId, "add", User.Identity.Name);
            result.message = Resources.TaskSuccessAdd;

            return result;
        }

        [HttpPost]
        [Route("remove_iomtasktaskgroup")]
        public ApiResult RemoveIOMTaskTaskGroup(TaskGroupItemModel teamTaskGroupModel)
        {
            ApiResult result = new ApiResult();

            if (teamTaskGroupModel == null) throw new ArgumentNullException(nameof(teamTaskGroupModel));

            _taskGroupServices.UpdateTaskGroupItems(teamTaskGroupModel.Id,
                teamTaskGroupModel.TaskGroupId, "remove", User.Identity.Name);
            result.message = Resources.TaskSuccessDelete;

            return result;
        }

        [HttpGet]
        [Route("unassigned")]
        public ApiResult GetUnassigned([FromUri] int teamId)
        {
            ApiResult result = new ApiResult();

            result.data = _taskGroupServices.GetUnassigned(teamId);

            return result;
        }
    }
}
