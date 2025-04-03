namespace IOM.Models.ApiControllerModels
{
    public class UserShiftDataRequest
    {
        public int ShiftDetailsId { get; set; }
        public int UserDetailsId { get; set; }
        public string ShiftStart { get; set; }
        public string ShiftEnd { get; set; }
        public float LunchBreak { get; set; }
        public int PaidBreaks { get; set; }
        public int? TimeZoneId { get; set; }
    }
}