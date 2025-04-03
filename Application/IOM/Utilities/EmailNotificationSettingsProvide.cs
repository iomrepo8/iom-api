using System.Collections.Generic;
using IOM.Helpers;

namespace IOM.Utilities
{
    internal static class EmailNotificationSettingsProvide
    {
        internal static  Dictionary<string, IList<EmailNotificationType>> RoleSetting =>
            new Dictionary<string, IList<EmailNotificationType>>
            {
                {
                    Globals.SYSAD_RC, new List<EmailNotificationType>
                    {
                        EmailNotificationType.EodReport, EmailNotificationType.EodEdiRequest,
                        EmailNotificationType.EodEditConfirmation, EmailNotificationType.EodEditDeny,
                        EmailNotificationType.NewUserCreated, EmailNotificationType.DeletedUser,
                        EmailNotificationType.SupportInquiry
                    }
                },
                {
                    Globals.ACCOUNT_MANAGER_RC, new List<EmailNotificationType>
                    {
                        EmailNotificationType.EodReport, EmailNotificationType.EodEdiRequest,
                        EmailNotificationType.EodEditConfirmation, EmailNotificationType.EodEditDeny
                    }
                },
                {
                    Globals.TEAM_MANAGER_RC, new List<EmailNotificationType>
                    {
                        EmailNotificationType.EodReport, EmailNotificationType.EodEdiRequest,
                        EmailNotificationType.EodEditConfirmation, EmailNotificationType.EodEditDeny
                    }
                },
                {
                    Globals.LEAD_AGENT_RC, new List<EmailNotificationType>
                    {
                        EmailNotificationType.EodReport, EmailNotificationType.EodEdiRequest,
                        EmailNotificationType.EodEditConfirmation, EmailNotificationType.EodEditDeny
                    }
                }
            };
    }
}