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
    
    public partial class NotificationSetting
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NotificationSetting()
        {
            this.NotificationRecipientRoles = new HashSet<NotificationRecipientRole>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificationRecipientRole> NotificationRecipientRoles { get; set; }
    }
}
