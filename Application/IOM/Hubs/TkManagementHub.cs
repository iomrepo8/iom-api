using IOM.Hubs.Models;
using IOM.Services;
using Microsoft.AspNet.SignalR;
using System.Linq;
using System.Security.Claims;
using IOM.Services.Interface;

namespace IOM.Hubs
{
    public class TkManagementHub : Hub
    {
        private readonly IRepositoryService _repositoryService;

        public TkManagementHub(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        public void FetchTkManagementData(TkManagementRequest request)
        {
            _repositoryService.UpdateUsersActiveHours();
            
            if (Context.User != null)
            {
                var identity = (ClaimsIdentity)Context.User.Identity;

                Claim userIdentity = identity.Claims.First(c =>
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

                Clients.Client(Context.ConnectionId)
                    .TkManagementData(_repositoryService.GetTkManagementData(request, userIdentity.Value));
            }
        }
    }
}