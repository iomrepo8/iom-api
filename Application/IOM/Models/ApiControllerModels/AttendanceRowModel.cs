using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOM.Models.ApiControllerModels
{
    public class AttendanceRowModel
    {
        
        public double Hours { get; set; }

        public int UserDetailsId { get; set; }
        public String AttendanceTag { get; set; }

        public DateTime AttendanceDate { get; set; }
    }
}