using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class TaskAssigneesModel
    {
        public int TaskId { get; set; }
        public string TaskNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public List<UserBasicInfo> Users { get; set; }
        public List<TeamBasicInfo> Teams { get; set; }
        public List<GroupTaskAssignees> Groups { get; set; }
    }

    public class UserBasicInfo
    {
        public int UserDetailsId { get; set; }
        public string NetUserId { get; set; }
        public string Name { get; set; }
        public bool IsLocked { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
    }

    public class TeamBasicInfo
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class GroupTaskAssignees
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class TaskUserModel
    {
        public string UserId { get; set; }
        public int TaskId { get; set; }
    }

    public class TaskUsersModel
    {
        public IList<string> UserIds { get; set; }
        public int TaskId { get; set; }
    }

    public class TaskItemModel
    {
        public int ItemId { get; set; }
        public int TaskId { get; set; }
    }
}