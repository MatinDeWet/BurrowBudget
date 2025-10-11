using CQRS.Contracts;
using WebApi.Application.Features.CategoryGroupFeatures.Commands.UpdateCategoryGroup;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.UpdateCategoryGroup;

public class UpdateCategoryGroupEndpoint(ICommandManager<UpdateCategoryGroupRequest> manager)
    : CommandEndpoint<UpdateCategoryGroupRequest>(manager)
{
    public override void Configure()
    {
        Put("category-group/{Id:guid}");
        Summary(x =>
        {
            x.Summary = "Update a category group";
            x.Description = "Updates an existing category group";
        });
    }
}
