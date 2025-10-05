using WebApi.Application.Features.CategoryGroupFeatures.DeleteCategoryGroup;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.DeleteCategoryGroup;

public class DeleteCategoryGroupRequestValidator : Validator<DeleteCategoryGroupRequest>
{
    public DeleteCategoryGroupRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
