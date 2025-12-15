using Microsoft.EntityFrameworkCore;
using RachaConta.Core.Entities;

namespace RachaConta.Infrastructure.Data;

public class RachaContaDbContext : DbContext
{
    public RachaContaDbContext(DbContextOptions<RachaContaDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<Invite> Invites { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Friendship> Friendships { get; set; }

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

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.UserId);
            
            entity.Property(e => e.Token).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.IsUsed).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Apply Invite configuration
        modelBuilder.ApplyConfiguration(new Configurations.InviteConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.FriendshipConfiguration());
    }
}
