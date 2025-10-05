using Domain.Core.Entities;
using Repository.Core.Enums;
using Repository.Core.Lock;
using WebApi.Infrastructure.Data.Contexts;

namespace WebApi.Infrastructure.Locks;
internal sealed class AccountLock(BudgetContext context) : Lock<Account>
{
    public override async Task<bool> HasAccess(Account obj, Guid identityId, RepositoryOperationEnum operation, CancellationToken cancellationToken)
    {
        Guid userId = obj.UserId;

        if (userId == Guid.Empty)
        {
            return false;
        }

        return await Task.FromResult(userId == identityId);
    }

    public override IQueryable<Account> Secured(Guid identityId)
    {
        return from a in context.Set<Account>()
               where a.UserId == identityId
               select a;
    }
}
