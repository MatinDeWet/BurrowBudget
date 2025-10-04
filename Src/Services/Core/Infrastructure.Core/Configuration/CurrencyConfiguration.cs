using Domain.Core.Entities;
using Infrastructure.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> entity)
    {
        entity.ToTable(nameof(Currency), SchemaConstants.Default, t => t.HasCheckConstraint("ck_currency_code", "length(\"Code\")=3 AND \"Code\" ~ '^[A-Z]{3}$'"));

        entity.Ignore(x => x.Id);

        entity.HasKey(x => x.Code);

        entity.Property(x => x.Code)
            .ValueGeneratedNever();

        entity.Property(x => x.Code)
            .HasMaxLength(3)
            .IsFixedLength()
            .IsRequired();

        entity.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(x => x.Symbol)
            .HasMaxLength(8);

        entity.Property(x => x.MinorUnits)
            .HasDefaultValue(2);

        entity.HasIndex(x => x.Name).IsUnique();

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<Currency> entity);
}

