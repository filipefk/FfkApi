using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class UsuarioConfiguration : EntityBaseConfiguration<Usuario>
{
    public override void Configure(EntityTypeBuilder<Usuario> builder)
    {
        base.Configure(builder);

        builder.ToTable("Usuarios");

        builder.Property(u => u.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(u => u.Email)
           .IsUnique()
           .HasDatabaseName("UK_Usuarios_Email");

        builder.Property(u => u.Senha)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(u => u.Cpf)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(u => u.Telefone)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder
            .HasMany(p => p.PerfisAcesso)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "UsuariosPerfisAcesso",
                j => j.HasOne<PerfilAcesso>()
                      .WithMany()
                      .HasForeignKey("IdPerfilAcesso")
                      .HasConstraintName("FK_UsuariosPerfisAcesso_Ref_PerfisAcesso")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey("IdUsuario")
                      .HasConstraintName("FK_UsuariosPerfisAcesso_Ref_Usuarios")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("IdUsuario", "IdPerfilAcesso")
                     .HasName("PK_UsuariosPerfisAcesso");
                });

        builder
            .HasMany(p => p.Permissoes)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "UsuariosPermissoes",
                j => j.HasOne<Permissao>()
                      .WithMany()
                      .HasForeignKey("IdPermissao")
                      .HasConstraintName("FK_UsuariosPermissoes_Ref_Permissoes")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey("IdUsuario")
                      .HasConstraintName("FK_UsuariosPermissoes_Ref_Usuarios")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("IdUsuario", "IdPermissao")
                     .HasName("PK_UsuariosPermissoes");
                });

        builder.Property(u => u.IdOrganizacao)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(u => u.IdOrganizacao)
            .HasDatabaseName("IX_Usuarios_IdOrganizacao");

        builder.HasOne(u => u.Organizacao)
            .WithMany()
            .HasForeignKey(u => u.IdOrganizacao)
            .HasConstraintName("FK_Usuarios_Ref_Organizacoes")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
