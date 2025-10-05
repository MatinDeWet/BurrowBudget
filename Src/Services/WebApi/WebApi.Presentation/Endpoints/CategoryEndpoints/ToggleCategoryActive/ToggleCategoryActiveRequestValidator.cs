using WebApi.Application.Features.CategoryFeatures.ToggleCategoryActive;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.CategoryEndpoints.ToggleCategoryActive;

public class ToggleCategoryActiveRequestValidator : Validator<ToggleCategoryActiveRequest>
{
    public ToggleCategoryActiveRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
