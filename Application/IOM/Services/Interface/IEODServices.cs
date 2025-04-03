using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IOM.DbContext;
using IOM.Models.ApiControllerModels;
using IOM.Models.Data;
using IOM.Utilities;
using Microsoft.AspNet.Identity;
using SendGrid.Helpers.Mail;
using NotificationType = IOM.Models.NotificationType;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        EODReportStatus GetEODStatus(int eodId);
        object GetSentEOD(int eodId);
        EODReportDetails GetEODReportDetails(int eodId);
        EODReport GetReportDetail(int eodId);
        object GetSentEODList(string fromDate, string toDate, bool withActionOnly, int[] userIds, int[] accountIds, int[] teamIds, int[] tagIds, string[] roles, string username);
        EODReportModel GenerateEOD(string fromDate, string toDate, string username);
        Task BroadcastEditedEOD(int eodid, string userFullName, int userId, string returnURL, DateTime eodDate, List<EmailAddress> manualRecipients);
        Task<int> SendEOD(EODRecipients eodModel, string fromDate, string toDate, double clientOffset, string userId, string chronoRoute);
        Task ApproveEOD(int eodId, string senderUserId, int currentUserId, string userFullName, string eodUrl);
        Task DenyEOD(int eodid, int currentUserId, string userFullName, string eodURL);
        Task NotifyTeamAdmins(int teamId, string title, string message, int userid, NoteType nType, List<int> additionalRecipients = null);
        IList<Recipient> GetUserEODRecipients(int userDetailsId, NotificationType notificationType);
        List<EmailAddress> GetApprovedDeniedEODRecipients(int teamId, List<EmailAddress> manualRecipients);
        bool SaveEOD(EODRecipients eodReport, string fromDate, string toDate, double clientOffset, string userId, out int newEODId);
        Task SendEODEmailAsync(IdentityMessage message, List<EmailAddress> recipients, EODAction eodAction, int userDetailsId);
    }
}