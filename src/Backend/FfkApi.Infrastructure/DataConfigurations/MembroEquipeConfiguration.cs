using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class MembroEquipeConfiguration : EntityBaseConfiguration<MembroEquipe>
{
    public override void Configure(EntityTypeBuilder<MembroEquipe> builder)
    {
        base.Configure(builder);

        builder.ToTable("MembrosEquipe");

        builder.Property(m => m.IdEquipe)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(m => m.IdEquipe)
            .HasDatabaseName("IX_MembrosEquipe_IdEquipe");

        builder.Property(m => m.IdUsuario)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(m => m.IdUsuario)
            .HasDatabaseName("IX_MembrosEquipe_IdUsuario");

        builder.HasIndex(m => new { m.IdEquipe, m.IdUsuario })
            .IsUnique()
            .HasDatabaseName("UK_MembrosEquipe_IdEquipe_IdUsuario");

        builder.HasOne(m => m.Equipe)
            .WithMany(e => e.Membros)
            .HasForeignKey(m => m.IdEquipe)
            .HasConstraintName("FK_MembrosEquipe_Ref_Equipes")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Usuario)
            .WithMany()
            .HasForeignKey(m => m.IdUsuario)
            .HasConstraintName("FK_MembrosEquipe_Ref_Usuarios")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(m => m.Lider)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
