using System.Data;

namespace Gateway.Domain.Persistence;

public interface IConnectionProvider
{
    IDbConnection GetConnection(string? connectionName = null);
}
