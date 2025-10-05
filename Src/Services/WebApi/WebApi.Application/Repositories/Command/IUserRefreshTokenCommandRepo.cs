using Repository.Base;

namespace WebApi.Application.Repositories.Command;
public interface IUserRefreshTokenCommandRepo : ICommandRepo
{
    Task CreateAndResetToken(Guid userId, string token, DateTime expirationDate);
}

