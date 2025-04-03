using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IOM.Models.ApiControllerModels;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        Task<IList<UserShiftModel>> GetUserShiftAsync(string[] roles, int[] accountIds, int[] teamIds,
            int[] tagIds, int[] userIds, string username);

        Task SaveUserShiftDataAsync(UserShiftDataRequest userShiftDataRequest, CancellationToken cancellationToken);

        Task<IList<TimeZoneModel>> GetTimeZones(CancellationToken cancellationToken);
    }
}