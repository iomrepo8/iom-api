using System;

namespace IOM.Models.ApiControllerModels
{
    public class NotificationModel : BaseModel
    {
        public string Message { get; set; }
        public DateTime NoteDate { get; set; }
        public bool IsRead { get; set; }
        public bool IsArChived { get; set; }
        public string NoteType { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
    }
}