using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Minimes.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for ApplicationDbContext
/// Used by EF Core tools (dotnet ef migrations) to create DbContext at design time
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Build configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Minimes.Web"))
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Read provider from configuration
        var provider = configuration.GetValue<string>("Database:Provider") ?? "SQLite";

        if (provider.Equals("MySQL", StringComparison.OrdinalIgnoreCase))
        {
            // Configure MySQL
            var connectionString = configuration.GetConnectionString("MySqlConnection")
                ?? throw new InvalidOperationException("MySqlConnection string is not configured.");

            var serverVersion = new MySqlServerVersion(new Version(8, 0));
            optionsBuilder.UseMySql(connectionString, serverVersion);
        }
        else
        {
            // Configure SQLite (default)
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=minimes.db";
            optionsBuilder.UseSqlite(connectionString);
        }

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
