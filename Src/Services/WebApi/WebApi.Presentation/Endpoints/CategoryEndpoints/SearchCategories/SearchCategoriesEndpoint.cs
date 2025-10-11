using CQRS.Contracts;
using Pagination.Models.Responses;
using WebApi.Application.Features.CategoryFeatures.Queries.SearchCategories;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryEndpoints.SearchCategories;

public class SearchCategoriesEndpoint(IQueryManager<SearchCategoriesRequest, PageableResponse<SearchCategoriesResponse>> manager)
    : QueryEndpoint<SearchCategoriesRequest, PageableResponse<SearchCategoriesResponse>>(manager)
{
    public override void Configure()
    {
        Get("category/search");
        Summary(x =>
        {
            x.Summary = "Search categories";
            x.Description = "Search and filter categories with pagination support";
        });
    }
}
