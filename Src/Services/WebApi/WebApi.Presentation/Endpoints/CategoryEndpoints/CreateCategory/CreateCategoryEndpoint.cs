using CQRS.Contracts;
using WebApi.Application.Features.CategoryFeatures.CreateCategoryHandler;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryEndpoints.CreateCategory;

public class CreateCategoryEndpoint(ICommandManager<CreateCategoryRequest, Guid> manager)
    : CommandEndpoint<CreateCategoryRequest, Guid>(manager)
{
    public override void Configure()
    {
        Post("category");
        Summary(x =>
        {
            x.Summary = "Create a new category";
            x.Description = "Creates a new category within the specified category group";
        });
    }
}
