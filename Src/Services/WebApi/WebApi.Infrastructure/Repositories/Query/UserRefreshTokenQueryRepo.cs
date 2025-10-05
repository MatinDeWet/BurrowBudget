using Microsoft.EntityFrameworkCore;
using Repository.Core.Implementation;
using WebApi.Application.Repositories.Query;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Data.Contexts;

namespace WebApi.Infrastructure.Repositories.Query;
internal sealed class UserRefreshTokenQueryRepo : QueryRepo<BudgetContext>, IUserRefreshTokenQueryRepo
{
    public UserRefreshTokenQueryRepo(BudgetContext context) : base(context)
    {
    }

    public IQueryable<UserRefreshToken> UserRefreshTokens => _context.Set<UserRefreshToken>();

    public async Task<bool> IsValidToken(Guid userId, string token)
    {
        bool isValid = await UserRefreshTokens
            .Where(x => x.UserID == userId
                && x.Token == token
                && x.ExpiryDate >= DateTime.UtcNow
                )
            .AnyAsync();

        return isValid;
    }
}
