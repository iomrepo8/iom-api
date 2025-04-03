using System.Collections.Generic;
using System.Threading.Tasks;
using IOM.Models.ApiControllerModels;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        TeamDashboard GetTeamsDashboardData(int[] accountIds, int[] teamids, string username, bool showInactive = false);
        int GetTeamsCount(string username);
        IList<TeamLookUp> TeamsLookup(string username);
        IList<TeamLookUp> GetTeamsByAccounts(int[] accountIds, string username);
        IList<TeamLookUp> GetAllTeamsByAccounts(List<int> accountIds);
        IList<LookUpModel> GetAssignedTeamsList(string username, bool includeDeleted, string query = "");
        IEnumerable<TeamRawModel> GetAssignedTeamsRaw(string username, bool includeDeleted, string query = "");
        TeamDetailModel GetTeamDetails(int teamId, string username);
        IList<BaseModel> GetTeams(int userDetailsId);
        Task AddClientAsync(TeamMemberModel client);
        void UpdateDayoffHoliday(DayOffHolidayModel dayOffHoliday);
        void ChangeAgentTeams(ChangeAccountModel model);
        void RemoveAgent(TeamMemberModel agentEDModel);
        void RemoveClient(TeamMemberModel client);
        Task AddAgentAsync(TeamMemberModel addModel);
        object AgentsLookup(int teamId, string query);
        void RemoveTeamMembers(int accountId, int userDetailsId);
        Task AddLeadAgentAsync(TeamMemberModel leadAgent);
        IList<LookUpModel> LeadAgents(int teamId, string query = "");
        void RemoveLeadAgent(TeamMemberModel leadAgent);
        object GetManagers(int teamId);
        void RemoveManager(TeamMemberModel manager);
        Task AddManagerAsync(TeamMemberModel manager);
        Task AssignTask(TeamTaskBase addTeamTask, string username);
        Task RemoveTask(int teamTaskId, string username);
        IList<LookUpModel> GetClients(int teamId, string query = "");
        void SaveTeam(TeamModel teamModel);
        TeamModel GetTeam(int teamId);
        void DeleteTeam(int teamId);
        void DeactivateTeam(int teamId);
        void ActivateTeam(int teamId);
        bool IsDeletable(int teamId);
        IList<LookUpModel> GetTeamsByAccountId(int accountId, string query);
    }
}