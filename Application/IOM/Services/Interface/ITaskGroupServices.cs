using System.Collections.Generic;
using IOM.Models.ApiControllerModels;

namespace IOM.Services.Interface
{
    public interface ITaskGroupServices
    {
        object GetTaskGroupList(string userid);
        object GetTaskGroup(int taskGroupId);
        void DeleteTaskGroup(TaskGroupModel tgModel);
        void SaveTaskGroup(TaskGroupModel tgModel, string userid);
        void UpdateTaskGroupItems(List<int> taskids, int groupid, string username);
        void UpdateTaskGroupItems(int taskid, int groupid, string mode, string username);
        void UpdateUserTaskGroup(int groupId, string userId, string mode, string username);
        void UpdateUserTaskGroup(List<string> userids, int groupid, string username);
        IList<TaskGroupModel> GetUnassigned(int teamId);
        void UpdateTeamTaskGroup(int groupid, int teamid, string mode, string username);
        void UpdateTeamTaskGroup(List<int> teamids, int groupid, string username);
        TaskGroupDetails GetTaskGroupDetails(int groupid, string username);
    }
}