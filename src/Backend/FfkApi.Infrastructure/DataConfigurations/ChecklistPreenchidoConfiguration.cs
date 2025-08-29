using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class ChecklistPreenchidoConfiguration : EntityBaseConfiguration<ChecklistPreenchido>
{
    public override void Configure(EntityTypeBuilder<ChecklistPreenchido> builder)
    {
        base.Configure(builder);

        builder.ToTable("ChecklistsPreenchidos");

        builder.Property(cp => cp.NomeChecklist)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(cp => cp.DescricaoChecklist)
            .HasMaxLength(2000)
            .IsRequired();

        builder.HasMany(cp => cp.ChecklistPreenchidoItens)
            .WithOne(cpi => cpi.ChecklistPreenchido)
            .HasForeignKey(cpi => cpi.IdChecklistPreenchido)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(cp => cp.IdChecklist)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(cp => cp.IdChecklist)
            .HasDatabaseName("IX_ChecklistsPreenchidos_IdChecklist");

        builder.HasOne(cp => cp.Checklist)
            .WithMany()
            .HasForeignKey(cp => cp.IdChecklist)
            .HasConstraintName("FK_ChecklistsPreenchidos_Ref_Checklists")
            .OnDelete(DeleteBehavior.SetNull);
    }

}
