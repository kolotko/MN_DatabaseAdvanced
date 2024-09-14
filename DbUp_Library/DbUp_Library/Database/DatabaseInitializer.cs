using DbUp;

namespace DbUp_Library.Database;

public class DatabaseInitializer(string connectionString)
{
    private readonly string _connectionString = connectionString;
    public async Task InitializeAsync()
    {
        EnsureDatabase.For.PostgresqlDatabase(_connectionString);
        var upgrader = DeployChanges.To.PostgresqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DatabaseInitializer).Assembly)
            .LogToConsole()
            .Build();

        if (upgrader.IsUpgradeRequired())
        {
            upgrader.PerformUpgrade();
        }
    }
}