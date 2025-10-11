using Pagination.Models.Requests;
using Pagination.Models.Responses;

namespace WebApi.Application.Features.CategoryGroupFeatures.Queries.SearchCategoryGroups;
public sealed class SearchCategoryGroupsRequest : PageableRequest, IQuery<PageableResponse<SearchCategoryGroupsResponse>>
{
    public string? SearchTerms { get; init; }

    public bool? IsActive { get; init; }
}
