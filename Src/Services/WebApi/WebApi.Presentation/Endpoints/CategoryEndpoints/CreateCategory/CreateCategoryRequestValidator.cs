using WebApi.Application.Features.CategoryFeatures.CreateCategory;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.CategoryEndpoints.CreateCategory;

public class CreateCategoryRequestValidator : Validator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.CategoryGroupId)
              .NotEmpty();

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
