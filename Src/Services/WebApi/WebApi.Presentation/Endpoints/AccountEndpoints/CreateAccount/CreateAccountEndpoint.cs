using CQRS.Contracts;
using WebApi.Application.Features.AccountFeatures.Commands.CreateAccount;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.AccountEndpoints.CreateAccount;

public class CreateAccountEndpoint(ICommandManager<CreateAccountRequest, Guid> manager)
    : CommandEndpoint<CreateAccountRequest, Guid>(manager)
{
    public override void Configure()
    {
        Post("account");
        Summary(x =>
        {
            x.Summary = "Create a new account";
            x.Description = "Creates a new account with the specified details";
        });
    }
}
