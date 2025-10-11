using CQRS.Contracts;
using WebApi.Application.Features.CategoryGroupFeatures.Commands.DeleteCategoryGroup;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.DeleteCategoryGroup;

public class DeleteCategoryGroupEndpoint(ICommandManager<DeleteCategoryGroupRequest> manager)
    : CommandEndpoint<DeleteCategoryGroupRequest>(manager)
{
    public override void Configure()
    {
        Delete("category-group/{Id:guid}");
        Summary(x =>
        {
            x.Summary = "Delete a category group";
            x.Description = "Deletes a category group by its ID";
        });
    }
}
