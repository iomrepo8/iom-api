using System.Collections.Generic;
using IOM.Models.ApiControllerModels;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        IList<UserModel> GetTeamMembers(int teamId, string roleCode);
    }
}