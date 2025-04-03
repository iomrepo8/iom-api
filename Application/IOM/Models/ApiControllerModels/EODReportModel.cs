using System;
using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class EODBaseModel
    {
        public string UserUniqueId { get; set; }
        public int UserDetailsId { get; set; }
        public string Fullname { get; set; }
        public string UserRole { get; set; }
        public string Note { get; set; }
    }

    public class EODReportModel : EODBaseModel
    {
        public List<EODTaskModel> EODTaskList { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IList<BaseModel> Accounts { get; set; } = new List<BaseModel>();
        public IList<BaseModel> Teams { get; set; } = new List<BaseModel>();
        public string ChronoDetailUrl { get; set; }
    }

    public class EODTaskModel
    {
        public string TeamName { get; set; }
        public int TeamId { get; set; }
        public string TeamDescription { get; set; }
        public string ShiftSchedule { get; set; }
        public string AccountName { get; set; }
        public int AccountId { get; set; }
        public string ContactPerson { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public int TaskId { get; set; }
        public string TaskNo { get; set; }
        public bool IsTaskActive { get; set; }
        public decimal TotalActiveTime { get; set; }
        public int TaskHistoryTypeId { get; set; }
        public decimal AdjustedTotalActiveTime { get; set; }
        public bool IsAdjusted { get; set; }
        public bool IsRemoved { get; set; }
        public bool IsInserted { get; set; }

    }

    public class SentEODList : EODBaseModel
    {
        public int Id { get; set; }
        public int? UserDetailsId { get; set; }
        public bool? IsConfirmed { get; set; }
        public DateTime? ConfirmedUTCDate { get; set; }
        public string DateConfirmed { get; set; }
        public string TimeConfirmed { get; set; }
        public string EODDate { get; set; }
        public string ConfirmedByFullName { get; set; }
        public IList<BaseModel> Accounts { get; set; } = new List<BaseModel>();
        public IList<BaseModel> Teams { get; set; } = new List<BaseModel>();
    }

    public class EODReportStatus
    {
        public EODStatus EODEnumStatus { get; set; }
        public DateTime? ConfirmedUTCDate { get; set; }
        public int? ConfirmedBy { get; set; }
        public string ConfirmedByFullname { get; set; }

    }
}