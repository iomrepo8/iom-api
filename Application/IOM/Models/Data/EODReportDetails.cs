using IOM.Models.ApiControllerModels;
using System;

namespace IOM.Models.Data
{
    public class EODReportDetails
    {
        public int Id { get; set; }
        public DateTime EODDate { get; set; }
        public UserBasicModel SenderDetails { get; set; }
    }
}