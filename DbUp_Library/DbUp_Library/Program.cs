using DbUp_Library.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = CreateHostBuilder(args).Build();
var databaseInitializer = host.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();
await host.RunAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((builder, services) =>
        {
            services.AddSingleton<DatabaseInitializer>(_ => new DatabaseInitializer(builder.Configuration["Database:ConnectionString"]!));
        });