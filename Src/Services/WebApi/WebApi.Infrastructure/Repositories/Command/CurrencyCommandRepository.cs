using Repository.Core.Implementation;
using WebApi.Application.Repositories.Command;
using WebApi.Infrastructure.Data.Contexts;

namespace WebApi.Infrastructure.Repositories.Command;
internal sealed class CurrencyCommandRepository : CommandRepo<BudgetContext>, ICurrencyCommandRepository
{
    public CurrencyCommandRepository(BudgetContext context) : base(context)
    {
    }
}
