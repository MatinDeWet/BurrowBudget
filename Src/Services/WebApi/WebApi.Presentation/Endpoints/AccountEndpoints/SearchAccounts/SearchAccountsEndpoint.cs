using CQRS.Contracts;
using Pagination.Models.Responses;
using WebApi.Application.Features.AccountFeatures.Queries.SearchAccounts;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.AccountEndpoints.SearchAccounts;

public class SearchAccountsEndpoint(IQueryManager<SearchAccountsRequest, PageableResponse<SearchAccountsResponse>> manager)
    : QueryEndpoint<SearchAccountsRequest, PageableResponse<SearchAccountsResponse>>(manager)
{
    public override void Configure()
    {
        Get("account/search");
        Summary(x =>
        {
            x.Summary = "Search accounts";
            x.Description = "Search and filter accounts with pagination support";
        });
    }
}
