using Repository.Core.Implementation;
using WebApi.Application.Repositories.Command;
using WebApi.Infrastructure.Data.Contexts;

namespace WebApi.Infrastructure.Repositories.Command;
internal sealed class CountryCommandRepository : CommandRepo<BudgetContext>, ICountryCommandRepository
{
    public CountryCommandRepository(BudgetContext context) : base(context)
    {
    }
}
