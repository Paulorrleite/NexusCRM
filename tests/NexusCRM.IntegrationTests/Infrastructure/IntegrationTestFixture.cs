using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusCRM.Application;
using NexusCRM.Infrastructure;
using NexusCRM.Infrastructure.Persistence;
using Npgsql;

namespace NexusCRM.IntegrationTests.Infrastructure;

public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private const string ConnectionStringName = "NexusCRMIntegrationTests";
    private ServiceProvider? _serviceProvider;

    public IServiceScope CreateScope()
    {
        if (_serviceProvider is null)
        {
            throw new InvalidOperationException("Integration test fixture was not initialized.");
        }

        return _serviceProvider.CreateScope();
    }

    public async Task InitializeAsync()
    {
        var configuration = BuildConfiguration();
        var connectionString = configuration.GetConnectionString(ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string 'ConnectionStrings:{ConnectionStringName}' is required for integration tests.");
        }

        await EnsureDatabaseExistsAsync(connectionString);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddInfrastructure(CreateInfrastructureConfiguration(connectionString));

        _serviceProvider = services.BuildServiceProvider(validateScopes: true);

        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NexusCrmDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NexusCrmDbContext>();

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            TRUNCATE TABLE
                subscriptions,
                organizations,
                plans
            RESTART IDENTITY CASCADE;
            """);
    }

    public async Task DisposeAsync()
    {
        if (_serviceProvider is not null)
        {
            await _serviceProvider.DisposeAsync();
        }
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddUserSecrets<IntegrationTestFixture>(optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private static IConfiguration CreateInfrastructureConfiguration(string connectionString)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:NexusCRM"] = connectionString
            })
            .Build();
    }

    private static async Task EnsureDatabaseExistsAsync(string connectionString)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = connectionStringBuilder.Database;

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("Integration test connection string must include a database name.");
        }

        connectionStringBuilder.Database = "postgres";

        await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
        await connection.OpenAsync();

        await using var existsCommand = connection.CreateCommand();
        existsCommand.CommandText = "SELECT 1 FROM pg_database WHERE datname = @database_name;";
        existsCommand.Parameters.AddWithValue("database_name", databaseName);

        var exists = await existsCommand.ExecuteScalarAsync();

        if (exists is not null)
        {
            return;
        }

        await using var createCommand = connection.CreateCommand();
        createCommand.CommandText = $"CREATE DATABASE {QuoteIdentifier(databaseName)};";
        await createCommand.ExecuteNonQueryAsync();
    }

    private static string QuoteIdentifier(string identifier)
    {
        return "\"" + identifier.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
    }
}
