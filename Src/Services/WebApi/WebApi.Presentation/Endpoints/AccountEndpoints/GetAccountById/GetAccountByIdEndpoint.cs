using CQRS.Contracts;
using WebApi.Application.Features.AccountFeatures.GetAccountById;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.AccountEndpoints.GetAccountById;

public class GetAccountByIdEndpoint(IQueryManager<GetAccountByIdRequest, GetAccountByIdResponse> manager)
    : QueryEndpoint<GetAccountByIdRequest, GetAccountByIdResponse>(manager)
{
    public override void Configure()
    {
        Get("account/{Id:guid}");
        Summary(x =>
        {
            x.Summary = "Get account by ID";
            x.Description = "Retrieves a single account by its ID";
        });
    }
}
