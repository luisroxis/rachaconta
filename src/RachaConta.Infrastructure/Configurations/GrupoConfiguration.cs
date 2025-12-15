using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RachaConta.Core.Entities;

namespace RachaConta.Infrastructure.Configurations;

public class GrupoConfiguration : IEntityTypeConfiguration<Grupo>
{
    public void Configure(EntityTypeBuilder<Grupo> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.IdCategoria)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(g => g.Descricao)
            .HasMaxLength(1000);

        builder.Property(g => g.OutrasCategorias)
            .HasMaxLength(500);

        builder.Property(g => g.LinkConvite)
            .HasMaxLength(500);

        builder.Property(g => g.ImgGrupo)
            .HasMaxLength(500);

        builder.Property(g => g.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(g => g.Deleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(g => g.Deleted);
        builder.HasIndex(g => g.Ativo);
    }
}
