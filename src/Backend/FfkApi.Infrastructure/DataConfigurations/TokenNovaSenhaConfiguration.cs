using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class TokenNovaSenhaConfiguration : EntityBaseConfiguration<TokenNovaSenha>
{
    public override void Configure(EntityTypeBuilder<TokenNovaSenha> builder)
    {
        base.Configure(builder);

        builder.ToTable("TokensNovaSenha");

        builder.Property(t => t.Valor)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.IdUsuario)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasAlternateKey(t => new { t.IdUsuario })
            .HasName("UK_TokensNovaSenha_IdUsuario");

        builder.HasOne(t => t.Usuario)
            .WithMany()
            .HasForeignKey(t => t.IdUsuario)
            .HasConstraintName("FK_TokensNovaSenha_Ref_Usuarios")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(t => t.EmailEnviado)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.UltimaTentativaEnvioEmail)
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(t => t.ErroEnvioEmail)
            .HasMaxLength(1000)
            .IsRequired(false);
    }
}
