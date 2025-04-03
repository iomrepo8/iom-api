using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;

namespace IOM.DbContext
{
    [DbConfigurationType(typeof(IOMDbConfig))]
    public partial class Entities
    {
        public Entities(string nameOrConnectionString) : base(nameOrConnectionString) { }

        /// <summary>
        /// Create a new EF6 dynamic data context using the specified provider connection string.
        /// </summary>
        /// <param name="providerConnectionString">Provider connection string to use. Usually a standart ADO.NET connection string.</param>
        /// <returns></returns>
        public static Entities Create()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

            var entityBuilder = new EntityConnectionStringBuilder();

            // use your ADO.NET connection string
            entityBuilder.ProviderConnectionString = connectionString;

            entityBuilder.Provider = "System.Data.SqlClient";

            // Set the Metadata location.
            entityBuilder.Metadata = @"res://*/DbContext.IOMEntities.csdl|res://*/DbContext.IOMEntities.ssdl|res://*/DbContext.IOMEntities.msl";

            return new Entities(entityBuilder.ConnectionString);
        }
    }
}