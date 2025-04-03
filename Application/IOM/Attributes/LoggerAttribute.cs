using IOM.DbContext;
using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Services;
using IOM.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using IOM.Services.Interface;

namespace IOM.Attributes
{
    public sealed class LoggerAttribute : ActionFilterAttribute
    {
        public HttpRequestMessage Request { get; private set; }
        private Stopwatch stopwatch;
        private SystemLog systemLog;

        private IList<string> exceptedLog = new List<string>
        {
            EODAction.ApproveEOD.ToString().ToLower(CultureInfo.CurrentCulture),
            EODAction.DenyEOD.ToString().ToLower(CultureInfo.CurrentCulture),
            EODAction.EditEOD.ToString().ToLower(CultureInfo.CurrentCulture)
        };

        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            if (actionContext.Request.RequestUri.AbsolutePath.Contains("/time/"))
            {
                return;
            }

            if (actionContext.Request.Method == HttpMethod.Post)
            {
                stopwatch = Stopwatch.StartNew();

                using (var stream = new MemoryStream())
                {
                    var context = (HttpContextBase)actionContext.Request.Properties["MS_HttpContext"];
                    context.Request.InputStream.Seek(0, SeekOrigin.Begin);
                    context.Request.InputStream.CopyTo(stream);
                    var requestBody = Encoding.UTF8.GetString(stream.ToArray());

                    var actionType = HandleActionType(
                            GetSegment(actionContext.Request.RequestUri.AbsolutePath, UriSegmentType.Entity),
                            GetSegment(actionContext.Request.RequestUri.AbsolutePath, UriSegmentType.Action),
                            requestBody, actionContext.Request.RequestUri.Query);

                    
                    var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

                    var userDetailsId = GetUserDetailsId(principal?.Identity?.Name);
                    var userId = 0;
                    var description = "";

                    userId = userDetailsId;

                    if (actionContext.Request.RequestUri.AbsolutePath.Contains("save_profile_picture"))
                    {
                        requestBody = "";
                    }

                    systemLog = new SystemLog
                    {
                        LogDate = DateTime.UtcNow,
                        ActorUserId = userId,
                        RawUrl = actionContext.Request.RequestUri.AbsoluteUri,
                        ActionType = actionType,
                        Description = description,
                        Entity = GetSegment(actionContext.Request.RequestUri.AbsolutePath, UriSegmentType.Entity),
                        BrowserUsed = (context.Request.Browser.Browser + " Version: " 
                            + context.Request.Browser.Version + " " + context.Request.Browser.Type),
                        IPAddress = GetClientIp(actionContext.Request),
                        RequestBody = requestBody,
                        UrlParams = actionContext.Request.RequestUri.Query
                    };
                }
            }

            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Request.RequestUri.AbsolutePath.Contains("/time/"))
            {
                return;
            }

            if (actionExecutedContext.ActionContext.Request.Method == HttpMethod.Post 
                && actionExecutedContext.Exception == null)
            {
                var response = GetResponseContent(actionExecutedContext.Response.Content);

                if (response != null && response.isSuccessful)
                {
                    var elapsedTime = stopwatch.ElapsedMilliseconds;
                    stopwatch.Stop();

                    systemLog.ResponseStatusCode = (int)actionExecutedContext.ActionContext.Response.StatusCode;
                    systemLog.ElapseTime = elapsedTime;

                    if (systemLog.Entity != "time" &&
                        !exceptedLog.Contains(systemLog.ActionType.ToLower(CultureInfo.CurrentCulture)))
                    {
                        var task = System.Threading.Tasks.Task.Run(() => {
                            //SystemLogServices.Instance.SaveToDb(systemLog);
                        });
                        task.Wait();
                    }
                }                
            }

            base.OnActionExecuted(actionExecutedContext);
        }

        private ApiResult GetResponseContent(HttpContent content)
        {
            ApiResult responseContent = null;
            try
            {
                var jsonContent = content.ReadAsStringAsync().Result;
                responseContent = JsonConvert.DeserializeObject<ApiResult>(jsonContent);
            }
            catch
            {
                // ignored
            }

            return responseContent;
        }

        private string HandleActionType(string entity, string initialActionType, string requestBody = "", 
            string urlParams = "")
        {
            var actionType = "";

            var logEntity = Enum.Parse(typeof(LogEntity), entity.ToLower());

            switch (logEntity)
            {
                case LogEntity.task:
                    switch (initialActionType)
                    {
                        case "save":
                            var task = JsonConvert.DeserializeObject<TaskModel>(requestBody);

                            if (task.Id > 0)
                            {
                                actionType = "edit";
                            }
                            else
                            {
                                actionType = "add";
                            }
                            break;
                        case "activate":
                            urlParams = urlParams.Replace("?", "");
                            var valArray = urlParams.Split('&')[0].Split('=')[1];

                            switch (valArray[0])
                            {
                                case '1':
                                    actionType = "activate";
                                    break;
                                case '2':
                                    actionType = "meeting";
                                    break;
                                case '3':
                                    actionType = "Lunch";
                                    break;
                                case '4':
                                    actionType = "break";
                                    break;
                                case '5':
                                    actionType = "break";
                                    break;
                                case '6':
                                    actionType = "bio_break";
                                    break;
                                case '7':
                                    actionType = "out";
                                    break;
                                case '9':
                                    actionType = "avail";
                                    break;
                            }
                            break;

                        case "delete":
                            actionType = "delete";
                            break;
                        case "update_single_usertotask":
                            actionType = "update";
                            break;
                        default:
                            actionType = initialActionType;
                            break;
                    }
                    break;
                case LogEntity.teams:
                    if (initialActionType == "save")
                    {
                        var team = JsonConvert.DeserializeObject<TeamModel>(requestBody);

                        if (team.Id > 0)
                        {
                            actionType = "edit";
                        }
                        else
                        {
                            actionType = "add";
                        }
                    }
                    else
                    {
                        actionType = initialActionType;
                    }
                    break;
                case LogEntity.accounts:
                    if (initialActionType == "save")
                    {
                        var account = JsonConvert.DeserializeObject<AccountDataModel>(requestBody);

                        if (account.Id > 0)
                        {
                            actionType = "edit";
                        }
                        else
                        {
                            actionType = "add";
                        }
                    }
                    else
                    {
                        actionType = initialActionType;
                    }
                    break;
                case LogEntity.user:
                case LogEntity.tkdata:
                case LogEntity.role:
                case LogEntity.notification:
                case LogEntity.taskgroup:
                case LogEntity.attendance:
                case LogEntity.settings:
                    actionType = initialActionType;
                    break;
            }

            return actionType;
        }

        private string GetClientIp(HttpRequestMessage request = null)
        {
            request = request ?? Request;

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }

        private string GetSegment(string absolutePath, UriSegmentType type)
        {
            absolutePath = absolutePath.Trim('/');

            var segment = "";

            switch (type)
            {
                case UriSegmentType.Action:
                    segment = absolutePath.Split('/')[1];
                    break;
                case UriSegmentType.Entity:
                    segment = absolutePath.Split('/')[0];
                    break;
            }

            return segment;
        }

        private int GetUserDetailsId(string userName)
        {
            return 1;
            // using (var ctx = Entities.Create())
            // {
            //     return (from ud in ctx.UserDetails
            //         join au in ctx.AspNetUsers on ud.UserId equals au.Id
            //         where au.UserName == userName
            //         select new
            //         {
            //             UserDetailsId = ud.Id,
            //             NetUserId = au.Id,
            //             FirstName = ud.FirstName,
            //             LastName = ud.LastName,
            //             Email = au.Email,
            //             RoleCode = ud.Role,
            //             FullName = ud.Name,
            //             Username = au.UserName
            //         }).SingleOrDefault();
            // }
        }
    }
}