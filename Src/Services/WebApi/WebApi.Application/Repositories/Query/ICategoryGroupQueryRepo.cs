using Domain.Core.Entities;
using Repository.Base;

namespace WebApi.Application.Repositories.Query;
public interface ICategoryGroupQueryRepo : ISecureQueryRepo
{
    IQueryable<CategoryGroup> CategoryGroups { get; }
}
