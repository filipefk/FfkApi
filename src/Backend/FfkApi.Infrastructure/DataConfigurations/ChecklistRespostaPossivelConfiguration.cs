using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class ChecklistRespostaPossivelConfiguration : EntityBaseConfiguration<ChecklistRespostaPossivel>
{
    public override void Configure(EntityTypeBuilder<ChecklistRespostaPossivel> builder)
    {
        base.Configure(builder);

        builder.ToTable("ChecklistRespostasPossiveis");

        builder.Property(cr => cr.Ordem)
            .HasColumnType("int")
            .IsRequired();

        builder.Property(cr => cr.Descricao)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(cr => cr.GeraInconformidade)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(cr => cr.IdChecklistItem)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(cr => cr.IdChecklistItem)
            .HasDatabaseName("IX_ChecklistRespostasPossiveis_IdChecklistItem");

        builder.HasOne(cr => cr.ChecklistItem)
            .WithMany(ci => ci.ChecklistRespostasPossiveis)
            .HasForeignKey(cr => cr.IdChecklistItem)
            .HasConstraintName("FK_ChecklistRespostasPossiveis_Ref_ChecklistItens")
            .OnDelete(DeleteBehavior.Cascade);

    }

}
