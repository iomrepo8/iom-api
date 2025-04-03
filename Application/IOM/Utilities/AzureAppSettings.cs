using System.Configuration;

namespace IOM.Utilities
{
    public static class AzureAppSettings
    {

        /// <summary>
        ///     Gets the Azure connection string from the app.config.
        /// </summary>
        public static string AzureConnectionString
        {
            get
            {
                return ConfigurationManager
                  .AppSettings[@"AzureStorageConnectionString"] ?? @"";
            }
        }

        /// <summary>
        ///     Gets the Azure default container from the app.config.
        /// </summary>
        public static string AzureDefaultContainer
        {
            get
            {
                return ConfigurationManager
                  .AppSettings[@"AzureDefaultContainer"] ?? @"";
            }
        }

        /// <summary>
        ///     Gets the Azure default blob reference from the app.config.
        /// </summary>
        public static string AzureDefaultBlobReference
        {
            get
            {
                return ConfigurationManager
                  .AppSettings[@"AzureDefaultBlockBlobReference"] ?? @"";
            }
        }

    }
}