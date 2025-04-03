using System;
using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class DayOff
    {
        public int Id { get; set; }
        public string Day { get; set; }
        public int NumericDay { get; set; }
    }

    public class Holiday
    {
        public int Id { get; set; }
        public DateTime HolidayDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class DayOffHolidayModel
    {
        public int TeamId { get; set; }
        public IList<DayOff> DayOffs { get; } = new List<DayOff>();
        public IList<Holiday> Holidays { get; } = new List<Holiday>();
    }
}