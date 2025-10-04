using Domain.Core.Entities;
using Repository.Core.Implementation;
using WebApi.Application.Repositories.Query;
using WebApi.Infrastructure.Data.Contexts;

namespace WebApi.Infrastructure.Repositories.Query;
internal sealed class CurrencyQueryRepository : QueryRepo<BudgetContext>, ICurrencyQueryRepository
{
    public CurrencyQueryRepository(BudgetContext context) : base(context)
    {
    }

    public IQueryable<Currency> Currencies => GetQueryable<Currency>();
}
