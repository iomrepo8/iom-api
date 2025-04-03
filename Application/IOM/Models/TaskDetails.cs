using System.Collections.Generic;

namespace IOM.Models
{
    public class TaskDetails
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string AccountName { get; set; }
        public string TeamName { get; set; }
        public string TaskNumber { get; set; }
    }

    public class AssignTaskToTeamModel
    {
        public int TaskId { get; set; }
        public List<int> TeamIds { get; set; }
    }

    public class AssignTaskToUsersModel
    {
        public int TaskId { get; set; }
        public List<string> UserIds { get; set; }
    }

    public class AssignTaskToGroupModel
    {
        public int TaskId { get; set; }
        public List<int> GroupIds { get; set; }
    }
}