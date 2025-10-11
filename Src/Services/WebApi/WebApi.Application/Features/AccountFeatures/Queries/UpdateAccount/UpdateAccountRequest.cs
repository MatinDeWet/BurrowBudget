using Domain.Core.Enums;

namespace WebApi.Application.Features.AccountFeatures.Queries.UpdateAccount;
public sealed record UpdateAccountRequest : ICommand
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public string? Description { get; init; }

    public AccountTypeEnum AccountType { get; init; }
}
