using System;
using System.Collections.Generic;
using IOM.Hubs.Models;
using IOM.Models.ApiControllerModels;
using IOM.Utilities;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        IList<TimekeepingReport> ReportGridData(int[] accountIds, int[] teamIds, 
            int[] userIds, int[] tags, string[] roles, string startDate, string endDate, string username, bool hasLogHoursOnly);

        void CollectTimeLog();

        IList<TimekeepingManagement> ManagementData(string[] roles, int[] accountIds, int[] teamIds, int[] tagIds,
            int[] userIds, string startDate, string endDate, string username, bool includeInactive, bool hasLiveHoursOnly,
            EmployeeStatusFilter statusfilter);

        object GetStatusViewData(string fromDate, string toDate, string userId, int[] userIds, string[] roles);
        object GetAttendanceStatusOptions();
        void SetAttendanceStatusOptions(string mode, string statusname, int statusid);
        void SetAttendanceStatus(string datestatus, string oldVal, string newVal, int statusOwner, string userId);
        void SaveAttendanceData(IList<AttendanceDay> attendanceData, string username);

        void SaveIndividualAttendanceData(AttendanceIndividual attendanceIndividual, string username);
        void SaveAttendanceRow(AttendanceRowModel attendanceRowModel, string username);
        object WeekHours(int userId, string startDate, string endDate);
        decimal GetTodayHourCount(string username);
        void UpdateUsersActiveHours();
        void AutoOutThreeAMUTC();
        object GetTkManagementData(TkManagementRequest request, string netUserId);

        double CalculateWorkedHours(DateTime? start, DateTime? end, double mgtEdit);

        void UpdateWorkedHours(DateTime? attendanceDate, int userDetailsId, string editorId);

        object AddAttendanceOTAfterUpdate(DateTime? attendanceDate, int userDetailsId, decimal workedHrs, string editorId);
    }
}