using IOM.DbContext;
using IOM.Models.ApiControllerModels;
using System.Linq;
using IOM.Services.Interface;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public DashboardData GetDashboardDataByUserId(string netUserId)
        {
            using (var ctx = Entities.Create())
            {
                var username = (from u in ctx.UserDetails
                           join au in ctx.AspNetUsers on u.UserId equals au.Id
                           where u.UserId == netUserId
                           select au.UserName).FirstOrDefault();

                return GetDashboardData(username);
            }
        }

        public DashboardData GetDashboardData(string username)
        {
            return new DashboardData
            {
                AccountCount = GetAccountsCount(username),
                TeamCount = GetTeamsCount(username),
                UserCount = GetUsersCount(username),
                OnlineUserCount = GetOnlineUserCount(username),
                HourCount = GetTodayHourCount(username)
            };
        }
    }
}