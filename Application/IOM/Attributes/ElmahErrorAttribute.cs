using IOM.Providers;
using System;
using System.Web.Http.Filters;

namespace IOM.Attributes
{
    internal sealed class ElmahErrorAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(
                    new Exception(ErrorMailModuleProvider.PreEmailContent(
                        actionExecutedContext.Exception,
                        actionExecutedContext.Request)));
            }

            base.OnException(actionExecutedContext);
        }
    }
}