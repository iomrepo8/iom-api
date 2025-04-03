using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class TaskGroupModel: BaseModel
    {
        public bool IsActive { get; set; }
        public bool NotificationEnabled { get; set; }
        public int GroupId { get; set; }
        public int TaskItemCount { get; set; }
    }

    public class TaskGroupItemModel
    {
        public int Id { get; set; }
        public int TaskGroupId { get; set; }
    }

    public class TaskGroupUserModel
    {
        public string UserId { get; set; }
        public int TaskGroupId { get; set; }
    }

    public class AssignItemsToGroupModel
    {
        public int GroupId { get; set; }
        public List<int> Ids { get; set; }
    }

    public class AssignItemsStringToGroupModel
    {
        public int GroupId { get; set; }
        public List<string> Ids { get; set; }
    }

    public class TaskGroupDetails
    {
        public TaskGroupModel TaskGroupInfo { get; set; }
        public List<TeamBasicInfo> Teams { get; set; }
        public List<UserBasicInfo> Users { get; set; }
        public List<TaskModel> Task { get; set; }
    }
}