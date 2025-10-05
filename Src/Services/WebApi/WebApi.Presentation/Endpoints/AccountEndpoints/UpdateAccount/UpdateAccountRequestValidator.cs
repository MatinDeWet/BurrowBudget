using WebApi.Application.Features.AccountFeatures.UpdateAccount;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.AccountEndpoints.UpdateAccount;

public class UpdateAccountRequestValidator : Validator<UpdateAccountRequest>
{
    public UpdateAccountRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .StringInput(32);

        RuleFor(x => x.Description)
            .StringInput(256, false);

        RuleFor(x => x.AccountType)
            .IsInEnum();
    }
}
