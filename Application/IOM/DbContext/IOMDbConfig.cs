using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace IOM.DbContext
{
    public class IOMDbConfig : DbConfiguration
    {
        public IOMDbConfig()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}