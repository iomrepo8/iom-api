namespace IOM.Models.ApiControllerModels.Settings
{
    public class NotificationRecipientRoleModel
    {
        public int Id { get; set; }
        public int NotificationSettingId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class RecipientRole
    {
        public string Id { get; set; }
    }
}