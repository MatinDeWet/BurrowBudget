using CQRS.Contracts;
using Pagination.Models.Responses;
using WebApi.Application.Features.CategoryGroupFeatures.SearchCategoryGroups;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.SearchCategoryGroups;

public class SearchCategoryGroupsEndpoint(IQueryManager<SearchCategoryGroupsRequest, PageableResponse<SearchCategoryGroupsResponse>> manager)
    : QueryEndpoint<SearchCategoryGroupsRequest, PageableResponse<SearchCategoryGroupsResponse>>(manager)
{
    public override void Configure()
    {
        Get("category-group/search");
        Summary(x =>
        {
            x.Summary = "Search category groups";
            x.Description = "Search and filter category groups with pagination support";
        });
    }
}
