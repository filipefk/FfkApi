using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class PessoaConfiguration : EntityBaseConfiguration<Pessoa>
{
    public override void Configure(EntityTypeBuilder<Pessoa> builder)
    {
        base.Configure(builder);

        builder.ToTable("Pessoas");

        builder.Property(p => p.TipoPessoa)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(p => p.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.Telefone)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.CpfCnpj)
            .HasMaxLength(14)
            .IsRequired();

        builder.Property(p => p.IdOrganizacao)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(p => p.IdOrganizacao)
            .HasDatabaseName("IX_Pessoas_IdOrganizacao");

        builder.HasOne(p => p.Organizacao)
            .WithMany()
            .HasForeignKey(p => p.IdOrganizacao)
            .HasConstraintName("FK_Pessoas_Ref_Organizacoes")
            .OnDelete(DeleteBehavior.Restrict);
    }

}
