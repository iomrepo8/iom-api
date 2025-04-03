using System;

namespace IOM.Models.ApiControllerModels
{
    public class AttendanceIndividual
    {
        public int Id { get; set; }
        public int UserDetailsId { get; set; }
        
        public DateTime? SDate { get; set; }
        public String Start { get; set; }

        public String End { get; set; }


    }
}