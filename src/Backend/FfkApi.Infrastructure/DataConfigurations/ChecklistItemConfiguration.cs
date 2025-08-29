using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class ChecklistItemConfiguration : EntityBaseConfiguration<ChecklistItem>
{
    public override void Configure(EntityTypeBuilder<ChecklistItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("ChecklistItens");

        builder.Property(ci => ci.Ordem)
            .HasColumnType("int")
            .IsRequired();

        builder.Property(ci => ci.Descricao)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(ci => ci.TipoChecklistItem)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(ci => ci.IdChecklist)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(ci => ci.IdChecklist)
            .HasDatabaseName("IX_ChecklistItens_IdChecklist");

        builder.HasOne(ci => ci.Checklist)
            .WithMany(c => c.ChecklistItens)
            .HasForeignKey(ci => ci.IdChecklist)
            .HasConstraintName("FK_ChecklistItens_Ref_Checklists")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(ci => ci.IdDependeDeChecklistItem)
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.HasIndex(ci => ci.IdDependeDeChecklistItem)
            .HasDatabaseName("IX_ChecklistItens_IdDependeDeChecklistItem");

        builder.HasOne(ci => ci.DependeDeChecklistItem)
            .WithMany()
            .HasForeignKey(ci => ci.IdDependeDeChecklistItem)
            .HasConstraintName("FK_ChecklistItens_Ref_ChecklistItens")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ci => ci.ChecklistRespostasPossiveis)
            .WithOne(cr => cr.ChecklistItem)
            .HasForeignKey(cr => cr.IdChecklistItem)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ci => ci.GatilhosDeResposta)
            .WithOne(cg => cg.ChecklistItem)
            .HasForeignKey(cg => cg.IdChecklistItem)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
