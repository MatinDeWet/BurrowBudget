using CQRS.Contracts;
using WebApi.Application.Features.CategoryGroupFeatures.Queries.GetCategoryGroupCategoryCount;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.GetCategoryGroupCategoryCount;

public class GetCategoryGroupCategoryCountEndpoint(IQueryManager<GetCategoryGroupCategoryCountRequest, int> manager)
    : QueryEndpoint<GetCategoryGroupCategoryCountRequest, int>(manager)
{
    public override void Configure()
    {
        Get("category-group/{GroupId:guid}/category-count");
        Summary(x =>
        {
            x.Summary = "Get category group category count";
            x.Description = "Returns the total number of categories in a specific category group";
        });
    }
}
