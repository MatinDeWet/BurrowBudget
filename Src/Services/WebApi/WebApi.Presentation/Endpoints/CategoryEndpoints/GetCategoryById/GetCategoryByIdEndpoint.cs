using CQRS.Contracts;
using WebApi.Application.Features.CategoryFeatures.Queries.GetCategoryById;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryEndpoints.GetCategoryById;

public class GetCategoryByIdEndpoint(IQueryManager<GetCategoryByIdRequest, GetCategoryByIdResponse> manager)
    : QueryEndpoint<GetCategoryByIdRequest, GetCategoryByIdResponse>(manager)
{
    public override void Configure()
    {
        Get("category/{Id:guid}");
        Summary(x =>
        {
            x.Summary = "Get category by ID";
            x.Description = "Retrieves a single category by its ID";
        });
    }
}
