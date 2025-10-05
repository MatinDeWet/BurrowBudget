using WebApi.Application.Features.CategoryGroupFeatures.GetCategoryGroupCategories;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.GetCategoryGroupCategories;

public class GetCategoryGroupCategoriesRequestValidator : Validator<GetCategoryGroupCategoriesRequest>
{
    public GetCategoryGroupCategoriesRequestValidator()
    {
        this.ValidatePageableRequest();

        RuleFor(x => x.CategoryGroupId)
            .NotEmpty();
    }
}
