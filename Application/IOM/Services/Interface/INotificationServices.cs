using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IOM.Models.ApiControllerModels;

namespace IOM.Services.Interface
{
    public interface INotificationServices
    {
        Task MarkAsRead(int notificationId, CancellationToken cancellationToken);
        Task BatchMarkAsRead(int[] notificationIds, CancellationToken cancellationToken);
        Task MarkAsUnread(int notificationId, CancellationToken cancellationToken);
        object GetNotifications(string startDate, string endDate, int userDetailsId);
        Task<bool> IsThereNotificationAsync(int userDetailsId, CancellationToken cancellationToken);
        void NotifyReminder();
        void AttendanceReminder();
        List<NotificationModel> FetchRecentNotifications(string netUserId);
    }
}