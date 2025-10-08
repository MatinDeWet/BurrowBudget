using Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Core.Enums;
using Repository.Core.Lock;
using WebApi.Infrastructure.Data.Contexts;

namespace WebApi.Infrastructure.Locks;
internal sealed class TransactionImportBatchLock(BudgetContext context) : Lock<TransactionImportBatch>
{
    public override async Task<bool> HasAccess(TransactionImportBatch obj, Guid identityId, RepositoryOperationEnum operation, CancellationToken cancellationToken)
    {
        Guid accountId = obj.AccountId;

        if (accountId == Guid.Empty)
        {
            return false;
        }

        Guid userId = await context.Set<Account>()
            .Where(a => a.Id == accountId)
            .Select(a => a.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        return userId == identityId;
    }

    public override IQueryable<TransactionImportBatch> Secured(Guid identityId)
    {
        return from batch in context.Set<TransactionImportBatch>()
               join account in context.Set<Account>() on batch.AccountId equals account.Id
               where account.UserId == identityId
               select batch;
    }
}
