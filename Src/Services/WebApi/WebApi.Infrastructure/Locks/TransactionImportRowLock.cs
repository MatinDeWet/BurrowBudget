using Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Core.Enums;
using Repository.Core.Lock;
using WebApi.Infrastructure.Data.Contexts;

namespace WebApi.Infrastructure.Locks;
internal sealed class TransactionImportRowLock(BudgetContext context) : Lock<TransactionImportRow>
{
    public override async Task<bool> HasAccess(TransactionImportRow obj, Guid identityId, RepositoryOperationEnum operation, CancellationToken cancellationToken)
    {
        Guid importBatchId = obj.ImportBatchId;

        if (importBatchId == Guid.Empty)
        {
            return false;
        }

        Guid userId = await (from batch in context.Set<TransactionImportBatch>()
                            join account in context.Set<Account>() on batch.AccountId equals account.Id
                            where batch.Id == importBatchId
                            select account.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        return userId == identityId;
    }

    public override IQueryable<TransactionImportRow> Secured(Guid identityId)
    {
        return from row in context.Set<TransactionImportRow>()
               join batch in context.Set<TransactionImportBatch>() on row.ImportBatchId equals batch.Id
               join account in context.Set<Account>() on batch.AccountId equals account.Id
               where account.UserId == identityId
               select row;
    }
}
