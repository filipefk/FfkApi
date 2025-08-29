using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class IndisponibilidadeConfiguration : EntityBaseConfiguration<Indisponibilidade>
{
    public override void Configure(EntityTypeBuilder<Indisponibilidade> builder)
    {
        base.Configure(builder);

        builder.ToTable("Indisponibilidades");

        builder.Property(i => i.Descricao)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(i => i.DataInicial)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(i => i.DataFinal)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(i => i.IdUsuario)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(i => i.IdUsuario)
            .HasDatabaseName("IX_Indisponibilidades_IdUsuario");

        builder.HasOne(i => i.Usuario)
            .WithMany()
            .HasForeignKey(i => i.IdUsuario)
            .HasConstraintName("FK_Indisponibilidades_Ref_Usuarios")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);


    }
}
