using System.Data;
using System.Reflection;
using System.Text.Json;
using Dapper;
using FluentMigrator.Runner;
using Gateway.Domain;
using Gateway.Domain.Entities;
using Gateway.Domain.Persistence;
using Gateway.Infrastructure.Persistence;
using Gateway.Infrastructure.Persistence.TypeHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Serilog;

namespace Gateway.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.TryAddScoped<IConnectionProvider, PostgresConnectionProvider>();
        services.AddMediatR(static conf =>
            conf.RegisterServicesFromAssembly(typeof(DataAccessDummy).Assembly));
        SqlMapper.AddTypeHandler(typeof (JsonDocument), new JsonDocumentTypeHandler());
        NpgsqlConnection.GlobalTypeMapper.MapEnum<SavedPacketState>("saved_packet_state");
        return services;
    }

    public static void MigrateDatabaseByType<TMigration>(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var connectionProvider = scope.ServiceProvider.GetRequiredService<IConnectionProvider>();
        connectionProvider.MigrateDatabaseByType<TMigration>();
    }

    public static void MigrateDatabaseByType<TMigration>(this IConnectionProvider connectionProvider)
    {
        using var connection = (NpgsqlConnection)connectionProvider.GetConnection();
        connection.Open();
        connection.ReloadTypes();

        var runner = GetMigrationRunner<TMigration>(connection);
        runner.MigrateUp();
    }

    public static IMigrationRunner GetMigrationRunner<TMigration>(IDbConnection connection)
    {
        return GetMigrationRunner(connection, typeof(TMigration).Assembly);
    }

    public static IMigrationRunner GetMigrationRunner(IDbConnection connection, Assembly migrationAssembly)
    {
        var connectionString = connection.ConnectionString;

        var serviceProvider = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .WithVersionTable(new CustomVersionTable())
                .AddPostgres()
                .WithGlobalConnectionString(_ => connectionString)
                .ScanIn(migrationAssembly).For.Migrations())
            .AddLogging(lb => lb.AddSerilog())
            .BuildServiceProvider(false);

        return serviceProvider.GetRequiredService<IMigrationRunner>();
    }
}
