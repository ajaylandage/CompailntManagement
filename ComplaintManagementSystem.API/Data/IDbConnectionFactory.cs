using System.Data;

namespace ComplaintManagementSystem.API.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}