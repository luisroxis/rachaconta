using Microsoft.EntityFrameworkCore;

namespace RachaConta.Infrastructure.Data;

public class RachaContaDbContext : DbContext
{
    public RachaContaDbContext(DbContextOptions<RachaContaDbContext> options) : base(options)
    {
    }
}
