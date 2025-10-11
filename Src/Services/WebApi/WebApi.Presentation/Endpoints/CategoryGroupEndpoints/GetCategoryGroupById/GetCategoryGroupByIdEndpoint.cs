using CQRS.Contracts;
using WebApi.Application.Features.CategoryGroupFeatures.Queries.GetCategoryGroupById;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.GetCategoryGroupById;

public class GetCategoryGroupByIdEndpoint(IQueryManager<GetCategoryGroupByIdRequest, GetCategoryGroupByIdResponse> manager)
    : QueryEndpoint<GetCategoryGroupByIdRequest, GetCategoryGroupByIdResponse>(manager)
{
    public override void Configure()
    {
        Get("category-group/{Id:guid}");
        Summary(x =>
        {
            x.Summary = "Get category group by ID";
            x.Description = "Retrieves a single category group by its ID";
        });
    }
}
