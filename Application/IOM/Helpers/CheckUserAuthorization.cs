using System;

namespace IOM.Helpers
{
    public class CheckUserAuthorization
    {
        public bool IsGranted { get; set; }
        public string message { get; set; }        
    }

    public static class UnAuthorizedMessage
    {
        static string msg = "You do not have permission to {0}. Please contact your system administrator.";

        public static string Edit = String.Format(msg, "edit");

        public static string Add = String.Format(msg, "add");

        public static string View = String.Format(msg, "view"); 

        public static string Delete = String.Format(msg, "delete");
    }

    public class UserBaseModel
    {
        public string UserId { get; set; }
        public string BaseId { get; set; }
    }
}