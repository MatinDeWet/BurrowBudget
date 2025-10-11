using WebApi.Application.Features.CategoryGroupFeatures.Queries.GetCategoryGroupCategoryCount;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.GetCategoryGroupCategoryCount;

public class GetCategoryGroupCategoryCountRequestValidator : Validator<GetCategoryGroupCategoryCountRequest>
{
    public GetCategoryGroupCategoryCountRequestValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();
    }
}
