using System.Collections.Generic;
using System.Threading.Tasks;
using IOM.DbContext;
using Microsoft.AspNet.Identity;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        IList<AccountMember> GetMembers(int userDetailsId, string roleCode);
        AccountMember GetMember(int userDetailsId, string roleCode);
        Task SendSeatStatusUpdateEmailUser(IdentityMessage message, string recipient);
    }
}