using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class FilaItemConfiguration : EntityBaseConfiguration<FilaItem>
{
    public override void Configure(EntityTypeBuilder<FilaItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("FilaItens");

        builder.Property(f => f.IdFila)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(f => f.IdFila)
            .HasDatabaseName("IX_FilaItens_IdFila");

        builder.HasOne(f => f.Fila)
            .WithMany(f => f.FilaItens)
            .HasForeignKey(f => f.IdFila)
            .HasConstraintName("FK_FilaItens_Ref_Filas")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(m => m.Posicao)
            .HasColumnType("bigint")
            .IsRequired();
    }

}
