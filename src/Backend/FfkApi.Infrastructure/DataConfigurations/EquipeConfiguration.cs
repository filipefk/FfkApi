using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class EquipeConfiguration : EntityBaseConfiguration<Equipe>
{
    public override void Configure(EntityTypeBuilder<Equipe> builder)
    {
        base.Configure(builder);

        builder.ToTable("Equipes");

        builder.Property(e => e.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Descricao)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.HasMany(e => e.Membros)
            .WithOne(e => e.Equipe)
            .HasForeignKey(m => m.IdEquipe)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.IdOrganizacao)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(e => e.IdOrganizacao)
            .HasDatabaseName("IX_Equipes_IdOrganizacao");

        builder.HasOne(e => e.Organizacao)
            .WithMany()
            .HasForeignKey(e => e.IdOrganizacao)
            .HasConstraintName("FK_Equipes_Ref_Organizacoes")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.Nome, e.IdOrganizacao })
            .IsUnique()
            .HasDatabaseName("UK_Equipes_Nome_IdOrganizacao");
    }
}
