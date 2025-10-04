using Ardalis.GuardClauses;
using Domain.Base.Extensions;
using Domain.Base.Implementation;

namespace Domain.Core.Entities;
public class Currency : Entity<string>
{
    public string Code
    {
        get => base.Id;
        private set => base.Id = value;
    }
    public string Name { get; private set; }

    public string? Symbol { get; private set; }

    public byte MinorUnits { get; private set; } = 2;

    public bool IsActive { get; private set; } = true;

    public virtual ICollection<CountryCurrency> Countries { get; init; } = [];

    public static Currency Create(
        string code,
        string name,
        string? symbol = null,
        byte minorUnits = 2,
        bool isActive = true)
    {
        return new Currency
        {
            Code = Guard.Against.ValidCurrencyCode(code, nameof(code)),
            Name = Guard.Against.ValidCurrencyName(name, nameof(name)),
            Symbol = Guard.Against.ValidCurrencySymbol(symbol, nameof(symbol)),
            MinorUnits = Guard.Against.Default(minorUnits),
            IsActive = isActive
        };
    }

    public void Update(
        string code,
        string name,
        string? symbol = null,
        byte minorUnits = 2)
    {
        Code = Guard.Against.ValidCurrencyCode(code, nameof(code));
        Name = Guard.Against.ValidCurrencyName(name, nameof(name));
        Symbol = Guard.Against.ValidCurrencySymbol(symbol, nameof(symbol));
        MinorUnits = Guard.Against.Default(minorUnits);
    }

    public void ToggleActive(bool? isActive = null)
    {
        if (isActive.HasValue)
        {
            IsActive = isActive.Value;
            return;
        }

        IsActive = !IsActive;
    }
}
