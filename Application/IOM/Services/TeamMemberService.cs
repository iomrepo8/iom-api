using IOM.DbContext;
using IOM.Models.ApiControllerModels;
using System.Collections.Generic;
using System.Linq;
using IOM.Services.Interface;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public IList<UserModel> GetTeamMembers(int teamId, string roleCode)
        {
            using (var ctx = Entities.Create())
            {
                return (from tm in ctx.TeamMembers
                        join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                        where tm.TeamId == teamId && u.Role == roleCode && tm.IsDeleted != true
                        select new UserModel
                        {
                            UserDetailsId = u.UserDetailsId,
                            NetUserId = u.NetUserId,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            RoleCode = u.Role
                        }).ToList();
            }
        }
    }
}