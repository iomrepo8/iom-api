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
    
    public partial class AspNetUserPermission
    {
        public int id { get; set; }
        public string UserId { get; set; }
        public string ModuleCode { get; set; }
        public Nullable<bool> CanView { get; set; }
        public Nullable<bool> CanEdit { get; set; }
        public Nullable<bool> CanDelete { get; set; }
        public Nullable<bool> CanAdd { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}
