namespace IOM.Models
{
    public enum UserRole
    {
        SA, AM, CL, LA, AG
    }

    public enum EODStatus
    {
        Approved, Denied, Pending
    }

    public enum NotificationType
    {
        EodReport = 1,
        EodEditRequest = 2,
        EodEditConfirmation = 3,
        EodEditDeny = 4,
        NewUserCreated = 5,
        DeletedUser = 6,
        SupportInquiry = 7
    }

    public enum NotificationAction
    {
        ChangePassword = 1,
        ResetPassword = 2,
        ResetPasswordRequest = 3,
        DeleteInactiveUser = 4,
        DeleteInactiveUserAdmin = 5,
        AddUser = 6,
        AddUserAdmin = 7
    }
}