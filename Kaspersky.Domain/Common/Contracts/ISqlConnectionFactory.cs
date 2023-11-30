using Microsoft.Data.SqlClient;

namespace Kaspersky.Domain.Common.Contracts
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}
