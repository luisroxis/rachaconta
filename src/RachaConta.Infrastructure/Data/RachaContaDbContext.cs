using Microsoft.EntityFrameworkCore;
using RachaConta.Core.Entities;

namespace RachaConta.Infrastructure.Data;

public class RachaContaDbContext : DbContext
{
    public RachaContaDbContext(DbContextOptions<RachaContaDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.UserName).IsUnique();
            
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Avatar).HasMaxLength(500);
            entity.Property(e => e.Password).IsRequired().HasMaxLength(500);
        });
    }
}
