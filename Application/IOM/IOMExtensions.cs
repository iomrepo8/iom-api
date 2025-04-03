using System;
using System.Globalization;

namespace IOM
{
    public static class IOMExtensions
    {
        private static DateTime epoch
          = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static string ToUserDisplay(this DateTime dt, string defaultVal, string format)
        {
            return dt == null ? defaultVal : dt.ToString(format == "dt" ? "yyyy-MM-dd HH:mm:ss" : format, CultureInfo.InvariantCulture);
        }

        public static string ToUserDisplay(this DateTime? dt, string defaultVal = "", string format = "")
        {
            return dt == null ? defaultVal : dt.Value.ToUserDisplay(defaultVal = "", format);
        }

        public static string ToUserDisplay(this DateTime dt, string defaultVal = "")
        {
            return dt == null ? defaultVal : dt.ToUserDisplay(defaultVal, "yyyy-MM-dd");
        }

        public static DateTime ToUniversalDateTime(this int timestamp)
        {
            return epoch.AddSeconds(timestamp).ToUniversalTime();
        }

    }
}