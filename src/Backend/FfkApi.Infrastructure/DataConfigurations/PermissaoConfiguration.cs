using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class PermissaoConfiguration : EntityBaseConfiguration<Permissao>
{
    public override void Configure(EntityTypeBuilder<Permissao> builder)
    {
        base.Configure(builder);

        builder.ToTable("Permissoes");

        builder.Property(p => p.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(p => p.Nome)
           .IsUnique()
           .HasDatabaseName("UK_Permissoes_Nome");

        builder.Property(p => p.Descricao)
            .HasMaxLength(2000)
            .IsRequired();
    }
}
