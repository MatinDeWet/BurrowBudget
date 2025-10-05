using CQRS.Contracts;
using Pagination.Models.Responses;
using WebApi.Application.Features.CategoryGroupFeatures.GetCategoryGroupCategories;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.GetCategoryGroupCategories;

public class GetCategoryGroupCategoriesEndpoint(IQueryManager<GetCategoryGroupCategoriesRequest, PageableResponse<GetCategoryGroupCategoriesResponse>> manager)
    : QueryEndpoint<GetCategoryGroupCategoriesRequest, PageableResponse<GetCategoryGroupCategoriesResponse>>(manager)
{
    public override void Configure()
    {
        Get("category-group/{CategoryGroupId:guid}/categories");
        Summary(x =>
        {
            x.Summary = "Get category group categories";
            x.Description = "Retrieves all categories belonging to a specific category group with pagination support";
        });
    }
}
