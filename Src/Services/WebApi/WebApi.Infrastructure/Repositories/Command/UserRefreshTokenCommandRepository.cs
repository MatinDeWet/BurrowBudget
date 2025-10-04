using Microsoft.EntityFrameworkCore;
using Repository.Core.Implementation;
using WebApi.Application.Repositories.Command;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Data.Contexts;

namespace WebApi.Infrastructure.Repositories.Command;
internal sealed class UserRefreshTokenCommandRepository : CommandRepo<BudgetContext>, IUserRefreshTokenCommandRepository
{
    public UserRefreshTokenCommandRepository(BudgetContext context) : base(context)
    {
    }

    public async Task CreateAndResetToken(Guid userId, string token, DateTime expirationDate)
    {
        var refreshToken = UserRefreshToken.Create(userId, token, expirationDate);

        List<UserRefreshToken> existingTokens = await _context.Set<UserRefreshToken>()
            .Where(x => x.UserID == userId)
            .ToListAsync();

        foreach (UserRefreshToken existingToken in existingTokens)
        {
            await DeleteAsync(existingToken, CancellationToken.None);
        }

        if (existingTokens.Any())
        {
            await SaveAsync(CancellationToken.None);
        }

        await InsertAsync(refreshToken, true, CancellationToken.None);
    }
}
