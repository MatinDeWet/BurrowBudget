using CQRS.Contracts;
using WebApi.Application.Features.CategoryFeatures.DeleteCategory;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryEndpoints.DeleteCategory;

public class DeleteCategoryEndpoint(ICommandManager<DeleteCategoryRequest> manager)
    : CommandEndpoint<DeleteCategoryRequest>(manager)
{
    public override void Configure()
    {
        Delete("category/{Id:guid}");
        Summary(x =>
        {
            x.Summary = "Delete a category";
            x.Description = "Deletes a category by its ID";
        });
    }
}
