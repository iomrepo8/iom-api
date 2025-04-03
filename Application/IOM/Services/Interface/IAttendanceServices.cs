using System;
using System.Collections.Generic;
using IOM.Models.ApiControllerModels;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        void SaveAttendanceOTUpdate(OvertimeData attendanceData, string username);
        object AttendanceOT(int[] userIds, int[] accountIds, int[] teamIds, string[] roles, string startDate, string endDate, string username);
        object GetAttendanceStatusUpdates(string startDate, string endDate, string userId);
        IList<AttendanceDefaultView> AttendanceDefault(int[] userIds, int[] accountIds, int[] teamIds, string[] roles,
            string startDate, string endDate , string username, string[] tags, bool hasLiveHoursOnly);
        IList<TaskChronoItem> GetChronoData(int userId, string date);
        bool IsWeekend(DateTime date);

        object GetAttendanceRows(int AttendanceId);
    }
}