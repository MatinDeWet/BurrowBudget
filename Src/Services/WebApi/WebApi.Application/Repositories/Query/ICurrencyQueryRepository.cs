using Domain.Core.Entities;
using Repository.Base;

namespace WebApi.Application.Repositories.Query;
public interface ICurrencyQueryRepository : IQueryRepo
{
    IQueryable<Currency> Currencies { get; }
}
