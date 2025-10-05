using Domain.Core.Entities;
using Identification.Base;
using Repository.Core.Contracts;
using Repository.Core.Implementation;
using WebApi.Application.Repositories.Query;
using WebApi.Infrastructure.Data.Contexts;

namespace WebApi.Infrastructure.Repositories.Query;
internal sealed class CategoryGroupQueryRepo : SecureQueryRepo<BudgetContext>, ICategoryGroupQueryRepo
{
    public CategoryGroupQueryRepo(BudgetContext context, IIdentityInfo info, IEnumerable<IProtected> protection) : base(context, info, protection)
    {
    }

    public IQueryable<CategoryGroup> CategoryGroups => Secure<CategoryGroup>();
}
