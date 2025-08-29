using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class OrganizacaoConfiguration : EntityBaseConfiguration<Organizacao>
{
    public override void Configure(EntityTypeBuilder<Organizacao> builder)
    {
        base.Configure(builder);

        builder.ToTable("Organizacoes");

        builder.Property(o => o.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(o => o.Nome)
           .IsUnique()
           .HasDatabaseName("UK_Organizacoes_Nome");

        builder.Property(o => o.Descricao)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(o => o.RemetenteEmail)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(o => o.RemetenteNome)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(o => o.ModeloEmailAtivacao)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(o => o.ModeloEmailNovaSenha)
            .HasMaxLength(100)
            .IsRequired(false);
    }
}
