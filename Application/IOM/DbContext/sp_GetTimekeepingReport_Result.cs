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
    
    public partial class sp_GetTimekeepingReport_Result
    {
        public int UserDetailsId { get; set; }
        public Nullable<decimal> TotalActiveTime { get; set; }
        public int TaskId { get; set; }
        public int TaskHistoryTypeId { get; set; }
        public Nullable<bool> IsTaskActive { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
    }
}
