using WebApi.Application.Features.AccountFeatures.SearchAccounts;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.AccountEndpoints.SearchAccounts;

public class SearchAccountsRequestValidator : Validator<SearchAccountsRequest>
{
    public SearchAccountsRequestValidator()
    {
        this.ValidatePageableRequest();
    }
}
