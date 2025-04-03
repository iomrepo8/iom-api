using System.Configuration;

namespace IOM.Utilities
{
    public class EmailSettings
    {
        private static EmailSettings _instance;
        private static readonly object _lock = new object();

        private EmailSettings() { }

        public static EmailSettings Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new EmailSettings();
                    }

                    return _instance;
                }
            }
        }

        internal string SendGridApiKey 
        { 
            get
            {
                return ConfigurationManager.AppSettings["SendGridApiKey"];
            } 
        }

        internal string EmailAccount
        {
            get
            {
                return ConfigurationManager.AppSettings["mailAccount"];
            }
        }

        internal string SenderName
        {
            get
            {
                return ConfigurationManager.AppSettings["SenderName"];
            }
        }
    }
}