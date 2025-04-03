using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class TaskDashboardDataModel: TaskModel
    {
        public List<IOMTaskAssignee> Assignees { get; set; }
        public List<TaskGroupModel> Groups { get; set; }
        public List<TeamModel> Teams { get; set; }
        public bool UserEnabledNotification { get; set; }
    }

    public class IOMTaskAssignee
    {
        public int UserdetailsId { get; set; }
        public string NetUserId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string RoleCode { get; set; }
        public string ProfileImage { get; set; }
        public string Teams { get; set; }
        public string TeamsAsString { get; set; }
        public string Accounts { get; set; }
        public string AccountsAsString { get; set; }
    }

    public class TaskAssigneeUpdate
    {
        public int RemoveAssigneeCount { get; set; }
        public int AddedAssigneeCount { get; set; }
        public IList<string> FailRemovalErrors { get; set; } = new List<string>();
    }
}