using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class FilaConfiguration : EntityBaseConfiguration<Fila>
{
    public override void Configure(EntityTypeBuilder<Fila> builder)
    {
        base.Configure(builder);

        builder.ToTable("Filas");

        builder.Property(f => f.IdEquipe)
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.HasIndex(f => f.IdEquipe)
            .HasDatabaseName("IX_Filas_IdEquipe");

        builder.HasOne(f => f.Equipe)
            .WithOne(e => e.Fila)
            .HasForeignKey<Fila>(f => f.IdEquipe)
            .HasConstraintName("FK_Filas_Ref_Equipes")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(f => f.IdUsuario)
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.HasIndex(f => f.IdUsuario)
            .HasDatabaseName("IX_Filas_IdUsuario");

        builder.HasOne(f => f.Usuario)
            .WithOne(u => u.Fila)
            .HasForeignKey<Fila>(f => f.IdUsuario)
            .HasConstraintName("FK_Filas_Ref_Usuarios")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }

}
