using CQRS.Contracts;
using WebApi.Application.Features.AccountFeatures.DeleteAccount;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.AccountEndpoints.DeleteAccount;

public class DeleteAccountEndpoint(ICommandManager<DeleteAccountRequest> manager)
    : CommandEndpoint<DeleteAccountRequest>(manager)
{
    public override void Configure()
    {
        Delete("account/{Id:guid}");
        Summary(x =>
        {
            x.Summary = "Delete an account";
            x.Description = "Deletes an account by its ID";
        });
    }
}
