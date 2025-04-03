using System;
using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class SystemLogDataModel
    {
        public string LogDate { get; set; }
        public string ActorName { get; set; }
        public string ActorRole { get; set; }
        public int ActorUserId { get; set; }
        public string StaffId { get; set; }
        public string ActionType { get; set; }
        public string Entity { get; set; }
        public string IPAddress { get; set; }
        public string ElapsedTime { get; set; }
        public string RequestBody { get; set; }
        public string UrlParams { get; set; }
        public int? ResponseStatusCode { get; set; }
        public string BrowserUsed { get; set; }
        public string Note { get; set; }
        public string EODEmailReference { get; set; }
        public IList<BaseModel> Accounts { get; set; } = new List<BaseModel>();
        public IList<BaseModel> Teams { get; set; } = new List<BaseModel>();
    }

    public class SysLogRequestDataModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int[] UserIds { get; set; }
        public int[] AccountIds { get; set; }
        public int[] TeamIds { get; set; }
        public string[] ActionTypes { get; set; }
        public string[] Entities { get; set; }
        public string[] Roles { get; set; }
    }
}