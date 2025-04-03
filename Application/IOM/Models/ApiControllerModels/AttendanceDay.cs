using System;

namespace IOM.Models.ApiControllerModels
{
    public class AttendanceDay
    {
        public int Id { get; set; }
        public int UserDetailsId { get; set; }
        public double WorkedDay { get; set; }
        public DateTime? SDate { get; set; }
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set;}

        public double MgtEdit { get; set; }

    }
}