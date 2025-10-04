using Domain.Core.Entities;
using Repository.Core.Implementation;
using WebApi.Application.Repositories.Query;
using WebApi.Infrastructure.Data.Contexts;

namespace WebApi.Infrastructure.Repositories.Query;
internal sealed class CountryQueryRepository : QueryRepo<BudgetContext>, ICountryQueryRepository
{
    public CountryQueryRepository(BudgetContext context) : base(context)
    {
    }

    public IQueryable<Country> Countries => GetQueryable<Country>();
}
