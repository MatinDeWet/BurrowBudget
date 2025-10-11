using Domain.Core.Enums;

namespace WebApi.Application.Features.AccountFeatures.Queries.GetAccountById;
public sealed record GetAccountByIdResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public string? Description { get; init; }

    public AccountTypeEnum AccountType { get; init; }
}
