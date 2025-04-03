using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class AttendanceDataModel : UserListModel
    {
        public IList<AttendanceDay> Attendance { get; set; } = new List<AttendanceDay>();
        public IList<DayOff> DayOffs { get; set; } = new List<DayOff>();
        public double WorkedDays { get; set; }
        public double Absence { get; set; }
    }

    public class AttendanceDefaultView : UserBasicModel
    {
        public string RoleCode { get; set; }
        public string AccountNames { get; set; }
        public string AccountIds { get; set; }
        public decimal? TotalActiveTime { get; set; }
        public decimal? TotalRegTime { get; set; }
        public decimal? TotalOTTime { get; set; }
        public IList<BaseModel> Accounts { get; set; } = new List<BaseModel>();
        public IList<BaseModel> Teams { get; set; } = new List<BaseModel>();
        public IList<BaseModel> Tags { get; set; } = new List<BaseModel>();
        public IList<AttendanceDefaultData> Attendance { get; set; } = new List<AttendanceDefaultData>();
    }

    public class AttendanceDefaultData
    {
        public int UserDetailsId { get; set; }
        public decimal? TotalActiveTime { get; set; }
        public string HistoryDate { get; set; }
        public decimal? RegularHours { get; set; }
        public decimal? OTHours { get; set; }
        public decimal? UpdatedRegHours { get; set; }
        public decimal? UpdatedOTHours { get; set; }
    }

    public class OvertimeData
    {
        public int UserDetailsId { get; set; }
        public string HistoryDate { get; set; }
        public decimal RegularHours { get; set; }
        public decimal OvertimeHours { get; set; }
    }

    public class AttendanceDataRequestModel
    {
        public int[] UserIds { get; set; }
        public int[] AccountIds { get; set; }
        public int[] TeamIds { get; set; }
        public string[] Roles { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string[] Tags { get; set; }
        public bool HasLiveHours { get; set; }
    }

    public class AttOTDataRequestModel
    {
        public int[] UserIds { get; set; }
        public int[] AccountIds { get; set; }
        public int[] TeamIds { get; set; }
        public string[] Roles { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

    }

    public class AttStatusRequestDataModel
    {
        public int[] UserIds { get; set; }
        public string[] Roles { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class AttendanceRowView
    {
        public string Tag { get; set; }
        public decimal Hours { get; set; }
    }
}
