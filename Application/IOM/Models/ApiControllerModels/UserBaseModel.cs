using IOM.Utilities;
using System;
using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class UserBaseModel
    {
        public string FullName { get; set; }
        public int UserDetailsId { get; set; }
        public string NetUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public IList<BaseModel> Accounts { get; set; } = new List<BaseModel>();
        public IList<BaseModel> Teams { get; set; } = new List<BaseModel>();
        public IList<BaseModel> Tags { get; set; } = new List<BaseModel>();
        public string StaffId { get; set; }
        public bool? IsLocked { get; set; }
        public string TaskName { get; set; }
        public string TaskNumber { get; set; }
    }

    public class TimekeepingManagement : UserBaseModel
    {
        public decimal? TaskActiveTime { get; set; }
        public decimal? TotalActiveHours { get; set; }
        public decimal? LunchTime { get; set; }
        public decimal? FirstBreakTime { get; set; }
        public decimal? SecondBreakTime { get; set; }
        public decimal? BioTime { get; set; }
        public decimal? AvailTime { get; set; }
        public string Status { get; set; }
        public string TaskNumber { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double? WorkedDay { get; set; }
        public double? Absences { get; set; }
        public bool IsActive { get; set; }
        public int ShiftDetailsId { get; set; }
        public TimeSpan ShiftStart { get; set; }
        public TimeSpan ShiftEnd { get; set; }
        public float LunchBreak { get; set; }
        public int PaidBreaks { get; set; }
        public int DaysPerWeek { get; set; }
    }

    public class TimekeepingReport : UserBaseModel
    {
        public int? TaskId { get; set; }
        public string TaskDescription { get; set; }
        public bool IsActiveTask { get; set; }
        public decimal? TaskActiveTime { get; set; }
    }

    public class TimeKeepingDataRequestModel
    {
        public string[] Roles { get; set; }
        public int[] AccountIds { get; set; }
        public int[] TeamIds { get; set; }
        public int[] TagIds { get; set; }
        public int[] UserIds { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IncludeInactive { get; set; }
        public bool HasLiveHours { get; set; }
        public EmployeeStatusFilter StatusFilter { get; set; }
    }

    public class TkReportDataRequestModel
    {
        public int[] AccountIds { get; set; }
        public int[] TeamIds { get; set; }
        public int[] UserIds { get; set; }
        public string[] Roles { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int[] TagIds { get; set; }
        public bool HasLiveHours { get; set; }
    }

    public class EodListDataRequestModel
    {
        public int[] AccountIds { get; set; }
        public int[] TeamIds { get; set; }
        public int[] UserIds { get; set; }
        public int[] TagIds { get; set; }
        public string[] Roles { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool WithActionOnly { get; set; }
    }

    public class UserShiftModel : UserBaseModel
    {
        public int ShiftDetailsId { get; set; }
        public TimeSpan ShiftStart { get; set; }
        public TimeSpan ShiftEnd { get; set; }
        public float LunchBreak { get; set; }
        public int PaidBreaks { get; set; }
        public int DaysPerWeek { get; set; }
        public int TimeZoneId { get; set; }
        public double DailyHours { get; set; }
        public IList<UserWorkday> WorkDays { get; set; }
    }
}