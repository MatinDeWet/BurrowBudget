using WebApi.Application.Features.AccountFeatures.CreateAccount;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.AccountEndpoints.CreateAccount;

public class CreateAccountRequestValidator : Validator<CreateAccountRequest>
{
    public CreateAccountRequestValidator()
    {
        RuleFor(x => x.Name)
            .StringInput(32);

        RuleFor(x => x.Description)
            .StringInput(256, false);

        RuleFor(x => x.AccountType)
            .IsInEnum();
    }
}
