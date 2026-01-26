using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Minimes.Infrastructure.Persistence;

/// <summary>
/// Database provider configuration extensions
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Add database context with provider switching support
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration.GetValue<string>("Database:Provider") ?? "SQLite";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            ConfigureProvider(options, configuration, provider);
        });

        return services;
    }

    private static void ConfigureProvider(DbContextOptionsBuilder options, IConfiguration configuration, string provider)
    {
        switch (provider.ToUpperInvariant())
        {
            case "MYSQL":
                ConfigureMySql(options, configuration);
                break;
            case "SQLITE":
            default:
                ConfigureSqlite(options, configuration);
                break;
        }
    }

    private static void ConfigureSqlite(DbContextOptionsBuilder options, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=minimes.db";
        options.UseSqlite(connectionString);
    }

    private static void ConfigureMySql(DbContextOptionsBuilder options, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MySqlConnection")
            ?? throw new InvalidOperationException("MySqlConnection string is not configured.");

        // Pomelo.EntityFrameworkCore.MySql requires ServerVersion
        ServerVersion serverVersion;
        try
        {
            serverVersion = ServerVersion.AutoDetect(connectionString);
        }
        catch
        {
            // Fallback to MySQL 8.0 if AutoDetect fails (e.g., during design-time migrations)
            serverVersion = new MySqlServerVersion(new Version(8, 0));
        }

        options.UseMySql(connectionString, serverVersion, mySqlOptions =>
        {
            // Enable retry on failure for transient errors
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);

            // Use MigrationsAssembly to ensure migrations are stored in Infrastructure project
            mySqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
        });
    }
}
