using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOM.Models
{
    public class SeatDashboardViewModel
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountSeatCode { get; set; }
        public int SeatNumber { get; set; }
        public string SeatId { get; set; }
        public string SeatStatus { get; set; }
        public int UserId { get; set; }
        public string StaffName { get; set; }
        public string StaffPositioon { get; set; }
        public string StaffId { get; set; }

    }

    public enum SeatStatuses
    {
        Filled,
        Vacant,
        Temporary,
        Training,
    }
}