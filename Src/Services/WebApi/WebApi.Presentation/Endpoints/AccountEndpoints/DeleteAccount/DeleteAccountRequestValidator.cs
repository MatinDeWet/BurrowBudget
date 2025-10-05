using WebApi.Application.Features.AccountFeatures.DeleteAccount;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.AccountEndpoints.DeleteAccount;

public class DeleteAccountRequestValidator : Validator<DeleteAccountRequest>
{
    public DeleteAccountRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
