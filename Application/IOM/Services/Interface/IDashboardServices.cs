using IOM.Models.ApiControllerModels;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        DashboardData GetDashboardDataByUserId(string netUserId);
        DashboardData GetDashboardData(string username);
    }
}