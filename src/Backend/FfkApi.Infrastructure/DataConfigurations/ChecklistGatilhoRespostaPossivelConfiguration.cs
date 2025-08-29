using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class ChecklistGatilhoRespostaPossivelConfiguration : EntityBaseConfiguration<ChecklistGatilhoRespostaPossivel>
{
    public override void Configure(EntityTypeBuilder<ChecklistGatilhoRespostaPossivel> builder)
    {
        base.Configure(builder);

        builder.ToTable("ChecklistGatilhosRespostasPossiveis");

        builder.Property(cg => cg.IdChecklistItem)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(cg => cg.IdChecklistItem)
            .HasDatabaseName("IX_ChecklistGatilhosRespostasPossiveis_IdChecklistItem");

        builder.HasOne(cg => cg.ChecklistItem)
            .WithMany(ci => ci.GatilhosDeResposta)
            .HasForeignKey(cg => cg.IdChecklistItem)
            .HasConstraintName("FK_ChecklistGatilhosRespostasPossiveis_Ref_ChecklistItens")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(cg => cg.IdChecklistRespostaPossivel)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(cg => cg.IdChecklistRespostaPossivel)
            .HasDatabaseName("IX_ChecklistGatilhosRespostasPossiveis_IdChecklistRespostaPossivel");

        builder.HasOne(cg => cg.RespostaGatilho)
            .WithMany()
            .HasForeignKey(cg => cg.IdChecklistRespostaPossivel)
            .HasConstraintName("FK_ChecklistGatilhosRespostasPossiveis_Ref_ChecklistRespostasPossiveis")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
