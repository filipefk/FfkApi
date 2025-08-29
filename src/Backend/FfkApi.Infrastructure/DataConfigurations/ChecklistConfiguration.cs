using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class ChecklistConfiguration : EntityBaseConfiguration<Checklist>
{
    public override void Configure(EntityTypeBuilder<Checklist> builder)
    {
        base.Configure(builder);

        builder.ToTable("Checklists");

        builder.Property(c => c.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Descricao)
            .HasMaxLength(2000)
            .IsRequired();

        builder.HasMany(c => c.ChecklistItens)
            .WithOne(ci => ci.Checklist)
            .HasForeignKey(ci => ci.IdChecklist)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.IdOrganizacao)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(c => c.IdOrganizacao)
            .HasDatabaseName("IX_Checklists_IdOrganizacao");

        builder.HasOne(c => c.Organizacao)
            .WithMany()
            .HasForeignKey(c => c.IdOrganizacao)
            .HasConstraintName("FK_Checklists_Ref_Organizacoes")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
