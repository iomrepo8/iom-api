using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels.Settings
{
    public class NotificationSettingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public string Subject { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public IList<RecipientRole> RecipientRoles { get; set; }
    }
}