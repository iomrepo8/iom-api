//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IOM.DbContext
{
    using System;
    
    public partial class fn_TimeKeepingMgtData_Result
    {
        public string UserUniqueId { get; set; }
        public int UserDetailId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Fullname { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
        public Nullable<int> AccountId { get; set; }
        public string AccountName { get; set; }
        public Nullable<int> TeamId { get; set; }
        public string TeamName { get; set; }
        public string Status { get; set; }
        public string C_stat { get; set; }
        public Nullable<bool> IsLocked { get; set; }
        public Nullable<decimal> TaskActiveTime { get; set; }
        public Nullable<decimal> LunchActiveTime { get; set; }
        public Nullable<decimal> FirstBreakTime { get; set; }
        public Nullable<decimal> SecondBreakTime { get; set; }
        public Nullable<decimal> BioActiveTime { get; set; }
        public Nullable<decimal> AvailTime { get; set; }
        public Nullable<decimal> TotalActiveHours { get; set; }
        public int IsActive { get; set; }
        public Nullable<int> TaskId { get; set; }
        public string TaskName { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
    }
}
