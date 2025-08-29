using FfkApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FfkApi.Infrastructure.DataConfigurations;

public class EntityBaseConfiguration<T> : IEntityTypeConfiguration<T> where T : EntityBase
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        var tableName = builder.Metadata.GetTableName();

        builder.HasKey(e => e.Id)
            .HasName($"PK_{tableName}");

        builder.Property(e => e.Id)
            .HasColumnType("uuid")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(e => e.DataCriacaoUtc)
            .HasColumnType("timestamptz")
            .IsRequired();
    }
}
