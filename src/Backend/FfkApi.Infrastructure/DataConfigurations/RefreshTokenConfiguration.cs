using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class RefreshTokenConfiguration : EntityBaseConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.ToTable("RefreshTokens");

        builder.Property(r => r.Valor)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(r => r.IdUsuario)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasAlternateKey(r => new { r.IdUsuario })
            .HasName("UK_RefreshTokens_IdUsuario");

        builder.HasOne(r => r.Usuario)
            .WithMany()
            .HasForeignKey(r => r.IdUsuario)
            .HasConstraintName("FK_RefreshTokens_Ref_Usuarios")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
