using Domain.Core.Entities;
using Repository.Base;

namespace WebApi.Application.Repositories.Query;
public interface IAccountQueryRepo : ISecureQueryRepo
{
    IQueryable<Account> Accounts { get; }
}
