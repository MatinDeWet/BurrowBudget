using Ardalis.Result;
using Domain.Core.Entities;
using Identification.Base;
using WebApi.Application.Repositories.Command;

namespace WebApi.Application.Features.AccountFeatures.Commands.CreateAccount;
internal sealed class CreateAccountHandler(IAccountCommandRepo commandRepo, IIdentityInfo identityInfo)
    : ICommandManager<CreateAccountRequest, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAccountRequest command, CancellationToken cancellationToken)
    {
        var account = Account.Create(
            identityInfo.GetIdentityId(),
            command.Name,
            command.AccountType,
            command.Description);

        await commandRepo.InsertAsync(account, true, cancellationToken);

        return account.Id;
    }
}
