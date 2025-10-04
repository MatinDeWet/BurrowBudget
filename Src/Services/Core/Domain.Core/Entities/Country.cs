using Ardalis.GuardClauses;
using Domain.Base.Extensions;
using Domain.Base.Implementation;

namespace Domain.Core.Entities;
public class Country : Entity<string>
{
    public string Iso2
    {
        get => base.Id;
        private set => base.Id = value;
    }

    public string Iso3 { get; private set; }

    public short IsoNumeric { get; private set; }

    public string Name { get; private set; }

    public bool IsActive { get; private set; } = true;

    public virtual ICollection<CountryCurrency> Currencies { get; private set; } = [];

    public static Country Create(
        string iso2,
        string iso3,
        short isoNumeric,
        string name,
        bool isActive = true)
    {
        return new Country
        {
            Iso2 = Guard.Against.ValidIsoCode(iso2, nameof(iso2), 2),
            Iso3 = Guard.Against.ValidIsoCode(iso3, nameof(iso3), 3),
            IsoNumeric = Guard.Against.Default(isoNumeric),
            Name = Guard.Against.ValidCountryName(name, nameof(name)),
            IsActive = isActive
        };
    }

    public void Update(
        string iso2,
        string iso3,
        short isoNumeric,
        string name)
    {
        Iso2 = Guard.Against.ValidIsoCode(iso2, nameof(iso2), 2);
        Iso3 = Guard.Against.ValidIsoCode(iso3, nameof(iso3), 3);
        IsoNumeric = Guard.Against.Default(isoNumeric);
        Name = Guard.Against.ValidCountryName(name, nameof(name));
    }

    public void ToggleActive(bool? isActive)
    {
        if (isActive.HasValue)
        {
            IsActive = isActive.Value;
        }

        IsActive = !IsActive;
    }
}
