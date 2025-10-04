using Domain.Core.Entities;
using Infrastructure.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class CountryCurrencyConfiguration : IEntityTypeConfiguration<CountryCurrency>
{
    public void Configure(EntityTypeBuilder<CountryCurrency> entity)
    {
        entity.ToTable(nameof(CountryCurrency), SchemaConstants.Default);

        entity.HasKey(x => new { x.CurrencyCode, x.CountryIso2 });

        entity.Property(x => x.CountryIso2)
            .HasMaxLength(2)
            .IsFixedLength()
            .IsRequired();

        entity.Property(x => x.CurrencyCode)
            .HasMaxLength(3)
            .IsFixedLength()
            .IsRequired();

        entity.HasOne(x => x.Country)
         .WithMany(c => c.Currencies)
         .HasForeignKey(x => x.CountryIso2)
         .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.Currency)
         .WithMany(c => c.Countries)
         .HasForeignKey(x => x.CurrencyCode)
         .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(x => new { x.CountryIso2, x.CurrencyCode, x.ValidFrom })
            .IsUnique();

        entity.HasIndex(x => new { x.CountryIso2, x.IsPrimary, x.ValidFrom })
         .HasFilter("\"IsPrimary\" = true");

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<CountryCurrency> entity);
}
