using System.Collections.Concurrent;
using System.Linq;
using System.Security.Claims;
using IOM.Services.Interface;
using Microsoft.AspNet.SignalR;

namespace IOM.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly INotificationServices _notificationServices;

        public NotificationHub(INotificationServices notificationServices)
        {
            _notificationServices = notificationServices;
        }
        
        private static readonly ConcurrentDictionary<string, string> Users
            = new ConcurrentDictionary<string, string>();
        public void FetchNotification()
        {
            if (Context.User != null)
            {
                var identity = (ClaimsIdentity)Context.User.Identity;
                Claim userIdentity = identity.Claims.First(c =>
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                Clients.Client(Context.ConnectionId)
                    .NotificationData(_notificationServices.FetchRecentNotifications(userIdentity.Value));
            }
        }
    }
}