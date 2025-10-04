using Repository.Base;

namespace WebApi.Application.Repositories.Command;
public interface IUserRefreshTokenCommandRepository : ICommandRepo
{
    Task CreateAndResetToken(Guid userId, string token, DateTime expirationDate);
}

