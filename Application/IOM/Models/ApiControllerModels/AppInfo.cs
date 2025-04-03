using System;

namespace IOM.Models.ApiControllerModels
{
    public class AppInfo
    {
        public string AppName { get; set; }
        public string AppEnvironment { get; set; }
        public string BuildVersion { get; set; }
        public DateTime SystemDate { get; set; }
    }
}