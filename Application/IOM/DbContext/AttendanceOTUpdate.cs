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
    using System.Collections.Generic;
    
    public partial class AttendanceOTUpdate
    {
        public int Id { get; set; }
        public System.DateTime HistoryDate { get; set; }
        public int UserDetailsId { get; set; }
        public Nullable<decimal> RegularHours { get; set; }
        public Nullable<decimal> OTHours { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<int> LastUpdatedBy { get; set; }
    }
}
