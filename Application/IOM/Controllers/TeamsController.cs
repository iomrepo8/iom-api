using IOM.Models.ApiControllerModels;
using IOM.Properties;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using IOM.Services.Interface;

namespace IOM.Controllers.WebApi
{
    [RoutePrefix("teams")]
    [Authorize]
    public class TeamsController : ApiController
    {
        private readonly IRepositoryService _repositoryService;
        public TeamsController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        
        [HttpPost]
        [Route("list")]
        public ApiResult TeamsList(TeamDataRequestModel model)
        {
            var result = new ApiResult
            {
                data = _repositoryService
                    .GetTeamsDashboardData(model.AccountIds, model.TeamIds, User.Identity.Name, model.ShowInactive)
            };

            return result;
        }

        [HttpGet]
        [Route("assigned")]
        public ApiResult TeamsLookup()
        {
            var result = new ApiResult
            {
                data = _repositoryService.TeamsLookup(User.Identity.Name)
            };

            return result;
        }

        [HttpGet]
        [Route("by_account_id")]
        public ApiResult TeamsLookup([FromUri] int accountId = 0, [FromUri] string q = "")
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetTeamsByAccountId(accountId, q)
            };

            return result;
        }

        [HttpGet]
        [Route("by_accounts")]
        public ApiResult TeamsLookupByAccounts([FromUri] int[] accountIds)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetTeamsByAccounts(accountIds, User.Identity.Name)
            };

            return result;
        }

        [HttpGet]
        [Route("get")]
        public ApiResult GetTeam([FromUri] int teamId)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetTeam(teamId)
            };

            return result;
        }

        [HttpGet]
        [Route("details")]
        public ApiResult GetTeamDetails([FromUri] int teamId)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetTeamDetails(teamId, User.Identity.Name)
            };

            return result;
        }

        [HttpGet]
        [Route("is_deletable")]
        public ApiResult IsDeletable([FromUri] int teamId)
        {
            var result = new ApiResult
            {
                //result.data = new { IsDeletable = TeamServices.Instance.IsDeletable(teamId) };
                data = new { IsDeletable = true }
            };

            return result;
        }

        [HttpPost]
        [Route("delete")]
        public ApiResult DeleteTeam([FromBody] int teamId)
        {
            var result = new ApiResult();

            _repositoryService.DeleteTeam(teamId);
            result.message = Resources.TeamSuccessDelete;

            return result;
        }

        [HttpPost]
        [Route("save")]
        public ApiResult SaveTeam(TeamModel teamModel)
        {
            var result = new ApiResult();

            _repositoryService.SaveTeam(teamModel);
            result.message = teamModel.Id > 0 ? Resources.TeamSuccessUpdate : Resources.TeamSuccessAdd;

            return result;
        }

        [HttpPost]
        [Route("deactivate")]
        public ApiResult DeactivateTeam([FromBody] int teamId)
        {
            var result = new ApiResult();

            _repositoryService.DeactivateTeam(teamId);
            result.message = Resources.TeamSuccessDeactivate;

            return result;
        }

        [HttpPost]
        [Route("activate")]
        public ApiResult ActivateTeam([FromBody] int teamId)
        {
            var result = new ApiResult();

            _repositoryService.ActivateTeam(teamId);
            result.message = Resources.TeamSuccessActivate;

            return result;
        }

        #region Lead Agent
        [HttpPost]
        [Route("remove_lead_agent")]
        public ApiResult RemoveLeadAgent(TeamMemberModel teamModel)
        {
            var result = new ApiResult();

            _repositoryService.RemoveLeadAgent(teamModel);
            result.message = Resources.LASuccessRemove;

            return result;
        }

        [HttpPost]
        [Route("add_lead_agent")]
        public async Task<ApiResult> AddLeadAgent(TeamMemberModel teamModel)
        {
            var result = new ApiResult();

            await _repositoryService.AddLeadAgentAsync(teamModel);
            result.message = Resources.LASuccessAdd;

            return result;
        }

        [HttpGet]
        [Route("lead_agents")]
        public ApiResult LeadAgentsLookup([FromUri] int teamId)
        {
            var result = new ApiResult
            {
                data = _repositoryService.LeadAgents(teamId)
            };

            return result;
        }
        #endregion

        #region Team Managers
        [HttpPost]
        [Route("add_manager")]
        public async Task<ApiResult> AddManager(TeamMemberModel manager)
        {
            var result = new ApiResult();

            if (manager is null) throw new ArgumentNullException(nameof(manager));

            await _repositoryService.AddManagerAsync(manager).ConfigureAwait(false);
            result.message = Resources.TeamManagerSuccessAdd;

            return result;
        }

        [HttpPost]
        [Route("remove_manager")]
        public ApiResult RemoveManager(TeamMemberModel manager)
        {
            var result = new ApiResult();

            if (manager is null) throw new ArgumentNullException(nameof(manager));

            _repositoryService.RemoveManager(manager);
            result.message = Resources.TeamManagerSuccessRemove;

            return result;
        }

        [HttpGet]
        [Route("managers")]
        public ApiResult GetManagers([FromUri] int teamId)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetManagers(teamId)
            };

            return result;
        }
        #endregion

        #region Agent
        [HttpGet]
        [Route("agents")]
        public ApiResult AgentsLookup([FromUri] int teamId, [FromUri] string q = "")
        {
            var result = new ApiResult
            {
                data = _repositoryService.AgentsLookup(teamId, q)
            };

            return result;
        }

        [HttpPost]
        [Route("add_agent")]
        public async Task<ApiResult> AddAgent(TeamMemberModel agentEDModel)
        {
            var result = new ApiResult();

            if (agentEDModel is null) throw new ArgumentNullException(nameof(agentEDModel));

            await _repositoryService.AddAgentAsync(agentEDModel).ConfigureAwait(false);
            result.message = Resources.AgentSuccessAdd;

            return result;
        }

        [HttpPost]
        [Route("remove_agent")]
        public ApiResult RemoveAgent(TeamMemberModel teamModel)
        {
            var result = new ApiResult();

            _repositoryService.RemoveAgent(teamModel);
            result.message = Resources.AgentSuccessRemove;

            return result;
        }
        #endregion

        #region Tasks
        [HttpPost]
        [Route("assign_task")]
        public async Task<ApiResult> AssignTask(TeamTaskBase addTeamTask)
        {
            var result = new ApiResult();

            if (addTeamTask == null) throw new ArgumentNullException(nameof(addTeamTask));

            await _repositoryService.AssignTask(addTeamTask, User.Identity.Name).ConfigureAwait(false);
            result.message = Resources.TaskSuccessAssign;

            return result;
        }

        [HttpPost]
        [Route("remove_task")]
        public async Task<ApiResult> RemoveTask(TeamTaskBase addTeamTask)
        {
            var result = new ApiResult();

            if (addTeamTask == null) throw new ArgumentNullException(nameof(addTeamTask));

            await _repositoryService.RemoveTask(addTeamTask.Id, User.Identity.Name).ConfigureAwait(false);
            result.message = Resources.TaskSuccessRemove;

            return result;
        }
        #endregion

        #region Clients
        [HttpPost]
        [Route("add_client")]
        public async Task<ApiResult> AddClient(TeamMemberModel teamCPModel)
        {
            var result = new ApiResult();

            await _repositoryService.AddClientAsync(teamCPModel).ConfigureAwait(false);
            result.message = Resources.ClientSuccessAdd;

            return result;
        }

        [HttpPost]
        [Route("remove_client")]
        public ApiResult RemoveClient(TeamMemberModel teamModel)
        {
            var result = new ApiResult();

            _repositoryService.RemoveClient(teamModel);
            result.message = Resources.ClientSuccessRemove;

            return result;
        }

        [HttpGet]
        [Route("clients")]
        public ApiResult GetClients([FromUri] int teamId)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetClients(teamId)
            };

            return result;
        }
        #endregion

        [HttpPost]
        [Route("update_dayoff_holidays")]
        public ApiResult UpdateDayoffHoliday(DayOffHolidayModel dayOffHoliday)
        {
            var result = new ApiResult();

            _repositoryService.UpdateDayoffHoliday(dayOffHoliday);
            result.message = Resources.TeamUpdateDOHSuccess;

            return result;
        }

    }
}