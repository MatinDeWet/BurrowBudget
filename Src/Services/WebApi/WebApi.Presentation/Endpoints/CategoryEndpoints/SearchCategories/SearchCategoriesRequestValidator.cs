using WebApi.Application.Features.CategoryFeatures.Queries.SearchCategories;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.CategoryEndpoints.SearchCategories;

public class SearchCategoriesRequestValidator : Validator<SearchCategoriesRequest>
{
    public SearchCategoriesRequestValidator()
    {
        this.ValidatePageableRequest();
    }
}
