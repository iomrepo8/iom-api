using System;
using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class UserBasicModel
    {
        public int UserDetailsId { get; set; }
        public string StaffId { get; set; }
        public string NetUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string UserTimezone { get; set; }
        public int? TimeZoneId { get; set; }
        public string IpAddress { get; set; }
    }

    public class UserModel : UserBasicModel    
    {
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public bool? IsLocked { get; set; }
        public bool? IsDeleted { get; set; }
        public bool IsNameMasked { get; set; }
        public string ShiftName { get; set; }
        public string Image { get; set; }
        public int? Seat { get; set; }
        public bool IsUnrestrictedIp { get; set; }

        
        public DateTime Created { get; set; }
        public IList<BaseModel> Tags { get; set; } = new List<BaseModel>();
    }

    public class UserInfoModel : UserModel
    {
        public bool IsOut { get; set; }
        public int? TaskTypeId { get; set; }
        public string Status { get; set; }
        public int? TaskId { get; set; }
        public int TaskHistoryId { get; set; }
        public string CurrentTaskComment { get; set; }
        public string TaskName { get; set; }
        public decimal? ActiveTimeOnTask { get; set; }
        public IList<PermissionsModel> Permissions { get; set; }
	    public DateTime? StatusUpdate { get; set; }

        public IList<NotificationModel> Notifications { get; set; }

        public IList<UserNotificationSettingResult> EmailNotificationSettings { get; set; }
    }

    public class UserListModel : UserModel
    {
        public int IsOnline { get; set; }
        public int? AccountId { get; set; }
        public string AccountName { get; set; }
        public int? TeamId { get; set; }
        public string TeamName { get; set; }
        public IList<BaseModel> Accounts { get; set; } = new List<BaseModel>();
        public IList<BaseModel> Teams { get; set; } = new List<BaseModel>();
        public IList<object> Seats { get; set; } = new List<Object>();
    }

    public class UserDetailModel : UserModel
    {
        public int? AccountId { get; set; }
        public int? EmployeeStatusId { get; set; }
        public string EmployeeStatus { get; set; }
        public decimal? HourlyRate { get; set; }
        public int? EmployeeShiftId { get; set; }
        public string Password { get; set; }
        public TimeSpan ShiftStartTime { get; set; }
        public TimeSpan ShiftEndTime { get; set; }
        public string Timezone { get; set; }
        public bool IsOnline { get; set; }
        public IList<TaskModel> Tasks { get; set; }
        public IList<ProfileAccountModel> Accounts { get; set; } = new List<ProfileAccountModel>();
        public IList<BaseModel> TaskGroups { get; set; } = new List<BaseModel>();
        public IList<DayOff> WeekSchedule { get; set; } = new List<DayOff>();
    }

    public class TeamUsersModel: UserModel
    {
        public IList<BaseModel> Teams { get; set; } = new List<BaseModel>();
        public int TeamId { get; set; }
        public string TeamName { get; set; }
    }
}
