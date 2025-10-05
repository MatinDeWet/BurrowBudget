using Ardalis.Result;
using Domain.Core.Entities;
using Pagination;
using Pagination.Enums;
using Pagination.Models.Responses;
using Searchable.Core;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.AccountFeatures.SearchAccounts;
internal sealed class SearchAccountsHandler(IAccountQueryRepo queryRepo)
    : IQueryManager<SearchAccountsRequest, PageableResponse<SearchAccountsResponse>>
{
    public async Task<Result<PageableResponse<SearchAccountsResponse>>> Handle(SearchAccountsRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Account> query = queryRepo.Accounts;

        if (request.AccountType.HasValue)
        {
            query = query.Where(a => a.AccountType == request.AccountType.Value);
        }

        query = query.FullTextSearch(request.SearchTerms);

        PageableResponse<SearchAccountsResponse> response = await query
            .Select(a => new SearchAccountsResponse
            {
                Id = a.Id,
                Name = a.Name,
                AccountType = a.AccountType,
            })
            .ToPageableListAsync(x => x.Name, OrderDirectionEnum.Ascending, request, cancellationToken);

        return response;
    }
}
