using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RachaConta.Application.Interfaces;
using RachaConta.Infrastructure.Data;

namespace RachaConta.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IEmailService> EmailServiceMock { get; } = new Mock<IEmailService>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<RachaContaDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add InMemory database for testing
            services.AddDbContext<RachaContaDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            // Remove the real email service
            var emailServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IEmailService));

            if (emailServiceDescriptor != null)
            {
                services.Remove(emailServiceDescriptor);
            }

            // Add mocked email service
            services.AddSingleton(EmailServiceMock.Object);

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<RachaContaDbContext>();

                // Ensure the database is created
                db.Database.EnsureCreated();
            }
        });
    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RachaContaDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }
}
