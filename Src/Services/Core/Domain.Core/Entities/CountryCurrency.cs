namespace Domain.Core.Entities;
public class CountryCurrency
{
    public string CountryIso2 { get; init; } = default!;
    public Country Country { get; set; } = default!;

    public string CurrencyCode { get; init; } = default!;
    public Currency Currency { get; set; } = default!;

    public DateOnly? ValidFrom { get; set; }

    public DateOnly? ValidTo { get; set; }

    public bool IsPrimary { get; set; }
}
