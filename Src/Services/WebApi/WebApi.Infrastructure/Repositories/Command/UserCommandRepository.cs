using Identification.Base;
using Repository.Core.Contracts;
using Repository.Core.Implementation;
using WebApi.Application.Repositories.Command;
using WebApi.Infrastructure.Data.Contexts;

namespace WebApi.Infrastructure.Repositories.Command;
internal sealed class UserCommandRepository : SecureCommandRepo<BudgetContext>, IUserCommandRepository
{
    public UserCommandRepository(BudgetContext context, IIdentityInfo info, IEnumerable<IProtected> protection)
        : base(context, info, protection)
    {
    }
}
