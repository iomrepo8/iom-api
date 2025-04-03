using IOM.Helpers;
using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Services;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using IOM.Services.Interface;

namespace IOM.Controllers.WebApi
{
    [RoutePrefix("user")]
    [System.Web.Http.Authorize]
    public class UserController : ApiController
    {
        private readonly IAzureStorageServices _azureStorageServices;
        private readonly IRepositoryService _repositoryService;

        public UserController(IAzureStorageServices azureStorageServices, IRepositoryService repositoryService)
        {
            _azureStorageServices = azureStorageServices;
            _repositoryService = repositoryService;
        }
        
        [HttpPost]
        [Route("list")]
        public ApiResult UserList(UsersDataRequestModal model)
        {
            var result = new ApiResult();

            var userList = _repositoryService.UserList(model.UserIds, model.Roles, model.AccountIds,
                model.TeamIds, model.Tags, User.Identity.Name, model.ShowInactive);


            result.data = new
            {
                UserList = userList,
                UserCount = userList.Count,
                AccountCount = userList.Where(d => d.AccountId.HasValue && d.AccountId != 0).IOMDistinctBy(e => e.AccountId).Count(),
                TeamCount = userList.Where(d => d.TeamId.HasValue && d.TeamId != 0).IOMDistinctBy(e => e.TeamId).Count()
            };

            return result;
        }
        
        [HttpGet]
        [System.Web.Http.Route("info")]
        public ApiResult CurrentUserInfo()
        {
            var result = new ApiResult();
            var userService = _repositoryService;

            userService.UpdateActiveTime(User.Identity.Name);
            result.data = userService.GetCurrentUserInfo(User.Identity.Name);

            return result;
        }

        [HttpGet]
        [Route("UserNotifications")]
        public async Task<ApiResult> UserNotifications()
        {
            var result = new ApiResult();
            var userService = _repositoryService;

            result.data = await userService.GetUserNotifications(User.Identity.Name).ConfigureAwait(false);

            return result;
        }

        [HttpGet]
        [Route("details")]
        public ApiResult GetUserDetails([FromUri] int userDetailsId)
        {
            var result = new ApiResult
            {
                data = _repositoryService.GetUserDetails(userDetailsId, User.Identity.Name)
            };

            return result;
        }

        [HttpGet]
        [Route("taskstatus")]
        public ApiResult UserStatus([FromUri] int userDetailId)
        {
            ApiResult result = new ApiResult();

            var username = _repositoryService.GetUserName(userDetailId);
            result.data = _repositoryService.GetCurrentUserInfo(username);

            return result;
        }

        [HttpGet]
        [Route("assigned")]
        public ApiResult UsersLookup()
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.GetAssignedUsers(User.Identity.Name);

            return result;
        }

        [HttpGet]
        [System.Web.Http.Route("task_assignees")]
        public ApiResult GetTaskAssignees(int taskId)
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.TaskAssignees(taskId, User.Identity.Name);

            return result;
        }

        [HttpGet]
        [Route("userslookup")]
        public ApiResult UsersLookupv2()
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.GetUsersLookupv2(User.Identity.Name);

            return result;
        }

        [HttpGet]
        [Route("by_teams")]
        public ApiResult UsersLookupByTeams([FromUri] int[] teamIds, [FromUri] int[] accountIds)
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.GetTaskClockers(teamIds, accountIds, User.Identity.Name);

            return result;
        }

        [HttpGet]
        [Route("by_accounts")]
        public ApiResult UsersLookupByAccounts([FromUri] int[] accountIds, [FromUri] bool includeInactive)
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.GetAGTSByAccounts(accountIds, includeInactive, User.Identity.Name);

            return result;
        }

        [HttpGet, System.Web.Http.Route("emp-status")]
        public ApiResult EmployeeStatusLookup([FromUri] string q = "")
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.GetEmployeeStatuses(q);

            return result;
        }

        [HttpGet]
        [Route("emp-shift")]
        public ApiResult EmployeeShiftLookup([FromUri] string q = "")
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.GetEmployeeShifts(q);

            return result;
        }

        [HttpGet]
        [Route("emp-week-schedule")]
        public ApiResult EmployeeWeekScheduleLookup([FromUri] string q = "")
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.GetEmployeeWeekSchedule(q);

            return result;
        }

        [HttpPost]
        [Route("change_status")]
        public ApiResult ChangeStatus(StatusBindingModel statusBindingModel)
        {
            var result = new ApiResult();

            _repositoryService.SetStatus(statusBindingModel?.TaskId,
                                            statusBindingModel.TaskTypeId,
                                            statusBindingModel.UserDetailId);
            result.message = Resources.UserSuccessStatusChange;

            return result;
        }

        [HttpPost]
        [Route("change_account")]
        public ApiResult ChangeAccount(ChangeAccountModel model)
        {
            ApiResult result = new ApiResult();

            _repositoryService.ChangeAgentAccount(model);
            result.message = Resources.UserSuccessAccountChange;

            return result;
        }

        [HttpPost]
        [Route("change_teams")]
        public ApiResult ChangeTeams(ChangeAccountModel model)
        {
            ApiResult result = new ApiResult();

            _repositoryService.ChangeAgentTeams(model);
            result.message = Resources.UserSuccessTeamsChange;

            return result;
        }

        [HttpPost]
        [Route("save_detail")]
        public ApiResult SaveUser(UserDetailModel userDetailModel)
        {
            ApiResult result = new ApiResult();

            _repositoryService.SaveUser(userDetailModel, User.Identity.Name);
            result.message = Resources.UserSuccessUpdateDetail;

            return result;
        }

        [HttpPost]
        [Route("save_basic_info")]
        public ApiResult SaveBasicInfo(UserModel userModel)
        {
            ApiResult result = new ApiResult();

            _repositoryService.SaveBasicInfo(userModel);

            result.data = _repositoryService.GetUserDetails(userModel.UserDetailsId, User.Identity.Name);
            return result;
        }

        [HttpPost]
        [Route("save_emp_details")]
        public ApiResult SaveEmpDetails(UserDetailModel userModel)
        {
            ApiResult result = new ApiResult();

            _repositoryService.SaveEmpDetails(userModel);

            result.data = _repositoryService.GetUserDetails(userModel.UserDetailsId, User.Identity.Name);
            result.message = Resources.UserSuccessUpdateDetail;

            return result;
        }

        [HttpPost]
        [Route("delete")]
        public async Task<ApiResult> Deleteuser([FromUri] int userId)
        {
            var result = new ApiResult();

            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

            await _repositoryService.DeleteUserAsync(userId, userManager, User.Identity).ConfigureAwait(false);

            result.message = Resources.UserSuccessDelete;

            return result;
        }

        [HttpPost]
        [Route("add")]
        public async Task<ApiResult> CreateUser(UserDetailModel userDetailModel, CancellationToken cancellationToken)
        {
            var result = new ApiResult();
            try
            {
                var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

                await _repositoryService.CreateUserAsync(userDetailModel, userManager, User.Identity, cancellationToken).ConfigureAwait(false);

                result.message = Resources.UserSuccessCreate;
            }
            catch (Exception e)
            {
                var d = e;
            }

            return result;
        }

        [HttpPost]
        [Route("lock")]
        public ApiResult LockUser([FromUri] int userId)
        {
            ApiResult result = new ApiResult();

            var isLocked = _repositoryService.LockUser(User.Identity, userId);
            result.message = isLocked ? Resources.UserSuccessLock : Resources.UserSuccessUnlock;

            return result;
        }

        [HttpPost]
        [Route("send_reset_pass")]
        public async Task<ApiResult> SendResetPass([FromUri] string email)
        {
            ApiResult result = new ApiResult();

            ApplicationUserManager usermanager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var host = Request.Headers.Referrer;

            await _repositoryService.SendResetPass(email, usermanager, host).ConfigureAwait(false);

            result.message = Resources.PasswordResetEmailSent;

            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("forgot_password")]
        public async Task<ApiResult> ForgotPassword(ResetPasswordModel model)
        {
            ApiResult result = new ApiResult();

            if (model == null) throw new ArgumentNullException(nameof(model));

            ApplicationUserManager usermanager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var host = Request.Headers.Referrer;

            await _repositoryService.SendResetPass(model.Email, usermanager, host).ConfigureAwait(false);
            result.message = Resources.PasswordResetForgot;

            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("reset_pass")]
        public async Task<ApiResult> ResetPassword(ResetPasswordModel resetPassword)
        {
            ApiResult result = new ApiResult();

            ApplicationUserManager usermanager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            await _repositoryService.ResetPassword(resetPassword, usermanager);
            result.message = Resources.PasswordResetSuccess;

            return result; 
        }

        [HttpPost]
        [Route("change_password")]
        public async Task<ApiResult> ChangePassword(ChangePasswordModel cpModel)
        {
            ApiResult result = new ApiResult();

            ApplicationUserManager usermanager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

            await _repositoryService.ChangePassword(cpModel, usermanager);
            result.message = Resources.PasswordResetSuccess;

            return result;
        }

        [HttpGet]
        [Route("userpermission")]
        public ApiResult GetUserPermission([FromUri] int userid = 0)
        {
            ApiResult result = new ApiResult();

            result.data = _repositoryService.GetUserRolePermission(userid);

            return result;
        }

        [HttpPost]
        [Route("save_permission")]
        public ApiResult SaveUserPermission(UserPermissionModel userPermissions)
        {
            ApiResult result = new ApiResult();

            _repositoryService.SavePermissions(userPermissions);

            return result;
        }

        [HttpPost]
        [Route("save_profile_picture")]
        public async Task<ApiResult> SaveProfilePicture([FromUri] int userDetailsId, CancellationToken cancellationToken)
        {
            var result = new ApiResult();

            if (userDetailsId < 1) throw new ArgumentNullException(nameof(userDetailsId));

            var iUploadedCnt = 0;
            var imageUri = "";
            
            var request = Request; 
            if (!request.Content.IsMimeMultipartContent()) 
            { 
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType); 
            } 
            
            var provider = new RestrictiveMultipartMemoryStreamProvider();

            //READ CONTENTS OF REQUEST TO MEMORY WITHOUT FLUSHING TO DISK
            await Request.Content.ReadAsMultipartAsync(provider, cancellationToken).ConfigureAwait(false);

            foreach (var content in provider.Contents)
            {
                //now read individual part into STREAM
                var stream = await content.ReadAsStreamAsync().ConfigureAwait(false);
                var contentType = content.Headers.ContentType;
                
                if (stream.Length != 0)
                {
                    imageUri = _azureStorageServices.StoreProfileImage(stream, contentType.ToString());
                    _repositoryService.SaveImageProperty(userDetailsId, imageUri, User.Identity.Name);
                    iUploadedCnt++;
                }
            }
            
            result.data = new
            {
                fileCount = iUploadedCnt,
                filename = imageUri
            };

            result.message = iUploadedCnt > 0 ?
                Resources.ProfilePictureSuccessUpload : Resources.ProfilePictureFailUpload;

            return result;
        }
    }
}
