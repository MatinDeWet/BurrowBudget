using Domain.Core.Entities;
using Repository.Base;

namespace WebApi.Application.Repositories.Query;
public interface IUserQueryRepo : ISecureQueryRepo
{
    IQueryable<User> Users { get; }
}
