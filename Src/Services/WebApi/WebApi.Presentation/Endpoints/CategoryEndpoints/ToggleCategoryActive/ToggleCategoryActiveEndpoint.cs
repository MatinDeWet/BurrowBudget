using CQRS.Contracts;
using WebApi.Application.Features.CategoryFeatures.Commands.ToggleCategoryActive;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryEndpoints.ToggleCategoryActive;

public class ToggleCategoryActiveEndpoint(ICommandManager<ToggleCategoryActiveRequest> manager)
    : CommandEndpoint<ToggleCategoryActiveRequest>(manager)
{
    public override void Configure()
    {
        Patch("category/{Id:guid}/toggle-active");
        Summary(x =>
        {
            x.Summary = "Toggle category active state";
            x.Description = "Activates or deactivates a category";
        });
    }
}
