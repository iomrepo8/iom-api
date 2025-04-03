namespace IOM.Helpers
{
    internal class AppConfigurations
    {
        internal static string ChronoDetailRoute
        {
            get
            {
                return @"/UserAttendance/ChronoDetail?token={0}&userId={1}&date={2}";
            }
        }
    }
}