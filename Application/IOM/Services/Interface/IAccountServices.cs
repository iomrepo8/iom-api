using System.Collections.Generic;
using System.Threading.Tasks;
using IOM.DbContext;
using IOM.Models.ApiControllerModels;
using Microsoft.AspNet.Identity;
using SendGrid.Helpers.Mail;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        IList<AccountLookUp> AccountsLookup(string username);
        AccountDataModel GetAccountDetails(int accountId, string username);
        IList<LookUpModel> GetTeamManagers(int accountId, string query = "");
        void RemoveTeamManager(AccountMemberModel manager);
        Task AddManagerAsync(AccountMemberModel manager);
        IList<LookUpModel> GetLeadAgents(int accountId);
        void RemoveLeadAgent(AccountMemberModel leadAgent);
        Task AddLeadAgentAsync(AccountMemberModel amModel);
        IList<LookUpModel> GetAccountManagers(int accountId, string key = "");
        IList<UserBasicInfoModel> GetAccountManagers(List<int> accountId);
        List<vw_ActiveUsers> GetAccountManagersFullDetails(int accountId, string key = "");
        Task AddAccountManagerAsync(AccountMemberModel amModel);
        void RemoveAccountManager(AccountMemberModel amModel);
        IList<LookUpModel> GetClients(string key, int accountId);
        Task AddClientAsync(Personel client);
        void RemoveClient(AccountMemberModel amModel);
        void RemoveAgent(AccountUserModel accUserModel);
        Task AddAgentAsync(AddAgentRequest addAgentRequest);
        IList<LookUpModel> GetAssignedAccountsList(string username, bool includeDeleted, string query = "");
        IEnumerable<AccountModel> GetAssignedAccountsRaw(string username, bool includeDeleted, string query = "");
        IList<BaseModel> GetAccounts(int userDetailsId);
        void ChangeAgentAccount(ChangeAccountModel model);
        void SaveAccount(AccountDataModel accountModel);
        void SaveAccountSeatDetails(AccountDataModel accountModel, string userActionId, string siteurl);
        void DeleteAccount(int accountId);
        void DeactivateAccount(int accountId);
        void ActivateAccount(int accountId);
        AccountDataModel GetAccount(int accountId);
        int GetAccountsCount(string username);
        AccountDashboard GetAccountsDashboardData(string username, bool showInactive = false);
        bool IsNameDuplicate(string name, int accountId = 0);
        IList<TeamUsersModel> CompileList(IList<TeamUsersModel> list);

        Task NotifyAndMail(IdentityMessage message,
            List<vw_ActiveUsers> managers,
            List<vw_ActiveUsers> sysAds,
            string action,
            int userDetailsId);

        Task SendSeatStatusUpdateEmail(IdentityMessage message,
            List<EmailAddress> recipients,
            string action,
            int userDetailsId);
    }
}