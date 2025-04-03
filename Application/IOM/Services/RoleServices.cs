using IOM.DbContext;
using IOM.Helpers;
using System.Linq;
using IOM.Services.Interface;

namespace IOM.Services
{
    public class RoleServices : IRoleServices
    {
        public dynamic GetAssocRoleList(bool includeAdmins, string query = "")
        {
            query = query.ToLower();

            using (var ctx = Entities.Create())
            {
                var dataQuery = ctx.AspNetRoles.Select(e => new { e.Id, e.Name, e.RoleCode });

                if(!includeAdmins)
                {
                    dataQuery = dataQuery.Where(e => e.RoleCode != "SA" && e.RoleCode != "AM" && e.RoleCode != Globals.CLIENT_RC);
                }

                if (query != "")
                {
                    dataQuery = dataQuery.Where(e => e.Name.ToLower().Contains(query) ||
                                                     query.Contains(e.Name.ToLower()));
                }

                return dataQuery.ToList();
            }
        }
    }
}