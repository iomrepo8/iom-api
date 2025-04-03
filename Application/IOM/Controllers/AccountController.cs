using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using IOM.Services.Interface;

namespace IOM.Controllers.WebApi
{
    [Authorize]
    [RoutePrefix("accounts")]
    public class AccountsController : ApiController
    {
        private readonly IRepositoryService _repositoryService;

        public AccountsController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        
        [HttpGet]
        [Route("assigned")]
        public ApiResult AccountsLookup()
        {
            var result = new ApiResult
            {
                data = _repositoryService.AccountsLookup(User.Identity.Name)
            };

            return result;
        }

        [HttpGet]
        [Route("list")]
        public ApiResult AccountsData([FromUri] bool showInactive)
        {
            var result = new ApiResult
            {
                data = _repositoryService
                    .GetAccountsDashboardData(User.Identity.Name, showInactive)
            };

            return result;
        }

        [HttpGet]
        [Route("get")]
        public ApiResult GetAccount([FromUri] int accountId)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetAccount(accountId)
            };

            return result;
        }

        [HttpGet]
        [Route("details")]
        public ApiResult GetAccountDetails([FromUri] int accountId)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetAccountDetails(accountId, User.Identity.Name)
            };

            return result;
        }

        [HttpGet]
        [Route("is_deletable")]
        public ApiResult IsDeletable([FromUri] int accountId)
        {
            var result = new ApiResult
            {
                //result.data = new { IsDeletable = _repositoryService.IsDeletable(accountId) };
                data = new { IsDeletable = true }
            };

            return result;
        }

        [HttpPost]
        [Route("save")]
        public ApiResult SaveAccount(AccountDataModel accountModel)
        {
            var result = new ApiResult();

            _repositoryService.SaveAccount(accountModel);
            result.message = accountModel.Id > 0 ?
                Resources.AccountSuccessUpdate : Resources.AccountSuccessAdd;

            return result;
        }

        [HttpPost]
        [Route("saveseatdetails")]
        public ApiResult SaveAccountSeatDetails(AccountDataModel accountModel)
        {
            var result = new ApiResult();
            var siteUrl = Request.RequestUri.Authority + "/seats";

            _repositoryService.SaveAccountSeatDetails(accountModel, User.Identity.GetUserId(), siteUrl);
            result.message = Resources.AccountSuccessUpdate;

            return result;
        }

        [HttpPost]
        [Route("delete")]
        public ApiResult DeleteAccount([FromBody] int accountId)
        {
            var result = new ApiResult();

            _repositoryService.DeleteAccount(accountId);
            result.message = Resources.AccountSuccessDelete;

            return result;
        }

        [HttpPost]
        [Route("deactivate")]
        public ApiResult DeactivateAccount([FromBody] int accountId)
        {
            var result = new ApiResult();

            _repositoryService.DeactivateAccount(accountId);
            result.message = Resources.AccountSuccessDeactivate;

            return result;
        }

        [HttpPost]
        [Route("activate")]
        public ApiResult ActivateAccount([FromBody] int accountId)
        {
            var result = new ApiResult();

            _repositoryService.ActivateAccount(accountId);
            result.message = Resources.AccountSuccessActivate;

            return result;
        }

        [HttpPost]
        [Route("add_agent")]
        public async Task<ApiResult> AddAgent(AddAgentRequest agentRequest)
        {
            var result = new ApiResult();

            await _repositoryService.AddAgentAsync(agentRequest);
            
            result.message = agentRequest.IsEdit ? Resources.AgentSuccessEditAssTeams
                : Resources.AgentSuccessAdd;

            return result;
        }

        [HttpPost]
        [Route("remove_agent")]
        public ApiResult RemoveAgent(AccountUserModel teamModel)
        {
            var result = new ApiResult();

            _repositoryService.RemoveAgent(teamModel);
            result.message = Resources.AgentSuccessRemove;

            return result;
        }

        #region Lead Agents
        [HttpPost]
        [Route("add_lead_agent")]
        public async Task<ApiResult> AddLeadAgent(AccountMemberModel model)
        {
            var result = new ApiResult();

            await _repositoryService.AddLeadAgentAsync(model);
            result.message = Resources.LASuccessAdd;

            return result;
        }

        [HttpPost]
        [Route("remove_lead_agent")]
        public ApiResult RemoveLeadAgent(AccountMemberModel teamModel)
        {
            ApiResult result = new ApiResult();

            _repositoryService.RemoveLeadAgent(teamModel);
            result.message = Resources.LASuccessRemove;

            return result;
        }

        [HttpGet]
        [Route("lead_agents")]
        public ApiResult GetLeadAgents([FromUri] int accountId)
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.GetLeadAgents(accountId);

            return result;
        }
        #endregion

        #region Team Managers
        [HttpPost]
        [Route("add_team_manager")]
        public async Task<ApiResult> AddTeamManager(AccountMemberModel manager)
        {
            var result = new ApiResult();

            if (manager is null) throw new ArgumentNullException(nameof(manager));

            await _repositoryService.AddManagerAsync(manager);
            result.message = Resources.TeamManagerSuccessAdd;

            return result;
        }

        [HttpPost]
        [Route("remove_team_manager")]
        public ApiResult RemoveTeamManager(AccountMemberModel manager)
        {
            ApiResult result = new ApiResult();

            if (manager is null) throw new ArgumentNullException(nameof(manager));

            _repositoryService.RemoveTeamManager(manager);
            result.message = Resources.TeamManagerSuccessRemove;

            return result;
        }

        [HttpGet]
        [Route("team_managers")]
        public ApiResult GetTeamManagers([FromUri] int accountId)
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.GetTeamManagers(accountId);

            return result;
        }
        #endregion

        #region Account Managers
        [HttpGet]
        [Route("managers")]
        public ApiResult GetManagers([FromUri] string key, [FromUri] int accountId = 0)
        {
            ApiResult result = new ApiResult();


            result.data = _repositoryService.GetAccountManagers(accountId, key);

            return result;
        }

        [HttpPost]
        [Route("add_manager")]
        public async Task<ApiResult> AddManager(AccountMemberModel amModel)
        {
            var result = new ApiResult();

            if (amModel == null) throw new ArgumentNullException(nameof(amModel));
            await _repositoryService.AddAccountManagerAsync(amModel).ConfigureAwait(false);
            result.message = Resources.AccountManagerSuccessAdd;

            return result;
        }

        [HttpPost]
        [Route("remove_manager")]
        public ApiResult RemoveManager(AccountMemberModel amModel)
        {
            ApiResult result = new ApiResult();

            if (amModel == null) throw new ArgumentNullException(nameof(amModel));
            _repositoryService.RemoveAccountManager(amModel);
            result.message = Resources.AccountManagerSuccessRemove;

            return result;
        }
        #endregion

        #region Clients
        [HttpGet]
        [Route("clients")]
        public ApiResult GetClients([FromUri] string key, [FromUri] int accountId = 0)
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.GetClients(key, accountId);

            return result;
        }

        [HttpPost]
        [Route("add_client")]
        public async Task<ApiResult> AddClient(Personel cpModel)
        {
            var result = new ApiResult();

            if (cpModel == null) throw new ArgumentNullException(nameof(cpModel));

            await _repositoryService.AddClientAsync(cpModel).ConfigureAwait(false);
            result.message = Resources.ClientSuccessAdd;

            return result;
        }

        [HttpPost]
        [Route("remove_client")]
        public ApiResult RemoveClient(AccountMemberModel amModel)
        {
            ApiResult result = new ApiResult();

            if (amModel == null) throw new ArgumentNullException(nameof(amModel));
            _repositoryService.RemoveClient(amModel);
            result.message = Resources.ClientSuccessRemove;

            return result;
        }
        #endregion
    }
}