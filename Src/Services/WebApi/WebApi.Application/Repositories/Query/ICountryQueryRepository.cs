using Domain.Core.Entities;
using Repository.Base;

namespace WebApi.Application.Repositories.Query;
public interface ICountryQueryRepository : IQueryRepo
{
    IQueryable<Country> Countries { get; }
}
