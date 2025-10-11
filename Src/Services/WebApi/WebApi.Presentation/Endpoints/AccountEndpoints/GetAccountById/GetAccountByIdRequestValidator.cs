using WebApi.Application.Features.AccountFeatures.Queries.GetAccountById;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.AccountEndpoints.GetAccountById;

public class GetAccountByIdRequestValidator : Validator<GetAccountByIdRequest>
{
    public GetAccountByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
