using WebApi.Application.Features.CategoryGroupFeatures.SearchCategoryGroups;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.CategoryGroupEndpoints.SearchCategoryGroups;

public class SearchCategoryGroupsRequestValidator : Validator<SearchCategoryGroupsRequest>
{
    public SearchCategoryGroupsRequestValidator()
    {
        this.ValidatePageableRequest();
    }
}
