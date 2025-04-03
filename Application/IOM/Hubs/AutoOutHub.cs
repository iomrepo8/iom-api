using IOM.Services;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using IOM.Services.Interface;

namespace IOM.Hubs
{
    [Authorize]
    public class AutoOutHub : Hub
    {
        private readonly IRepositoryService _repositoryService;
        public AutoOutHub(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        public void Heartbeat()
        {
            Clients.All.heartbeat(GetOnlineUsersCount());

            var user = Context.User.Identity.Name;
        }

        public void UpdateUserActiveTime()
        {
            _repositoryService.UpdateActiveTime(Context.User.Identity.Name);
            _repositoryService.SetUserOnline(Context.User.Identity.Name);
        }

        public override Task OnConnected()
        {
            _repositoryService.SetUserOnline(Context.User.Identity.Name);
            Heartbeat();

            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            _repositoryService.SetUserOnline(Context.User.Identity.Name);
            Heartbeat();

            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            _repositoryService.SetUserOffline(Context.User.Identity.Name);
            Heartbeat();

            return base.OnDisconnected(stopCalled);
        }

        private int GetOnlineUsersCount()
        {
            return _repositoryService.GetOnlineUserCount(Context.User.Identity.Name);
        }
    }
}