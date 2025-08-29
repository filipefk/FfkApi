using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class AuditoriaSegurancaConfiguration : EntityBaseConfiguration<AuditoriaSeguranca>
{
    public override void Configure(EntityTypeBuilder<AuditoriaSeguranca> builder)
    {
        base.Configure(builder);

        builder.ToTable("AuditoriasSeguranca");

        builder.Property(a => a.Evento)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Usuario)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(a => a.EnderecoIp)
            .HasMaxLength(45)
            .IsRequired(false);

        builder.Property(a => a.Caminho)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(a => a.Metodo)
            .HasMaxLength(10)
            .IsRequired(false);

        builder.Property(a => a.Detalhes)
            .HasColumnType("text")
            .IsRequired(false);

    }

}
