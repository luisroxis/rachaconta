using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RachaConta.Core.Entities;

namespace RachaConta.Infrastructure.Configurations;

public class InviteConfiguration : IEntityTypeConfiguration<Invite>
{
    public void Configure(EntityTypeBuilder<Invite> builder)
    {
        builder.ToTable("Invites");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.CorpoEmail)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(i => i.AmigoId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.DataEnvio)
            .IsRequired();

        builder.Property(i => i.Reenvio)
            .IsRequired();

        builder.Property(i => i.DataReenvio)
            .IsRequired(false);

        builder.Property(i => i.Aceite)
            .IsRequired();

        builder.Property(i => i.UsuarioId)
            .IsRequired();

        // Foreign key relationship
        builder.HasOne(i => i.Usuario)
            .WithMany()
            .HasForeignKey(i => i.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(i => i.UsuarioId);
        builder.HasIndex(i => i.Email);
        builder.HasIndex(i => i.DataEnvio);
    }
}
