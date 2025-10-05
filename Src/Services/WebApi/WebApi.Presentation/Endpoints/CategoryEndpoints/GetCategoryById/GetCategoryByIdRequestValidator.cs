using WebApi.Application.Features.CategoryFeatures.GetCategoryById;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.CategoryEndpoints.GetCategoryById;

public class GetCategoryByIdRequestValidator : Validator<GetCategoryByIdRequest>
{
    public GetCategoryByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
