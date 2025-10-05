using Ardalis.Result;
using Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Command;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.AccountFeatures.UpdateAccount;
internal sealed class UpdateAccountHandler(IAccountQueryRepo queryRepo, IAccountCommandRepo commandRepo)
    : ICommandManager<UpdateAccountRequest>
{
    public async Task<Result> Handle(UpdateAccountRequest command, CancellationToken cancellationToken)
    {
        Account? account = await queryRepo.Accounts.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (account is null)
        {
            return Result.NotFound($"Account with Id {command.Id} was not found.");
        }

        account.Update(
            command.Name,
            command.AccountType,
            command.Description);

        await commandRepo.UpdateAsync(account, true, cancellationToken);

        return Result.Success();
    }
}
