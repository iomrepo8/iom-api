using IOM.Models.ApiControllerModels;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace IOM.Models
{
    public class EmailRecipients
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public static class EmailSubject
    {
        private const string IOMFullName = "iLucent Operations Manager";

        public static string Registration()
        {
            return $"Welcome New User to {IOMFullName}";
        }

        public static string RegistrationForOthers(string role = "User")
        {
            return $"{IOMFullName} - New {role} Created";
        }

        public const string DeleteTeam = "iLucent Team Announcement: Team Deleted";
    }

    public static class EmailBody
    {
        private static string ReadTemplate(string file)
        {
            using (var reader = new StreamReader(HostingEnvironment.MapPath("~\\Templates\\" + file)))
            {
                return reader.ReadToEnd();
            }
        }

        public static string ForgotPassword(string callbackUrl)
        {
            string hours = ConfigurationManager.AppSettings["mailTokenLifespanInHours"];
            string body = ReadTemplate("ForgotPassword.html");

            body = body.Replace("{callbackUrl}", callbackUrl);
            body = body.Replace("{hours}", hours);

            return body;
        }

        #region User Creation

        public static string Registration(this string body, ApplicationUser user, UserDetails userDetail, string password)
        {
            var urlHost = ConfigurationManager.AppSettings["SiteUri"];

            body = body.Replace("{userDetail.FirstName}", userDetail.FirstName);
            body = body.Replace("{userDetail.LastName}", userDetail.LastName);
            body = body.Replace("{userDetail.Name}", userDetail.Name);
            body = body.Replace("{urlLink}", urlHost);
            body = body.Replace("{urlHost}", urlHost);
            body = body.Replace("{user.UserName}", user.UserName);
            body = body.Replace("{user.Email}", user.Email);
            body = body.Replace("{password}", password);

            if (userDetail.TemporaryPassword)
            {
                body = body.Remove(body.IndexOf("{tempStart}", StringComparison.Ordinal), "{tempStart}".Length);
                body = body.Remove(body.IndexOf("{tempEnd}", StringComparison.Ordinal), "{tempEnd}".Length);
            }
            else
            {
                var tempStart = body.IndexOf("{tempStart}", StringComparison.Ordinal);
                var tempEnd = body.IndexOf("{tempEnd}", StringComparison.Ordinal);

                if (tempStart >= 0 && tempEnd > tempStart)
                {
                    body = body.Remove(tempStart, tempEnd - tempStart + "{tempEnd}".Length);
                }
            }

            return body;
        }

        public static string RegistrationForAdmin(this string body, UserModel model)
        {
            body = body.Replace("{userDetail.Name}", model.FullName);
            body = body.Replace("{email}", model.Email);
            body = body.Replace("{roleName}", model.RoleName);

            return body;
        }

        #endregion

        #region User Delete

        public static string UserDelete(UserDetailModel user)
        {
            string body = ReadTemplate("UserDelete.html");

            body = body.Replace("{user.FirstName}", user.FirstName);
            body = body.Replace("{user.LastName}", user.LastName);
            body = body.Replace("{user.Name}", user.FullName);

            return body;
        }

        public static string UserDeleteForAdmin(string recipient, string deletedName, string deletedEmail, string deleteRole)
        {
            string body = ReadTemplate("UserDeleteForAdmin.html");

            body = body.Replace("{recipient}", recipient);
            body = body.Replace("{deletedName}", deletedName);
            body = body.Replace("{deletedEmail}", deletedEmail);
            body = body.Replace("{deleteRole}", deleteRole);

            return body;
        }

        #endregion

        #region Password Reset

        public static string PasswordResetSuccess(UserModel user)
        {
            string body = ReadTemplate("PasswordResetSuccess.html");

            body = body.Replace("{user.Name}", user.FullName);

            return body;
        }

        public static string ApprovedDeniedEOD(
            bool isApproved,
            string approverName,
            string agentName,
            string eoddate,
            string account,
            string team,
            string eodUrl)
        {
            string body = ReadTemplate("EODApprovedDenied.html");

            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

            string dt = easternTime.ToString("MM/dd/yyyy hh:mm tt") + "(EST)";

            string note = string.Empty;
            if (isApproved)
            {
                note = "The updated edit now reflects on the system database.";
            }

            string emailContent = body
                .Replace("{{ApprovedDeniedDateTime}}", dt)
                .Replace("{{Approver}}", ToTitleCase(approverName))
                .Replace("{{ApprovedOrDenied}}", (isApproved) ? "Approved" : "Denied")
                .Replace("{{AgentName}}", ToTitleCase(agentName))
                .Replace("{{EODDate}}", eoddate)
                .Replace("{{Account}}", account)
                .Replace("{{Team}}", team)
                .Replace("{{URL}}", eodUrl)
                .Replace("{{Note}}", note);

            return emailContent;
        }

        public static string RequestApproval(
            string agentName,
            string eoddate,
            string account,
            string team,
            string eodUrl)
        {
            string body = ReadTemplate("EODRequestApproval.html");

            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

            string dt = easternTime.ToString("MM/dd/yyyy hh:mm tt") + "(EST)";

            string emailContent = body
                .Replace("{{AgentName}}", ToTitleCase(agentName))
                .Replace("{{EODDate}}", eoddate)
                .Replace("{{Account}}", account)
                .Replace("{{Team}}", team)
                .Replace("{{URL}}", eodUrl);

            return emailContent;
        }

        public static string SeatStatusUpdate(
            string msg,
            string siteUrl)
        {
            var body = ReadTemplate("UpdateSeatStatus.html");

            var emailContent = body
                .Replace("{{Message}}", msg)
                .Replace("{{URL}}", siteUrl);

            return emailContent;
        }

        public static string EODReport(EODRecipients eodModel, string fromDate, string toDate)
        {
            string body = ReadTemplate("EODReportTPL.html");

            decimal totalHours = 0;
            string eodRows = GenerateEODRows(eodModel, out totalHours);
            string adjustedRows = GenerateAdjustedEODRows(eodModel);
            string emailContent = string.Empty;

            emailContent = body
                .Replace("{{EmployeeName}}", ToTitleCase(eodModel.Fullname))
                .Replace("{{UserRole}}", ToTitleCase(eodModel.UserRole))
                .Replace("{{EODTotalHours}}", totalHours.ToString("0.##"))
                .Replace("{{EODRows}}", eodRows)
                .Replace("{{AdjustedEODRows}}", adjustedRows)
                .Replace("{{EODDate}}", DateTime.Parse(fromDate).ToString("MM/dd/yyyy"))
                .Replace("{{Note}}", (eodModel.Note != null) ? eodModel.Note : string.Empty)
                .Replace("{{ChronoUrl}}", eodModel.ChronoDetailUrl)
                .Replace("{{Accounts}}", String.Join(", ", eodModel.Accounts.Select(a => a.Name).ToList()))
                .Replace("{{Teams}}", String.Join(", ", eodModel.Teams.Select(t => t.Name).ToList()));

            return emailContent;
        }

        private static string GenerateEODRows(EODReportModel eodReport, out decimal totalHours)
        {
            string eodRows = string.Empty;

            string trStyle1 = "style='font-weight: 500;padding: 0.75rem; border: 1px solid rgba(0, 0, 0, 0.1) ; vertical-align: middle;'";
            string trStyle2 = "style='background-color: #f4f6f9; font-weight: 500;padding: 0.75rem; border: 1px solid rgba(0, 0, 0, 0.1); vertical-align: middle;'";

            string rowsTPL =
                  "<tr>"
                      + "<td {{style}}>{{RowCount}}</td>"
                      + "<td {{style}}>{{TaskNo}}</td>"
                      + "<td {{style}}>{{TaskName}}</td>"
                      + "<td {{style}}>{{TaskDescription}}</td>"
                      + "<td {{style}}>{{TotalActiveTime}}</td>"
                  + "</tr>";
            decimal time = 0;
            totalHours = 0;
            int rowCount = 0;

            foreach (EODTaskModel item in eodReport.EODTaskList)
            {
                if (item.IsRemoved) continue;

                time = (item.IsAdjusted ? item.AdjustedTotalActiveTime : item.TotalActiveTime);

                if (time <= 0) continue;

                totalHours += time;
                rowCount++;

                eodRows += rowsTPL
                    .Replace("{{RowCount}}", rowCount.ToString())
                    .Replace("{{TaskNo}}", item.TaskNo)
                    .Replace("{{TaskName}}", item.TaskName)
                    .Replace("{{TaskDescription}}", item.TaskDescription)
                    .Replace("{{TotalActiveTime}}", time.ToString("0.##"))
                    .Replace("{{style}}", (rowCount % 2 == 0) ? trStyle1 : trStyle2);
            }

            return eodRows;
        }

        private static string GenerateAdjustedEODRows(EODReportModel eodReport)
        {
            string tplContainer = "<li style=\"color: #67757c; font-weight: 400;\">Adjustment was made to hours for {0}</li>";
            string adjList = string.Empty;
            string tblStyle = "border=\"0\" width=\"100%\" cellspacing=\"2\" style=\"width: 100 %; color: #67757c; border-collapse: collapse; font-size: 14px; font-family: 'Montserrat', sans-serif\"";
            string tdStyle = "style='font-weight: 500;padding: 0.75rem; border: 1px solid rgba(0, 0, 0, 0.1) ; vertical-align: middle;'";

            foreach (EODTaskModel item in eodReport.EODTaskList)
            {
                if (item.IsRemoved) continue;

                var time = (item.IsAdjusted ? item.AdjustedTotalActiveTime : item.TotalActiveTime);

                if (time <= 0) continue;

                if (item.IsAdjusted)
                {
                    adjList += string.Format(tplContainer, item.TaskName);
                }
            }

            if (adjList != string.Empty)
            {
                return $"<table {tblStyle}><tr><td {tdStyle}><ul style='padding: 0;'>{adjList}</ul></td></tr></table><tr><td>&nbsp;</td></tr>";
            }
            else
            {
                return string.Empty;
            }
        }

        public static string ToTitleCase(string str)
        {
            if (str != null)
            {
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
            }
            else
            {
                return str;
            }
        }

        public static string RemoveMember(
            string msg)
        {
            string body = ReadTemplate("RemoveMember.html");

            string emailContent = body
                .Replace("{{Message}}", msg);

            return emailContent;
        }
        public static string TaskUpdate(
            string msg)
        {
            string body = ReadTemplate("UpdateTask.html");

            string emailContent = body
                .Replace("{{Message}}", msg);

            return emailContent;
        }

        #endregion
    }
}