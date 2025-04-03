using System;
using System.Linq;

namespace IOM.Utilities
{
    public static class ServiceUtility
    {
        private static Random random = new Random();

        public static bool NullCheck(object obj, string message = "")
        {
            if(obj == null)
            {
                message = message.Length > 0 ? "Data not found" : message;
                throw new ArgumentNullException(message);
            }

            return true;
        }

        public static string GetRandomCode(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}