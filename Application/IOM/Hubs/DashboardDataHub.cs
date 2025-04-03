using IOM.Services;
using Microsoft.AspNet.SignalR;
using System.Linq;
using System.Security.Claims;
using IOM.Services.Interface;

namespace IOM.Hubs
{
    public class DashboardDataHub : Hub
    {
        private readonly IRepositoryService _repositoryService;

        public DashboardDataHub(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        public void FetchDashboardData()
        {
            if (Context.User != null)
            {
                var identity = (ClaimsIdentity)Context.User.Identity;

                var userIdentity = identity.Claims.First(c =>
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

                Clients.Client(Context.ConnectionId)
                    .DashboardData(_repositoryService.GetDashboardDataByUserId(userIdentity.Value));
            }
        }

        public void TickTimer()
        {
            _repositoryService.UpdateUsersActiveHours();
        }
    }
}