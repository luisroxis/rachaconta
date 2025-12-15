using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RachaConta.Core.Entities;

namespace RachaConta.Infrastructure.Configurations;

public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.UserId)
            .IsRequired();

        builder.Property(f => f.AmigoId)
            .IsRequired();

        builder.Property(f => f.Convidado)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.ConvidadoEmail)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Approved)
            .IsRequired();

        // Relationships
        builder.HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Amigo)
            .WithMany()
            .HasForeignKey(f => f.AmigoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
