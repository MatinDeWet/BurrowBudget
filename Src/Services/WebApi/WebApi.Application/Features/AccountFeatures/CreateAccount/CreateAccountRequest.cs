using Domain.Core.Enums;

namespace WebApi.Application.Features.AccountFeatures.CreateAccount;
public sealed record CreateAccountRequest : ICommand<Guid>
{
    public string Name { get; init; }

    public string? Description { get; init; }

    public AccountTypeEnum AccountType { get; init; }
}
