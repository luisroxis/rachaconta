using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RachaConta.Core.Entities;

namespace RachaConta.Infrastructure.Configurations;

public class ParticipanteGrupoConfiguration : IEntityTypeConfiguration<ParticipanteGrupo>
{
    public void Configure(EntityTypeBuilder<ParticipanteGrupo> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.IdGrupo)
            .IsRequired();

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(p => p.IsAdm)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.HasPendent)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(p => new { p.IdGrupo, p.UserId })
            .IsUnique();

        builder.HasIndex(p => p.IdGrupo);
        builder.HasIndex(p => p.UserId);

        builder.HasOne<Grupo>()
            .WithMany()
            .HasForeignKey(p => p.IdGrupo)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
