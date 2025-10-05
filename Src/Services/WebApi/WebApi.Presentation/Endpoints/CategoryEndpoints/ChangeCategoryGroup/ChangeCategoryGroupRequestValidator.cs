using WebApi.Application.Features.CategoryFeatures.ChangeCategoryGroup;

namespace WebApi.Presentation.Endpoints.CategoryEndpoints.ChangeCategoryGroup;

public class ChangeCategoryGroupRequestValidator : Validator<ChangeCategoryGroupRequest>
{
    public ChangeCategoryGroupRequestValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.CategoryGroupId)
            .NotEmpty();
    }
}
