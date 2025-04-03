using IOM.Models.ApiControllerModels;
using System.Collections.Generic;

namespace IOM.Models.Data
{
    public class EODList
    {
        public int Id { get; set; }
        public string EODDate { get; set; }
        public string FullName { get; set; }
        public string RoleCode { get; set; }
        public int UserDetailsId { get; set; }
        public string NetUserId { get; set; }
        public int IsConfirmed { get; set; }
        public string  AccountIds { get; set; }
        public string TeamIds { get; set; }
        public string IsTouched { get; set; }
        public string DateTimeConfirmed { get; set; }
        public string ConfirmedByFullName { get; set; }
        public string EODAction { get; set; }
        public string SentUTCDateTime { get; set; }
        public IList<BaseModel> Teams { get; set; } = new List<BaseModel>();
        public IList<BaseModel> Accounts { get; set; } = new List<BaseModel>();
        public IList<BaseModel> Tags { get; set; } = new List<BaseModel>();

    }
}