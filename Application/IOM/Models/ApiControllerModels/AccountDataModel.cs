using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{

    public class AccountModel : BaseModel
    {
        public bool IsActive { get; set; }
        public int? Seat { get; set; }
        public string SeatCode { get; set; }
    }

    public class AccountsListModel : AccountModel
    {
        public int? AgentCount { get; set; }
        public int? TeamCount { get; set; }
        public decimal? HoursActive { get; set; }
    }

    public class AccountDataModel : AccountModel
    {
        public string ContactPerson { get; set; }
        public string EmailAddress { get; set; }
        public string OfficeAddress { get; set; }
        public string Website { get; set; }
        public string SeatCode { get; set; }
        public int? SeatSlot { get; set; }
        public IList<UserModel> AccountManagers { get; set; } = new List<UserModel>();
        public IList<UserModel> TeamManagers { get; set; } = new List<UserModel>();
        public IList<UserModel> ClientPOCs { get; set; } = new List<UserModel>();
        public IList<TeamModel> Teams { get; set; } = new List<TeamModel>();
        public IList<TeamUsersModel> Supervisors { get; set; } = new List<TeamUsersModel>();
        public IList<TeamUsersModel> Agents { get; set; } = new List<TeamUsersModel>();
    }

    public class ProfileAccountModel : AccountModel
    {
        public IList<TeamModel> Teams { get; set; } = new List<TeamModel>();
    }

    public class AccountUserModel
    {
        public int AccountId { get; set; }
        public string UserId { get; set; }
    }

    public class Personel
    {
        public int AccountId { get; set; }
        public int UserDetailsId { get; set; }
        public int[] UserIds { get; set; }
    }

    public class AccountsRawModel : AccountModel
    {
        public bool? IsAccountDeleted { get; set; }
    }

    public class AccountDashboard
    {
        public int TotalAccounts { get; set; }
        public int TotalTeams { get; set; }
        public int TotalAgents { get; set; }
        public int TotalPaidSeats { get; set; }
        public decimal? TotalHours { get; set; }
        public IList<AccountsListModel> AccountsList { get; set; }
    }

    public class AccountMemberModel
    {
        public int AccountId { get; set; }
        public int UserDetailsId { get; set; }
        public int[] UserIds { get; set; }
    }
}