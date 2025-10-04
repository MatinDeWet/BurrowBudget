using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.AuthEndpoints.AuthLogin;

internal sealed class AuthLoginRequestValidator : Validator<AuthLoginRequest>
{
    public AuthLoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .StringInput(256)
            .EmailAddress()
            .WithMessage("{PropertyName} is not valid");

        RuleFor(x => x.Password)
            .StringInput(512);
    }
}
