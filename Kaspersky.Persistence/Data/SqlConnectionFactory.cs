using Kaspersky.Domain.Common.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Kaspersky.Persistence.Data
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;
        public SqlConnectionFactory(IConfiguration configuration) 
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnect");
        }
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
