using WebApi.Application.Features.CategoryGroupFeatures.GetCategoryGroupById;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.GetCategoryGroupById;

public class GetCategoryGroupByIdRequestValidator : Validator<GetCategoryGroupByIdRequest>
{
    public GetCategoryGroupByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
