using IOM.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace IOM.Models
{
    public class UserDetails
    {
        [Key]
        public int Id { get; set; }

        #region User
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser UserApplicationUser { get; set; }
        #endregion

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }
        
        [Display(Name = "Employee Status")]
        public string EmployeeStatus { get; set; }
        public IEnumerable<SelectListItem> EmployeeStatuses { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Hourly Rate")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        [Required(AllowEmptyStrings = true)]
        [Display(Name = "Shift Schedule")]
        public string Shift { get; set; }

        [Display(Name = "Week Schedule")]
        public string WeekSchedule { get; set; }

        [Display(Name = "Generate Temporary Password")]
        public bool TemporaryPassword { get; set; }

        [Display(Name = "Image")]
        public string Image { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date Added")]
        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }
        public bool? IsLocked { get; set; }
        public bool? IsDeleted { get; set; }

        public UserDetails()
        {
            Role = Globals.SYSAD_RC;
            HourlyRate = Convert.ToDecimal(0.0);
            Created = DateTime.Now;
            IsLocked = false;
        }
    }
}