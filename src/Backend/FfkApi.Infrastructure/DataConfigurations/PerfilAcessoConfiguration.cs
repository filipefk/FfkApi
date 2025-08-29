using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class PerfilAcessoConfiguration : EntityBaseConfiguration<PerfilAcesso>
{
    public override void Configure(EntityTypeBuilder<PerfilAcesso> builder)
    {
        base.Configure(builder);

        builder.ToTable("PerfisAcesso");

        builder.Property(p => p.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(p => p.Nome)
           .IsUnique()
           .HasDatabaseName("UK_PerfisAcesso_Nome");

        builder.Property(p => p.Descricao)
            .HasMaxLength(2000)
            .IsRequired();

        builder
            .HasMany(p => p.Permissoes)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "PerfisAcessoPermissoes",
                j => j.HasOne<Permissao>()
                      .WithMany()
                      .HasForeignKey("IdPermissao")
                      .HasConstraintName("FK_PerfisAcessoPermissoes_Ref_Permissoes")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<PerfilAcesso>()
                      .WithMany()
                      .HasForeignKey("IdPerfilAcesso")
                      .HasConstraintName("FK_PerfisAcessoPermissoes_Ref_PerfisAcesso")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("IdPerfilAcesso", "IdPermissao")
                     .HasName("PK_PerfisAcessoPermissoes");
                });
    }
}
