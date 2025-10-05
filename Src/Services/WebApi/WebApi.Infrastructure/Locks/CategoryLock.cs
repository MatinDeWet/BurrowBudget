using Domain.Core.Entities;
using Repository.Core.Enums;
using Repository.Core.Lock;
using WebApi.Infrastructure.Data.Contexts;

namespace WebApi.Infrastructure.Locks;
internal sealed class CategoryLock(BudgetContext context) : Lock<Category>
{
    public override async Task<bool> HasAccess(Category obj, Guid identityId, RepositoryOperationEnum operation, CancellationToken cancellationToken)
    {
        Guid userId = obj.UserId;

        if (userId == Guid.Empty)
        {
            return false;
        }

        return await Task.FromResult(userId == identityId);
    }

    public override IQueryable<Category> Secured(Guid identityId)
    {
        return from c in context.Set<Category>()
               where c.UserId == identityId
               select c;
    }
}
