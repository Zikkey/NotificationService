using System.Data;
using Gateway.Domain.Persistence;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Gateway.Infrastructure.Persistence;

public class PostgresConnectionProvider(IConfiguration configuration) : IConnectionProvider
{
    private const string DefaultDb = "Db";

    public IDbConnection GetConnection(string? connectionName = null)
    {
        var connectionStringName = connectionName ?? DefaultDb;
        var connectionString = configuration.GetConnectionString(connectionStringName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Строка подключения с именем '{connectionStringName}' не найдена.");
        }

        return new NpgsqlConnection(connectionString);
    }
}
