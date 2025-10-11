using Ardalis.Result;
using Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Command;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.AccountFeatures.Commands.DeleteAccount;
internal sealed class DeleteAccountHandler(IAccountQueryRepo queryRepo, IAccountCommandRepo commandRepo)
    : ICommandManager<DeleteAccountRequest>
{
    public async Task<Result> Handle(DeleteAccountRequest command, CancellationToken cancellationToken)
    {
        Account? account = await queryRepo.Accounts.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (account is null)
        {
            return Result.NotFound($"Account with Id {command.Id} was not found.");
        }

        await commandRepo.DeleteAsync(account, true, cancellationToken);

        return Result.Success();
    }
}
