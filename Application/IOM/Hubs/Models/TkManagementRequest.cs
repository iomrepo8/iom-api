using IOM.Utilities;

namespace IOM.Hubs.Models
{
    public class TkManagementRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string[] Roles { get; set; }
        public int[] AccountIds { get; set; }
        public int[] TeamIds { get; set; }
        public int[] UserIds { get; set; }
        public int[] TagIds { get; set; }
        public bool IncludeInactive { get; set; }
        public bool HasLiveHours { get; set; }
        public EmployeeStatusFilter StatusFilter { get; set; }
    }
}