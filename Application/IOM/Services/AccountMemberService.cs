using IOM.DbContext;
using System.Collections.Generic;
using System.Linq;
using IOM.Services.Interface;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public IList<AccountMember> GetMembers(int userDetailsId, string roleCode)
        {
            using (var ctx = Entities.Create())
            {
                return (from am in ctx.AccountMembers
                        join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                        where u.Role == roleCode && am.UserDetailsId == userDetailsId
                        select am).ToList();
            }
        }

        public AccountMember GetMember(int userDetailsId, string roleCode)
        {
            using (var ctx = Entities.Create())
            {
                return (from am in ctx.AccountMembers
                        join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                        where am.UserDetailsId == userDetailsId && u.Role == roleCode
                        select am).FirstOrDefault();
            }
        }
    }
}