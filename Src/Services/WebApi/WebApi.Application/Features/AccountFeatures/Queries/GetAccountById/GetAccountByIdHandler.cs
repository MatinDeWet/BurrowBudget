using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.AccountFeatures.Queries.GetAccountById;
internal sealed class GetAccountByIdHandler(IAccountQueryRepo queryRepo)
    : IQueryManager<GetAccountByIdRequest, GetAccountByIdResponse>
{
    public async Task<Result<GetAccountByIdResponse>> Handle(GetAccountByIdRequest query, CancellationToken cancellationToken)
    {
        GetAccountByIdResponse? account = await queryRepo.Accounts
            .Where(x => x.Id == query.Id)
            .Select(x => new GetAccountByIdResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                AccountType = x.AccountType,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (account is null)
        {
            return Result.NotFound($"Account with Id {query.Id} was not found.");
        }

        return account;
    }
}
