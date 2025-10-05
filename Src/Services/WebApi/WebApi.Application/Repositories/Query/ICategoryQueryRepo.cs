using Domain.Core.Entities;
using Repository.Base;

namespace WebApi.Application.Repositories.Query;
public interface ICategoryQueryRepo : ISecureQueryRepo
{
    IQueryable<Category> Categories { get; }
}
