using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RachaConta.Infrastructure.Data;

public class RachaContaDbContextFactory : IDesignTimeDbContextFactory<RachaContaDbContext>
{
    public RachaContaDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        
        // If running from project dir 'src/RachaConta.Infrastructure'
        var apiPath = Path.Combine(basePath, "../RachaConta.Api");
        
        // If running from root solution dir '...'
        if (!Directory.Exists(apiPath))
        {
             apiPath = Path.Combine(basePath, "src/RachaConta.Api");
        }
        
        // Final fallback or throw
        if (!Directory.Exists(apiPath))
        {
             // Try absolute path assuming standard structure if all else fails, or just throw with debug info
             throw new DirectoryNotFoundException($"Could not find API directory. Current: {basePath}");
        }

        // Build configuration
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetFullPath(apiPath))
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var builder = new DbContextOptionsBuilder<RachaContaDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseNpgsql(connectionString, b => b.MigrationsAssembly("RachaConta.Infrastructure"));

        return new RachaContaDbContext(builder.Options);
    }
}
