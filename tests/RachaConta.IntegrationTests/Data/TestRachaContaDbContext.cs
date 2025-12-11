using Microsoft.EntityFrameworkCore;
using RachaConta.Infrastructure.Data;

namespace RachaConta.IntegrationTests.Data;

public class TestRachaContaDbContext : RachaContaDbContext
{
    public TestRachaContaDbContext(DbContextOptions<RachaContaDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Ensure no other provider is configured if options are not already set
        if (!optionsBuilder.IsConfigured)
        {
            // Fallback or explicit configuration if needed
        }
    }
}
