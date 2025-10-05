using Repository.Base;
using WebApi.Domain.Entities;

namespace WebApi.Application.Repositories.Query;
public interface IUserRefreshTokenQueryRepo : IQueryRepo
{
    IQueryable<UserRefreshToken> UserRefreshTokens { get; }

    Task<bool> IsValidToken(Guid userId, string token);
}
