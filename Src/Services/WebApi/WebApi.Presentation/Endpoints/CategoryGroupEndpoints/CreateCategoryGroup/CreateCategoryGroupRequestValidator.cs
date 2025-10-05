using WebApi.Application.Features.CategoryGroupFeatures.CreateCategoryGroup;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.CreateCategoryGroup;

public class CreateCategoryGroupRequestValidator : Validator<CreateCategoryGroupRequest>
{
    public CreateCategoryGroupRequestValidator()
    {
        RuleFor(x => x.Name)
            .StringInput(32);

        RuleFor(x => x.Description)
            .StringInput(256, false);

        RuleFor(x => x.SortOrder)
            .NotEmpty();

        RuleFor(x => x.IsActive)
            .NotEmpty();
    }
}
