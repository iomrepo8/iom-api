using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class UserNotificationSettingResult
    {
        public int Id { get; set; }
        public int NotificationTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAllowed { get; set; }
        public int UserDetailsId { get; set; }
    }

    public class EmailNotificationSetting
    {
        public int UserDetailsId { get; set; }
        public string NetUserId { get; set; }
        public string Email { get; set; }
        public IList<UserNotificationSettingResult> NotificationSettings { get; set; }
    }
}