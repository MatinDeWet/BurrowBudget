using CQRS.Contracts;
using WebApi.Application.Features.CategoryGroupFeatures.ToggleCategoryGroupActive;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.ToggleCategoryGroupActive;

public class ToggleCategoryGroupActiveEndpoint(ICommandManager<ToggleCategoryGroupActiveRequest> manager)
    : CommandEndpoint<ToggleCategoryGroupActiveRequest>(manager)
{
    public override void Configure()
    {
        Patch("category-group/{Id:guid}/toggle-active");
        Summary(x =>
        {
            x.Summary = "Toggle category group active state";
            x.Description = "Activates or deactivates a category group";
        });
    }
}
