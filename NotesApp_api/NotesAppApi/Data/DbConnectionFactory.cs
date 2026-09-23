using System.Data;
using Microsoft.Data.SqlClient;

namespace NotesAppApi.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}