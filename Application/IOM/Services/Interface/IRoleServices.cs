namespace IOM.Services.Interface
{
    public interface IRoleServices
    {
        dynamic GetAssocRoleList(bool includeAdmins, string query = "");
    }
}