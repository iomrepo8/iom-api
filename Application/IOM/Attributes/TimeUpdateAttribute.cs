using IOM.Services;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using IOM.Services.Interface;

namespace IOM.Attributes
{
    public class TimeUpdateAttribute : ActionFilterAttribute
    {
        private readonly IRepositoryService _repositoryService;
        public TimeUpdateAttribute(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            _repositoryService.UpdateUsersActiveHours();

            base.OnActionExecuting(actionContext);
        }
    }
}