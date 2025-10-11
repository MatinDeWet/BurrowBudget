using Pagination.Models.Requests;
using Pagination.Models.Responses;

namespace WebApi.Application.Features.CategoryGroupFeatures.Queries.GetCategoryGroupCategories;
public sealed class GetCategoryGroupCategoriesRequest : PageableRequest, IQuery<PageableResponse<GetCategoryGroupCategoriesResponse>>
{
    public Guid CategoryGroupId { get; init; }

    public string? SearchTerms { get; init; }

    public bool? IsActive { get; init; }
}
