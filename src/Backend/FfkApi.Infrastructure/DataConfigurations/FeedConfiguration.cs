using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class FeedConfiguration : EntityBaseConfiguration<Feed>
{
    public override void Configure(EntityTypeBuilder<Feed> builder)
    {
        base.Configure(builder);

        builder.ToTable("Feeds");

        builder.Property(f => f.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(f => f.Descricao)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(f => f.PalavrasChave)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(f => f.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder
            .HasMany(f => f.Anexos)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "FeedsAnexos",
                j => j.HasOne<Anexo>()
                      .WithMany()
                      .HasForeignKey("IdAnexo")
                      .HasConstraintName("FK_FeedsAnexos_Ref_Anexos")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Feed>()
                      .WithMany()
                      .HasForeignKey("IdFeed")
                      .HasConstraintName("FK_FeedsAnexos_Ref_Feeds")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("IdFeed", "IdAnexo")
                     .HasName("PK_FeedsAnexos");
                });

        builder
            .HasMany(f => f.VisibilidadeUsuarios)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "FeedsUsuarios",
                j => j.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey("IdUsuario")
                      .HasConstraintName("FK_FeedsUsuarios_Ref_Usuarios")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Feed>()
                      .WithMany()
                      .HasForeignKey("IdFeed")
                      .HasConstraintName("FK_FeedsUsuarios_Ref_Feeds")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("IdFeed", "IdUsuario")
                     .HasName("PK_FeedsUsuarios");
                });

        builder
            .HasMany(f => f.VisibilidadeEquipes)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "FeedsEquipes",
                j => j.HasOne<Equipe>()
                      .WithMany()
                      .HasForeignKey("IdEquipe")
                      .HasConstraintName("FK_FeedsEquipes_Ref_Equipes")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Feed>()
                      .WithMany()
                      .HasForeignKey("IdFeed")
                      .HasConstraintName("FK_FeedsEquipes_Ref_Feeds")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("IdFeed", "IdEquipe")
                     .HasName("PK_FeedsEquipes");
                });

        builder.Property(f => f.ExpiraEm)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(f => f.IdOrganizacao)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(f => f.IdOrganizacao)
            .HasDatabaseName("IX_Feeds_IdOrganizacao");

        builder.HasOne(f => f.Organizacao)
            .WithMany()
            .HasForeignKey(f => f.IdOrganizacao)
            .HasConstraintName("FK_Feeds_Ref_Organizacoes")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
