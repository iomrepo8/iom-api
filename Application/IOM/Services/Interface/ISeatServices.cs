using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IOM.Models;
using IOM.Models.ApiControllerModels;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        List<AccountSeatModel> GetAvailableAccountSeats(int accountid);
        List<SeatDashboardViewModel> GetDashboardSeats(int[] accountIds);

        Task<ApiResult> OccupySeat(int accountId, int userid, int seatNumber, string occupyType, string siteUrl, string userInAction,
            CancellationToken cancellationToken);
        bool UpdateStatus(int accountid, int userid, int seatnumber, string occupytype, string siteUrl, string userInAction, out string msg);
        bool VacanSeat(int accountid, int userid, int seatnumber, string siteUrl, string userInaction, out string msg);

        string SeatNameGenerator(int seat, string code);
    }
}