using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using RachaConta.Application.Interfaces;
using RachaConta.Infrastructure.Data;
using RachaConta.IntegrationTests.Data;

namespace RachaConta.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "..", "..", "..", "..", "DataBase", "TestDatabase.db");

    public Mock<IEmailService> EmailServiceMock { get; } = new Mock<IEmailService>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "ThisIsASecretKeyForIntegrationTesting2025!" },
                { "Jwt:Issuer", "RachaConta.IntegrationTests" },
                { "Jwt:Audience", "RachaConta.IntegrationTests" }
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // 1. Remove all existing Context and Options registrations to clear the deck
            var descriptors = services.Where(d => 
                d.ServiceType == typeof(DbContextOptions<RachaContaDbContext>) ||
                d.ServiceType == typeof(RachaContaDbContext) ||
                d.ServiceType == typeof(DbContextOptions)).ToList();
            
            foreach (var d in descriptors)
            {
                services.Remove(d);
            }

            // 2. Create options for SQLite
            var optionsBuilder = new DbContextOptionsBuilder<RachaContaDbContext>();
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
            var options = optionsBuilder.Options;

            // 3. Register the Options so the TestRachaContaDbContext constructor can find them
            services.AddSingleton(options);

            // 4. Register our Test Context
            services.AddScoped<TestRachaContaDbContext>();

            // 5. Redirect the main application Context to our Test Context
            services.AddScoped<RachaContaDbContext>(provider => 
                provider.GetRequiredService<TestRachaContaDbContext>());

            // Remove the real email service
            var emailServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IEmailService));

            if (emailServiceDescriptor != null)
            {
                services.Remove(emailServiceDescriptor);
            }

            // Add mocked email service
            services.AddSingleton(EmailServiceMock.Object);
        });

        builder.UseEnvironment("Testing");
    }

    public CustomWebApplicationFactory()
    {
        // No database creation here to avoid double-initialization or scope issues
        // The ResetDatabase method handles ensure created/deleted
    }

    public void ResetDatabase()
    {
        EmailServiceMock.Reset();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RachaContaDbContext>();
        
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }
}
