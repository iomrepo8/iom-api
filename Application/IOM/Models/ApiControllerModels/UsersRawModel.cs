using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class UsersRawModel : BaseModel
    {
        public string FirstName { get; set; }
        public bool? IsLocked { get; set; }
        public string LastName { get; set; }
        public IList<int> TeamIds { get; set; }
        public IList<int> AccountIds { get; set; }
        public string NetUserId { get; set; }
        public string RoleCode { get; set; }
        public string StaffId { get; set; }
    }

    public class UsersDataRequestModal
    { 
        public int[] UserIds { get; set; }
        public int[] AccountIds { get; set; }
        public int[] TeamIds { get; set; }
        public string[] Roles { get; set; }
        public string[] Tags { get; set; }
        public bool ShowInactive { get; set; }
    }
}