using IOM.DbContext;
using IOM.Helpers;
using IOM.Models;
using IOM.Models.ApiControllerModels;
using IOM.Models.Data;
using IOM.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using IOM.Services.Interface;
using NotificationType = IOM.Models.NotificationType;
using System.Security.Cryptography;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public EODReportStatus GetEODStatus(int eodId)
        {
            using (var ctx = Entities.Create())
            {
                EODReport eod = ctx.EODReports.FirstOrDefault(a => a.Id == eodId);

                if (eod.ConfirmedBy.HasValue && eod.IsConfirmed.HasValue)
                {
                    UserDetail user = ctx.UserDetails.SingleOrDefault(u => u.Id == eod.ConfirmedBy.Value);

                    if (eod.IsConfirmed.HasValue && eod.IsConfirmed == true)
                    {
                        return new EODReportStatus
                        {
                            EODEnumStatus = EODStatus.Approved,
                            ConfirmedBy = eod.ConfirmedBy,
                            ConfirmedUTCDate = eod.ConfirmedUTCDate,
                            ConfirmedByFullname = $"{user.FirstName} {user.LastName}"
                        };
                    }
                    else if (eod.IsConfirmed.HasValue && eod.IsConfirmed == false)
                    {
                        return new EODReportStatus
                        {
                            EODEnumStatus = EODStatus.Denied,
                            ConfirmedBy = eod.ConfirmedBy,
                            ConfirmedUTCDate = eod.ConfirmedUTCDate,
                            ConfirmedByFullname = $"{user.FirstName} {user.LastName}"
                        };
                    }
                }

                return new EODReportStatus
                {
                    EODEnumStatus = EODStatus.Pending
                };
            }
        }

        public object GetSentEOD(int eodId)
        {
            using (var ctx = Entities.Create())
            {
                return ctx.sp_GetSentEODReport(eodId);
            }
        }

        public EODReportDetails GetEODReportDetails(int eodId)
        {
            using (var ctx = Entities.Create())
            {
                var data = (from r in ctx.EODReports
                            join u in ctx.vw_ActiveUsers on r.UserId equals u.NetUserId
                            where r.Id == eodId
                            select new EODReportDetails
                            {
                                Id = r.Id,
                                EODDate = r.EODDate,
                                SenderDetails = new UserBasicModel
                                {
                                    UserDetailsId = u.UserDetailsId,
                                    FullName = u.FullName
                                }
                            }).SingleOrDefault();

                return data;
            }
        }

        public EODReport GetReportDetail(int eodId)
        {
            using (var ctx = Entities.Create())
            {
                return ctx.EODReports.SingleOrDefault(e => e.Id == eodId);
            }
        }

        public object GetSentEODList(string fromDate, string toDate, bool withActionOnly, int[] userIds, 
            int[] accountIds, int[] teamIds, int[] tagIds, string[] roles, string username)
        {
            var userInfo = GetCurrentUserInfo(username);

            using (var ctx = Entities.Create())
            {
                var userIdsParams = string.Join(",", userIds);
                var accountIdsParams = string.Join(",", accountIds);
                var teamIdsParams = string.Join(",", teamIds);
                var tagIdsParams = string.Join(",", tagIds);
                var rolesParams = string.Join(",", roles);

                var jsonResult = string.Join("", ctx.sp_GetEODList(userInfo.UserDetailsId, fromDate, toDate,
                        userIdsParams, accountIdsParams, teamIdsParams, tagIdsParams, rolesParams, withActionOnly)
                    .ToList());
                var dataQuery = (new JavaScriptSerializer()).Deserialize<IList<EODList>>(jsonResult);

                return dataQuery ?? new List<EODList>();
            }
        }

        public EODReportModel GenerateEOD(string fromDate, string toDate, string username)
        {
            var result = new EODReportModel();

            var userInfo = GetCurrentUserInfo(username);

            using (var ctx = Entities.Create())
            {
                List<sp_GenerateEODReportPerUser_Result> spRes
                    = ctx.sp_GenerateEODReportPerUser(fromDate, toDate, userInfo.NetUserId).ToList();

                var userData = spRes.FirstOrDefault();

                if (userData != null)
                {
                    result.UserUniqueId = userData.UserUniqueId;
                    result.UserDetailsId = userData.UserDetailId.Value;
                    result.Fullname = userData.Fullname;
                    result.UserRole = userData.UserRole;
                    result.FirstName = userData.FirstName;
                    result.LastName = userData.LastName;

                    result.EODTaskList = spRes.Select(a => new EODTaskModel()
                    {
                        TaskNo = a.TaskNumber,
                        TaskName = a.TaskName,
                        TaskDescription = a.TaskDescription,
                        TaskId = a.TaskId,
                        IsTaskActive = (a.IsTaskActive).HasValue ? a.IsTaskActive.Value : false,
                        TotalActiveTime = (a.TotalActiveTime).HasValue ? a.TotalActiveTime.Value : 0,
                        TaskHistoryTypeId = a.TaskHistoryTypeId
                    }).ToList();
                }
            }

            return result;
        }

        public async Task BroadcastEditedEOD(int eodid, string userFullName, int userId,
            string returnURL, DateTime eodDate, List<EmailAddress> manualRecipients)
        {

            string userTeam = string.Empty;
            string userAccount = string.Empty;
            List<EmailAddress> recipients = new List<EmailAddress>();

            using (var ctx = Entities.Create())
            {

                /*ctx.Database.Log = Console.Write;*/
                List<int> sentRecipients = new List<int>();

                var taskIds = ctx.EODTaskItems
                    .Where(a => a.EODReportId == eodid)
                    .Select(b => b.TaskId)
                    .ToList();

                var taskList = ctx.IOMTasks.Where(a => taskIds.Contains(a.Id)).ToList();

                var uTeam = ctx.TeamMembers.Where(a => (a.UserDetailsId == userId) && (a.IsDeleted == false)).FirstOrDefault();

                
                
                
                if (uTeam != null)
                {
                    //get team detail
                    var teamDt = ctx.Teams.Where(a => a.Id == uTeam.TeamId).FirstOrDefault();

                    //if team detail is not null, get name of team and account of team
                    if (teamDt != null)
                    {
                        userTeam = teamDt.Name;
                        userAccount = ctx.Accounts.Where(a => a.Id == teamDt.AccountId).Select(a => a.Name)
                            .FirstOrDefault();
                        
                        
                        
                    }
                }

                string title = string.Empty;

                foreach (var task in taskList)
                {
                    try
                    {
                        var notificationRecipients = new List<int>();

                        var additionalRecipients = (from r in manualRecipients
                                                    join au in ctx.AspNetUsers on r.Email equals au.Email
                                                    join ud in ctx.UserDetails on au.Id equals ud.UserId
                                                    select ud.Id).ToList();

                        notificationRecipients.AddRange(additionalRecipients);
                        

                        var teamids = ctx.IOMTeamTasks.Where(a => a.TaskId == task.Id).Select(a => a.TeamId)
                            .ToList();

                       
                        foreach (var teamid in teamids)
                        {
                            var teammanagers = ctx.sp_GetTeamManagersandSupervisors(teamid)
                            .ToList();
                            
                            foreach (var manager in teammanagers)
                            {
                                var nameLocal = ctx.UserDetails.Where(a => a.Id == manager.UserId).Select(a => a.Name);
                                
                            }
                            notificationRecipients.AddRange(teammanagers.Select(t => t.UserId).ToList());

                            recipients.AddRange(teammanagers.Select(t => new EmailAddress
                            {
                                Email = t.Email,
                                Name = t.Name
                            }));
                        }

                        //Add all system administrators as recipients
                        notificationRecipients.AddRange(
                            ctx.UserDetails
                            .Where(a => a.Role == Globals.SYSAD_RC && a.IsDeleted != true && a.IsLocked != true)
                            .Select(a => a.Id)
                            .ToList());

                        
                        // Create Notifications
                        var cleanedList = notificationRecipients.Distinct().ToList();

                        

                        foreach (var uid in cleanedList)
                        {
                            /*System.Diagnostics.Debug.WriteLine("new Id: " + uid);*/
                            if (uid == userId) continue;

                            if (sentRecipients.IndexOf(uid) == -1)
                            {
                                sentRecipients.Add(uid);
                            }
                            else
                            {
                                continue;
                            }


                            


                            title = $"EOD Edit Request by {userFullName}";
                            var notificationMsg = $"{userFullName} has made an edit on their EOD report and is requiring your confirmation so that their edit will reflect on the Time Keeping Records. To confirm, please click on this link <a target='_blank' href='{ returnURL }'>LINK</a>";
                            
                            ctx.Notifications.Add(new Notification()
                            {
                                ToUserId = uid,
                                NoteDate = DateTime.UtcNow,
                                Icon = "fa-bell",
                                Title = title,
                                NoteType = NoteType.EODEdit.ToString(),
                                Message = notificationMsg,
                                SenderId = userId
                            });
                        }

                        ctx.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }

                recipients.AddRange(GetApprovedDeniedEODRecipients(0, manualRecipients));

                recipients.Add(new EmailAddress()
                {
                    Email = "administrator@ilucent.com",
                    Name = "administrator@ilucent.com "
                });

                recipients = recipients.IOMDistinctBy(r => r.Email).ToList();

                string emailBody = EmailBody.RequestApproval(userFullName, eodDate.ToShortDateString(),
                    userAccount, userTeam, returnURL);
                await SendEODEmailAsync(new IdentityMessage()
                {
                    Subject = title,
                    Body = emailBody
                }, recipients,
                EODAction.EditEOD,
                userId).ConfigureAwait(false);
            }
        }

        public async Task<int> SendEOD(EODRecipients eodModel, string fromDate,
            string toDate, double clientOffset, string userId, string chronoRoute)
        {
            var newEODId = -1;

            var userInfo = GetUserInfoById(userId);

            eodModel.Fullname = userInfo.FullName;

            if (SaveEOD(eodModel, fromDate, toDate, clientOffset, userId, out newEODId))
            {
                eodModel.ChronoDetailUrl = $"{chronoRoute}&eodId={newEODId}";
                var emailBody = EmailBody.EODReport(eodModel, fromDate, toDate);

                await SendEODEmailAsync(new IdentityMessage()
                    {
                        Subject = $"EOD Report: {EmailBody.ToTitleCase(eodModel.LastName)}, " +
                                  $"{EmailBody.ToTitleCase(eodModel.FirstName)} - {DateTime.Parse(fromDate).ToString("MM/dd/yyyy")}",
                        Body = emailBody
                    },
                    eodModel.Recipients
                        .Select(a => new EmailAddress() {Email = a.Email, Name = a.Name})
                        .IOMDistinctBy(e => e.Email)
                        .ToList(),
                    EODAction.SendEOD,
                    userInfo.UserDetailsId).ConfigureAwait(false);
            }

            return newEODId;
        }

        public async Task ApproveEOD(int eodId, string senderUserId, int currentUserId,
            string userFullName, string eodUrl)
        {
            var senderData = new UserDetail();
            var eodDate = string.Empty;
            var account = string.Empty;
            var team = string.Empty;
            var teamId = 0;
            var currentAspNetUser = new UserDetail();

            using (var ctx = Entities.Create())
            {
                var eod = ctx.EODReports.FirstOrDefault(a =>
                    a.Id == eodId && a.UserId == senderUserId && a.IsConfirmed == null);

                if (eod != null)
                {
                    eod.IsConfirmed = true;
                    eod.ConfirmedBy = currentUserId;
                    eod.ConfirmedUTCDate = DateTime.UtcNow;

                    currentAspNetUser = ctx.UserDetails.FirstOrDefault(a => a.Id == currentUserId);

                    var sentEOD = ctx.sp_GetSentEODReport(eodId).ToList();
                    if (sentEOD.Count > 0)
                    {
                        senderData = ctx.UserDetails.FirstOrDefault(a => a.UserId == senderUserId);

                        var uTeam = ctx.TeamMembers.Where(a => a.UserDetailsId == senderData.Id).FirstOrDefault();
                        teamId = uTeam.TeamId;

                        if (eodDate.Length == 0)
                        {
                            var firstEODitem = sentEOD.First();

                            eodDate = firstEODitem.EODDate.ToString("MM/dd/yyyy");
                        }

                        foreach (var item in sentEOD)
                        {
                            if (item.IsInserted == false && item.IsEdited == false && item.IsRemoved == false) continue;

                            decimal duration = 0;

                            if (item.IsRemoved == true)
                            {
                                duration = (-1 * (item.TotalTaskHours.Value * 60));

                                ctx.TaskHistories.Add(new TaskHistory()
                                {
                                    TaskId = item.TaskId,
                                    UserDetailsId = senderData.Id,
                                    HistoryDate = item.EODDate,
                                    TaskHistoryTypeId = 8,
                                    IsActive = false,
                                    Duration = duration
                                });
                            }
                            else if (item.IsInserted == false && item.IsEdited == true)
                            {
                                duration = item.AdjustedTotalHours.Value - item.TotalTaskHours.Value;
                                ctx.TaskHistories.Add(new TaskHistory()
                                {
                                    TaskId = item.TaskId,
                                    UserDetailsId = senderData.Id,
                                    HistoryDate = item.EODDate,
                                    TaskHistoryTypeId = 8,
                                    IsActive = false,
                                    Duration = (duration * 60)
                                });

                            }
                            else
                            {
                                duration = (item.IsEdited == true) ? item.AdjustedTotalHours.Value : item.TotalTaskHours.Value;
                                ctx.TaskHistories.Add(new TaskHistory()
                                {
                                    TaskId = item.TaskId,
                                    UserDetailsId = senderData.Id,
                                    HistoryDate = item.EODDate,
                                    TaskHistoryTypeId = 8,
                                    IsActive = false,
                                    Duration = (duration * 60)
                                });
                            }

                        }

                        await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }
                }

                var eodRecipients = eod.Recipients ?? "[]";
                var result = JsonConvert.DeserializeObject<List<EmailAddress>>(eodRecipients);
                var recipients = GetApprovedDeniedEODRecipients(teamId, result);
                recipients.Add(new EmailAddress()
                    {Email = "administrator@ilucent.com", Name = "administrator@ilucent.com "});

                var subj = $"EOD Confirmation for {senderData.Name} by {userFullName}";
                var emailBody = EmailBody.ApprovedDeniedEOD(true, userFullName, 
                    senderData.Name, eodDate, account, team, eodUrl);
                await SendEODEmailAsync(new IdentityMessage()
                {
                    Subject = subj,
                    Body = emailBody
                }, recipients,
                EODAction.ApproveEOD,
                currentUserId).ConfigureAwait(false);
            }

            if (currentAspNetUser != null)
            {
                var senderid = new List<int>();
                senderid.Add(senderData.Id);
                
                var approverDetails = GetUserInfoById(currentAspNetUser.UserId);
                var subj = $"EOD Confirmation for {senderData.Name} by {userFullName}";
                var notifmsg = $"Submitted <a href='{eodUrl}' target='_blank'>EOD</a> for approval was <b>APPROVED</b> by {approverDetails.FullName}";
                await NotifyTeamAdmins(teamId, subj, notifmsg, currentUserId, Utilities.NoteType.EODConfirm, senderid).ConfigureAwait(false);
            }
        }

        public async Task DenyEOD(int eodid, int currentUserId, string userFullName, string eodURL)
        {
            var senderData = new UserDetail();
            var eodDate = string.Empty;
            var account = string.Empty;
            var team = string.Empty;
            var teamId = 0;
            UserDetail currentAspNetUser = null;


            var result = new List<EmailAddress>();// JsonConvert.DeserializeObject<List<EmailAddress>>(eodRecipients);

            using (var ctx = Entities.Create())
            {
                var eod = ctx.EODReports.FirstOrDefault(a => a.Id == eodid
                                                             && a.IsConfirmed == null);

                currentAspNetUser = ctx.UserDetails.FirstOrDefault(a => a.Id == currentUserId);

                eod.IsConfirmed = false;
                eod.ConfirmedBy = currentUserId;
                eod.ConfirmedUTCDate = DateTime.UtcNow;

                var sentEOD = ctx.sp_GetSentEODReport(eodid).FirstOrDefault();
                if (sentEOD != null)
                {
                    senderData = ctx.UserDetails.FirstOrDefault(a => a.UserId == eod.UserId);
                    eodDate = sentEOD.EODDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);

                var eodRecipients = eod.Recipients;
                result = JsonConvert.DeserializeObject<List<EmailAddress>>(eodRecipients);
            }

            var recipients = GetApprovedDeniedEODRecipients(teamId, result);
            recipients.Add(
                new EmailAddress() {Email = "administrator@ilucent.com", Name = "administrator@ilucent.com "});

            var emailBody =
                EmailBody.ApprovedDeniedEOD(false, userFullName, senderData.Name, eodDate, account, team, eodURL);
            var subj = $"EOD Confirmation for {senderData.Name} by {userFullName}";
            await SendEODEmailAsync(new IdentityMessage()
            {
                Subject = subj,
                Body = emailBody
            }, recipients,
            EODAction.DenyEOD,
            currentUserId).ConfigureAwait(false);


            if (currentAspNetUser != null)
            {
                var senderid = new List<int>();
                senderid.Add(senderData.Id);

                var approverDetails = GetUserInfoById(currentAspNetUser.UserId);
                var notifmsg = $"Submitted <a href='{eodURL}' target='_blank'>EOD</a> for approval was <b>DENIED</b> by {approverDetails.FullName}";
                await NotifyTeamAdmins(teamId, "EOD Edit Request Notification", notifmsg, currentUserId, NoteType.EODEdit, senderid).ConfigureAwait(false);
            }
        }

        public async Task NotifyTeamAdmins(int teamId, string title, string message, int userid, NoteType nType, List<int> additionalRecipients = null)
        {
            using (var ctx = Entities.Create())
            {
                var sentRecipients = new List<int>();

                var timeUtc = DateTime.UtcNow;
                var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

                var notificationRecipients = ctx.sp_GetTeamManagersandSupervisors(teamId)
                        .Select(a => new
                        {
                            UserId = a.UserId,
                            AccountName = a.AccountName,
                            TeamName = a.TeamName
                        })
                        .ToList();

                    notificationRecipients.AddRange(
                        ctx.UserDetails
                        .Where(a => a.Role == "SA")
                        .Select(a => new { UserId = a.Id, AccountName = string.Empty, TeamName = string.Empty })
                        .ToList());

                    // Create Notifications
                    var cleanedList = notificationRecipients.Distinct().ToList();
                    foreach (var item in cleanedList)
                    {
                        if (item.UserId == userid) continue;

                        if (sentRecipients.IndexOf(item.UserId) == -1)
                        {
                            sentRecipients.Add(item.UserId);
                        }
                        else
                        {
                            continue;
                        }

                        ctx.Notifications.Add(new Notification()
                        {
                            ToUserId = item.UserId,
                            NoteDate = easternTime,
                            Icon = "fa-bell",
                            Title = title,
                            NoteType = nType.ToString(),
                            Message = message
                        });
                    }

                    if (additionalRecipients != null)
                    {
                        foreach (var rec in additionalRecipients)
                        {
                            ctx.Notifications.Add(new Notification()
                            {
                                ToUserId = rec,
                                NoteDate = easternTime,
                                Icon = "fa-bell",
                                Title = title,
                                NoteType = nType.ToString(),
                                Message = message
                            });
                        }
                    }

                    await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public IList<Recipient> GetUserEODRecipients(int userDetailsId, NotificationType notificationType)
        {
            using (var ctx = Entities.Create())
            {
                return ctx.sp_GetUserSuperiors(userDetailsId, (int)notificationType)
                    .Where(f => f.IsAllowed.HasValue && f.IsAllowed.Value)
                    .Select(r => new Recipient
                    {
                        Email = r.Email,
                        Name = r.FirstName + " " + r.LastName,
                        IsAllowed = r.IsAllowed
                    }).ToList();
            }
        }

        public List<EmailAddress> GetApprovedDeniedEODRecipients(int teamId, List<EmailAddress> manualRecipients)
        {
            List<EmailAddress> result = new List<EmailAddress>();

            using (var ctx = Entities.Create())
            {
                result = ctx.sp_GetTeamManagersandSupervisors(teamId)
                                    .Select(a => new EmailAddress()
                                    {
                                        Email = a.Email,
                                        Name = a.Name
                                    })
                                    .ToList();

                var additionalRecipients = (from r in manualRecipients
                                            join au in ctx.AspNetUsers on r.Email equals au.Email
                                            join ud in ctx.UserDetails on au.Id equals ud.UserId
                                            select new EmailAddress()
                                            {
                                                Email = r.Email,
                                                Name = ud.Name
                                            }).ToList();

                result.AddRange(additionalRecipients);
            }

            return result.Distinct().ToList();
        }

        public bool SaveEOD(EODRecipients eodReport, string fromDate, string toDate,
            double clientOffset, string userId, out int newEODId)
        {
            bool result = true;
            newEODId = 0;

            DateTime eodDate;
            if (!DateTime.TryParse(fromDate, out eodDate))
            {
                return false;
            }

            bool eodIsEdited = false;

            // check for edited items
            foreach (var item in eodReport.EODTaskList)
            {
                if (item.IsAdjusted || item.IsInserted || item.IsRemoved)
                {
                    eodIsEdited = true;
                    break;
                }
            }

            using (var ctx = Entities.Create())
            {
                using (var transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var newEodRecord = ctx.EODReports.Add(new EODReport()
                        {
                            UserId = userId,
                            Note = eodReport.Note,
                            EODDate = eodDate,
                            SentUTCDateTime = DateTime.UtcNow,
                            DateCreated = DateTime.UtcNow,
                            ClientOffset = clientOffset,
                            IsEdited = eodIsEdited,
                            Recipients = new JavaScriptSerializer().Serialize(eodReport.Recipients)
                        });

                        ctx.SaveChanges();

                        newEODId = newEodRecord.Id;

                        ctx.EODTaskItems.AddRange(eodReport.EODTaskList.Select(a => new EODTaskItem()
                        {
                            TaskId = a.TaskId,
                            TotalTaskHours = a.TotalActiveTime,
                            AdjustedTotalHours = a.AdjustedTotalActiveTime,
                            EODReportId = newEodRecord.Id,
                            IsEdited = a.IsAdjusted,
                            IsInserted = a.IsInserted,
                            IsRemoved = a.IsRemoved
                        }));

                        ctx.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        result = false;

                        throw new Exception(e.Message, e.InnerException);
                    }
                }

            }

            return result;
        }

        public async Task SendEODEmailAsync(IdentityMessage message,
            List<EmailAddress> recipients,
            EODAction eodAction,
            int userDetailsId)
        {
            var refCode = ServiceUtility.GetRandomCode();
            message.Subject = $"Ref: {refCode} | {message.Subject}";
            message.Destination = string.Join(", ", recipients.Select(r => r.Email).ToArray());

            var systemLog = new SystemLog
            {
                LogDate = DateTime.UtcNow,
                ActorUserId = userDetailsId,
                ActionType = eodAction.GetStringValue(),
                Entity = "EOD",
                RequestBody = (new JavaScriptSerializer()).Serialize(message),
                EODEmailReference = refCode
            };

            await SendGridMailServices.Instance.SendMultipleAsync(message, recipients)
                .ConfigureAwait(false);

            SaveToDb(systemLog);
        }
    }
}