using Domain.Core.Enums;
using Pagination.Models.Requests;
using Pagination.Models.Responses;

namespace WebApi.Application.Features.AccountFeatures.Queries.SearchAccounts;
public sealed class SearchAccountsRequest : PageableRequest, IQuery<PageableResponse<SearchAccountsResponse>>
{
    public string? SearchTerms { get; init; }

    public AccountTypeEnum? AccountType { get; init; }
}
