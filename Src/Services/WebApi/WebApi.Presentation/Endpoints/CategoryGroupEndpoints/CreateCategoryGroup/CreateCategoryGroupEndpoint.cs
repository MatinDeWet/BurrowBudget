using CQRS.Contracts;
using WebApi.Application.Features.CategoryGroupFeatures.Commands.CreateCategoryGroup;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.CreateCategoryGroup;

public class CreateCategoryGroupEndpoint(ICommandManager<CreateCategoryGroupRequest, Guid> manager)
    : CommandEndpoint<CreateCategoryGroupRequest, Guid>(manager)
{
    public override void Configure()
    {
        Post("category-group");
        Summary(x =>
        {
            x.Summary = "Create a new category group";
            x.Description = "Creates a new category group";
        });
    }
}
