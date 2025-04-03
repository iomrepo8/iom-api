using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class BaseLookUpModel
    {
        public int Id { get; set; }
        public string Text { get; set; }        
    }

    public class LookUpModel : BaseLookUpModel
    {
        public string StringId { get; set; }
        public string Description { get; set; }
    }

    public class UserLookUpModel : BaseLookUpModel
    {
        public string RoleCode { get; set; }
        public IList<int> TeamIds { get; set; } = new List<int>();
        public IList<int> AccountIds { get; set; } = new List<int>();
        public string NetUserId { get; set; }
        public string StaffId { get; set; }
    }

    public class UserBasicInfoModel
    {
        public int UserDetailsId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class UserLookUpModelv2 : BaseLookUpModel
    {
        public string RoleCode { get; set; }
        public List<CommonItem1> Teams { get; set; }
        public List<CommonItem1> Accounts { get; set; }
        public string NetUserId { get; set; }
    }

    public class CommonItem1
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TeamLookUpModel : BaseLookUpModel
    {
        public int? AccountId { get; set; }
    }

    public class AccountLookUp : BaseLookUpModel
    {
        public IList<int> TeamIds { get; set; } = new List<int>();
    }

    public class TeamLookUp : BaseLookUpModel
    {
        public int AccountId { get; set; }
    }
}