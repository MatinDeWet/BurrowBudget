using CQRS.Contracts;
using WebApi.Application.Features.CategoryFeatures.ChangeCategoryGroup;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.CategoryEndpoints.ChangeCategoryGroup;

public class ChangeCategoryGroupEndpoint(ICommandManager<ChangeCategoryGroupRequest> manager)
    : CommandEndpoint<ChangeCategoryGroupRequest>(manager)
{
    public override void Configure()
    {
        Patch("category/{CategoryId:guid}/change-group");
        Summary(x =>
        {
            x.Summary = "Change category group";
            x.Description = "Moves a category to a different category group";
        });
    }
}
