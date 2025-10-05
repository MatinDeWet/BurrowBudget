using WebApi.Application.Features.CategoryFeatures.UpdateCategory;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.CategoryEndpoints.UpdateCategory;

public class UpdateCategoryRequestValidator : Validator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .StringInput(32);

        RuleFor(x => x.Description)
            .StringInput(256);

        RuleFor(x => x.SortOrder)
            .NotEmpty();
    }
}
