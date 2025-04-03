using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IOM.DbContext;
using IOM.Models;
using IOM.Models.ApiControllerModels;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        IList<LookUpModel> GetTaskList(int[] teamIds);
        object TaskLookup(string username, string query, int[] filterAccIds, int[] filterTeamIds);
        object TaskLookup(int userId, string query);
        IList<BaseModel> GetUnassigned(int teamId, string username);
        IList<BaseModel> GetUnassignedUserTask(string userid);
        TaskComment SaveComment(TaskComment taskComment, string username);
        TaskDetails GetTaskData(int taskId);
        object GetTask(int taskId);
        void DeleteTask(TaskModel taskModel);
        TransactionResult SaveTask(TaskModel taskModel, string userid);
        void DeactivateTask(TaskModel taskModel);
        void ActivateTask(TaskModel taskModel);
        void TaskNotification(int taskId, bool isEnabled);
        TaskAssigneesModel GetTaskAssignees(string username, int taskid);
        TaskDashboardDataModel GetTaskDetails(string username, int taskid);
        Task AssignUsersAsync(TaskUsersModel taskUsersModel, string username);
        List<TaskDashboardDataModel> GetTaskDashboardData(string username, int[] groupids);
        Task<TaskAssigneeUpdate> UpdateTaskAssigneesAsync(List<string> usersid, int taskId, string username);
        bool AssignTaskToGroup(List<int> groupids, int taskId, string username);
        bool AssignTeamsToTask(List<int> teamsid, int taskId, string username);
        bool UpdateUserTaskNotifications(int taskId, string username, string mode);
        bool UpdateSingleTeamTask(int taskId, int teamId, string mode, string username);
        Task<bool> UpdateSingleUserTask(int taskId, string netUserId, string mode, string username);
        bool UpdateSingleGroupToTask(int taskId, int groupId, string mode, string username);
        void SetStatus_(int? taskId, int taskTypeId, int userDetailsId);
        TransactionResult SetStatus(int? taskId, int taskTypeId, int userDetailsId);
        Task BroadcastTaskActivation(int taskId, string userFullName, int userId, string username);
        string GenerateTaskNotificationEmailBody(string name, string taskName, string tname, string actname, DateTime est);

        /// <summary>
        /// from "1, 2, ,3, 4" to "team1, team2, team3, team4"
        /// </summary>
        /// <param name="teamList"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        string FromIdToString(List<Team> teamList, string ids);

        string FromIdToString(List<Account> accounts, string ids);
    }
}