using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using IOM.Models;
using IOM.Models.ApiControllerModels;
using SendGrid.Helpers.Mail;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        UserInfoModel GetCurrentUserInfo(string username);
        string[] GetAccessedRolesByRole(string role);
        Task<IList<NotificationModel>> GetUserNotifications(string username);
        object GetTaskClockers(int[] teamIds, int[] accountIds, string username);
        object GetAGTSByAccounts(int[] accountIds, bool includeInactive, string username);
        UserInfoModel GetUserInfoById(string id);
        string GetUserName(int userDetailId);
        IList<UserLookUpModel> TaskAssignees(int taskId, string username);
        IList<UserLookUpModel> GetAssignedUsers(string username, bool includeInactive = false, string query = "");
        IList<UsersRawModel> GetAssignedUsersRaw(string username, bool includeInActive = true, string query = "");
        IList<UserLookUpModelv2> GetUsersLookupv2(string username, bool includeInActive = true, string query = "");
        int GetOnlineUserCount(string username);
        void SetUserOnline(string username);
        void SetUserOffline(string username);
        IList<UserListModel> UserList(int[] userIds, string[] roles, int[] accountIds, int[] teamIds, string[] tags, string username, bool showInactive = false);
        IList<UserListModel> UserList(int userDetailsId, bool showInactive = false);
        int GetUsersCount(string username);
        void SaveImageProperty(int userDetailsId, string imageUri, string username);
        void SavePermissions(UserPermissionModel userPermissions);
        UserDetailModel GetUserDetails(int userDetailsId, string username);
        UserDetailModel GetUserDetails(int userDetailId);
        IList<LookUpModel> GetEmployeeStatuses(string query);
        IList<LookUpModel> GetEmployeeShifts(string query);
        IList<LookUpModel> GetEmployeeWeekSchedule(string query);
        void SaveBasicInfo(UserModel userModel);
        void SaveUser(UserDetailModel userDetailModel, string username);
        void SaveEmpDetails(UserDetailModel userModel);
        Task DeleteUserAsync(int userId, ApplicationUserManager userManager, IIdentity identity);
        Task ChangePassword(ChangePasswordModel cpModel, ApplicationUserManager usermanager);
        Task CreateUserAsync(UserDetailModel userDetailModel, ApplicationUserManager userManager, IIdentity currentUser, CancellationToken cancellationToken);
        Task ResetPassword(ResetPasswordModel resetPassword, ApplicationUserManager usermanager);
        Task SendResetPass(string email, ApplicationUserManager usermanager, Uri uri);
        bool LockUser(IIdentity identity, int? userId);
        void UpdateActiveTime(string username);
        IList<PermissionsModel> GetUserPermission(int userId);
        NetRolePermission GetUserRolePermission(int userId);
        List<EmailAddress> GetAdminEmailRecipients(NotificationType notificationType, out List<int> adminUds);
        List<UserBasicModel> GetAllSystemAdmins();
        Task UpdateUsedIpAddressAsync(int userDetailsId, string ipAddress, CancellationToken cancellationToken);
    }
}