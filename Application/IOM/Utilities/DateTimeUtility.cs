using System;

namespace IOM.Utilities
{
    public enum SysEnvironment
    {
        Production,
        Development,
        Staging
    }

    public class DateTimeUtility
    {
        private readonly TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        private static DateTimeUtility _instance;
        private static readonly object _lock = new object();

        private DateTimeUtility() { }

        public static DateTimeUtility Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new DateTimeUtility();
                    }

                    return _instance;
                }
            }
        }

        public DateTime DateTimeNow()
        {
            var timeUtc = DateTime.UtcNow;

            return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, estZone);
        }
    }
}