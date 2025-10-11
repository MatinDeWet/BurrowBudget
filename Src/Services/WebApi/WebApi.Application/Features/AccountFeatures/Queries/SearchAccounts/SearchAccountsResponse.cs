using Domain.Core.Enums;

namespace WebApi.Application.Features.AccountFeatures.Queries.SearchAccounts;
public sealed record SearchAccountsResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public AccountTypeEnum AccountType { get; init; }
}
