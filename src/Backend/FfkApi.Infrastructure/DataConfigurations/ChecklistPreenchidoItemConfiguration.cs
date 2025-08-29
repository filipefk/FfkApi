using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class ChecklistPreenchidoItemConfiguration : EntityBaseConfiguration<ChecklistPreenchidoItem>
{
    public override void Configure(EntityTypeBuilder<ChecklistPreenchidoItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("ChecklistPreenchidoItens");

        builder.Property(cpi => cpi.OrdemItem)
            .HasColumnType("int")
            .IsRequired();

        builder.Property(cpi => cpi.DescricaoItem)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(cpi => cpi.DescricaoRespostaEscolhida)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(cpi => cpi.GeraInconformidade)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(cpi => cpi.Observacao)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(cpi => cpi.IdChecklistPreenchido)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(cpi => cpi.IdChecklistPreenchido)
            .HasDatabaseName("IX_ChecklistPreenchidoItens_IdChecklistPreenchido");

        builder.HasOne(cpi => cpi.ChecklistPreenchido)
            .WithMany(cp => cp.ChecklistPreenchidoItens)
            .HasForeignKey(cpi => cpi.IdChecklistPreenchido)
            .HasConstraintName("FK_ChecklistPreenchidoItens_Ref_ChecklistPreenchido")
            .OnDelete(DeleteBehavior.Cascade);
    }

}
