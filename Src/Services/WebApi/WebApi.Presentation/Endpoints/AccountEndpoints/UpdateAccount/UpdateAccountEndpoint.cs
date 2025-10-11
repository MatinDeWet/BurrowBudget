using CQRS.Contracts;
using WebApi.Application.Features.AccountFeatures.Queries.UpdateAccount;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.AccountEndpoints.UpdateAccount;

public class UpdateAccountEndpoint(ICommandManager<UpdateAccountRequest> manager)
    : CommandEndpoint<UpdateAccountRequest>(manager)
{
    public override void Configure()
    {
        Put("account/{Id:guid}");
        Summary(x =>
        {
            x.Summary = "Update an account";
            x.Description = "Updates an existing account";
        });
    }
}
