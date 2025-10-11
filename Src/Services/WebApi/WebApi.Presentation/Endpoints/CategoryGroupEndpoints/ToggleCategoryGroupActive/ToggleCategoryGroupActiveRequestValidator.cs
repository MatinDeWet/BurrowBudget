using WebApi.Application.Features.CategoryGroupFeatures.Commands.ToggleCategoryGroupActive;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.ToggleCategoryGroupActive;

public class ToggleCategoryGroupActiveRequestValidator : Validator<ToggleCategoryGroupActiveRequest>
{
    public ToggleCategoryGroupActiveRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
