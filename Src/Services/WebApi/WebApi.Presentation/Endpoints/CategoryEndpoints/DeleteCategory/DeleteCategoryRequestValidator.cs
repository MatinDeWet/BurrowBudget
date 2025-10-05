using WebApi.Application.Features.CategoryFeatures.DeleteCategory;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.CategoryEndpoints.DeleteCategory;

public class DeleteCategoryRequestValidator : Validator<DeleteCategoryRequest>
{
    public DeleteCategoryRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
