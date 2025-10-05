using CQRS.Contracts;
using WebApi.Application.Features.CategoryFeatures.UpdateCategory;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryEndpoints.UpdateCategory;

public class UpdateCategoryEndpoint(ICommandManager<UpdateCategoryRequest> manager)
    : CommandEndpoint<UpdateCategoryRequest>(manager)
{
    public override void Configure()
    {
        Put("category/{Id:guid}");
        Summary(x =>
        {
            x.Summary = "Update a category";
            x.Description = "Updates an existing category";
        });
    }
}
