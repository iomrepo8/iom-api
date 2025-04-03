using IOM.DbContext;
using IOM.Models.ApiControllerModels;
using IOM.Services.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public Task<IList<UserShiftModel>> GetUserShiftAsync(string[] roles, int[] accountIds, int[] teamIds, 
            int[] tagIds, int[] userIds, string username)
        {
            var userInfo = GetCurrentUserInfo(username);
            var userIdsParams = string.Join(",", userIds);
            var accountIdsParams = string.Join(",", accountIds);
            var teamIdsParams = string.Join(",", teamIds);
            var rolesParams = string.Join(",", roles);
            var tagIdsParams = string.Join(",", tagIds);
            
            using (var ctx = Entities.Create())
            {
                var jsonResult = string.Join("", ctx.sp_GetUserShiftData(roles: rolesParams, userIds: userIdsParams,
                    accountIds: accountIdsParams, tagIds: tagIdsParams, teamIds: teamIdsParams,
                    userDetailsId: userInfo.UserDetailsId).ToList());

                if (jsonResult.Length > 0)
                {
                    var dataQuery = (new JavaScriptSerializer())
                        .Deserialize<IList<UserShiftModel>>(jsonResult);

                    dataQuery = dataQuery.OrderBy(e => e.FirstName).ToList();

                    return Task.FromResult(dataQuery);
                }

                return Task.FromResult<IList<UserShiftModel>>(new List<UserShiftModel>());
            }
        }

        public async Task SaveUserShiftDataAsync(UserShiftDataRequest userShiftDataRequest, 
            CancellationToken cancellationToken)
        {
            using (var ctx = Entities.Create())
            {
                var _user = ctx.UserDetails.SingleOrDefault(u => u.Id == userShiftDataRequest.UserDetailsId);

                _user.TimeZoneId = userShiftDataRequest.TimeZoneId;

                if (userShiftDataRequest != null && userShiftDataRequest.ShiftDetailsId > 0)
                {
                    var existingData = ctx.UserShiftDetails.SingleOrDefault(s =>
                        s.Id == userShiftDataRequest.ShiftDetailsId && s.UserDetailsId == userShiftDataRequest.UserDetailsId);

                    if (existingData != null)
                    {
                        existingData.LunchBreak = userShiftDataRequest.LunchBreak;
                        existingData.PaidBreaks = (byte)userShiftDataRequest.PaidBreaks;
                        existingData.ShiftStart = TimeSpan.Parse(userShiftDataRequest.ShiftStart);
                        existingData.ShiftEnd = TimeSpan.Parse(userShiftDataRequest.ShiftEnd);
                    }
                }
                else
                {
                    ctx.UserShiftDetails.Add(new UserShiftDetail
                    {
                        UserDetailsId = userShiftDataRequest.UserDetailsId,
                        ShiftStart = TimeSpan.Parse(userShiftDataRequest.ShiftStart),
                        ShiftEnd = TimeSpan.Parse(userShiftDataRequest.ShiftEnd),
                        LunchBreak = userShiftDataRequest.LunchBreak,
                        PaidBreaks = (byte) userShiftDataRequest.PaidBreaks
                    });
                }

                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<IList<TimeZoneModel>> GetTimeZones(CancellationToken cancellationToken)
        {
            using (var ctx = Entities.Create())
            {
                var data = await ctx.TimeZones.Select(t => new TimeZoneModel
                {
                    Id = t.Id,
                    Zone = t.Zone,
                    Value = t.Value
                }).ToArrayAsync(cancellationToken)
                    .ConfigureAwait(false);

                return data;
            }
        }
    }
}