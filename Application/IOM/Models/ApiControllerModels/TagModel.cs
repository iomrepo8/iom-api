using System;

namespace IOM.Models.ApiControllerModels
{
    public class TagModel
    {
        public int UserDetailsId { get; set; }
        public int TagId { get; set; }
        public string Name { get; set; }

        public DateTime? AttendanceDate { get; set; }

        public double Hours { get; set; }

    }
}