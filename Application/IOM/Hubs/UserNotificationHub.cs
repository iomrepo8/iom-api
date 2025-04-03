using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;

namespace IOM.Hubs
{
    [Authorize]
    public class UserNotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> Users
            = new ConcurrentDictionary<string, string>();

        public void Notify()
        {
            Clients.All.BroadcastNotification();
        }
    }
}