using Domain.Core.Entities;
using Infrastructure.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> entity)
    {
        entity.ToTable(nameof(Country), SchemaConstants.Default, t =>
        {
            t.HasCheckConstraint("ck_country_iso2", "length(\"Iso2\")=2 AND \"Iso2\" ~ '^[A-Z]{2}$'");
            t.HasCheckConstraint("ck_country_iso3", "length(\"Iso3\")=3 AND \"Iso3\" ~ '^[A-Z]{3}$'");
        });

        entity.Ignore(x => x.Id);

        entity.HasKey(x => x.Iso2);

        entity.Property(x => x.Iso2)
            .ValueGeneratedNever();

        entity.Property(x => x.Iso2)
            .HasMaxLength(2)
            .IsFixedLength()
            .IsRequired();

        entity.Property(x => x.Iso3)
            .HasMaxLength(3)
            .IsFixedLength()
            .IsRequired();

        entity.Property(x => x.IsoNumeric)
            .IsRequired();

        entity.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();


        entity.HasIndex(x => x.Iso3)
            .IsUnique();

        entity.HasIndex(x => x.IsoNumeric)
            .IsUnique();

        entity.HasIndex(x => x.Name)
            .IsUnique();

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<Country> entity);
}

