using Pagination.Models.Requests;
using Pagination.Models.Responses;

namespace WebApi.Application.Features.CategoryFeatures.SearchCategories;
public sealed class SearchCategoriesRequest : PageableRequest, IQuery<PageableResponse<SearchCategoriesResponse>>
{
    public string? SearchTerms { get; init; }

    public Guid? CategoryGroupId { get; init; }

    public bool? IsActive { get; init; }
}
