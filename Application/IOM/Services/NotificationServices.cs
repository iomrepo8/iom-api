using System.Collections.Generic;
using IOM.DbContext;
using IOM.Helpers;
using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Utilities;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IOM.Services.Interface;

namespace IOM.Services
{
    public class NotificationServices : INotificationServices
    {
        public async Task MarkAsRead(int notificationId, CancellationToken cancellationToken)
        {
            using (var ctx = Entities.Create())
            {
                var notification = await ctx.Notifications
                    .SingleOrDefaultAsync(n => n.Id == notificationId, cancellationToken)
                    .ConfigureAwait(false);

                if (notification != null)
                {
                    notification.IsRead = true;
                    notification.ReadDate = DateTimeUtility.Instance.DateTimeNow();
                }

                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task BatchMarkAsRead(int[] notificationIds, CancellationToken cancellationToken)
        {
            using (var ctx = Entities.Create())
            {
                var notifications = await ctx.Notifications
                    .Where(n => notificationIds.Contains(n.Id))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);

                var readDate = DateTimeUtility.Instance.DateTimeNow();

                foreach (Notification item in notifications)
                {
                    item.IsRead = true;
                    item.ReadDate = readDate;
                }

                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task MarkAsUnread(int notificationId, CancellationToken cancellationToken)
        {
            using (var ctx = Entities.Create())
            {
                var notification = await ctx.Notifications
                    .SingleOrDefaultAsync(n => n.Id == notificationId, cancellationToken)
                    .ConfigureAwait(false);

                if (notification != null)
                {
                    notification.IsRead = false;
                    notification.ReadDate = null;
                }

                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public object GetNotifications(string startDate, string endDate, int userDetailsId)
        {
            using (var ctx = Entities.Create())
            {
                var notifications = ctx.sp_GetNotifications(startDate, endDate, userDetailsId).ToList();

                return notifications;
            }
        }

        public async Task<bool> IsThereNotificationAsync(int userDetailsId, CancellationToken cancellationToken)
        {
            using (var ctx = Entities.Create())
            {
                var notifications = await ctx.Notifications.Where(n => n.ToUserId == userDetailsId 
                && n.IsRead == false && n.IsArchived == false).Select(q => q.Id)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

                return notifications.Count > 0;
            }
        }

        public void NotifyReminder()
        {
            var noteType = NoteType.ReminderAgentStatus.ToString();

            using (var ctx = Entities.Create())
            {
                var excludedUserIds = ctx.Notifications.Where(t => t.IsRead == false && t.NoteType == noteType).Select(e => e.ToUserId).ToList();

                var recipients = ctx.UserDetails.Where(u => u.Role == Globals.LEAD_AGENT_RC || u.Role == "SA").ToList();

                recipients = recipients.Where(r => !excludedUserIds.Contains(r.Id)).ToList();

                foreach(var recipient in recipients)
                {
                    _ = ctx.Notifications.Add(new Notification
                    {
                        ToUserId = recipient.Id,
                        NoteDate = DateTimeUtility.Instance.DateTimeNow(),
                        Message = Resources.RemAgentStatus,
                        Title = Resources.RemAgentStatusTitle,
                        Icon = Resources.ReminderIcon,
                        NoteType = NoteType.ReminderAgentStatus.ToString()
                    });
                }

                ctx.SaveChanges();
            }
        }

        public void AttendanceReminder()
        {
            var noteType = NoteType.ReminderAttendance.ToString();

            using (var ctx = Entities.Create())
            {
                var excludedUserIds = ctx.Notifications.Where(t => t.IsRead == false && t.NoteType == noteType).Select(e => e.ToUserId).ToList();

                var recipients = ctx.UserDetails.Where(u => u.Role == Globals.LEAD_AGENT_RC || u.Role == "SA" || u.Role == "AM").ToList();

                recipients = recipients.Where(r => !excludedUserIds.Contains(r.Id)).ToList();

                foreach (var recipient in recipients)
                {
                    _ = ctx.Notifications.Add(new Notification
                    {
                        ToUserId = recipient.Id,
                        NoteDate = DateTimeUtility.Instance.DateTimeNow(),
                        Message = Resources.RemAttendanceData,
                        Title = Resources.RemAttendanceDataTitle,
                        Icon = Resources.ReminderIcon,
                        NoteType = NoteType.ReminderAttendance.ToString()
                    });
                }

                ctx.SaveChanges();
            }
        }

        public List<NotificationModel> FetchRecentNotifications(string netUserId)
        {
            using (var ctx = Entities.Create())
            {
                var userDetailsId = (from u in ctx.UserDetails
                    where u.UserId == netUserId
                    select u.Id).FirstOrDefault();

                var notifications = ctx.sp_GetRecentNotifications(userDetailsId).ToList();

                foreach (var note in notifications)
                {
                    var noteData = ctx.Notifications.SingleOrDefault(n => n.Id == note.Id);

                    if (noteData != null) noteData.IsDisplayed = true;
                }

                ctx.SaveChanges();
                
                return notifications.Select(n => new NotificationModel
                {
                    NoteDate = n.NoteDate,
                    Icon = n.Icon,
                    Title = n.Title,
                    Description = n.Message,
                    IsDeleted = n.IsArchived
                }).ToList();
            }
        }
    }
}