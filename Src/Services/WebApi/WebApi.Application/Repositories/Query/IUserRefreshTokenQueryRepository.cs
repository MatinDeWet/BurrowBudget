using Repository.Base;
using WebApi.Domain.Entities;

namespace WebApi.Application.Repositories.Query;
public interface IUserRefreshTokenQueryRepository : IQueryRepo
{
    IQueryable<UserRefreshToken> UserRefreshTokens { get; }

    Task<bool> IsValidToken(Guid userId, string token);
}
