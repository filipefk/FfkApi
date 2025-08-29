using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class SistemaClienteConfiguration : EntityBaseConfiguration<SistemaCliente>
{
    public override void Configure(EntityTypeBuilder<SistemaCliente> builder)
    {
        base.Configure(builder);

        builder.ToTable("SistemasCliente");

        builder.Property(e => e.AppId)
            .HasColumnType("uuid")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.HasIndex(u => u.AppId)
           .IsUnique()
           .HasDatabaseName("UK_SistemasCliente_AppId");

        builder.Property(a => a.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.Descricao)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(u => u.Senha)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);
    }
}
