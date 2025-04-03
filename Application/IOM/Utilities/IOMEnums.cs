using IOM.Attributes;

namespace IOM.Utilities
{
    public enum ExceptionType
    {
        Thrown, Caught, Uncauht
    }

    public enum UriSegmentType
    {
        Entity, Action
    }

    public enum LogEntity
    {
        task, teams, accounts, user, role, time, notification, tkdata, app, attendance, taskgroup, 
        settings, tag, auth, seat, syslogs, usershift
    }

    public enum NoteType
    {
        ReminderAgentStatus, ReminderAttendance, TaskNotification, EODEdit, EODConfirm, EODSubmit, SeatStatus
    }

    public enum APIResultCode
    {
        AccessDenied = 401,
        BadRequest = 400
    }

    public enum EODAction
    {
        [StringValue("Approve EOD")]
        ApproveEOD,
        [StringValue("Deny EOD")]
        DenyEOD,
        [StringValue("Send EOD")]
        SendEOD,
        [StringValue("Edit EOD")]
        EditEOD
    }

    public enum EmployeeStatusFilter
    {
        All,
        Present,
        Active
    }

    public enum EmailNotificationType
    {
        EodReport = 1,
        EodEdiRequest = 2,
        EodEditConfirmation = 3,
        EodEditDeny = 4,
        NewUserCreated = 5,
        DeletedUser = 6,
        SupportInquiry = 7
    }

    public enum AuthFilter
    {
        Unauthorized,
        Unauthorized2Fa
    }

    public enum TwoFactorProvider
    {
        PhoneCode,
        EmailCode
    }
}