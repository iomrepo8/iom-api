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
    
    public partial class EODReport
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Note { get; set; }
        public System.DateTime EODDate { get; set; }
        public Nullable<System.DateTime> SentUTCDateTime { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<double> ClientOffset { get; set; }
        public Nullable<bool> IsConfirmed { get; set; }
        public Nullable<bool> IsEdited { get; set; }
        public Nullable<System.DateTime> ConfirmedUTCDate { get; set; }
        public Nullable<int> ConfirmedBy { get; set; }
        public string Recipients { get; set; }
    }
}
