using System;

namespace IOM.Models.ApiControllerModels
{
    public class TaskModel : BaseModel
    {
        public string TaskNumber { get; set; }
        public string ClickUpLink { get; set; }
        public string Manual { get; set; }
        public string Trigger { get; set; }
        public bool IsActive { get; set; }
        public bool NotificationEnabled { get; set; }
        public AssignTaskToTeamModel Teams { get; set; }
        public AssignTaskToUsersModel Users { get; set; }
        public AssignTaskToGroupModel Groups { get; set; }

    }

    public class TaskChronoItem
    {
        public int Id { get; set; }
        public decimal? Duration { get; set; }
        public DateTime HistoryDate { get; set; }
        public string TaskTypeName { get; set; }
        public int UserDetailsId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string TaskComment { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}