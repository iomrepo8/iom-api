using Microsoft.AspNet.SignalR;

namespace IOM.Hubs
{
    [Authorize]
    public class UserSessionHub : Hub
    {
        public void SignalUserDataUpdate()
        {
            Clients.All.Trigger_FetchUserData();
        }
    }
}