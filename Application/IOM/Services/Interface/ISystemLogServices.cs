using System.Collections.Generic;
using IOM.DbContext;
using IOM.Models.ApiControllerModels;

namespace IOM.Services.Interface
{
    public partial interface IRepositoryService
    {
        IList<SystemLogDataModel> GetSystemLogData(SysLogRequestDataModel model);
        object GetEntities(string query);
        void SaveToDb(SystemLog systemLog);
    }
}