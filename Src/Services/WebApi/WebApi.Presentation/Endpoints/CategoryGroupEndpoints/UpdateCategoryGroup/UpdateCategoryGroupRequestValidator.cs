using WebApi.Application.Features.CategoryGroupFeatures.Commands.UpdateCategoryGroup;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.UpdateCategoryGroup;

public class UpdateCategoryGroupRequestValidator : Validator<UpdateCategoryGroupRequest>
{
    public UpdateCategoryGroupRequestValidator()
    {
        RuleFor(x => x.Name)
            .StringInput(32);

        RuleFor(x => x.Description)
            .StringInput(256, false);

        RuleFor(x => x.Description)
            .StringInput(256);

        RuleFor(x => x.SortOrder)
            .NotEmpty();
    }
}
