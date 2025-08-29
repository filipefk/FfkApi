using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class AnexoConfiguration : EntityBaseConfiguration<Anexo>
{
    public override void Configure(EntityTypeBuilder<Anexo> builder)
    {
        base.Configure(builder);

        builder.ToTable("Anexos");

        builder.Property(a => a.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.Descricao)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(a => a.NomeArquivo)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.NomeArquivoArmazenamento)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.Extensao)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(a => a.TamanhoBytes)
            .HasColumnType("bigint")
            .IsRequired()
            .HasDefaultValue(0L);

        builder.Property(a => a.MimeType)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(a => a.Texto)
            .HasColumnType("text")
            .IsRequired(false);

    }
}
