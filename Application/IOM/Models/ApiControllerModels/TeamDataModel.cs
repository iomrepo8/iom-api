using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class TeamModel : BaseModel
    {
        public IList<TaskModel> Tasks { get; set; }
        public int? AccountId { get; set; }
        public string AccountName { get; set; }
        public int? ShiftId { get; set; }
        public string ShiftName { get; set; }
        public bool IsActive { get; set; }
    }

    public class TeamListModel : TeamModel
    {
        public int? AgentCount { get; set; }
        public decimal ActiveTime { get; set; }
    }

    public class TeamDetailModel : TeamModel
    {
        public IList<UserModel> Managers { get; set; } = new List<UserModel>();
        public IList<UserModel> LeadAgents { get; set; } = new List<UserModel>();
        public IList<UserModel> Agents { get; set; } = new List<UserModel>();
        public IList<UserModel> Clients { get; set; } = new List<UserModel>();
        public IList<DayOff> DayOffs { get; set; } = new List<DayOff>();
        public IList<Holiday> Holidays { get; set; } = new List<Holiday>();
        public IList<BaseModel> TaskGroup { get; set; } = new List<BaseModel>();
    }

    public class TeamMemberModel
    {
        public int TeamId { get; set; }
        public int[] UserIds { get; set; }
        public int UserDetailsId { get; set; }
    }

    public class AddAgentRequest
    {
        public bool IsEdit { get; set; } = false;
        public string UserId { get; set; }
        public int AccountId { get; set; }
        public IList<int> TeamIds { get; set; }
        public string[] UserIds { get; set; }
    }

    public class TeamRawModel : BaseModel
    {
        public int AccountId { get; set; }
        public string AgentUserId { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class TeamDashboard
    {
        public int TotalTeams { get; set; }
        public int TotalAgents { get; set; }
        public decimal? TotalHours { get; set; }
        public IList<TeamListModel> TeamsList { get; set; }
    }

    public class TeamDataRequestModel
    {
        public int[] AccountIds { get; set; }
        public int[] TeamIds { get; set; }
        public bool ShowInactive { get; set; }
    }
}