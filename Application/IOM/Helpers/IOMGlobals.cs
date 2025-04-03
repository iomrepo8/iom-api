namespace IOM.Helpers
{
    public static class Globals
    {
        public const string AGENT_RC = "AG";
        public const string LEAD_AGENT_RC = "LA";
        public const string TEAM_MANAGER_RC = "TM";
        public const string ACCOUNT_MANAGER_RC = "AM";
        public const string CLIENT_RC = "CL";
        public const string SYSAD_RC = "SA";
    }

    public static class UserTimeZones
    {
        public const string EST = "Eastern Standard Time";
        //No Philippine Standard Time value in SQL timezone info use "TST" instead.
        public const string TST = "Taipei Standard Time";
    }
}